namespace PWC.Challenge.Application.Common.Entities;

public abstract class EntityDto: IEntityDto
{
    public Guid Id { get; set; } = default!;

    public DateTime CreatedAt { get; set; } = default!;

    public string? CreatedBy { get; set; } = default!;

    public DateTime? UpdatedAt { get; set; } = default!;

    public string? UpdatedBy { get; set; } = default!;

    public DateTime? DeletedAt { get; set; } = default!;

    public string? DeletedBy { get; set; } = default!;

    public bool IsDeleted { get; set; } = default!;
}
