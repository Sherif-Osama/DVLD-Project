using System;
using System.Security.Cryptography;
using System.Text;
namespace BusinessLayer
{
    public static class HashHelper
    {
        public static string ComputeHashing(string Text)
        {
            using (SHA256 sha256 = SHA256.Create())
            {
                byte[] HashBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(Text));

                return BitConverter.ToString(HashBytes).Replace("-", string.Empty).ToLower();
            }
        }
    }
}