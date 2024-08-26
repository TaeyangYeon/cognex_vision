using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PLC
{
    public interface ISignalLoop : IDisposable
    {
        /**
         * 서버 연결 메서드
         */
        void connectToServer();
        /**
         * Align 시작 신호 조회
         */
        int alignSignalCheck(AlignVariable alignVar, int measurePosition, int alignPosition);
        /**
         * Calibration 시작 신호 조회
         */
        int calibSignalCheck(string dataAddress, string calibSignalAddress, int alignPosition);
        int userVerifySignalCheck(string dataAddress, int alignPosition);
        /**
         * 서버 주소에 할당된 데이터를 조회하는 메서드
         */
        Task<string> getData(string code, string address);
        /**
         * 서버 주소에 데이터를 할당
         */
        void writeData(string code, string address, int value);
        /**
         * Align 신호가 변경됨을 조회
         * 2,000ms 동안 100ms 간격으로 조회
         */
        void checkAlignSignalChanged(int status, string code, string address);
        /**
         * 인자로 받는 시간만큼 대기하는 메서드
         */
        int waitSec(int seconds);
        /**
         * 서버 연결 차단 메서드
         */
        void closeToServer();
    }
}
