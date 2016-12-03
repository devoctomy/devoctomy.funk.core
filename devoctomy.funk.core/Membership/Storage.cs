using System;
using System.Threading.Tasks;
using devoctomy.funk.core.Environment;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;


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
        /// URI for the Azure table storage endpoint
        /// </summary>
        public Uri UsersTableURI
        {
            get { return (new Uri(String.Format("{0}/", RootURL, "Users"))); }
        }

        /// <summary>
        /// Users table
        /// </summary>
        public CloudTable UsersTable
        {
            get { return (cCTeUsers); }
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
        }

        #endregion

        #region public methods

        /// <summary>
        /// Get a user from storage by its email address
        /// </summary>
        /// <param name="iEmail">Email address of the user to get</param>
        /// <returns>The required user, if found in storage, otherwise null</returns>
        public async Task<User> GetUser(String iEmail)
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
                TableOperation operation = TableOperation.Retrieve<User>(pStrPartitionKey, iEmail);
                pTRtResult = UsersTable.Execute(operation);
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
        /// Replaces a user in the storage system
        /// </summary>
        /// <param name="iUser">User to replace</param>
        /// <returns>True if the operation was successful</returns>
        public async Task<Boolean> Replace(User iUser)
        {
            TableOperation pTOnReplace = TableOperation.Replace(iUser);
            TableResult pTRtResult = await UsersTable.ExecuteAsync(pTOnReplace);
            return (pTRtResult.HttpStatusCode == 200);
        }

        #endregion

    }

}
