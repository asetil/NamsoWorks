using System.Linq;
using System.Web.Mvc;
using System.Web;
using System.Text;
using System.Collections.Generic;
using Aware.Dependency;
using Aware.Cache;
using Aware.Util;

namespace CleanCode.Controllers
{
    public class OpenApiController : Controller
    {
        public ActionResult Index()
        {
            var cacher = WindsorBootstrapper.Resolve<ICacher>();
            ViewBag.CachedData = cacher.Get<string>("Oskima");

            var citibankManager = new CitiBankManager();
            ViewBag.CitiBankAuthUrl = citibankManager.GetAuthorizeUrl();

            return View();
        }

        public ActionResult DemoCiti(string code, string state = "", string error = "")
        {
            var result = new StringBuilder();
            result.AppendFormat("<h1>CITIBANK REQUEST INFO</h1><table class='table table-bordered table-striped'>");
            result.AppendFormat("<tr><td>DemoCiti Request Params</td><td>Code : {0},<br/> State:{1},<br/> Error:{2}</td></tr>", code, state, error);

            var requestStr = GetParams(Request, "Citibank");
            result.Append(requestStr);

            var citibankManager = new CitiBankManager();
            var citiResponse = citibankManager.GetAccessToken(code, state, error);
            result.Append(citiResponse);
            result.AppendFormat("</table>");

            ViewBag.AllIsWell = result.ToString();
            return View();
        }

        public ActionResult osman()
        {
            var url = "https://apis.garanti.com.tr/auth/oauth/v2/token?grant_type=client_credentials&scope=oob";
            var headers = new System.Collections.Specialized.NameValueCollection();
            headers.Add("client_id", "l7xxd8901aed3f804336a6057b95c6cea1b6");
            headers.Add("client_secret", "78f61701217d45ff9958ea369d17d5d7");
            headers.Add("redirect_uri", "https://www.osmansokuoglu.com/openapi/postdemog");

            var osm = "client_id=l7xxd8901aed3f804336a6057b95c6cea1b6&client_secret=78f61701217d45ff9958ea369d17d5d7&redirect_uri=https://www.osmansokuoglu.com/openapi/postdemog";
            //foreach (var key in headers.AllKeys)
            //{
            //    url += "&" + key + "=" + headers[key];
            //}

            var result = WebRequester.makePostRequest(url, osm);
            //var aa = result.DeSerialize<xxy>();


            //var headers = new System.Collections.Specialized.NameValueCollection();
            //headers.Add("Authorization", string.Format("{0} {1}", aa.token_type, aa.access_token));

            //var stockInfoUrl = "https://api.yapikredi.com.tr/api/stockmarket/v1/stockInformation?type=MOST_TRADED";
            //var branchUrl = "https://api.yapikredi.com.tr/api/common/branch/v1/branchList";

            //var branchList = WebRequester.DoRequest(stockInfoUrl, string.Empty, false, new System.Collections.Specialized.NameValueCollection(), headers);


            return Content(result + " => ");
        }


        public ActionResult yener()
        {
            var url = "https://api.yapikredi.com.tr/auth/oauth/v2/token?grant_type=client_credentials&scope=oob&client_id=l7xx6928cb8e44404d948dc7f81fca1cfbcc&client_secret=78eb47f244bb4ccf85b08b02fcf473b7";

            var result = WebRequester.DoRequest(url, string.Empty, true, new System.Collections.Specialized.NameValueCollection());
            var aa = result.DeSerialize<TokenModel>();


            var headers = new System.Collections.Specialized.NameValueCollection();
            headers.Add("Authorization", string.Format("{0} {1}", aa.token_type, aa.access_token));

            var stockInfoUrl = "https://api.yapikredi.com.tr/api/stockmarket/v1/stockInformation?type=MOST_TRADED";
            var branchUrl = "https://api.yapikredi.com.tr/api/common/branch/v1/branchList";

            var branchList = WebRequester.DoRequest(stockInfoUrl, string.Empty, false, new System.Collections.Specialized.NameValueCollection(), headers);


            return Content(result + " => " + branchList);
        }

        public ActionResult DemoG(string code, string error)
        {
            var requestStr = GetParams(Request, "Garanti");
            return Content(requestStr + "<br>code:" + code + "<br>error:" + error);
        }

        [HttpPost]
        public ActionResult PostDemoG()
        {
            var requestStr = GetParams(Request, "GarantiP");
            return Content(requestStr);
        }

        public ActionResult DemoYk()
        {
            var requestStr = GetParams(Request, "YapiKredi");
            return Content(requestStr);
        }

        [HttpPost]
        public ActionResult PostDemoYk()
        {
            var requestStr = GetParams(Request, "YapiKrediP");
            return Content(requestStr);
        }

        private string GetParams(HttpRequestBase request, string title)
        {
            var result = new StringBuilder();
            result.AppendFormat("<tr><td>Title : {0}</td><td>Url : {1}</td></tr>", title, request.Url.AbsoluteUri);

            if (request.UrlReferrer != null)
            {
                result.AppendFormat("<tr><td colspan='2'>url_referer : {0}</td></tr>", request.UrlReferrer.AbsoluteUri);
            }

            foreach (var key in request.QueryString.AllKeys)
            {
                result.AppendFormat("<tr><td>QS_Key : {0}</td><td>Value : {1}</td></tr>", key, request.QueryString[key]);
            }

            foreach (var key in Request.Headers.AllKeys)
            {
                result.AppendFormat("<tr><td>HD_Key : {0}</td><td>Value : {1}</td></tr>", key, request.Headers[key]);
            }

            var formValues = Request.Form.Keys.Cast<string>().ToDictionary(key => key, key => Request.Form.Get(key));
            foreach (KeyValuePair<string, string> item in formValues)
            {
                result.AppendFormat("<tr><td>Form_Key : {0}</td><td>Value : {1}</td></tr>", item.Key, item.Value);
            }

            var cacher = WindsorBootstrapper.Resolve<ICacher>();
            var val = cacher.Get<string>("Oskima");
            val = val ?? "";
            val += "<tr><td colspan='2'>YENİ İSTEK</td></tr>" + result;
            cacher.Add("Oskima", val);

            return result.ToString();
        }
    }


