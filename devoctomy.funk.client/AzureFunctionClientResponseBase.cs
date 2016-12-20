using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace devoctomy.funk.client
{

    public class AzureFunctionClientResponseBase<ResponseType>
    {

        #region private objects

        private Boolean cBlnSuccess;
        private ResponseType cRTeResponse;

        #endregion

        #region public properties

        public Boolean Success
        {
            get { return (cBlnSuccess); }
        }

        public ResponseType Response
        {
            get { return (cRTeResponse); }
        }

        #endregion

        #region constructor / destructor

        public AzureFunctionClientResponseBase(Boolean iSuccess,
            ResponseType iResponse)
        {
            cBlnSuccess = iSuccess;
            cRTeResponse = iResponse;
        }

        #endregion

    }

}
