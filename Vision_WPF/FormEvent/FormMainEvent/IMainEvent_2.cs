using System;
using System.Threading.Tasks;
using Cognex.VisionPro;
using Cognex.VisionPro.Display;
using UI;

namespace FormEvent
{
    public interface IMainEvent_2 : IDisposable
    {
        void setMainForm(Form_Main body);
        void setCurrentPositionAndListBox(Form_Main body, string position);
        void selectPosition(string position, Form_Main form);
        Task socketConnect();
        Task<string> getModelName(string address);
        Task testCycle(int index, Form_Main form, int result, (int x, int y) AlignValues);
        Task socketClose();
        Task initializeCamera();
        void toggleLiveMode(int cameraIndex, bool isLiveMode, CogDisplay display);
        Task<ICogImage> grabImage(int cameraIndex, CogDisplay display);
        Task loadImage(CogDisplay display);
    }
}
