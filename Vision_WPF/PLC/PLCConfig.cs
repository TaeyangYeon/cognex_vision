using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PLC
{
    class PLCConfig
    {
        private string ipAddress = "192.168.65.246";

        /**
         * SocketCummunication 객체 반환
         */
        public ISocketCommunication iSocketCommunication(int port)
        {
            return new SocketCummunication(ipAddress, port);
        }

        /**
         * SignalLoop 객체 반환
         */
        public ISignalLoop iSignalLoop(int port)
        {
            return new SignalLoop(iSocketCommunication(port), new OutAlignVariable());
        }
    }
}
