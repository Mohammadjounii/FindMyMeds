using FindMyMeds.Infrastructure.Data;
using FindMyMeds.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace FindMyMeds.Services.Background
{
    public class LowStockNotificationService : BackgroundService
    {
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly ILogger<LowStockNotificationService> _logger;

        private const int LowStockThreshold = 5;

        public LowStockNotificationService(
            IServiceScopeFactory scopeFactory,
            ILogger<LowStockNotificationService> logger)
        {
            _scopeFactory = scopeFactory;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("LowStockNotificationService started");

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    using var scope = _scopeFactory.CreateScope();
                    var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

                    var items = await context.PharmacyMedications
                        .Include(pm => pm.Pharmacy)
                        .Include(pm => pm.Medication)
                        .ToListAsync(stoppingToken);

                    foreach (var pm in items)
                    {
                        var ownerId = pm.Pharmacy.OwnerUserId;
                        var medicationId = pm.MedicationId;

                        var existingNotification = await context.Notifications
                            .FirstOrDefaultAsync(n =>
                                n.UserId == ownerId &&
                                n.MedicationId == medicationId,
                                stoppingToken);

                        if (pm.Quantity <= LowStockThreshold)
                        {
                            if (existingNotification == null)
                            {
                                context.Notifications.Add(new Notification
                                {
                                    UserId = ownerId,
                                    MedicationId = medicationId,
                                    Message = $"Low stock: {pm.Medication.Name} (Qty: {pm.Quantity})",
                                    IsRead = false
                                });
                            }
                            else
                            {
                                existingNotification.Message =
                                    $"Low stock: {pm.Medication.Name} (Qty: {pm.Quantity})";
                            }
                        }
                        else
                        {
                            if (existingNotification != null)
                            {
                                context.Notifications.Remove(existingNotification);
                            }
                        }
                    }

                    await context.SaveChangesAsync(stoppingToken);

                    _logger.LogInformation("Low stock check finished at {Time}", DateTime.Now);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error in LowStockNotificationService");
                }

                await Task.Delay(TimeSpan.FromSeconds(10), stoppingToken);
            }
        }
    }
}
