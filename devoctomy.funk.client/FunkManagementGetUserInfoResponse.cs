using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace devoctomy.funk.client
{

    public class FunkManagementGetUserInfoResponse : AzureFunctionClientResponseBase<FunkUserInfo>
    {

        #region constructor / destructor

        public FunkManagementGetUserInfoResponse(Boolean iSuccess,
            FunkUserInfo iResponse) :
            base(iSuccess, iResponse)
        { }

        #endregion

    }

}
