using devoctomy.funk.core.Membership;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace devoctomy.funk.client
{
    public static class FunkClientManagementExtensions
    {

        #region methods

        public static async Task<FunkManagementGetProfileResponse> GetProfile(this FunkClient iFuncClient)
        {
            HttpResponseMessage pHRMResponse = await iFuncClient.AuthenticatedRequestAsync("GET",
                iFuncClient.GetFunctionURI("Profile"));
            if (pHRMResponse.IsSuccessStatusCode)
            {
                String pStrResponse = await pHRMResponse.Content.ReadAsStringAsync();
                Profile pProProfile = Profile.FromJSON(pStrResponse);
                return (new FunkManagementGetProfileResponse(true, pProProfile));
            }
            else
            {
                return (new FunkManagementGetProfileResponse(false, null));
            }
        }

        public static async Task<Boolean> SetProfile(this FunkClient iFuncClient, Profile iProfile)
        {
            HttpResponseMessage pHRMResponse = await iFuncClient.AuthenticatedRequestAsync("PUT",
                iFuncClient.GetFunctionURI("Profile"),
                iProfile.ToJSON(Newtonsoft.Json.Formatting.None));
            if (pHRMResponse.IsSuccessStatusCode)
            {
                String pStrResponse = await pHRMResponse.Content.ReadAsStringAsync();
                Profile pProProfile = Profile.FromJSON(pStrResponse);
                return (true);
            }
            else
            {
                return (false);
            }
        }

        public static async Task<FunkManagementServiceInfoResponse> ServiceInfo(this FunkClient iFuncClient)
        {
             HttpResponseMessage pHRMResponse = await iFuncClient.AuthenticatedRequestAsync("GET",
                 iFuncClient.GetFunctionURI("ServiceInfo"));
            if(pHRMResponse.IsSuccessStatusCode)
            {
                String pStrResponse = await pHRMResponse.Content.ReadAsStringAsync();
                FunkServiceInfo pFSIResponse = FunkServiceInfo.FromJSON(pStrResponse);
                return (new FunkManagementServiceInfoResponse(true, pFSIResponse));
            }
            else
            {
                return (new FunkManagementServiceInfoResponse(false, null));
            }
        }

        #endregion

    }

}
