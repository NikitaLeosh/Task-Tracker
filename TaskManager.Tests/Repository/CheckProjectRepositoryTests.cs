using FakeItEasy;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaskManager.Data;
using TaskManager.Models;
using TaskManager.Models.Enum;
using TaskManager.Repositories;

namespace TaskManager.Tests.Repository
{
	public class CheckProjectRepositoryTests
	{
		private readonly Guid _testGuid = Guid.NewGuid();
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
						Id = _testGuid,
						ProjectName = $"project 1",
						StartDate = new DateTime(2020, 2, 2),
						CompletionDate = new DateTime(2021, 2, 1),
						ProjectStatus = (ProjectStatus)(0),
						Priority = 1,
						Tasks = new List<ProjectTask>
						{
							new ProjectTask
							{
								TaskName = $"Task number 1 of project 1",
								TaskDescription = "blablabla",
								TaskStatus = (ProjectTaskStatus)(1),
								Priority = 1
							},
							new ProjectTask
							{
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
		private readonly ProjectDbContext dbContext;
		private readonly ProjectRepository projectRepository;
		private readonly CheckProjectRepository checkProjectRepository;
		public CheckProjectRepositoryTests()
		{
			dbContext = GetDatabaseContext();
			projectRepository = new ProjectRepository(dbContext);
			checkProjectRepository = new CheckProjectRepository(dbContext);
		}

		[Fact]
		public async void ProjectRepository_ProjectNameAlreadyTakenAsync_ReturnsBool()
		{
			//Arrange
			Project free = new() { ProjectName = "blablabla" };
			Project taken = new() { ProjectName = "project 1" };

			//Act
			var falseResult = await checkProjectRepository.ProjectNameAlreadyTakenAsync(free);
			var trueResult = await checkProjectRepository.ProjectNameAlreadyTakenAsync(taken);
			//Assert
			falseResult.Should().BeFalse();
			trueResult.Should().BeTrue();
		}

		[Fact]
		public async void ProjectRepository_ProjectExistsAsync_ReturnsBool()
		{
			//Arrange
			Guid projectIdTrue = _testGuid;
			Guid projectIdFalse = Guid.NewGuid();
			//Act
			var trueResult = await checkProjectRepository.ProjectExistsAsync(projectIdTrue);
			var falseResult = await checkProjectRepository.ProjectExistsAsync(projectIdFalse);
			//Assert
			trueResult.Should().BeTrue();
			falseResult.Should().BeFalse();
		}
	}
}
