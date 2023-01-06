using TaskManager.Models;

namespace TaskManager.Interfaces
{
	public interface ICheckProjectRepository
	{

		Task<bool> ProjectNameAlreadyTakenAsync(Project project);
		Task<bool> ProjectExistsAsync(Guid projectId);

	}
}
