using devoctomy.funk.core.Cryptography;
using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Threading.Tasks;

namespace devoctomy.funk.core.Membership
{
    public class User : TableEntity
    {

        #region public properties

        /// <summary>
        /// Username of the user
        /// </summary>
        public String UserName { get; set; }

        /// <summary>
        /// Activation code used to enable signing in of the account, after creation
        /// </summary>
        public String ActivationCode { get; set; }

        /// <summary>
        /// Is the user activated?
        /// </summary>
        public Boolean Activated { get; set; }

        /// <summary>
        /// Date / Time the last OTP was requested
        /// </summary>
        public DateTime OTPRequestedAt { get; set; }

        /// <summary>
        /// OTP / One Time Password
        /// </summary>
        public String OTP { get; set; }

        #endregion

        #region public methods

        public async Task<Boolean> Activate(Storage iStorage, String iActivationCde)
        {
            if(ActivationCode == iActivationCde)
            {
                Activated = true;
                return(await iStorage.Replace(this));
            }
            else
            {
                return (false);
            }
        }

        public void RandomiseActivationCode(Int32 iLength)
        {
            ActivationCode = CryptographyHelpers.RandomString(iLength);
        }

        public async Task<Boolean> RandomiseOTP(Storage iStorage, Int32 iLength)
        {
            OTP = CryptographyHelpers.RandomString(iLength);
            OTPRequestedAt = DateTime.UtcNow;
            return (await iStorage.Replace(this));
        }

        public async Task<UserLoginResult> Login(Storage iStorage, String iOTP)
        {
            UserLoginResult pULRResult = new UserLoginResult();
            if (OTP == iOTP)
            {
                OTP = String.Empty;
                if (await iStorage.Replace(this))
                {
                    pULRResult.Success = true;
                    pULRResult.SessionToken = new SessionToken();
                }
            }
            return (pULRResult);
        }

        #endregion

    }

}
