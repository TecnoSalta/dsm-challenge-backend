
using Microsoft.AspNetCore.Mvc;
using PWC.Challenge.Api.Common.Controllers.Traditional;
using PWC.Challenge.Application.Common;
using PWC.Challenge.Application.Dtos;
using PWC.Challenge.Domain.Entities;

namespace PWC.Challenge.Api.Controllers
{
    [Route("customers")]
    public class CustomersController:BaseWriteController<Customer,CustomerDto>
    {
        public CustomersController(ILogger<BaseWriteController<Customer, CustomerDto>> logger, IBaseService<Customer, CustomerDto> service) : base(logger, service)
        {
        }

        [HttpGet("foo")]
        public  async Task<IActionResult> GetDummy()
        {
            return Ok("foo");
        }
    }
}
