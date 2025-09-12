using System;
using System.Collections.Generic;

namespace CrimeManagement.Models;

public partial class Firattachment
{
    public int FirattachmentId { get; set; }

    public string? AttachmentPath { get; set; }

    public int? ComplaintId { get; set; }

    public int? CreatedBy { get; set; }

    public DateTime? CreatedOn { get; set; }

    public int? ModifyBy { get; set; }

    public DateTime? ModifyOn { get; set; }
}
