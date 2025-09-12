﻿using MediatR;

namespace PWC.Challenge.Domain.Common;

public interface IDomainEvent:INotification
{
    Guid EventId { get; }
    DateTime OccurredOn { get; }
}