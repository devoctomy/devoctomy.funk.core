using devoctomy.funk.core.Membership;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace devoctomy.funk.client
{

    public class FunkManagementGetProfileResponse : AzureFunctionClientResponseBase<Profile>
    {

        #region constructor / destructor

        public FunkManagementGetProfileResponse(Boolean iSuccess,
            Profile iResponse) :
            base(iSuccess, iResponse)
        { }

        #endregion

    }

}
