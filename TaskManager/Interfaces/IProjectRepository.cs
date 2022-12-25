using TaskManager.Data.Enum;
using TaskManager.Models;

namespace TaskManager.Interfaces
{
	public interface IProjectRepository
	{
		Task<ICollection<Project>> GetAllProjectsAsync();
		Task<Project> GetProjectByIdAsync(int projectId);
		Task<Project> GetProjectByNameAsync(string name);
		Task<ICollection<ProjectTask>> GetTasksOfAProjectAsync(int projectId);
		Task<ICollection<ProjectTask>> GetTasksOfAProjectNoTrackingAsync(int projectId);
		Task<ICollection<Project>> GetProjectsPriorityRangeAsync(int priorityLow, int priorityHigh);
		Task<ICollection<Project>> GetProjectsStartAfterAsync(DateTime start);
		Task<ICollection<Project>> GetProjectsEndBeforeAsync(DateTime end);
		Task<ICollection<Project>> GetProjectsStartAtRangeAsync(DateTime start, DateTime end);
		Task<ICollection<Project>> GetProjectsWithStatusAsync(ProjectStatus status);
		Task<bool> ProjectNameAlreadyTakenAsync(Project project);
		Task<bool> ProjectExistsAsync(int projectId);
		bool CreateProject(Project project);
		bool UpdateProject(Project project);
		bool DeleteProject(Project project);
		bool Save();
	}
}
