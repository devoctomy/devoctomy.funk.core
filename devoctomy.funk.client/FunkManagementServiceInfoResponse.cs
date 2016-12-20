using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace devoctomy.funk.client
{

    public class FunkManagementServiceInfoResponse : AzureFunctionClientResponseBase<FunkServiceInfo>
    {

        #region constructor / destructor

        public FunkManagementServiceInfoResponse(Boolean iSuccess,
            FunkServiceInfo iResponse) :
            base(iSuccess, iResponse)
        { }

        #endregion

    }

}
