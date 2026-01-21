namespace FindMyMeds.Services.Interfaces
{
    public interface IMedicationNotifier
    {
        Task MedicationUpdatedAsync(int medicationId);
     
    }
}
    