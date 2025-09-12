using MediatR;
using PWC.Challenge.Domain.Entities;
using PWC.Challenge.Domain.Rentals;
using PWC.Challenge.Application.Common;
using PWC.Challenge.Domain.Common;

namespace PWC.Challenge.Application.Features.Cars.EventHandlers
{
    // Handler para hacer disponible el coche cuando se cancela un rental
    public class MakeCarAvailableOnRentalCancelledHandler
        : INotificationHandler<RentalCancelledDomainEvent>
    {
        private readonly IBaseRepository<Car> _carRepository;

        // Inyección de dependencias correcta
        public MakeCarAvailableOnRentalCancelledHandler(IBaseRepository<Car> carRepository)
        {
            _carRepository = carRepository;
        }

        public async Task Handle(RentalCancelledDomainEvent notification, CancellationToken cancellationToken)
        {
            var car = await _carRepository.GetByIdAsync(
                notification.CarId,
                asNoTracking: false,
                cancellationToken: cancellationToken
            );

            if (car is null) return;

            // Marcamos el coche como disponible
            car.MarkAsAvailable();

            await _carRepository.UpdateAsync(car, saveChanges: true, cancellationToken);
        }
    }
}
