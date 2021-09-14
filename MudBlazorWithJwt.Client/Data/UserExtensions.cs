using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace MudBlazorWithJwt.Client.Data
{
    public static class UserExtensions
    {
        public static string FirstName(this ClaimsPrincipal user)
            => user.FindFirst("FirstName").Value;

        public static string LastName(this ClaimsPrincipal user)
            => user.FindFirst("LastName").Value;

        public static string Email(this ClaimsPrincipal user)
            => user.Identity.Name;
    }
}
