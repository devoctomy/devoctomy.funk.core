using System;
using System.Threading.Tasks;
using devoctomy.funk.core.Environment;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;
using System.IO;

namespace devoctomy.funk.core.Membership
{

    /// <summary>
    /// Storage logic for the membership system
    /// </summary>
    public class Storage
    {

        #region private objects

        String cStrRootURL = String.Empty;
        String cStrConnectionString = String.Empty;
        CloudStorageAccount cCSAAccount;
        CloudTable cCTeUsers;
        CloudTable cCTeProfiles;

        #endregion

        #region public properties

        /// <summary>
        /// Root url of the Azure storage account
        /// </summary>
        public String RootURL
        {
            get { return (cStrRootURL); }
        }
        
        /// <summary>
        /// Connection string for the Azure storage account
        /// </summary>
        public String ConnectionString
        {
            get { return (cStrConnectionString); }
        }

        /// <summary>
        /// Cloud storage account instance, initialised upon construction
        /// </summary>
        public CloudStorageAccount Account
        {
            get { return (cCSAAccount); }
        }

        /// <summary>
        /// URI for the users Azure table storage endpoint
        /// </summary>
        public Uri UsersTableURI
        {
            get { return (new Uri(String.Format("{0}/{1}", RootURL, "users"))); }
        }

        /// <summary>
        /// URI for the profiles Azure table storage endpoint
        /// </summary>
        public Uri ProfilesTableURI
        {
            get { return (new Uri(String.Format("{0}/{1}", RootURL, "profiles"))); }
        }

        /// <summary>
        /// Users table
        /// </summary>
        public CloudTable UsersTable
        {
            get { return (cCTeUsers); }
        }

        /// <summary>
        /// Users table
        /// </summary>
        public CloudTable ProfilesTable
        {
            get { return (cCTeProfiles); }
        }

        #endregion

        #region constructor / destructor

        /// <summary>
        /// Construct an instance of the membership storage system
        /// </summary>
        /// <param name="iRootURL">Root Azure storage URL</param>
        /// <param name="iConnectionStringEnvironVarName">Environment variable name of the connection string to use</param>
        public Storage(String iRootURL, 
            String iConnectionStringEnvironVarName)
        {
            cStrRootURL = iRootURL;
            if (cStrRootURL.EndsWith("/")) cStrRootURL.TrimEnd('/');
            cStrConnectionString = EnvironmentHelpers.GetEnvironmentVariable(iConnectionStringEnvironVarName);
            cCSAAccount = CloudStorageAccount.Parse(cStrConnectionString);
            cCTeUsers = new CloudTable(UsersTableURI, cCSAAccount.Credentials);
            cCTeProfiles = new CloudTable(ProfilesTableURI, cCSAAccount.Credentials);
        }

        #endregion

        #region public methods

        /// <summary>
        /// Get a user from storage by its email address asynchronously
        /// </summary>
        /// <param name="iEmail">Email address of the user to get</param>
        /// <returns>The required user, if found in storage, otherwise null</returns>
        public async Task<User> GetUserAsync(String iEmail)
        {
            String pStrPartitionKey = AzureTableHelpers.GetPartitionKeyFromEmailString(iEmail);
            Boolean pBlnCreatedTable = await UsersTable.CreateIfNotExistsAsync();
            TableResult pTRtResult = new TableResult() { HttpStatusCode = 404 };
            if (pBlnCreatedTable)
            {
                return (null);
            }
            else
            {
                TableOperation pTOnRetrieve = TableOperation.Retrieve<User>(pStrPartitionKey, iEmail);
                pTRtResult = UsersTable.Execute(pTOnRetrieve);
                switch(pTRtResult.HttpStatusCode)
                {
                    case 200:
                        {
                            return ((User)pTRtResult.Result);
                        }
                }
                return (null);
            }
        }

        /// <summary>
        /// Get a user from storage by its email address
        /// </summary>
        /// <param name="iEmail">Email address of the user to get</param>
        /// <returns>The required user, if found in storage, otherwise null</returns>
        public User GetUser(String iEmail)
        {
            String pStrPartitionKey = AzureTableHelpers.GetPartitionKeyFromEmailString(iEmail);
            Boolean pBlnCreatedTable = UsersTable.CreateIfNotExists();
            TableResult pTRtResult = new TableResult() { HttpStatusCode = 404 };
            if (pBlnCreatedTable)
            {
                return (null);
            }
            else
            {
                TableOperation pTOnRetrieve = TableOperation.Retrieve<User>(pStrPartitionKey, iEmail);
                pTRtResult = UsersTable.Execute(pTOnRetrieve);
                switch (pTRtResult.HttpStatusCode)
                {
                    case 200:
                        {
                            return ((User)pTRtResult.Result);
                        }
                }
                return (null);
            }
        }

        /// <summary>
        /// Replaces a user in the storage system
        /// </summary>
        /// <param name="iUser">User to replace</param>
        /// <returns>True if the operation was successful</returns>
        public async Task<Boolean> Replace(User iUser)
        {
            TableOperation pTOnReplace = TableOperation.Replace(iUser);
            TableResult pTRtResult = await UsersTable.ExecuteAsync(pTOnReplace);
            return (pTRtResult.HttpStatusCode == 200 ||
                pTRtResult.HttpStatusCode == 204);
        }

