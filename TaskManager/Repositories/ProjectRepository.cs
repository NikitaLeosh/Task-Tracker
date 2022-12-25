using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using TaskManager.Data;
using TaskManager.Data.Enum;
using TaskManager.Interfaces;
using TaskManager.Models;

namespace TaskManager.Repositories
{
	public class ProjectRepository : IProjectRepository
	{

		//DI section
		private readonly ApplicationDbContext _context;

		public ProjectRepository(ApplicationDbContext context)
		{
			_context = context;
		}

		//CREATE
		public bool CreateProject(Project project)
		{
			_context.Add(project);
			return Save();
		}
		//DELETE
		public bool DeleteProject(Project project)
		{
			_context.Remove(project);
			return Save();
		}
		//GET ALL
		public async Task<ICollection<Project>> GetAllProjectsAsync()
		{
			return await _context.Projects.Include(t => t.Tasks).ToListAsync();
		}
		//GET ONE BY ID
		public async Task<Project> GetProjectByIdAsync(int projectId)
		{
			return await _context.Projects.Where(p => p.Id == projectId).FirstOrDefaultAsync();
			
		}

		public async Task<Project> GetProjectByNameAsync(string name)
		{
			return await _context.Projects.FirstOrDefaultAsync
				(n => n.ProjectName.Trim().ToUpper() == name.Trim().ToUpper());
		}

		//GET TASKS OF GIVEN PROJECT
		//No tracking modifiction created for update method to avoid conflicting IDs
		public async Task<ICollection<ProjectTask>> GetTasksOfAProjectNoTrackingAsync(int projectId)
		{
			//getting from db all tasks that the related project implies by
			//including the project field to the queries
			return await _context.ProjectTasks.Include(p => p.Project).AsNoTracking().Where(t => t.Project.Id == projectId).ToListAsync();
		}
		public async Task<ICollection<ProjectTask>> GetTasksOfAProjectAsync(int projectId)
		{
			//getting from db all tasks that the related project implies by
			//including the project field to the queries
			return await _context.ProjectTasks.Include(p => p.Project).Where(t => t.Project.Id == projectId).ToListAsync();
		}
		//CHECK PROJECT FOR EXISTANCE
		public async Task<bool> ProjectExistsAsync(int projectId)
		{
			return await  _context.Projects.AnyAsync(p => p.Id == projectId);
		}
		//CHECK FOR THE NAME AVAILABILITY
		public async Task<bool> ProjectNameAlreadyTakenAsync(Project project)
		{
			return await _context.Projects
				.AnyAsync(n => n.ProjectName.Trim().ToUpper() == project.ProjectName.Trim().ToUpper());
		}
		//SAVE CHANGES IN THE DB
		//returns true if "saveChanges" method returned a value > 0
		public bool Save()
		{
			var result = _context.SaveChanges();
			return result > 0;
		}
		//UPDATE THE DB WITH THE GIVEN PROJECT
		public bool UpdateProject(Project project)
		{
			_context.Update(project);
			return Save();
		}
		//GET PROJECTS INSIDE PRIORITY RANGE
		public async Task<ICollection<Project>> GetProjectsPriorityRangeAsync(int priorityLow, int priorityHigh)
		{
			return await _context.Projects.Where(p => p.Priority>priorityLow && p.Priority<priorityHigh).ToListAsync();
		}
		//GET PROJECTS THAT START AAFTER INPUT DATE
		public async Task<ICollection<Project>> GetProjectsStartAfterAsync(DateTime start)
		{
			return await _context.Projects.Where(d => d.StartDate > start).ToListAsync();
		}
		//GET PROJECTS THAT START IN THE INPUT DATES RANGE
		public async Task<ICollection<Project>> GetProjectsStartAtRangeAsync(DateTime start, DateTime end)
		{
			return await _context.Projects.Where(d => d.StartDate > start && d.StartDate < end).ToListAsync();
		}
		//GET PROJECTS ENDING BEFORE INPUT DATE
		public async Task<ICollection<Project>> GetProjectsEndBeforeAsync(DateTime end)
		{
			return await _context.Projects.Where(d => d.CompletionDate < end).ToListAsync();
		}
		//GET PROJECTS HAVING INPUT STATUS
		public async Task<ICollection<Project>> GetProjectsWithStatusAsync(ProjectStatus status)
		{
			return await _context.Projects.Where(s => s.ProjectStatus == status).ToListAsync();
		}
	}
}
