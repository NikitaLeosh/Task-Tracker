namespace TaskManager.Exceptions
{
	public class PriorityOutOfRangeException : Exception
	{
		public PriorityOutOfRangeException() : base() { }
		public PriorityOutOfRangeException(string message) : base(message) { }
	}
}
