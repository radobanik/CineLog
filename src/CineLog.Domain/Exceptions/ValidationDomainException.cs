namespace CineLog.Domain.Exceptions;

public class ValidationDomainException : DomainException
{
    public ValidationDomainException(string message) : base(message) { }
}
