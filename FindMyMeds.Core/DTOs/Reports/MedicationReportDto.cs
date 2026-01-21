namespace FindMyMeds.Core.DTOs.Reports
{
    public class MedicationReportDto
    {
        public string MedicationName { get; set; } = "";
        public int TotalPharmacies { get; set; }
        public int AvailableCount { get; set; }
        public int OutOfStockCount { get; set; }
    }
}
