using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Studi.Api.Proctoring.Helpers
{
    public class StudiJwtTokenValidator : ISecurityTokenValidator
    {
        private IEnumerable<SecurityKey> _secretKeys;
        private int _maximumTokenSize;
        private ISecurityTokenValidator _handler;

        public StudiJwtTokenValidator(IEnumerable<SecurityKey> keys)
        {
            _secretKeys = keys;
            _handler = new JwtSecurityTokenHandler();
        }

        public bool CanValidateToken => true;

        public int MaximumTokenSizeInBytes { get => _maximumTokenSize; set => _maximumTokenSize = value; }

        public bool CanReadToken(string securityToken)
        {
            var jwtToken = Encoding.UTF8.GetString(Convert.FromBase64String(securityToken));
            return _handler.CanReadToken(jwtToken);
        }

        public ClaimsPrincipal ValidateToken(string securityToken, TokenValidationParameters validationParameters, out SecurityToken validatedToken)
        {
            validatedToken = null;

            // Decode the token from BASE64 as the handler expects the token in normal JWT form
            var jwtToken = Encoding.UTF8.GetString(Convert.FromBase64String(securityToken));

            foreach(var key in _secretKeys)
            {
                try
                {
                    var clondeValidationParameters = validationParameters.Clone();
                    clondeValidationParameters.IssuerSigningKey = key;
                    return _handler.ValidateToken(jwtToken, clondeValidationParameters, out validatedToken);
                }
                catch (Exception ex)
                {
                    // TODO: May be here we should log the exception, just due to selection of the
                    // correct secret the exceptions are expected
                }
            }
            throw new SecurityTokenInvalidSignatureException();
        }
    }
}
