using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PLC
{
    public class AlignConstValue
    {
        public const int MAX_UD_ALIGN_POSITION = 3;
        public const int MAX_ALIGN_POSITION = 5;
    }

    public class AlignData
    {
        public string x { get; set; }
        public string y { get; set; }
        public string a { get; set; }
    }

    public class OutAlignVariable
    {
        public short[] alignOk = new short[AlignConstValue.MAX_ALIGN_POSITION];
        public short[] markNg = new short[AlignConstValue.MAX_ALIGN_POSITION];
        public short[] limitNg = new short[AlignConstValue.MAX_ALIGN_POSITION];
        public short[] camsetNg = new short[AlignConstValue.MAX_ALIGN_POSITION];
        public short[] retry = new short[AlignConstValue.MAX_ALIGN_POSITION];
        public short[] retryCountNg = new short[AlignConstValue.MAX_ALIGN_POSITION];

        public short modelChangeCheck = 0;
        public short onlineVisionOut = 0;
        public bool alignSignalComplete = false;
    }

    public class AlignVariable
    {
        public string[,] alignSignalStart { get; private set; }
        public string[] alignOutSignal { get; private set; }

        public string alignOnlineCheck { get; set; }
        public string alignOnlineAck { get; set; }

        public string alignModelNumber { get; set; }
        public string alignModelName { get; set; }
        public string pcOff { get; set; }

        public string[] plugIndex { get; private set; }
        public string[] idAddress { get; private set; }

        public AlignData[] alignData { get; private set; }
        public AlignData[] alignRead { get; private set; }
        public AlignData[] offsetRead { get; private set; }
        public AlignData[] alignDirection { get; private set; }

        public string[] userVerifyComplete { get; private set; }
        public string[] UserVerifyAck { get; private set; }

        public AlignVariable()
        {
            int maxAlignPosition = AlignConstValue.MAX_ALIGN_POSITION;
            int maxUdAlignPosition = AlignConstValue.MAX_UD_ALIGN_POSITION;

            alignSignalStart = new string[maxUdAlignPosition, maxAlignPosition];
            alignOutSignal = new string[maxAlignPosition];

            plugIndex = new string[maxAlignPosition];
            idAddress = new string[maxAlignPosition];

            alignData = new AlignData[maxAlignPosition];
            alignRead = new AlignData[maxAlignPosition];
            offsetRead = new AlignData[maxAlignPosition];
            alignDirection = new AlignData[maxAlignPosition];

            userVerifyComplete = new string[maxAlignPosition];
            UserVerifyAck = new string[maxAlignPosition];

            for (int i = 0; i < maxAlignPosition; i++)
            {
                alignData[i] = new AlignData();
                alignRead[i] = new AlignData();
                offsetRead[i] = new AlignData();
                alignDirection[i] = new AlignData();
            }
        }
    }
}
