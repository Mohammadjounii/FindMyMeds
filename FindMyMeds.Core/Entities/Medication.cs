using System.ComponentModel.DataAnnotations;

namespace FindMyMeds.Core.Entities
{
    public class Medication
    {
        public int Id { get; set; }
        [Required]
        [MaxLength(200)]
        public string Name { get; set; } = null!;
        public string? Description { get; set; }
        public ICollection<MedicationSubstitute> AsPrimarySubstitutes { get; set; } = new List<MedicationSubstitute>();
        public ICollection<MedicationSubstitute> AsAlternativeSubstitutes { get; set; } = new List<MedicationSubstitute>();
    }
}

