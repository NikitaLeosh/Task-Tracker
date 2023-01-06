using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaskManager.Data;
using TaskManager.Models;
using TaskManager.Repositories;
using FluentAssertions;
using FakeItEasy;
using TaskManager.Models.Enum;

namespace TaskManager.Tests.Repository
{
	public class ProjectTaskRepositoryTests
	{
		private readonly Guid _testProjectId = Guid.NewGuid();
		private readonly Guid _testTaskId = Guid.NewGuid();
		private ApplicationDbContext GetDatabaseContext()
		{
			var options = new DbContextOptionsBuilder<ApplicationDbContext>()
				.UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
				.Options;
			var databaseContext = new ApplicationDbContext(options);
			databaseContext.Database.EnsureCreated();
			if (databaseContext.Projects.Count() <= 0)
			{
				databaseContext.Projects.Add(
					new Project
					{
						Id = _testProjectId,
						ProjectName = $"project 1",
						StartDate = new DateTime(2020, 2, 2),
						CompletionDate = new DateTime(2021, 2, 1),
						ProjectStatus = (ProjectStatus)(0),
						Priority = 1,
						Tasks = new List<ProjectTask>
						{
							new ProjectTask
							{
								Id = _testTaskId,
								TaskName = $"Task number 1 of project 1",
								TaskDescription = "blablabla",
								TaskStatus = (ProjectTaskStatus)(1),
								Priority = 1
							},
							new ProjectTask
							{
								Id = Guid.NewGuid(),
								TaskName = $"Task number 2 of project 1",
								TaskDescription = "blablabla",
								TaskStatus = (ProjectTaskStatus)(1),
								Priority = 1
							}
						}
					});
				for (int i = 1; i < 10; i++)
				{
					databaseContext.Projects.Add(
					new Project
					{
						ProjectName = $"project {i + 1}",
						StartDate = new DateTime(2020, 2, 2),
						CompletionDate = new DateTime(2021, 2, 1),
						ProjectStatus = (ProjectStatus)(i % 2),
						Priority = (i % 3) + 1,
						Tasks = new List<ProjectTask>
						{
							new ProjectTask
							{
								TaskName = $"Task number 1 of project {i+1}",
								TaskDescription = "blablabla",
								TaskStatus = (ProjectTaskStatus)(i % 2),
								Priority = (i % 3) + 1
							},
							new ProjectTask
							{
								TaskName = $"Task number 2 of project {i+1}",
								TaskDescription = "blablabla",
								TaskStatus = (ProjectTaskStatus)(i % 2),
								Priority = (i % 3) + 1
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
			Guid taskId = _testTaskId;

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
			var taskName = "tAsk nuMbeR 1 of project 3 ";

			//Act
			var result = await taskRepository.GetTaskByNameAsync(taskName);
			//Assert
			result.Should().NotBeNull();
			result.Should().BeOfType(typeof(ProjectTask));
		}
		[Fact]
		public async void ProjectRepository_GetTasksOfAProjectAsync_ReturnsList()
		{
			//Arrange
			Guid projetId = _testProjectId;
			//Act
			var result = await taskRepository.GetTasksOfAProjectAsync(projetId);
			//Assert
			result.Should().NotBeNull();
			result.Should().BeOfType(typeof(List<ProjectTask>));
		}
		[Fact]
		public async void ProjectTaskRepository_GetTasksPriorityRange_ReturnsList()
		{
			//Arrange
			int priorityLow = 1;
			int priorityHigh = 3;
			int priorityLowEmptyResult = 4;
			int priorityHighEmptyResult = 5;

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

			ProjectTask task = await dbContext.ProjectTasks.FirstOrDefaultAsync(t => t.Id == _testTaskId);

			//Act
			var trueResult = taskRepository.UpdateProjectTask(task);
			//Assert
			trueResult.Should().BeTrue();
		}
		[Fact]
		public async void ProjectTaskRepository_DeleteProjectTask_ReturnsBool()
		{
			//Arrange

			ProjectTask task = await dbContext.ProjectTasks.FirstOrDefaultAsync(t => t.Id == _testTaskId);

			//Act
			var trueResult = taskRepository.DeleteProjectTask(task);
			//Assert
			trueResult.Should().BeTrue();
		}
	}
}
