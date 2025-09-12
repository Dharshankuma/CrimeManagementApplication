using System;
using System.Collections.Generic;

namespace CrimeManagement.Models;

public partial class ComplaintIoassign
{
    public int AssignId { get; set; }

    public int? ComplaintId { get; set; }

    public string? Identifier { get; set; }

    public int? UserId { get; set; }

    public int? ComplaintStatus { get; set; }

    public int? CreatedBy { get; set; }

    public DateTime? CreatedOn { get; set; }

    public int? ModifyBy { get; set; }

    public DateTime? ModifyOn { get; set; }
}
