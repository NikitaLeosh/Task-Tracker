using AutoMapper;
using FakeItEasy;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.WebSockets;
using System.Text;
using System.Threading.Tasks;
using TaskManager.Controllers;
using TaskManager.Dto;
using TaskManager.Dto.Enum;
using TaskManager.Exceptions;
using TaskManager.Interfaces;
using TaskManager.Models;
using TaskManager.Models.Enum;

namespace TaskManager.Tests.Controller
{
	public class ProjectControllerTests
	{
		private readonly IProjectRepository _projectRepository;
		private readonly IMapper _mapper;
		private readonly IProjectTaskRepository _taskRepository;
		private readonly ICheckProjectRepository _checkProjectRepository;
		private readonly IProjectValidationService _projectValidationService;
		public ProjectControllerTests()
		{
			_projectRepository = A.Fake<IProjectRepository>();
			_mapper = A.Fake<IMapper>();
			_taskRepository = A.Fake<IProjectTaskRepository>();
			_checkProjectRepository = A.Fake<ICheckProjectRepository>();
			_projectValidationService = A.Fake<IProjectValidationService>();
		}
		[Fact]
		public async void ProjectController_GetAllProjects_ReturnsOk()
		{
			//Arrange
			var projects = A.CollectionOfFake<Project>(1);
			var projectsMap = A.Fake<List<ProjectNoTasksDto>>();
			projectsMap.Add(new ProjectNoTasksDto
			{
				Id = 1,
				ProjectName = "asd",
				Priority = 3,
				ProjectStatus = DtoProjectStatus.Active

			});
			A.CallTo(() => _projectRepository.GetAllProjectsAsync()).Returns(projects);
			A.CallTo(() => _mapper.Map<List<ProjectNoTasksDto>>(projects)).Returns(projectsMap);
			var controller = new ProjectController(_projectRepository, _mapper, _taskRepository, _checkProjectRepository, _projectValidationService);
			//Act
			var result = await controller.GetAllProjects();
			//Assert
			result.Should().NotBeNull();
			result.Should().BeOfType(typeof(OkObjectResult));
		}
		[Fact]
		public async void ProjectController_GetProjectById_ReturnsOk()
		{
			//Arrange
			Guid projectId = Guid.NewGuid();
			var project = A.Fake<Project>();
			project.Id = projectId;
			var projectMap = A.Fake<ProjectNoTasksDto>();
			A.CallTo(() => _mapper.Map<ProjectNoTasksDto>(project)).Returns(projectMap);
			A.CallTo(() => _checkProjectRepository.ProjectExistsAsync(projectId)).Returns(true);
			var controller = new ProjectController(_projectRepository, _mapper, _taskRepository, _checkProjectRepository, _projectValidationService);

			//Act
			var result = await controller.GetProjectById(projectId);

			//Assert
			result.Should().NotBeNull();
			result.Should().BeOfType(typeof(OkObjectResult));
		}
		[Fact]
		public async void ProjectController_GetProjectByName_RetusnsOf()
		{
			//Arrange
			var projectName = "projectname";
			var project = A.Fake<Project>();
			var projectMap = A.Fake<ProjectNoTasksDto>();
			//project.ProjectName = projectName;
			A.CallTo(() => _mapper.Map<ProjectNoTasksDto>(project)).Returns(projectMap);
			var controller = new ProjectController(_projectRepository, _mapper, _taskRepository, _checkProjectRepository, _projectValidationService);
			//Act
			var result = await controller.GetProjectByName(projectName);
			//Assert
			result.Should().NotBeNull();
			result.Should().BeOfType(typeof(OkObjectResult));
		}
		[Fact]
		public async void ProjectController_GetProjectsWithStatus_ReturnsOk()
		{
			//Arrange
			var status = ProjectStatus.Active;
			var statusEmptyResult = ProjectStatus.NotStarted;
			//fake populated collections
			var projects = A.CollectionOfFake<Project>(1);
			var projectsMap = A.Fake<List<ProjectNoTasksDto>>();
			projectsMap.Add(A.Fake<ProjectNoTasksDto>());
			//fake empty collections
			var projectsEmpty = A.CollectionOfFake<Project>(0);
			var emptyProjectsMap = A.Fake<List<ProjectNoTasksDto>>();
			A.CallTo(() => _mapper.Map<List<ProjectNoTasksDto>>(projects)).Returns(projectsMap);
			A.CallTo(() => _mapper.Map<List<ProjectNoTasksDto>>(projectsEmpty)).Returns(emptyProjectsMap);
			A.CallTo(() => _projectRepository.GetProjectsWithStatusAsync(status)).Returns(projects);
			A.CallTo(() => _projectRepository.GetProjectsWithStatusAsync(statusEmptyResult)).Returns(projectsEmpty);
			var controller = new ProjectController(_projectRepository, _mapper, _taskRepository, _checkProjectRepository, _projectValidationService);
			//Act
			var result = await controller.GetProjectsWithStatus(status);
			var notFoundResult = await controller.GetProjectsWithStatus(statusEmptyResult);
			//Assert
			result.Should().NotBeNull();
			result.Should().BeOfType(typeof(OkObjectResult));
			notFoundResult.Should().NotBeNull();
			notFoundResult.Should().BeOfType(typeof(NotFoundObjectResult));
		}
		[Fact]
		public async void ProjectController_GetProjectsInPriorityRange_ReturnsOk()
		{
			//Arrange
			var priorityLow = 50;
			var priorityHigh = 80;
			var priorityInvalid = 101;
			var priorityEmptyResult = 90;
			//fake populated collections
			var projects = A.CollectionOfFake<Project>(1);
			var projectsMap = A.Fake<List<ProjectNoTasksDto>>();
			projectsMap.Add(A.Fake<ProjectNoTasksDto>());
			//fake empty collections
			var projectsEmpty = A.CollectionOfFake<Project>(0);
			var projectsMapEmpty = A.Fake<List<ProjectNoTasksDto>>();
			//normal logic
			A.CallTo(() => _projectRepository.GetProjectsPriorityRangeAsync(priorityLow, priorityHigh)).Returns(projects);
			A.CallTo(() => _mapper.Map<List<ProjectNoTasksDto>>(projects)).Returns(projectsMap);
			A.CallTo(() => _projectValidationService.PriorityIsValid(priorityLow)).Returns(true);
			A.CallTo(() => _projectValidationService.PriorityIsValid(priorityHigh)).Returns(true);
			//logic to return empty result
			A.CallTo(() => _mapper.Map<List<ProjectNoTasksDto>>(projects)).Returns(projectsMap);
			A.CallTo(() => _projectValidationService.PriorityIsValid(priorityEmptyResult)).Returns(true);
			A.CallTo(() => _projectRepository.GetProjectsPriorityRangeAsync(priorityLow, priorityEmptyResult)).Returns(projectsEmpty);
			//logic for invalid priority input
			A.CallTo(() => _projectValidationService.PriorityIsValid(priorityInvalid)).Throws(
				new InvalidPriorityException("Invalid priority")); ;
			var controller = new ProjectController(_projectRepository, _mapper, _taskRepository, _checkProjectRepository, _projectValidationService);
			//Act
			var positiveResult = await controller.GetProjectsInPriorityRange(priorityLow, priorityHigh);
			var notFoundNegativeResult = await controller.GetProjectsInPriorityRange(priorityLow, priorityEmptyResult);
			//priority value out of range
			var negativeResult = await controller.GetProjectsInPriorityRange(priorityLow, priorityInvalid);
			//Assert
			//normal
			positiveResult.Should().NotBeNull();
			positiveResult.Should().BeOfType(typeof(OkObjectResult));
			//empty collection returns - not found
			notFoundNegativeResult.Should().NotBeNull();
			notFoundNegativeResult.Should().BeOfType(typeof(NotFoundObjectResult));
			//invalid request - priority value is out of range
			negativeResult.Should().NotBeNull();
			negativeResult.Should().BeOfType(typeof(BadRequestObjectResult));
		}
		[Fact]
		public async void ProjectController_GetProjectsStartInDatesRange_ReturnsOk()
		{
			//Arrange
			DateTime start = new DateTime(2020, 5, 6);
			DateTime end = new DateTime(2021, 5, 6);
			DateTime startEmpty = new DateTime(2020, 6, 6);
			DateTime endEmpty = new DateTime(2021, 6, 6);
			//populated collections
			var projects = A.CollectionOfFake<Project>(1);
			var projectsMap = A.Fake<List<ProjectNoTasksDto>>();
			projectsMap.Add(A.Fake<ProjectNoTasksDto>());
			//empty collections
			var projectsEmpty = A.CollectionOfFake<Project>(0);
			var projectsMapEmpty = A.Fake<List<ProjectNoTasksDto>>();

			//logic to return populated collection
			A.CallTo(() => _projectRepository.GetProjectsStartAtRangeAsync(start, end)).Returns(projects);
			A.CallTo(() => _mapper.Map<List<ProjectNoTasksDto>>(projects)).Returns(projectsMap);
			A.CallTo(() => _projectValidationService.ProjectDatesAreValid(start, end)).Returns(true);
			A.CallTo(() => _projectValidationService.ProjectDatesAreValid(end, start)).Returns(false);
			//logic to return empty collection
			A.CallTo(() => _projectRepository.GetProjectsStartAtRangeAsync(startEmpty, endEmpty)).Returns(projectsEmpty);
			A.CallTo(() => _mapper.Map<List<ProjectNoTasksDto>>(projectsEmpty)).Returns(projectsMapEmpty);
			A.CallTo(() => _projectValidationService.ProjectDatesAreValid(startEmpty, endEmpty)).Returns(true);
			var controller = new ProjectController(_projectRepository, _mapper, _taskRepository, _checkProjectRepository, _projectValidationService);
			//Act
			//normal
			var positiveResult = await controller.GetProjectsStartInDatesRange(start, end);
			//empty result - not found
			var negativeResultEmpty = await controller.GetProjectsStartInDatesRange(startEmpty, endEmpty);
			//incorrect input - start is later then finish
			var negativeResult = await controller.GetProjectsStartInDatesRange(end, start);
			//Assert
			positiveResult.Should().NotBeNull();
			positiveResult.Should().BeOfType(typeof(OkObjectResult));
			negativeResult.Should().NotBeNull();
			negativeResult.Should().BeOfType(typeof(BadRequestObjectResult));
			negativeResultEmpty.Should().NotBeNull();
			negativeResultEmpty.Should().BeOfType(typeof(NotFoundObjectResult));
		}

