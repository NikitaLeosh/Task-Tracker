using TaskManager.Exceptions;
using TaskManager.Interfaces;
using TaskManager.Models;
using TaskManager.Repositories;

namespace TaskManager.Services
{
	public class TaskValidationService : ITaskValidationService
	{
		private readonly ICheckProjectRepository _checkProjectRepository;
		private readonly ICheckTaskRepository _checkTaskRepository;

		public TaskValidationService(ICheckProjectRepository checkProjectRepository, ICheckTaskRepository checkTaskRepository)
		{
			_checkProjectRepository = checkProjectRepository;
			_checkTaskRepository = checkTaskRepository;
		}
		public bool PriorityIsValid(int priority)
		{
			if (priority < 1 || priority > 5)
				return false;
			return true;
		}

		public async Task<bool> TaskIsValidAsync(ProjectTask task, Guid projectId)
		{
			if (task == null)
				throw new NullReferenceException("input is empty");
			if (!await _checkProjectRepository.ProjectExistsAsync(projectId))
				throw new ObjectDoesNotExistException("There is no project with such number");
			if (await _checkTaskRepository.TaskNameAlreadyTakenAsync(task, projectId))
				throw new ObjectNameAlreadyTakenException(
					$"Task with a name \"{task.TaskName}\" already exists in current project.");
			if (!PriorityIsValid(task.Priority))
				throw new InvalidPriorityException("Priority must be between 1 and 5");
			return true;
		}
	}
}
