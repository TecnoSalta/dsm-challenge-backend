using MediatR;
using PWC.Challenge.Application.Dtos;
using System.Collections.Generic;

namespace PWC.Challenge.Application.Features.Cars.Queries.GetCars;

public record GetCarsQuery() : IRequest<IReadOnlyList<CarDto>>;
