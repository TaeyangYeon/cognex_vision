using System;
using System.Windows.Forms;
using System.Text;
using System.Threading;
using System.Diagnostics;
using System.Threading.Tasks;
using Log;

namespace PLC
{
    public class SignalLoop : ISignalLoop
    {
        public ISocketCommunication socket;
        private OutAlignVariable outAlignVar;
        private bool disposed = false;

        private readonly Logger logger = Logger.instance();

        public SignalLoop(ISocketCommunication _socket, OutAlignVariable _outAlignVar)
        {
            this.socket = _socket;
            this.outAlignVar = _outAlignVar;
        }

        public void connectToServer()
        {
            try
            {
                socket.connectToServer();
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public int alignSignalCheck(AlignVariable alignVar, int measurePosition, int alignPosition)
        {
            int result = 0;
            int timer = Environment.TickCount;
            int elapsed = 0;
            int response = 0;
            string signal = alignVar.alignSignalStart[measurePosition, alignPosition];

            do
            {
                result = socket.readBitPublic(PLCCommand.subCommandBit, signal);

                if (result == 1 || response != 0)
                {
                    socket.writeBitPublic(PLCCommand.subCommandBit, alignVar.alignOutSignal[alignPosition], true);

                    if (response != 0) result = 1;
                }

                elapsed = Environment.TickCount - timer;
            }
            while (result == 1 && elapsed <= 1000);

            clearAlignSignals(alignPosition);
            socket.writeBitPublic(PLCCommand.subCommandBit, alignVar.alignOutSignal[alignPosition], true);

            return result;
        }

        public int calibSignalCheck(string dataAddress, string calibSignalAddress, int alignPosition)
        {
            int result = 0;
            int calibSignal = 0;
            int timer = Environment.TickCount;
            int elapsed = 0;

            do
            {
                result = socket.readBitPublic(PLCCommand.subCommandBit, dataAddress);
                calibSignal = socket.readBitPublic(PLCCommand.subCommandBit, calibSignalAddress);
                elapsed = Environment.TickCount - timer;
                Application.DoEvents();
            }
            while ((result == 0 || calibSignal == 0) && elapsed <= 30000);

            return result;
        }

        public int userVerifySignalCheck(string dataAddress, int alignPosition)
        {
            int result = 0;
            int timer = Environment.TickCount;
            int elapsed = 0;

            do
            {
                result = socket.readBitPublic(PLCCommand.subCommandBit, dataAddress);
                elapsed = Environment.TickCount - timer;
                Application.DoEvents();
            }
            while (result == 0 && elapsed <= 1000);

            return result;
        }

        public async Task<string> getData(string code, string address)
        {
            return await parseAsciiToString(socket.readWordPublic(code, address));
        }

        private async Task<string> parseAsciiToString(string response)
        {
            return await Task.Run(() =>
            {
                Console.WriteLine("response : " + response);

                string data = response.Substring(22);
                StringBuilder modelName = new StringBuilder();
                for (int i = 0; i < data.Length; i += 2)
                {
                    string hex = data.Substring(i, 2);
                    int value = Convert.ToInt32(hex, 16);
                    if (value == 0) break; // 널 터미네이터를 만나면 중지
                    modelName.Append((char)value);
                }

                return modelName.ToString().Trim();
            });
        }

        public void writeData(string code, string address, int value)
        {
            socket.writeWordPublic(code, address, value);
        }
        public int waitSec(int seconds)
        {
            int timer = Environment.TickCount;
            int elapsed = 0;

            do
            {
                Application.DoEvents();
                elapsed = Environment.TickCount - timer;
            }
            while (seconds * 1000 >= elapsed);

            return 0;
        }

        private void clearAlignSignals(int alignPosition)
        {
            outAlignVar.alignOk[alignPosition] = 0;
            outAlignVar.markNg[alignPosition] = 0;
            outAlignVar.limitNg[alignPosition] = 0;
            outAlignVar.camsetNg[alignPosition] = 0;
            outAlignVar.retry[alignPosition] = 0;
            outAlignVar.retryCountNg[alignPosition] = 0;
        }

        public async void checkAlignSignalChanged(int status, string code, string address)
        {
            int result = status == 1 ? 0 : 1;

            Stopwatch timer = new Stopwatch();
            timer.Start();

            while (status == result)
            {
                if (timer.ElapsedMilliseconds > 2000)
                {
                    throw new NotChangedPlcSignal("'Align Start Signal'이 바뀌지 않습니다.");
                }

                await Task.Delay(100);
                status = int.Parse(await getData(code, address));
            }
        }

        public void closeToServer()
        {
            socket.closeConnection();
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposed)
            {
                if (disposing)
                {
                    // 관리되는 자원 해제
                    if (socket != null)
                    {
                        socket.Dispose();
                    }
                }
                disposed = true;
            }
        }

        // 소멸자
        ~SignalLoop()
        {
            Dispose(false);
        }
    }

    public class NotChangedPlcSignal : Exception
    {
        public override string Message => base.Message;
        public NotChangedPlcSignal() { }
        public NotChangedPlcSignal(string message) : base(message) { }

    }
}
