namespace Aware.Util.Constants
{
    public class CommonConstants
    {
        public const string AuthenticationSchema = "Worchart";
        public const string AccessToken = "AccessToken";
        public const string CustomerUserToken = "CustomerToken";
        public const int CustomerUserTokenTime = 30 * 60; //30 dk
        public const int MailQueueTimerInterval = 30 * 1000; //30 sn

        public const int ActivationCodeExpire = 5 * 60; //5 dk
        public const int JwtTokenExpire = 60; //dk
        public const int DailyCacheTime = 24 * 60 * 60; //1 day
        public const string DefaultIp = "0.0.0.1";

        public const string LanguageTR = "tr-TR";
        public const string LanguageEN = "en-US";
        public const string LanguageHeader = "lang";

        //Cache keys
        public const string ApplicationSessionKey = "Application:Session:{0}";
        public const string UserInfoKey = "User:Info:{0}";
        public const string LookupListKey = "Lookup:List:{0}";

        //Date formats
        public const string TimeStampFormat = "yyyyMMddHHmmssffff";
    }

    public class TokenConstants
    {
        public const string AuthorizeTokenKey = "Token:Authorize:{0}";
        public const string AccessTokenKey = "Token:Access:{0}";
        public const string RefreshTokenKey = "Token:Refresh:{0}";

        public const int AuthorizeTokenTime = 180; //sec
        public const int AccessTokenTime = 20 * 60; //sec
        public const int RefreshDuration = 180; //sec
    }

    public class MailTemplates
    {
        public const string ActivationMail = "ActivationMail";
        public const string ForgotPasswordMail = "ForgotPasswordMail";
        public const string NotifyPasswordChanged = "NotifyPasswordChanged";
    }
}
