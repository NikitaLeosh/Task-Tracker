namespace TaskManager.Exceptions
{
	public class TaskDoesNotBelongToProjectException : Exception
	{
		public TaskDoesNotBelongToProjectException() : base() { }
		public TaskDoesNotBelongToProjectException(string message) : base(message) { }
	}
}
