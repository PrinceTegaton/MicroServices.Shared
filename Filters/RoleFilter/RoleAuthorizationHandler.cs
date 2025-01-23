using Microsoft.AspNetCore.Authorization;
using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace MicroServices.Filters
{
    public class RoleAuthorizationHandler
       : AuthorizationHandler<RoleAuthorizationRequirement>
    {
        protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context,
                                                             RoleAuthorizationRequirement requirement)
        {
            var userRole = context.User.Claims.FirstOrDefault(a => a.Type == ClaimTypes.Role)?.Value;
            bool authorized = requirement.RequiredRoles.AsParallel().Any(r => r == Enum.Parse<UserRoleName>(userRole));

            if (authorized)
            {
                context.Succeed(requirement);
            }

            await Task.FromResult(0);
        }
    }

    public class RoleDenyAuthorizationHandler
       : AuthorizationHandler<RoleDenyAuthorizationRequirement>
    {
        protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context,
                                                             RoleDenyAuthorizationRequirement requirement)
        {
            var userRole = context.User.Claims.FirstOrDefault(a => a.Type == ClaimTypes.Role)?.Value;
            var role = Enum.Parse<UserRoleName>(userRole);

            bool denied = requirement.DeniedRoles.AsParallel().Any(r => r == role);

            if (denied)
            {
                context.Succeed(requirement);
            }

            await Task.FromResult(0);
        }
    }
}