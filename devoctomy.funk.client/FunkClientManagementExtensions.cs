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

        public static async Task<FunkManagementGetRegisterResponse> GetRegisterAsync(this FunkClient iFunkClient)
        {
            HttpResponseMessage pHRMResponse = await iFunkClient.AuthenticatedRequestAsync("GET",
                iFunkClient.GetFunctionURI("Register"));
            if (pHRMResponse.IsSuccessStatusCode)
            {
                String pStrResponse = await pHRMResponse.Content.ReadAsStringAsync();
                
                return (new FunkManagementGetRegisterResponse(true, null));
            }
            else
            {
                return (new FunkManagementGetRegisterResponse(false, null));
            }
        }

        public static async Task<FunkManagementGetActivateResponse> GetActivateAsync(this FunkClient iFunkClient,
            String iActivationCode,
            String iUserName)
        {
            HttpResponseMessage pHRMResponse = await iFunkClient.AuthenticatedRequestAsync("GET",
                iFunkClient.GetFunctionURI("ActivateAccount", new KeyValuePair<String, String>[] {
                    new KeyValuePair<String, String>("activationcode", iActivationCode),
                    new KeyValuePair<String, String>("username", iUserName)
                }));
            if (pHRMResponse.IsSuccessStatusCode)
            {
                String pStrResponse = await pHRMResponse.Content.ReadAsStringAsync();
                return (new FunkManagementGetActivateResponse(true, null));
            }
            else
            {
                String pStrResponse = await pHRMResponse.Content.ReadAsStringAsync();
                return (new FunkManagementGetActivateResponse(false, null));
            }
        }

        public static async Task<FunkManagementGetProfileResponse> GetProfileAsync(this FunkClient iFunkClient)
        {
            HttpResponseMessage pHRMResponse = await iFunkClient.AuthenticatedRequestAsync("GET",
                iFunkClient.GetFunctionURI("Profile"));
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

        public static async Task<Boolean> SetProfileAsync(this FunkClient iFunkClient, Profile iProfile)
        {
            HttpResponseMessage pHRMResponse = await iFunkClient.AuthenticatedRequestAsync("PUT",
                iFunkClient.GetFunctionURI("Profile"),
                iProfile.ToJObject().ToString(Newtonsoft.Json.Formatting.None));
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

        public static async Task<FunkManagementGetUserInfoResponse> GetUserInfoAsync(this FunkClient iFunkClient)
        {
            HttpResponseMessage pHRMResponse = await iFunkClient.AuthenticatedRequestAsync("GET",
                iFunkClient.GetFunctionURI("UserInfo"));
            if (pHRMResponse.IsSuccessStatusCode)
            {
                String pStrResponse = await pHRMResponse.Content.ReadAsStringAsync();
                FunkUserInfo pFIOResponse = FunkUserInfo.FromJSON(pStrResponse);
                return (new FunkManagementGetUserInfoResponse(true, pFIOResponse));
            }
            else
            {
                return (new FunkManagementGetUserInfoResponse(false, null));
            }
        }

        public static async Task<FunkManagementServiceInfoResponse> GetServiceInfoAsync(this FunkClient iFunkClient)
        {
             HttpResponseMessage pHRMResponse = await iFunkClient.AuthenticatedRequestAsync("GET",
                 iFunkClient.GetFunctionURI("ServiceInfo"));
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
