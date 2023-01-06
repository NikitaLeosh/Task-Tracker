using TaskManager.Models;
using TaskManager.Models.Enum;

namespace TaskManager.Interfaces
{
	public interface IProjectRepository
	{
		Task<ICollection<Project>> GetAllProjectsAsync();
		Task<Project> GetProjectByIdAsync(Guid projectId);
		Task<Project> GetProjectByNameAsync(string name);
		Task<ICollection<Project>> GetProjectsPriorityRangeAsync(int priorityLow, int priorityHigh);
		Task<ICollection<Project>> GetProjectsStartAfterAsync(DateTime start);
		Task<ICollection<Project>> GetProjectsEndBeforeAsync(DateTime end);
		Task<ICollection<Project>> GetProjectsStartAtRangeAsync(DateTime start, DateTime end);
		Task<ICollection<Project>> GetProjectsWithStatusAsync(ProjectStatus status);

		bool CreateProject(Project project);
		bool UpdateProject(Project project);
		bool DeleteProject(Project project);
		bool Save();
	}
}
