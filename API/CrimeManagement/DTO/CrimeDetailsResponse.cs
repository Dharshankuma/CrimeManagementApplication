namespace CrimeManagement.DTO
{
    public class CrimeDetailsResponse
    {
        public string? complaintIdentifier { get; set; }
        public string? complaintName { get; set; }
        public string? crimeType { get; set; }
        public string? jurisdiction { get; set; }
        public string? incidentDate { get; set; }
        public string? crimeDescription { get; set; }
        public InvestigationSetupDTO? investigation { get; set; }
        public WorkFlowControlDTO? workflow { get; set; }
        public List<CommentDTO>? comments { get; set; }
        public List<AuditTrailDTO> auditTrail { get; set; }
        public bool? isDisable { get; set; }
        public List<EvidenceGridData>? evidenceAttachments { get; set; }
    }

    public class InvestigationSetupDTO
    {
        public string? priority { get; set; }
        public string? assignedIo { get; set; }
        public string? startDate { get; set; }
        public string? investigationDescription { get; set; }
        public string? identifier { get; set; }
    }

    public class WorkFlowControlDTO
    {
        public string? CurrentStatus { get; set; }
        public List<StatusOptionDTO>? availableNextStatus { get; set; }
    }

    public class StatusOptionDTO
    {
        public string? statusIdentifier { get; set; }
        public string? statusName { get; set; }
        public int? statusId { get; set; }
    }

    public class CommentDTO
    {
        public string? commentText { get; set; }
        public string? commentBy { get; set; }
        public string? commentDate { get; set; }
    }

    public class AuditTrailDTO
    {
        public string? previousStatus { get; set; }
        public string? newStatus { get; set; }
        public string? changedBy { get; set; }
        public string? dateTime { get; set; }
    }
}
