using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cognex.VisionPro;

namespace Vision
{
    public interface IGrabControl : IDisposable
    {
        /**
         * 카메라 초기화 메서드
         * CogFrameGrabbers() 객체 생성으로 PC와 연결된 모든 카메라 정보를 가져온다.
         * 카메라의 수량만큼 CogFrameGrabbers 안에 요소가 생긴다.
         * 여기서 카메라를 하나씩 ICogFrameGrabber()로 분리 할당하고, 
         * ICogFrameGrabber에서 영상을 설정하여 CreateAcqFifo를 사용하여 ICogAcqFifo에 할당한다.
         */
        void initializeCamera();
        /**
         * Display의 라이브 모드를 제어하는 메서드
         * 클라이언트에서 사용 여부를 불리언 값으로 받아
         * 라이브 모드를 시작 or 종료한다.
         */
        void toggleLiveMode(bool isLiveMode);
        /**
         * 그랩 메서드
         * 라이브 모드일 경우 중지시키고
         * acqFifo에서 영상을 그랩하여 ICogImage로 반환한다.
         */
        ICogImage grabImage();
        /**
         * 카메라 연결 초기화를 관리
         */
        bool isInitialized { get; }
    }
}
