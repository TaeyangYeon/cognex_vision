using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using Cognex.VisionPro;
using Cognex.VisionPro.Display;
using Log;

namespace Vision
{
    public class GrabControlSync : IGrabControl
    {
        private CogDisplay display;
        private int cameraIndex;
        private ICogFrameGrabber frameGrabber;
        private ICogAcqFifo acqFifo;
        private bool isLive;
        private bool disposed = false;

        private readonly Logger logger = Logger.instance();

        public bool isInitialized { get; private set; }

        public GrabControlSync(CogDisplay _display, int _cameraIndex)
        {
            this.display = _display;
            this.cameraIndex = _cameraIndex;
            this.isInitialized = false;
        }

        public void initializeCamera()
        {
            try
            {
                CogFrameGrabbers grabbers = new CogFrameGrabbers();
                if (grabbers.Count <= cameraIndex)
                {
                    throw new NotMatchedCameraQuantityException("요청한 카메라의 수량과 연결된 카메라의 수량이 맞지 않습니다.");
                }

                frameGrabber = grabbers[cameraIndex];
                acqFifo = frameGrabber.CreateAcqFifo("Generic GigEVision (Mono)", CogAcqFifoPixelFormatConstants.Format8Grey, 0, true);
                isLive = false;
                isInitialized = true;
            }
            catch (NotMatchedCameraQuantityException e)
            {
                logger.pushLog(LogLevel.ERROR, e.Message);
                throw e;
            }
            catch (COMException e)
            {
                frameGrabber.Disconnect(false);
                frameGrabber = null;

                logger.pushLog(LogLevel.ERROR, "Failed Init Camera.");
                logger.pushLog(LogLevel.ERROR, e.Message);

                throw new COMException("카메라 초기화에 실패했습니다. 다시 시도하세요.");
            }
            catch (Exception e) 
            {
                frameGrabber.Disconnect(false);
                frameGrabber = null;

                logger.pushLog(LogLevel.ERROR, e.Message);

                throw new Exception("카메라 연결 실패");
            }
        }

        private void checkInitialized()
        {
            if (!isInitialized)
            {
                var e = new InvalidOperationException("Camera is not initialized.");
                logger.pushLog(LogLevel.ERROR, e.Message);
                throw e;
            }

        }

        public void toggleLiveMode(bool isLiveMode)
        {
            checkInitialized();

            if (isLiveMode)
            {
                acqFifo.OwnedTriggerParams.TriggerEnabled = true;
                acqFifo.OwnedTriggerParams.TriggerModel = CogAcqTriggerModelConstants.Manual;

                display.StartLiveDisplay(acqFifo);

                isLive = true;
            }
            else
            {
                display.StopLiveDisplay();

                isLive = false;
            }
        }

        public ICogImage grabImage()
        {
            checkInitialized();

            if (isLive) toggleLiveMode(false);

            return acqFifo.Acquire(out _);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        public virtual void Dispose(bool disposing)
        {
            if (!disposed)
            {
                if (disposing)
                {
                    if (display != null)
                    {
                        display.StopLiveDisplay();
                        display = null;
                    }

                    if (frameGrabber != null)
                    {
                        frameGrabber.Disconnect(false);
                        frameGrabber = null;
                    }

                    if (acqFifo != null)
                    {
                        acqFifo = null;
                    }
                }
                logger.pushLog(LogLevel.DEBUG, "Grab Instance Dispose.");
                disposed = true;
            }
        }

        ~GrabControlSync()
        {
            Dispose(false);
        }

    }

}
