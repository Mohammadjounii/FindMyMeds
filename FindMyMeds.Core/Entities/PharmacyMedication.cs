using System.ComponentModel.DataAnnotations;

namespace FindMyMeds.Core.Entities
{
    public class PharmacyMedication
    {
        public int Id { get; set; }

        [Required]
        public int PharmacyId { get; set; }
        public Pharmacy? Pharmacy { get; set; }

        [Required(ErrorMessage = "Please select a medication")]
        public int MedicationId { get; set; }
        public Medication? Medication { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "Quantity must be greater than 0")]
        public int Quantity { get; set; }

        public DateTime? ExpiryDate { get; set; }

        [Range(0.01, double.MaxValue, ErrorMessage = "Price must be greater than 0")]
        public decimal? Price { get; set; }
        public DateTime LastUpdatedAt { get; set; } = DateTime.UtcNow;  
       
    }
}