        /// <summary>
        /// Insert an instance of User into storage asynchronously
        /// </summary>
        /// <param name="iUser">The instance of user to insert</param>
        /// <returns>True if the insert was successful</returns>
        public async Task<Boolean> InsertUserAsync(User iUser)
        {
            Boolean pBlnCreatedTable = await UsersTable.CreateIfNotExistsAsync();
            if (pBlnCreatedTable)
            {
                return (false);
            }
            else
            {
                TableOperation pTOnInsert = TableOperation.Insert(iUser);
                TableResult pTRtResult;
                try
                {
                    pTRtResult = await UsersTable.ExecuteAsync(pTOnInsert);
                    switch (pTRtResult.HttpStatusCode)
                    {
                        case 200:
                        case 204:
                            {
                                Boolean pBlnRetVal;
                                pBlnRetVal = CreateDefaultUserProfile(iUser);
                                return (pBlnRetVal);
                            }
                        default:
                            {
                                return (false);
                            }
                    }
                }
                catch //(Exception ex)
                {
                    return (false);
                }
            }
        }

        /// <summary>
        /// Insert an instance of User into storage
        /// </summary>
        /// <param name="iUser">The instance of user to insert</param>
        /// <returns>True if the insert was successful</returns>
        public Boolean InsertUser(User iUser)
        {
            Boolean pBlnCreatedTable = UsersTable.CreateIfNotExists();
            try
            {
                TableOperation pTOnInsert = TableOperation.Insert(iUser);
                TableResult pTRtResult = UsersTable.Execute(pTOnInsert);
                switch (pTRtResult.HttpStatusCode)
                {
                    case 200:
                    case 204:
                        {
                            Boolean pBlnRetVal;
                            pBlnRetVal = CreateDefaultUserProfile(iUser);
                            return (pBlnRetVal);
                        }
                    default:
                        {
                            return (false);
                        }
                }
            }
            catch //(Exception ex)
            {
                return (false);
            }
        }

        public Profile GetUserProfile(User iUser)
        {
            String pStrPartitionKey = AzureTableHelpers.GetPartitionKeyFromEmailString(iUser.RowKey);
            Boolean pBlnCreatedTable = ProfilesTable.CreateIfNotExists();
            if(pBlnCreatedTable)
            {
                String pStrProfile = File.ReadAllText(@"Assets\ProfileDefaults.json");
                Profile pProProfile = new Profile(pStrProfile);
                return (pProProfile);
            }
            else
            {
                TableOperation pTOnRetrieve = TableOperation.Retrieve<DynamicTableEntity>(pStrPartitionKey, iUser.RowKey);
                TableResult pTRtResult = ProfilesTable.Execute(pTOnRetrieve);
                switch (pTRtResult.HttpStatusCode)
                {
                    case 200:
                        {
                            Profile pProProfile = Profile.FromDynamicTableEntity((DynamicTableEntity)pTRtResult.Result);
                            return (pProProfile);
                        }
                }
            }
            return (null);
        }

        public Boolean CreateDefaultUserProfile(User iUser)
        {
            String pStrDefaultProfile = File.ReadAllText(@"Assets\ProfileDefaults.json");
            Profile pProProfile = new Profile(pStrDefaultProfile);
            return (InsertProfile(iUser,
                pProProfile));
        }

        public Boolean InsertProfile(User iUser,
            Profile iProfile)
        {
            String pStrPartitionKey = AzureTableHelpers.GetPartitionKeyFromEmailString(iUser.RowKey);
            Boolean pBlnCreatedTable = ProfilesTable.CreateIfNotExists();
            DynamicTableEntity pDTEProfile = iProfile.ToDynamicTableEntity(pStrPartitionKey, iUser.RowKey);
            TableOperation pTOnInsert = TableOperation.Insert(pDTEProfile);
            TableResult pTRtResult;
            try
            {
                pTRtResult = ProfilesTable.Execute(pTOnInsert);
                switch (pTRtResult.HttpStatusCode)
                {
                    case 200:
                    case 204:
                        {
                            return (true);
                        }
                    default:
                        {
                            return (false);
                        }
                }
            }
            catch (Exception ex)
            {
                return (false);
            }
        }

        public Boolean ReplaceProfile(User iUser,
            Profile iProfile)
        {
            String pStrPartitionKey = AzureTableHelpers.GetPartitionKeyFromEmailString(iUser.RowKey);
            Boolean pBlnCreatedTable = ProfilesTable.CreateIfNotExists();
            DynamicTableEntity pDTEProfile = iProfile.ToDynamicTableEntity(pStrPartitionKey, iUser.RowKey);
            pDTEProfile.ETag = "*";
            TableOperation pTOnReplace = TableOperation.Replace(pDTEProfile);
            TableResult pTRtResult;
            try
            {
                pTRtResult = ProfilesTable.Execute(pTOnReplace);
                switch (pTRtResult.HttpStatusCode)
                {
                    case 200:
                    case 204:
                        {
                            return (true);
                        }
                    default:
                        {
                            return (false);
                        }
                }
            }
            catch (Exception ex)
            {
                return (false);
            }
        }

        #endregion

    }

}
