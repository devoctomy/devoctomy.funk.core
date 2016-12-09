﻿using devoctomy.funk.core.Cryptography;
using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Threading.Tasks;

namespace devoctomy.funk.core.Membership
{

    /// <summary>
    /// User of the membership system
    /// </summary>
    public class User : TableEntity
    {

        #region public properties

        /// <summary>
        /// Date / Time this user was created
        /// </summary>
        public DateTime CreatedAt { get; set; }

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

        /// <summary>
        /// Number of times this user has attempted to login incorrectly, in a row
        /// </summary>
        public Int32 IncorrectLoginAttempts { get; set; }

        /// <summary>
        /// This account is locked, either from incorrect login attemps or administrative action
        /// </summary>
        public Boolean Locked { get; set; }

        #endregion

        #region constructor / destructor

        /// <summary>
        /// Constructions an instance of user
        /// </summary>
        public User()
        { }

        /// <summary>
        /// Constructions an instance of user with the provided parameters
        /// </summary>
        /// <param name="iEmail">Email address of the user to create, this must be unique</param>
        /// <param name="iUserName">Username of the user to create</param>
        public User(String iEmail, 
            String iUserName,
            Int32 iActivationCodeLength)
        {
            CreatedAt = DateTime.UtcNow;
            RowKey = iEmail;
            PartitionKey = AzureTableHelpers.GetPartitionKeyFromEmailString(iEmail);
            UserName = iUserName;
            RandomiseActivationCode(iActivationCodeLength);
            Activated = false;
            OTPRequestedAt = new DateTime(1982,4,3);
            OTP = string.Empty;
        }

        #endregion

        #region public methods

        /// <summary>
        /// Attempt to activate the user, the activation code must match the one in storage
        /// </summary>
        /// <param name="iStorage">The storage instance to use</param>
        /// <param name="iActivationCde">The activation code provided during the activation process by the user</param>
        /// <returns>True if the activation code matched and storage was updated</returns>
        public async Task<Boolean> Activate(Storage iStorage, 
            String iActivationCde)
        {
            if(!Activated && ActivationCode == iActivationCde)
            {
                Activated = true;
                ActivationCode = String.Empty;
                return (await iStorage.Replace(this));
            }
            else
            {
                return (false);
            }
        }

        /// <summary>
        /// Randomises the activationm code, called on construction
        /// </summary>
        /// <param name="iLength">The length in characters of the newly generated activation code</param>
        public void RandomiseActivationCode(Int32 iLength)
        {
            ActivationCode = CryptographyHelpers.RandomString(iLength);
        }

        /// <summary>
        /// Randomises the one time password, called on login request
        /// </summary>
        /// <param name="iStorage">The storage instance to use</param>
        /// <param name="iLength">The length in characters of the newly generated one time password</param>
        /// <returns>True if the OTP was randomised and storage was updated</returns>
        public async Task<Boolean> RandomiseOTP(Storage iStorage, 
            Int32 iLength)
        {
            OTP = CryptographyHelpers.RandomString(iLength);
            OTPRequestedAt = DateTime.UtcNow;
            return (await iStorage.Replace(this));
        }

        /// <summary>
        /// Attempt to login the user, the OTP provided must match the one in storage.
        /// </summary>
        /// <param name="iStorage">The storage instance to use</param>
        /// <param name="iOTP">The one time password provided during the login process by the user</param>
        /// <param name="iSessionLifeSpan">The lifespan of the session token to generate upon successful login</param>
        /// <returns>Thue if the OTP matched and storage was updated</returns>
        public async Task<UserLoginResult> Login(Storage iStorage, 
            String iOTP,
            TimeSpan iSessionLifeSpan)
        {
            UserLoginResult pULRResult = new UserLoginResult();
            if (!Locked && OTP == iOTP)
            {
                OTP = String.Empty;
                IncorrectLoginAttempts = 0;
                if (await iStorage.Replace(this))
                {
                    pULRResult.Success = true;
                    pULRResult.SessionToken = new SessionToken(RowKey,
                        iSessionLifeSpan);
                }
            }
            else
            {
                if(Locked)
                {
                    pULRResult.AccountPreviouslyLocked = true;
                }
                else
                {
                    IncorrectLoginAttempts += 1;
                    if (IncorrectLoginAttempts >= 3)
                    {
                        Locked = true;
                    }
                    await iStorage.Replace(this);
                }
            }
            return (pULRResult);
        }

        /// <summary>
        /// Insert this user instance into member storage asynchronously
        /// </summary>
        /// <param name="iStorage">The storage instance to use</param>
        /// <returns>True on success</returns>
        public async Task<Boolean> InsertAsync(Storage iStorage)
        {
            return (await iStorage.InsertUserAsync(this));
        }

        /// <summary>
        /// Insert this user instance into member storage
        /// </summary>
        /// <param name="iStorage">The storage instance to use</param>
        /// <returns>True on success</returns>
        public Boolean Insert(Storage iStorage)
        {
            return (iStorage.InsertUser(this));
        }

        /// <summary>
        /// Get this users profile
        /// </summary>
        /// <param name="iStorage">The storage instance to use</param>
        /// <returns>True on success</returns>
        public Profile GetProfile(Storage iStorage)
        {
            return (iStorage.GetUserProfile(this));
        }

        /// <summary>
        /// Update the profile
        /// </summary>
        /// <param name="iStorage">The storage instance to use</param>
        /// <param name="iProfile">The profile to associate with the user</param>
        /// <returns></returns>
        public Boolean ReplaceProfile(Storage iStorage,
            Profile iProfile)
        {
            return(iProfile.Replace(iStorage,
                this));
        }

        #endregion

    }

}
