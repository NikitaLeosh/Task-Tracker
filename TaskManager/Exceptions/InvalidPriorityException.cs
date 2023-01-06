namespace TaskManager.Exceptions
{
	public class InvalidPriorityException : Exception
	{
		public InvalidPriorityException() : base() { }
		public InvalidPriorityException(string message) : base(message) { }
	}
}
