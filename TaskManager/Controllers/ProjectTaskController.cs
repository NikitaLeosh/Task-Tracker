using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Update.Internal;
using TaskManager.Dto;
using TaskManager.Interfaces;
using TaskManager.Models;
using TaskManager.Models.Enum;

namespace TaskManager.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class ProjectTaskController : Controller
	{
		//DI section
		private readonly IProjectTaskRepository _taskRepository;
		private readonly IProjectRepository _projectRepository;
		private readonly ICheckTaskRepository _checkTaskRepository;
		private readonly ICheckProjectRepository _checkProjectRepository;
		private readonly IMapper _mapper;
		private readonly ITaskValidationService _taskValidationService;

		public ProjectTaskController(IProjectTaskRepository taskRepository, IProjectRepository projectRepository,
			ICheckTaskRepository checkTaskRepository, ICheckProjectRepository checkProjectRepository, IMapper mapper,
			ITaskValidationService taskValidationService)
		{
			_taskRepository = taskRepository;
			_projectRepository = projectRepository;
			_checkTaskRepository = checkTaskRepository;
			_checkProjectRepository = checkProjectRepository;
			_mapper = mapper;
			_taskValidationService = taskValidationService;
		}

		[HttpGet]
		[ProducesResponseType(200, Type = typeof(IEnumerable<TaskNoNavigationPropsDto>))]
		public async Task<IActionResult> GetAllTasks()
		{
			//returns the list of all the tasks and shows it without the navigational properties
			//using auto mapper to create the instances of DTOs to show
			var tasksMap = _mapper.Map<List<TaskNoNavigationPropsDto>>(await _taskRepository.GetAllTasksAsync());
			if (!ModelState.IsValid)
				return BadRequest(ModelState);
			if (tasksMap.Count() > 0)
				return Ok(tasksMap);
			//no tasks were found
			return NotFound();
		}
		[HttpGet("task-id/{projectTaskId}")]
		[ProducesResponseType(200, Type = typeof(TaskNoNavigationPropsDto))]
		[ProducesResponseType(400)]
		[ProducesResponseType(404)]
		public async Task<IActionResult> GetTaskById(Guid projectTaskId)
		{
			if (!ModelState.IsValid)
				return BadRequest(ModelState);
			if (!(await _checkTaskRepository.ProjectTaskExistsAsync(projectTaskId)))
				return NotFound();
			//using the auto mapper to create the instanse that will be brought in the response
			var projectTaskMap = _mapper.Map<TaskNoNavigationPropsDto>(
				await _taskRepository.GetTaskByIdAsync(projectTaskId));
			return Ok(projectTaskMap);
		}
		[HttpGet("task-name/{taskName}")]
		[ProducesResponseType(200, Type = typeof(TaskNoNavigationPropsDto))]
		[ProducesResponseType(400)]
		[ProducesResponseType(404)]
		public async Task<IActionResult> GetTaskByName(string taskName)
		{
			if (!ModelState.IsValid)
				return BadRequest(ModelState);
			var projectTask = await _taskRepository.GetTaskByNameAsync(taskName);
			if (projectTask == null)
			{
				//no task has been found
				ModelState.AddModelError("", "There is no task with this name");
				return NotFound(ModelState);
			}
			//using the auto mapper to create the instanse that will be brought in the response
			var projectTaskMap = _mapper.Map<TaskNoNavigationPropsDto>(projectTask);
			return Ok(projectTaskMap);
		}

		[HttpGet("{projectId}/tasks")]
		[ProducesResponseType(200, Type = typeof(IEnumerable<ProjectTaskDto>))]
		[ProducesResponseType(400)]
		[ProducesResponseType(404)]
		public async Task<IActionResult> GetTasksOfAProject(Guid projectId)
		{
			ModelState.Clear();
			if (!(await _checkProjectRepository.ProjectExistsAsync(projectId)))
			{
				//the project does not exist. Adding the model error and returning "not found" code
				ModelState.AddModelError("", "Project has not been found");
				return NotFound(ModelState);
			}
			if (!ModelState.IsValid)
				return BadRequest(ModelState);
			var tasksMap = _mapper.Map<List<ProjectTaskDto>>(await _taskRepository.GetTasksOfAProjectAsync(projectId));
			if (tasksMap.Count == 0)
			{
				//project has no tasks
				ModelState.AddModelError("", "Project has no tasks");
				return NotFound(ModelState);
			}
			return Ok(tasksMap);
		}

		[HttpGet("in-priority-range")]
		[ProducesResponseType(200, Type = typeof(IEnumerable<TaskNoNavigationPropsDto>))]
		[ProducesResponseType(404)]
		[ProducesResponseType(400)]
		public async Task<IActionResult> GetTasksInPriorityRange([FromQuery] int priorityLow, [FromQuery] int priorityHigh)
		{
			if (!(_taskValidationService.PriorityIsValid(priorityLow) && _taskValidationService.PriorityIsValid(priorityHigh)))
			{
				ModelState.AddModelError("", "Priority value mast be in range if 1 and 5");
				return BadRequest(ModelState);
			}
			//returns the list of all the tasks inside the input range
			//and shows it without the navigational properties
			//using auto mapper to create the instances of DTOs to show
			var tasksMap = _mapper.Map<List<TaskNoNavigationPropsDto>>
				(await _taskRepository.GetTasksPriorityRangeAsync(priorityLow, priorityHigh));
			if (!ModelState.IsValid)
				return BadRequest(ModelState);
			if (tasksMap.Count() > 0)
				return Ok(tasksMap);
			//no projects have been found:
			ModelState.AddModelError("", "No projects were found in a given range");
			return NotFound(ModelState);
		}

		[HttpGet("with-status{status}")]
		[ProducesResponseType(200, Type = typeof(IEnumerable<TaskNoNavigationPropsDto>))]
		[ProducesResponseType(404)]
		[ProducesResponseType(400)]
		public async Task<IActionResult> GetTasksWithStatus(ProjectTaskStatus status)
		{
			if (status == null)
				return BadRequest(ModelState);

			//returns the list of all the tasks eith input status
			//and shows it without the navigationsl properties
			//using auto mapper to create the instances of DTOs to show
			var taskMap = _mapper.Map<List<TaskNoNavigationPropsDto>>
				(await _taskRepository.GetTasksWithStatusAsync(status));

			if (!ModelState.IsValid)
				return BadRequest(ModelState);
			if (taskMap.Count() > 0)
				return Ok(taskMap);
			//no projects have been found:
			ModelState.AddModelError("", "No projects have this status");
			return NotFound(ModelState);
		}


		[HttpPost("create/inproject{projectId}")]
		[ProducesResponseType(204)]
		[ProducesResponseType(400)]
		[ProducesResponseType(404)]
		[ProducesResponseType(422)]
		public async Task<IActionResult> CreateTask(Guid projectId, [FromBody] ProjectTaskDto taskCreate)
		{
			var taskMap = _mapper.Map<ProjectTask>(taskCreate);
			//adding the the project property
			taskMap.Project = await _projectRepository.GetProjectByIdAsync(projectId);
			try
			{
				await _taskValidationService.TaskIsValidAsync(taskMap, projectId);
			}
			catch (Exception ex)
			{
				return BadRequest(ex.ToString());
			}
			if (!ModelState.IsValid)
				return BadRequest();
			if (!_taskRepository.CreateProjectTask(taskMap))
			{
				//chectking for errors in the repository
				ModelState.AddModelError("", "Something went wrong during the creation process");
				return StatusCode(422, ModelState);
			}
			return NoContent();
		}

		[HttpPut("update/{taskId}")]
		[ProducesResponseType(204)]
		[ProducesResponseType(400)]
		[ProducesResponseType(404)]
		[ProducesResponseType(422)]

		//getting the project ID from query
		public async Task<IActionResult> UpdateTask([FromQuery] Guid projectId, Guid taskId, [FromBody] ProjectTaskDto taskUpdate)
		{
			///<summary>
			///The following line is for unit testing purposes.
			/// </summary>
			ModelState.Clear();
			var taskMap = _mapper.Map<ProjectTask>(taskUpdate);
			taskMap.Id = taskId;
			//checking the task and related project for existance
			if (await _checkTaskRepository.ProjectTaskExistsAsync(taskId))
			{
				try
				{
					_taskValidationService.TaskIsValidAsync(taskMap, projectId);
				}
				catch (Exception ex)
				{
					return BadRequest(ex.ToString());
				}
			}
			else
			{
				ModelState.AddModelError("", "Task not found");
				return NotFound(ModelState);
			}
			if (!await _checkTaskRepository.TaskBelongsToProjectNoTrackingAsync(taskId, projectId))
			{
				ModelState.AddModelError("", "Given project does not contain sucn task");
				return UnprocessableEntity(ModelState);
			}
			if (!ModelState.IsValid)
				return BadRequest(ModelState);
			if (!_taskRepository.UpdateProjectTask(taskMap))
			{
				//Checking for errors in the repository
				ModelState.AddModelError("", "Something went wrong while saving changes");
				return UnprocessableEntity(ModelState);
			}
			return NoContent();
		}
		[HttpDelete("taskId")]
		[ProducesResponseType(204)]
		[ProducesResponseType(404)]
		[ProducesResponseType(400)]
		[ProducesResponseType(422)]
		public async Task<IActionResult> DeleteTask(Guid taskId)
		{
			if (!(await _checkTaskRepository.ProjectTaskExistsAsync(taskId)))
				return NotFound();
			var taskToDelete = await _taskRepository.GetTaskByIdAsync(taskId);
			if (!ModelState.IsValid)
				return BadRequest(ModelState);

			if (!_taskRepository.DeleteProjectTask(taskToDelete))
			{
				//Checking for errors in the repository
				ModelState.AddModelError("", "Something went wrong during deleting the task");
				return StatusCode(422, ModelState);
			}
			//successfully deleted
			return NoContent();
		}

	}
}
