using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cognex.VisionPro.Display;
using PLC;
using Vision;

namespace FormEvent
{
    public class MainEventConfig
    {
        public IMainEvent iMainEvent(int port)
        {
            return new MainEvent(
                new PositionChangeEvent(),
                new PLCConfig().iSignalLoop(port)
                );
        }

        public IMainEvent iMainEvent_2(int port)
        {
            return new MainEventGrabAsync(
                new PositionChangeEvent(), 
                new PLCConfig().iSignalLoop(port), 
                new ImageFileControl_2()
                );
        }

        public IMainEvent iMainEvent(int port, CogDisplay display, int cameraIndex)
        {
            return new MainEvent(
                new PositionChangeEvent(),
                new PLCConfig().iSignalLoop(port),
                new VisionConfig().iGrabControl(display, cameraIndex),
                new VisionConfig().iImageFileControl(display)
                );
        }
    }
}
