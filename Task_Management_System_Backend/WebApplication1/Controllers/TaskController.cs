using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Web.Http.Results;
using TMS_DAL.Common;
using TMS_DAL.Entities;
using TMS_DAL.Interfaces.Task;
using TMS_DAL.Models.Common;
using TMS_DAL.Models.Task;

namespace TMS_API.Controllers
{
    [Authorize]
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class TaskController : ControllerBase
    {
        private readonly ITaskRepository _taskRepo;

        public TaskController(ITaskRepository taskRepo)
        {
            _taskRepo = taskRepo;
        }

        [HttpPost]
        public async Task<IActionResult> CreateTask([FromBody] TaskModel model)
        {
            #region Get User Id
            var currentUserId = User.FindFirst("UserId")?.Value;

            if (currentUserId == null)
            {
                return Unauthorized();
            }
            #endregion

            var result = new ExecutionResponseModel();

            if(string.IsNullOrEmpty(model.Title) || model.DueDateTime == null || 
                (model.TaskId == null && model.EntryIdentifier == null) ||
                (model.TaskId != null && model.EntryVersion == null))
            {
                result = new ExecutionResponseModel()
                {
                    ExecutionResultId = (int)ExecutionResultEnum.ValidationError,
                    ResponseText = "Please provide the all the required fields."
                };

                return BadRequest(result);
            }
            else
            {
                model.CurrentUserId = currentUserId;

                result = await _taskRepo.SaveTask(model);

                if(result.ExecutionResultId == (int)ExecutionResultEnum.Success)
                {
                    if(model.TaskId != null)
                    {
                        return Ok(result);
                    }
                    else
                    {
                        return Created("",result);
                    }
                }
                else if(result.ExecutionResultId == (int)ExecutionResultEnum.RecordExist ||
                    result.ExecutionResultId == (int)ExecutionResultEnum.VersionMismatch)
                {
                    return Conflict(result);
                }
                else if (result.ExecutionResultId == (int)ExecutionResultEnum.ResourceNotFound)
                {
                    return NotFound(result);
                }
                else
                {
                    return StatusCode(500, result);
                }

            }
        }

        [HttpPost]
        public async Task<IActionResult>DeleteTask([FromBody] TaskModel model)
        {
            #region Get User Id
            var currentUserId = User.FindFirst("UserId")?.Value;

            if (currentUserId == null)
            {
                return Unauthorized();
            }
            #endregion

            var result = new ExecutionResponseModel();

            if (model.TaskId == null || model.EntryVersion == null)
            {
                result = new ExecutionResponseModel()
                {
                    ExecutionResultId = (int)ExecutionResultEnum.ValidationError,
                    ResponseText = "There is a validation error. Please try again."
                };

                return BadRequest(result);
            }
            else
            {
                model.CurrentUserId = currentUserId;

                result = await _taskRepo.DeleteTask(model);

                if (result.ExecutionResultId == (int)ExecutionResultEnum.Success)
                {
                    return Ok(result);
                }
                else if (result.ExecutionResultId == (int)ExecutionResultEnum.VersionMismatch)
                {
                    return Conflict(result);
                }
                else if (result.ExecutionResultId == (int)ExecutionResultEnum.ResourceNotFound)
                {
                    return NotFound(result);
                }
                else
                {
                    return StatusCode(500, result);
                }
            }

        }

        [HttpPost]
        public async Task<IActionResult> ChangeTaskStatus([FromBody] TaskModel model)
        {
            #region Get User Id
            var currentUserId = User.FindFirst("UserId")?.Value;

            if (currentUserId == null)
            {
                return Unauthorized();
            }
            #endregion

            var result = new ExecutionResponseModel();

            if (model.TaskId == null || model.EntryVersion == null)
            {
                result = new ExecutionResponseModel()
                {
                    ExecutionResultId = (int)ExecutionResultEnum.ValidationError,
                    ResponseText = "There is a validation error. Please try again."
                };

                return BadRequest(result);
            }
            else
            {
                model.CurrentUserId = currentUserId;

                result = await _taskRepo.ChangeTaskStatus(model);

                if (result.ExecutionResultId == (int)ExecutionResultEnum.Success)
                {
                    return Ok(result);
                }
                else if (result.ExecutionResultId == (int)ExecutionResultEnum.VersionMismatch)
                {
                    return Conflict(result);
                }
                else if (result.ExecutionResultId == (int)ExecutionResultEnum.ResourceNotFound)
                {
                    return NotFound(result);
                }
                else
                {
                    return StatusCode(500, result);
                }
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetTaskById(int taskId)
        {
            #region Get User Id
            var currentUserId = User.FindFirst("UserId")?.Value;

            if (currentUserId == null)
            {
                return Unauthorized();
            }
            #endregion

            var result = new ExecutionResponseModel();

            var task = await _taskRepo.GetTaskById(taskId, currentUserId);

            if (task == null)
            {
                result = new ExecutionResponseModel()
                {
                    ExecutionResultId = (int)ExecutionResultEnum.ResourceNotFound,
                    ResponseText = "An error has occured while processing your request. Please contact your administrator if this repeatedly occurs."
                };

                return StatusCode(500,result);
            }
            else if (task.TaskId == null)
            {
                result = new ExecutionResponseModel()
                {
                    ExecutionResultId = (int)ExecutionResultEnum.ResourceNotFound,
                    ResponseText = "The Task, you are requesting is no found. Please try again."
                };

                return NotFound(result);
            }
            else
            {
                result = new ExecutionResponseModel()
                {
                    ExecutionResultId = (int)ExecutionResultEnum.Success,
                    Data = task
                };

                return Ok(result);
            }

        }

        [HttpPost]
        public async Task<IActionResult> GetTaskList([FromBody] TaskListFilterModel model)
        {
            #region Get User Id
            var currentUserId = User.FindFirst("UserId")?.Value;

            if(currentUserId == null)
            {
                return Unauthorized();
            }
            #endregion

            model.CurrentUserId = currentUserId;

            var result = new ExecutionResponseModel();

            var taskListModel = await _taskRepo.GetTaskList(model);

            if(taskListModel == null)
            {
                result = new ExecutionResponseModel()
                {
                    ExecutionResultId = (int)ExecutionResultEnum.InternalServerError,
                    ResponseText = "There was an error while fetching the task list. Please try again."
                };

                return StatusCode(500,result);
            }
            else
            {
                result = new ExecutionResponseModel()
                {
                    ExecutionResultId = (int)ExecutionResultEnum.Success,
                    Data = taskListModel
                };

                return Ok(result);
            }
        }
    }
}
