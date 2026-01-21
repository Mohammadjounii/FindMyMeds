using FindMyMeds.Infrastructure.Data;
using FindMyMeds.Services.Managers;
using Microsoft.EntityFrameworkCore;
using FluentAssertions;
using Xunit;

public class MedicationManagerTests
{
    private ApplicationDbContext CreateDbContext()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        return new ApplicationDbContext(options);
    }

    [Fact]
    public async Task SearchAsync_WhenNoData_ReturnsEmptyList()
    {
        var context = CreateDbContext();
        var manager = new MedicationManager(null!, context);

        var result = await manager.SearchAsync("pa");

        result.Should().BeEmpty();
    }

    [Fact]
    public async Task SearchAsync_QueryTooShort_ReturnsEmpty()
    {
        var context = CreateDbContext();
        var manager = new MedicationManager(null!, context);

        var result = await manager.SearchAsync("p");

        result.Should().BeEmpty();
    }

}
