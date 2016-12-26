using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;
using Newtonsoft.Json.Linq;
using devoctomy.funk.core.Environment;
using devoctomy.funk.core.Cryptography;
using devoctomy.funk.core.Membership;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Security.Claims;
using System.Security.Principal;

namespace devoctomy.funk.core.tests
{
    [TestClass]
    public class ProfileTests
    {

        #region private objects

        private String cStrTableStorageRootURL = String.Empty;
        private String cStrConnectionString = String.Empty;
        private String cStrDefaultProfile = String.Empty;
        private String cStrEmail = String.Empty;
        private String cStrUserName = String.Empty;
        private String cStrActivationCode = String.Empty;
        private Storage cStoStorage;
        private User cUsrUser;

        #endregion

        #region private methods

        private ClaimsPrincipal GetTestUserPrincipal()
        {
            GenericIdentity pGIyTestUser = new GenericIdentity("Test User");
            pGIyTestUser.AddClaim(new Claim(ClaimTypes.Email, cStrEmail));
            ClaimsPrincipal pCPlTestUser = new ClaimsPrincipal(pGIyTestUser);
            return (pCPlTestUser);
        }

        #endregion

        #region public methods

        [TestInitialize()]
        public void Init()
        {
            String pStrCreds = File.ReadAllText("../../../../azure/creds.json");
            JObject pJOtCreds = JObject.Parse(pStrCreds);
            cStrTableStorageRootURL = pJOtCreds["TableStorageRootURL"].Value<String>();
            cStrConnectionString = pJOtCreds["ConnectionString"].Value<String>();
            EnvironmentHelpers.SetEnvironmentVariable("HOME", @"C:\Temp", EnvironmentVariableTarget.Process);
            EnvironmentHelpers.SetEnvironmentVariable("TableStorageRootURL", cStrTableStorageRootURL, EnvironmentVariableTarget.Process);
            EnvironmentHelpers.SetEnvironmentVariable("AzureWebJobsStorage", cStrConnectionString, EnvironmentVariableTarget.Process);
            cStrDefaultProfile = File.ReadAllText(@"Assets\ProfileDefaults.json");
            cStrEmail = String.Format("{0}@{1}.com",
                CryptographyHelpers.RandomString(12),
                CryptographyHelpers.RandomString(12));
            cStrActivationCode = CryptographyHelpers.RandomString(6);

            //Create the user
             cStoStorage = new Storage("TableStorageRootURL",
                "AzureWebJobsStorage",
                "Test");
            cUsrUser = new User(GetTestUserPrincipal(), 6);
            cUsrUser.ActivationCode = cStrActivationCode;
            Assert.IsTrue(cUsrUser.Insert(cStoStorage));
        }

        [TestMethod()]
        public void ProfileFromJSONToJSONAndBackAgain()
        {
            Membership.Profile pProProfile = Membership.Profile.FromJSON(cStrDefaultProfile);
            String pStrProfile = pProProfile.ToJObject().ToString(Newtonsoft.Json.Formatting.Indented);
            pProProfile = Membership.Profile.FromJSON(pStrProfile);
        }

        [TestMethod()]
        public void GetUserProfile()
        {
            devoctomy.funk.core.Membership.Profile pProProfile = cUsrUser.GetProfile(cStoStorage);
        }

        [TestMethod()]
        public void UpdatetUserProfile()
        {
            String pStrNewFullName = CryptographyHelpers.RandomString(12);
            devoctomy.funk.core.Membership.Profile pProProfile = cUsrUser.GetProfile(cStoStorage);
            List<String> pLisKeys = pProProfile.GetAllKeys();
            Dictionary<String, String> pDicNewValues = new Dictionary<String, String>();
            foreach(String curKey in pLisKeys)
            {
                String pStrRandom = CryptographyHelpers.RandomString(8);
                pProProfile[curKey] = pStrRandom;
                pDicNewValues.Add(curKey, pStrRandom);
            }
            Assert.IsTrue(cUsrUser.ReplaceProfile(cStoStorage,
                pProProfile));
            pProProfile = cUsrUser.GetProfile(cStoStorage);
            foreach (String curKey in pLisKeys)
            {
                String pStrValue = pProProfile[curKey];
                String pStrExpecting = pDicNewValues[curKey];
                Assert.IsTrue(pStrValue.Equals(pStrExpecting));
            }
        }

        [TestMethod()]
        public void SetProfile()
        {
            Membership.Profile pProSource = Membership.Profile.FromJSON(cStrDefaultProfile);
            List<String> pLisAllKeys = pProSource.GetAllKeys();
            Dictionary<String, String> pDicNewValues = new Dictionary<String, String>();
            foreach(String curKey in pLisAllKeys)
            {
                pDicNewValues.Add(curKey, Guid.NewGuid().ToString());
                pProSource[curKey] = pDicNewValues[curKey];
            }
 
            Membership.Profile pProTarget = Membership.Profile.FromJSON(cStrDefaultProfile);
            pProTarget.SetFrom(pProSource);
            foreach (String curKey in pLisAllKeys)
            {
                Assert.IsTrue(pProTarget[curKey] == pDicNewValues[curKey]);
            }
        }

        #endregion


    }
}
