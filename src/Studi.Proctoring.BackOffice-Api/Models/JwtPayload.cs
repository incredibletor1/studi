using Newtonsoft.Json;
using System;

namespace Studi.Proctoring.BackOffice_Api.Models
{
    public class JwtPayloadBase
    {
        public int client { get; set; }
        public long exp { get; set; }
        public long nbf { get; set; }
        public string iss { get; set; }
    }

    // Copy/Paste from LMS.Authentication (Lms.Api)
    public class JwtPayload
    {
        public JwtPayload(JwtPayloadBase originalPayload)
        {
            if (originalPayload != null)
            {
                this.UserId = originalPayload.client;
                this.ExpirationUnixTimeStamp = originalPayload.exp;
                this.NotBeforeUnixTimeStamp = originalPayload.nbf;
                this.Issuer = originalPayload.iss;
            }
        }

        [JsonProperty("client")]
        public int UserId { get; set; }

        [JsonProperty("exp")]
        public long ExpirationUnixTimeStamp
        {
            get => new DateTimeOffset(Expiration).ToUnixTimeSeconds();
            set => Expiration = DateTimeOffset.FromUnixTimeSeconds(value).UtcDateTime;
        }

        [JsonProperty("nbf")]
        public long NotBeforeUnixTimeStamp
        {
            get => new DateTimeOffset(NotBefore).ToUnixTimeSeconds();
            set => NotBefore = DateTimeOffset.FromUnixTimeSeconds(value).UtcDateTime;
        }

        [JsonProperty("iss")]
        public string Issuer { get; set; }


        [JsonIgnore]
        public DateTime NotBefore { get; set; }

        [JsonIgnore]
        public DateTime Expiration { get; set; }
    }
}
