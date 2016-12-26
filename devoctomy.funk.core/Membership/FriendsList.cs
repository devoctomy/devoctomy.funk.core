using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace devoctomy.funk.core.Membership
{

    /// <summary>
    /// 
    /// </summary>
    public class FriendsList
    {

        #region private objects

        private User cUsrOwner;
        private List<Friend> cLisFriends = null;

        #endregion

        #region public properties

        /// <summary>
        /// 
        /// </summary>
        public User Owner
        {
            get { return (cUsrOwner); }
        }

        /// <summary>
        /// 
        /// </summary>
        public IReadOnlyList<Friend> Friends
        {
            get { return (cLisFriends); }
        }

        #endregion

        #region constructor / destructor

        /// <summary>
        /// 
        /// </summary>
        /// <param name="iOwner"></param>
        /// <param name="iFriends"></param>
        public FriendsList(User iOwner,
            List<Friend> iFriends)
        {
            cUsrOwner = iOwner;
            cLisFriends = iFriends;
        }

        #endregion

        #region public methods

        /// <summary>
        /// Get a users friends list from storage
        /// </summary>
        /// <param name="iStorage">The storage instance to use</param>
        /// <param name="iUser">The user to get the friends list for</param>
        /// <returns></returns>
        public static FriendsList Get(Storage iStorage,
            User iUser)
        {
            return (iStorage.GetUserFriendsList(iUser));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="iStorage">The storage instance to use</param>
        /// <param name="iFriendUser">The user to add as a friend</param>
        /// <returns></returns>
        public Boolean AddFriend(Storage iStorage,
            User iFriendUser)
        {
            if (GetFriendByUserName(iFriendUser.UserName) == null)
            {
                Friend pFrnFriend = new Friend();
                pFrnFriend.PartitionKey = Owner.RowKey;
                pFrnFriend.RowKey = Guid.NewGuid().ToString();
                pFrnFriend.UserRowKey = iFriendUser.RowKey;
                pFrnFriend.UserName = iFriendUser.UserName;

                return (iStorage.InsertFriendIntoFriendsList(pFrnFriend));
            }
            else
            {
                return (false);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="iStorage">The storage instance to use</param>
        /// <param name="iFriend">The friend to remove from the friends list</param>
        /// <returns></returns>
        public Boolean RemoveFriend(Storage iStorage, 
            Friend iFriend)
        {
            return (iStorage.DeleteFriendFromUsersFriendList(iFriend));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="iUserName"></param>
        /// <returns></returns>
        public Friend GetFriendByUserName(String iUserName)
        {
            foreach(Friend curFriend in Friends)
            {
                if(curFriend.UserName == iUserName)
                {
                    return (curFriend);
                }
            }
            return (null);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="iFormatting"></param>
        /// <returns></returns>
        public String ToJSON(Newtonsoft.Json.Formatting iFormatting)
        {
            JObject pJOtObject = new JObject();
            JArray pJAyFriends = new JArray();
            foreach(Friend curFriend in Friends)
            {
                pJAyFriends.Add(curFriend.ToJObject());
            }
            pJOtObject.Add("Friends", pJAyFriends);
            return (pJOtObject.ToString(iFormatting));
        }

        #endregion

    }

}
