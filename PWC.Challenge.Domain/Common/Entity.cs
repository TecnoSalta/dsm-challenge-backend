namespace PWC.Challenge.Domain.Common;

public abstract class Entity : IEntity
{
    public Guid Id { get; set; } = Guid.NewGuid();

    public DateTime CreatedAt { get; set; } = default!;

    public string CreatedBy { get; set; } = default!;

    public DateTime? UpdatedAt { get; set; } = default!;

    public string? UpdatedBy { get; set; } = default!;

    public DateTime? DeletedAt { get; set; } = default!;

    public string? DeletedBy { get; set; } = default!;

    public bool IsDeleted { get; set; } = default!;
}
