using Microsoft.Identity.Client;

namespace CrimeManagement.DTO
{
    
    public class InvestigationDTO
    {
        public string? crimeType { get; set; }

        public string? complaintName { get; set; }

        public string? complaintDescription { get; set; }
        
        public string? complaintDateReported { get; set; }

        public string? statusName { get; set; }

        public string? complaintRaiseName { get; set; }

        public string? location { get; set; }

        public string? userIdentifier { get; set; }

        public string? Identifier { get; set; }

        public int? ComplaintId { get; set; }

        public string? complaintIdentifier { get; set; }

        public int? IoOfficerId { get; set; }

        public string? ioOfficerName { get; set; }

        public string? ioOfficerIdentifier { get; set; }

        public DateTime? StartDate { get; set; }

        public string? startDateString { get; set; }

        public DateTime? EndDate { get; set; }

        public string? endDateString { get; set; }

        public string? Priority { get; set; }

        public int? StatusId { get; set; }

        public string? statusIdentifier { get; set; }

        public int? EvidenceAttachmentId { get; set; }

        public int? CrimeId { get; set; }

        public string? crimeIdentifier { get; set; }

        public int? CriminalId { get; set; }

        public string? criminalIdentifier { get; set; }

        public int? FirattachmentId { get; set; }

        public string? InvestigationDescription { get; set; }

        public int? CreatedBy { get; set; }

        public DateTime? CreatedOn { get; set; }

        public int? ModifyBy { get; set; }

        public DateTime? ModifyOn { get; set; }
    }

    public class InvestigationViewDTO
    {
        public string? investigationIdentifer { get; set; }
        public string? investigationId { get; set; }
        public string? complaintName { get; set; }
        public string? crimeType { get; set; }
        public string? crimeStatus { get; set; }
        public string? lastUpdatedDate { get; set; }
        public string? Location { get; set; }
        public int? statusId { get; set; }
    }

    public class InvestigationRequestviewDTO
    {
        public string? columnName { get; set; }
        public bool? sortOrder { get; set; }
        public string? userIdentifier { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }  
    }
}
