using System;
using System.Collections.Generic;

namespace CrimeManagement.Models;

public partial class CrimeCriminal
{
    public int CrimeCriminalId { get; set; }

    public int? CrimeId { get; set; }

    public int? CriminalId { get; set; }

    public string? Identifier { get; set; }

    public int? CreatedBy { get; set; }

    public DateTime? CreatedOn { get; set; }

    public int? ModifyBy { get; set; }

    public DateTime? ModifyOn { get; set; }

    public string? EvidenceAttachment { get; set; }
}
