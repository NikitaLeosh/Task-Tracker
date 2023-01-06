using TaskManager.Exceptions;
using TaskManager.Interfaces;
using TaskManager.Models;

namespace TaskManager.Services
{
	public class ProjectValidationService : IProjectValidationService
	{
		private readonly ICheckProjectRepository _checkProjectRepository;

		public ProjectValidationService(ICheckProjectRepository checkProjectRepository)
		{
			_checkProjectRepository = checkProjectRepository;
		}
		public bool PriorityIsValid(int priority)
		{
			if (priority > 5 || priority < 1)
				throw new PriorityOutOfRangeException("Priority must be between 1 and 5");
			return true;
		}

		public bool ProjectDatesAreValid(DateTime startDate, DateTime endDate)
		{
			if (startDate != null && endDate != null)
			{
				if (startDate < endDate)
					return true;
			}
			return false;
		}

		public async Task<bool> ProjectIsValidAsync(Project project)
		{
			if (project == null)
				throw new NullReferenceException();
			if (project.StartDate >= project.CompletionDate)
				throw new InvalidProjectDatesException("Invalid start/completion dates");
			if (await _checkProjectRepository.ProjectNameAlreadyTakenAsync(project))
				throw new ObjectNameAlreadyTakenException($"Name \"{project.ProjectName}\" is already taken. Try another one");
			if (PriorityIsValid(project.Priority))
			{
				return true;
			}
			return false;
		}
	}
}
