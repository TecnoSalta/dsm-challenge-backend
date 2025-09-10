﻿namespace PWC.Challenge.Domain.Common;

public interface IEntity
{
    Guid Id { get; set; }

    DateTime CreatedAt { get; set; }

    string CreatedBy { get; set; }

    DateTime? UpdatedAt { get; set; }

    string? UpdatedBy { get; set; }

    DateTime? DeletedAt { get; set; }

    string? DeletedBy { get; set; }

    bool IsDeleted { get; set; }
}
