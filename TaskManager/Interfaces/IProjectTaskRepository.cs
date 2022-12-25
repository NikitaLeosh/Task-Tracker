using TaskManager.Data.Enum;
using TaskManager.Models;

namespace TaskManager.Interfaces
{
	public interface IProjectTaskRepository
	{
		Task<ICollection<ProjectTask>> GetAllTasksAsync();
		Task<ProjectTask> GetTaskByIdAsync(int taskId);
		Task<ProjectTask> GetTaskByNameAsync(string taskName);
		Task<ICollection<ProjectTask>> GetTasksPriorityRangeAsync(int priorityLow, int priorityHigh);
		Task<ICollection<ProjectTask>> GetTasksWithStatusAsync(ProjectTaskStatus status);
		Task<bool> TaskBelongsToProjectNoTrackingAsync(int taskId, int projectId);
		Task<bool> TaskNameAlreadyTakenAsync(ProjectTask task, int projectId);
		Task<bool> ProjectTaskExistsAsync(int taskId);
		bool CreateProjectTask(ProjectTask projectTask);
		bool UpdateProjectTask(ProjectTask projectTask);
		bool DeleteProjectTask(ProjectTask projectTask);
		bool Save();
	}
}
