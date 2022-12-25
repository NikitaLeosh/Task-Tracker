using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel;
using TaskManager.Data.Enum;
using TaskManager.Dto;
using TaskManager.Interfaces;
using TaskManager.Models;

namespace TaskManager.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class ProjectController : Controller
	{
		
		private readonly IProjectRepository _projectRepository;
		private readonly IMapper _mapper;
		private readonly IProjectTaskRepository _taskRepository;

		public ProjectController(IProjectRepository projectRepository, IMapper mapper, IProjectTaskRepository taskRepository)
		{
			_projectRepository = projectRepository;
			_mapper = mapper;
			_taskRepository = taskRepository;
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
		public async Task<IActionResult> GetProjectById(int projectId)
		{
			if (!ModelState.IsValid)
				return BadRequest(ModelState);
			//checking project for existance
			if (!(await _projectRepository.ProjectExistsAsync(projectId)))
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
			if(project == null)
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
			//Check the validity of input values
			bool priorityLowIsValid = (priorityLow < priorityHigh)&&(priorityLow>0)&&(priorityLow<101);
			bool priorityHighIsValid = priorityHigh < 101;
			if(!(priorityLowIsValid && priorityHighIsValid))
			{
				ModelState.AddModelError("", "Priority values must be in range of 1 and 100");
				return BadRequest(ModelState);
			}	
			//returns the list of all the projects having priority inside the input range and shows it without the tasks
			//using auto mapper to create the instances of DTOs to show
			var projectsMap = _mapper.Map<List<ProjectNoTasksDto>>
				(await _projectRepository.GetProjectsPriorityRangeAsync(priorityLow,priorityHigh));
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
			if (start == null || end == null)
				return BadRequest(ModelState);
			if (!ModelState.IsValid)
				return BadRequest(ModelState);
			//Check the validity of input values
			if (start>=end)
			{
				ModelState.AddModelError("", "Start date should be before the end date");
				return BadRequest(ModelState);
			}
			//returns the list of all the projects starting in the period inside
			//the input dates range and shows it without the tasks
			//using auto mapper to create the instances of DTOs to show
			var projectsMap = _mapper.Map<List<ProjectNoTasksDto>>
				(await _projectRepository.GetProjectsStartAtRangeAsync(start,end));
			
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

		
		[HttpGet("{projectId}/tasks")]
		[ProducesResponseType(200, Type = typeof(IEnumerable<ProjectTaskDto>))]
		[ProducesResponseType(400)]
		[ProducesResponseType(404)]
		public async Task<IActionResult> GetTasksOfAProject(int projectId)
		{
			ModelState.Clear();
			//checking the project for existance
			if (!(await _projectRepository.ProjectExistsAsync(projectId)))
			{
				//the project does not exist. Adding the model error and returning "not found" code
				ModelState.AddModelError("", "Project has not been found");
				return NotFound(ModelState);
			}
			//validating the model state
			if (!ModelState.IsValid)
				return BadRequest(ModelState);
			//using auto mapper to create instanse of returned DTO object
			var tasksMap = _mapper.Map<List<ProjectTaskDto>>(await _projectRepository.GetTasksOfAProjectAsync(projectId));
			if (tasksMap.Count == 0)
			{
				//project has no tasks
				ModelState.AddModelError("", "Project has no tasks");
				return NotFound(ModelState);
			}
			return Ok(tasksMap);
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

			//checking if anything has been brought in the request body
			if (projectCreate == null)
				return BadRequest(ModelState);

			//vallidating the model state
			if (!ModelState.IsValid)
				return BadRequest(ModelState);

			//check the dates are correct
			if(projectCreate.StartDate>=projectCreate.CompletionDate)
			{
				ModelState.AddModelError("", "Project should start earlier then it ends");
				return BadRequest(ModelState);
			}

			//using auto mapper to create the project instance to actually save it
			var projectMap = _mapper.Map<Project>(projectCreate);

			//checking for the exixting projects with the same names
			if (await _projectRepository.ProjectNameAlreadyTakenAsync(projectMap))
			{
				//there is project with the same name
				ModelState.AddModelError("", "This project already exists");
				return StatusCode(422, ModelState);
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
		public async Task<IActionResult> UpdateProject(int projectId, [FromBody] ProjectDto projectUpdate)
		{
			ModelState.Clear();
			//checking if anything has been brought in the request body
			if (projectUpdate == null)
				return BadRequest(ModelState);

			//checking the project for existance
			if(!(await _projectRepository.ProjectExistsAsync(projectId)))
				return NotFound();

			//Checking the dates are correct
			if (projectUpdate.StartDate >= projectUpdate.CompletionDate)
			{
				ModelState.AddModelError("", "Project should start earlier then it ends");
				return BadRequest(ModelState);
			}

			//using auto mapper to create the project instance to actually update it
			var projectMap = _mapper.Map<Project>(projectUpdate);
			projectMap.Id = projectId;

			//checking for the exixting projects with the same names
			if (await _projectRepository.ProjectNameAlreadyTakenAsync(projectMap))
			{
				//there is project with the same name
				ModelState.AddModelError("", "This project already exists");
				return StatusCode(422, ModelState);
			}
			//validating the model state
			if (!ModelState.IsValid)
				return BadRequest(ModelState);

			

			//Updating the project
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
		public async Task<IActionResult> DeleteProject(int projectId)
		{
			//checking the project for extatance
			if (!(await _projectRepository.ProjectExistsAsync(projectId)))
				return NotFound();
			//bringing the actual project
			var projectToDelete = await _projectRepository.GetProjectByIdAsync(projectId);
			//bringing in the project's tasks to also delete
			var tasksOfProjectToDelete = await _projectRepository.GetTasksOfAProjectAsync(projectId);
			//checking if there are any tasks
			if (tasksOfProjectToDelete.Count != 0)
			{
				//deleting the project's tasks first
				foreach(ProjectTask task in tasksOfProjectToDelete)
				{ 
					if (!_taskRepository.DeleteProjectTask(task))
					{
						//checking for errors  in the repository while deleting the project's tasks
						ModelState.AddModelError("", "Something went wrong during deleting the project's tasks");
						return StatusCode(422, ModelState);
					}
				}
			}
			//validating the model state
			if (!ModelState.IsValid)
				return BadRequest(ModelState);
			//deleting the project
			if(!_projectRepository.DeleteProject(projectToDelete))
			{
				//checking for errors  in the repository while deleting
				ModelState.AddModelError("", "Something went wrong during deleting the project");
				return StatusCode(422, ModelState);
			}
			return NoContent();
		}

	}
}
