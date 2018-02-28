using System;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security;

namespace AutomaticReservation_UI.Common
{
    public static class SecureStringEx
    {
        /// <summary>
        /// SecureStringの比較
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static bool Equals(this SecureString a, SecureString b)
        {
            if (a == null && b == null)
            {
                return true;
            }
            if (a == null || b == null)
            {
                return false;
            }
            if(a.Length != b.Length)
            {
                return false;
            }

            var aPtr = Marshal.SecureStringToBSTR(a);
            var bPtr = Marshal.SecureStringToBSTR(b);

            try
            {
                return Enumerable.Range(0, a.Length).All(i => Marshal.ReadInt16(aPtr, i) == Marshal.ReadInt16(bPtr, i));
            }
            finally
            {
                Marshal.ZeroFreeBSTR(aPtr);
                Marshal.ZeroFreeBSTR(bPtr);
            }
        }

        /// <summary>
        /// BSTRの文字列をSecureStringにコピー
        /// </summary>
        /// <param name="self"></param>
        /// <param name="bstr"></param>
        /// <param name="count"></param>
        public static void CopyFromBSTR(this SecureString self, IntPtr bstr, int count)
        {
            self.Clear();
            var chars = Enumerable.Range(0, count).Select(i => (char)Marshal.ReadInt16(bstr, i * 2));
            foreach(char c in chars)
            {
                self.AppendChar(c);
            }
        }
    }
}
