using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace devoctomy.funk.core.Membership
{

    public class SessionTokenSignature
    {

        #region private objects

        private String cStrBase64Value = String.Empty;

        #endregion

        #region public properties

        public String Base64Value
        {
            get { return (cStrBase64Value); }
        }

        #endregion

        #region constructor / destructor

        private SessionTokenSignature(String iBase64Value)
        {
            cStrBase64Value = iBase64Value;
        }

        #endregion

        #region public methods

        public static SessionTokenSignature FromJSON(JObject iJSON)
        {
            return (new SessionTokenSignature(iJSON["Base64Value"].Value<String>()));
        }

        public JObject ToJSON()
        {
            JObject pJOtJSON = new JObject();
            pJOtJSON.Add("Base64Value", new JValue(Base64Value));
            return (pJOtJSON);
        }

        #endregion

    }

}
