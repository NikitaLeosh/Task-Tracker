using AutoMapper;
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
using TaskManager.Data.Enum;
using TaskManager.Dto;
using TaskManager.Interfaces;
using TaskManager.Models;

namespace TaskManager.Tests.Controller
{
	public class ProjectTaskControllerTests
	{
		private readonly IProjectRepository _projectRepository;
		private readonly IMapper _mapper;
		private readonly IProjectTaskRepository _taskRepository;
		public ProjectTaskControllerTests()
		{
			_projectRepository = A.Fake<IProjectRepository>();
			_mapper = A.Fake<IMapper>();
			_taskRepository = A.Fake<IProjectTaskRepository>();
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
			var controller = new ProjectTaskController(_taskRepository, _projectRepository, _mapper);
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
			int taskIdExists = 1;
			int taskIdNotExists = 2;
			var task = A.Fake<ProjectTask>();
			var taskMap = A.Fake<TaskNoNavigationPropsDto>();
			A.CallTo(() => _taskRepository.ProjectTaskExistsAsync(taskIdExists)).Returns(Task.FromResult(true));
			A.CallTo(() => _taskRepository.ProjectTaskExistsAsync(taskIdNotExists)).Returns(Task.FromResult(false));
			A.CallTo(() => _mapper.Map<TaskNoNavigationPropsDto>(task)).Returns(taskMap);
			var controller = new ProjectTaskController(_taskRepository, _projectRepository, _mapper);

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
			var controller = new ProjectTaskController(_taskRepository, _projectRepository, _mapper);
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
		public async void ProjectTaskController_GetTasksInPriorityRange_RetusnsOk()
		{
			int priorityLow = 10;
			int priorityHigh = 70;
			int priorityEmptyResult = 60;
			int priorityInvalid = 101;
			//fake populated collections
			var tasksPopulated = A.CollectionOfFake<ProjectTask>(1);
			var tasksDtoPopulated = A.Fake<List<TaskNoNavigationPropsDto>>();
			tasksDtoPopulated.Add(A.Fake<TaskNoNavigationPropsDto>());
			//fake empty collections
			var tasksEmpty = A.CollectionOfFake<ProjectTask>(0);
			var tasksDtoEmpty = A.Fake<List<TaskNoNavigationPropsDto>>();
			//fake return populated collections
			A.CallTo(() => _taskRepository.GetTasksPriorityRangeAsync(priorityLow, priorityHigh)).Returns(tasksPopulated);
			A.CallTo(() => _mapper.Map<List<TaskNoNavigationPropsDto>>(tasksPopulated)).Returns(tasksDtoPopulated);
			//fake return empty collections
			A.CallTo(() => _taskRepository.GetTasksPriorityRangeAsync(priorityLow, priorityEmptyResult)).Returns(tasksEmpty);
			A.CallTo(() => _mapper.Map<List<TaskNoNavigationPropsDto>>(tasksEmpty)).Returns(tasksDtoEmpty);
			var controller = new ProjectTaskController(_taskRepository, _projectRepository, _mapper);

			//Act
			var okResult = await controller.GetTasksInPriorityRange(priorityLow, priorityHigh);
			var notFoundResult = await controller.GetTasksInPriorityRange(priorityLow, priorityEmptyResult);
			var badRequestResult1 = await controller.GetTasksInPriorityRange(priorityHigh, priorityLow);
			var badRequestResult2 = await controller.GetTasksInPriorityRange(priorityLow, priorityInvalid);

			//assert
			okResult.Should().NotBeNull();
			okResult.Should().BeOfType(typeof(OkObjectResult));
			notFoundResult.Should().NotBeNull();
			notFoundResult.Should().BeOfType(typeof(NotFoundObjectResult));
			badRequestResult1.Should().NotBeNull();
			badRequestResult1.Should().BeOfType(typeof(BadRequestObjectResult));
			badRequestResult2.Should().NotBeNull();
			badRequestResult2.Should().BeOfType(typeof(BadRequestObjectResult));
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
			var controller = new ProjectTaskController(_taskRepository, _projectRepository, _mapper);

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
			int projectIdExist = 1;
			int projectIdNotExist = 2;
			int projectIdNameTaken = 3;
			var projectTaskDto = A.Fake<ProjectTaskDto>();
			var projectTask = A.Fake<ProjectTask>();
			//fake returns projects exist
			A.CallTo(() => _projectRepository.ProjectExistsAsync(projectIdExist)).Returns(true);
			A.CallTo(() => _projectRepository.ProjectExistsAsync(projectIdNameTaken)).Returns(true);
			//fake return project does not exist
			A.CallTo(() => _projectRepository.ProjectExistsAsync(projectIdNotExist)).Returns(false);
			//fake returns of tasks from DTOm by using mapper
			A.CallTo(() => _mapper.Map<ProjectTask>(projectTaskDto)).Returns(projectTask);
			//fake return name is available
			A.CallTo(() => _taskRepository.TaskNameAlreadyTakenAsync(projectTask, projectIdExist)).Returns(false);
			A.CallTo(() => _taskRepository.CreateProjectTask(projectTask)).Returns(true);
			//fake return name is taken
			A.CallTo(() => _taskRepository.TaskNameAlreadyTakenAsync(projectTask, projectIdNameTaken)).Returns(true);
			var controller = new ProjectTaskController(_taskRepository, _projectRepository, _mapper);
			//Act
			var okResult = await controller.CreateTask(projectIdExist, projectTaskDto);
			var notFoundResult = await controller.CreateTask(projectIdNotExist, projectTaskDto);
			var nameTakenResult = await controller.CreateTask(projectIdNameTaken, projectTaskDto);
			//Assert
			okResult.Should().NotBeNull();
			okResult.Should().BeOfType(typeof(NoContentResult));
			notFoundResult.Should().NotBeNull();
			notFoundResult.Should().BeOfType(typeof(NotFoundResult));
			nameTakenResult.Should().NotBeNull();
			nameTakenResult.Should().BeOfType(typeof(ObjectResult));
		}
		[Fact]
		public async void ProjectTaskController_UpdateTask_ReturnsNoContent()
		{
			//Arrange
			int projectOneId = 1;
			int projectTwoId = 2;
			int projectNameTakenId = 3;
			int projectNotExistId = 4;
			int projectOneTaskId = 5;
			var taskUpdate = A.Fake<ProjectTask>();
			var taskUpdateDto = A.Fake<ProjectTaskDto>();
			//fake returns about existance
			A.CallTo(() => _projectRepository.ProjectExistsAsync(projectOneId)).Returns(true);
			A.CallTo(() => _projectRepository.ProjectExistsAsync(projectTwoId)).Returns(true);
			A.CallTo(() => _projectRepository.ProjectExistsAsync(projectNameTakenId)).Returns(true);
			A.CallTo(() => _projectRepository.ProjectExistsAsync(projectNotExistId)).Returns(false);
			A.CallTo(() => _taskRepository.ProjectTaskExistsAsync(projectOneTaskId)).Returns(true);
			//fake returns about task belonging to a project
			A.CallTo(() => _taskRepository.TaskBelongsToProjectNoTrackingAsync(projectOneTaskId, projectOneId)).Returns(true);
			A.CallTo(() => _taskRepository.TaskBelongsToProjectNoTrackingAsync(projectOneTaskId, projectTwoId)).Returns(false);
			A.CallTo(() => _taskRepository.TaskBelongsToProjectNoTrackingAsync(projectOneTaskId, projectNameTakenId)).Returns(true);

			//fake return of task model to update
			A.CallTo(() => _mapper.Map<ProjectTask>(taskUpdateDto)).Returns(taskUpdate);
			//fake return that name is available
			A.CallTo(() => _taskRepository.TaskNameAlreadyTakenAsync(taskUpdate, projectNameTakenId)).Returns(true);
			//fake return saving is ok
			A.CallTo(() => _taskRepository.TaskNameAlreadyTakenAsync(taskUpdate,projectOneId)).Returns(false);
			//fake return that name is taken
			
			A.CallTo(() => _taskRepository.UpdateProjectTask(taskUpdate)).Returns(true);
			var controller = new ProjectTaskController(_taskRepository, _projectRepository, _mapper);

			//Act

			var okResult = await controller.UpdateTask(projectOneId, projectOneTaskId, taskUpdateDto);
			var notFoundResult = await controller.UpdateTask(projectNotExistId, projectOneTaskId, taskUpdateDto);
			var nameTakenResult = await controller.UpdateTask(projectNameTakenId, projectOneTaskId, taskUpdateDto);
			var taskFromDiffProjResult = await controller.UpdateTask(projectTwoId, projectOneTaskId, taskUpdateDto);

			//Assert
			okResult.Should().NotBeNull();
			okResult.Should().BeOfType(typeof(NoContentResult));
			notFoundResult.Should().NotBeNull();
			notFoundResult.Should().BeOfType(typeof(NotFoundResult));
			nameTakenResult.Should().NotBeNull();
			nameTakenResult.Should().BeOfType(typeof(ObjectResult));
			taskFromDiffProjResult.Should().NotBeNull();
			taskFromDiffProjResult.Should().BeOfType(typeof(ObjectResult));
		}
		[Fact]
		public async void ProjectTaskController_DeleteTask_ReturnsNoContent()
		{
			//Arrange

			int projectTaskExistsId = 1;
			int projectTaskNotExistId = 2;
			int projectTaskCanNotDeleteId = 3;
			var projectTask = A.Fake<ProjectTask>();
			var projectTaskCanNotDelete = A.Fake<ProjectTask>();
			A.CallTo(() => _taskRepository.ProjectTaskExistsAsync(projectTaskExistsId)).Returns(true);
			A.CallTo(() => _taskRepository.ProjectTaskExistsAsync(projectTaskNotExistId)).Returns(false);
			A.CallTo(() => _taskRepository.ProjectTaskExistsAsync(projectTaskCanNotDeleteId)).Returns(true);
			A.CallTo(() => _taskRepository.GetTaskByIdAsync(projectTaskExistsId)).Returns(projectTask);
			A.CallTo(() => _taskRepository.GetTaskByIdAsync(projectTaskCanNotDeleteId)).Returns(projectTaskCanNotDelete);
			A.CallTo(() => _taskRepository.DeleteProjectTask(projectTask)).Returns(true);
			A.CallTo(() => _taskRepository.DeleteProjectTask(projectTaskCanNotDelete)).Returns(false);
			var controller = new ProjectTaskController(_taskRepository, _projectRepository, _mapper);

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
