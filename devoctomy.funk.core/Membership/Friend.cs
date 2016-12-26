using devoctomy.funk.core.Cryptography;
using devoctomy.funk.core.Extensions;
using Microsoft.WindowsAzure.Storage.Table;
using Newtonsoft.Json.Linq;
using System;
using System.Security.Claims;
using System.Threading.Tasks;

namespace devoctomy.funk.core.Membership
{

    /// <summary>
    /// Friend of a user of the membership system
    /// </summary>
    public class Friend : TableEntity
    {

        #region public properties

        /// <summary>
        /// Row key of the user associated with this friend
        /// </summary>
        public String UserRowKey { get; set; }

        /// <summary>
        /// User name of the user associated with this friend
        /// </summary>
        public String UserName { get; set; }

        #endregion

        #region constructor / destructor

        /// <summary>
        /// Constructions an instance of user
        /// </summary>
        public Friend()
        { }

        #endregion

        #region public methods

        public JObject ToJObject()
        {
            JObject pJOtObject = new JObject();
            pJOtObject.Add("UserName", new JValue(UserName));
            return (pJOtObject);
        }

        #endregion

    }

}
