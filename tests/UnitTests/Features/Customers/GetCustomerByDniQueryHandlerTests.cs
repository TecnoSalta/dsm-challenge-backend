using FluentAssertions;
using Moq;
using PWC.Challenge.Application.Features.Customers.Queries;
using PWC.Challenge.Domain.Entities;
using PWC.Challenge.Domain.Interfaces;

namespace UnitTests.Features.Customers;

public class GetCustomerByDniQueryHandlerTests
{
    private readonly Mock<ICustomerRepository> _customerRepositoryMock;
    private readonly GetCustomerByDniQueryHandler _handler;

    public GetCustomerByDniQueryHandlerTests()
    {
        _customerRepositoryMock = new Mock<ICustomerRepository>();
        _handler = new GetCustomerByDniQueryHandler(_customerRepositoryMock.Object);
    }

    [Fact]
    public async Task Handle_GivenExistingDni_ReturnsCustomerDto()
    {
        // Arrange
        var dni = "1122";
        var customer = new Customer(Guid.NewGuid(), dni, "Juan Pérez", "Av. Siempreviva 742", "juan.perez@example.com");
        _customerRepositoryMock.Setup(repo => repo.GetByDniAsync(dni)).ReturnsAsync(customer);

        var query = new GetCustomerByDniQuery(dni);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Dni.Should().Be(dni);
        result.FullName.Should().Be("Juan Pérez");
        result.Address.Should().Be("Av. Siempreviva 742");
    }

    [Fact]
    public async Task Handle_GivenNonExistingDni_ReturnsNull()
    {
        // Arrange
        var dni = "9999";
        _customerRepositoryMock.Setup(repo => repo.GetByDniAsync(dni)).ReturnsAsync((Customer?)null);

        var query = new GetCustomerByDniQuery(dni);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().BeNull();
    }
}
