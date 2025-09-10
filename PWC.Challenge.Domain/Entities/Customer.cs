using PWC.Challenge.Domain.Common;
using PWC.Challenge.Domain.Events;
namespace PWC.Challenge.Domain.Entities;

public class Customer : Entity, IAggregateRoot
{
    public string FullName { get; private set; }
    public string Address { get; private set; }
    public DateTime RegistrationDate { get; private set; }

    // Constructor privado para EF Core
    private Customer() { }

    public Customer( string fullName, string address)
    {
        FullName = fullName ?? throw new ArgumentNullException(nameof(fullName));
        Address = address ?? throw new ArgumentNullException(nameof(address));
        RegistrationDate = DateTime.UtcNow;

        // Ejemplo de evento de dominio
        AddDomainEvent(new CustomerRegisteredEvent(Id));
    }

    //TODO Métodos de negocio
   /* public void UpdateAddress(string newAddress)
    {
        if (string.IsNullOrWhiteSpace(newAddress))
            throw new ArgumentException("La dirección no puede estar vacía");

        Address = newAddress;
        AddDomainEvent(new CustomerAddressUpdatedEvent(Id, newAddress));
    }*/
}