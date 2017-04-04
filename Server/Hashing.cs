using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security.Cryptography;

namespace Ember
{
    class Hashing
    {
        static readonly char[] AvailableCharacters = {
            'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'I', 'J', 'K', 'L', 'M', 
            'N', 'O', 'P', 'Q', 'R', 'S', 'T', 'U', 'V', 'W', 'X', 'Y', 'Z', 
            'a', 'b', 'c', 'd', 'e', 'f', 'g', 'h', 'i', 'j', 'k', 'l', 'm', 
            'n', 'o', 'p', 'q', 'r', 's', 't', 'u', 'v', 'w', 'x', 'y', 'z', 
            '0', '1', '2', '3', '4', '5', '6', '7', '8', '9', '-', '_', '`',
            '~', '!', '@', '#', '$', '%', '^', '&', '*', '(', ')', '_', '+',
            '-', '=', '{', '}', '|', '[', ']', '\\', ':', '\"', ',', '.', '/', '?'
        };

        public static string GenerateRandomKey()
        {
            char[] identifier = new char[8];
            byte[] randomData = new byte[8];

            using (RNGCryptoServiceProvider rng = new RNGCryptoServiceProvider())
            {
                rng.GetBytes(randomData);
            }

            for (int idx = 0; idx < identifier.Length; idx++)
            {
                int pos = randomData[idx] % AvailableCharacters.Length;
                identifier[idx] = AvailableCharacters[pos];
            }

            return new string(identifier);
        }

        public static string GenerateMD5(string data)
        {
            MD5CryptoServiceProvider x = new MD5CryptoServiceProvider();
            
            byte[] bytes = System.Text.Encoding.UTF8.GetBytes(data);
            bytes = x.ComputeHash(bytes);
            
            StringBuilder builder = new StringBuilder();
            
            foreach (byte b in bytes)
            {
                builder.Append(b.ToString("x2").ToLower());
            }

            return builder.ToString();
        }

        public static string EncryptPassword(string pw)
        {
            string pass = pw.Substring(16, 16) + pw.Substring(0, 16);
            return pass;
        }

        public static string HashPassword(string pw, string rndk)
        {
            string pass = EncryptPassword(GenerateMD5(pw)).ToUpper();
            pass += rndk;
            pass += "a1ebe00441f5aecb185d0ec178ca2305Y(02.>'H}t\":E1_root";
            return EncryptPassword(GenerateMD5(pass));
        }
    }
}
