using System.Collections.Generic;
using Aware.Authenticate.Model;
using Aware.Util.Model;

namespace Aware.Authenticate
{
    public interface ISocialAuthManager
    {
        string GetFacebookLoginUrl(string redirectUrl,List<string> scopeList=null);
        string GetGoogleLoginUrl(string redirectUrl, List<string> scopeList = null);

        Result ProcessFacebookLogin(string accessCode, string redirectUrl, List<string> scopeList = null);
        Result ProcessGoogleLogin(string accessCode, string redirectUrl);
        Result HandleSocialClient(FacebookProfileModel model);
        
    }
}