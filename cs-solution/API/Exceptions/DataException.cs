namespace Backend.Exceptions;

public class DataException : Exception
{
	public DataException() : base() { }

	public DataException(string message) : base(message) { }

	public DataException(string message,  Exception innerException) : base(message, innerException) { }
}
