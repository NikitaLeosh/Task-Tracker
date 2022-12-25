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
		public async Task<bool> TaskNameAlreadyTaken(ProjectTask task, int projectId)
		{
			//Here we select the given project's tasks and check them for matching the input name
			return await _context.ProjectTasks.Include(p => p.Project).Where(p => p.Project.Id == projectId).
				AnyAsync(n => n.TaskName.Trim().ToUpper() == task.TaskName.Trim().ToUpper());
		}
		//GET ALL
		public ICollection<ProjectTask> GetAllTasks()
		{
			return _context.ProjectTasks.ToList();
		}
		//GET ONE BY ID
		public ProjectTask GetTaskById(int taskId)
		{
			return _context.ProjectTasks.Where(t => t.Id == taskId).FirstOrDefault();
		}
		//CHECK FOR EXISTANCE
		public bool ProjectTaskExists(int taskId)
		{
			return _context.ProjectTasks.Any(t => t.Id == taskId);
		}
		//SAVE CHANGES
		public bool Save()
		{
			var result = _context.SaveChanges();
			return result > 0;
		}
		//CHECK TASK IS IN THE GIVEN PROJECT
		public bool TaskBelongsToProjectNoTracking(int taskId, int projectId)
		{
			var existingTask = _context.ProjectTasks.Include(p => p.Project).AsNoTracking()
				.Where(p => p.Project.Id == projectId).FirstOrDefault(i => i.Id == taskId);
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
		public ProjectTask GetTaskByName(string taskName)
		{
			return _context.ProjectTasks.FirstOrDefault
				(n => n.TaskName.Trim().ToUpper() == taskName.Trim().ToUpper());
		}
		//GET TASKS HAVING PRIORITY IN A GIVEN RANGE
		public ICollection<ProjectTask> GetTasksPriorityRange(int priorityLow, int priorityHigh)
		{
			return _context.ProjectTasks.Where(p => p.Priority > priorityLow && p.Priority < priorityHigh).ToList();
		}
		//GET TASKS HAVING THE GIVEN STATUS
		public ICollection<ProjectTask> GetTasksWithStatus(ProjectTaskStatus status)
		{
			return _context.ProjectTasks.Where(s => s.TaskStatus == status).ToList();
		}
	}
}
