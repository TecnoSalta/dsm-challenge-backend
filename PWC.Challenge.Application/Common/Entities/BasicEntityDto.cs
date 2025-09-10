namespace PWC.Challenge.Application.Common.Entities;

public abstract class BasicEntityDto: IBasicEntityDto
{
    public Guid Id { get; set; } = default!;
}
