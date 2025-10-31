using System;
using System.Collections.Generic;

namespace TMS_DAL.Entities;

public partial class TaskHasTaskStatus
{
    public int TaskHasTaskStatusId { get; set; }

    public int TaskId { get; set; }

    public int TaskStatusId { get; set; }

    public DateTimeOffset CreatedDateTime { get; set; }

    public DateTimeOffset? UpdatedDateTime { get; set; }

    public DateTimeOffset? DeletedDateTime { get; set; }

    public string CreatedBy { get; set; } = null!;

    public string? UpdatedBy { get; set; }

    public string? DeletedBy { get; set; }

    public virtual User CreatedByNavigation { get; set; } = null!;

    public virtual User? DeletedByNavigation { get; set; }

    public virtual Task Task { get; set; } = null!;

    public virtual User? UpdatedByNavigation { get; set; }
}
