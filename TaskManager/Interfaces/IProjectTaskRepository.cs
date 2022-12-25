using TaskManager.Data.Enum;
using TaskManager.Models;

namespace TaskManager.Interfaces
{
	public interface IProjectTaskRepository
	{
		ICollection<ProjectTask> GetAllTasks();
		ProjectTask GetTaskById(int taskId);
		ProjectTask GetTaskByName(string taskName);
		ICollection<ProjectTask> GetTasksPriorityRange(int priorityLow, int priorityHigh);
		ICollection<ProjectTask> GetTasksWithStatus(ProjectTaskStatus status);
		bool TaskBelongsToProjectNoTracking(int taskId, int projectId);
		bool TaskNameAlreadyTaken(ProjectTask task, int projectId);
		bool ProjectTaskExists(int taskId);
		bool CreateProjectTask(ProjectTask projectTask);
		bool UpdateProjectTask(ProjectTask projectTask);
		bool DeleteProjectTask(ProjectTask projectTask);
		bool Save();
	}
}
