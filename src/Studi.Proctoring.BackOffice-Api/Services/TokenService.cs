using System;
using System.Text;
using Jose;
using Microsoft.Extensions.Options;
using Studi.Proctoring.BackOffice_Api.Models;

namespace Studi.Proctoring.BackOffice_Api.Services
{
    public class TokenService : ITokenService
    {
        private static IOptions<JwtInfo> _config;

        public TokenService(IOptions<JwtInfo> config)
        {
            _config = config;
        }

        /// <summary>
        /// Decode token
        /// </summary>
        /// <param name="token">the user token</param>
        /// <returns>returns user id</returns>
        public int GetUserIdFromToken(string token)
        {
            var tokenDecoded = this.DecodeToken(token);
            return tokenDecoded.UserId;
        }

        public string GetIssuerFromToken(string token)
        {
            var tokenDecoded = this.DecodeToken(token);
            return tokenDecoded.Issuer;
        }

        public string CreateToken(int userId, string urlSchool, TimeSpan lifetimeSpan)
        {
            var payload = new JwtPayloadBase()
            {
                nbf = DateTimeOffset.UtcNow.ToUnixTimeSeconds(),
                exp = DateTimeOffset.UtcNow.Add(lifetimeSpan).ToUnixTimeSeconds(),
                client= userId,
                iss = urlSchool
            };

            var fromBase64KeyBytes = Convert.FromBase64String(_config.Value.keyString);
            var newToken =  JWT.Encode(payload, fromBase64KeyBytes, JwsAlgorithm.HS256);
            var encodedToken = Convert.ToBase64String(Encoding.UTF8.GetBytes(newToken));
            return "Bearer " + encodedToken;
        }

        private JwtPayload DecodeToken(string jwtToken)
        {
            JwtPayload payload = null;
            try 
            {
                if(jwtToken.StartsWith("Bearer "))
                {
                    jwtToken = jwtToken.Substring(7);
                }

                var fromBase64TokenBytes = Convert.FromBase64String(jwtToken);
                var decodedFromBase64Token = Encoding.UTF8.GetString(fromBase64TokenBytes);
                var fromBase64KeyBytes = Convert.FromBase64String(_config.Value.keyString);
                payload = new JwtPayload(JWT.Decode<JwtPayloadBase>(decodedFromBase64Token, fromBase64KeyBytes, JwsAlgorithm.HS256));

                if (DateTime.UtcNow < payload.NotBefore)
                {
                    throw new UnauthorizedAccessException("Token is not yet valid");
                }

                if (DateTime.UtcNow > payload.Expiration)
                {
                    throw new UnauthorizedAccessException("Token is not longer valid");
                }

                if (payload.UserId == 0)
                {
                    throw new UnauthorizedAccessException("Token does not defined for a valid user");
                }
            }
            catch (Exception ex)
            {
                var errorMsg = "Error decoding the JWT token";
                throw new UnauthorizedAccessException(errorMsg, ex);
            }
            return payload;
        }
    }
}
