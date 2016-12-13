﻿using System;
using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json.Linq;
using devoctomy.funk.core.Membership;
using System.Threading.Tasks;
using devoctomy.funk.core.Environment;
using devoctomy.funk.core.Cryptography;

namespace devoctomy.funk.core.tests
{
    [TestClass]
    public class UserManagement
    {

        #region private objects

        private String cStrEmail = String.Empty;
        private String cStrUserName = String.Empty;
        private String cStrTableStorageRootURL = String.Empty;
        private String cStrConnectionString = String.Empty;
        private String cStrActivationCode = String.Empty;
        private String cStrOTP = String.Empty;

        #endregion

        #region private methods

        private void LoadAzureCredentials()
        {
            String pStrCreds = File.ReadAllText("../../../../azure/creds.json");
            JObject pJOtCreds = JObject.Parse(pStrCreds);
            cStrTableStorageRootURL = pJOtCreds["TableStorageRootURL"].Value<String>();
            cStrConnectionString = pJOtCreds["ConnectionString"].Value<String>();
            EnvironmentHelpers.SetEnvironmentVariable("AzureWebJobsStorage", cStrConnectionString, EnvironmentVariableTarget.Process);
            cStrEmail = String.Format("{0}@{1}.com",
                CryptographyHelpers.RandomString(12),
                CryptographyHelpers.RandomString(12));
            cStrUserName = CryptographyHelpers.RandomString(12);
            cStrActivationCode = CryptographyHelpers.RandomString(6);
            cStrOTP = CryptographyHelpers.RandomString(6);
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
            await ActivateUser();
            await RandomiseOTP();
            await VerifyOTP();
        }

        //[TestMethod]
        public async Task CreateUser(Boolean iFail)
        {
            Storage pStoStorage = new Storage(cStrTableStorageRootURL,
                "AzureWebJobsStorage",
                "Test");
            User pUsrUser = new User(cStrEmail, cStrUserName, 6);
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

        //[TestMethod]
        public async Task ActivateUser()
        {
            Storage pStoStorage = new Storage(cStrTableStorageRootURL,
                "AzureWebJobsStorage",
                "Test");
            User pUsrUser = await pStoStorage.GetUserAsync(cStrEmail);
            if(pUsrUser != null)
            {
                Assert.IsTrue(await pUsrUser.Activate(pStoStorage, cStrActivationCode));
            }
            else
            {
                Assert.Fail("Failed to get user '{0}' from storage.", cStrEmail);
            }
        }

        //[TestMethod]
        public async Task RandomiseOTP()
        {
            Storage pStoStorage = new Storage(cStrTableStorageRootURL,
                "AzureWebJobsStorage",
                "Test");
            User pUsrUser = await pStoStorage.GetUserAsync(cStrEmail);
            if (pUsrUser != null)
            {
                pUsrUser.OTP = cStrOTP;
                pUsrUser.OTPRequestedAt = DateTime.UtcNow;
                Assert.IsTrue(await pStoStorage.Replace(pUsrUser));
            }
            else
            {
                Assert.Fail("Failed to get user '{0}' from storage.", cStrEmail);
            }
        }

        //[TestMethod]
        public async Task VerifyOTP()
        {
            Storage pStoStorage = new Storage(cStrTableStorageRootURL,
                "AzureWebJobsStorage",
                "Test");
            User pUsrUser = await pStoStorage.GetUserAsync(cStrEmail);
            if (pUsrUser != null)
            {
                UserLoginResult pULRLogin = await pUsrUser.Login(pStoStorage, cStrOTP, new TimeSpan(0, 0, 30));
                Assert.IsTrue(pULRLogin.Success);
            }
            else
            {
                Assert.Fail("Failed to get user '{0}' from storage.", cStrEmail);
            }
        }

        #endregion

    }
}
