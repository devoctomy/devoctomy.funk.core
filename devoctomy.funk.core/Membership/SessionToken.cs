using System;
using Newtonsoft.Json.Linq;
using devoctomy.funk.core.Environment;
using devoctomy.funk.core.Cryptography;
using System.IO;
using Newtonsoft.Json;

namespace devoctomy.funk.core.Membership
{

    /// <summary>
    /// Session token, required for making authenticated API calls
    /// </summary>
    public class SessionToken
    {

        #region private objects

        private String cStrID = Guid.NewGuid().ToString();
        private String cStrEmail = String.Empty;
        private DateTime cDTeCreatedAt = new DateTime(1982, 4, 3);
        private DateTime cDTeExpiresAt = new DateTime(1982, 4, 3);
        private SessionTokenSignature cSTSSignature;

        #endregion

        #region public properties

        /// <summary>
        /// Unique ID for this session
        /// </summary>
        public String ID
        {
            get { return (cStrID); }
        }

        /// <summary>
        /// Email address of the user that the session was created for
        /// </summary>
        public String Email
        {
            get { return (cStrEmail); }
        }

        /// <summary>
        /// Date / Time the session was created
        /// </summary>
        public DateTime CreatedAt
        {
            get { return (cDTeCreatedAt); }
        }

        /// <summary>
        /// Date / Time the session token expires
        /// </summary>
        public DateTime ExpiresAt
        {
            get { return (cDTeExpiresAt); }
        }

        /// <summary>
        /// Session token signature, if signed
        /// </summary>
        public SessionTokenSignature Signature
        {
            get { return (cSTSSignature); }
        }

        #endregion

        #region constructor / destructor

        /// <summary>
        /// Private constructor used for static method construction
        /// </summary>
        private SessionToken()
        { }

        /// <summary>
        /// Constructs an instance of the session token for the provided user email
        /// </summary>
        /// <param name="iEmail">Email address of the user to create the session for</param>
        /// <param name="iLifeSpan">The lifespan of the session token to generate</param>
        public SessionToken(String iEmail,
            TimeSpan iLifeSpan)
        {
            cStrEmail = iEmail;
            cDTeCreatedAt = DateTime.UtcNow;
            cDTeExpiresAt = cDTeCreatedAt.Add(iLifeSpan);
        }

        #endregion

        #region public methods

        /// <summary>
        /// Parses a session token from its JSON string
        /// </summary>
        /// <param name="iJSON">JSON string representation of the session token</param>
        /// <returns>A session token instance</returns>
        public static SessionToken FromJSON(String iJSON)
        {
            SessionToken pSTnToken = new SessionToken();
            JsonReader pJRrReader = new JsonTextReader(new StringReader(iJSON));
            pJRrReader.DateParseHandling = DateParseHandling.None;
            JObject pJOtJSON = JObject.Load(pJRrReader);
            pSTnToken.cStrID = pJOtJSON["ID"].Value<String>();
            pSTnToken.cStrEmail = pJOtJSON["Email"].Value<String>();
            pSTnToken.cDTeCreatedAt = DateTime.ParseExact(pJOtJSON["CreatedAt"].Value<String>(), EnvironmentHelpers.GetEnvironmentVariable("DateTimeFormat"), System.Globalization.CultureInfo.InvariantCulture);
            pSTnToken.cDTeExpiresAt = DateTime.ParseExact(pJOtJSON["ExpiresAt"].Value<String>(), EnvironmentHelpers.GetEnvironmentVariable("DateTimeFormat"), System.Globalization.CultureInfo.InvariantCulture);
            if(pJOtJSON["Signature"] != null)
            {
                pSTnToken.cSTSSignature = SessionTokenSignature.FromJSON(pJOtJSON["Signature"].Value<JObject>());
            }
            return (pSTnToken);
        }

        /// <summary>
        /// Serialise this instance of the session token into a JSON string, used for transporting to the user
        /// </summary>
        /// <param name="iFormatting">JSON formatting to use when serialising</param>
        /// <param name="iSign">True if the resulting JSON should be signed</param>
        /// <param name="iPrivateRSAKey">The hex encoded RSA private key used to sign the JSON</param>
        /// <returns>A JSON string representation of this session token</returns>
        public String ToString(Newtonsoft.Json.Formatting iFormatting,
            Boolean iSign = false,
            String iPrivateRSAKey = "")
        {
            if (iSign && String.IsNullOrEmpty(iPrivateRSAKey)) throw new ArgumentException("Private RSA key has not been supplied.", "iPrivateRSAKey");

            JObject pJOtJSON = new JObject();
            pJOtJSON.Add("ID", new JValue(ID));
            pJOtJSON.Add("Email", new JValue(Email));
            pJOtJSON.Add("CreatedAt", new JValue(CreatedAt.ToString(EnvironmentHelpers.GetEnvironmentVariable("DateTimeFormat"))));
            pJOtJSON.Add("ExpiresAt", new JValue(ExpiresAt.ToString(EnvironmentHelpers.GetEnvironmentVariable("DateTimeFormat"))));
            if(iSign)
            {
                RSAParametersSerialisable pRSACrypto = RSAParametersSerialisable.FromJSON(iPrivateRSAKey, true);
                pRSACrypto.Sign(pJOtJSON);
            }
            else
            {
                if(Signature != null)
                {
                    pJOtJSON.Add("Signature", Signature.ToJSON());
                }
            }
            return (pJOtJSON.ToString(iFormatting));
        }

        /// <summary>
        /// Verify that this session token is still valid using the provided RSA public key
        /// </summary>
        /// <param name="iPublicRSAKey">Public RSA key to verify the sdession token with as hex string</param>
        /// <returns>True if the session token has a valid signature and has not expired</returns>
        public Boolean Verify(String iPublicRSAKey)
        {
            RSAParametersSerialisable pRSAPublic = RSAParametersSerialisable.FromJSON(iPublicRSAKey,
                true);
            String pStrJSON = ToString(Newtonsoft.Json.Formatting.None);
            JObject pJOtJSON = JObject.Parse(pStrJSON);
            if(pRSAPublic.VerifySignature(pJOtJSON))
            {
                if(ExpiresAt > DateTime.UtcNow)
                {
                    return (true);
                }
            }
            return (false);
        }

        #endregion

    }

}
