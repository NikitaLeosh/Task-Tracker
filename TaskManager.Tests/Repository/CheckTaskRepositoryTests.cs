using FakeItEasy;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaskManager.Data;
using TaskManager.Interfaces;
using TaskManager.Models;
using TaskManager.Models.Enum;
using TaskManager.Repositories;

namespace TaskManager.Tests.Repository
{
	public class CheckTaskRepositoryTests
	{
		private readonly Guid _testProjectId = Guid.NewGuid();
		private readonly Guid _testTaskId = Guid.NewGuid();
		private ProjectDbContext GetDatabaseContext()
		{
			var options = new DbContextOptionsBuilder<ProjectDbContext>()
				.UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
				.Options;
			var databaseContext = new ProjectDbContext(options);
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

		private readonly CheckTaskRepository checkTaskRepository;
		private readonly ProjectDbContext dbContext;
		private readonly ICheckProjectRepository checkProjectRepository;
		public CheckTaskRepositoryTests()
		{
			dbContext = GetDatabaseContext();
			checkProjectRepository = A.Fake<ICheckProjectRepository>();
			checkTaskRepository = new CheckTaskRepository(dbContext, checkProjectRepository);
		}


		[Fact]
		public async void CheckTaskRepository_TaskBelongsToProjectNoTracking_ReturnsBool()
		{
			//Arrange
			Guid projectId = _testProjectId;
			Guid taskIdTrue = _testTaskId;
			Guid taskIdFalse = Guid.NewGuid();

			//Act
			var trueResult = await checkTaskRepository.TaskBelongsToProjectNoTrackingAsync(taskIdTrue, projectId);
			var falseResult = await checkTaskRepository.TaskBelongsToProjectNoTrackingAsync(taskIdFalse, projectId);
			//Assert
			trueResult.Should().BeTrue();
			falseResult.Should().BeFalse();
		}
		[Fact]
		public async void CheckTaskRepository_TaskNameAlreadyTaken_ReturnsBool()
		{
			//Arrange
			Guid projectId = _testProjectId;
			ProjectTask free = new() { TaskName = "free name" };
			ProjectTask taken = new() { TaskName = "task number 1 of project 1" };

			//Act
			var falseResult = await checkTaskRepository.TaskNameAlreadyTakenAsync(free, projectId);
			var trueResult = await checkTaskRepository.TaskNameAlreadyTakenAsync(taken, projectId);
			//Assert
			falseResult.Should().BeFalse();
			trueResult.Should().BeTrue();
		}
		[Fact]
		public async void CheckTaskRepository_ProjectTaskExists_ReturnsBool()
		{
			//Arrange
			Guid taskExistId = _testTaskId;
			Guid taskNotExistId = Guid.NewGuid();

			//Act
			var trueResult = await checkTaskRepository.ProjectTaskExistsAsync(taskExistId);
			var falseResult = await checkTaskRepository.ProjectTaskExistsAsync(taskNotExistId);
			//Assert
			trueResult.Should().BeTrue();
			falseResult.Should().BeFalse();
		}
	}
}
