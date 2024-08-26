using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PLC
{
    public interface ISocketCommunication : IDisposable
    {
        /**
         * PLC 연결 상태를 반환
         */
        bool isConnected();
        /**
         * PLC에 연결을 시도하는 메서드
         * 포트 사용여부 및 연결 상태 확인
         * ip, port 할당하여 socket 생성
         */
        void connectToServer();
        /**
         * PLC로 메세지 전송 메서드
         * 문자열 값을 바이트(ASCII)로 변환
         * socket.Send
         */
        void sendMessage(string message);
        /**
         * PLC로부터 메세지를 수신하는 메서드
         * 크기 제한 : 1,024byte
         */
        string receiveMessage();
        /**
         * 비트 신호를 읽는 메서드
         */
        int readBitPublic(string devideCode, string deviceAddress);
        /**
         * 비트 신호를 쓰는 메서드
         */
        void writeBitPublic(string deviceCode, string deviceAddress, bool value);
        /**
         * 워드 값을 읽는 메서드
         */
        string readWordPublic(string deviceCode, string deviceAddress);
        /**
         * 워드 값을 쓰는 메서드
         */
        void writeWordPublic(string deviceCode, string deviceAddress, int value);
        /**
         * socket연결 차단 및 초기화
         */
        void closeConnection();
    }
}