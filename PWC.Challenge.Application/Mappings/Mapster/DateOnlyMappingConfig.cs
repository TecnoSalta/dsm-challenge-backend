using Mapster;

namespace PWC.Challenge.Application.Mappings.Mapster;

public static class DateOnlyMappingConfig
{
    public static void RegisterDateOnlyMappings()
    {
        TypeAdapterConfig<DateOnly, DateTimeOffset>.NewConfig()
            .MapWith(src => new DateTimeOffset(src.ToDateTime(TimeOnly.MinValue), TimeSpan.Zero));

        TypeAdapterConfig<DateTimeOffset, DateOnly>.NewConfig()
            .MapWith(src => DateOnly.FromDateTime(src.UtcDateTime));
    }
}