using TaskManager.Models;

namespace TaskManager.Interfaces
{
	public interface ICheckTaskRepository
	{
		Task<bool> TaskBelongsToProjectNoTrackingAsync(Guid taskId, Guid projectId);
		Task<bool> TaskNameAlreadyTakenAsync(ProjectTask task, Guid projectId);
		Task<bool> ProjectTaskExistsAsync(Guid taskId);
	}
}
