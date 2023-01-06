using TaskManager.Models;

namespace TaskManager.Interfaces
{
	public interface IProjectValidationService
	{
		bool PriorityIsValid(int priority);
		Task<bool> ProjectIsValidAsync(Project project);
		bool ProjectDatesAreValid(DateTime startDate, DateTime endDate);
	}
}
