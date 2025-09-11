
namespace PWC.Challenge.Application.Dtos.Rentals
{
    public record CancelRentalDto
    {
        public Guid RentalId { get; init; }

        public CancelRentalDto(Guid rentalId)
        {
            RentalId = rentalId;
        }
    }
}
