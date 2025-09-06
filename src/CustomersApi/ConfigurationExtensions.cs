namespace Argo.VS.CustomersApi;

using Asp.Versioning;

using Domain.Common.Events;

using FluentValidation;

using Infrastructure.CQRS.Behaviors;
using Infrastructure.Persistence;
using Infrastructure.Persistence.Interceptors;
using Infrastructure.Web;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.OpenApi.Models;

using Serilog;
using Serilog.Events;

using Swashbuckle.AspNetCore.SwaggerGen;

public static class ConfigurationExtensions
{
    public static WebApplicationBuilder ConfigureApplicationBuilder(this WebApplicationBuilder builder)
    {
        var assembly = typeof(ApiRoot).Assembly;

        builder.AddCustomSerilog();

        builder.AddMinimalEndpoints(assemblies: assembly);

        builder.Services
            .AddHttpContextAccessor()
            .AddProblemDetails()
            .AddValidatorsFromAssembly(typeof(ApiRoot).Assembly)
            .AddPersistence(builder.Configuration)
            .AddCustomMediatr()
            .AddCustomApiVersioning()
            .AddCustomSwagger(o =>
            {
                o.SwaggerDoc("v1", new OpenApiInfo { Version = "1.0.0", Title = "Customers API V1" });
            });

        return builder;
    }

    public static WebApplication ConfigureApplication(this WebApplication app)
    {
        app.UseCustomProblemDetails();

        app.UseSerilogRequestLogging(cfg =>
        {
            cfg.GetLevel = GetCustomLogEventLevel;
        });

        app.UseHttpsRedirection();

        if (app.Environment.IsDevelopment())
        {
            app.UseCustomSwagger();
        }

        app.MapMinimalEndpoints();
        app.MapGet("/", x => x.Response.WriteAsync("Customers Api"));

        return app;
    }

    private static void AddCustomSerilog(this WebApplicationBuilder builder)
    {
        builder.Host.UseSerilog((ctx, services, cfg) =>
        {
            cfg.MinimumLevel.Is(LogEventLevel.Information)
                .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
                .MinimumLevel.Override("System", LogEventLevel.Warning)
                .Enrich.FromLogContext()
                .Enrich.WithProperty("Application", "Customers.Api")
                .Enrich.WithProperty("Environment", ctx.HostingEnvironment.EnvironmentName)
                .ReadFrom.Services(services);

            cfg.WriteTo.Console();
        });
    }

    private static IServiceCollection AddPersistence(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddScoped<IDomainEventPublisher, DomainEventPublisher>();
        services.AddScoped<ISaveChangesInterceptor, DomainEventDispatchingInterceptor>();

        services.AddDbContext<CustomerDbContext>((sp, options) =>
        {
            var connectionString = configuration.GetConnectionString("CustomerDb");
            ArgumentException.ThrowIfNullOrEmpty(connectionString);

            options.UseSqlServer(
                connectionString,
                b => b.MigrationsAssembly(typeof(CustomerDbContext).Assembly.FullName));

            options.AddInterceptors(sp.GetServices<ISaveChangesInterceptor>());
        });

        return services;
    }

    private static IServiceCollection AddCustomMediatr(this IServiceCollection services)
    {
        services.AddMediatR(cfg =>
        {
            cfg.RegisterServicesFromAssemblies(typeof(ApiRoot).Assembly);

            cfg.AddOpenBehavior(typeof(LoggingBehaviour<,>));
            cfg.AddOpenBehavior(typeof(ValidationBehavior<,>));
            cfg.AddOpenBehavior(typeof(TransactionBehavior<,>));
        });

        return services;
    }

    private static IServiceCollection AddCustomSwagger(
        this IServiceCollection services,
        Action<SwaggerGenOptions>? configureSwaggerGen = null)
    {
        services.AddEndpointsApiExplorer();

        services.AddSwaggerGen(
            options =>
            {
                options.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "Bearer" },
                            Scheme = "oauth2",
                            Name = "Bearer",
                            In = ParameterLocation.Header
                        },
                        new List<string>()
                    }
                });

                options.ResolveConflictingActions(apiDescriptions => apiDescriptions.First());

                options.SupportNonNullableReferenceTypes();
                options.NonNullableReferenceTypesAsRequired();
                options.UseAllOfToExtendReferenceSchemas();

                configureSwaggerGen?.Invoke(options);
            });

        services.Configure<SwaggerGeneratorOptions>(o => o.InferSecuritySchemes = true);

        return services;
    }

    private static void UseCustomSwagger(this WebApplication app)
    {
        app.UseSwagger();

        app.UseSwaggerUI(
            options =>
            {
                var descriptions = app.DescribeApiVersions();

                // build a swagger endpoint for each discovered API version
                foreach (var description in descriptions)
                {
                    var url = $"/swagger/{description.GroupName}/swagger.json";
                    var name = description.GroupName.ToUpperInvariant();
                    options.SwaggerEndpoint(url, name);
                }
            });
    }

    private static IServiceCollection AddCustomApiVersioning(this IServiceCollection services)
    {
        services.AddApiVersioning(o =>
            {
                o.ReportApiVersions = true;
                o.ApiVersionReader = new UrlSegmentApiVersionReader();
            })
            .AddApiExplorer(o =>
            {
                // Use short group names like v1, v2 (matches your Swagger doc keys)
                o.GroupNameFormat = "'v'V";
                o.SubstituteApiVersionInUrl = true;
            });

        return services;
    }

    private static LogEventLevel GetCustomLogEventLevel(HttpContext ctx, double timeout, Exception? ex)
    {
        if (ex != null)
        {
            return LogEventLevel.Error;
        }

        if (IsSwaggerEndpoint(ctx))
        {
            return LogEventLevel.Verbose;
        }

        if (ctx.Response.StatusCode > 499)
        {
            return LogEventLevel.Error;
        }

        return LogEventLevel.Information;
    }

    private static bool IsSwaggerEndpoint(HttpContext ctx)
    {
        var isSwagger = ctx.Request.Path.StartsWithSegments(new PathString("/swagger"), StringComparison.OrdinalIgnoreCase);

        return isSwagger;
    }
}
