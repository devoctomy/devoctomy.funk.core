using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace devoctomy.funk.core.Cryptography
{
    using Extensions;
    using Newtonsoft.Json.Linq;
    using System;
    using System.Runtime.Serialization;
    using System.Security.Cryptography;

    public class RSAParametersSerialisable
    {

        #region private objects

        private RSAParameters cRSAParameters;

        #endregion

        #region public properties

        public RSAParameters RSAParameters
        {
            get { return(cRSAParameters); }
        }

        #endregion

        #region constructor / destructor

        public RSAParametersSerialisable(RSAParameters iRSAParameters)
        {
            cRSAParameters = iRSAParameters;
        }

        #endregion

        #region public methods

        public void Sign(JObject iJSON)
        {
            Byte[] pBytData = Encoding.UTF8.GetBytes(iJSON.ToString(Newtonsoft.Json.Formatting.None));
            String pStrSignature = Sign(pBytData);
            JObject pJOtSignature = new JObject();
            pJOtSignature.Add("Base64Value", pStrSignature);
            iJSON.Add("Signature", pJOtSignature);
        }

        public Boolean VerifySignature(JObject iJSON)
        {
            JObject pJOtCopy = JObject.Parse(iJSON.ToString(Newtonsoft.Json.Formatting.None));
            String pStrSignature = pJOtCopy["Signature"]["Base64Value"].Value<String>();
            pJOtCopy.Remove("Signature");
            Byte[] pBytData = Encoding.UTF8.GetBytes(pJOtCopy.ToString(Newtonsoft.Json.Formatting.None));
            return (VerifySignature(pBytData, Convert.FromBase64String(pStrSignature)));
        }

        public String Sign(Byte[] iData)
        {
            using (SHA256 pSHAHash = SHA256CryptoServiceProvider.Create())
            {
                Byte[] pBytSHAHash = pSHAHash.ComputeHash(iData);
                using (RSACryptoServiceProvider pCSPRSA = (RSACryptoServiceProvider)RSACryptoServiceProvider.Create())
                {
                    pCSPRSA.ImportParameters(RSAParameters);
                    Byte[] pBytSignature = pCSPRSA.SignHash(pBytSHAHash, CryptoConfig.MapNameToOID("SHA256"));
                    return (Convert.ToBase64String(pBytSignature));
                }
            }
        }

        public Boolean VerifySignature(Byte[] iData,
            String iSignature)
        {
            return (VerifySignature(iData,
                Convert.FromBase64String(iSignature)));
        }

        public Boolean VerifySignature(Byte[] iData,
            Byte[] iSignature)
        {
            using (SHA256 pSHAHash = SHA256CryptoServiceProvider.Create())
            {
                Byte[] pBytSHAHash = pSHAHash.ComputeHash(iData);
                using (RSACryptoServiceProvider pCSPRSA = (RSACryptoServiceProvider)RSACryptoServiceProvider.Create())
                {
                    pCSPRSA.ImportParameters(RSAParameters);
                    return(pCSPRSA.VerifyHash(pBytSHAHash, CryptoConfig.MapNameToOID("SHA256"), iSignature));
                }
            }
        }

        public String ToJSON(Boolean iPrivate,
            Newtonsoft.Json.Formatting iFormatting)
        {
            JObject pJOtRSAParams = new JObject();
            if(iPrivate)
            {
                pJOtRSAParams.Add("D", Convert.ToBase64String(RSAParameters.D));
                pJOtRSAParams.Add("DP", Convert.ToBase64String(RSAParameters.DP));
                pJOtRSAParams.Add("DQ", Convert.ToBase64String(RSAParameters.DQ));
                pJOtRSAParams.Add("Exponent", Convert.ToBase64String(RSAParameters.Exponent));
                pJOtRSAParams.Add("InverseQ", Convert.ToBase64String(RSAParameters.InverseQ));
                pJOtRSAParams.Add("Modulus", Convert.ToBase64String(RSAParameters.Modulus));
                pJOtRSAParams.Add("P", Convert.ToBase64String(RSAParameters.P));
                pJOtRSAParams.Add("Q", Convert.ToBase64String(RSAParameters.Q));
            }
            else
            {
                pJOtRSAParams.Add("Exponent", Convert.ToBase64String(RSAParameters.Exponent));
                pJOtRSAParams.Add("Modulus", Convert.ToBase64String(RSAParameters.Modulus));
            }
            return (pJOtRSAParams.ToString(iFormatting));
        }

        public static RSAParametersSerialisable FromJSON(String iJSON,
            Boolean iHexEncoded)
        {
            if (iHexEncoded) iJSON = iJSON.HexDecode();
            JObject pJOtJSON = JObject.Parse(iJSON);
            RSAParameters pRSAParams = new RSAParameters();
            if (pJOtJSON["D"] != null) pRSAParams.D = Convert.FromBase64String(pJOtJSON["D"].Value<String>());
            if (pJOtJSON["DP"] != null) pRSAParams.DP = Convert.FromBase64String(pJOtJSON["DP"].Value<String>());
            if (pJOtJSON["DQ"] != null) pRSAParams.DQ = Convert.FromBase64String(pJOtJSON["DQ"].Value<String>());
            if (pJOtJSON["Exponent"] != null) pRSAParams.Exponent = Convert.FromBase64String(pJOtJSON["Exponent"].Value<String>());
            if (pJOtJSON["InverseQ"] != null) pRSAParams.InverseQ = Convert.FromBase64String(pJOtJSON["InverseQ"].Value<String>());
            if (pJOtJSON["Modulus"] != null) pRSAParams.Modulus = Convert.FromBase64String(pJOtJSON["Modulus"].Value<String>());
            if (pJOtJSON["P"] != null) pRSAParams.P = Convert.FromBase64String(pJOtJSON["P"].Value<String>());
            if (pJOtJSON["Q"] != null) pRSAParams.Q = Convert.FromBase64String(pJOtJSON["Q"].Value<String>());
            RSAParametersSerialisable pRSASerialParams = new RSAParametersSerialisable(pRSAParams);
            return (pRSASerialParams);
        }

        #endregion

    }

}
