
namespace PWC.Challenge.Application.Dtos.Rentals
{
    public record CompletedRentalDto
    {
        public Guid RentalId { get; init; }

        public CompletedRentalDto(Guid rentalId)
        {
            RentalId = rentalId;
        }
    }
}
