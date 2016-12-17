using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Net;

namespace devoctomy.funk.core.Azure.Authentication
{

    public class FacebookProvider
    {

        #region public enums

        public enum Claim
        {
            none = 0,
            nameidentifier = 1,
            emailaddress = 2,
            name = 3,
            facebookprofileurl = 4,
            givenname = 5,
            surname = 6,
            gender = 7,
            locale = 8,
            timezone = 9

        }

        #endregion

        #region private objects

        private String cStrAccessToken = String.Empty;
        private DateTime cDTeExpiresOn = new DateTime(1982, 4, 3);
        private Dictionary<String, String> cDicClaims;
        private String cStrUserID = String.Empty;

        #endregion

        #region public properties

        public String AccessToken
        {
            get { return (cStrAccessToken); }
        }

        public DateTime ExpiresOn
        {
            get { return (cDTeExpiresOn); }
        }

        public IReadOnlyDictionary<String, String> Claims
        {
            get { return (cDicClaims); }
        }

        public String UserID
        {
            get { return (cStrUserID); }
        }

        #endregion

        #region public methods

        public String ClaimAsString(Claim iClaim)
        {
            switch(iClaim)
            {
                case Claim.nameidentifier:
                    {
                        return (@"http:\\schemas.xmlsoap.org\ws\2005\05\identity\claims\nameidentifier");
                    }
                case Claim.emailaddress:
                    {
                        return (@"http:\\schemas.xmlsoap.org\ws\2005\05\identity\claims\emailaddress");
                    }
                case Claim.name:
                    {
                        return (@"http:\\schemas.xmlsoap.org\ws\2005\05\identity\claims\name");
                    }
                case Claim.facebookprofileurl:
                    {
                        return (@"urn:facebook:link");
                    }
                case Claim.givenname:
                    {
                        return (@"http:\\schemas.xmlsoap.org\ws\2005\05\identity\claims\givenname");
                    }
                case Claim.surname:
                    {
                        return (@"http:\\schemas.xmlsoap.org\ws\2005\05\identity\claims\surname");
                    }
                case Claim.gender:
                    {
                        return (@"http:\\schemas.xmlsoap.org\ws\2005\05\identity\claims\gender");
                    }
                case Claim.locale:
                    {
                        return (@"urn:facebook:locale");
                    }
                case Claim.timezone:
                    {
                        return (@"urn:facebook:timezone");
                    }
                default:
                    {
                        throw new NotImplementedException(String.Format("Unsupported scope '{0}'.", iClaim.ToString()));
                    }
            }
        }

        public static FacebookProvider Get(String iFunctionAppRootURL = "")
        {
            if (iFunctionAppRootURL.EndsWith("/")) iFunctionAppRootURL.TrimEnd('/');
            String pStrEndpoint = String.Format("{0}/{1}", iFunctionAppRootURL, ".auth/me");

            using (WebClient pWCtClient = new WebClient())
            {
                String pStrMe = pWCtClient.DownloadString(pStrEndpoint);
                JObject pJOtMe = JObject.Parse(pStrMe);
                FacebookProvider pFPrProvider = new FacebookProvider();
                pFPrProvider.cStrAccessToken = pJOtMe["access_token"].Value<String>();
                pFPrProvider.cDTeExpiresOn = DateTime.ParseExact(pJOtMe["expires_on"].Value<String>(), "O", System.Globalization.CultureInfo.InvariantCulture);
                pFPrProvider.cDicClaims = new Dictionary<String, String>();
                JArray pJAyClaims = pJOtMe["user_claims"].Value<JArray>();
                foreach(JObject curClaim in pJAyClaims)
                {
                    pFPrProvider.cDicClaims.Add(pJOtMe["type"].Value<String>(),
                        pJOtMe["val"].Value<String>());
                }
                pFPrProvider.cStrUserID = pJOtMe["user_id"].Value<String>();
                return (pFPrProvider);
            }
        }

        #endregion

    }

}
