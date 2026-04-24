using System;
using System.Linq;
using System.Security.Claims;

namespace Telemachus.Helpers
{
    public static class ClaimHelper
    {
        public static string GetUserId(ClaimsPrincipal claimsPrincipal)
        {
            var idClaim = claimsPrincipal.Claims.FirstOrDefault(a => a.Type == ClaimTypes.NameIdentifier);
            if (idClaim != null)
            {
                return idClaim.Value;
            }
            return string.Empty;
        }
        public static string GetUserName(ClaimsPrincipal claimsPrincipal)
        {
            var nameClaim = claimsPrincipal.Claims.FirstOrDefault(a => a.Type.Equals("UserName", StringComparison.OrdinalIgnoreCase));
            if (nameClaim != null)
            {
                return nameClaim.Value;
            }

            return string.Empty;
        }
        public static bool IsAdmin(ClaimsPrincipal claimsPrincipal)
        {
            return claimsPrincipal.IsInRole("Admin");
        }
    }
}
