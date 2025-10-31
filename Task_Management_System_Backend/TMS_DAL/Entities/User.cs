using System;
using System.Collections.Generic;

namespace TMS_DAL.Entities;

public partial class User
{
    public string UserId { get; set; } = null!;

    public string Username { get; set; } = null!;

    public string PasswordHash { get; set; } = null!;

    public string FirstName { get; set; } = null!;

    public string? LastName { get; set; }

    public int StatusId { get; set; }

    public string CreatedBy { get; set; } = null!;

    public string? UpdatedBy { get; set; }

    public string? DeletedBy { get; set; }

    public DateTimeOffset CreatedDateTime { get; set; }

    public DateTimeOffset? UpdatedDateTime { get; set; }

    public DateTimeOffset? DeletedDateTime { get; set; }

    public virtual ICollection<Task> TaskCreatedByNavigations { get; set; } = new List<Task>();

    public virtual ICollection<Task> TaskDeletedByNavigations { get; set; } = new List<Task>();

    public virtual ICollection<TaskHasTaskStatus> TaskHasTaskStatusCreatedByNavigations { get; set; } = new List<TaskHasTaskStatus>();

    public virtual ICollection<TaskHasTaskStatus> TaskHasTaskStatusDeletedByNavigations { get; set; } = new List<TaskHasTaskStatus>();

    public virtual ICollection<TaskHasTaskStatus> TaskHasTaskStatusUpdatedByNavigations { get; set; } = new List<TaskHasTaskStatus>();

    public virtual ICollection<Task> TaskUpdatedByNavigations { get; set; } = new List<Task>();
}
