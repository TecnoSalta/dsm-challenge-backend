using MediatR;
using PWC.Challenge.Application.Dtos;

namespace PWC.Challenge.Application.Features.Customers.Queries;

public record GetCustomerByDniQuery(string Dni) : IRequest<CustomerDto?>;
