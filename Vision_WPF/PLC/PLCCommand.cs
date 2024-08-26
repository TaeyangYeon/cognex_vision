using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PLC
{
    public static class PLCCommand
    {
        public const string header = "500000FF03FF00";
        public const string networkNumber = "00";
        public const string plcNumber = "FF";
        public const string moduleIoNumber = "03FF";
        public const string moduleDeviceNumber = "00";

        public const string cpuInspectorData = "0010";
        public const string bitReadCommand = "0401";
        public const string bitWriteCommand = "1402";
        public const string wordReadCommand = "0401";
        public const string wordWriteCommand = "1401";

        public const string subCommandSub = "D";
        public const string subCommandWord = "W";
        public const string subCommandBit = "B";
    }
}