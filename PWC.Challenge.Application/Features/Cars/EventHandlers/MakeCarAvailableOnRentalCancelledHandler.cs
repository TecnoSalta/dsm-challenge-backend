using MediatR;
using PWC.Challenge.Application.Features.Rentals.Events;
using PWC.Challenge.Domain.Common;
using PWC.Challenge.Domain.Entities;

namespace PWC.Challenge.Application.Features.Cars.EventHandlers
{
    public class MakeCarAvailableOnRentalCancelledHandler : INotificationHandler<RentalCancelledDomainEvent>
    {
        private readonly IBaseRepository<Car> _carRepository;

        public MakeCarAvailableOnRentalCancelledHandler(IBaseRepository<Car> carRepository)
        {
            _carRepository = carRepository;
        }

        public async Task Handle(RentalCancelledDomainEvent notification, CancellationToken cancellationToken)
        {
            var car = await _carRepository.GetByIdAsync(notification.CarId, cancellationToken);
            if (car == null)
                throw new InvalidOperationException("Car not found.");

            car.MarkAsAvailable();

            await _carRepository.UpdateAsync(car, cancellationToken);
        }
    }
}
