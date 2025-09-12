using FluentAssertions;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using PWC.Challenge.Application.Features.Cars.EventHandlers;
using PWC.Challenge.Domain.Common;
using PWC.Challenge.Domain.Entities;
using PWC.Challenge.Domain.Events.Rentals;

namespace UnitTests.Common;
public class MediatRHandlerRegistrationTests
{
    [Fact]
    public void MakeCarAvailableOnRentalCancelledHandler_ShouldBeRegistered()
    {
        // Arrange
        var services = new ServiceCollection();

        // Mock del repositorio
        var carRepoMock = new Mock<IBaseRepository<Car>>();
        services.AddSingleton(carRepoMock.Object);

        // Registrar MediatR apuntando al ensamblado donde está el handler
        var applicationAssembly = typeof(MakeCarAvailableOnRentalCancelledHandler).Assembly;
        services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(applicationAssembly));

        var provider = services.BuildServiceProvider();

        // Act
        var handlers = provider.GetServices<INotificationHandler<RentalCancelledDomainEvent>>();

        // Assert
        handlers.Should().ContainSingle(h => h.GetType() == typeof(MakeCarAvailableOnRentalCancelledHandler));
    }
}
