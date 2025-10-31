using System;
using System.Collections.Generic;

namespace TMS_DAL.Entities;

public partial class Task
{
    public int TaskId { get; set; }

    public string Title { get; set; } = null!;

    public string? Description { get; set; }

    public DateTimeOffset DueDateTime { get; set; }

    public int CurrentTaskStatusId { get; set; }

    public string AssignedTo { get; set; } = null!;

    public int EntryVersion { get; set; }

    public string EntryIdentifier { get; set; } = null!;

    public int StatusId { get; set; }

    public string CreatedBy { get; set; } = null!;

    public string? UpdatedBy { get; set; }

    public string? DeletedBy { get; set; }

    public DateTimeOffset CreatedDateTime { get; set; }

    public DateTimeOffset? UpdatedDateTime { get; set; }

    public DateTimeOffset? DeletedDateTime { get; set; }

    public virtual User CreatedByNavigation { get; set; } = null!;

    public virtual User? DeletedByNavigation { get; set; }

    public virtual ICollection<TaskHasTaskStatus> TaskHasTaskStatuses { get; set; } = new List<TaskHasTaskStatus>();

    public virtual User? UpdatedByNavigation { get; set; }
}
