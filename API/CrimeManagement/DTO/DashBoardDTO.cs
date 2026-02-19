namespace CrimeManagement.DTO
{
    public class DashBoardDTO
    {
        public int? totalComplaints { get; set; }
        public int? underInvestigation { get; set; }
        public int? evidenceCollected { get; set; }
        public int? resolvedCases { get; set; }
        public double? overAllStatus { get; set; }
        public List<DashBoardCrimeReportDTO>? recentsComplaints { get; set; }

    }

    public class DashBoardCrimeReportDTO
    {
        public string? crimeId { get; set; }
        public string? crimeType { get; set; }
        public string? ioOfficerName { get; set; }
        public int? officerId { get; set; }
        public DateTime? modifyOn { get; set; }
        public string? lastUpdated { get; set; }
        public int? createdBy { get; set; }
    }
}
