using TaskManager.Models;
using TaskManager.Models.Enum;

namespace TaskManager.Interfaces
{
	public interface IProjectTaskRepository
	{
		Task<ICollection<ProjectTask>> GetAllTasksAsync();
		Task<ProjectTask> GetTaskByIdAsync(Guid taskId);
		Task<ProjectTask> GetTaskByNameAsync(string taskName);
		Task<ICollection<ProjectTask>> GetTasksPriorityRangeAsync(int priorityLow, int priorityHigh);
		Task<ICollection<ProjectTask>> GetTasksWithStatusAsync(ProjectTaskStatus status);
		Task<ICollection<ProjectTask>> GetTasksOfAProjectAsync(Guid projectId);

		bool CreateProjectTask(ProjectTask projectTask);
		bool UpdateProjectTask(ProjectTask projectTask);
		bool DeleteProjectTask(ProjectTask projectTask);
		bool Save();
	}
}
