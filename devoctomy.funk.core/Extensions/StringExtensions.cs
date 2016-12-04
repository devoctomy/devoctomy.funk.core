using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace devoctomy.funk.core.Extensions
{

    public static class StringExtensions
    {

        #region public methods

        public static String HexEncode(this String iString)
        {
            Byte[] pBytBuffer = System.Text.Encoding.UTF8.GetBytes(iString);
            return (BitConverter.ToString(pBytBuffer).Replace("-", String.Empty).ToLower());
        }

        public static String HexDecode(this String iHexString)
        {
            MemoryStream pMSmOutput = new MemoryStream();
            using (pMSmOutput)
            {
                String pStrOutput = String.Empty;
                String pStrInput = iHexString;
                String pStrCurrentHex = String.Empty;
                while (!String.IsNullOrEmpty(pStrInput))
                {
                    pStrCurrentHex = pStrInput.Substring(0, 2);
                    pStrInput = pStrInput.Substring(2);
                    List<Byte> pLisByte = new List<Byte>();
                    pLisByte.Add(Byte.Parse(pStrCurrentHex.ToUpper(), System.Globalization.NumberStyles.HexNumber));
                    pMSmOutput.Write(pLisByte.ToArray(), 0, pLisByte.Count);
                }
                return (Encoding.UTF8.GetString(pMSmOutput.ToArray()));
            }
        }

        #endregion

    }

}
