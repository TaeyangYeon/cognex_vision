using System;
using System.Threading.Tasks;
using Cognex.VisionPro;
using UI;

namespace FormEvent
{
    public interface IMainEvent : IDisposable
    {
        void setMainForm(Form_Main body);
        void setCurrentPositionAndListBox(Form_Main body, string position);
        void selectPosition(string position, Form_Main form);
        Task socketConnect();
        Task<string> getModelName(string address);
        Task testCycle(int index, Form_Main form, int result, (int x, int y) AlignValues);
        Task socketClose();
        void initializeCamera();
        void toggleLiveMode(bool isLiveMode);
        ICogImage grabImage();
        Task loadImage();
    }
}
