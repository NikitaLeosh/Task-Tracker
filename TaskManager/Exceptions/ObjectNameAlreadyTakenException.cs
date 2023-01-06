namespace TaskManager.Exceptions
{
	public class ObjectNameAlreadyTakenException : Exception
	{
		public ObjectNameAlreadyTakenException() : base() { }
		public ObjectNameAlreadyTakenException(string message) : base(message) { }
	}
}
