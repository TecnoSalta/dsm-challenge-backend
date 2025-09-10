using Microsoft.AspNetCore.Http;
using PWC.Challenge.Common.Exceptions;

namespace PWC.Challenge.Application.Exceptions;

public class EntityNotFoundException<TPrimitiveId> : NotFoundException, IExceptionMetadata
{
    public EntityNotFoundException(string entity, TPrimitiveId id) : base(entity, id)
    {
    }

    public string TranslationKey => "errors.entityNotFoundException";

    public int StatusCode => StatusCodes.Status404NotFound;
}
