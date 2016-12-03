using System;
using System.Linq;

namespace devoctomy.funk.core
{

    /// <summary>
    /// Helper functions for Azure table storage
    /// </summary>
    public class AzureTableHelpers
    {

        #region public methods

        /// <summary>
        /// Takes an email as input and turns it into a basic partition key for use in Azure table storage.
        /// </summary>
        /// <param name="iString">Email to generate partition key from.</param>
        /// <returns>Partition key in uppercase, consisting of first character and first character after '@' symbol.</returns>
        public static String GetPartitionKeyFromEmailString(String iString)
        {
            if (!iString.Contains('@')) throw new ArgumentException("iString must be a vailid email format.", "iString");

            String[] pStrParts = iString.ToUpper().Split('@');
            return (String.Format("{0}{1}", pStrParts[0][0], pStrParts[1][0]));
        }

        #endregion

    }

}
