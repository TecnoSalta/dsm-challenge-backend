using PWC.Challenge.Domain.Common;
using PWC.Challenge.Domain.Entities;

namespace PWC.Challenge.Domain.Interfaces;

/// <summary>
/// Repositorio especializado para acceso y validación de disponibilidad de autos.
/// </summary>
public interface ICarRepository : IBaseRepository<Car>
{
    /// <summary>
    /// Obtiene la lista de autos disponibles en el rango de fechas indicado,
    /// aplicando opcionalmente filtros por tipo y modelo.
    /// Considera reglas de negocio: solapamiento de reservas, servicios programados
    /// y día de bloqueo posterior a cada alquiler.
    /// </summary>
    Task<List<Car>> GetAvailableCarsAsync(
        DateOnly startDate,
        DateOnly endDate,
        string? carType = null,
        string? model = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Verifica si un auto específico está disponible en un rango de fechas.
    /// Permite excluir una reserva actual (para escenarios de modificación).
    /// </summary>
    Task<bool> IsCarAvailableAsync(
        Guid carId,
        DateOnly startDate,
        DateOnly endDate,
        Guid? excludedRentalId = null,
        CancellationToken cancellationToken = default);

    Task<List<Car>> GetNextCarServicesAsync(int nextDays);
}