using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using TaskManager.Data;
using TaskManager.Interfaces;
using TaskManager.Models;
using TaskManager.Models.Enum;

namespace TaskManager.Repositories
{
	public class ProjectRepository : IProjectRepository
	{

		private readonly ProjectDbContext _context;

		public ProjectRepository(ProjectDbContext context)
		{
			_context = context;
		}

		public bool CreateProject(Project project)
		{
			_context.Add(project);
			return Save();
		}
		public bool DeleteProject(Project project)
		{
			_context.Remove(project);
			return Save();
		}
		public async Task<ICollection<Project>> GetAllProjectsAsync()
		{
			return await _context.Projects.Include(t => t.Tasks).ToListAsync();
		}
		public async Task<Project> GetProjectByIdAsync(Guid projectId)
		{
			return await _context.Projects.Where(p => p.Id == projectId).FirstOrDefaultAsync();

		}

		public async Task<Project> GetProjectByNameAsync(string name)
		{
			return await _context.Projects.FirstOrDefaultAsync
				(n => n.ProjectName.Trim().ToUpper() == name.Trim().ToUpper());
		}

		public bool Save()
		{
			var result = _context.SaveChanges();
			return result > 0;
		}
		public bool UpdateProject(Project project)
		{
			_context.Update(project);
			return Save();
		}
		public async Task<ICollection<Project>> GetProjectsPriorityRangeAsync(int priorityLow, int priorityHigh)
		{
			return await _context.Projects.Where(p => p.Priority >= priorityLow && p.Priority <= priorityHigh).ToListAsync();
		}
		public async Task<ICollection<Project>> GetProjectsStartAfterAsync(DateTime start)
		{
			return await _context.Projects.Where(d => d.StartDate > start).ToListAsync();
		}
		public async Task<ICollection<Project>> GetProjectsStartAtRangeAsync(DateTime start, DateTime end)
		{
			return await _context.Projects.Where(d => d.StartDate > start && d.StartDate < end).ToListAsync();
		}
		public async Task<ICollection<Project>> GetProjectsEndBeforeAsync(DateTime end)
		{
			return await _context.Projects.Where(d => d.CompletionDate < end).ToListAsync();
		}
		public async Task<ICollection<Project>> GetProjectsWithStatusAsync(ProjectStatus status)
		{
			return await _context.Projects.Where(s => s.ProjectStatus == status).ToListAsync();
		}
	}
}
