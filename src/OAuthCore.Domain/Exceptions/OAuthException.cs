namespace OAuthCore.Domain.Exceptions;

public class OAuthException : Exception
{
    public int StatusCode { get; }

    public OAuthException(string message, int statusCode) : base(message)
    {
        StatusCode = statusCode;
    }
}

public class InvalidClientException : OAuthException
{
    public InvalidClientException(string message) : base(message, 401) { }
}

public class InvalidGrantException : OAuthException
{
    public InvalidGrantException(string message) : base(message, 400) { }
}

// Add more custom exceptions as needed