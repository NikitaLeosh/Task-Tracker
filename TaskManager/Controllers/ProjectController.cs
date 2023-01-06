using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel;
using TaskManager.Dto;
using TaskManager.Interfaces;
using TaskManager.Models;
using TaskManager.Models.Enum;

namespace TaskManager.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class ProjectController : Controller
	{

		private readonly IProjectRepository _projectRepository;
		private readonly IMapper _mapper;
		private readonly IProjectTaskRepository _taskRepository;
		private readonly ICheckProjectRepository _checkProjectRepository;
		private readonly IProjectValidationService _projectValidationService;

		public ProjectController(IProjectRepository projectRepository, IMapper mapper,
			IProjectTaskRepository taskRepository, ICheckProjectRepository checkProjectRepository, IProjectValidationService projectValidationService)
		{
			_projectRepository = projectRepository;
			_mapper = mapper;
			_taskRepository = taskRepository;
			_checkProjectRepository = checkProjectRepository;
			_projectValidationService = projectValidationService;
		}
		[HttpGet]
		[ProducesResponseType(200, Type = typeof(IEnumerable<ProjectNoTasksDto>))]
		[ProducesResponseType(404)]
		public async Task<IActionResult> GetAllProjects()
		{
			//returns the list of all the projects and shows it without the tasks
			//using auto mapper to create the instances of DTOs to show
			var projectsMap = _mapper.Map<List<ProjectNoTasksDto>>(await _projectRepository.GetAllProjectsAsync());
			if (!ModelState.IsValid)
				return BadRequest(ModelState);
			if (projectsMap.Count() > 0)
				return Ok(projectsMap);
			//no projects have been found:
			return NotFound("There is no projects here yet");
		}

		[HttpGet("{projectId}")]
		[ProducesResponseType(200, Type = typeof(ProjectNoTasksDto))]
		[ProducesResponseType(400)]
		public async Task<IActionResult> GetProjectById(Guid projectId)
		{
			if (!ModelState.IsValid)
				return BadRequest(ModelState);
			//checking project for existance
			if (!(await _checkProjectRepository.ProjectExistsAsync(projectId)))
				return NotFound();
			//using auto mapper to create instanse of returned DTO object
			var projectMap = _mapper.Map<ProjectNoTasksDto>(await _projectRepository.GetProjectByIdAsync(projectId));
			return Ok(projectMap);
		}

		[HttpGet("name/{projectName}")]
		[ProducesResponseType(200, Type = typeof(ProjectNoTasksDto))]
		[ProducesResponseType(400)]
		public async Task<IActionResult> GetProjectByName(string projectName)
		{
			if (!ModelState.IsValid)
				return BadRequest(ModelState);
			//using auto mapper to create instanse of returned DTO object
			var project = await _projectRepository.GetProjectByNameAsync(projectName);
			if (project == null)
			{
				ModelState.AddModelError("", "There is no project with this name");
				return NotFound(ModelState);
			}
			var projectMap = _mapper.Map<ProjectNoTasksDto>(project);
			return Ok(projectMap);
		}

		[HttpGet("with-status{status}")]
		[ProducesResponseType(200, Type = typeof(IEnumerable<ProjectNoTasksDto>))]
		[ProducesResponseType(404)]
		[ProducesResponseType(400)]
		public async Task<IActionResult> GetProjectsWithStatus(ProjectStatus status)
		{
			if (status == null)
				return BadRequest(ModelState);
			//returns the list of all the projects that have the given status
			//and shows it without the tasks
			//using auto mapper to create the instances of DTOs to show
			var projectsMap = _mapper.Map<List<ProjectNoTasksDto>>
				(await _projectRepository.GetProjectsWithStatusAsync(status));
			if (!ModelState.IsValid)
				return BadRequest(ModelState);
			if (projectsMap.Count() > 0)
				return Ok(projectsMap);
			//no projects have been found:
			ModelState.AddModelError("", "No projects have this status");
			return NotFound(ModelState);
		}

		[HttpGet("In-priority-range")]
		[ProducesResponseType(200, Type = typeof(IEnumerable<ProjectNoTasksDto>))]
		[ProducesResponseType(404)]
		[ProducesResponseType(400)]
		public async Task<IActionResult> GetProjectsInPriorityRange([FromQuery] int priorityLow, [FromQuery] int priorityHigh)
		{
			ModelState.Clear();
			try
			{
				_projectValidationService.PriorityIsValid(priorityLow);
				_projectValidationService.PriorityIsValid(priorityHigh);
			}
			catch (Exception ex)
			{
				return BadRequest(ex.ToString());
			}
			//returns the list of all the projects having priority inside the input range and shows it without the tasks
			//using auto mapper to create the instances of DTOs to show
			var projectsMap = _mapper.Map<List<ProjectNoTasksDto>>
				(await _projectRepository.GetProjectsPriorityRangeAsync(priorityLow, priorityHigh));
			if (!ModelState.IsValid)
				return BadRequest(ModelState);
			if (projectsMap.Count() > 0)
				return Ok(projectsMap);
			//no projects have been found:
			ModelState.AddModelError("", "No projects were found in a given range");
			return NotFound(ModelState);
		}

		[HttpGet("Start-in-date-range")]
		[ProducesResponseType(200, Type = typeof(IEnumerable<ProjectNoTasksDto>))]
		[ProducesResponseType(404)]
		[ProducesResponseType(400)]
		public async Task<IActionResult> GetProjectsStartInDatesRange([FromQuery] DateTime start, [FromQuery] DateTime end)
		{
			if (!ModelState.IsValid)
				return BadRequest(ModelState);
			if (!_projectValidationService.ProjectDatesAreValid(start, end))
			{
				ModelState.AddModelError("", "Input dates are incorrect");
				return BadRequest(ModelState);
			}
			//returns the list of all the projects starting in the period inside
			//the input dates range and shows it without the tasks
			//using auto mapper to create the instances of DTOs to show
			var projectsMap = _mapper.Map<List<ProjectNoTasksDto>>
				(await _projectRepository.GetProjectsStartAtRangeAsync(start, end));

			if (projectsMap.Count() > 0)
				return Ok(projectsMap);
			//no projects have been found:
			ModelState.AddModelError("", "No projects were found in a given range");
			return NotFound(ModelState);
		}
		[HttpGet("Start-after-date")]
		[ProducesResponseType(200, Type = typeof(IEnumerable<ProjectNoTasksDto>))]
		[ProducesResponseType(404)]
		[ProducesResponseType(400)]
		public async Task<IActionResult> GetProjectsStartAfterDate([FromQuery] DateTime start)
		{
			if (start == null)
				return BadRequest(ModelState);
			//returns the list of all the projects that start after input date and shows it without the tasks
			//using auto mapper to create the instances of DTOs to show
			var projectsMap = _mapper.Map<List<ProjectNoTasksDto>>
				(await _projectRepository.GetProjectsStartAfterAsync(start));
			if (!ModelState.IsValid)
				return BadRequest(ModelState);
			if (projectsMap.Count() > 0)
				return Ok(projectsMap);
			//no projects have been found:
			ModelState.AddModelError("", "No projects were found in a given range");
			return NotFound(ModelState);
		}

		[HttpGet("ends-before-date")]
		[ProducesResponseType(200, Type = typeof(IEnumerable<ProjectNoTasksDto>))]
		[ProducesResponseType(404)]
		[ProducesResponseType(400)]
		public async Task<IActionResult> GetProjectsEndBeforeDate([FromQuery] DateTime end)
		{
			if (end == null)
				return BadRequest(ModelState);

			//returns the list of all the projects with completion date before input
			//and shows it without the tasks
			//using auto mapper to create the instances of DTOs to show
			var tasksMap = _mapper.Map<List<ProjectNoTasksDto>>
				(await _projectRepository.GetProjectsEndBeforeAsync(end));
			if (!ModelState.IsValid)
				return BadRequest(ModelState);
			if (tasksMap.Count() > 0)
				return Ok(tasksMap);
			//no projects have been found:
			ModelState.AddModelError("", "No projects were found in a given range");
			return NotFound(ModelState);
		}

		[HttpPost]
		[ProducesResponseType(204)]
		[ProducesResponseType(400)]
		[ProducesResponseType(422)]
		//receiving the project information from the request body
		//in form of safe DTO for creating and editing the projects
		public async Task<IActionResult> CreateProject([FromBody] ProjectDto projectCreate)
		{
			ModelState.Clear();
			//using auto mapper to create the project instance to actually save it
			var projectMap = _mapper.Map<Project>(projectCreate);
			try
			{
				await _projectValidationService.ProjectIsValidAsync(projectMap);
			}
			catch (Exception ex)
			{
				return BadRequest(ex.ToString());
			}
			if (!_projectRepository.CreateProject(projectMap))
			{
				//checking for errors in the repository
				ModelState.AddModelError("", "Something went wrong during tha creation process");
				return StatusCode(422, ModelState);
			}
			return NoContent();
		}
		[HttpPut("projectId")]
		[ProducesResponseType(204)]
		[ProducesResponseType(400)]
		[ProducesResponseType(422)]
		public async Task<IActionResult> UpdateProject(Guid projectId, [FromBody] ProjectDto projectUpdate)
		{
			ModelState.Clear();

			//using auto mapper to create the project instance to actually update it
			var projectMap = _mapper.Map<Project>(projectUpdate);
			projectMap.Id = projectId;
			if (!(await _checkProjectRepository.ProjectExistsAsync(projectMap.Id)))
			{
				ModelState.AddModelError("", "Project you want to update does not exist");
				return NotFound(ModelState);
			}
			if (!ModelState.IsValid)
				return BadRequest(ModelState);
			try
			{
				await _projectValidationService.ProjectIsValidAsync(projectMap);
			}
			catch (Exception ex)
			{
				return BadRequest(ex.ToString());
			}
			if (!_projectRepository.UpdateProject(projectMap))
			{
				//checking for errors in repository
				ModelState.AddModelError("", "Something went wrong while saving changes");
				return StatusCode(422, ModelState);
			}
			return NoContent();
		}
		[HttpDelete("{projectId}")]
		[ProducesResponseType(204)]
		[ProducesResponseType(400)]
		[ProducesResponseType(404)]
		[ProducesResponseType(422)]
		public async Task<IActionResult> DeleteProject(Guid projectId)
		{

			if (!(await _checkProjectRepository.ProjectExistsAsync(projectId)))
				return NotFound();
			//bringing the actual project
			var projectToDelete = await _projectRepository.GetProjectByIdAsync(projectId);
			if (!ModelState.IsValid)
				return BadRequest(ModelState);
			//bringing in the project's tasks to also delete
			var tasksOfProjectToDelete = await _taskRepository.GetTasksOfAProjectAsync(projectId);
			if (tasksOfProjectToDelete.Count != 0)
			{
				foreach (ProjectTask task in tasksOfProjectToDelete)
				{
					if (!_taskRepository.DeleteProjectTask(task))
					{
						//checking for errors  in the repository while deleting the project's tasks
						ModelState.AddModelError("", "Something went wrong during deleting the project's tasks");
						return StatusCode(422, ModelState);
					}
				}
			}
			//deleting the project
			if (!_projectRepository.DeleteProject(projectToDelete))
			{
				//checking for errors  in the repository while deleting
				ModelState.AddModelError("", "Something went wrong during deleting the project");
				return StatusCode(422, ModelState);
			}
			return NoContent();
		}

	}
}
