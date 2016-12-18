using System;
using System.Threading.Tasks;
using devoctomy.funk.core.Environment;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;
using System.IO;
using System.Collections.Generic;
using Microsoft.WindowsAzure.Storage.Queue;

namespace devoctomy.funk.core.Membership
{

    /// <summary>
    /// Storage logic for the membership system
    /// </summary>
    public class Storage
    {

        #region private objects

        String cStrTableStorageRootURL = String.Empty;
        String cStrConnectionString = String.Empty;
        String cStrFunctionName = String.Empty;
        CloudStorageAccount cCSAAccount;
        CloudTable cCTeUsers;
        CloudTable cCTeProfiles;
        CloudTable cCTeFunctionHits;

        #endregion

        #region public properties

        /// <summary>
        /// Table storage root url of the Azure storage account
        /// </summary>
        public String TaleStorageRootURL
        {
            get { return (cStrTableStorageRootURL); }
        }

        /// <summary>
        /// Connection string for the Azure storage account
        /// </summary>
        public String ConnectionString
        {
            get { return (cStrConnectionString); }
        }

        /// <summary>
        /// Name of the current function
        /// </summary>
        public String FunctionName
        {
            get { return (cStrFunctionName); }
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
            get { return (new Uri(String.Format("{0}/{1}", TaleStorageRootURL, "users"))); }
        }

        /// <summary>
        /// URI for the profiles Azure table storage endpoint
        /// </summary>
        public Uri ProfilesTableURI
        {
            get { return (new Uri(String.Format("{0}/{1}", TaleStorageRootURL, "profiles"))); }
        }

