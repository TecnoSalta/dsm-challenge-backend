using Microsoft.AspNetCore.Http;
using PWC.Challenge.Common.Exceptions;

namespace PWC.Challenge.Application.Exceptions;

public class EntityAlreadyExistsException<TPrimitiveId> : AlreadyExistsException, IExceptionMetadata
{
    public EntityAlreadyExistsException(string entity, TPrimitiveId id) : base(entity, id)
    {
    }

    public EntityAlreadyExistsException(string entity, string value) : base(entity, value)
    {
    }

    public string TranslationKey => "errors.entityAlreadyExistsException";

    public int StatusCode => StatusCodes.Status400BadRequest;
}
