using System;
using Worchart.BL.Constants;

namespace Worchart.BL.Token
{
    public class TokenResponse
    {
        public string Token { get; set; }

        public DateTime CreateDate { get; set; }

        public string RefreshToken { get; set; }

        public int Duration { get; set; }

        public int RefreshDuration { get; set; }


        public bool IsExpired()
        {
            return CreateDate.AddSeconds(Duration) < DateTime.Now;
        }

        public bool CanRefresh()
        {
            return CreateDate.AddSeconds(Duration + TokenConstants.RefreshDuration) > DateTime.Now;
        }
    }
}
