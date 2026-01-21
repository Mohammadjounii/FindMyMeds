namespace FindMyMeds.Core.DTOs
{
    public class MedicationDetailsDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public string? Description { get; set; }
    }
}