    public class CitiBankManager
    {
        private string clientID = "e4d4ac5d-04dc-4c6e-8e13-2f84f0877b88";
        private string clientSecret = "uQ8sS3eO5bK6pD4pP4tT6eF6jU2jN2yJ2oP3hQ8uY4sW6eK1rR";

        private string scopes = "customers_profiles"; //cards, accounts_details_transactions  
        private string localeInfo = "businessCode=GCB&locale=en_US";
        //private string localeInfo = "countryCode=AU&businessCode=GCB&locale=en_US&state=namso";

        private string redirectUrl = "https://www.osmansokuoglu.com/openapi/demociti";
        private string authorizeUrl = "https://sandbox.apihub.citi.com/gcb/api/authCode/oauth2/authorize";
        private string tokenUrl = "https://sandbox.apihub.citi.com/gcb/api/authCode/oauth2/token/us/gcb";

        public string GetAuthorizeUrl()
        {
            var result = string.Format("{0}?response_type=code&client_id={1}&{2}&redirect_uri={3}&state=namso_",
                authorizeUrl, clientID, localeInfo, redirectUrl);
            return result;
        }

        public string GetAccessToken(string code, string state, string error)
        {
            var result = new StringBuilder();

            try
            {
                if (!string.IsNullOrEmpty(code) && string.IsNullOrEmpty(error))
                {
                    var parameters = new System.Collections.Specialized.NameValueCollection();
                    var headers = new System.Collections.Specialized.NameValueCollection();
                    //headers.Add("accept", "application/json");
                    headers.Add("Authorization", "Basic " + Base64Encode(clientID + ":" + clientSecret));

                    result.AppendFormat("<tr><td colspan='2'>Osman Breakpoint</td></tr>");

                    var data = string.Format("grant_type=authorization_code&code={0}&redirect_uri={1}", code, redirectUrl);
                    var response = WebRequester.makePostRequest(tokenUrl, data, headers);
                    result.AppendFormat("<tr><td colspan='2'>GetAccessToken Response</td></tr>", response);
                    result.AppendFormat("<tr><td>Response</td><td>{0}</td></tr>", response);

                    var tokenModel = response.DeSerialize<TokenModel>();
                    var operation = CallApi(tokenModel, state);
                    result.AppendFormat("<tr><td colspan='2'>CallApi Response</td></tr>");
                    result.Append(operation);
                }
                else
                {
                    result.AppendFormat("<tr><td>GetAccessToken Response</td><td>No result calculated, error:{0}</td></tr>", error);
                }
            }
            catch (System.Exception ex)
            {
                result.AppendFormat("<tr><td>GetAccessToken Response</td><td>Exception Occurred :{0}-{1}</td></tr>", ex.Message,ex);
            }
            return result.ToString();
        }

        public string CallApi(TokenModel tokenModel, string state)
        {
            var result = new StringBuilder();
            try
            {
                if (tokenModel != null && !string.IsNullOrEmpty(state))
                {
                    var headers = new System.Collections.Specialized.NameValueCollection();
                    headers.Add("accept", "application/json");
                    headers.Add("content-type", "application/json");
                    
                    headers.Add("Authorization", string.Format("Bearer {0}", tokenModel.access_token));
                    headers.Add("client_id", clientID);
                    headers.Add("uuid", System.Guid.NewGuid().ToString());

                    var url = string.Empty;
                    switch (state)
                    {
                        case "namso_cusprofile":
                            url = "https://sandbox.apihub.citi.com/gcb/api/v1/customers/profiles"; break;
                        case "namso_cusaccount":
                            url = "https://sandbox.apihub.citi.com/gcb/api/v1/accounts?nextStartIndex=50"; break;
                        case "namso_cusaccountdetail":
                            url = "https://sandbox.apihub.citi.com/gcb/api/v1/accounts/AB34512DEF984DBBACEA"; break;
                        case "namso_cuscards":
                            url = "https://sandbox.apihub.citi.com/gcb/api/v1/cards?cardFunction=ALL"; break;
                    }

                    var apiResponse = WebRequester.DoRequest(url, string.Empty, false, new System.Collections.Specialized.NameValueCollection(), headers);
                    result.AppendFormat("<tr><td>apiResponse</td><td>{0}</td></tr>", apiResponse);
                }
                else
                {
                    result.AppendFormat("<tr><td>CallApi Error</td><td>Check params for {0}</td></tr>", state);
                }
            }
            catch (System.Exception ex)
            {
                result.AppendFormat("<tr><td>Exception occurred</td><td>{0}</td></tr>", ex);
            }
            return result.ToString();
        }

        public string Base64Encode(string plainText)
        {
            var plainTextBytes = System.Text.Encoding.UTF8.GetBytes(plainText);
            return System.Convert.ToBase64String(plainTextBytes);
        }

        public string Base64Decode(string base64EncodedData)
        {
            var base64EncodedBytes = System.Convert.FromBase64String(base64EncodedData);
            return System.Text.Encoding.UTF8.GetString(base64EncodedBytes);
        }
    }

    public class TokenModel
    {
        public string access_token { get; set; }
        public string refresh_token { get; set; }
        public string token_type { get; set; }
        public string expires_in { get; set; }
        public string scope { get; set; }
    }
}