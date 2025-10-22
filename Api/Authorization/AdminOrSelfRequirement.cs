using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace Api.Authorization
{
    public class AdminOrSelfRequirement : IAuthorizationRequirement
    {
        // No properties needed for this simple check
    }
}
