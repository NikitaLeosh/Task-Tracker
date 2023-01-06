using AutoMapper;
using AutoMapper.Configuration.Annotations;
using FakeItEasy;
using FluentAssertions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ActionConstraints;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaskManager.Controllers;
using TaskManager.Dto;
using TaskManager.Exceptions;
using TaskManager.Interfaces;
using TaskManager.Models;
using TaskManager.Models.Enum;
using TaskManager.Repositories;

namespace TaskManager.Tests.Controller
{
	public class ProjectTaskControllerTests
	{
		private readonly IProjectRepository _projectRepository;
		private readonly IMapper _mapper;
		private readonly IProjectTaskRepository _taskRepository;
		private readonly ICheckProjectRepository _checkProjectRepository;
		private readonly ICheckTaskRepository _checkTaskRepository;
		private readonly ITaskValidationService _taskValidationService;
		public ProjectTaskControllerTests()
		{
			_projectRepository = A.Fake<IProjectRepository>();
			_mapper = A.Fake<IMapper>();
			_taskRepository = A.Fake<IProjectTaskRepository>();
			_checkProjectRepository = A.Fake<ICheckProjectRepository>();
			_checkTaskRepository = A.Fake<ICheckTaskRepository>();
			_taskValidationService = A.Fake<ITaskValidationService>();
		}
		[Fact]
		public async void ProjectTaskController_GetAllTasks_ReturnsOk()
		{

			//Arrange for normal result
			//create populated collections
			var tasksPopulated = A.CollectionOfFake<ProjectTask>(1);
			var tasksMapPopulated = A.Fake<List<TaskNoNavigationPropsDto>>();
			tasksMapPopulated.Add(A.Fake<TaskNoNavigationPropsDto>());
			//fake return of collection
			A.CallTo(() => _taskRepository.GetAllTasksAsync()).Returns(tasksPopulated);
			//fake return of populated DTO collection
			A.CallTo(() => _mapper.Map<List<TaskNoNavigationPropsDto>>(tasksPopulated)).Returns(tasksMapPopulated);
			var controller = new ProjectTaskController(_taskRepository, _projectRepository, _checkTaskRepository, _checkProjectRepository, _mapper, _taskValidationService);
			//Act
			var okResult = await controller.GetAllTasks();
			//Assert
			okResult.Should().NotBeNull();
			okResult.Should().BeOfType(typeof(OkObjectResult));

			//Arrange for NotFound result
			//create empty collections
			var tasksEmpty = A.CollectionOfFake<ProjectTask>(0);
			var tasksMapEmpty = A.Fake<List<TaskNoNavigationPropsDto>>();
			//fake return of collection
			A.CallTo(() => _taskRepository.GetAllTasksAsync()).Returns(tasksEmpty);
			//fake return of empty DTO collection
			A.CallTo(() => _mapper.Map<List<TaskNoNavigationPropsDto>>(tasksEmpty)).Returns(tasksMapEmpty);
			//Act
			var notFoundResult = await controller.GetAllTasks();
			//Assert
			notFoundResult.Should().NotBeNull();
			notFoundResult.Should().BeOfType(typeof(NotFoundResult));
		}
		[Fact]
		public async void ProjectTaskController_GetTaskById_ReturnsOk()
		{
			//Arrange
			Guid taskIdExists = Guid.NewGuid();
			Guid taskIdNotExists = Guid.NewGuid();
			var task = A.Fake<ProjectTask>();
			var taskMap = A.Fake<TaskNoNavigationPropsDto>();
			A.CallTo(() => _checkTaskRepository.ProjectTaskExistsAsync(taskIdExists)).Returns(Task.FromResult(true));
			A.CallTo(() => _checkTaskRepository.ProjectTaskExistsAsync(taskIdNotExists)).Returns(Task.FromResult(false));
			A.CallTo(() => _mapper.Map<TaskNoNavigationPropsDto>(task)).Returns(taskMap);
			var controller = new ProjectTaskController(_taskRepository, _projectRepository, _checkTaskRepository, _checkProjectRepository, _mapper, _taskValidationService);

			//Act
			var okResult = await controller.GetTaskById(taskIdExists);
			var notFoundResult = await controller.GetTaskById(taskIdNotExists);
			//Assert
			okResult.Should().NotBeNull();
			okResult.Should().BeOfType(typeof(OkObjectResult));
			notFoundResult.Should().NotBeNull();
			notFoundResult.Should().BeOfType(typeof(NotFoundResult));
		}
		[Fact]
		public async void ProjecrTaskController_GetTaskByName_ReturnsOk()
		{
			//Arrange
			var existingTaskName = "name";
			var notExistingTaskName = "name1";
			var task = A.Fake<ProjectTask>();
			var taskMap = new TaskNoNavigationPropsDto();
			A.CallTo(() => _taskRepository.GetTaskByNameAsync(existingTaskName)).Returns(Task.FromResult(task));
			A.CallTo(() => _taskRepository.GetTaskByNameAsync(notExistingTaskName)).Returns(Task.FromResult<ProjectTask>(null));
			var controller = new ProjectTaskController(_taskRepository, _projectRepository, _checkTaskRepository, _checkProjectRepository, _mapper, _taskValidationService);
			//Act
			var okResult = await controller.GetTaskByName(existingTaskName);
			var notFoundResult = await controller.GetTaskByName(notExistingTaskName);
			//Assert
			okResult.Should().NotBeNull();
			okResult.Should().BeOfType(typeof(OkObjectResult));
			notFoundResult.Should().NotBeNull();
			notFoundResult.Should().BeOfType(typeof(NotFoundObjectResult));
		}
		[Fact]
		public async void ProjectController_GetTasksOfAProject_ReturnsOk()
		{
			//Arrange
			Guid projectIdNormal = new();
			Guid projectIdNotExist = Guid.NewGuid();
			Guid projectIdNoTasks = Guid.NewGuid();
			//fake populated collections
			var tasksFound = A.CollectionOfFake<ProjectTask>(1);
			var tasksMapFound = A.Fake<List<ProjectTaskDto>>();
			tasksMapFound.Add(A.Fake<ProjectTaskDto>());
			//fake empty collections
			var tasksNotFound = A.CollectionOfFake<ProjectTask>(0);
			var taskMapNotFound = A.Fake<List<ProjectTaskDto>>();
			A.CallTo(() => _checkProjectRepository.ProjectExistsAsync(projectIdNormal)).Returns(true);
			A.CallTo(() => _checkProjectRepository.ProjectExistsAsync(projectIdNotExist)).Returns(false);
			A.CallTo(() => _checkProjectRepository.ProjectExistsAsync(projectIdNoTasks)).Returns(true);
			A.CallTo(() => _taskRepository.GetTasksOfAProjectAsync(projectIdNormal)).Returns(tasksFound);
			A.CallTo(() => _taskRepository.GetTasksOfAProjectAsync(projectIdNoTasks)).Returns(tasksNotFound);
			A.CallTo(() => _mapper.Map<List<ProjectTaskDto>>(tasksFound)).Returns(tasksMapFound);
			A.CallTo(() => _mapper.Map<List<ProjectTaskDto>>(tasksNotFound)).Returns(taskMapNotFound);
			var controller = new ProjectTaskController(_taskRepository, _projectRepository, _checkTaskRepository, _checkProjectRepository, _mapper, _taskValidationService);

			//Act
			var okResult = await controller.GetTasksOfAProject(projectIdNormal);
			var projectNotFoundResult = await controller.GetTasksOfAProject(projectIdNotExist);
			var noTasksFoundResult = await controller.GetTasksOfAProject(projectIdNoTasks);
			//Assert
			okResult.Should().NotBeNull();
			okResult.Should().BeOfType(typeof(OkObjectResult));
			projectNotFoundResult.Should().NotBeNull();
			projectNotFoundResult.Should().BeOfType(typeof(NotFoundObjectResult));
			noTasksFoundResult.Should().NotBeNull();
			noTasksFoundResult.Should().BeOfType(typeof(NotFoundObjectResult));
		}

