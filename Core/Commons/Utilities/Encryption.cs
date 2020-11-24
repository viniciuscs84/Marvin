using System.Text;
using System.Security.Cryptography;

namespace Marvin.Commons.Utilities
{
    /// <summary>
    /// Utility Encryption class
    /// </summary>
    public class Encryption
    {
        /// <summary>
        /// Parse strinfg to MD5 hash encrypt string
        /// </summary>
        /// <param name="text">Original string</param>
        /// <returns>Encrypted string</returns>
        public static string GetMD5Hash(string text)
        {
            if (text != null)
            {
                MD5 md5 = new MD5CryptoServiceProvider();
                md5.ComputeHash(Encoding.ASCII.GetBytes(text));
                byte[] result = md5.Hash;

                StringBuilder strBuilder = new StringBuilder();
                for (int i = 0; i < result.Length; i++)
                {
                    strBuilder.Append(result[i].ToString("x2"));
                }
                return strBuilder.ToString();
            }
            return null;
        }
    }
}