		[Fact]
		public async void ProjectController_GetProjectsStartAfterDate_returnsOk()
		{
			//Arrange
			var start = new DateTime(2020, 11, 2);
			var startEmptyResult = new DateTime(2022, 11, 2);
			//populated collections
			var projects = A.CollectionOfFake<Project>(1);
			var projectsMap = A.Fake<List<ProjectNoTasksDto>>();
			projectsMap.Add(A.Fake<ProjectNoTasksDto>());
			//empty collections
			var projectsEmpty = A.CollectionOfFake<Project>(0);
			var projectsMapEmpty = A.Fake<List<ProjectNoTasksDto>>();
			A.CallTo(() => _projectRepository.GetProjectsStartAfterAsync(start)).Returns(projects);
			A.CallTo(() => _mapper.Map<List<ProjectNoTasksDto>>(projects)).Returns(projectsMap);
			//logic to return empty collection
			A.CallTo(() => _projectRepository.GetProjectsStartAfterAsync(startEmptyResult)).Returns(projectsEmpty);
			A.CallTo(() => _mapper.Map<List<ProjectNoTasksDto>>(projectsEmpty)).Returns(projectsMapEmpty);
			var controller = new ProjectController(_projectRepository, _mapper, _taskRepository, _checkProjectRepository, _projectValidationService);
			//Act
			var okResult = await controller.GetProjectsStartAfterDate(start);
			var notFoundResult = await controller.GetProjectsStartAfterDate(startEmptyResult);
			//Assert
			okResult.Should().NotBeNull();
			okResult.Should().BeOfType(typeof(OkObjectResult));
			notFoundResult.Should().NotBeNull();
			notFoundResult.Should().BeOfType(typeof(NotFoundObjectResult));
		}

