using System;
using System.Collections.Generic;

namespace CrimeManagement.Models;

public partial class StatusTransitionRule
{
    public int StatusTransitionRuleId { get; set; }

    public int FromStatusId { get; set; }

    public int ToStatusId { get; set; }

    public int AllowedRoleId { get; set; }

    public bool IsActive { get; set; }

    public int? CreatedBy { get; set; }

    public DateTime CreatedOn { get; set; }
}
