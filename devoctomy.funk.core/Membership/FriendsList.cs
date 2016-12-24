using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace devoctomy.funk.core.Membership
{

    public class FriendsList
    {

        #region private objects

        private List<Friend> cLisFriends = null;

        #endregion

        #region public properties

        public IReadOnlyList<Friend> Friends
        {
            get { return (cLisFriends); }
        }

        #endregion

        #region constructor / destructor

        public FriendsList()
        { }

        #endregion

        #region public methods



        #endregion

    }

}
