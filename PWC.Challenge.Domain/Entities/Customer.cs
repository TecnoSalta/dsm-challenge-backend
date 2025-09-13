using PWC.Challenge.Domain.Common;

namespace PWC.Challenge.Domain.Entities;

public class Customer : Entity
{
    public static readonly Guid FooId = Guid.Parse("ef1112d6-3447-49e7-8783-7d18d67cd073");
    public IReadOnlyList<Rental> Rentals => _rentals.AsReadOnly();
    private readonly List<Rental> _rentals = new();

    public Customer() { }

    public Customer(Guid id, string fullName, string address, string email)
    {
        Id = id;
        FullName = fullName ?? throw new ArgumentNullException(nameof(fullName));
        Address = address ?? throw new ArgumentNullException(nameof(address));
        Email = email ?? throw new ArgumentNullException(nameof(email));
    }

    public string FullName { get; private set; } = null!;
    public string Address { get; private set; } = null!;
    public string Email { get; private set; } = null!;

    public void AddRental(Rental rental)
    {
        if (rental == null) throw new ArgumentNullException(nameof(rental));
        if (rental.CustomerId != Id) throw new InvalidOperationException("Rental does not belong to this customer");

        _rentals.Add(rental);
    }
}