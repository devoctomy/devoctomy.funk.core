using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace devoctomy.funk.core.Azure.Functions
{

    public static class FunctionsHelpers
    {

        #region public methods

        public static async Task<String> GetAuthProviderParam(String iAuthMeURL,
            String iXZumoAUth,
            String iParamKey)
        {
            using (HttpClient pHCtClient = new HttpClient())
            {
                pHCtClient.DefaultRequestHeaders.Add("x-zumo-auth", iXZumoAUth);
                String pStrResponse = await pHCtClient.GetStringAsync(iAuthMeURL);
                JObject pJOtResponse = JObject.Parse(pStrResponse.Trim(new Char[] { '[', ']' }));
                if(pJOtResponse[iParamKey] != null)
                {
                    return (pJOtResponse[iParamKey].Value<String>());
                }
                else
                {
                    throw new KeyNotFoundException(String.Format("A parameter with the key '{0}' was not found.", iParamKey));
                }
            }
        }

        public static async Task<String> GetAuthProviderClaim(String iAuthMeURL,
            String iXZumoAUth,
            String iClaimType)
        {
            using (HttpClient pHCtClient = new HttpClient())
            {
                pHCtClient.DefaultRequestHeaders.Add("x-zumo-auth", iXZumoAUth);
                String pStrResponse = await pHCtClient.GetStringAsync(iAuthMeURL);
                JObject pJOtResponse = JObject.Parse(pStrResponse.Trim(new Char[] { '[', ']' }));
                JArray pJAyClaims = pJOtResponse["user_claims"].Value<JArray>();
                foreach(JObject curClaim in pJAyClaims)
                {
                    if(curClaim["typ"].Value<String>() == iClaimType)
                    {
                        return (curClaim["val"].Value<String>());
                    }
                }
                throw new KeyNotFoundException(String.Format("A claim with the type '{0}' was not found.", iClaimType));
            }
        }

        #endregion

    }

}
