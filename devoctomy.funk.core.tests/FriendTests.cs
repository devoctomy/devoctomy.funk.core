using devoctomy.funk.core.Cryptography;
using devoctomy.funk.core.Environment;
using devoctomy.funk.core.Membership;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;

namespace devoctomy.funk.core.tests
{

    [TestClass]
    public class FriendTests
    {

        #region private constants

        private const Int32 NOOFUSERS = 10;
        private const Int32 NOOFRIENDSPERUSER = 5;

        #endregion

        #region private classes

        private class CreateUserArgs
        {
            public String Email;
            public String UserName;
            public String ActivationCode;
            public List<CreateUserArgs> Friends;

            public Boolean HasFriend(String iUserName)
            {
                foreach(CreateUserArgs curFriend in Friends)
                {
                    if(curFriend.UserName == iUserName)
                    {
                        return (true);
                    }
                }
                return (false);
            }
        }

        #endregion

        #region private objects

        private Random cRndRandom;
        private String cStrTableStorageRootURL = String.Empty;
        private String cStrConnectionString = String.Empty;
        private List<CreateUserArgs> cLisUsers;

        #endregion

        #region private methods

        private void LoadAzureCredentials()
        {
            cRndRandom = new Random(System.Environment.TickCount);
            String pStrCreds = File.ReadAllText("../../../../azure/creds.json");
            JObject pJOtCreds = JObject.Parse(pStrCreds);
            cStrTableStorageRootURL = pJOtCreds["TableStorageRootURL"].Value<String>();
            cStrConnectionString = pJOtCreds["ConnectionString"].Value<String>();
            EnvironmentHelpers.SetEnvironmentVariable("HOME", @"C:\Temp", EnvironmentVariableTarget.Process);
            EnvironmentHelpers.SetEnvironmentVariable("TableStorageRootURL", cStrTableStorageRootURL, EnvironmentVariableTarget.Process);
            EnvironmentHelpers.SetEnvironmentVariable("AzureWebJobsStorage", cStrConnectionString, EnvironmentVariableTarget.Process);

            cLisUsers = new List<CreateUserArgs>();
            for(Int32 curUserIndex = 1; curUserIndex <= NOOFUSERS; curUserIndex++)
            {
                String pStrEmail = String.Format("{0}@{1}.com",
                    CryptographyHelpers.RandomString(12),
                    CryptographyHelpers.RandomString(12));
                String pStrUserName = CryptographyHelpers.RandomString(12);
                String pStrActivationCode = CryptographyHelpers.RandomString(6);
                cLisUsers.Add(new CreateUserArgs()
                {
                    Email = pStrEmail,
                    UserName = pStrUserName,
                    ActivationCode = pStrActivationCode
                });
            }

            for (Int32 curUserIndex = 1; curUserIndex <= NOOFUSERS; curUserIndex++)
            {
                CreateUserArgs pCUAUser = cLisUsers[curUserIndex-1];
                pCUAUser.Friends = new List<CreateUserArgs>();
                List<Int32> pLisIndexes = new List<Int32>();
                for (Int32 curFriendIndex = 1; curFriendIndex <= NOOFRIENDSPERUSER; curFriendIndex++)
                {
                    List<CreateUserArgs> pLisFriends = new List<CreateUserArgs>();

                    //Get a random user that isn't us
                    Int32 pIntUserIndex = cRndRandom.Next(0, NOOFUSERS);
                    while (pIntUserIndex == curFriendIndex - 1 ||
                        pLisIndexes.Contains(pIntUserIndex))
                    {
                        pIntUserIndex = cRndRandom.Next(0, NOOFUSERS);
                    }

                    pLisIndexes.Add(pIntUserIndex);
                    pCUAUser.Friends.Add(cLisUsers[pIntUserIndex]);
                }
            }
        }

        #endregion

        #region private methods

        private ClaimsPrincipal GetTestUserPrincipal(String iEmail)
        {
            GenericIdentity pGIyTestUser = new GenericIdentity("Test User");
            pGIyTestUser.AddClaim(new Claim(ClaimTypes.Email, iEmail));
            ClaimsPrincipal pCPlTestUser = new ClaimsPrincipal(pGIyTestUser);
            return (pCPlTestUser);
        }

        #endregion

        #region public methods

