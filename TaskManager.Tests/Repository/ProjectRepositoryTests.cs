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
using TaskManager.Data.Enum;
using TaskManager.Models;
using TaskManager.Repositories;

namespace TaskManager.Tests.Repository
{
	public class ProjectRepositoryTests
	{
		private ApplicationDbContext GetDatabaseContext()
		{
			var options = new DbContextOptionsBuilder<ApplicationDbContext>()
				.UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
				.Options;
			var databaseContext = new ApplicationDbContext(options);
			databaseContext.Database.EnsureCreated();
			if( databaseContext.Projects.Count() <= 0)
			{
				for(int i=0;i<10;i++)
				{
					databaseContext.Projects.Add(
					new Project
					{
						ProjectName = $"project {i}",
						StartDate = new DateTime(2020, 2, 2),
						CompletionDate = new DateTime(2021, 2, 1),
						ProjectStatus = (ProjectStatus)(i % 2),
						Priority = i + 1,
						Tasks = new List<ProjectTask>
						{
							new ProjectTask
							{
								TaskName = $"Task number {i}",
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
		private readonly ApplicationDbContext dbContext;
		private readonly ProjectRepository projectRepository;
		public ProjectRepositoryTests()
		{
			dbContext = GetDatabaseContext();
			projectRepository = new ProjectRepository(dbContext);
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
			int projetId = 1;
			
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
		public async void ProjectRepository_GetTasksOfAProjectAsync_ReturnsList()
		{
			//Arrange
			int projetId = 1;
			//var dbContext = await GetDatabaseContext();
			//var projectRepository = new ProjectRepository(dbContext);
			//Act
			var result = await projectRepository.GetTasksOfAProjectAsync(projetId);
			//Assert
			result.Should().NotBeNull();
			result.Should().BeOfType(typeof(List<ProjectTask>));
		}
		[Fact]
		public async void ProjectRepository_GetTasksOfAProjectNoTrackingAsync_ReturnsList()
		{
			//Arrange
			int projetId = 1;
			//var dbContext = await GetDatabaseContext();
			//var projectRepository = new ProjectRepository(dbContext);
			//Act
			var result = await projectRepository.GetTasksOfAProjectNoTrackingAsync(projetId);
			//Assert
			result.Should().NotBeNull();
			result.Should().BeOfType(typeof(List<ProjectTask>));
		}
		[Fact]
		public async void ProjectRepository_GetProjectsPriorityRangeAsync_ReturnsList()
		{
			//Arrange
			int priorityLow = 1;
			int priorityHigh = 50;
			int priorityLowEmptyResult = 90;
			int priorityHighEmptyResult = 93;
			//var dbContext = await GetDatabaseContext();
			//var projectRepository = new ProjectRepository(dbContext);
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
		public async void ProjectRepository_ProjectNameAlreadyTakenAsync_ReturnsBool()
		{
			//Arrange
			Project free = new(){ProjectName = "blablabla"};
			Project taken = new() { ProjectName = "project 1" };
			
			//Act
			var falseResult= await projectRepository.ProjectNameAlreadyTakenAsync(free);
			var trueResult = await projectRepository.ProjectNameAlreadyTakenAsync(taken);
			//Assert
			falseResult.Should().BeFalse();
			trueResult.Should().BeTrue();
		}

		[Fact]
		public async void ProjectRepository_ProjectExistsAsync_ReturnsBool()
		{
			//Arrange
			int projectIdTrue = 1;
			int projectIdFalse = 0;
			
			//Act
			var trueResult = await projectRepository.ProjectExistsAsync(projectIdTrue);
			var falseResult = await projectRepository.ProjectExistsAsync(projectIdFalse);
			//Assert
			trueResult.Should().BeTrue();
			falseResult.Should().BeFalse();
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
			var project = await dbContext.Projects.FirstOrDefaultAsync(p => p.Id == 1);

			//Act
			var result = projectRepository.UpdateProject(project);
			//Assert
			result.Should().BeTrue();
		}
		[Fact]
		public async void ProjectRepository_DeleteProject_ReturnsBool()
		{
			//Arrange
			
			var project = await dbContext.Projects.FirstOrDefaultAsync(p => p.Id == 1);
			
			//Act
			var result = projectRepository.DeleteProject(project);
			//Assert
			result.Should().BeTrue();
		}
	}
}
