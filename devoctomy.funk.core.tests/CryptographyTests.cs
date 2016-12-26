using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using devoctomy.funk.core.Cryptography;
using Newtonsoft.Json.Linq;

namespace devoctomy.funk.core.tests
{

    [TestClass]
    public class CryptographyTests
    {

        #region private objects

        private String cStrPrivateKey = String.Empty;
        private String cStrPublicKey = String.Empty;

        #endregion

        #region public methods

        [TestInitialize()]
        public void Init()
        {
            CryptographyHelpers.CreateRSAKeyPair(out cStrPrivateKey,
                out cStrPublicKey,
                true);
        }

        [TestMethod]
        public void DeserialisePrivateKey()
        {
            RSAParametersSerialisable pRSAPrivate = RSAParametersSerialisable.FromJSON(cStrPrivateKey,
                true);
        }

        [TestMethod]
        public void DeserialisePublicKey()
        {
            RSAParametersSerialisable pRSAPublic = RSAParametersSerialisable.FromJSON(cStrPublicKey,
                true);
        }

        [TestMethod]
        public void SignDataWithPrivateVerifyWithPrivate()
        {
            RSAParametersSerialisable pRSAPrivate = RSAParametersSerialisable.FromJSON(cStrPrivateKey,
                true);
            Byte[] pBytData = System.Text.Encoding.UTF8.GetBytes("Hello World!");
            String pStrSignature = pRSAPrivate.Sign(pBytData);

            Assert.IsTrue(pRSAPrivate.VerifySignature(pBytData, pStrSignature));
        }

        [TestMethod]
        public void SignDataWithPrivateVerifyWithPublic()
        {
            RSAParametersSerialisable pRSAPrivate = RSAParametersSerialisable.FromJSON(cStrPrivateKey,
                true);
            Byte[] pBytData = System.Text.Encoding.UTF8.GetBytes("Hello World!");
            String pStrSignature = pRSAPrivate.Sign(pBytData);

            RSAParametersSerialisable pRSAPublic = RSAParametersSerialisable.FromJSON(cStrPublicKey,
                true);
            Assert.IsTrue(pRSAPublic.VerifySignature(pBytData, pStrSignature));
        }

        [TestMethod]
        public void SignJSONWithPrivateVerifyWithPrivate()
        {
            RSAParametersSerialisable pRSAPrivate = RSAParametersSerialisable.FromJSON(cStrPrivateKey,
                true);
            JObject pJOtJSON = new JObject();
            pJOtJSON.Add("FooBar", new JValue("Hello World!"));
            pRSAPrivate.Sign(pJOtJSON);

            Assert.IsTrue(pRSAPrivate.VerifySignature(pJOtJSON));
        }

        [TestMethod]
        public void SignJSONWithPrivateVerifyWithPublic()
        {
            RSAParametersSerialisable pRSAPrivate = RSAParametersSerialisable.FromJSON(cStrPrivateKey,
                true);
            JObject pJOtJSON = new JObject();
            pJOtJSON.Add("FooBar", new JValue("Hello World!"));
            pRSAPrivate.Sign(pJOtJSON);

            RSAParametersSerialisable pRSAPublic = RSAParametersSerialisable.FromJSON(cStrPublicKey,
                true);
            Assert.IsTrue(pRSAPublic.VerifySignature(pJOtJSON));
        }

        #endregion
    }

}
