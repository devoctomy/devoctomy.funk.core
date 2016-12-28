using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace devoctomy.funk.client
{

    public class FunkManagementRegisterResponse : AzureFunctionClientResponseBase<FunkServiceInfo>
    {

        #region constructor / destructor

        public FunkManagementRegisterResponse(Boolean iSuccess,
            FunkServiceInfo iResponse) :
            base(iSuccess, iResponse)
        { }

        #endregion

    }

}
