using System;
using System.Collections.Generic;

namespace CrimeManagement.Models;

public partial class IojurisdictionAssign
{
    public int IojurisdictionId { get; set; }

    public string? Identifier { get; set; }

    public int? UserId { get; set; }

    public int? JurisdictionId { get; set; }

    public int? CreatedBy { get; set; }

    public DateTime? CreatedOn { get; set; }

    public int? ModifyBy { get; set; }

    public DateTime? ModifyOn { get; set; }
}
