using Microsoft.AspNetCore.Http;
using PWC.Challenge.Common.Exceptions;

namespace PWC.Challenge.Application.Exceptions;

public class BusinessException(string entity, string value) :
    BadRequestException(entity, value), IExceptionMetadata
{
    public string TranslationKey => "errors.businessException";

    public int StatusCode => StatusCodes.Status400BadRequest;
}
