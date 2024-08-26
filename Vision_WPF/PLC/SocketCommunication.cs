using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;
using System.Threading;
using System.IO;
using System.Net;
using Log;

namespace PLC
{
    public class ConnectPortManager
    {
        private static readonly Lazy<ConnectPortManager> instance = new Lazy<ConnectPortManager>(() => new ConnectPortManager());
        private readonly HashSet<int> usedPorts = new HashSet<int>();
        private readonly object lockObj = new object();

        private ConnectPortManager() { }

        public static ConnectPortManager Instance => instance.Value;

        public bool TryAddPort(int port)
        {
            lock (lockObj)
            {
                if (usedPorts.Contains(port))
                {
                    return false;
                }

                usedPorts.Add(port);

                return true;
            }
        }

        public void RemovePort(int port)
        {
            lock (lockObj)
            {
                usedPorts.Remove(port);
            }
        }
    }

    public class SocketCummunication : ISocketCommunication
    {
        private string ipAddress;
        private int port;
        public Socket clientSocket; // 클라이언트 소켓 인스턴스
        private AsyncCallback receiveCallback; // 데이터 수신 완료 시 호출될 콜백 메서드
        private AsyncCallback sendCallback; // 데이터 전송 완료 시  호출될 콜백 메서드
        private string receiveData;
        private ManualResetEvent receiveDone = new ManualResetEvent(false);
        private bool disposed = false; // Dispose 호출 여부를 추적

        private readonly Logger logger = Logger.instance();

        // 생성자 : IP 주소와 포트를 받아서 초기화하고 서버에 연결을 시도
        public SocketCummunication(string _ipAddress, int _port)
        {
            this.ipAddress = _ipAddress;
            this.port = _port;
            clientSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            clientSocket.ReceiveTimeout = 5000; // 5초 타임아웃 설정
            receiveCallback = new AsyncCallback(handleDataReceive);
            sendCallback = new AsyncCallback(handleDataSend);

            logger.pushLog(LogLevel.INFO, $"Create Socket Instance. IP : {ipAddress}, PORT : {port}");
        }

        public bool isConnected()
        {
            return clientSocket.Connected;
        }

        public void connectToServer()
        {
            try
            {
                if (!ConnectPortManager.Instance.TryAddPort(port) || checkConnection())
                {
                    string message = "Port is already in use. Cannot connect to the same port multiple times.";
                    logger.pushLog(LogLevel.ERROR, message);
                    throw new SocketConnectFailException(message);
                }

                if (clientSocket == null)
                {
                    clientSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                    clientSocket.ReceiveTimeout = 5000;
                }

                clientSocket.Connect(ipAddress, port); // PLC에 연결
                logger.pushLog(LogLevel.INFO, "Connect To Server.");
            }
            catch (SocketException e)
            {
                ConnectPortManager.Instance.RemovePort(port);
                logger.pushLog(LogLevel.ERROR, e.Message);
                throw e;
            }
        }

        // 연결 상태를 확인하는 메서드
        private bool checkConnection()
        {
            try
            {
                // Poll로 연결 상태 확인
                if (clientSocket != null && clientSocket.Connected)
                {
                    bool part1 = clientSocket.Poll(1000, SelectMode.SelectRead);
                    bool part2 = (clientSocket.Available == 0);

                    return !(part1 && part2);
                }
                return false;
            }
            catch (Exception e)
            {
                logger.pushLog(LogLevel.ERROR, "Socket Connection Check is fail.");
                logger.pushLog(LogLevel.ERROR, e.Message);
                return false;
            }
        }

        public void sendMessage(string message)
        {
            if (checkConnection())
            {
                byte[] data = Encoding.ASCII.GetBytes(message + "\n");
                clientSocket.Send(data);
            }
            else
            {
                throw new IOException();
            }
        }

        // 데이터 전송 완료 시 호출되는 콜백 메서드
        private void handleDataSend(IAsyncResult ar)
        {
            try
            {
                Socket handler = (Socket)ar.AsyncState;
                handler.EndSend(ar); // 전송 완료
            }
            catch (Exception e)
            {
                logger.pushLog(LogLevel.ERROR, "Send Fail");
                logger.pushLog(LogLevel.ERROR, e.Message);
                throw e;
            }
        }

