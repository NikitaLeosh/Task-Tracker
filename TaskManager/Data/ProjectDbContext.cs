using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage;
using System.Linq.Expressions;
using TaskManager.Helpers;
using TaskManager.Models;

namespace TaskManager.Data
{
	public class ProjectDbContext : DbContext
	{
		public ProjectDbContext(DbContextOptions<ProjectDbContext> options) : base(options)
		{
			try
			{
				var databaseCreator = Database.GetService<IDatabaseCreator>() as RelationalDatabaseCreator;
				if (databaseCreator != null)
				{
					if (!databaseCreator.CanConnect()) databaseCreator.Create();
					if (!databaseCreator.HasTables()) databaseCreator.CreateTables();
				}
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex.ToString());
			}
		}

		public DbSet<ProjectTask> ProjectTasks { get; set; }
		public DbSet<Project> Projects { get; set; }
	}
}
