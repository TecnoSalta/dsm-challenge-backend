using Microsoft.EntityFrameworkCore;
using PWC.Challenge.Application.Dtos;
using PWC.Challenge.Common.CQRS;
using PWC.Challenge.Common.Extensions;
using PWC.Challenge.Domain.Common;
using PWC.Challenge.Domain.Entities;

namespace PWC.Challenge.Application.Features.Querys.GetAvailableCars
{
    public class GetAvailableCarsQueryHandler
    : IQueryHandler<GetAvailableCarsQuery, IReadOnlyList<AvailableCarDto>>
    {
        private readonly IBaseRepository<Car> _carRepo;
        //TODO private readonly IRepository<Rental> _rentalRepo;
        //TODO private readonly IRepository<Service> _serviceRepo;

        public GetAvailableCarsQueryHandler(
            IBaseRepository<Car> carRepo)
        {
            _carRepo = carRepo;
        }

        public async Task<IReadOnlyList<AvailableCarDto>> Handle(
            GetAvailableCarsQuery request,
            CancellationToken ct)
        {
            var (pickup, returnD, type, model) = request.Filter;

            // 1. Coches activos
            var carsQ = _carRepo.Query()
                        .Where(c => c.Status == "available");

           
            var available = await carsQ
                .WhereIf(!string.IsNullOrWhiteSpace(type), c => c.Type == type)
                .WhereIf(!string.IsNullOrWhiteSpace(model), c => c.Model.Contains(model))
                .Select(c => new AvailableCarDto(
                    c.Id,
                    c.Type,
                    c.Model))
                .ToListAsync(ct);

            return available;
        }
    }
}
