using TaskManager.Data.Enum;
using TaskManager.Models;

namespace TaskManager.Interfaces
{
	public interface IProjectRepository
	{
		ICollection<Project> GetAllProjects();
		Project GetProjectById(int projectId);
		Project GetProjectByName(string name);
		ICollection<ProjectTask> GetTasksOfAProject(int projectId);
		ICollection<ProjectTask> GetTasksOfAProjectNoTracking(int projectId);
		ICollection<Project> GetProjectsPriorityRange(int priorityLow, int priorityHigh);
		ICollection<Project> GetProjectsStartAfter(DateTime start);
		ICollection<Project> GetProjectsEndBefore(DateTime end);
		ICollection<Project> GetProjectsStartAtRange(DateTime start, DateTime end);
		ICollection<Project> GetProjectsWithStatus(ProjectStatus status);
		bool ProjectNameAlreadyTaken(Project project);
		bool ProjectExists(int projectId);
		bool CreateProject(Project project);
		bool UpdateProject(Project project);
		bool DeleteProject(Project project);
		bool Save();
	}
}