		[Fact]
		public async void ProjectTaskController_GetTasksInPriorityRange_RetusnsOk()
		{
			int priorityLow = 1;
			int priorityHigh = 2;
			int priorityInvalid = 3;
			//fake populated collections
			var tasksPopulated = A.CollectionOfFake<ProjectTask>(1);
			var tasksDtoPopulated = A.Fake<List<TaskNoNavigationPropsDto>>();
			tasksDtoPopulated.Add(A.Fake<TaskNoNavigationPropsDto>());
			//fake empty collections
			var tasksEmpty = A.CollectionOfFake<ProjectTask>(0);
			var tasksDtoEmpty = A.Fake<List<TaskNoNavigationPropsDto>>();
			//fake return positive priority validation
			A.CallTo(() => _taskValidationService.PriorityIsValid(priorityLow)).Returns(true);
			A.CallTo(() => _taskValidationService.PriorityIsValid(priorityHigh)).Returns(true);
			A.CallTo(() => _taskValidationService.PriorityIsValid(priorityInvalid)).Returns(false);
			//fake return populated collections
			A.CallTo(() => _taskRepository.GetTasksPriorityRangeAsync(priorityLow, priorityHigh)).Returns(tasksPopulated);
			A.CallTo(() => _mapper.Map<List<TaskNoNavigationPropsDto>>(tasksPopulated)).Returns(tasksDtoPopulated);
			//fake return empty collections by switching priority values
			A.CallTo(() => _taskRepository.GetTasksPriorityRangeAsync(priorityHigh, priorityLow)).Returns(tasksEmpty);
			A.CallTo(() => _mapper.Map<List<TaskNoNavigationPropsDto>>(tasksEmpty)).Returns(tasksDtoEmpty);
			var controller = new ProjectTaskController(_taskRepository, _projectRepository, _checkTaskRepository, _checkProjectRepository, _mapper, _taskValidationService);

			//Act
			var okResult = await controller.GetTasksInPriorityRange(priorityLow, priorityHigh);
			var notFoundResult = await controller.GetTasksInPriorityRange(priorityHigh, priorityLow);
			var badRequestResult1 = await controller.GetTasksInPriorityRange(priorityLow, priorityInvalid);
			//assert
			okResult.Should().NotBeNull();
			okResult.Should().BeOfType(typeof(OkObjectResult));
			notFoundResult.Should().NotBeNull();
			notFoundResult.Should().BeOfType(typeof(NotFoundObjectResult));
			badRequestResult1.Should().NotBeNull();
			badRequestResult1.Should().BeOfType(typeof(BadRequestObjectResult));
		}
		[Fact]
		public async void ProjectTaskController_GetTasksWithStatus_ReturnsOk()
		{
			//Arrange
			var status = ProjectTaskStatus.ToDo;
			var statusEmptyResult = ProjectTaskStatus.InProgress;
			//creating populated collections
			var tasksPopulated = A.CollectionOfFake<ProjectTask>(1);
			var tasksDtoPopulated = A.Fake<List<TaskNoNavigationPropsDto>>();
			tasksDtoPopulated.Add(A.Fake<TaskNoNavigationPropsDto>());
			//fake empty collections
			var tasksEmpty = A.CollectionOfFake<ProjectTask>(0);
			var tasksDtoEmpty = A.Fake<List<TaskNoNavigationPropsDto>>();
			//fake return populated collections
			A.CallTo(() => _taskRepository.GetTasksWithStatusAsync(status)).Returns(tasksPopulated);
			A.CallTo(() => _mapper.Map<List<TaskNoNavigationPropsDto>>(tasksPopulated)).Returns(tasksDtoPopulated);
			//fake return empty collections
			A.CallTo(() => _taskRepository.GetTasksWithStatusAsync(statusEmptyResult)).Returns(tasksEmpty);
			A.CallTo(() => _mapper.Map<List<TaskNoNavigationPropsDto>>(tasksEmpty)).Returns(tasksDtoEmpty);
			var controller = new ProjectTaskController(_taskRepository, _projectRepository, _checkTaskRepository, _checkProjectRepository, _mapper, _taskValidationService);

			//Act
			var okResult = await controller.GetTasksWithStatus(status);
			var notFoundResult = await controller.GetTasksWithStatus(statusEmptyResult);

			//Assert
			okResult.Should().NotBeNull();
			okResult.Should().BeOfType(typeof(OkObjectResult));
			notFoundResult.Should().NotBeNull();
			notFoundResult.Should().BeOfType(typeof(NotFoundObjectResult));
		}
		[Fact]
		public async void ProjectTaskController_CreateTask_RetusnsNoContent()
		{
			//Arrange
			Guid projectId = new();
			Guid projectIdInvalid = Guid.NewGuid();
			var projectTaskDto = A.Fake<ProjectTaskDto>();
			var projectTask = A.Fake<ProjectTask>();
			var projectTaskDtoErrorWhileSave = A.Fake<ProjectTaskDto>();
			var projectTaskErrorWhileSave = A.Fake<ProjectTask>();
			//fake returns of tasks from DTOm by using mapper
			A.CallTo(() => _mapper.Map<ProjectTask>(projectTaskDto)).Returns(projectTask);
			A.CallTo(() => _mapper.Map<ProjectTask>(projectTaskDtoErrorWhileSave)).Returns(projectTaskErrorWhileSave);
			//fake return of positive task validation
			A.CallTo(() => _taskValidationService.TaskIsValidAsync(projectTask, projectId)).Returns(true);
			A.CallTo(() => _taskValidationService.TaskIsValidAsync(projectTaskErrorWhileSave, projectId)).Returns(true);
			//fake return of task validation with exception
			A.CallTo(() => _taskValidationService.TaskIsValidAsync(projectTask, projectIdInvalid)).Throws(
				new ObjectNameAlreadyTakenException("task name taken"));
			A.CallTo(() => _taskRepository.CreateProjectTask(projectTask)).Returns(true);
			A.CallTo(() => _taskRepository.CreateProjectTask(projectTaskErrorWhileSave)).Returns(false);
			var controller = new ProjectTaskController(_taskRepository, _projectRepository, _checkTaskRepository, _checkProjectRepository, _mapper, _taskValidationService);
			//Act
			var okResult = await controller.CreateTask(projectId, projectTaskDto);
			var BadRequestResult = await controller.CreateTask(projectIdInvalid, projectTaskDto);
			var errorWhileSaveResult = await controller.CreateTask(projectId, projectTaskDtoErrorWhileSave);
			//Assert
			okResult.Should().NotBeNull();
			okResult.Should().BeOfType(typeof(NoContentResult));
			BadRequestResult.Should().NotBeNull();
			BadRequestResult.Should().BeOfType(typeof(BadRequestObjectResult));
			errorWhileSaveResult.Should().NotBeNull();
			errorWhileSaveResult.Should().BeOfType(typeof(ObjectResult));
		}
		[Fact]
		public async void ProjectTaskController_UpdateTask_ReturnsNoContent()
		{
			//Arrange
			Guid projectOneId = Guid.NewGuid();
			Guid projectTwoId = Guid.NewGuid();
			Guid taskProjectOneId = Guid.NewGuid();
			Guid taskProjectTwoIdTaskNotExist = Guid.NewGuid();
			var taskUpdate = A.Fake<ProjectTask>();
			var taskUpdateDto = A.Fake<ProjectTaskDto>();
			var taskDtoInvalid = A.Fake<ProjectTaskDto>();
			var taskInvalid = A.Fake<ProjectTask>();
			//fake return of task model to update
			A.CallTo(() => _mapper.Map<ProjectTask>(taskUpdateDto)).Returns(taskUpdate);
			A.CallTo(() => _mapper.Map<ProjectTask>(taskDtoInvalid)).Returns(taskInvalid);
			//fake returns about existance of tasks
			A.CallTo(() => _checkTaskRepository.ProjectTaskExistsAsync(taskProjectOneId)).Returns(true);
			A.CallTo(() => _checkTaskRepository.ProjectTaskExistsAsync(taskProjectTwoIdTaskNotExist)).Returns(false);
			//fake validation of tasks
			A.CallTo(() => _taskValidationService.TaskIsValidAsync(taskUpdate, projectOneId)).Returns(true);
			A.CallTo(() => _taskValidationService.TaskIsValidAsync(taskUpdate, projectTwoId)).Returns(true);
			A.CallTo(() => _taskValidationService.TaskIsValidAsync(taskInvalid, projectOneId)).
				Throws<ObjectNameAlreadyTakenException>();
			//fake returns about task belonging to a project
			A.CallTo(() => _checkTaskRepository.TaskBelongsToProjectNoTrackingAsync(taskProjectOneId, projectOneId)).Returns(true);
			A.CallTo(() => _checkTaskRepository.TaskBelongsToProjectNoTrackingAsync(taskProjectOneId, projectTwoId)).Returns(false);
			//fake return update is successful
			A.CallTo(() => _taskRepository.UpdateProjectTask(taskUpdate)).Returns(true);
			var controller = new ProjectTaskController(_taskRepository, _projectRepository, _checkTaskRepository, _checkProjectRepository, _mapper, _taskValidationService);
			//Act
			var okResult = await controller.UpdateTask(projectOneId, taskProjectOneId, taskUpdateDto);
			var taskFromDiffProjResult = await controller.UpdateTask(projectTwoId, taskProjectOneId, taskUpdateDto);
			var notFoundResult = await controller.UpdateTask(projectOneId, taskProjectTwoIdTaskNotExist, taskUpdateDto);
			var badRequestResult = await controller.UpdateTask(projectOneId, taskProjectOneId, taskDtoInvalid);
			//Assert
			okResult.Should().NotBeNull();
			okResult.Should().BeOfType(typeof(NoContentResult));
			notFoundResult.Should().NotBeNull();
			notFoundResult.Should().BeOfType(typeof(NotFoundObjectResult));
			badRequestResult.Should().NotBeNull();
			badRequestResult.Should().BeOfType(typeof(BadRequestObjectResult));
			taskFromDiffProjResult.Should().NotBeNull();
			taskFromDiffProjResult.Should().BeOfType(typeof(UnprocessableEntityObjectResult));
		}
		[Fact]
		public async void ProjectTaskController_DeleteTask_ReturnsNoContent()
		{
			//Arrange
			Guid projectTaskExistsId = Guid.NewGuid();
			Guid projectTaskNotExistId = Guid.NewGuid();
			Guid projectTaskCanNotDeleteId = Guid.NewGuid();
			var projectTask = A.Fake<ProjectTask>();
			var projectTaskCanNotDelete = A.Fake<ProjectTask>();
			A.CallTo(() => _checkTaskRepository.ProjectTaskExistsAsync(projectTaskExistsId)).Returns(true);
			A.CallTo(() => _checkTaskRepository.ProjectTaskExistsAsync(projectTaskNotExistId)).Returns(false);
			A.CallTo(() => _checkTaskRepository.ProjectTaskExistsAsync(projectTaskCanNotDeleteId)).Returns(true);
			A.CallTo(() => _taskRepository.GetTaskByIdAsync(projectTaskExistsId)).Returns(projectTask);
			A.CallTo(() => _taskRepository.GetTaskByIdAsync(projectTaskCanNotDeleteId)).Returns(projectTaskCanNotDelete);
			A.CallTo(() => _taskRepository.DeleteProjectTask(projectTask)).Returns(true);
			A.CallTo(() => _taskRepository.DeleteProjectTask(projectTaskCanNotDelete)).Returns(false);
			var controller = new ProjectTaskController(_taskRepository, _projectRepository, _checkTaskRepository, _checkProjectRepository, _mapper, _taskValidationService);

			//Act
			var noContentResult = await controller.DeleteTask(projectTaskExistsId);
			var notFoundResult = await controller.DeleteTask(projectTaskNotExistId);
			var canNotDeleteResult = await controller.DeleteTask(projectTaskCanNotDeleteId);

			//Assert
			noContentResult.Should().NotBeNull();
			noContentResult.Should().BeOfType(typeof(NoContentResult));
			notFoundResult.Should().NotBeNull();
			notFoundResult.Should().BeOfType(typeof(NotFoundResult));
			canNotDeleteResult.Should().NotBeNull();
			canNotDeleteResult.Should().BeOfType(typeof(ObjectResult));

		}
	}
}
