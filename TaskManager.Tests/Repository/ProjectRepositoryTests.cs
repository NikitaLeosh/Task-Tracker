using FakeItEasy;
using FluentAssertions;
using Microsoft.AspNetCore.Rewrite;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Scaffolding.Metadata;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using TaskManager.Data;
using TaskManager.Models;
using TaskManager.Models.Enum;
using TaskManager.Repositories;

namespace TaskManager.Tests.Repository
{
	public class ProjectRepositoryTests
	{
		private readonly Guid _testGuid = Guid.NewGuid();
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
		private readonly ApplicationDbContext dbContext;
		private readonly ProjectRepository projectRepository;
		private readonly CheckProjectRepository checkProjectRepository;
		public ProjectRepositoryTests()
		{
			dbContext = GetDatabaseContext();
			projectRepository = new ProjectRepository(dbContext);
			checkProjectRepository = new CheckProjectRepository(dbContext);
		}
		[Fact]
		public async void ProjectRepository_GetAllProjectsAsync_RetusnsICollection()
		{
			//Arrange

			//Act
			var result = await projectRepository.GetAllProjectsAsync();
			//Assert
			result.Should().NotBeNull();
			result.Should().BeOfType(typeof(List<Project>));
		}
		[Fact]
		public async void ProjectRepository_GetProjectByIdAsync_ReturnsProject()
		{
			//Arrange
			Guid projetId = _testGuid;
			//Act
			var result = await projectRepository.GetProjectByIdAsync(projetId);
			//Assert
			result.Should().NotBeNull();
			result.Should().BeOfType(typeof(Project));
		}
		[Fact]
		public async void ProjectRepository_GetProjectByName_ReturnsProject()
		{
			//Arrange
			var projectName = "prOject 1  ";

			//Act
			var result = await projectRepository.GetProjectByNameAsync(projectName);
			//Assert
			result.Should().NotBeNull();
			result.Should().BeOfType(typeof(Project));
		}

		[Fact]
		public async void ProjectRepository_GetProjectsPriorityRangeAsync_ReturnsList()
		{
			//Arrange
			int priorityLow = 1;
			int priorityHigh = 3;
			int priorityLowEmptyResult = 4;
			int priorityHighEmptyResult = 5;
			//Act
			var okResult = await projectRepository.GetProjectsPriorityRangeAsync(priorityLow, priorityHigh);
			var emptyResult = await projectRepository.GetProjectsPriorityRangeAsync(priorityLowEmptyResult, priorityHighEmptyResult);
			//Assert
			okResult.Should().NotBeNull();
			okResult.Should().BeOfType(typeof(List<Project>));
			emptyResult.Should().BeEmpty();
		}
		[Fact]
		public async void ProjectRepository_GetProjectsStartAfterAsync_ReturnsList()
		{
			//Arrange
			DateTime start = new DateTime(2010, 3, 4);
			DateTime startEmptyReturn = new DateTime(2030, 3, 4);

			//Act
			var okResult = await projectRepository.GetProjectsStartAfterAsync(start);
			var emptyResult = await projectRepository.GetProjectsStartAfterAsync(startEmptyReturn);
			//Assert
			okResult.Should().NotBeNull();
			okResult.Should().BeOfType(typeof(List<Project>));
			emptyResult.Should().BeEmpty();
		}
		[Fact]
		public async void ProjectRepository_GetProjectsEndBeforeAsync_ReturnsList()
		{
			//Arrange
			DateTime end = new DateTime(2030, 3, 4);
			DateTime endEmptyReturn = new DateTime(2000, 3, 4);

			//Act
			var okResult = await projectRepository.GetProjectsEndBeforeAsync(end);
			var emptyResult = await projectRepository.GetProjectsEndBeforeAsync(endEmptyReturn);
			//Assert
			okResult.Should().NotBeNull();
			okResult.Should().BeOfType(typeof(List<Project>));
			emptyResult.Should().BeEmpty();
		}

		[Fact]
		public async void ProjectRepository_GetProjectsStartAtRangeAsync_ReturnsList()
		{
			//Arrange
			DateTime start = new DateTime(2010, 3, 4);
			DateTime startEmptyReturn = new DateTime(2030, 3, 4);
			DateTime end = new DateTime(2030, 3, 4);
			DateTime endEmptyReturn = new DateTime(2030, 3, 5);


			//Act
			var okResult = await projectRepository.GetProjectsStartAtRangeAsync(start, end);
			var emptyResult = await projectRepository.GetProjectsStartAtRangeAsync(startEmptyReturn, endEmptyReturn);
			//Assert
			okResult.Should().NotBeNull();
			okResult.Should().BeOfType(typeof(List<Project>));
			emptyResult.Should().BeEmpty();
		}
		[Fact]
		public async void ProjectRepositpry_GetProjectsWithStatusAsync_ReturnsList()
		{
			//Arrange
			var status = ProjectStatus.Active;
			var statusEmptyReturn = ProjectStatus.Completed;

			//Act
			var okResult = await projectRepository.GetProjectsWithStatusAsync(status);
			var emptyResult = await projectRepository.GetProjectsWithStatusAsync(statusEmptyReturn);
			//Assert
			okResult.Should().NotBeNull();
			okResult.Should().BeOfType(typeof(List<Project>));
			emptyResult.Should().BeEmpty();
		}

		[Fact]
		public async void ProjectRepository_CreateProject_ReturnsBool()
		{
			//Arrange
			Project project = A.Fake<Project>();
			project.ProjectName = "Project";

			//Act
			var result = projectRepository.CreateProject(project);
			//Assert
			result.Should().BeTrue();
		}
		[Fact]
		public async void ProjectRepository_UpdateProject_ReturnsBool()
		{
			//Arrange
			var project = await dbContext.Projects.FirstOrDefaultAsync(p => p.Id == _testGuid);

			//Act
			var result = projectRepository.UpdateProject(project);
			//Assert
			result.Should().BeTrue();
		}
		[Fact]
		public async void ProjectRepository_DeleteProject_ReturnsBool()
		{
			//Arrange

			var project = await dbContext.Projects.FirstOrDefaultAsync(p => p.Id == _testGuid);

			//Act
			var result = projectRepository.DeleteProject(project);
			//Assert
			result.Should().BeTrue();
		}
	}
}
