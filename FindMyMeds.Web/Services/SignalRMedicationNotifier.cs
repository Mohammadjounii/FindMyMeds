using FindMyMeds.Services.Interfaces;
using FindMyMeds.Web.Hubs;
using Microsoft.AspNetCore.SignalR;

namespace FindMyMeds.Web.Services
{
    public class SignalRMedicationNotifier : IMedicationNotifier
    {
        private readonly IHubContext<MedicationHub> _hub;

        public SignalRMedicationNotifier(IHubContext<MedicationHub> hub)
        {
            _hub = hub;
        }

        public async Task MedicationUpdatedAsync(int medicationId)
        {
            await _hub.Clients.All.SendAsync(
                "MedicationUpdated",
                medicationId
            );
        }
    
    }
}
