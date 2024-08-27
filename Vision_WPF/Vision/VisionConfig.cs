using Cognex.VisionPro.Display;

namespace Vision
{
    public class VisionConfig
    {
        public IGrabControl iGrabControl(CogDisplay display, int cameraIndex)
        {
            return new GrabControlSync(display, cameraIndex);
        }

        public IImageFileControl iImageFileControl(CogDisplay display)
        {
            return new ImageFileControl(display);
        }
    }
}
