
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using PWC.Challenge.Api.Common.Controllers.Traditional;
using PWC.Challenge.Application.Common;
using PWC.Challenge.Application.Dtos;
using PWC.Challenge.Application.Features.Customers.Commands.CreateCustomer;
using PWC.Challenge.Application.Features.Customers.Queries; // Added for GetCustomerByDniQuery
using PWC.Challenge.Domain.Entities;

namespace PWC.Challenge.Api.Controllers
{
    [Authorize]
    [Route("customers")]
    public class CustomersController:BaseWriteController<Customer,CustomerDto>
    {
        private readonly ISender sender;

        public CustomersController(
            ILogger<CustomersController> logger, 
            IBaseService<Customer, CustomerDto> service,
            ISender sender) 
            : base(logger, service)
        {
            this.sender = sender;
        }

        [Authorize(Roles = "Admin")]
        [HttpPost("create")]
        public  async Task<IActionResult> Create(CustomerDto dto)
        {
            var command= new CreateCustomerCommand(dto);
            var response = await sender.Send(command);
            return Ok(response);
        }

        [HttpGet("{dni}")]
        public async Task<IActionResult> GetByDni(string dni)
        {
            var query = new GetCustomerByDniQuery(dni);
            var customer = await sender.Send(query);

            if (customer == null)
            {
                return NotFound(new
                {
                    status = 404,
                    title = "Customer not found",
                    detail = $"No customer was found with DNI {dni}"
                });
            }

            return Ok(customer);
        }
    }
}

