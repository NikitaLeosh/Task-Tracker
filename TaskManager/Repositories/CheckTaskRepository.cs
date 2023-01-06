using Microsoft.EntityFrameworkCore;
using TaskManager.Data;
using TaskManager.Exceptions;
using TaskManager.Interfaces;
using TaskManager.Models;

namespace TaskManager.Repositories
{
	public class CheckTaskRepository : ICheckTaskRepository
	{
		private readonly ApplicationDbContext _context;
		private readonly ICheckProjectRepository _checkProjectRepository;

		public CheckTaskRepository(ApplicationDbContext context, ICheckProjectRepository checkProjectRepository)
		{
			_context = context;
			_checkProjectRepository = checkProjectRepository;
		}

		public async Task<bool> ProjectTaskExistsAsync(Guid taskId)
		{
			return await _context.ProjectTasks.AnyAsync(t => t.Id == taskId);
		}

		public async Task<bool> TaskBelongsToProjectNoTrackingAsync(Guid taskId, Guid projectId)
		{
			return await _context.ProjectTasks.Include(p => p.Project).AsNoTracking()
				.Where(p => p.Project.Id == projectId).AnyAsync(i => i.Id == taskId);
		}

		public async Task<bool> TaskNameAlreadyTakenAsync(ProjectTask task, Guid projectId)
		{
			return await _context.ProjectTasks.Include(p => p.Project).Where(p => p.Project.Id == projectId).
				AnyAsync(n => n.TaskName.Trim().ToUpper() == task.TaskName.Trim().ToUpper());
		}
	}
}
