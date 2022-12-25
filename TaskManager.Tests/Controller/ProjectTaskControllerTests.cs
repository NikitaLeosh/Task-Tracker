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
		public void ProjectTaskController_GetAllTasks_ReturnsOk()
		{

			//Arrange for normal result
			//create populated collections
			var tasksPopulated = A.CollectionOfFake<ProjectTask>(1);
			var tasksMapPopulated = A.Fake<List<TaskNoNavigationPropsDto>>();
			tasksMapPopulated.Add(A.Fake<TaskNoNavigationPropsDto>());
			//fake return of collection
			A.CallTo(() => _taskRepository.GetAllTasks()).Returns(tasksPopulated);
			//fake return of populated DTO collection
			A.CallTo(() => _mapper.Map<List<TaskNoNavigationPropsDto>>(tasksPopulated)).Returns(tasksMapPopulated);
			var controller = new ProjectTaskController(_taskRepository, _projectRepository, _mapper);
			//Act
			var okResult = controller.GetAllTasks();
			//Assert
			okResult.Should().NotBeNull();
			okResult.Should().BeOfType(typeof(OkObjectResult));

			//Arrange for NotFound result
			//create empty collections
			var tasksEmpty = A.CollectionOfFake<ProjectTask>(0);
			var tasksMapEmpty = A.Fake<List<TaskNoNavigationPropsDto>>();
			//fake return of collection
			A.CallTo(() => _taskRepository.GetAllTasks()).Returns(tasksEmpty);
			//fake return of empty DTO collection
			A.CallTo(() => _mapper.Map<List<TaskNoNavigationPropsDto>>(tasksEmpty)).Returns(tasksMapEmpty);
			//Act
			var notFoundResult = controller.GetAllTasks();
			//Assert
			notFoundResult.Should().NotBeNull();
			notFoundResult.Should().BeOfType(typeof(NotFoundResult));
		}
		[Fact]
		public void ProjectTaskController_GetTaskById_ReturnsOk()
		{
			//Arrange
			int taskIdExists = 1;
			int taskIdNotExists = 2;
			var task = A.Fake<ProjectTask>();
			var taskMap = A.Fake<TaskNoNavigationPropsDto>();
			A.CallTo(() => _taskRepository.ProjectTaskExists(taskIdExists)).Returns(true);
			A.CallTo(() => _taskRepository.ProjectTaskExists(taskIdNotExists)).Returns(false);
			A.CallTo(() => _mapper.Map<TaskNoNavigationPropsDto>(task)).Returns(taskMap);
			var controller = new ProjectTaskController(_taskRepository, _projectRepository, _mapper);

			//Act
			var okResult = controller.GetTaskById(taskIdExists);
			var notFoundResult = controller.GetTaskById(taskIdNotExists);
			//Assert
			okResult.Should().NotBeNull();
			okResult.Should().BeOfType(typeof(OkObjectResult));
			notFoundResult.Should().NotBeNull();
			notFoundResult.Should().BeOfType(typeof(NotFoundResult));
		}
		[Fact]
		public void ProjecrTaskController_GetTaskByName_ReturnsOk()
		{
			//Arrange
			var existingTaskName = "name";
			var notExistingTaskName = "name1";
			var task = A.Fake<ProjectTask>();
			var taskMap = new TaskNoNavigationPropsDto();
			A.CallTo(() => _taskRepository.GetTaskByName(existingTaskName)).Returns(task);
			A.CallTo(() => _taskRepository.GetTaskByName(notExistingTaskName)).Returns(null);
			var controller = new ProjectTaskController(_taskRepository, _projectRepository, _mapper);
			//Act
			var okResult = controller.GetTaskByName(existingTaskName);
			var notFoundResult = controller.GetTaskByName(notExistingTaskName);
			//Assert
			okResult.Should().NotBeNull();
			okResult.Should().BeOfType(typeof(OkObjectResult));
			notFoundResult.Should().NotBeNull();
			notFoundResult.Should().BeOfType(typeof(NotFoundObjectResult));
		}
		[Fact]
		public void ProjectTaskController_GetTasksInPriorityRange_RetusnsOk()
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
			A.CallTo(() => _taskRepository.GetTasksPriorityRange(priorityLow, priorityHigh)).Returns(tasksPopulated);
			A.CallTo(() => _mapper.Map<List<TaskNoNavigationPropsDto>>(tasksPopulated)).Returns(tasksDtoPopulated);
			//fake return empty collections
			A.CallTo(() => _taskRepository.GetTasksPriorityRange(priorityLow, priorityEmptyResult)).Returns(tasksEmpty);
			A.CallTo(() => _mapper.Map<List<TaskNoNavigationPropsDto>>(tasksEmpty)).Returns(tasksDtoEmpty);
			var controller = new ProjectTaskController(_taskRepository, _projectRepository, _mapper);

			//Act
			var okResult = controller.GetTasksInPriorityRange(priorityLow, priorityHigh);
			var notFoundResult = controller.GetTasksInPriorityRange(priorityLow, priorityEmptyResult);
			var badRequestResult1 = controller.GetTasksInPriorityRange(priorityHigh, priorityLow);
			var badRequestResult2 = controller.GetTasksInPriorityRange(priorityLow, priorityInvalid);

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
		public void ProjectTaskController_GetTasksWithStatus_ReturnsOk()
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
			A.CallTo(() => _taskRepository.GetTasksWithStatus(status)).Returns(tasksPopulated);
			A.CallTo(() => _mapper.Map<List<TaskNoNavigationPropsDto>>(tasksPopulated)).Returns(tasksDtoPopulated);
			//fake return empty collections
			A.CallTo(() => _taskRepository.GetTasksWithStatus(statusEmptyResult)).Returns(tasksEmpty);
			A.CallTo(() => _mapper.Map<List<TaskNoNavigationPropsDto>>(tasksEmpty)).Returns(tasksDtoEmpty);
			var controller = new ProjectTaskController(_taskRepository, _projectRepository, _mapper);

			//Act
			var okResult = controller.GetTasksWithStatus(status);
			var notFoundResult = controller.GetTasksWithStatus(statusEmptyResult);

			//Assert
			okResult.Should().NotBeNull();
			okResult.Should().BeOfType(typeof(OkObjectResult));
			notFoundResult.Should().NotBeNull();
			notFoundResult.Should().BeOfType(typeof(NotFoundObjectResult));
		}
		[Fact]
		public void ProjectTaskController_CreateTask_RetusnsNoContent()
		{
			//Arrange
			int projectIdExist = 1;
			int projectIdNotExist = 2;
			int projectIdNameTaken = 3;
			var projectTaskDto = A.Fake<ProjectTaskDto>();
			var projectTask = A.Fake<ProjectTask>();
			//fake returns projects exist
			A.CallTo(() => _projectRepository.ProjectExists(projectIdExist)).Returns(true);
			A.CallTo(() => _projectRepository.ProjectExists(projectIdNameTaken)).Returns(true);
			//fake return project does not exist
			A.CallTo(() => _projectRepository.ProjectExists(projectIdNotExist)).Returns(false);
			//fake returns of tasks from DTOm by using mapper
			A.CallTo(() => _mapper.Map<ProjectTask>(projectTaskDto)).Returns(projectTask);
			//fake return name is available
			A.CallTo(() => _taskRepository.TaskNameAlreadyTaken(projectTask, projectIdExist)).Returns(false);
			A.CallTo(() => _taskRepository.CreateProjectTask(projectTask)).Returns(true);
			//fake return name is taken
			A.CallTo(() => _taskRepository.TaskNameAlreadyTaken(projectTask, projectIdNameTaken)).Returns(true);
			var controller = new ProjectTaskController(_taskRepository, _projectRepository, _mapper);
			//Act
			var okResult = controller.CreateTask(projectIdExist, projectTaskDto);
			var notFoundResult = controller.CreateTask(projectIdNotExist, projectTaskDto);
			var nameTakenResult = controller.CreateTask(projectIdNameTaken, projectTaskDto);
			//Assert
			okResult.Should().NotBeNull();
			okResult.Should().BeOfType(typeof(NoContentResult));
			notFoundResult.Should().NotBeNull();
			notFoundResult.Should().BeOfType(typeof(NotFoundResult));
			nameTakenResult.Should().NotBeNull();
			nameTakenResult.Should().BeOfType(typeof(ObjectResult));
		}
		[Fact]
		public void ProjectTaskController_UpdateTask_ReturnsNoContent()
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
			A.CallTo(() => _projectRepository.ProjectExists(projectOneId)).Returns(true);
			A.CallTo(() => _projectRepository.ProjectExists(projectTwoId)).Returns(true);
			A.CallTo(() => _projectRepository.ProjectExists(projectNameTakenId)).Returns(true);
			A.CallTo(() => _projectRepository.ProjectExists(projectNotExistId)).Returns(false);
			A.CallTo(() => _taskRepository.ProjectTaskExists(projectOneTaskId)).Returns(true);
			//fake returns about task belonging to a project
			A.CallTo(() => _taskRepository.TaskBelongsToProjectNoTracking(projectOneTaskId, projectOneId)).Returns(true);
			A.CallTo(() => _taskRepository.TaskBelongsToProjectNoTracking(projectOneTaskId, projectTwoId)).Returns(false);
			A.CallTo(() => _taskRepository.TaskBelongsToProjectNoTracking(projectOneTaskId, projectNameTakenId)).Returns(true);

			//fake return of task model to update
			A.CallTo(() => _mapper.Map<ProjectTask>(taskUpdateDto)).Returns(taskUpdate);
			//fake return that name is available
			A.CallTo(() => _taskRepository.TaskNameAlreadyTaken(taskUpdate, projectNameTakenId)).Returns(true);
			//fake return saving is ok
			A.CallTo(() => _taskRepository.TaskNameAlreadyTaken(taskUpdate,projectOneId)).Returns(false);
			//fake return that name is taken
			
			A.CallTo(() => _taskRepository.UpdateProjectTask(taskUpdate)).Returns(true);
			var controller = new ProjectTaskController(_taskRepository, _projectRepository, _mapper);

			//Act

			var okResult = controller.UpdateTask(projectOneId, projectOneTaskId, taskUpdateDto);
			var notFoundResult = controller.UpdateTask(projectNotExistId, projectOneTaskId, taskUpdateDto);
			var nameTakenResult = controller.UpdateTask(projectNameTakenId, projectOneTaskId, taskUpdateDto);
			var taskFromDiffProjResult = controller.UpdateTask(projectTwoId, projectOneTaskId, taskUpdateDto);

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
		public void ProjectTaskController_DeleteTask_ReturnsNoContent()
		{
			//Arrange

			int projectTaskExistsId = 1;
			int projectTaskNotExistId = 2;
			int projectTaskCanNotDeleteId = 3;
			var projectTask = A.Fake<ProjectTask>();
			var projectTaskCanNotDelete = A.Fake<ProjectTask>();
			A.CallTo(() => _taskRepository.ProjectTaskExists(projectTaskExistsId)).Returns(true);
			A.CallTo(() => _taskRepository.ProjectTaskExists(projectTaskNotExistId)).Returns(false);
			A.CallTo(() => _taskRepository.ProjectTaskExists(projectTaskCanNotDeleteId)).Returns(true);
			A.CallTo(() => _taskRepository.GetTaskById(projectTaskExistsId)).Returns(projectTask);
			A.CallTo(() => _taskRepository.GetTaskById(projectTaskCanNotDeleteId)).Returns(projectTaskCanNotDelete);
			A.CallTo(() => _taskRepository.DeleteProjectTask(projectTask)).Returns(true);
			A.CallTo(() => _taskRepository.DeleteProjectTask(projectTaskCanNotDelete)).Returns(false);
			var controller = new ProjectTaskController(_taskRepository, _projectRepository, _mapper);

			//Act
			var noContentResult = controller.DeleteTask(projectTaskExistsId);
			var notFoundResult = controller.DeleteTask(projectTaskNotExistId);
			var canNotDeleteResult = controller.DeleteTask(projectTaskCanNotDeleteId);

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
