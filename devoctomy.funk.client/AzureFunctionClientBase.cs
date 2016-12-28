using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Windows.Security.Authentication.Web;

namespace devoctomy.funk.client
{

    public class AzureFunctionClientBase
    {

        #region private objects

        private HttpClient cHCtClient;

        private String cStrClientID = String.Empty;
        private String cStrAzureFunctionRootURL = String.Empty;
        private String cStrAuthToken = String.Empty;

        #endregion

        #region public properties

        public String ClientID
        {
            get { return (cStrClientID); }
        }

        public Uri AzureFunctionFacebookAuthURI
        {
            get
            {
                String pStrFacebookAuthURI = cStrAzureFunctionRootURL;
                if (!pStrFacebookAuthURI.EndsWith("/")) pStrFacebookAuthURI += "/";
                return (new Uri(String.Format("{0}{1}", pStrFacebookAuthURI, ".auth/login/facebook")));
            }
        }

        public static Uri ApplicationCallbackURI
        {
            get { return (WebAuthenticationBroker.GetCurrentApplicationCallbackUri()); }
        }

        public Uri FacebookAuthURI
        {
            get
            {
                String pStrFacebookAuthURI = String.Format("https://www.facebook.com/dialog/oauth" + 
                    "?client_id={0}" +
                    "&redirect_uri={1}" + 
                    "&scope={2}" +
                    "&display={3}" +
                    "&response_type={4}", ClientID, Uri.EscapeDataString(ApplicationCallbackURI.ToString()), "email", "popup", "token");
                return (new Uri(pStrFacebookAuthURI));
            }
        }

        #endregion

        #region constructor / destructor

        public AzureFunctionClientBase(String iClientID,
            String iAzureFunctionRootURL)
        {
            cStrClientID = iClientID;
            cStrAzureFunctionRootURL = iAzureFunctionRootURL;

            cHCtClient = new HttpClient();
        }

        #endregion

        #region public methods

        public String GetFunctionURI(String iFunctionName)
        {
            String pStrFacebookAuthURI = cStrAzureFunctionRootURL;
            if (!pStrFacebookAuthURI.EndsWith("/")) pStrFacebookAuthURI += "/";
            pStrFacebookAuthURI += String.Format("api/{0}", iFunctionName);
            return (pStrFacebookAuthURI);
        }

        public async Task<Boolean> AuthenticateAsync()
        {
            WebAuthenticationResult pWARAuthResult = await WebAuthenticationBroker.AuthenticateAsync(WebAuthenticationOptions.None, 
                FacebookAuthURI, 
                ApplicationCallbackURI);
            switch(pWARAuthResult.ResponseStatus)
            {
                case WebAuthenticationStatus.Success:
                    {
                        Dictionary<String, String> pDicArgs = new Dictionary<String, String>();
                        String pStrArgs = pWARAuthResult.ResponseData.Substring(pWARAuthResult.ResponseData.IndexOf('#') + 1);
                        String[] pStrKeyPairs = pStrArgs.Split('&');
                        foreach (String curPair in pStrKeyPairs)
                        {
                            String[] pStrPair = curPair.Split('=');
                            pDicArgs.Add(pStrPair[0], pStrPair[1]);
                        }

                        JObject pJOtResponseData = new JObject();
                        pJOtResponseData.Add("access_token", new JValue(pDicArgs["access_token"]));

                        HttpClient pHCtClient = new HttpClient();
                        HttpResponseMessage pHRMFunctionAuthResp = await pHCtClient.PostAsync(AzureFunctionFacebookAuthURI, new StringContent(pJOtResponseData.ToString()));
                        if (pHRMFunctionAuthResp.IsSuccessStatusCode)
                        {
                            String pStrAuthResp = await pHRMFunctionAuthResp.Content.ReadAsStringAsync();
                            JObject pJOtResponse = JObject.Parse(pStrAuthResp);
                            cStrAuthToken = pJOtResponse["authenticationToken"].Value<String>();
                            return (true);
                        }
                        else
                        {
                            return (false);
                        }
                    }
                case WebAuthenticationStatus.ErrorHttp:
                    {
                        return (false);
                    }
                default:
                    {
                        return (false);
                    }
            }
        }

        public async Task<HttpResponseMessage> AuthenticatedRequestAsync(String iMethod,
            String iRequestURI,
            String iContent = "")
        {
            cHCtClient.DefaultRequestHeaders.Remove("x-zumo-auth");
            cHCtClient.DefaultRequestHeaders.Add("x-zumo-auth", cStrAuthToken);
            switch (iMethod.ToUpper())
            {
                case "GET":
                    {
                        HttpResponseMessage pHRMResponse = await cHCtClient.GetAsync(iRequestURI);
                        return (pHRMResponse);
                    }
                case "PUT":
                    {
                        HttpResponseMessage pHRMResponse = await cHCtClient.PutAsync(iRequestURI, new StringContent(iContent));
                        return (pHRMResponse);
                    }
                default:
                    {
                        throw new NotImplementedException(String.Format("The method '{0}' is not supported.", iMethod.ToUpper()));
                    }
            }
        }

        #endregion

    }

}
