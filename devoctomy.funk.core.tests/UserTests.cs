using System;
using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json.Linq;
using devoctomy.funk.core.Membership;
using System.Threading.Tasks;
using devoctomy.funk.core.Environment;
using devoctomy.funk.core.Cryptography;
using System.Security.Claims;
using System.Security.Principal;

namespace devoctomy.funk.core.tests
{
    [TestClass]
    public class UserTests
    {

        #region private objects

        private String cStrEmail = String.Empty;
        private String cStrUserName = String.Empty;
        private String cStrTableStorageRootURL = String.Empty;
        private String cStrConnectionString = String.Empty;
        private String cStrActivationCode = String.Empty;

        #endregion

        #region private methods

        private void LoadAzureCredentials()
        {
            String pStrCreds = File.ReadAllText("../../../../azure/creds.json");
            JObject pJOtCreds = JObject.Parse(pStrCreds);
            cStrTableStorageRootURL = pJOtCreds["TableStorageRootURL"].Value<String>();
            cStrConnectionString = pJOtCreds["ConnectionString"].Value<String>();
            EnvironmentHelpers.SetEnvironmentVariable("HOME", @"C:\Temp", EnvironmentVariableTarget.Process);
            EnvironmentHelpers.SetEnvironmentVariable("TableStorageRootURL", cStrTableStorageRootURL, EnvironmentVariableTarget.Process);
            EnvironmentHelpers.SetEnvironmentVariable("AzureWebJobsStorage", cStrConnectionString, EnvironmentVariableTarget.Process);
            cStrEmail = String.Format("{0}@{1}.com",
                CryptographyHelpers.RandomString(12),
                CryptographyHelpers.RandomString(12));
            cStrUserName = CryptographyHelpers.RandomString(12);
            cStrActivationCode = CryptographyHelpers.RandomString(6);
        }

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
            LoadAzureCredentials();
        }

        [TestMethod]
        public async Task AllUserManagementTestsSequenced()
        {
            await CreateUser(false);
            await CreateUser(true);
            await ActivateUserWithAvailableUserName();
            //DeleteUser?
        }

        //[TestMethod]
        public async Task CreateUser(Boolean iFail)
        {
            Storage pStoStorage = new Storage("TableStorageRootURL",
                "AzureWebJobsStorage",
                "Test");

            User pUsrUser = new User(GetTestUserPrincipal(), 6);
            pUsrUser.ActivationCode = cStrActivationCode;
            if(iFail)
            {
                Assert.IsFalse(await pUsrUser.InsertAsync(pStoStorage));
            }
            else
            {
                Assert.IsTrue(await pUsrUser.InsertAsync(pStoStorage));
            }
        }

        public async Task FindUser(String iUserName)
        {
            throw new NotImplementedException();
        }

        //[TestMethod]
        public async Task ActivateUserWithAvailableUserName()
        {
            Storage pStoStorage = new Storage("TableStorageRootURL",
                "AzureWebJobsStorage",
                "Test");
            User pUsrUser = await pStoStorage.GetUserAsync(GetTestUserPrincipal());
            if (pUsrUser != null)
            {
                Boolean pBlnUserNameTaken = false;
                Assert.IsTrue(pUsrUser.Activate(pStoStorage, 
                    cStrActivationCode, 
                    cStrUserName, 
                    out pBlnUserNameTaken));
            }
            else
            {
                Assert.Fail("Failed to get user '{0}' from storage.", cStrEmail);
            }
        }

        #endregion

    }
}
