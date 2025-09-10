﻿using MediatR;

namespace PWC.Challenge.Common.CQRS;

public interface IQuery<out TResponse> : IRequest<TResponse> where TResponse : notnull
{
}
