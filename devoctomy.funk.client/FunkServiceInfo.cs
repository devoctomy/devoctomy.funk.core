using devoctomy.funk.core.JSON;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace devoctomy.funk.client
{

    public class FunkServiceInfo : IJSONSerialisable
    {

        #region private objects

        private String cStrName = String.Empty;
        private String cStrVersion = String.Empty;
        private DateTime cDTeInfoRequestedAt = new DateTime(1982, 4, 3);
        private String cStrPublicKey = String.Empty;
        private String cStrFacebookCallbackURL = String.Empty;

        #endregion

        #region public properties

        public String Name
        {
            get { return (cStrName); }
        }

        public String Version
        {
            get { return (cStrVersion); }
        }

        public DateTime InfoRequestedAt
        {
            get { return (cDTeInfoRequestedAt); }
        }

        public String PublicKey
        {
            get { return (cStrPublicKey); }
        }

        public String FacebookCallbackURL
        {
            get { return (cStrFacebookCallbackURL); }
        }

        #endregion

        #region public methods

        public static FunkServiceInfo FromJSON(String iJSON)
        {
            FunkServiceInfo pFIoInfo = new FunkServiceInfo();
            using (JsonReader pJRrReader = new JsonTextReader(new StringReader(iJSON)))
            {
                pJRrReader.DateParseHandling = DateParseHandling.None;
                JObject pJOtJSON = JObject.Load(pJRrReader);
                pFIoInfo.cStrName = pJOtJSON["Name"].Value<String>();
                pFIoInfo.cStrVersion = pJOtJSON["Version"].Value<String>();
                //pFIoInfo.cDTeInfoRequestedAt = DateTime.ParseExact(pJOtJSON["InfoRequestedAt"].Value<String>(), EnvironmentHelpers.GetEnvironmentVariable("DateTimeFormat"), System.Globalization.CultureInfo.InvariantCulture);
                pFIoInfo.cStrPublicKey = pJOtJSON["PublicKey"].Value<String>();
                pFIoInfo.cStrFacebookCallbackURL = pJOtJSON["FacebookCallbackURL"].Value<String>();
            }
            return (pFIoInfo);
        }

        public JObject ToJObject()
        {
            JObject pJOtJSON = new JObject();
            pJOtJSON.Add("Name", new JValue(Name));
            pJOtJSON.Add("Version", new JValue(Version));
            pJOtJSON.Add("InfoRequestedAt", new JValue(InfoRequestedAt.ToString("")));
            pJOtJSON.Add("PublicKey", new JValue(PublicKey));
            pJOtJSON.Add("FacebookCallbackURL", new JValue(FacebookCallbackURL));
            return (pJOtJSON);
        }

        #endregion

    }

}