        /// <summary>
        /// URI for the function hits Azure table storage endpoint
        /// </summary>
        public Uri FunctionHitsTableURI
        {
            get { return (new Uri(String.Format("{0}/{1}", TaleStorageRootURL, "functionhits"))); }
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

        /// <summary>
        /// Function hits table
        /// </summary>
        public CloudTable FunctionHitsTable
        {
            get { return (cCTeFunctionHits); }
        }

        /// <summary>
        /// The assets path
        /// </summary>
        public String FunctionAssetsPath
        {
            get
            {
                String pStrHome = EnvironmentHelpers.GetEnvironmentVariable("HOME");
                if (!pStrHome.EndsWith("\\")) pStrHome += "\\";
                String pStrFuncPath = pStrHome + $"site\\wwwroot\\{FunctionName}\\bin\\Assets\\";
                return (pStrFuncPath);
            }
        }

        #endregion

        #region constructor / destructor

        /// <summary>
        /// Construct an instance of the membership storage system
        /// </summary>
        /// <param name="iTableStorageRootURLEnvironVarName">Root Azure table storage URL</param>
        /// <param name="iConnectionStringEnvironVarName">Environment variable name of the connection string to use</param>
        /// <param name="iFunctionName">The name of the function</param>
        public Storage(String iTableStorageRootURLEnvironVarName,
            String iConnectionStringEnvironVarName,
            String iFunctionName)
        {
            cStrTableStorageRootURL = EnvironmentHelpers.GetEnvironmentVariable(iTableStorageRootURLEnvironVarName);
            if (cStrTableStorageRootURL.EndsWith("/")) cStrTableStorageRootURL.TrimEnd('/');
            cStrConnectionString = EnvironmentHelpers.GetEnvironmentVariable(iConnectionStringEnvironVarName);
            cStrFunctionName = iFunctionName;
            cCSAAccount = CloudStorageAccount.Parse(cStrConnectionString);
            cCTeUsers = new CloudTable(UsersTableURI, cCSAAccount.Credentials);
            cCTeProfiles = new CloudTable(ProfilesTableURI, cCSAAccount.Credentials);
            cCTeFunctionHits = new CloudTable(FunctionHitsTableURI, cCSAAccount.Credentials);
        }

        #endregion

        #region public methods

        /// <summary>
        /// Get all hits for a specific function, from a specific source
        /// </summary>
        /// <param name="iFunctionName">The name of the function to get the registered hits for</param>
        /// <param name="iSource">The source of the request, for example ipaddress</param>
        /// <param name="iEarliest">The earliest registered datetime to get</param>
        /// <returns></returns>
        public List<FunctionHit> GetHits(String iFunctionName,
            String iSource,
            DateTime iEarliest)
        {
            String pStrPartitionFilter = TableQuery.GenerateFilterCondition(
               "PartitionKey",
               QueryComparisons.Equal,
               String.Format("{0}_{1}", iFunctionName, iSource));
            String pStrTimestampFilter = TableQuery.GenerateFilterConditionForDate(
               "Timestamp",
               QueryComparisons.GreaterThanOrEqual,
               iEarliest);
            String pStrFilter = TableQuery.CombineFilters(pStrPartitionFilter, 
                TableOperators.And,
                pStrTimestampFilter);

            TableQuery<FunctionHit> pTQyQuery = new TableQuery<FunctionHit>().Where(pStrFilter);
            FunctionHitsTable.CreateIfNotExists();
            IEnumerable<FunctionHit> pIEeHits = FunctionHitsTable.ExecuteQuery<FunctionHit>(pTQyQuery);
            List<FunctionHit> pLisHits = new List<FunctionHit>(pIEeHits);

            return (pLisHits);
        }

        /// <summary>
        /// Register a hit for a function
        /// </summary>
        /// <param name="iFunctionName">The name of the function to register the hit for</param>
        /// <param name="iSource">The source of the request, for example ipaddress</param>
        /// <returns></returns>
        public Boolean RegisterHit(String iFunctionName,
            String iSource)
        {
            FunctionHit pFHtHit = FunctionHit.Create(iFunctionName,
                iSource);
            FunctionHitsTable.CreateIfNotExists();
            TableOperation pTOnInsert = TableOperation.Insert(pFHtHit);
            TableResult pTRtResult;
            try
            {
                pTRtResult = FunctionHitsTable.Execute(pTOnInsert);
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
            catch
            {
#if (DEBUG)
                throw;
#else
                return (false);
#endif
            }
        }

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
        /// Replaces a user in the storage system asynchronously
        /// </summary>
        /// <param name="iUser">User to replace</param>
        /// <returns>True if the operation was successful</returns>
        public async Task<Boolean> ReplaceAsync(User iUser)
        {
            TableOperation pTOnReplace = TableOperation.Replace(iUser);
            TableResult pTRtResult = await UsersTable.ExecuteAsync(pTOnReplace);
            return (pTRtResult.HttpStatusCode == 200 ||
                pTRtResult.HttpStatusCode == 204);
        }

        /// <summary>
        /// Replaces a user in the storage system
        /// </summary>
        /// <param name="iUser">User to replace</param>
        /// <returns>True if the operation was successful</returns>
        public Boolean Replace(User iUser)
        {
            TableOperation pTOnReplace = TableOperation.Replace(iUser);
            TableResult pTRtResult = UsersTable.Execute(pTOnReplace);
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
                catch
                {
#if (DEBUG)
                    throw;
#else
                    return (false);
#endif
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
            catch
            {
#if (DEBUG)
                throw;
#else
                return (false);
#endif
            }
        }

        /// <summary>
        /// Get a users profile
        /// </summary>
        /// <param name="iUser">The user of the profile to get</param>
        /// <returns>The users profile if found, otherwise null</returns>
        public Profile GetUserProfile(User iUser)
        {
            String pStrPartitionKey = AzureTableHelpers.GetPartitionKeyFromEmailString(iUser.RowKey);
            Boolean pBlnCreatedTable = ProfilesTable.CreateIfNotExists();
            if(pBlnCreatedTable)
            {
                String pStrProfile = File.ReadAllText(FunctionAssetsPath + "ProfileDefaults.json");
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

        /// <summary>
        /// Create the default profile for a user
        /// </summary>
        /// <param name="iUser">The user to create the default profile for</param>
        /// <returns>True if successsful</returns>
        public Boolean CreateDefaultUserProfile(User iUser)
        {
            String pStrDefaultProfile = File.ReadAllText(FunctionAssetsPath + "ProfileDefaults.json");
            Profile pProProfile = new Profile(pStrDefaultProfile);
            return (InsertProfile(iUser,
                pProProfile));
        }

        /// <summary>
        /// Insert a profile into storage
        /// </summary>
        /// <param name="iUser">The user to associate the profile with</param>
        /// <param name="iProfile">The profile instance to insert</param>
        /// <returns>True if successful</returns>
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
            catch
            {
#if (DEBUG)
                throw;
#else
                return (false);
#endif
            }
        }

        /// <summary>
        /// Replaces a profile in storage with the provided profile
        /// </summary>
        /// <param name="iUser">The user to accociate the profile with</param>
        /// <param name="iProfile">The profile instance to replace the existing one with</param>
        /// <returns>True if successful</returns>
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
            catch
            {
#if (DEBUG)
                throw;
#else
                return (false);
#endif
            }
        }

        /// <summary>
        /// Queue a message into the desired Azure Queue
        /// </summary>
        /// <param name="iMessage">The message to queue, typically this should be JSON</param>
        /// <param name="iQueueName">The name of the queue to queue the message in</param>
        public void QueueMessage(String iMessage, 
            String iQueueName)
        {
            CloudQueueClient queueClient = cCSAAccount.CreateCloudQueueClient();
            CloudQueue pCQeQueue = queueClient.GetQueueReference(iQueueName);
            pCQeQueue.CreateIfNotExists();
            CloudQueueMessage pCQMMessage = new CloudQueueMessage(iMessage);
            pCQeQueue.AddMessage(pCQMMessage);
        }

        #endregion

    }

}
