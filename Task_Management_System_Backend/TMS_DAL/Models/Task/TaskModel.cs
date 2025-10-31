using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMS_DAL.Models.Common;

namespace TMS_DAL.Models.Task
{
    public class TaskModel:BaseModel
    {
        public int? TaskId { get; set; }
        public string? Title { get; set; }
        public string? Description { get; set; }
        public DateTimeOffset? DueDateTime { get; set; }
        public int? CurrentTaskStatusId { get; set; }
        public string? CurrentTaskStatus { get; set; }
        public string? NextTaskStatus { get; set; }
        public string? AssignedToUserId { get; set; }
        public List<TaskHasStatusModel> TaskStatusChangeList { get; set; } = new List<TaskHasStatusModel>();
    }

    public class TaskListFilterModel:ListFilterBaseModel
    {
        public string? Title { get; set; }
        public string? Description { get; set; }
        public DateTimeOffset? FromDueDateTime { get; set; }
        public DateTimeOffset? ToDueDateTime { get; set; }
        public DateTimeOffset? FromCreatedDateTime { get; set; }
        public DateTimeOffset? ToCreatedDateTime { get; set; }
        public int? CurrentTaskStatusId { get; set; }

    }

    public class TaskListModel
    {
        public List<TaskListDetailModel> TaskList { get; set; } = new List<TaskListDetailModel>();
        public int? TotalRecords { get; set; }

    }

    public class TaskListDetailModel
    {
        public int? TaskId { get; set; }
        public string? Title { get; set; }
        public string? Description { get; set; }
        public DateTimeOffset? DueDateTime { get; set; }
        public string? CurrentTaskStatus { get; set; }
        public int? CurrentTaskStatusId { get; set; }
        public DateTimeOffset? CreatedDateTime { get; set; }
        public DateTimeOffset? UpdatedDateTime { get; set; }
    }
}
