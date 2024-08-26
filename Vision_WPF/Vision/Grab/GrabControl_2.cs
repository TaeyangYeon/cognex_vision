using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Cognex.VisionPro;
using Cognex.VisionPro.Display;
using System.Runtime.InteropServices;
using SystemConf;

namespace Vision
{
    public class GrabControl_2 : IDisposable
    {
        private static readonly Lazy<GrabControl_2> _instance = new Lazy<GrabControl_2>(() => new GrabControl_2());
        public static GrabControl_2 instance => _instance.Value;

        private readonly Dictionary<int, CameraInfo> cameras = new Dictionary<int, CameraInfo>();
        private readonly SemaphoreSlim semaphore = new SemaphoreSlim(4, 4);
        private bool isInitialized = false;
        private readonly int cameraCount = 4;

        private GrabControl_2() { }

        public async Task initializeAsync()
        {
            await semaphore.WaitAsync();
            try
            {
                if (isInitialized)
                {
                    throw new InvalidOperationException("Cameras are already initialized.");
                }
                await Task.Run(() =>
                {
                    CogFrameGrabbers grabbers = new CogFrameGrabbers();

                    for (int i = 0; i < cameraCount; i++)
                    {
                        if (grabbers.Count <= i)
                        {
                            throw new InvalidOperationException($"Requested camera {i} is not available.");
                        }

                        var frameGrabber = grabbers[i];
                        var acqFifo = frameGrabber.CreateAcqFifo("Generic GigEVision (Mono)", CogAcqFifoPixelFormatConstants.Format8Grey, 0, true);
                        
                        cameras[i] = new CameraInfo(frameGrabber, acqFifo);
                    }
                });

                isInitialized = true;
            }
            catch (COMException ex)
            {
                foreach (var camera in cameras.Values)
                {
                    camera.Dispose();
                }

                cameras.Clear();
                throw new InvalidOperationException("Failed to initialize cameras.", ex);
            }
            finally
            {
                semaphore.Release();
            }
        }

        public async Task toggleLiveModeAsync(int cameraIndex, bool isLiveMode, CogDisplay display)
        {
            await semaphore.WaitAsync();
            try
            {
                checkInitialized();
                if (cameras.TryGetValue(cameraIndex, out var cameraInfo))
                {
                    await cameraInfo.toggleLiveModeAsync(isLiveMode, display);
                }
                else
                {
                    throw new InvalidOperationException($"Camera {cameraIndex} is not initialized.");
                }
            }
            finally
            {
                semaphore.Release();
            }
        }

        public async Task<ICogImage> grabImageAsync(int cameraIndex, CogDisplay display)
        {
            await semaphore.WaitAsync();
            try
            {
                checkInitialized();
                if (cameras.TryGetValue(cameraIndex, out var cameraInfo))
                {
                    return await cameraInfo.grabImageAsync(display);
                }
                else
                {
                    throw new InvalidOperationException($"Camera {cameraIndex} is not initialized.");
                }
            }
            finally
            {
                semaphore.Release();
            }
        }

        private void checkInitialized()
        {
            if (!isInitialized)
            {
                throw new InvalidOperationException("Cameras are not initialized. Call InitializeAllCameraAsync first.");
            }
        }

        public void Dispose()
        {
            foreach (var camera in cameras.Values)
            {
                camera.Dispose();
            }
            cameras.Clear();
            isInitialized = false;
        }
    }

    public class CameraInfo : IDisposable
    {
        private readonly ICogFrameGrabber frameGrabber;
        private readonly ICogAcqFifo acqFifo;
        private bool isLive;
        private bool disposed = false;

        public CameraInfo(ICogFrameGrabber _frameGrabber, ICogAcqFifo _acqFifo)
        {
            this.frameGrabber = _frameGrabber;
            this.acqFifo = _acqFifo;
            this.isLive = false;
        }
        
        public async Task toggleLiveModeAsync(bool isLiveMode, CogDisplay display)
        {
            await Task.Run(() =>
            {
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
            });
        }

        public async Task<ICogImage> grabImageAsync(CogDisplay display)
        {
            return await Task.Run(() =>
            {
                if (isLive)
                {
                    display.StopLiveDisplay();
                    isLive = false;
                }

                return acqFifo.Acquire(out _);
            });
        }

        public void Dispose()
        {
            if (disposed) return;
            
            frameGrabber?.Disconnect(false);
            disposed = true;
        }
    }

}