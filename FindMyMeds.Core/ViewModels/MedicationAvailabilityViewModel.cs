namespace FindMyMeds.Core.ViewModels
{
    public class MedicationAvailabilityViewModel
    {
        public int MedicationId { get; set; }
        public string MedicationName { get; set; } = null!;

        public int? PharmacyId { get; set; }
        public string PharmacyName { get; set; } = null!;

        public string? City { get; set; }

        public int Quantity { get; set; }
        public decimal? Price { get; set; }
    }
}