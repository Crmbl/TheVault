using System;
using System.Security.Cryptography;
using System.Text;

namespace TheVault.Utils
{
    public class MD5Util
    {
        public static string GetMd5Hash(MD5 md5Hash, string input)
        {
            if (string.IsNullOrWhiteSpace(input))
                return "";

            byte[] data = md5Hash.ComputeHash(Encoding.UTF8.GetBytes(input));
            StringBuilder sBuilder = new StringBuilder();

            for (int i = 0; i < data.Length; i++)
                sBuilder.Append(data[i].ToString("x2"));

            return sBuilder.ToString();
        }
    }
}
