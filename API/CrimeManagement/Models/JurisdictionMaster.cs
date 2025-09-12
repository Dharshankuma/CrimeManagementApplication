using System;
using System.Collections.Generic;

namespace CrimeManagement.Models;

public partial class JurisdictionMaster
{
    public int JurisdictionId { get; set; }

    public string? JurisdictionName { get; set; }

    public string? Identifier { get; set; }

    public int? LocationId { get; set; }

    public int? CreatedBy { get; set; }

    public DateTime? CreatedOn { get; set; }

    public int? ModifyBy { get; set; }

    public DateTime? ModifyOn { get; set; }
}
