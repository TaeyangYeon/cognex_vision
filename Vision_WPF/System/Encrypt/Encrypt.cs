using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security.Cryptography;

namespace Encrypt
{
    public class Encrypt : IEncrypt
    {
        public string getHashString(string value)
        {
            using (SHA256 sha256 = SHA256.Create())
            {
                byte[] hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(value));
                return Convert.ToBase64String(hashedBytes);
            }
        }

        public bool isVerifyString(string value, string hashString)
        {
            return getHashString(value).Equals(hashString);
        }
    }
}
