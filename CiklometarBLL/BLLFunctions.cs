using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace CiklometarBLL
{
    public class BLLFunctions
    {
        private const int SALT_SIZE = 24; // size in bytes
        private const int HASH_SIZE = 24; // size in bytes
        private const int ITERATIONS = 100000; // number of pbkdf2 iterations

        public static string GenerateRandomString(int length)
        {
            var random = new Random();
            var chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            return new string(Enumerable.Repeat(chars, length)
                .Select(s => s[random.Next(s.Length)]).ToArray());
        }
        public static string CreateHash(string input, string salt)
        {
            byte[] input_bytes = Encoding.ASCII.GetBytes(input);
            byte[] salt_bytes = Encoding.ASCII.GetBytes(salt);
            // Generate the hash
            Rfc2898DeriveBytes pbkdf2 = new Rfc2898DeriveBytes(input_bytes, salt_bytes, ITERATIONS);
            return Encoding.Default.GetString(pbkdf2.GetBytes(HASH_SIZE));
        }
        public static string CreateSalt()
        {
            // Generate a salt
            RNGCryptoServiceProvider provider = new RNGCryptoServiceProvider();
            byte[] salt = new byte[SALT_SIZE];
            provider.GetBytes(salt);
            //return as string
            return Encoding.Default.GetString(salt);
        }
    }
}
