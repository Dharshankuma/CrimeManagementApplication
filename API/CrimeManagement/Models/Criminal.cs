using System;
using System.Collections.Generic;

namespace CrimeManagement.Models;

public partial class Criminal
{
    public int CriminalId { get; set; }

    public string? CriminalName { get; set; }

    public int? CrimeTypeId { get; set; }

    public int? JurisdictionId { get; set; }

    public string? Identifier { get; set; }

    public int? CreatedBy { get; set; }

    public DateTime? CreatedOn { get; set; }

    public int? ModifyBy { get; set; }

    public DateTime? ModifyOn { get; set; }
}
