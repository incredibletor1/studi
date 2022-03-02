using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Studi.Proctoring.BackOffice_Api.Helpers
{
    public static class HttpHelper
    {
        /// <summary>
        /// Get authorization token from request header
        /// </summary>
        /// <returns></returns>
        public static StringValues GetAuthorizationTokenFromCallerToken(HttpContext httpContext)
        {
            // Init authorization token for requesting
            StringValues token = default;

            // Retrieve authorization token from caller request header
            if (!httpContext.Request.Headers.ContainsKey("Authorization")
             || !httpContext.Request.Headers.TryGetValue("Authorization", out token))
                throw new UnauthorizedAccessException("Unable to get 'Authorization' from request's headers");

            return token.ToString();
        }
    }

}
