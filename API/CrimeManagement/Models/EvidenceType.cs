using System;
using System.Collections.Generic;

namespace CrimeManagement.Models;

public partial class EvidenceType
{
    public int EvidenceTypeId { get; set; }

    public string? EvidenceName { get; set; }

    public string? Identifier { get; set; }

    public int? CreatedBy { get; set; }

    public DateTime? CreatedOn { get; set; }

    public int? ModifyBy { get; set; }

    public DateTime? ModifyOn { get; set; }
}
