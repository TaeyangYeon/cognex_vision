using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cognex.VisionPro;
using Cognex.VisionPro.Display;

namespace Vision
{
    public interface IImageFileControl
    {
        /**
         * 이미지를 Display에 로드하는 메서드
         * CogImageFileTool()을 사용하여
         * 이미지의 경로를 입력받아 ICogImage를 반환하여
         * Display.Image 할당한다.
         */
        Task loadImage(string filePath);
    }
}
