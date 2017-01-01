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

    public class FunkUserInfo : IJSONSerialisable
    {


        #region private objects

        private String cStrAuthenticated = String.Empty;
        private String cStrEmail = String.Empty;
        private String cStrNameIdentifier = String.Empty;
        private Boolean cBlnRegistered = false;
        private Boolean cBlnActivated = false;
        private Boolean cBlnLocked = false;

        #endregion

        #region public properties

        public String Authenticated
        {
            get { return (cStrAuthenticated); }
        }

        public String Email
        {
            get { return (cStrEmail); }
        }

        public String NameIdentifier
        {
            get { return (cStrNameIdentifier); }
        }

        public Boolean Registered
        {
            get { return (cBlnRegistered); }
        }

        public Boolean Activated
        {
            get { return (cBlnActivated); }
        }

        public Boolean Locked
        {
            get { return (cBlnLocked); }
        }

        #endregion

        #region public methods

        public static FunkUserInfo FromJSON(String iJSON)
        {
            FunkUserInfo pFIoInfo = new FunkUserInfo();
            using (JsonReader pJRrReader = new JsonTextReader(new StringReader(iJSON)))
            {
                pJRrReader.DateParseHandling = DateParseHandling.None;
                JObject pJOtJSON = JObject.Load(pJRrReader);
                pFIoInfo.cStrAuthenticated = pJOtJSON["Authenticated"].Value<String>();
                pFIoInfo.cStrEmail = pJOtJSON["Email"].Value<String>();
                pFIoInfo.cStrNameIdentifier = pJOtJSON["NameIdentifier"] != null ? pJOtJSON["NameIdentifier"].Value<String>() : String.Empty;
                pFIoInfo.cBlnRegistered = pJOtJSON["Registered"].Value<Boolean>();
                pFIoInfo.cBlnActivated  = pJOtJSON["Activated"] != null ? pJOtJSON["Activated"].Value<Boolean>() : false;
                pFIoInfo.cBlnLocked = pJOtJSON["Locked"] != null ? pJOtJSON["Locked"].Value<Boolean>() : false;
            }
            return (pFIoInfo);
        }

        public JObject ToJObject()
        {
            JObject pJOtJSON = new JObject();
            pJOtJSON.Add("Authenticated", new JValue(Authenticated));
            pJOtJSON.Add("Email", new JValue(Email));
            pJOtJSON.Add("NameIdentifier", new JValue(NameIdentifier));
            pJOtJSON.Add("Registered", new JValue(Registered));
            pJOtJSON.Add("Activated", new JValue(Activated));
            pJOtJSON.Add("Locked", new JValue(Locked));
            return (pJOtJSON);
        }

        #endregion

    }

}
