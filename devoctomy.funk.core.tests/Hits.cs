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
    public class Hits
    {

        #region private objects

        private String cStrTableStorageRootURL = String.Empty;
        private String cStrConnectionString = String.Empty;
        private Storage cStoStorage;

        #endregion

        #region private methods

        private void LoadAzureCredentials()
        {
            String pStrCreds = File.ReadAllText("../../../../azure/creds.json");
            JObject pJOtCreds = JObject.Parse(pStrCreds);
            cStrTableStorageRootURL = pJOtCreds["TableStorageRootURL"].Value<String>();
            cStrConnectionString = pJOtCreds["ConnectionString"].Value<String>();
            EnvironmentHelpers.SetEnvironmentVariable("AzureWebJobsStorage", cStrConnectionString, EnvironmentVariableTarget.Process);

            //Init storage
            cStoStorage = new Storage(cStrTableStorageRootURL,
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
            Storage pStoStorage = new Storage(cStrTableStorageRootURL,
                "AzureWebJobsStorage",
                "Test");
            for(Int32 curHit = 1; curHit <= 10; curHit++)
            {
                pStoStorage.RegisterHit("TestFunction", 
                    "127.0.0.1");
            }
            List<FunctionHit> pLisHits = pStoStorage.GetHits("TestFunction",
                "127.0.0.1",
                DateTime.UtcNow.Subtract(new TimeSpan(1, 0, 0)));
            Assert.IsTrue(pLisHits.Count > 10);
        }

        #endregion

    }

}
