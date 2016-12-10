using Microsoft.WindowsAzure.Storage.Table;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace devoctomy.funk.core.Membership
{

    /// <summary>
    /// 
    /// </summary>
    public class Profile
    {

        #region private objects

        private Dictionary<String, Dictionary<String, Dictionary<String, String>>> cDicParams = null;

        #endregion

        #region public properties

        /// <summary>
        /// 
        /// </summary>
        /// <param name="iKey"></param>
        /// <returns></returns>
        public String this[String iKey]
        {
            get
            {
                String[] pStrKeyParts = iKey.Split('_');
                return(GetValue(pStrKeyParts[0],
                    pStrKeyParts[1],
                    pStrKeyParts[2]));
            }
            set
            {
                String[] pStrKeyParts = iKey.Split('_');
                SetValue(pStrKeyParts[0],
                    pStrKeyParts[1],
                    pStrKeyParts[2],
                    value);
            }
        }

        #endregion

        #region constructor / destructor

        /// <summary>
        /// 
        /// </summary>
        private Profile()
        { }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="iDefaultJSON"></param>
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
                        String pStrDefaultValue = curField["Value"].Value<String>();
                        cDicParams[pStrCategory][pStrSubCategory].Add(pStrFieldName, pStrDefaultValue);
                    }
                }
            }
        }

        #endregion

        #region public methods

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public List<String> GetAllKeys()
        {
            List<String> pLisKeys = new List<String>();
            foreach (String curCategory in cDicParams.Keys)
            {
                foreach (String curSubCategory in cDicParams[curCategory].Keys)
                {
                    foreach (String curField in cDicParams[curCategory][curSubCategory].Keys)
                    {
                        String pStrKey = String.Format("{0}_{1}_{2}",
                            curCategory,
                            curSubCategory,
                            curField);
                        EntityProperty pEPyField = new EntityProperty(cDicParams[curCategory][curSubCategory][curField]);
                        pLisKeys.Add(pStrKey);
                    }
                }
            }
            return (pLisKeys);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="iDynamicTableEntity"></param>
        /// <returns></returns>
        public static Profile FromDynamicTableEntity(DynamicTableEntity iDynamicTableEntity)
        {
            Profile pProProfile = new Profile();
            pProProfile.cDicParams = new Dictionary<String, Dictionary<String, Dictionary<String, String>>>();
            foreach (String curKey in iDynamicTableEntity.Properties.Keys)
            {
                EntityProperty pEPyProperty = iDynamicTableEntity.Properties[curKey];
                String[] pStrKeyParts = curKey.Split('_');
                if(!pProProfile.cDicParams.ContainsKey(pStrKeyParts[0]))
                {
                    //category
                    pProProfile.cDicParams.Add(pStrKeyParts[0], new Dictionary<String, Dictionary<String, String>>());
                }
                if(!pProProfile.cDicParams[pStrKeyParts[0]].ContainsKey(pStrKeyParts[1]))
                {
                    //sub category
                    pProProfile.cDicParams[pStrKeyParts[0]].Add(pStrKeyParts[1], new Dictionary<String, String>());
                }
                pProProfile.cDicParams[pStrKeyParts[0]][pStrKeyParts[1]].Add(pStrKeyParts[2], pEPyProperty.StringValue);
            }
            return (pProProfile);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="iCategory"></param>
        /// <param name="iSubCategory"></param>
        /// <param name="iField"></param>
        /// <returns></returns>
        public String GetValue(String iCategory,
            String iSubCategory,
            String iField)
        {
            return (cDicParams[iCategory][iSubCategory][iField]);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="iCategory"></param>
        /// <param name="iSubCategory"></param>
        /// <param name="iField"></param>
        /// <param name="iValue"></param>
        public void SetValue(String iCategory,
            String iSubCategory,
            String iField,
            String iValue)
        {
            cDicParams[iCategory][iSubCategory][iField] = iValue;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="iPartitionKey"></param>
        /// <param name="iRowKey"></param>
        /// <returns></returns>
        public DynamicTableEntity ToDynamicTableEntity(String iPartitionKey,
            String iRowKey)
        {
            DynamicTableEntity pDTEEntity = new DynamicTableEntity(iPartitionKey,
                iRowKey);
            foreach (String curCategory in cDicParams.Keys)
            {
                foreach (String curSubCategory in cDicParams[curCategory].Keys)
                {
                    foreach (String curField in cDicParams[curCategory][curSubCategory].Keys)
                    {
                        String pStrKey = String.Format("{0}_{1}_{2}",
                            curCategory,
                            curSubCategory,
                            curField);
                        EntityProperty pEPyField = new EntityProperty(cDicParams[curCategory][curSubCategory][curField]);
                        pDTEEntity.Properties.Add(pStrKey, pEPyField);
                    }
                }
            }
            return (pDTEEntity);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="iFormatting"></param>
        /// <returns></returns>
        public String ToJSON(Newtonsoft.Json.Formatting iFormatting)
        {
            JObject pJOtJSON = new JObject();
            JArray pJAyCategories = new JArray();
            foreach(String curCategory in cDicParams.Keys)
            {
                JObject pJOtCategory = new JObject();
                pJOtCategory.Add("Name", new JValue(curCategory));
                JArray pJAySubCategories = new JArray();
                foreach (String curSubCategory in cDicParams[curCategory].Keys)
                {
                    JObject pJOtSubCategory = new JObject();
                    pJOtSubCategory.Add("Name", new JValue(curSubCategory));
                    JArray pJAyFields = new JArray();
                    foreach(String curField in cDicParams[curCategory][curSubCategory].Keys)
                    {
                        JObject pJOtField = new JObject();
                        pJOtField.Add("Name", new JValue(curField));
                        pJOtField.Add("Value", new JValue(cDicParams[curCategory][curSubCategory][curField]));
                        pJAyFields.Add(pJOtField);
                    }
                    pJOtSubCategory.Add("Fields", pJAyFields);
                    pJAySubCategories.Add(pJOtSubCategory);
                }
                pJOtCategory.Add("SubCategories", pJAySubCategories);
                pJAyCategories.Add(pJOtCategory);
            }
            pJOtJSON.Add("Categories", pJAyCategories);
            return (pJOtJSON.ToString(iFormatting));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="iStorage"></param>
        /// <param name="iUser"></param>
        /// <returns></returns>
        public Boolean Replace(Storage iStorage, 
            User iUser)
        {
            return(iStorage.ReplaceProfile(iUser,
                this));
        }

        #endregion

    }

}
