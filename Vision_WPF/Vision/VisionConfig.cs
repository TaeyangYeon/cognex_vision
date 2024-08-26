using Cognex.VisionPro.Display;

namespace Vision
{
    public class VisionConfig
    {
        public IGrabControl iGrabControl(CogDisplay display, int cameraIndex)
        {
            return new GrabControl(display, cameraIndex);
        }

        public IImageFileControl iImageFileControl(CogDisplay display)
        {
            return new ImageFileControl(display);
        }
    }
}
