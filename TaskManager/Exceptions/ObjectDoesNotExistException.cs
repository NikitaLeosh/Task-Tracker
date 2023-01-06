namespace TaskManager.Exceptions
{
	public class ObjectDoesNotExistException : Exception
	{
		public ObjectDoesNotExistException() : base() { }
		public ObjectDoesNotExistException(string message) : base(message) { }
	}
}
