namespace PWC.Challenge.Application.Features.Rentals.Commands.CompleteRental
{
    public record CompleteRentalDto(Guid RentalId)
    {
        public string Message { get; set; } = string.Empty;
    }
}