        [TestInitialize()]
        public void Init()
        {
            LoadAzureCredentials();
        }

        [TestMethod]
        public async Task AllFriendsTestsSequenced()
        {
            await CreateUsers();
            await ActivateUsers();
            await CreateFriendsListsAsync();
            await RemoveFriendsFromFriendsListsOneByOneAsync();
            //DeleteUser?
        }

        //[TestMethod]
        public async Task CreateUsers()
        {
            Storage pStoStorage = new Storage("TableStorageRootURL",
                "AzureWebJobsStorage",
                "Test");
            foreach(CreateUserArgs curUser in cLisUsers)
            {
                User pUsrUser = new User(GetTestUserPrincipal(curUser.Email), 6);
                pUsrUser.ActivationCode = curUser.ActivationCode;
                Assert.IsTrue(await pUsrUser.InsertAsync(pStoStorage));
            }
        }

        //[TestMethod]
        public async Task ActivateUsers()
        {
            Storage pStoStorage = new Storage("TableStorageRootURL",
                "AzureWebJobsStorage",
                "Test");
            foreach (CreateUserArgs curUser in cLisUsers)
            {
                User pUsrUser = await pStoStorage.GetUserAsync(GetTestUserPrincipal(curUser.Email));
                if (pUsrUser != null)
                {
                    Boolean pBlnUserNameTaken = false;
                    Assert.IsTrue(pUsrUser.Activate(pStoStorage,
                        curUser.ActivationCode,
                        curUser.UserName,
                        out pBlnUserNameTaken));
                }
                else
                {
                    Assert.Fail("Failed to get user '{0}' from storage.", curUser.Email);
                }
            }
        }

        public async Task CreateFriendsListsAsync()
        {
            Storage pStoStorage = new Storage("TableStorageRootURL",
                "AzureWebJobsStorage",
                "Test");
            foreach (CreateUserArgs curUser in cLisUsers)
            {
                FriendsList pFLtFriends = null;
                User pUsrUser = await pStoStorage.GetUserAsync(GetTestUserPrincipal(curUser.Email));

                foreach (CreateUserArgs curFriend in curUser.Friends)
                {
                    System.Diagnostics.Debug.WriteLine(String.Format("Adding friend '{0}' to user '{1}'.", curFriend.UserName, curUser.UserName));

                    User pUsrFriend = await pStoStorage.GetUserAsync(GetTestUserPrincipal(curFriend.Email));

                    pFLtFriends = pUsrUser.GetFriends(pStoStorage);
                    Assert.IsTrue(pFLtFriends.AddFriend(pStoStorage, pUsrFriend));
                }

                pFLtFriends = pUsrUser.GetFriends(pStoStorage);
                foreach(Friend curFriend in pFLtFriends.Friends)
                {
                    Assert.IsTrue(curUser.HasFriend(curFriend.UserName));
                }
            }
        }

        public async Task RemoveFriendsFromFriendsListsOneByOneAsync()
        {
            Storage pStoStorage = new Storage("TableStorageRootURL",
                "AzureWebJobsStorage",
                "Test");
            foreach (CreateUserArgs curUser in cLisUsers)
            {
                FriendsList pFLtFriends = null;
                User pUsrUser = await pStoStorage.GetUserAsync(GetTestUserPrincipal(curUser.Email));

                foreach (CreateUserArgs curFriend in curUser.Friends)
                {
                    System.Diagnostics.Debug.WriteLine(String.Format("Removing friend '{0}' from user '{1}'.", curFriend.UserName, curUser.UserName));

                    User pUsrFriend = await pStoStorage.GetUserAsync(GetTestUserPrincipal(curFriend.Email));

                    pFLtFriends = pUsrUser.GetFriends(pStoStorage);
                    Int32 pIntPreRemovalCount = pFLtFriends.Friends.Count;
                    Friend pFrdFriend = pFLtFriends.GetFriendByUserName(curFriend.UserName);
                    Assert.IsTrue(pFLtFriends.RemoveFriend(pStoStorage, pFrdFriend));
                    FriendsList pFLtFriendsPostRemoval = pUsrUser.GetFriends(pStoStorage);
                    pFrdFriend = pFLtFriendsPostRemoval.GetFriendByUserName(curFriend.UserName);
                    Assert.IsNull(pFrdFriend);
                }
            }
        }


        #endregion

    }

}
