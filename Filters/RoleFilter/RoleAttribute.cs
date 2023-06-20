using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MicroServices.Filters
{
    public class RoleAttribute : TypeFilterAttribute
    {
        public RoleAttribute(params UserRoleName[] role)
          : base(typeof(RoleAuthorizationRequirementImpl))
        {
            Arguments = new[] { new RoleAuthorizationRequirement(role) };
        }

        private class RoleAuthorizationRequirementImpl : Attribute, IAsyncResourceFilter
        {
            private readonly IAuthorizationService _authService;
            private readonly RoleAuthorizationRequirement _requiredRole;

            public RoleAuthorizationRequirementImpl(IAuthorizationService authService,
                                                    RoleAuthorizationRequirement requiredRole)
            {
                _authService = authService;
                _requiredRole = requiredRole;
            }

            public async Task OnResourceExecutionAsync(ResourceExecutingContext context,
                                                       ResourceExecutionDelegate next)
            {
                // grant "business admin" and "all access viewer" rights on all GET

                bool hasReadAccess = false;
                if (context.HttpContext.Request.Method.ToUpper() == "GET")
                {
                    var userRole = context.HttpContext.User.Claims.FirstOrDefault(a => a.Type == System.Security.Claims.ClaimTypes.Role)?.Value;
                    hasReadAccess = new[] { UserRoleName.AllAccessViewer }
                                            .Contains(Enum.Parse<UserRoleName>(userRole));
                }

                if (!hasReadAccess && !(await _authService.AuthorizeAsync(context.HttpContext.User,
                                          context.ActionDescriptor.ToString(),
                                          _requiredRole)).Succeeded)
                {
                    context.Result = new ForbidResult();
                    return;
                }

                await next();
            }
        }
    }

    public class RoleDenyAttribute : TypeFilterAttribute
    {
        public RoleDenyAttribute(params UserRoleName[] role)
          : base(typeof(RoleAuthorizationRequirementImpl))
        {
            Arguments = new[] { new RoleDenyAuthorizationRequirement(role) };
        }

        private class RoleAuthorizationRequirementImpl : Attribute, IAsyncResourceFilter
        {
            private readonly IAuthorizationService _authService;
            private readonly RoleDenyAuthorizationRequirement _deniedRole;

            public RoleAuthorizationRequirementImpl(IAuthorizationService authService,
                                                    RoleDenyAuthorizationRequirement requiredRole)
            {
                _authService = authService;
                _deniedRole = requiredRole;
            }

            public async Task OnResourceExecutionAsync(ResourceExecutingContext context,
                                                       ResourceExecutionDelegate next)
            {
                if ((await _authService.AuthorizeAsync(context.HttpContext.User,
                                            context.ActionDescriptor.ToString(),
                                            _deniedRole)).Succeeded)
                {
                    context.Result = new ForbidResult();
                    return;
                }

                await next();
            }
        }
    }



    public class RoleAuthorizationRequirement : IAuthorizationRequirement
    {
        public IEnumerable<UserRoleName> RequiredRoles { get; }

        public RoleAuthorizationRequirement(IEnumerable<UserRoleName> requiredRoles)
        {
            RequiredRoles = requiredRoles;
        }
    }

    public class RoleDenyAuthorizationRequirement : IAuthorizationRequirement
    {
        public IEnumerable<UserRoleName> DeniedRoles { get; }

        public RoleDenyAuthorizationRequirement(IEnumerable<UserRoleName> deniedRoles)
        {
            DeniedRoles = deniedRoles;
        }
    }
}