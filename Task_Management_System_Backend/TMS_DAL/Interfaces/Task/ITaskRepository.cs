using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMS_DAL.Models.Common;
using TMS_DAL.Models.Task;

namespace TMS_DAL.Interfaces.Task
{
    public interface ITaskRepository
    {
        Task<ExecutionResponseModel> SaveTask(TaskModel model);
        Task<ExecutionResponseModel> DeleteTask(TaskModel model);
        Task<ExecutionResponseModel> ChangeTaskStatus(TaskModel model);
        Task<TaskModel>  GetTaskById(int taskId, string currentUserId);
        Task<TaskListModel> GetTaskList(TaskListFilterModel model );

    }
}
