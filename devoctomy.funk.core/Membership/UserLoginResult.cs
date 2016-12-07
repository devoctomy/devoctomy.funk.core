using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace devoctomy.funk.core.Membership
{


    /// <summary>
    /// Result of the membership login process
    /// </summary>
    public class UserLoginResult
    {

        #region public methods

        /// <summary>
        /// Login was successful?
        /// </summary>
        public Boolean Success { get; set; }

        /// <summary>
        /// Signed session token created during a successful login / account activation
        /// </summary>
        public SessionToken SessionToken { get; set; }

        /// <summary>
        /// The user account was already locked
        /// </summary>
        public Boolean AccountPreviouslyLocked { get; set; }

        /// <summary>
        /// The user account was locked due to this login attempt being incorrect
        /// </summary>
        public Boolean AccountLocked { get; set; }

        #endregion

    }

}
