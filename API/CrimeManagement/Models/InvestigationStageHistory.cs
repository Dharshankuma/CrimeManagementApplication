using System;
using System.Collections.Generic;

namespace CrimeManagement.Models;

public partial class InvestigationStageHistory
{
    public int StageHistoryId { get; set; }

    public Guid? Identifier { get; set; }

    public int InvestigationId { get; set; }

    public int? FromStatusId { get; set; }

    public int ToStatusId { get; set; }

    public int? ChangedBy { get; set; }

    public DateTime? ChangedOn { get; set; }

    public string? Remarks { get; set; }

    public bool? IsActive { get; set; }
}
