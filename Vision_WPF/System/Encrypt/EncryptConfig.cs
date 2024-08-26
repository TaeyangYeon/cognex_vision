using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Encrypt
{
    public class EncryptConfig
    {
        public IEncrypt iEncryptUseSha()
        {
            return new Encrypt();
        }
    }
}
