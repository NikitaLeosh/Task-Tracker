namespace TaskManager.Exceptions
{
	[Serializable]
	public class InvalidProjectDatesException : Exception
	{
		public InvalidProjectDatesException() : base() { }
		public InvalidProjectDatesException(string message) : base(message){ }

	}
}
