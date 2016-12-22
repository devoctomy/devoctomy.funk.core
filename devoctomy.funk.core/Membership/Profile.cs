#if !WINDOWS_UWP
    using Microsoft.WindowsAzure.Storage.Table;
#endif

using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace devoctomy.funk.core.Membership
{

    /// <summary>
    /// User profile, for containing per/user system settings
    /// </summary>
    public class Profile
    {

        #region private objects

        private Dictionary<String, Dictionary<String, Dictionary<String, String>>> cDicParams = null;

        #endregion

        #region public properties

        /// <summary>
        /// Get / set a field via its key, 'category_subcategory_field'
        /// </summary>
        /// <param name="iKey">Key of the field to get / set</param>
        /// <returns>The desired field as a string</returns>
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
        /// Private constructor used for internal use only
        /// </summary>
        private Profile()
        { }

        #endregion

        #region public methods

        /// <summary>
        /// Constructs an instance of Profile using the provided JSON string
        /// </summary>
        /// <param name="iJSON">JSON string representation of the profile to create</param>
        public static Profile FromJSON(String iJSON)
        {
            Profile pProProfile = new Profile();
            pProProfile.cDicParams = new Dictionary<String, Dictionary<String, Dictionary<String, String>>>();
            JObject pJOtDefaults = JObject.Parse(iJSON);
            foreach (JObject curCategory in pJOtDefaults["Categories"].Value<JArray>())
            {
                String pStrCategory = curCategory["Name"].Value<String>();
                pProProfile.cDicParams.Add(pStrCategory, new Dictionary<String, Dictionary<String, String>>());
                foreach (JObject curSubCategory in curCategory["SubCategories"].Value<JArray>())
                {
                    String pStrSubCategory = curSubCategory["Name"].Value<String>();
                    pProProfile.cDicParams[pStrCategory].Add(pStrSubCategory, new Dictionary<String, String>());
                    foreach (JObject curField in curSubCategory["Fields"].Value<JArray>())
                    {
                        String pStrFieldName = curField["Name"].Value<String>();
                        String pStrDefaultValue = curField["Value"].Value<String>();
                        pProProfile.cDicParams[pStrCategory][pStrSubCategory].Add(pStrFieldName, pStrDefaultValue);
                    }
                }
            }
            return (pProProfile);
        }

        /// <summary>
        /// Checks whether a key exists in this profile instance
        /// </summary>
        /// <param name="iKey">Key to check the existance of</param>
        /// <returns></returns>
        public Boolean HasKey(String iKey)
        {
            String[] pStrKeyParts = iKey.Split('_');
            if (pStrKeyParts.Length != 3) throw new ArgumentException("Key must contain 3 parts, category, sub category and value.  For example 'category_subcategory_value'.", "iKey");
            if(cDicParams.ContainsKey(pStrKeyParts[0]))
            {
                if(cDicParams[pStrKeyParts[0]].ContainsKey(pStrKeyParts[1]))
                {
                    return (cDicParams[pStrKeyParts[0]][pStrKeyParts[1]].ContainsKey(pStrKeyParts[2]));
                }
            }
            return (false);
        }

        /// <summary>
        /// Get a list of all profile keys, delimited by an underscore.  These can be passed into the
        /// default indexed property to set / get values
        /// </summary>
        /// <returns>A list of keys delimited by an underscore, 'category_subcategory_field'</returns>
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
                       pLisKeys.Add(pStrKey);
                    }
                }
            }
            return (pLisKeys);
        }

#if !WINDOWS_UWP

        /// <summary>
        /// Parse the profile from a dynamic table entity
        /// </summary>
        /// <param name="iDynamicTableEntity">The dynamic table entity to parse</param>
        /// <returns>An instance of profile with values parsed from the provided dynamic table entity</returns>
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

#endif

        /// <summary>
        /// Get the value of a specific field
        /// </summary>
        /// <param name="iCategory">Category of the field to get</param>
        /// <param name="iSubCategory">Sub category of the field to get</param>
        /// <param name="iField">Name of the field to get</param>
        /// <returns>The value of the field, as a string</returns>
        public String GetValue(String iCategory,
            String iSubCategory,
            String iField)
        {
            return (cDicParams[iCategory][iSubCategory][iField]);
        }

        /// <summary>
        /// Set the value of a specific field
        /// </summary>
        /// <param name="iCategory">Category of the field to set</param>
        /// <param name="iSubCategory">Sub category of the field to set</param>
        /// <param name="iField">Name of the field to set</param>
        /// <param name="iValue">New value for the field</param>
        public void SetValue(String iCategory,
            String iSubCategory,
            String iField,
            String iValue)
        {
            cDicParams[iCategory][iSubCategory][iField] = iValue;
        }

#if !WINDOWS_UWP

        /// <summary>
        /// Convert this profile instance to a dynamic table entity ready for inserting into storage
        /// </summary>
        /// <param name="iPartitionKey">The partition key to use for the profile</param>
        /// <param name="iRowKey">The row key to use for the profile</param>
        /// <returns>A dynamic table entity representing this profile instance</returns>
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

#endif

        /// <summary>
        /// Serialise this profile instance as a JSON string
        /// </summary>
        /// <param name="iFormatting">JSON formatting to use when serialising</param>
        /// <returns>A JSON string representation of this profile instance</returns>
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

#if !WINDOWS_UWP

        /// <summary>
        /// Inserts this profile instance into storage, under the provided user
        /// </summary>
        /// <param name="iStorage">The storage instance to use</param>
        /// <param name="iUser">The user to associate this profile with</param>
        /// <returns>True if the profile was replaced</returns>
        public Boolean Replace(Storage iStorage, 
            User iUser)
        {
            return(iStorage.ReplaceProfile(iUser,
                this));
        }

#endif

        /// <summary>
        /// Set values on this profile instance with values from another profile instance.  Only replaces values with matching keys.
        /// </summary>
        /// <param name="iSource">The source profile to get the values from</param>
        public void SetFrom(Profile iSource)
        {
            List<String> pLisSourceKeys = iSource.GetAllKeys();
            foreach(String curKey in pLisSourceKeys)
            {
                if(HasKey(curKey))
                {
                    this[curKey] = iSource[curKey];
                }
            }
        }

#endregion

    }

}
