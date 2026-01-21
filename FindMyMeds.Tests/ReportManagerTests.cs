using FindMyMeds.Infrastructure.Data;
using FindMyMeds.Services.Managers;
using Microsoft.EntityFrameworkCore;
using FluentAssertions;
using Xunit;

public class ReportManagerTests
{
    private ApplicationDbContext CreateDbContext()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        return new ApplicationDbContext(options);
    }

    [Fact]
    public async Task GetDashboardStatsAsync_WhenNoData_ReturnsZeros()
    {
        // Arrange
        var context = CreateDbContext();
        var manager = new ReportManager(context);

        // Act
        var stats = await manager.GetDashboardStatsAsync();

        // Assert
        stats.TotalPharmacies.Should().Be(0);
        stats.TotalMedications.Should().Be(0);
        stats.AvailableMedications.Should().Be(0);
        stats.OutOfStockMedications.Should().Be(0);
    }

    [Fact]
    public async Task GetPharmacyActivityReportAsync_WhenNoData_ReturnsEmptyList()
    {
        var context = CreateDbContext();
        var manager = new ReportManager(context);

        var result = await manager.GetPharmacyActivityReportAsync();

        result.Should().BeEmpty();
    }
}
