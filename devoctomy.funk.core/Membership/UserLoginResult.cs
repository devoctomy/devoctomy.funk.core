using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace devoctomy.funk.core.Membership
{

    public class UserLoginResult
    {

        #region public methods

        public Boolean Success { get; set; }

        public SessionToken SessionToken { get; set; }

        #endregion

    }

}
