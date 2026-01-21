using FindMyMeds.Core.Identity;
using System.ComponentModel.DataAnnotations;

namespace FindMyMeds.Core.Entities
{
    public class Pharmacy
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(200)]
        public string Name { get; set; } = null!;
        public string? City { get; set; }
        public string? Phone { get; set; }
        public ApplicationUser? OwnerUser { get; set; }
        public string? OwnerUserId { get; set; }
        public bool IsApproved { get; set; } = false;
        public ICollection<PharmacyMedication> PharmacyMedications { get; set; } = new List<PharmacyMedication>();
        public string? LogoPath { get; set; }
    }
}

