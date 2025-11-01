using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMS_DAL.Common;
using TMS_DAL.Entities;
using TMS_DAL.Interfaces.Task;
using TMS_DAL.Models.Common;
using TMS_DAL.Models.Task;

namespace TMS_DAL.Implementations.Task
{
    public class TaskRepository : ITaskRepository
    {
        private readonly DbTaskManagementSystemContext _dbContext;

        public TaskRepository(DbTaskManagementSystemContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<ExecutionResponseModel> ChangeTaskStatus(TaskModel model)
        {
            try
            {
                var task = await _dbContext.Tasks.
                    Where(t => t.TaskId == model.TaskId && t.StatusId != (int)StatusEnum.Deleted && t.AssignedTo.Equals(model.CurrentUserId))
                    .FirstOrDefaultAsync();

                if (task == null)
                {
                    return new ExecutionResponseModel()
                    {
                        ExecutionResultId = (int)ExecutionResultEnum.ResourceNotFound,
                        ResponseText = "The task, you are trying to change the status is not found."
                    };
                }

                if (task.EntryVersion != model.EntryVersion)
                {
                    return new ExecutionResponseModel()
                    {
                        ExecutionResultId = (int)ExecutionResultEnum.VersionMismatch,
                        ResponseText = "The task, you are trying to change update has a newer version. Please reload the Task before proceeding."
                    };
                }

                if (task.CurrentTaskStatusId == (int)TaskStatusEnum.Completed)
                {
                    return new ExecutionResponseModel()
                    {
                        ExecutionResultId = (int)ExecutionResultEnum.VersionMismatch,
                        ResponseText = $"The task, you are trying to update is in the {TaskStatusEnum.Completed.ToString()}."
                    };
                }

                var nextStatusId = task.CurrentTaskStatusId == (int)TaskStatusEnum.Pending ? (int)TaskStatusEnum.Started : (int)TaskStatusEnum.Completed;

                task.TaskHasTaskStatuses.Add(new TaskHasTaskStatus()
                {
                    TaskStatusId = nextStatusId,
                    CreatedBy = model.CurrentUserId,
                    CreatedDateTime = DateTimeOffset.UtcNow,
                });

                task.CurrentTaskStatusId = nextStatusId;
                task.UpdatedDateTime = DateTimeOffset.UtcNow;
                task.UpdatedBy = model.CurrentUserId;
                task.EntryVersion++;

                await _dbContext.SaveChangesAsync();

                return new ExecutionResponseModel()
                {
                    ExecutionResultId = (int)ExecutionResultEnum.Success,
                    ResponseText = "The task's status has been updated successfully."
                };
            }
            catch (Exception ex)
            {
                return new ExecutionResponseModel()
                {
                    ExecutionResultId = (int)ExecutionResultEnum.InternalServerError,
                    ResponseText = "An error has occured while processing your request. Please contact your administrator if this repeatedly occurs."
                };
            }
        }

        public async Task<ExecutionResponseModel> DeleteTask(TaskModel model)
        {
            try
            {
                var task = await _dbContext.Tasks.
                    Where(t => t.TaskId == model.TaskId && t.StatusId != (int)StatusEnum.Deleted && t.AssignedTo.Equals(model.CurrentUserId))
                    .FirstOrDefaultAsync();

                if (task == null)
                {
                    return new ExecutionResponseModel()
                    {
                        ExecutionResultId = (int)ExecutionResultEnum.ResourceNotFound,
                        ResponseText = "The task, you are trying to delete is not found."
                    };
                }

                if (task.EntryVersion != model.EntryVersion)
                {
                    return new ExecutionResponseModel()
                    {
                        ExecutionResultId = (int)ExecutionResultEnum.VersionMismatch,
                        ResponseText = "The task, you are trying to delete has a newer version. Please reload the Task before proceeding."
                    };
                }

                task.StatusId = (int)StatusEnum.Deleted;
                task.DeletedDateTime = DateTimeOffset.UtcNow;
                task.DeletedBy = model.CurrentUserId;

                await _dbContext.SaveChangesAsync();

                return new ExecutionResponseModel()
                {
                    ExecutionResultId = (int)ExecutionResultEnum.Success,
                    ResponseText = "The task has been deleted successfully."
                };
            }
            catch(Exception ex)
            {
                return new ExecutionResponseModel()
                {
                    ExecutionResultId = (int)ExecutionResultEnum.InternalServerError,
                    ResponseText = "An error has occured while processing your request. Please contact your administrator if this repeatedly occurs."
                };
            }
            
        }

        public async Task<TaskModel> GetTaskById(int taskId, string currentUserId)
        {
            try
            {
                var task = await _dbContext.Tasks.
                    Where(t => t.TaskId == taskId && t.StatusId != (int)StatusEnum.Deleted && t.AssignedTo.Equals(currentUserId))
                    .Include(t=> t.TaskHasTaskStatuses)
                    .FirstOrDefaultAsync();

                if(task == null)
                {
                    return new TaskModel();
                }

                return new TaskModel()
                {
                    TaskId = task.TaskId,
                    Title = task.Title,
                    Description = task.Description,
                    EntryVersion = task.EntryVersion,
                    CurrentTaskStatusId = task.CurrentTaskStatusId,
                    CurrentTaskStatus = Functions.GetTaskStatusString(task.CurrentTaskStatusId),
                    NextTaskStatus = Functions.GetTaskStatusString(task.CurrentTaskStatusId + 1),
                    CreatedDateTime = task.CreatedDateTime,
                    UpdatedDateTime = task.UpdatedDateTime,
                    DueDateTime = task.DueDateTime,
                    TaskStatusChangeList = task.TaskHasTaskStatuses
                    .Select(s => new TaskHasStatusModel()
                    {
                        TaskStatus = Functions.GetTaskStatusString(s.TaskStatusId),
                        TimeStamp = s.CreatedDateTime,
                    })
                    .OrderByDescending(x=> x.TimeStamp)
                    .ToList()
                };
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public async Task<TaskListModel> GetTaskList(TaskListFilterModel model)
        {
            var taskListModel = new TaskListModel();

            try
            {
                var queryFilter = _dbContext.Tasks.Where(x => x.StatusId != (int)StatusEnum.Deleted && x.AssignedTo.Equals(model.CurrentUserId));

                if (!string.IsNullOrEmpty(model.Title))
                {
                    queryFilter = queryFilter.Where(x => x.Title.Contains(model.Title));
                }

                if (!string.IsNullOrEmpty(model.Description))
                {
                    queryFilter = queryFilter.Where(x => x.Description.Contains(model.Description));
                }

                if (model.FromCreatedDateTime != null)
                {
                    queryFilter = queryFilter.Where(x => x.CreatedDateTime >= model.FromCreatedDateTime);
                }

                if (model.ToCreatedDateTime != null)
                {
                    queryFilter = queryFilter.Where(x => x.CreatedDateTime <= model.ToCreatedDateTime);
                }

                if (model.FromDueDateTime != null)
                {
                    queryFilter = queryFilter.Where(x => x.DueDateTime >= model.FromDueDateTime);
                }

                if (model.ToDueDateTime != null)
                {
                    queryFilter = queryFilter.Where(x => x.DueDateTime <= model.ToDueDateTime);
                }

                if (model.CurrentTaskStatusId != null)
                {
                    queryFilter = queryFilter.Where(x => x.CurrentTaskStatusId == model.CurrentTaskStatusId);
                }

                taskListModel.TaskList = await queryFilter
                    .OrderBy(x => x.DueDateTime)
                    .Skip((int)model.Start)
                    .Take((int)model.Length)
                    .Select(t => new TaskListDetailModel()
                    {
                        TaskId = t.TaskId,
                        Title = t.Title,
                        Description = (t.Description != null ? (t.Description.Length > 15 ? t.Description.Substring(0, 20) + " ..." : t.Description) : "N/A"),
                        DueDateTime = t.DueDateTime,
                        CurrentTaskStatus = Functions.GetTaskStatusString(t.CurrentTaskStatusId),
                        CurrentTaskStatusId = t.CurrentTaskStatusId,
                        CreatedDateTime = t.CreatedDateTime,
                        UpdatedDateTime = t.UpdatedDateTime
                    })
                    .ToListAsync();

                taskListModel.TotalRecords = await queryFilter.CountAsync();

                return taskListModel;
            }
            catch(Exception ex)
            {
                return null;
            }
        }

        public async Task<ExecutionResponseModel> SaveTask(TaskModel model)
        {
            try
            {
                if(model.TaskId == null)
                {
                    var existingTaskEntry = await _dbContext.Tasks.Where(t => t.EntryIdentifier.Equals(model.EntryIdentifier)).FirstOrDefaultAsync();

                    if(existingTaskEntry != null)
                    {
                        return new ExecutionResponseModel()
                        {
                            ExecutionResultId = (int)ExecutionResultEnum.RecordExist,
                            ResponseText = "The Task, you are trying to create is already created before."
                        };
                    }

                    var taskStatusChangeList = new List<TaskHasTaskStatus>()
                    {
                        new TaskHasTaskStatus()
                        {
                            TaskStatusId = (int)TaskStatusEnum.Pending,
                            CreatedBy = model.CurrentUserId,
                            CreatedDateTime = DateTimeOffset.UtcNow
                        }
                    };


                    var task = new Entities.Task()
                    {
                        Title = model.Title,
                        Description = model.Description,
                        DueDateTime = (DateTimeOffset)model.DueDateTime,
                        AssignedTo = model.CurrentUserId,
                        CurrentTaskStatusId = (int)TaskStatusEnum.Pending,
                        EntryVersion = 1,
                        EntryIdentifier = model.EntryIdentifier,
                        StatusId = (int)StatusEnum.Active,
                        CreatedDateTime = DateTimeOffset.UtcNow,
                        CreatedBy = model.CurrentUserId,
                        TaskHasTaskStatuses = taskStatusChangeList

                    };

                    await _dbContext.AddAsync(task);
                    await _dbContext.SaveChangesAsync();

                    return new ExecutionResponseModel()
                    {
                        ExecutionResultId = (int)ExecutionResultEnum.Success,
                        ResponseText = "The Task created successfully."
                    };
                }
                else
                {
                    var existingTask = await _dbContext.Tasks
                        .Where(t => t.TaskId == model.TaskId && t.StatusId != (int)StatusEnum.Deleted)
                        .FirstOrDefaultAsync();
                    
                    if (existingTask == null)
                    {
                        return new ExecutionResponseModel()
                        {
                            ExecutionResultId = (int)ExecutionResultEnum.ResourceNotFound,
                            ResponseText = "The task, you are trying to update is not found."
                        };
                    }

                    if (existingTask.EntryVersion != model.EntryVersion)
                    {
                        return new ExecutionResponseModel()
                        {
                            ExecutionResultId = (int)ExecutionResultEnum.VersionMismatch,
                            ResponseText = "The task, you are trying to update has a newer version. Please reload the Task before proceeding."
                        };
                    }

                    existingTask.Title = model.Title;
                    existingTask.Description = model.Description;
                    existingTask.DueDateTime = (DateTimeOffset)model.DueDateTime;
                    existingTask.UpdatedDateTime = DateTimeOffset.UtcNow;
                    existingTask.UpdatedBy = model.CurrentUserId;
                    existingTask.EntryVersion++;
                    
                    await _dbContext.SaveChangesAsync();

                    return new ExecutionResponseModel()
                    {
                        ExecutionResultId = (int)ExecutionResultEnum.Success,
                        ResponseText = "The Task updated successfully."
                    };
                }
            }
            catch
            {
                return new ExecutionResponseModel()
                {
                    ExecutionResultId = (int)ExecutionResultEnum.InternalServerError,
                    ResponseText = "An error has occured while processing your request. Please contact your administrator if this repeatedly occurs."
                };
            }
           
        }
    }
}
