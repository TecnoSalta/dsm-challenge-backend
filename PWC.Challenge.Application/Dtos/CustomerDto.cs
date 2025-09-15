
using PWC.Challenge.Application.Common.Entities;

namespace PWC.Challenge.Application.Dtos;

public class CustomerDto : EntityDto
{
    public string Dni { get; set; }
    public string FullName { get;  set; }
    public string Address { get;  set; }


}
