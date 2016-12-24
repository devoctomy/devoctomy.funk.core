using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Security.Claims;
using System.Threading.Tasks;

namespace devoctomy.funk.core.Extensions
{

    public static class ClaimsPrincipalExtensions
    {

        #region public enums

        public enum KeySource
        {
            none = 0,
            email = 1,
            custom = 2
        }

        #endregion

        #region public properties

        public static Func<ClaimsPrincipal, String> GetuserRowKeyCustomDelegate;

        public static Func<ClaimsPrincipal, String> GetuserPartitionKeyCustomDelegate;

        #endregion

        #region public methods

        public static String GetUserRowKey(this ClaimsPrincipal iUserPrincipal, 
            KeySource iSource)
        {
            switch(iSource)
            {
                case KeySource.none:
                    {
                        throw new NotImplementedException("iSource cannot be 'none'.");
                    }
                case KeySource.email:
                    {
                        return (iUserPrincipal.FindFirst(ClaimTypes.Email).Value);
                    }
                case KeySource.custom:
                    {
                        if(GetuserRowKeyCustomDelegate != null)
                        {
                            return (GetuserRowKeyCustomDelegate(iUserPrincipal));
                        }
                        else
                        {
                            throw new InvalidOperationException("iSource is 'custom' but GetUserRowKeyCustomDelegate has not been implemented.");
                        }
                    }
                default:
                    {
                        throw new NotImplementedException(String.Format("'{0}' has not been implemented as a user row key source.", iUserPrincipal.ToString()));
                    }
            }
        }

        public static String GetUserPartitionKey(this ClaimsPrincipal iUserPrincipal,
            KeySource iSource)
        {
            switch (iSource)
            {
                case KeySource.none:
                    {
                        throw new NotImplementedException("iSource cannot be 'none'.");
                    }
                case KeySource.email:
                    {
                        String pStrEmail = iUserPrincipal.FindFirst(ClaimTypes.Email).Value;
                        return (AzureTableHelpers.GetPartitionKeyFromEmailString(pStrEmail));
                    }
                case KeySource.custom:
                    {
                        if (GetuserPartitionKeyCustomDelegate != null)
                        {
                            return (GetuserPartitionKeyCustomDelegate(iUserPrincipal));
                        }
                        else
                        {
                            throw new InvalidOperationException("iSource is 'custom' but GetUserPartitionKeyCustomDelegate has not been implemented.");
                        }
                    }
                default:
                    {
                        throw new NotImplementedException(String.Format("'{0}' has not been implemented as a user parition key source.", iUserPrincipal.ToString()));
                    }
            }
        }

        #endregion

    }

}
