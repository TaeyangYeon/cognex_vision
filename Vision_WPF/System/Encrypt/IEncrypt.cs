using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Encrypt
{
    public interface IEncrypt
    {
        string getHashString(string value);
        bool isVerifyString(string value, string hashString);
    }
}