		[Fact]
		public async void ProjectController_GetProjectsEndBeforeDate_returnsOk()
		{
			//Arrange
			var end = new DateTime(2020, 11, 2);
			var endEmptyResult = new DateTime(2022, 11, 2);
			//populated collections
			var projects = A.CollectionOfFake<Project>(1);
			var projectsMap = A.Fake<List<ProjectNoTasksDto>>();
			projectsMap.Add(A.Fake<ProjectNoTasksDto>());
			//empty collections
			var projectsEmpty = A.CollectionOfFake<Project>(0);
			var projectsMapEmpty = A.Fake<List<ProjectNoTasksDto>>();
			A.CallTo(() => _projectRepository.GetProjectsEndBeforeAsync(end)).Returns(projects);
			A.CallTo(() => _mapper.Map<List<ProjectNoTasksDto>>(projects)).Returns(projectsMap);
			//logic to return empty collection
			A.CallTo(() => _projectRepository.GetProjectsEndBeforeAsync(endEmptyResult)).Returns(projectsEmpty);
			A.CallTo(() => _mapper.Map<List<ProjectNoTasksDto>>(projectsEmpty)).Returns(projectsMapEmpty);
			var controller = new ProjectController(_projectRepository, _mapper, _taskRepository, _checkProjectRepository, _projectValidationService);
			//Act
			var okResult = await controller.GetProjectsEndBeforeDate(end);
			var notFoundResult = await controller.GetProjectsEndBeforeDate(endEmptyResult);
			//Assert
			okResult.Should().NotBeNull();
			okResult.Should().BeOfType(typeof(OkObjectResult));
			notFoundResult.Should().NotBeNull();
			notFoundResult.Should().BeOfType(typeof(NotFoundObjectResult));
		}


