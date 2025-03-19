using System;
using System.Linq;
using System.Security.Cryptography;

namespace Nemeio.Infrastructure
{
    public class DbPasswordProvider
    {
        // Create a string of characters, numbers, special characters that allowed in the password  
        const string validChars = "ABCDEFGHJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789!@#$%^&*?_-";

        public string CreatePassword(int length = 15)
        {
            var randomGenerator = RandomNumberGenerator.Create(); // Compliant for security-sensitive use cases
            byte[] data = new byte[length];
            char[] chars = new char[length];
            randomGenerator.GetBytes(data);
            for (int i = 0; i < length; i++)
            {
                var offset = data[i];
                var rest = offset % validChars.Length;
                chars[i] = validChars[rest];
            }

            return new string(chars);
        }

        /// <summary>
        /// Cleans the string Password by ensuring it only contains chars in the validChars string
        /// Any char not allowed is simply removed before returning the new string
        /// </summary>
        /// <param name="password">The password to clean</param>
        /// <returns>the clean string containing only valid chars</returns>
        internal static string CleanPassword(string password)
        {
            return new string(password.Where(c => validChars.Contains(c)).ToArray());
        }
    }
}
