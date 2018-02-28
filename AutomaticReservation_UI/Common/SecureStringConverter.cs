using System.Runtime.InteropServices;
using System.Security;

namespace AutomaticReservation_UI.Common
{
    public static class SecureStringConverter
    {
        /// <summary>
        /// 平文を暗号化文字列に変換する
        /// </summary>
        /// <param name="strPlain">文字列</param>
        /// <returns>暗号化された文字列</returns>
        public static SecureString PlainToSecure(string strPlain)
        {
            var strSecure = new SecureString();

            foreach(char c in strPlain)
            {
                strSecure.AppendChar(c);
            }

            return strSecure;
        }

        /// <summary>
        /// 暗号化文字列を平文に変換する
        /// </summary>
        /// <param name="strSecure">暗号化された文字列</param>
        /// <returns>平文</returns>
        public static string SecureToPlain(SecureString strSecure)
        {
            return Marshal.PtrToStringUni(Marshal.SecureStringToGlobalAllocUnicode(strSecure));
        }
    }
}
