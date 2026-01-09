using PromoCodeFactory.Core.Domain.Administration;
using PromoCodeFactory.WebHost.Models.Response;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace PromoCodeFactory.WebHost.Models;

public static class EmployeeMapping
{
    public static EmployeeResponse ToEmployeeResponse(this Employee employee)
    {
        return new EmployeeResponse()
        {
            Id = employee.Id,
            Email = employee.Email,
            FullName = employee.FullName,
            AppliedPromocodesCount = employee.AppliedPromocodesCount,
            Roles = employee.Roles.ToRolesResponse()
        };
    }

    public static RoleItemResponse ToRoleItemResponse(this Role role)
    {
        return new RoleItemResponse()
        {
            Id = role.Id,
            Name = role.Name,
            Description = role.Description
        };
    }

    public static List<RoleItemResponse> ToRolesResponse(this List<Role> roles)
    {
        List<RoleItemResponse> RolesResponse = new List<RoleItemResponse>();
        foreach (Role role in roles)
        {
            RolesResponse.Add(role.ToRoleItemResponse());
        }
        return RolesResponse;
    }
}