		[Fact]
		public async void ProjectController_CreateProject_ReturnsNoContent()
		{
			//Arrange
			DateTime startDate = new DateTime(2020, 3, 4);
			DateTime endDate = new DateTime(2021, 3, 4);
			//fake valid Dto and project
			var projectDtoCorrect = A.Fake<ProjectDto>();
			projectDtoCorrect.StartDate = startDate;
			projectDtoCorrect.CompletionDate = endDate;
			var projectCorrect = A.Fake<Project>();
			A.CallTo(() => _mapper.Map<Project>(projectDtoCorrect)).Returns(projectCorrect);
			A.CallTo(() => _checkProjectRepository.ProjectNameAlreadyTakenAsync(projectCorrect)).Returns(false);
			A.CallTo(() => _projectRepository.CreateProject(projectCorrect)).Returns(true);
			//fake project Dto and project not passing the validation
			var projectDtoInvalid = A.Fake<ProjectDto>();
			var projectInvalid = A.Fake<Project>();
			A.CallTo(() => _mapper.Map<Project>(projectDtoInvalid)).Returns(projectInvalid);
			A.CallTo(() => _projectValidationService.ProjectIsValidAsync(projectInvalid)).Throws(new InvalidProjectDatesException("The dates are invalid"));
			var controller = new ProjectController(_projectRepository, _mapper, _taskRepository, _checkProjectRepository, _projectValidationService);
			//Act
			var noContentResult = await controller.CreateProject(projectDtoCorrect);
			var badRequestResult = await controller.CreateProject(projectDtoInvalid);
			//Assert
			noContentResult.Should().NotBeNull();
			noContentResult.Should().BeOfType(typeof(NoContentResult));
			badRequestResult.Should().NotBeNull();
			badRequestResult.Should().BeOfType(typeof(BadRequestObjectResult));
		}

