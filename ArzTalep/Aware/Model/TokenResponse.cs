using Aware.Util.Constants;
using System;

namespace Aware.Model
{
    public class TokenResponse
    {
        public string Token { get; set; }

        public string RefreshToken { get; set; }

        public DateTime Expires { get; set; }

        public int RefreshDuration { get; set; }

        public bool IsExpired()
        {
            return Expires < DateTime.Now;
        }

        public bool CanRefresh()
        {
            return Expires.AddSeconds(TokenConstants.RefreshDuration) > DateTime.Now;
        }
    }
}
