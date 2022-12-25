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
		public ICollection<Project> GetAllProjects()
		{
			return _context.Projects.Include(t => t.Tasks).ToList();
		}
		//GET ONE BY ID
		public Project GetProjectById(int projectId)
		{
			return _context.Projects.Where(p => p.Id == projectId).FirstOrDefault();
			
		}

		public Project GetProjectByName(string name)
		{
			return _context.Projects.FirstOrDefault
				(n => n.ProjectName.Trim().ToUpper() == name.Trim().ToUpper());
		}

		//GET TASKS OF GIVEN PROJECT
		//No tracking modifiction created for update method to avoid conflicting IDs
		public ICollection<ProjectTask> GetTasksOfAProjectNoTracking(int projectId)
		{
			//getting from db all tasks that the related project implies by
			//including the project field to the queries
			return _context.ProjectTasks.Include(p => p.Project).AsNoTracking().Where(t => t.Project.Id == projectId).ToList();
		}
		public ICollection<ProjectTask> GetTasksOfAProject(int projectId)
		{
			//getting from db all tasks that the related project implies by
			//including the project field to the queries
			return _context.ProjectTasks.Include(p => p.Project).Where(t => t.Project.Id == projectId).ToList();
		}
		//CHECK PROJECT FOR EXISTANCE
		public bool ProjectExists(int projectId)
		{
			return _context.Projects.Any(p => p.Id == projectId);
		}
		//CHECK FOR THE NAME AVAILABILITY
		public bool ProjectNameAlreadyTaken(Project project)
		{
			return _context.Projects
				.Any(n => n.ProjectName.Trim().ToUpper() == project.ProjectName.Trim().ToUpper());
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
		public ICollection<Project> GetProjectsPriorityRange(int priorityLow, int priorityHigh)
		{
			return _context.Projects.Where(p => p.Priority>priorityLow && p.Priority<priorityHigh).ToList();
		}
		//GET PROJECTS THAT START AAFTER INPUT DATE
		public ICollection<Project> GetProjectsStartAfter(DateTime start)
		{
			return _context.Projects.Where(d => d.StartDate > start).ToList();
		}
		//GET PROJECTS THAT START IN THE INPUT DATES RANGE
		public ICollection<Project> GetProjectsStartAtRange(DateTime start, DateTime end)
		{
			return _context.Projects.Where(d => d.StartDate > start && d.StartDate < end).ToList();
		}
		//GET PROJECTS ENDING BEFORE INPUT DATE
		public ICollection<Project> GetProjectsEndBefore(DateTime end)
		{
			return _context.Projects.Where(d => d.CompletionDate < end).ToList();
		}
		//GET PROJECTS HAVING INPUT STATUS
		public ICollection<Project> GetProjectsWithStatus(ProjectStatus status)
		{
			return _context.Projects.Where(s => s.ProjectStatus == status).ToList();
		}
	}
}
