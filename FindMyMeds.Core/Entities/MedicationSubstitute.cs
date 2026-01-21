using System.ComponentModel.DataAnnotations;

namespace FindMyMeds.Core.Entities
{
    public class MedicationSubstitute
    {
        public int Id { get; set; }

        [Required]
        public int PrimaryMedicationId { get; set; }
        public Medication PrimaryMedication { get; set; } = null!;

        [Required]
        public int AlternativeMedicationId { get; set; }
        public Medication AlternativeMedication { get; set; } = null!;

        public string? Notes { get; set; }
    }
}
