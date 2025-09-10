namespace PWC.Challenge.Common.Exceptions;

public interface IExceptionMetadata
{
    string TranslationKey { get; }
    int StatusCode { get; }
}
