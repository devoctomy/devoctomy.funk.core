using devoctomy.funk.core.Cryptography;
using devoctomy.funk.core.Environment;
using devoctomy.funk.core.Membership;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;

namespace devoctomy.funk.core.tests
{

    [TestClass]
    public class FunctionHitTests
    {

        #region private objects

        private String cStrTableStorageRootURL = String.Empty;
        private String cStrConnectionString = String.Empty;
        private Storage cStoStorage;
        private String cStrSource = String.Empty;

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
            cStrSource = CryptographyHelpers.RandomString(8);

            //Init storage
            cStoStorage = new Storage("TableStorageRootURL",
               "AzureWebJobsStorage",
               "Test");
        }

        #endregion

        #region public methods

        [TestInitialize()]
        public void Init()
        {
            LoadAzureCredentials();
        }

        [TestMethod()]
        public void RegisterAndGet10Hits()
        {
            for(Int32 curHit = 1; curHit <= 10; curHit++)
            {
                cStoStorage.RegisterHit("TestFunction",
                    cStrSource);
            }         
            List<FunctionHit> pLisHits = cStoStorage.GetHits("TestFunction",
                cStrSource,
                DateTime.UtcNow.Subtract(new TimeSpan(1, 0, 0)));
            Assert.IsTrue(pLisHits.Count == 10);
        }

        #endregion

    }

}
