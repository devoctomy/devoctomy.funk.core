using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace devoctomy.funk.client
{

    public class FunkManagementGetActivateResponse : AzureFunctionClientResponseBase<Object>
    {

        #region constructor / destructor

        public FunkManagementGetActivateResponse(Boolean iSuccess,
            Object iResponse) :
            base(iSuccess, iResponse)
        { }

        #endregion

    }

}
