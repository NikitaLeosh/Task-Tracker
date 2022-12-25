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
using TaskManager.Data.Enum;
using TaskManager.Dto;
using TaskManager.Interfaces;
using TaskManager.Models;

namespace TaskManager.Tests.Controller
{
	public class ProjectControllerTests
	{
		private readonly IProjectRepository _projectRepository;
		private readonly IMapper _mapper;
		private readonly IProjectTaskRepository _taskRepository;
		public ProjectControllerTests()
		{
			_projectRepository = A.Fake<IProjectRepository>();
			_mapper = A.Fake<IMapper>();
			_taskRepository = A.Fake<IProjectTaskRepository>();
		}
		[Fact]
		public void ProjectController_GetAllProjects_ReturnsOk()
		{
			//Arrange
			var projects = A.CollectionOfFake<Project>(1);
			var projectsMap = A.Fake<List<ProjectNoTasksDto>>();
			projectsMap.Add(new ProjectNoTasksDto
			{
				Id = 1,
				ProjectName = "asd",
				Priority = 3,
				ProjectStatus = ProjectStatus.Active

			});
			A.CallTo(() => _projectRepository.GetAllProjects()).Returns(projects);
			A.CallTo(() => _mapper.Map<List<ProjectNoTasksDto>>(projects)).Returns(projectsMap);
			var controller = new ProjectController(_projectRepository, _mapper, _taskRepository);
			//Act
			var result = controller.GetAllProjects();
			//Assert
			result.Should().NotBeNull();
			result.Should().BeOfType(typeof(OkObjectResult));
		}
		[Fact]
		public void ProjectController_GetProjectById_ReturnsOk()
		{
			//Arrange
			var projectId = 1;
			var project = A.Fake<Project>();
			project.Id = projectId;
			var projectMap = A.Fake<ProjectNoTasksDto>();
			A.CallTo(() => _mapper.Map<ProjectNoTasksDto>(project)).Returns(projectMap);
			A.CallTo(() => _projectRepository.ProjectExists(projectId)).Returns(true);
			var controller = new ProjectController(_projectRepository, _mapper, _taskRepository);

			//Act
			var result = controller.GetProjectById(projectId);

			//Assert
			result.Should().NotBeNull();
			result.Should().BeOfType(typeof(OkObjectResult));
		}
		[Fact]
		public void ProjectController_GetProjectByName_RetusnsOf()
		{
			//Arrange
			var projectName = "projectname";
			var project = A.Fake<Project>();
			var projectMap = A.Fake<ProjectNoTasksDto>();
			//project.ProjectName = projectName;
			A.CallTo(() => _mapper.Map<ProjectNoTasksDto>(project)).Returns(projectMap);
			var controller = new ProjectController(_projectRepository, _mapper, _taskRepository);
			//Act
			var result = controller.GetProjectByName(projectName);
			//Assert
			result.Should().NotBeNull();
			result.Should().BeOfType(typeof(OkObjectResult));
		}
		[Fact]
		public void ProjectController_GetProjectsWithStatus_ReturnsOk()
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
			A.CallTo(() => _projectRepository.GetProjectsWithStatus(status)).Returns(projects);
			A.CallTo(() => _projectRepository.GetProjectsWithStatus(statusEmptyResult)).Returns(projectsEmpty);
			var controller = new ProjectController(_projectRepository, _mapper, _taskRepository);
			//Act
			var result = controller.GetProjectsWithStatus(status);
			var notFoundResult = controller.GetProjectsWithStatus(statusEmptyResult);
			//Assert
			result.Should().NotBeNull();
			result.Should().BeOfType(typeof(OkObjectResult));
			notFoundResult.Should().NotBeNull();
			notFoundResult.Should().BeOfType(typeof(NotFoundObjectResult));
		}
		[Fact]
		public void ProjectController_GetProjectsInPriorityRange_ReturnsOk()
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
			A.CallTo(() => _projectRepository.GetProjectsPriorityRange(priorityLow, priorityHigh)).Returns(projects);
			A.CallTo(() => _mapper.Map<List<ProjectNoTasksDto>>(projects)).Returns(projectsMap);
			//logic to return empty result
			A.CallTo(() => _projectRepository.GetProjectsPriorityRange(priorityLow, priorityEmptyResult)).Returns(projectsEmpty);
			A.CallTo(() => _mapper.Map<List<ProjectNoTasksDto>>(projects)).Returns(projectsMap);
			var controller = new ProjectController(_projectRepository, _mapper, _taskRepository);
			//Act
			var positiveResult = controller.GetProjectsInPriorityRange(priorityLow, priorityHigh);
			var notFoundNegativeResult = controller.GetProjectsInPriorityRange(priorityLow, priorityEmptyResult);
			//low higher then high
			var negativeResult1 = controller.GetProjectsInPriorityRange(priorityHigh, priorityLow);
			//priority value out of range
			var negativeResult2 = controller.GetProjectsInPriorityRange(priorityLow, priorityInvalid);
			//Assert
			//normal
			positiveResult.Should().NotBeNull();
			positiveResult.Should().BeOfType(typeof(OkObjectResult));
			//empty collection returns - not found
			notFoundNegativeResult.Should().NotBeNull();
			notFoundNegativeResult.Should().BeOfType(typeof(NotFoundObjectResult));
			//invlid request - low priority is higher then high priority
			negativeResult1.Should().NotBeNull();
			negativeResult1.Should().BeOfType(typeof(BadRequestObjectResult));
			//invalid request - priority value is out of range
			negativeResult2.Should().NotBeNull();
			negativeResult2.Should().BeOfType(typeof(BadRequestObjectResult));

		}

		[Fact]
		public void ProjectController_GetProjectsStartInDatesRange_ReturnsOk()
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
			A.CallTo(() => _projectRepository.GetProjectsStartAtRange(start, end)).Returns(projects);
			A.CallTo(() => _mapper.Map<List<ProjectNoTasksDto>>(projects)).Returns(projectsMap);
			//logic to return empty collection
			A.CallTo(() => _projectRepository.GetProjectsStartAtRange(startEmpty, endEmpty)).Returns(projectsEmpty);
			A.CallTo(() => _mapper.Map<List<ProjectNoTasksDto>>(projectsEmpty)).Returns(projectsMapEmpty);
			var controller = new ProjectController(_projectRepository, _mapper, _taskRepository);
			//Act
			//normal
			var positiveResult = controller.GetProjectsStartInDatesRange(start, end);
			//empty result - not found
			var negativeResultEmpty = controller.GetProjectsStartInDatesRange(startEmpty, endEmpty);
			//incorrect input - start is later then finish
			var negativeResult = controller.GetProjectsStartInDatesRange(end, start);
			//Assert
			positiveResult.Should().NotBeNull();
			positiveResult.Should().BeOfType(typeof(OkObjectResult));
			negativeResult.Should().NotBeNull();
			negativeResult.Should().BeOfType(typeof(BadRequestObjectResult));
			negativeResultEmpty.Should().NotBeNull();
			negativeResultEmpty.Should().BeOfType(typeof(NotFoundObjectResult));
		}

		[Fact]
		public void ProjectController_GetProjectsStartAfterDate_returnsOk()
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
			A.CallTo(() => _projectRepository.GetProjectsStartAfter(start)).Returns(projects);
			A.CallTo(() => _mapper.Map<List<ProjectNoTasksDto>>(projects)).Returns(projectsMap);
			//logic to return empty collection
			A.CallTo(() => _projectRepository.GetProjectsStartAfter(startEmptyResult)).Returns(projectsEmpty);
			A.CallTo(() => _mapper.Map<List<ProjectNoTasksDto>>(projectsEmpty)).Returns(projectsMapEmpty);
			var controller = new ProjectController(_projectRepository, _mapper, _taskRepository);
			//Act
			var okResult = controller.GetProjectsStartAfterDate(start);
			var notFoundResult = controller.GetProjectsStartAfterDate(startEmptyResult);
			//Assert
			okResult.Should().NotBeNull();
			okResult.Should().BeOfType(typeof(OkObjectResult));
			notFoundResult.Should().NotBeNull();
			notFoundResult.Should().BeOfType(typeof(NotFoundObjectResult));
		}

		[Fact]
		public void ProjectController_GetProjectsEndBeforeDate_returnsOk()
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
			A.CallTo(() => _projectRepository.GetProjectsEndBefore(end)).Returns(projects);
			A.CallTo(() => _mapper.Map<List<ProjectNoTasksDto>>(projects)).Returns(projectsMap);
			//logic to return empty collection
			A.CallTo(() => _projectRepository.GetProjectsEndBefore(endEmptyResult)).Returns(projectsEmpty);
			A.CallTo(() => _mapper.Map<List<ProjectNoTasksDto>>(projectsEmpty)).Returns(projectsMapEmpty);
			var controller = new ProjectController(_projectRepository, _mapper, _taskRepository);
			//Act
			var okResult = controller.GetProjectsEndBeforeDate(end);
			var notFoundResult = controller.GetProjectsEndBeforeDate(endEmptyResult);
			//Assert
			okResult.Should().NotBeNull();
			okResult.Should().BeOfType(typeof(OkObjectResult));
			notFoundResult.Should().NotBeNull();
			notFoundResult.Should().BeOfType(typeof(NotFoundObjectResult));
		}
		[Fact]
		public void ProjectController_GetTasksOfAProject_ReturnsOk()
		{
			//Arrange
			int projectIdNormal = 1;
			int projectIdNotExist = 2;
			int projectIdNoTasks = 3;
			//fake populated collections
			var tasksFound = A.CollectionOfFake<ProjectTask>(1);
			var tasksMapFound = A.Fake<List<ProjectTaskDto>>();
			tasksMapFound.Add(A.Fake<ProjectTaskDto>());
			//fake empty collections
			var tasksNotFound = A.CollectionOfFake<ProjectTask>(0);
			var taskMapNotFound = A.Fake<List<ProjectTaskDto>>();
			A.CallTo(() => _projectRepository.ProjectExists(projectIdNormal)).Returns(true);
			A.CallTo(() => _projectRepository.ProjectExists(projectIdNotExist)).Returns(false);
			A.CallTo(() => _projectRepository.ProjectExists(projectIdNoTasks)).Returns(true);
			A.CallTo(() => _projectRepository.GetTasksOfAProject(projectIdNormal)).Returns(tasksFound);
			A.CallTo(() => _projectRepository.GetTasksOfAProject(projectIdNoTasks)).Returns(tasksNotFound);
			A.CallTo(() => _mapper.Map<List<ProjectTaskDto>>(tasksFound)).Returns(tasksMapFound);
			A.CallTo(() => _mapper.Map<List<ProjectTaskDto>>(tasksNotFound)).Returns(taskMapNotFound);
			var controller = new ProjectController(_projectRepository, _mapper, _taskRepository);

			//Act
			var okResult = controller.GetTasksOfAProject(projectIdNormal);
			var projectNotFoundResult = controller.GetTasksOfAProject(projectIdNotExist);
			var noTasksFoundResult = controller.GetTasksOfAProject(projectIdNoTasks);
			//Assert
			okResult.Should().NotBeNull();
			okResult.Should().BeOfType(typeof(OkObjectResult));
			projectNotFoundResult.Should().NotBeNull();
			projectNotFoundResult.Should().BeOfType(typeof(NotFoundObjectResult));
			noTasksFoundResult.Should().NotBeNull();
			noTasksFoundResult.Should().BeOfType(typeof(NotFoundObjectResult));
		}

		[Fact]
		public void ProjectController_CreateProject_ReturnsNoContent()
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
			A.CallTo(() => _projectRepository.ProjectNameAlreadyTaken(projectCorrect)).Returns(false);
			A.CallTo(() => _projectRepository.CreateProject(projectCorrect)).Returns(true);
			//fake invalid Dto
			var projectDtoIncorrectDates = A.Fake<ProjectDto>();
			projectDtoIncorrectDates.StartDate = endDate;
			projectDtoIncorrectDates.CompletionDate = startDate;
			//fake project Dto and project with used  name
			var projectDtoNameTaken = A.Fake<ProjectDto>();
			projectDtoNameTaken.StartDate = startDate;
			projectDtoNameTaken.CompletionDate = endDate;
			var projectNameTaken = A.Fake<Project>();
			A.CallTo(() => _mapper.Map<Project>(projectDtoNameTaken)).Returns(projectNameTaken);
			A.CallTo(() => _projectRepository.ProjectNameAlreadyTaken(projectNameTaken)).Returns(true);
			var controller = new ProjectController(_projectRepository, _mapper, _taskRepository);
			//Act
			var noContentResult = controller.CreateProject(projectDtoCorrect);
			var badRequestResult = controller.CreateProject(projectDtoIncorrectDates);
			var unprocessableEntityResult = controller.CreateProject(projectDtoNameTaken);
			//Assert
			noContentResult.Should().NotBeNull();
			noContentResult.Should().BeOfType(typeof(NoContentResult));
			badRequestResult.Should().NotBeNull();
			badRequestResult.Should().BeOfType(typeof(BadRequestObjectResult));
			unprocessableEntityResult.Should().NotBeNull();
			unprocessableEntityResult.Should().BeOfType(typeof(ObjectResult));
		}

		[Fact]
		public void ProjectController_UpdateProject_ReturnsNoContent()
		{
			//Arrange
			int projectIdExists = 1;
			DateTime startDate = new DateTime(2020, 3, 4);
			DateTime endDate = new DateTime(2021, 3, 4);
			//fake respond project does not exist
			int projectIdNotExists = 2;
			A.CallTo(() => _projectRepository.ProjectExists(projectIdNotExists)).Returns(false);
			//fake valid Dto and project
			var projectDtoCorrect = A.Fake<ProjectDto>();
			projectDtoCorrect.StartDate = startDate;
			projectDtoCorrect.CompletionDate = endDate;
			var projectCorrect = A.Fake<Project>();
			A.CallTo(() => _projectRepository.ProjectExists(projectIdExists)).Returns(true);
			A.CallTo(() => _mapper.Map<Project>(projectDtoCorrect)).Returns(projectCorrect);
			A.CallTo(() => _projectRepository.ProjectNameAlreadyTaken(projectCorrect)).Returns(false);
			A.CallTo(() => _projectRepository.UpdateProject(projectCorrect)).Returns(true);
			//fake invalid Dto
			var projectDtoIncorrectDates = A.Fake<ProjectDto>();
			projectDtoIncorrectDates.StartDate = endDate;
			projectDtoIncorrectDates.CompletionDate = startDate;
			//fake project Dto and project with a used  name
			var projectDtoNameTaken = A.Fake<ProjectDto>();
			projectDtoNameTaken.StartDate = startDate;
			projectDtoNameTaken.CompletionDate = endDate;
			var projectNameTaken = A.Fake<Project>();
			A.CallTo(() => _mapper.Map<Project>(projectDtoNameTaken)).Returns(projectNameTaken);
			A.CallTo(() => _projectRepository.ProjectNameAlreadyTaken(projectNameTaken)).Returns(true);
			var controller = new ProjectController(_projectRepository, _mapper, _taskRepository);
			//Act
			var noContentResult = controller.UpdateProject(projectIdExists, projectDtoCorrect);
			var badRequestResult = controller.UpdateProject(projectIdExists, projectDtoIncorrectDates);
			var unprocessableEntityResult = controller.UpdateProject(projectIdExists, projectDtoNameTaken);
			var notFoundResult = controller.UpdateProject(projectIdNotExists, projectDtoCorrect);
			//Assert
			noContentResult.Should().NotBeNull();
			noContentResult.Should().BeOfType(typeof(NoContentResult));
			badRequestResult.Should().NotBeNull();
			badRequestResult.Should().BeOfType(typeof(BadRequestObjectResult));
			unprocessableEntityResult.Should().NotBeNull();
			unprocessableEntityResult.Should().BeOfType(typeof(ObjectResult));
			notFoundResult.Should().NotBeNull();
			notFoundResult.Should().BeOfType(typeof(NotFoundResult));
		}

		[Fact]
		public void ProjectCOntroller_DeleteProject_ReturnsNoContent()
		{
			//Arrange
			int projectIdExist = 1;
			int projectIdNotExist = 2;
			int projectIdNoTasks = 3;
			//fake project with tasks
			var projectWithTasks = A.Fake<Project>();
			var task = A.Fake<ProjectTask>();
			var tasksPopulated = A.CollectionOfFake<ProjectTask>(0);
			tasksPopulated.Add(task);
			//fake project without tasks
			var projectWithoutTasks = A.Fake<Project>();
			var tasksEmpty = A.CollectionOfFake<ProjectTask>(0);
			//fake return of project with tasks
			A.CallTo(() => _projectRepository.ProjectExists(projectIdExist)).Returns(true);
			A.CallTo(() => _projectRepository.GetProjectById(projectIdExist)).Returns(projectWithTasks);
			A.CallTo(() => _taskRepository.DeleteProjectTask(task)).Returns(true);
			A.CallTo(() => _projectRepository.DeleteProject(projectWithTasks)).Returns(true);

			//fake return project does not exist
			A.CallTo(() => _projectRepository.ProjectExists(projectIdNotExist)).Returns(false);
			//fake returns project without tasks
			A.CallTo(() => _projectRepository.ProjectExists(projectIdNoTasks)).Returns(true);
			A.CallTo(() => _projectRepository.GetProjectById(projectIdNoTasks)).Returns(projectWithoutTasks);
			A.CallTo(() => _projectRepository.DeleteProject(projectWithoutTasks)).Returns(true);
			var controller = new ProjectController(_projectRepository, _mapper, _taskRepository);


			//Act
			var noContentResult = controller.DeleteProject(projectIdExist);
			var nocontentResuultNoTasks = controller.DeleteProject(projectIdNoTasks);
			var notFoundResult = controller.DeleteProject(projectIdNotExist);
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
