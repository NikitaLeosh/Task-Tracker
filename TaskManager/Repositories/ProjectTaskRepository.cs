using Microsoft.EntityFrameworkCore;
using TaskManager.Data;
using TaskManager.Data.Enum;
using TaskManager.Interfaces;
using TaskManager.Models;

namespace TaskManager.Repositories
{
	public class ProjectTaskRepository : IProjectTaskRepository
	{
		//DI part
		private readonly ApplicationDbContext _context;

		public ProjectTaskRepository(ApplicationDbContext context)
		{
			_context = context;
		}
		//CREATE
		public bool CreateProjectTask(ProjectTask projectTask)
		{
			_context.Add(projectTask);
			return Save();
		}
		//DELETE
		public bool DeleteProjectTask(ProjectTask projectTask)
		{
			_context.Remove(projectTask);
			return Save();

		}
		//CHECK FOR NAME AVAILABILITY
		public async Task<bool> TaskNameAlreadyTakenAsync(ProjectTask task, int projectId)
		{
			//Here we select the given project's tasks and check them for matching the input name
			return await _context.ProjectTasks.Include(p => p.Project).Where(p => p.Project.Id == projectId).
				AnyAsync(n => n.TaskName.Trim().ToUpper() == task.TaskName.Trim().ToUpper());
		}
		//GET ALL
		public async Task<ICollection<ProjectTask>> GetAllTasksAsync()
		{
			return await _context.ProjectTasks.ToListAsync();
		}
		//GET ONE BY ID
		public async Task<ProjectTask> GetTaskByIdAsync(int taskId)
		{
			return await _context.ProjectTasks.Where(t => t.Id == taskId).FirstOrDefaultAsync();
		}
		//CHECK FOR EXISTANCE
		public async Task<bool> ProjectTaskExistsAsync(int taskId)
		{
			return await _context.ProjectTasks.AnyAsync(t => t.Id == taskId);
		}
		//SAVE CHANGES
		public bool Save()
		{
			var result = _context.SaveChanges();
			return result > 0;
		}
		//CHECK TASK IS IN THE GIVEN PROJECT
		public async Task<bool> TaskBelongsToProjectNoTrackingAsync(int taskId, int projectId)
		{
			var existingTask = await _context.ProjectTasks.Include(p => p.Project).AsNoTracking()
				.Where(p => p.Project.Id == projectId).FirstOrDefaultAsync(i => i.Id == taskId);
			if (existingTask == null)
				return false;
			return true;
		}
		//UPDATE
		public bool UpdateProjectTask(ProjectTask projectTask)
		{
			_context.ProjectTasks.Update(projectTask);
			return Save();
		}
		//GET TASK BY NAME
		public async Task<ProjectTask> GetTaskByNameAsync(string taskName)
		{
			return await _context.ProjectTasks.FirstOrDefaultAsync
				(n => n.TaskName.Trim().ToUpper() == taskName.Trim().ToUpper());
		}
		//GET TASKS HAVING PRIORITY IN A GIVEN RANGE
		public async Task<ICollection<ProjectTask>> GetTasksPriorityRangeAsync(int priorityLow, int priorityHigh)
		{
			return await _context.ProjectTasks.Where
				(p => p.Priority > priorityLow && p.Priority < priorityHigh).ToListAsync();
		}
		//GET TASKS HAVING THE GIVEN STATUS
		public async Task<ICollection<ProjectTask>> GetTasksWithStatusAsync(ProjectTaskStatus status)
		{
			return await _context.ProjectTasks.Where(s => s.TaskStatus == status).ToListAsync();
		}
	}
}
