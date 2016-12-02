using System;
using System.Threading.Tasks;
using devoctomy.funk.core.Environment;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;


namespace devoctomy.funk.core.Membership
{

    public class Storage
    {

        #region private objects

        String cStrRootURL = String.Empty;
        String cStrConnectionString = String.Empty;
        CloudStorageAccount cCSAAccount;
        CloudTable cCTeUsers;

        #endregion

        #region public properties

        public String RootURL
        {
            get { return (cStrRootURL); }
        }
        
        public String ConnectionString
        {
            get { return (cStrConnectionString); }
        }

        public CloudStorageAccount Account
        {
            get { return (cCSAAccount); }
        }

        public Uri UsersTableURI
        {
            get { return (new Uri(String.Format("{0}/", RootURL, "Users"))); }
        }

        public CloudTable UsersTable
        {
            get { return (cCTeUsers); }
        }

        #endregion

        #region constructor / destructor

        public Storage(String iRootURL, String iEnvironVarName)
        {
            cStrRootURL = iRootURL;
            if (cStrRootURL.EndsWith("/")) cStrRootURL.TrimEnd('/');
            cStrConnectionString = EnvironmentHelpers.GetEnvironmentVariable(iEnvironVarName);
            cCSAAccount = CloudStorageAccount.Parse(cStrConnectionString);
            cCTeUsers = new CloudTable(UsersTableURI, cCSAAccount.Credentials);
        }

        #endregion

        #region public methods

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

        public async Task<Boolean> Replace(User iUser)
        {
            TableOperation pTOnReplace = TableOperation.Replace(iUser);
            TableResult pTRtResult = await UsersTable.ExecuteAsync(pTOnReplace);
            return (pTRtResult.HttpStatusCode == 200);
        }

        #endregion

    }

}
