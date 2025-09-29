namespace CrimeManagement.DTO
{
    public class CrimeReportDTO
    {
        public string? userIdentifier { get; set; }

        public string? Identifier { get; set; }

        public string? ComplaintName { get; set; }

        public int? JurisdictionId { get; set; }

        public string? jurisdictionIdentifier { get; set; }

        public string? jurisdictionName { get; set; }
        
        public string? ioOfficerName { get; set; }

        public string? startDateString { get; set; }

        public string? endDateString { get; set; }

        public string? investigationDescription { get; set; }

        public string? victimName { get; set; }

        public int? CrimeTypeId { get; set; }

        public string? crimeTypeIdentifier { get; set; }

        public string? crimeTypeName { get; set; }

        public string? statusName { get; set; }

        public string? CrimeDescription { get; set; }

        public string? PhoneNumber { get; set; }

        public int? EvidenceAttachmentId { get; set; }

        public DateTime? DateReported { get; set; }

        public string? dateReportString { get; set; }
    }

    public class CrimeReportViewDTO
    {
        public string? reportIdentifer { get; set; }
        public string? reportId { get; set; }
        public string? complaintName { get; set; }
        public string? jurisdictionName { get; set; }
        public string? crimeReportDate { get; set; }
        public string? crimeType { get; set; }
        public string? crimeStatus { get; set; }
        public string? crimeStatusStr { get; set; }
    }

    public class CrimeRequestviewDTO
    {
        public string? crimeType { get; set; }
        public string? reportDate { get; set; }
        public string? columnName { get; set; }
        public bool? sortOrder { get; set; }
        public string? crimeIdentifier { get; set; }
        public string? userIdentifier { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
    }

}
