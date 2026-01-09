using System;
using System.Collections.Generic;

namespace PromoCodeFactory.WebHost.Models.Request
{
    public record CreateEmployeeRequest(
        string FirstName,
        string LastName,
        string Email,
        List<Guid> Roles);
}