using System;
using Newtonsoft.Json.Linq;
using devoctomy.funk.core.Environment;

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
        private DateTime cDTeCreatedAt = DateTime.MinValue;
        private DateTime cDTeExpiresAt = DateTime.MinValue;

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
            cDTeExpiresAt = cDTeExpiresAt.Add(iLifeSpan);
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
            JObject pJOtJSON = JObject.Parse(iJSON);
            pSTnToken.cStrID = pJOtJSON["ID"].Value<String>();
            pSTnToken.cStrEmail = pJOtJSON["Email"].Value<String>();
            pSTnToken.cDTeCreatedAt = DateTime.ParseExact(pJOtJSON["CreatedAt"].Value<String>(), EnvironmentHelpers.GetEnvironmentVariable("DateTimeFormat"), System.Globalization.CultureInfo.InvariantCulture);
            pSTnToken.cDTeExpiresAt = DateTime.ParseExact(pJOtJSON["ExpiresAt"].Value<String>(), EnvironmentHelpers.GetEnvironmentVariable("DateTimeFormat"), System.Globalization.CultureInfo.InvariantCulture);
            return (pSTnToken);
        }

        /// <summary>
        /// Serialise this instance of the session token into a JSON string, used for transporting to the user
        /// </summary>
        /// <param name="iFormatting">JSON formatting to use when serialising</param>
        /// <returns>A JSON string representation of this session token</returns>
        public String ToString(Newtonsoft.Json.Formatting iFormatting)
        {
            JObject pJOtJSON = new JObject();
            pJOtJSON.Add("ID", new JValue(ID));
            pJOtJSON.Add("Email", new JValue(Email));
            pJOtJSON.Add("CreatedAt", new JValue(CreatedAt.ToString(EnvironmentHelpers.GetEnvironmentVariable("DateTimeFormat"))));
            pJOtJSON.Add("ExpiresAt", new JValue(ExpiresAt.ToString(EnvironmentHelpers.GetEnvironmentVariable("DateTimeFormat"))));
            return (pJOtJSON.ToString(iFormatting));
        }

        #endregion

    }

}
