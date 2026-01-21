namespace FindMyMeds.Core.DTOs
{
    public class MedicationAvailabilityDto
    {

        public string MedicationName { get; set; } = "";
        public string PharmacyName { get; set; } = "";
        public string City { get; set; } = "";
        public int Quantity { get; set; }
        public decimal? Price { get; set; }
        public int? PharmacyId { get; set; }
        public int MedicationId { get; set; }
    }
}
