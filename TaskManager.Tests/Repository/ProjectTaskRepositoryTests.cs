using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaskManager.Data.Enum;
using TaskManager.Data;
using TaskManager.Models;
using TaskManager.Repositories;
using FluentAssertions;
using FakeItEasy;

namespace TaskManager.Tests.Repository
{
	public class ProjectTaskRepositoryTests
	{
		private ApplicationDbContext GetDatabaseContext()
		{
			var options = new DbContextOptionsBuilder<ApplicationDbContext>()
				.UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
				.Options;
			var databaseContext = new ApplicationDbContext(options);
			databaseContext.Database.EnsureCreated();
			if (databaseContext.Projects.Count() <= 0)
			{
				for (int i = 0; i < 10; i++)
				{
					databaseContext.Projects.Add(
					new Project
					{
						ProjectName = $"project {i + 1}",
						StartDate = new DateTime(2020, 2, 2),
						CompletionDate = new DateTime(2021, 2, 1),
						ProjectStatus = (ProjectStatus)(i % 2),
						Priority = i + 1,
						Tasks = new List<ProjectTask>
						{
							new ProjectTask
							{
								TaskName = $"Task number {i + 1}",
								TaskDescription = "blablabla",
								TaskStatus = (ProjectTaskStatus)(i % 2),
								Priority = i * 4 + 1
							}
						}
					});
					databaseContext.SaveChanges();
				}
			}
			return databaseContext;
		}

		private readonly ProjectTaskRepository taskRepository;
		private readonly ApplicationDbContext dbContext;
		public ProjectTaskRepositoryTests()
		{
			dbContext = GetDatabaseContext();
			taskRepository = new ProjectTaskRepository(dbContext);
		}
		
		[Fact]
		public async void ProjectTaskRepository_GetAllTasks_RetusnsICollection()
		{
			//Arrange
			
			//Act
			var result = await taskRepository.GetAllTasksAsync();
			//Assert
			result.Should().NotBeNull();
			result.Should().BeOfType(typeof(List<ProjectTask>));
		}

		[Fact]
		public async void ProjectTaskRepository_GetTaskById_ReturnsTask()
		{
			//Arrange
			int taskId = 1;
			
			//Act
			var result = await taskRepository.GetTaskByIdAsync(taskId);
			//Assert
			result.Should().NotBeNull();
			result.Should().BeOfType(typeof(ProjectTask));
		}
		[Fact]
		public async void ProjectTaskRepository_GetTaskByName_ReturnsTask()
		{
			//Arrange
			var taskName = "tAsk nuMbeR 1  ";
			
			//Act
			var result = await taskRepository.GetTaskByNameAsync(taskName);
			//Assert
			result.Should().NotBeNull();
			result.Should().BeOfType(typeof(ProjectTask));
		}
		[Fact]
		public async void ProjectTaskRepository_GetTasksPriorityRange_ReturnsList()
		{
			//Arrange
			int priorityLow = 1;
			int priorityHigh = 50;
			int priorityLowEmptyResult = 90;
			int priorityHighEmptyResult = 93;
			
			//Act
			var okResult = await taskRepository.GetTasksPriorityRangeAsync(priorityLow, priorityHigh);
			var emptyResult = await taskRepository.GetTasksPriorityRangeAsync(priorityLowEmptyResult, priorityHighEmptyResult);
			//Assert
			okResult.Should().NotBeNull();
			okResult.Should().BeOfType(typeof(List<ProjectTask>));
			emptyResult.Should().BeEmpty();
		}
		[Fact]
		public async void ProjectTaskRepositpry_GetTasksWithStatus_ReturnsList()
		{
			//Arrange
			var status = ProjectTaskStatus.ToDo;
			var statusEmptyReturn = ProjectTaskStatus.Done;
			
			//Act
			var okResult = await taskRepository.GetTasksWithStatusAsync(status);
			var emptyResult = await taskRepository.GetTasksWithStatusAsync(statusEmptyReturn);
			//Assert
			okResult.Should().NotBeNull();
			okResult.Should().BeOfType(typeof(List<ProjectTask>));
			emptyResult.Should().BeEmpty();
		}
		[Fact]
		public async void ProjectTaskRepository_TaskBelongsToProjectNoTracking_ReturnsBool()
		{
			//Arrange
			int projectId = 1;
			int taskIdTrue = 1;
			int taskIdFalse = 12;
			
			//Act
			var trueResult = await taskRepository.TaskBelongsToProjectNoTrackingAsync(taskIdTrue, projectId);
			var falseResult = await taskRepository.TaskBelongsToProjectNoTrackingAsync(taskIdFalse, projectId);
			//Assert
			trueResult.Should().BeTrue();
			falseResult.Should().BeFalse();
		}
		[Fact]
		public async void ProjectTaskRepository_TaskNameAlreadyTaken_ReturnsBool()
		{
			//Arrange
			int projectId = 1;
			ProjectTask free = new() { TaskName = "free name" };
			ProjectTask taken = new() { TaskName = "task number 1" };
			
			//Act
			var falseResult = await taskRepository.TaskNameAlreadyTakenAsync(free, projectId);
			var trueResult = await taskRepository.TaskNameAlreadyTakenAsync(taken, projectId);
			//Assert
			falseResult.Should().BeFalse();
			trueResult.Should().BeTrue();
		}
		[Fact]
		public async void ProjectTaskRepository_ProjectTaskExists_ReturnsBool()
		{
			//Arrange
			int taskExistId = 2;
			int taskNotExistId = 0;
			
			//Act
			var trueResult = await taskRepository.ProjectTaskExistsAsync(taskExistId);
			var falseResult = await taskRepository.ProjectTaskExistsAsync(taskNotExistId);
			//Assert
			trueResult.Should().BeTrue();
			falseResult.Should().BeFalse();	
		}
		[Fact]
		public async void ProjectTaskRepository_CreateProjectTask_ReturnsBool()
		{
			//Arrange
			ProjectTask task = new() { TaskName = "Test task", TaskDescription = "blabla" };
			
			//Act
			var trueResult = taskRepository.CreateProjectTask(task);
			//Assert
			trueResult.Should().BeTrue();
		}
		[Fact]
		public async void ProjectTaskRepository_UpdateProjectTask_ReturnsBool()
		{
			//Arrange
			
			ProjectTask task = await dbContext.ProjectTasks.FirstOrDefaultAsync(t => t.Id == 1);
			
			//Act
			var trueResult = taskRepository.UpdateProjectTask(task);
			//Assert
			trueResult.Should().BeTrue();
		}
		[Fact]
		public async void ProjectTaskRepository_DeleteProjectTask_ReturnsBool()
		{
			//Arrange
			
			ProjectTask task = await dbContext.ProjectTasks.FirstOrDefaultAsync(t => t.Id == 2);
			
			//Act
			var trueResult = taskRepository.DeleteProjectTask(task);
			//Assert
			trueResult.Should().BeTrue();
		}
	}
}
