using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Update.Internal;
using TaskManager.Data.Enum;
using TaskManager.Dto;
using TaskManager.Interfaces;
using TaskManager.Models;

namespace TaskManager.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class ProjectTaskController : Controller
	{
		//DI section
		private readonly IProjectTaskRepository _taskRepository;
		private readonly IProjectRepository _projectRepository;
		private readonly IMapper _mapper;

		public ProjectTaskController(IProjectTaskRepository taskRepository, IProjectRepository projectRepository, IMapper mapper)
		{
			_taskRepository = taskRepository;
			_projectRepository = projectRepository;
			_mapper = mapper;
		}

		[HttpGet]
		[ProducesResponseType(200, Type = typeof(IEnumerable<TaskNoNavigationPropsDto>))]
		public IActionResult GetAllTasks()
		{
			//returns the list of all the tasks and shows it without the navigational properties
			//using auto mapper to create the instances of DTOs to show
			var tasksMap = _mapper.Map<List<TaskNoNavigationPropsDto>>(_taskRepository.GetAllTasks());
			//Validating the model state
			if (!ModelState.IsValid)
				return BadRequest(ModelState);
			//checking if there are any tasks at all
			if (tasksMap.Count() > 0)
				return Ok(tasksMap);
			//no tasks were found
			return NotFound();
		}
		[HttpGet("task-id/{projectTaskId}")]
		[ProducesResponseType(200, Type = typeof(TaskNoNavigationPropsDto))]
		[ProducesResponseType(400)]
		[ProducesResponseType(404)]
		public IActionResult GetTaskById(int projectTaskId)
		{
			//Validating the model state
			if (!ModelState.IsValid)
				return BadRequest(ModelState);
			//Checking the task for existance
			if (!_taskRepository.ProjectTaskExists(projectTaskId))
				return NotFound();
			//using the auto mapper to create the instanse that will be brought in the response
			var projectTaskMap = _mapper.Map<TaskNoNavigationPropsDto>(_taskRepository.GetTaskById(projectTaskId));
			return Ok(projectTaskMap);
		}
		[HttpGet("task-name/{taskName}")]
		[ProducesResponseType(200, Type = typeof(TaskNoNavigationPropsDto))]
		[ProducesResponseType(400)]
		[ProducesResponseType(404)]
		public IActionResult GetTaskByName(string taskName)
		{
			//Validating the model state
			if (!ModelState.IsValid)
				return BadRequest(ModelState);
			//bringing the task
			var projectTask = _taskRepository.GetTaskByName(taskName);
			if(projectTask == null)
			{
				//no task has been found
				ModelState.AddModelError("", "There is no task with this name");
				return NotFound(ModelState);
			}	
			//using the auto mapper to create the instanse that will be brought in the response
			var projectTaskMap = _mapper.Map<TaskNoNavigationPropsDto>(projectTask);
			return Ok(projectTaskMap);
		}

		[HttpGet("in-priority-range")]
		[ProducesResponseType(200, Type = typeof(IEnumerable<TaskNoNavigationPropsDto>))]
		[ProducesResponseType(404)]
		[ProducesResponseType(400)]
		public IActionResult GetTasksInPriorityRange([FromQuery] int priorityLow, [FromQuery] int priorityHigh)
		{
			//Check the validity of input values
			bool priorityLowIsValid = (priorityLow < priorityHigh) && (priorityLow > 0) && (priorityLow < 101);
			bool priorityHighIsValid = priorityHigh < 101;
			if (!(priorityLowIsValid && priorityHighIsValid))
			{
				ModelState.AddModelError("", "Priority values must be in range of 1 and 100");
				return BadRequest(ModelState);
			}

			//returns the list of all the tasks inside the input range
			//and shows it without the navigational properties
			//using auto mapper to create the instances of DTOs to show
			var tasksMap = _mapper.Map<List<TaskNoNavigationPropsDto>>
				(_taskRepository.GetTasksPriorityRange(priorityLow, priorityHigh));
			//validating the model state
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
		public IActionResult GetTasksWithStatus(ProjectTaskStatus status)
		{
			if (status == null)
				return BadRequest(ModelState);

			//returns the list of all the tasks eith input status
			//and shows it without the navigationsl properties
			//using auto mapper to create the instances of DTOs to show
			var taskMap = _mapper.Map<List<TaskNoNavigationPropsDto>>
				(_taskRepository.GetTasksWithStatus(status));

			//Validating the model state
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
		public IActionResult CreateTask(int projectId, [FromBody] ProjectTaskDto taskCreate)
		{
			//checking if any taskDTO was brought in with the request
			if (taskCreate == null)
				return BadRequest(ModelState);
			//checking the project with the given ID for existance
			if (!_projectRepository.ProjectExists(projectId))
				return NotFound();
			//validating the model state
			if (!ModelState.IsValid)
				return BadRequest(ModelState);
			
			//using the auto mapper to create an instance of task to actually save
			var taskMap = _mapper.Map<ProjectTask>(taskCreate);

			//adding the the project property
			taskMap.Project = _projectRepository.GetProjectById(projectId);

			//Checking for existance of tasks with the same name in the given project
			if (_taskRepository.TaskNameAlreadyTaken(taskMap, projectId))
			{
				ModelState.AddModelError("", "This task already exists in this project");
				return StatusCode(422, ModelState);
			}

			if (!_taskRepository.CreateProjectTask(taskMap))
			{
				//chectking for errors in the repository
				ModelState.AddModelError("", "Something went wrong during tha creation process");
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
		public IActionResult UpdateTask([FromQuery] int projectId, int taskId, [FromBody] ProjectTaskDto taskUpdate)
		{
			//checking the task and related project for existance
			if (!(_projectRepository.ProjectExists(projectId) && _taskRepository.ProjectTaskExists(taskId)))
				return NotFound();

			//checking the task is actually belongs to the given project
			if (!_taskRepository.TaskBelongsToProjectNoTracking(taskId, projectId))
			{
				//task is not in the given project
				ModelState.AddModelError("", "This task is not in the entered project");
				return StatusCode(422, ModelState);
			}

			//validating the model state
			if (!ModelState.IsValid)
				return BadRequest(ModelState);

			//use automapper to put changed fields to a new variable that will be used to update Db field
			var taskMap = _mapper.Map<ProjectTask>(taskUpdate);

			//specify the task's Id
			taskMap.Id = taskId;

			//Checking for existance of tasks with the same name in the given project
			if (_taskRepository.TaskNameAlreadyTaken(taskMap, projectId))
			{
				//task with the same name exists
				ModelState.AddModelError("", "Task with the similar name already exists in this project");
				return StatusCode(422, ModelState);
			}

			if (!_taskRepository.UpdateProjectTask(taskMap))
			{
				//Checking for errors in the repository
				ModelState.AddModelError("", "Something went wrong while saving changes");
				return StatusCode(422, ModelState);
			}
			return NoContent();
		}
		[HttpDelete("taskId")]
		[ProducesResponseType(204)]
		[ProducesResponseType(404)]
		[ProducesResponseType(400)]
		[ProducesResponseType(422)]
		public IActionResult DeleteTask(int taskId)
		{
			//Check for existance
			if (!_taskRepository.ProjectTaskExists(taskId))
				return NotFound();

			//bringing the task to delete by id
			var taskToDelete = _taskRepository.GetTaskById(taskId);

			//validating the model state
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
