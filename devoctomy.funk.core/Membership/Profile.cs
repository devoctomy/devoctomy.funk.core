using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace devoctomy.funk.core.Membership
{

    public class Profile
    {

        #region private objects

        private Dictionary<String, Dictionary<String, Dictionary<String, String>>> cDicParams = null;

        #endregion

        #region constructor / destructor

        public Profile(String iDefaultJSON)
        {
            cDicParams = new Dictionary<String, Dictionary<String, Dictionary<String, String>>>();
            JObject pJOtDefaults = JObject.Parse(iDefaultJSON);
            foreach (JObject curCategory in pJOtDefaults["Categories"].Value<JArray>())
            {
                String pStrCategory = curCategory["Name"].Value<String>();
                cDicParams.Add(pStrCategory, new Dictionary<String, Dictionary<String, String>>());
                foreach (JObject curSubCategory in curCategory["SubCategories"].Value<JArray>())
                {
                    String pStrSubCategory = curSubCategory["Name"].Value<String>();
                    cDicParams[pStrCategory].Add(pStrSubCategory, new Dictionary<String, String>());
                    foreach (JObject curField in curSubCategory["Fields"].Value<JArray>())
                    {
                        String pStrFieldName = curField["Name"].Value<String>();
                        String pStrDefaultValue = curField["Default"].Value<String>();
                        cDicParams[pStrCategory][pStrCategory].Add(pStrFieldName, pStrDefaultValue);
                    }
                }
            }
        }

        #endregion

        #region public methods

        public String GetValue(String iCategory,
            String iSubCategory,
            String iField)
        {
            return (cDicParams[iCategory][iSubCategory][iField]);
        }

        public void SetValue(String iCategory,
            String iSubCategory,
            String iField,
            String iValue)
        {
            cDicParams[iCategory][iSubCategory][iField] = iValue;
        }

        public void Update()
        {

        }

        #endregion

    }

}
