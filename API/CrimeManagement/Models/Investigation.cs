using System;
using System.Collections.Generic;

namespace CrimeManagement.Models;

public partial class Investigation
{
    public int InvestigationId { get; set; }

    public string? Identifier { get; set; }

    public int? ComplaintId { get; set; }

    public int? IoOfficerId { get; set; }

    public DateTime? StartDate { get; set; }

    public DateTime? EndDate { get; set; }

    public string? Priority { get; set; }

    public int? StatusId { get; set; }

    public int? EvidenceAttachmentId { get; set; }

    public int? CrimeId { get; set; }

    public int? CriminalId { get; set; }

    public int? FirattachmentId { get; set; }

    public string? InvestigationDescription { get; set; }

    public int? CreatedBy { get; set; }

    public DateTime? CreatedOn { get; set; }

    public int? ModifyBy { get; set; }

    public DateTime? ModifyOn { get; set; }
}
