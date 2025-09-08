namespace Argo.VS.CustomersApi.IntegrationTests.Features.Customers.Queries;

using Testing;
using Testing.Builders;
using Testing.Fixtures;

using AwesomeAssertions;

using Xunit.Abstractions;

using Domain.CustomersAggregate;

public class GetCustomerListEndpointTests(
    DatabaseFixture database,
    ITestOutputHelper output) : IntegrationTestBase(database, output)
{
    [Fact]
    public async Task GetCustomerList_WhenCustomersExist_ShouldReturnPagedResults()
    {
        // Arrange
        var seeded = new List<Customer>();
        for (var i = 1; i <= 7; i++)
        {
            var c = new CustomerBuilder()
                .WithFirstName($"First{i}")
                .WithLastName($"Last{i}")
                .WithEmailAddress($"user{i}@example.com")
                .Build();

            seeded.Add(c);
        }

        await this.AddEntityRangeToDb(seeded);

        await using var factory = this.CreateWebAppFactory();
        var client = factory.CreateApiClient();

        var pageNumber = 2;
        var pageSize = 3;

        // Act
        var response = await client.GetCustomerListAsync(pageNumber, pageSize);

        // Assert: Response shape & metadata
        response.Should().NotBeNull();
        response.PageNumber.Should().Be(pageNumber);
        response.TotalCount.Should().Be(seeded.Count);

        response.Items.Should().NotBeNull();
        response.Items.Should().HaveCount(pageSize);

        // Assert: Items correspond to seeded entities
        var seededById = seeded.ToDictionary(x => x.Id);
        response.Items.Select(i => i.Id).Should().OnlyContain(id => seededById.ContainsKey(id));

        foreach (var item in response.Items)
        {
            var source = seededById[item.Id];
            item.FirstName.Should().Be(source.FirstName);
            item.LastName.Should().Be(source.LastName);
            item.EmailAddress.Should().Be(source.EmailAddress);
        }
    }

    [Fact]
    public async Task GetCustomerList_WhenNoCustomers_ShouldReturnEmptyList()
    {
        // Arrange
        await using var factory = this.CreateWebAppFactory();
        var client = factory.CreateApiClient();

        // Act
        var response = await client.GetCustomerListAsync(pageNumber: 1, pageSize: 10);

        // Assert
        response.Should().NotBeNull();
        response.PageNumber.Should().Be(1);
        response.TotalCount.Should().Be(0);
        response.Items.Should().NotBeNull().And.BeEmpty();
    }

    [Fact]
    public async Task GetCustomerList_WhenRequestingDifferentPages_ShouldReturnNonOverlappingItems()
    {
        // Arrange
        var seeded = new List<Customer>();
        for (var i = 1; i <= 8; i++)
        {
            var c = new CustomerBuilder()
                .WithFirstName($"User{i}")
                .WithLastName($"Test{i}")
                .WithEmailAddress($"u{i}@example.com")
                .Build();

            seeded.Add(c);
        }

        await this.AddEntityRangeToDb(seeded);

        await using var factory = this.CreateWebAppFactory();
        var client = factory.CreateApiClient();

        // Act
        var page1 = await client.GetCustomerListAsync(pageNumber: 1, pageSize: 3);
        var page2 = await client.GetCustomerListAsync(pageNumber: 2, pageSize: 3);

        // Assert
        page1.Items.Should().HaveCount(3);
        page2.Items.Should().HaveCount(3);

        var ids1 = page1.Items.Select(i => i.Id).ToList();
        var ids2 = page2.Items.Select(i => i.Id).ToList();

        ids1.Should().NotIntersectWith(ids2);
    }

    [Fact]
    public async Task GetCustomerList_WhenPageBeyondRange_ShouldReturnEmptyItems()
    {
        // Arrange
        var seeded = new List<Customer>();
        for (var i = 1; i <= 4; i++)
        {
            var c = new CustomerBuilder()
                .WithFirstName($"P{i}")
                .WithLastName($"Q{i}")
                .WithEmailAddress($"pq{i}@example.com")
                .Build();

            await this.AddEntityToDb(c);
            seeded.Add(c);
        }

        await using var factory = this.CreateWebAppFactory();
        var client = factory.CreateApiClient();

        // Act: page 3 of size 3 when only 4 items exist => empty
        var response = await client.GetCustomerListAsync(pageNumber: 3, pageSize: 3);

        // Assert
        response.Items.Should().NotBeNull().And.BeEmpty();
        response.TotalCount.Should().Be(seeded.Count);
        response.PageNumber.Should().Be(3);
    }
}
