using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace devoctomy.funk.client
{

    public class FunkClient : AzureFunctionClientBase
    {

        #region constructor / destructor

        public FunkClient(String iClientID,
            String iAzureFunctionRootURL) : 
            base(iClientID, iAzureFunctionRootURL)
        { }

        #endregion

        #region public methods



        #endregion

    }

}
