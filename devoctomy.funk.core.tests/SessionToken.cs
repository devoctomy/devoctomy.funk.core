using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using devoctomy.funk.core.Cryptography;
using Newtonsoft.Json.Linq;
using devoctomy.funk.core.Environment;
using System.Threading;

namespace devoctomy.funk.core.tests
{
    [TestClass]
    public class SessionToken
    {

        #region private objects

        private String cStrPrivateKey = String.Empty;
        private String cStrPublicKey = String.Empty;
        private String cStrEmail = String.Empty;

        #endregion

        #region public methods

        [TestInitialize()]
        public void Init()
        {
            CryptographyHelpers.CreateRSAKeyPair(out cStrPrivateKey,
                out cStrPublicKey,
                true);
            cStrEmail = "helloworld@foobar.com";
            EnvironmentHelpers.SetEnvironmentVariable("DateTimeFormat", "yyyy-MM-ddThh:mm:ssZ", EnvironmentVariableTarget.Process);
        }

        [TestMethod]
        public void CreateSessionAndPrivateVerifyWithPrivate()
        {
            devoctomy.funk.core.Membership.SessionToken pSTnToken = new Membership.SessionToken(cStrEmail,
                new TimeSpan(1,0,0));
            String pStrJSON = pSTnToken.ToString(Newtonsoft.Json.Formatting.None,
                true,
                cStrPrivateKey);
            pSTnToken = Membership.SessionToken.FromJSON(pStrJSON);

            Assert.IsTrue(pSTnToken.Verify(cStrPublicKey));
        }

        [TestMethod]
        public void CreateSessionAndPrivateVerifyWithPrivateAfterExpired()
        {
            devoctomy.funk.core.Membership.SessionToken pSTnToken = new Membership.SessionToken(cStrEmail,
                new TimeSpan(0, 0, 1));
            String pStrJSON = pSTnToken.ToString(Newtonsoft.Json.Formatting.None,
                true,
                cStrPrivateKey);
            Thread.Sleep(2000);
            pSTnToken = Membership.SessionToken.FromJSON(pStrJSON);

            Assert.IsFalse(pSTnToken.Verify(cStrPublicKey));
        }

        #endregion

    }
}
