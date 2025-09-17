using PWC.Challenge.Domain.Common;
using PWC.Challenge.Domain.Enums;

namespace PWC.Challenge.Domain.Events;

public class CarReservedDomainEvent(Guid CarId, Guid RentalId, DateOnly StartDate, DateOnly EndDate) : DomainEvent;
public class CarReservationCancelledDomainEvent(Guid CarId, Guid RentalId) : DomainEvent;
public class CarPickedUpDomainEvent(Guid CarId, Guid RentalId, DateTime PickedUpDate) : DomainEvent;
public class CarReturnedDomainEvent(Guid CarId, Guid RentalId, DateTime ReturnedDate) : DomainEvent;
public class CarCleaningCompletedDomainEvent(Guid CarId, DateTime CleaningCompletedDate) : DomainEvent;
public class CarSentToMaintenanceDomainEvent(Guid CarId, CarStatus PreviousStatus, string Reason, DateTime SentToMaintenanceDate) : DomainEvent;
public class CarMaintenanceCompletedDomainEvent(Guid CarId, DateTime MaintenanceCompletedDate) : DomainEvent;
public class CarStatusForcedDomainEvent(Guid CarId, CarStatus PreviousStatus, CarStatus NewStatus, string Reason, DateTime ForcedDate) : DomainEvent;
public class CarRentalAddedDomainEvent(Guid CarId, Guid RentalId, DateOnly StartDate, DateOnly EndDate) : DomainEvent;
public class CarAssignedToActiveRentalDomainEvent(Guid CarId, Guid RentalId, DateOnly StartDate, DateOnly EndDate) : DomainEvent;