namespace PWC.Challenge.Application.Features.Cars.Queries.GetCarMetadata;

public class CarMetadataDto
{
    public string Type { get; set; } = null!;
    public List<string> Models { get; set; } = new List<string>();
}