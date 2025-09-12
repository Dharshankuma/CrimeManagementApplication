using System;
using System.Collections.Generic;

namespace CrimeManagement.Models;

public partial class ComplaintRequestBackup
{
    public int ComplaintRequestBackUpId { get; set; }

    public string? Identifier { get; set; }

    public string? ComplaintName { get; set; }

    public int? JurisdictionId { get; set; }

    public int? CrimeTypeId { get; set; }

    public string? CrimeDescription { get; set; }

    public string? PhoneNumber { get; set; }

    public int? EvidenceAttachmentId { get; set; }

    public int? IoofficerId { get; set; }

    public int? InvestigationId { get; set; }

    public int? StatusId { get; set; }

    public DateTime? DateReported { get; set; }

    public string? Priority { get; set; }

    public int? CreatedBy { get; set; }

    public DateTime? CreatedOn { get; set; }

    public int? ModifyBy { get; set; }

    public DateTime? ModifyOn { get; set; }
}
