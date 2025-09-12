using System;
using System.Collections.Generic;

namespace CrimeManagement.Models;

public partial class InvestigationStageHistory
{
    public int StageId { get; set; }

    public string? Identifier { get; set; }

    public string? InvestigationStageHistory1 { get; set; }

    public DateTime? StartDate { get; set; }

    public DateTime? EndDate { get; set; }

    public int? CreatedBy { get; set; }

    public DateTime? CreatedOn { get; set; }

    public int? ModifyBy { get; set; }

    public DateTime? ModifyOn { get; set; }
}
