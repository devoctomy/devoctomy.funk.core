using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace devoctomy.funk.core.Cryptography
{

    public static class CryptographyHelpers
    {

        #region public methods

        public static string RandomString(Int32 iLength)
        {
            const String cStrValid = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890";
            StringBuilder pSBRRandom = new StringBuilder();
            using (RNGCryptoServiceProvider rngPtovider = new RNGCryptoServiceProvider())
            {
                byte[] pBytBuffer = new byte[sizeof(uint)];

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
