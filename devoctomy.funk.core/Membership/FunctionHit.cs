using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace devoctomy.funk.core.Membership
{

    public class FunctionHit : TableEntity
    {

        #region constructor / destructor 

        private FunctionHit(String iFunctionName,
            String iSource)
        {
            PartitionKey = String.Format("{0}_{1}", iFunctionName, iSource);
            RowKey = Guid.NewGuid().ToString();
        }

        public FunctionHit()
        { }

        #endregion

        #region public methods

        public static FunctionHit Create(String iFunctionName,
            User iUser)
        {
            FunctionHit pFHtHit = new FunctionHit(iFunctionName,
                iUser.RowKey);
            return (pFHtHit);
        }

        public static FunctionHit Create(String iFunctionName,
            String iSource)
        {
            FunctionHit pFHtHit = new FunctionHit(iFunctionName,
                iSource);
            return (pFHtHit);
        }

        #endregion

    }

}
