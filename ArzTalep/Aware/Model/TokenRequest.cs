using Aware.Util;
using Aware.Util.Constants;
using System;

namespace Aware.Model
{
    public class TokenRequest
    {
        public string ClientID { get; set; }

        public string AuthorizeToken { get; set; }

        public TokenType Type { get; set; }

        public string RequestTime { get; set; }

        public string TokenHash { get; set; }

        public string RequestIp { get; set; }

        public string ApiKey { get; set; } //For public applications

        public string GetDescription()
        {
            return string.Format("{0}-{1}-{2}-{3}-{4}|{5}", ClientID, AuthorizeToken, (int)Type, RequestTime, TokenHash, ApiKey);
        }

        public bool IsValid
        {
            get
            {
                if (IsPublic)
                {
                    return ClientID.Valid() && Type != TokenType.None;
                }
                return ClientID.Valid() && RequestTime.Valid() && TokenHash.Valid() && Type != TokenType.None;
            }
        }

        public bool IsPublic
        {
            get
            {
                return ApiKey.Valid() && !TokenHash.Valid();
            }
        }

        public string Key
        {
            get
            {
                switch (Type)
                {
                    case TokenType.AuthorizeToken:
                        return TokenConstants.AuthorizeTokenKey;
                    case TokenType.AccessToken:
                        return TokenConstants.AccessTokenKey;
                    case TokenType.RefreshToken:
                        return TokenConstants.RefreshTokenKey;
                }
                return string.Empty;
            }
        }
    }

    public enum TokenType
    {
        None = 0,
        AuthorizeToken = 1,
        AccessToken = 2,
        RefreshToken = 3
    }

    public class TokenException : Exception
    {
        public TokenException(string code, Exception ex) : base(code, ex)
        {
            Code = code;
        }

        public string Code { get; set; }
    }
}
