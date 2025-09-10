namespace PWC.Challenge.Application.Common.Entities;

public interface IEntityDto : IBasicEntityDto
{
    DateTime CreatedAt { get; set; }

    string? CreatedBy { get; set; }

    DateTime? UpdatedAt { get; set; }

    string? UpdatedBy { get; set; }

    DateTime? DeletedAt { get; set; }

    string? DeletedBy { get; set; }

    bool IsDeleted { get; set; }
}
