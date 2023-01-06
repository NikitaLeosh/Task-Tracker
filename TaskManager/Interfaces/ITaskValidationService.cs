using TaskManager.Models;

namespace TaskManager.Interfaces
{
	public interface ITaskValidationService
	{
		bool PriorityIsValid(int priorityLow);
		Task<bool> TaskIsValidAsync(ProjectTask task, Guid projectId);
	}
}
