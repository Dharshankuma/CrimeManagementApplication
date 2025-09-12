using System;
using System.Collections.Generic;

namespace CrimeManagement.Models;

public partial class LocationMaster
{
    public int LocationId { get; set; }

    public string? LocationName { get; set; }

    public string? Identifier { get; set; }

    public int? CreatedBy { get; set; }

    public DateTime? CreatedOn { get; set; }

    public int? ModifyBy { get; set; }

    public DateTime? ModifyOn { get; set; }

    public int? StateId { get; set; }
}