        // PLC로부터 메시지를 수신하는 메서드
        public string receiveMessage()
        {
            if (checkConnection())
            {
                receiveDone.Reset();
                byte[] buffer = new byte[1024];
                clientSocket.BeginReceive(buffer, 0, buffer.Length, SocketFlags.None, receiveCallback, buffer);
                receiveDone.WaitOne();
                return receiveData;
            }
            else
            {
                throw new IOException();
            }
        }

        private void receiveCallBack(IAsyncResult ar)
        {
            try
            {
                byte[] buffer = (byte[])ar.AsyncState;
                int bytesRead = clientSocket.EndReceive(ar);
                if (bytesRead > 0)
                {
                    receiveData = Encoding.ASCII.GetString(buffer, 0, bytesRead);
                }
                receiveDone.Set();
            }
            catch (Exception e)
            {
                logger.pushLog(LogLevel.ERROR, "Receive failed.");
                logger.pushLog(LogLevel.ERROR, e.Message);
            }
        }

        // 데이터 수신 완료 시 호출되는 메서드
        private void handleDataReceive(IAsyncResult ar)
        {
            try
            {
                byte[] buffer = (byte[])ar.AsyncState;
                int bytesRead = clientSocket.EndReceive(ar); // 수신된 바이트 수
                if (bytesRead > 0)
                {
                    receiveData = Encoding.ASCII.GetString(buffer, 0, bytesRead);
                }
                receiveDone.Set(); // 수신 완료 신호
            }
            catch (Exception e)
            {
                logger.pushLog(LogLevel.ERROR, "Receive failed.");
                logger.pushLog(LogLevel.ERROR, e.Message);
                throw e;
            }
        }

        private string convertUnit(string cmd, int length)
        {
            return cmd.PadLeft(length, '0');
        }

        private string generateCommand(string length, string command, string subCommand, string deviceCode, string deviceAddress, string data)
        {
            return PLCCommand.header + length + command + subCommand + deviceCode + "*" + deviceAddress + data;
        }

        public int readBitPublic(string deviceCode, string deviceAddress)
        {
            string message = generateCommand("0010", PLCCommand.bitReadCommand, PLCCommand.subCommandBit, deviceCode, convertUnit(deviceAddress, 6), "0001");
            sendMessage(message);
            string response = receiveMessage();
            return parseBitResponse(response);
        }

        public void writeBitPublic(string deviceCode, string deviceAddress, bool value)
        {
            string message = generateCommand("0014", PLCCommand.bitWriteCommand, PLCCommand.subCommandBit, deviceCode, convertUnit(deviceAddress, 6), value ? "01" : "00");
            sendMessage(message);
        }

        public string readWordPublic(string deviceCode, string deviceAddress)
        {
            string message = generateCommand("0010", PLCCommand.wordReadCommand, PLCCommand.subCommandWord, deviceCode, convertUnit(deviceAddress, 6), "0001");
            sendMessage(message);
            return receiveMessage();
        }

        public void writeWordPublic(string deviceCode, string deviceAddress, int value)
        {
            string message = generateCommand("0014", PLCCommand.wordWriteCommand, PLCCommand.subCommandWord, deviceCode, convertUnit(deviceAddress, 6), convertUnit(Convert.ToString(value, 16), 4));
            sendMessage(message);
        }

        private int parseBitResponse(string response)
        {
            // 응답 메시지에서 비트 값을 추출하는 로직 구현
            // 여기서는 예시로 응답 메시지가 비트 값만 포함한다고 가정
            return int.Parse(response.Substring(22, 1)); // 응답 메시지에서 비트 값을 추출
        }

        public void closeConnection()
        {
            if (checkConnection())
            {
                ConnectPortManager.Instance.RemovePort(port); // 포트 삭제
                clientSocket.Shutdown(SocketShutdown.Both); // 송수신 종료
                clientSocket.Close(); // 소캣 닫기
            }

            clientSocket = null;
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
                    // 연결 차단
                    if (checkConnection()) closeConnection();

                    // 관리되는 자원 해제
                    receiveDone.Dispose();
                    if (clientSocket != null)
                    {
                        clientSocket.Close();
                        clientSocket.Dispose();
                        clientSocket = null;
                    }
                }
                // 관리되지 않는 자원 해제
                disposed = true;
                logger.pushLog(LogLevel.DEBUG, "Socket Instance Dispose.");
            }
        }

        // 소멸자
        ~SocketCummunication()
        {
            Dispose(false);
        }
    }

    public class SocketConnectFailException : Exception
    {
        public override string Message => base.Message;
        public SocketConnectFailException() { }
        public SocketConnectFailException(string message) : base(message) { }
    }
}
