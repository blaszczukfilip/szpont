using System.Linq;
using System.Security.Claims;

namespace szpont.Helpers
{
    public static class ClaimsPrincipalExtensions
    {
        // sprawdza czy uzytkownik ma podana role
        public static bool HasRole(this ClaimsPrincipal? user, string roleName)
        {
            if (user?.Identity?.IsAuthenticated != true || string.IsNullOrWhiteSpace(roleName))
            {
                return false;
            }

            return user.Claims.Any(c =>
                c.Type == ClaimTypes.Role &&
                string.Equals(c.Value, roleName, StringComparison.OrdinalIgnoreCase));
        }
        // sprawdza czy uztykownik ma jakaikolwiek z podanych roli
        public static bool HasAnyRole(this ClaimsPrincipal? user, params string[] roleNames)
        {
            if (user?.Identity?.IsAuthenticated != true || roleNames == null || roleNames.Length == 0)
            {
                return false;
            }

            return roleNames.Any(role => user.HasRole(role));
        }

    }
}

