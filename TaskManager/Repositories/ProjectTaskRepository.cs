using Microsoft.EntityFrameworkCore;
using TaskManager.Data;
using TaskManager.Interfaces;
using TaskManager.Models;
using TaskManager.Models.Enum;

namespace TaskManager.Repositories
{
	public class ProjectTaskRepository : IProjectTaskRepository
	{
		private readonly ProjectDbContext _context;

		public ProjectTaskRepository(ProjectDbContext context)
		{
			_context = context;
		}
		public bool CreateProjectTask(ProjectTask projectTask)
		{
			_context.Add(projectTask);
			return Save();
		}
		public bool DeleteProjectTask(ProjectTask projectTask)
		{
			_context.Remove(projectTask);
			return Save();

		}
		public async Task<ICollection<ProjectTask>> GetAllTasksAsync()
		{
			return await _context.ProjectTasks.ToListAsync();
		}
		public async Task<ProjectTask> GetTaskByIdAsync(Guid taskId)
		{
			return await _context.ProjectTasks.Where(t => t.Id == taskId).FirstOrDefaultAsync();
		}

		public async Task<ICollection<ProjectTask>> GetTasksOfAProjectAsync(Guid projectId)
		{
			return await _context.ProjectTasks.Include(p => p.Project).Where(
				t => t.Project.Id == projectId).ToListAsync();
		}
		public bool Save()
		{
			var result = _context.SaveChanges();
			return result > 0;
		}
		public bool UpdateProjectTask(ProjectTask projectTask)
		{
			_context.ProjectTasks.Update(projectTask);
			return Save();
		}
		public async Task<ProjectTask> GetTaskByNameAsync(string taskName)
		{
			return await _context.ProjectTasks.FirstOrDefaultAsync
				(n => n.TaskName.Trim().ToUpper() == taskName.Trim().ToUpper());
		}
		public async Task<ICollection<ProjectTask>> GetTasksPriorityRangeAsync(int priorityLow, int priorityHigh)
		{
			return await _context.ProjectTasks.Where
				(p => p.Priority > priorityLow && p.Priority < priorityHigh).ToListAsync();
		}
		public async Task<ICollection<ProjectTask>> GetTasksWithStatusAsync(ProjectTaskStatus status)
		{
			return await _context.ProjectTasks.Where(s => s.TaskStatus == status).ToListAsync();
		}
	}
}
