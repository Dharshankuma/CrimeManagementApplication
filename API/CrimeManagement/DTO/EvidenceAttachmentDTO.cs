namespace CrimeManagement.DTO
{
    public class EvidenceAttachmentDTO
    {
        public int EvidenceAttachmentId { get; set; }

        public string? EvidenceAttachmentPath { get; set; }

        public string? Identifier { get; set; }

        public int? ComplaintId { get; set; }

        public int? CreatedBy { get; set; }

        public DateTime? CreatedOn { get; set; }

        public int? ModifyBy { get; set; }

        public DateTime? ModifyOn { get; set; }
    }


    public class EvidenceDownloadDTO
    {
        public string? fileName { get; set; }
        public string? contentType { get; set; }
        public string? base64Content { get; set; }
    }


    public class EvidenceGridData
    {
        public string? identifier { get; set; }
        public string? evidenceName { get; set; }
        public string? createdDate { get; set; }
    }
}
