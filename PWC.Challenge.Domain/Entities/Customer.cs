using PWC.Challenge.Domain.Common;
namespace PWC.Challenge.Domain.Entities;

public class Customer : Entity
{
    public static readonly Guid  FooId  = Guid.Parse("d290f1ee-6c54-4b01-90e6-d701748f0851");
    public string FullName { get; private set; }
    public string Address { get; private set; }


    public Customer(Guid id, string fullName, string address)
    {
        Id = id;
        FullName = fullName ?? throw new ArgumentNullException(nameof(fullName));
        Address = address ?? throw new ArgumentNullException(nameof(address));

        // Ejemplo de evento de dominio
    }

    public static Customer[] Create(Guid fooId, string v1, string v2, DateTime utcNow, object value)
    {
        throw new NotImplementedException();
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