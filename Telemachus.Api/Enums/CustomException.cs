using System;

public class CustomException : Exception
{
    public CustomException() : base("A custom error occurred") { }

    public CustomException(string message) : base(message) { }

    public CustomException(string message, Exception innerException) : base(message, innerException) { }
}
