using PWC.Challenge.Application.Dtos.Rentals;

namespace PWC.Challenge.Application.Features.Rentals.Commands.CompleteRental.Services;

public interface ICompleteRentalService
{
    /// <summary>
    /// Marca un alquiler como completado y aplica las reglas de negocio (cooldown, service, etc.).
    /// </summary>
    /// <param name="rentalId">Identificador del rental</param>
    /// <param name="actualReturnDate">Fecha real de devolución, si no se pasa se usa UtcNow</param>
    /// <param name="cancellationToken">Token de cancelación</param>
    /// <returns>El rental actualizado</returns>
    Task<CompletedRentalDto> CompleteAsync(Guid rentalId, DateOnly? actualReturnDate, CancellationToken cancellationToken = default);
}