		[Fact]
		public async void ProjectController_UpdateProject_ReturnsNoContent()
		{
			//Arrange
			Guid projectIdExists = new();
			DateTime startDate = new DateTime(2020, 3, 4);
			DateTime endDate = new DateTime(2021, 3, 4);
			//fake respond project does not exist
			Guid projectIdNotExists = Guid.NewGuid();
			A.CallTo(() => _checkProjectRepository.ProjectExistsAsync(projectIdNotExists)).Returns(false);
			//fake valid Dto and project
			var projectDtoCorrect = A.Fake<ProjectDto>();
			var projectCorrect = A.Fake<Project>();
			A.CallTo(() => _checkProjectRepository.ProjectExistsAsync(projectIdExists)).Returns(true);
			A.CallTo(() => _mapper.Map<Project>(projectDtoCorrect)).Returns(projectCorrect);
			A.CallTo(() => _projectValidationService.ProjectIsValidAsync(projectCorrect)).Returns(true);
			A.CallTo(() => _projectRepository.UpdateProject(projectCorrect)).Returns(true);
			//fake invalid Dto
			var projectDtoInvalid = A.Fake<ProjectDto>();
			var projectInvalid = A.Fake<Project>();
			A.CallTo(() => _mapper.Map<Project>(projectDtoInvalid)).Returns(projectInvalid);
			A.CallTo(() => _projectValidationService.ProjectIsValidAsync(projectInvalid)).Throws(
				new InvalidProjectDatesException("Invalid start/completion dates"));
			var controller = new ProjectController(_projectRepository, _mapper, _taskRepository, _checkProjectRepository, _projectValidationService);
			//Act
			var noContentResult = await controller.UpdateProject(projectIdExists, projectDtoCorrect);
			var badRequestResult = await controller.UpdateProject(projectIdExists, projectDtoInvalid);
			var notFoundResult = await controller.UpdateProject(projectIdNotExists, projectDtoCorrect);
			//Assert
			noContentResult.Should().NotBeNull();
			noContentResult.Should().BeOfType(typeof(NoContentResult));
			badRequestResult.Should().NotBeNull();
			badRequestResult.Should().BeOfType(typeof(BadRequestObjectResult));
			notFoundResult.Should().NotBeNull();
			notFoundResult.Should().BeOfType(typeof(NotFoundObjectResult));
		}

		[Fact]
		public async void ProjectCOntroller_DeleteProject_ReturnsNoContent()
		{
			//Arrange
			Guid projectIdExist = new();
			Guid projectIdNotExist = Guid.NewGuid();
			Guid projectIdNoTasks = new();
			//fake project with tasks
			var projectWithTasks = A.Fake<Project>();
			var task = A.Fake<ProjectTask>();
			var tasksPopulated = A.CollectionOfFake<ProjectTask>(0);
			tasksPopulated.Add(task);
			//fake project without tasks
			var projectWithoutTasks = A.Fake<Project>();
			var tasksEmpty = A.CollectionOfFake<ProjectTask>(0);
			//fake return of project with tasks
			A.CallTo(() => _checkProjectRepository.ProjectExistsAsync(projectIdExist)).Returns(true);
			A.CallTo(() => _projectRepository.GetProjectByIdAsync(projectIdExist)).Returns(projectWithTasks);
			A.CallTo(() => _taskRepository.DeleteProjectTask(task)).Returns(true);
			A.CallTo(() => _projectRepository.DeleteProject(projectWithTasks)).Returns(true);
			//fake return project does not exist
			A.CallTo(() => _checkProjectRepository.ProjectExistsAsync(projectIdNotExist)).Returns(false);
			//fake returns project without tasks
			A.CallTo(() => _checkProjectRepository.ProjectExistsAsync(projectIdNoTasks)).Returns(true);
			A.CallTo(() => _projectRepository.GetProjectByIdAsync(projectIdNoTasks)).Returns(projectWithoutTasks);
			A.CallTo(() => _projectRepository.DeleteProject(projectWithoutTasks)).Returns(true);
			var controller = new ProjectController(_projectRepository, _mapper, _taskRepository, _checkProjectRepository, _projectValidationService);
			//Act
			var noContentResult = await controller.DeleteProject(projectIdExist);
			var nocontentResuultNoTasks = await controller.DeleteProject(projectIdNoTasks);
			var notFoundResult = await controller.DeleteProject(projectIdNotExist);
			//Assert
			noContentResult.Should().NotBeNull();
			noContentResult.Should().BeOfType(typeof(NoContentResult));
			nocontentResuultNoTasks.Should().NotBeNull();
			nocontentResuultNoTasks.Should().BeOfType(typeof(NoContentResult));
			notFoundResult.Should().NotBeNull();
			notFoundResult.Should().BeOfType(typeof(NotFoundResult));
		}

	}
}
