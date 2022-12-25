using Microsoft.EntityFrameworkCore;
using TaskManager.Helpers;
using TaskManager.Models;

namespace TaskManager.Data
{
	public class ApplicationDbContext : DbContext
	{
		public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
		{

		}
		
		public DbSet<ProjectTask> ProjectTasks { get; set; }
		public DbSet<Project> Projects { get; set; }
	}
}
