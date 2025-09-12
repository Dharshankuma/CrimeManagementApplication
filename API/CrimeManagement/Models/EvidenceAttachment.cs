using System;
using System.Collections.Generic;

namespace CrimeManagement.Models;

public partial class EvidenceAttachment
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
