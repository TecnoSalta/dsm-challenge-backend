using System;
using System.Threading;
using System.Threading.Tasks;

namespace PWC.Challenge.Domain.Services;

public interface IRentalAvailabilityService
{
    Task<bool> IsCarAvailableAsync(Guid carId, DateOnly startDate, DateOnly endDate, Guid? excludedRentalId = null, CancellationToken cancellationToken = default);
}