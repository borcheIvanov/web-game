using System.Security.Claims;
using System.Security.Principal;

namespace web_game.Identity
{
    public static class IdentityExtensions
    {
        public static string GetName(this IIdentity identity){
            return ((ClaimsIdentity)identity).FindFirst(IdentityConstants.NameType).Value;
        }

        public static string GetEmail(this IIdentity identity){
            var claim = ((ClaimsIdentity)identity).FindFirst(IdentityConstants.EmailType);

            if (claim == null){
                throw new System.Exception("email not provided");
            }

            return claim.Value;
        }
    }
}