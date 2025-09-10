using PWC.Challenge.Domain.Common;

namespace PWC.Challenge.Domain.Entities;

public class Customer : Entity
{
    public static readonly Guid FooId = Guid.Parse("ef1112d6-3447-49e7-8783-7d18d67cd073");

    public Customer() { }

    public Customer(Guid id, string fullName, string address)
    {
        Id = id;
        FullName = fullName ?? throw new ArgumentNullException(nameof(fullName));
        Address = address ?? throw new ArgumentNullException(nameof(address));
    }

    public string FullName { get; private set; } = null!;
    public string Address { get; private set; } = null!;
}