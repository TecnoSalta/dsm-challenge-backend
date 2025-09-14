
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using PWC.Challenge.Api.Common.Controllers.Traditional;
using PWC.Challenge.Application.Common;
using PWC.Challenge.Application.Dtos;
using PWC.Challenge.Application.Features.Customers.Commands.CreateCustomer;
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
    }
}
