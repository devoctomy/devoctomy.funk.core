using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace devoctomy.funk.core.Cryptography
{

    /// <summary>
    /// Helper functions for cryptographic functions
    /// </summary>
    public static class CryptographyHelpers
    {

        #region public methods

        /// <summary>
        /// Generate a cryptographically strong random string
        /// </summary>
        /// <param name="iLength">The length of the string to generate</param>
        /// <returns>A randomly generated string of lowercase, uppercase letters and digits of the required length</returns>
        public static string RandomString(Int32 iLength)
        {
            const String cStrValid = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890";
            StringBuilder pSBRRandom = new StringBuilder();
            using (RNGCryptoServiceProvider rngPtovider = new RNGCryptoServiceProvider())
            {
                Byte[] pBytBuffer = new Byte[sizeof(uint)];
                while (iLength-- > 0)
                {
                    rngPtovider.GetBytes(pBytBuffer);
                    uint pUIntVal = BitConverter.ToUInt32(pBytBuffer, 0);
                    pSBRRandom.Append(cStrValid[(int)(pUIntVal % (uint)cStrValid.Length)]);
                }
            }
            return (pSBRRandom.ToString());
        }


        #endregion

    }

}
