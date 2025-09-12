using System;
using System.Collections.Generic;

namespace CrimeManagement.Models;

public partial class StateMaster
{
    public int StateId { get; set; }

    public string? StateName { get; set; }

    public string? Identifier { get; set; }

    public int? CreatedBy { get; set; }

    public DateTime? CreatedOn { get; set; }

    public int? ModifyBy { get; set; }

    public DateTime? ModifyOn { get; set; }

    public int? CountryId { get; set; }
}
