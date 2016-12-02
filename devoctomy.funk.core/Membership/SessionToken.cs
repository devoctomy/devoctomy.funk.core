using System;
using Newtonsoft.Json.Linq;

namespace devoctomy.funk.core.Membership
{

    public class SessionToken
    {

        #region private objects

        private String cStrID = Guid.NewGuid().ToString();
        private String cStrEmail = String.Empty;
        private DateTime cDTeCreatedAt;
        private DateTime cDTeExpiresAt;

        #endregion

        #region public properties

        public String ID
        {
            get { return (cStrID); }
        }

        public String Email
        {
            get { return (cStrEmail); }
        }

        public DateTime CreatedAt
        {
            get { return (cDTeCreatedAt); }
        }

        public DateTime ExpiresAt
        {
            get { return (cDTeExpiresAt); }
        }

        #endregion

        #region constructor / destructor

        public SessionToken(String iEmail)
        {
            cStrEmail = iEmail;
        }

        #endregion

        #region public methods

        public SessionToken FromJSON(String iJSON)
        {
            JObject pJOtJSON = JObject.Parse(iJSON);
        }

        public String ToString(Newtonsoft.Json.Formatting iFormatting)
        {
            JObject pJOtJSON = new JObject();
            //add props here
            return(pJOtJSON.ToString(iFormatting));
        }

        #endregion

    }

}
