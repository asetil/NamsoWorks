namespace Aware.Util.Constants
{
    public class ResultCodes
    {
        public class Success
        {
            public const string OperationSuccess = "000";
        }

        public class Error
        {
            public class Login
            {
                public const string InvalidUsernamePassword = "ERR.LOGIN.INVALIDUSERNAMEORPASSWORD";
                public const string InvalidCompanyCode = "ERR.LOGIN.INVALIDCOMPANYCODE";
                public const string UserNotFound = "ERR.LOGIN.USERNOTFOUND";
                public const string CompanyNotFound = "ERR.LOGIN.COMPANYNOTFOUND";
                public const string CompanyLoginNotAllowed = "ERR.LOGIN.COMPANYLOGINNOTALLOWED";
                public const string PasswordDismatch = "ERR.LOGIN.PASSWORDDISMATCH";
                public const string LogoffFailed = "ERR.LOGIN.LOGOFFFAILED";
                public const string InvalidAuthorizeToken = "ERR.LOGIN.INVALIDAUTHORIZETOKEN";
                public const string EmailInUse = "ERR.LOGIN.EMAILUSEDBYANOTHERUSER";
                public const string PhoneNumberInUse = "ERR.LOGIN.PHONENUMBERUSEDBYANOTHERUSER";
                public const string AuthorizationFailed = "ERR.LOGIN.AUTHORIZATIONFAILED";
                public const string ActivationLinkExpired = "ERR.LOGIN.ACTIVATIONLINKEXPIRED";
                public const string ActivationMailSendFailure = "ERR.LOGIN.ACTIVATIONMAILSENDFAILED";
                public const string InvalidActivationCode = "ERR.LOGIN.INVALIDACTIVATIONCODE";
                public const string InvalidCurrentPassword = "ERR.LOGIN.INVALIDCURRENTPASSWORD";
                public const string CurrentPasswordAndNewPasswordAreSame = "ERR.LOGIN.NEWPASSWORDEQUALTOOLDPASSWORD";
                public const string AlreadyActivated = "ERR.LOGIN.ALREADYACTIVATED";
            }

            public class Session
            {
                public const string ApplicationNotFound = "ERR.TOKEN.NOAPPLICATION";
                public const string InvalidAuthorizeToken = "ERR.TOKEN.INVALIDAUTHORIZETOKEN";
                public const string InvalidRefreshToken = "ERR.TOKEN.INVALIDREFRESHTOKEN";
                public const string InvalidHash = "ERR.TOKEN.INVALIDHASH";
                public const string NoAccessToken = "ERR.TOKEN.NOACCESSTOKENFOUND";
                public const string InvalidAccessToken = "ERR.TOKEN.INVALIDACCESSTOKEN";
                public const string UnAuthenticatedRequest = "ERR.TOKEN.UNAUTHENTICATEDREQUEST";
                public const string IpAddressNotAllowed = "ERR.TOKEN.IPADDRESSNOTALLOWED";
                public const string InvalidApiKey = "ERR.TOKEN.INVALIDAPIKEY";
                public const string InvalidKey = "ERR.SESSION.INVALIDKEY";

            }

            public class File
            {
                public const string InvalidExtension = "ERR.FILE.INVALID_EXTENSION";
                public const string InvalidFile = "ERR.FILE.INVALID_FILE";
            }


            public const string OperationFailed = "ERR.COMMON.OPERATIONFAILED";
            public const string CheckParameters = "ERR.COMMON.CHECKINPUTPARAMETERS";
            public const string InvalidEmail = "ERR.COMMON.EMAILISNOTVALID";
        }



    }
}
