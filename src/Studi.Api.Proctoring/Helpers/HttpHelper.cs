using IO.Swagger.Client;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;
using System;

namespace Studi.Api.Proctoring.Helpers
{
    public static class HttpHelper
    {

        /// <summary>
        /// Get authorization token from request header
        /// </summary>
        /// <returns></returns>
        public static StringValues GetLmsApiClientAuthorizationTokenFromCallerToken(HttpContext httpContext)
        {

            // Init Lms.Api Client authorization token for requesting
            StringValues token = default;

#if DEBUG // TODO: to remove, set hardcoded token on DEBUG for:
            // Studi develop token:
            //token = "Bearer ZXlKaGJHY2lPaUpJVXpJMU5pSXNJblI1Y0NJNklrcFhWQ0o5LmV5SmpiR2xsYm5RaU9qSTFNVEU1TUN3aVpYaHdJam94TmpNMk1UazJPRFkwTENKdVltWWlPakUyTXpNMk1EUTROalFzSW1semN5STZJbVJsZGkxc2JYTXRjM1IxWkdrdWMzUjFaR2t1Wm5JaWZRLk5yQ3NnZGY0alhfSllOVGZaYzJVRDdnQ2laZU1yY2Z4VkRERmVDY0lybWM=";

            // local ETM token
            token = "Bearer ZXlKaGJHY2lPaUpJVXpJMU5pSXNJblI1Y0NJNklrcFhWQ0o5LmV5SmpiR2xsYm5RaU9qSTFNVEU1TUN3aVpYaHdJam94TmpNMk1UTXhPVGMwTENKdVltWWlPakUyTXpNMU16azVOelFzSW1semN5STZJbUZ3Y0M1amIyMXdkR0ZzYVdFdVkyOXRMbVJsZGlKOS5GZGtRYVdKVGJzNlpSOFk0VjZNTWZLQi0tRjZLeDl1b3hxalUybjhlVEdV";
#else
            // Retrieve authorization token from caller request header
            if (!httpContext.Request.Headers.ContainsKey("Authorization")
             || !httpContext.Request.Headers.TryGetValue("Authorization", out token))
                throw new UnauthorizedAccessException("Unable to get 'Authorization' from request's headers");
#endif
            return token.ToString();
        }
    }
}
