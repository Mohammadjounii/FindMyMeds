using System;

namespace FindMyMeds.Core.ViewModels
{
    public class PharmacyMedicationViewModel
    {
        public int Id { get; set; }

        public int PharmacyId { get; set; }
        public string PharmacyName { get; set; } = null!;

        public int MedicationId { get; set; }
        public string MedicationName { get; set; } = null!;

        public int Quantity { get; set; }

        public decimal? Price { get; set; }

        public DateTime? ExpiryDate { get; set; }
        public bool IsExpired =>
            ExpiryDate.HasValue &&
            ExpiryDate.Value.Date < DateTime.UtcNow.Date;
    }
}
