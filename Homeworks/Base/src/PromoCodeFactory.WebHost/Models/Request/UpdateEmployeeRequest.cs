using System;
using System.Collections.Generic;

namespace PromoCodeFactory.WebHost.Models.Request
{
    public record UpdateEmployeeRequest(
        Guid Id,
        string FirstName,
        string LastName,
        string Email,
        List<Guid> Roles,
        int AppliedPromocodesCount);
}