using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

// Copyright (c) Takahiro Horikawa. All rights reserved.
// Licensed under the MIT License.

namespace Microsoft.Azure.Kinect.Sensor.BodyTracking
{
    public class BodyTracker : IDisposable
    {
        internal BodyTracker(BodyTrackingNativeMethods.k4abt_tracker_t handle)
        {
            this.handle = handle;
        }

        public static BodyTracker Create(Calibration sensorCalibration)
        {
            AzureKinectException.ThrowIfNotSuccess(
                BodyTrackingNativeMethods.k4abt_tracker_create(sensorCalibration, out BodyTrackingNativeMethods.k4abt_tracker_t handle));

            return new BodyTracker(handle);
        }

        public void EnqueueCapture(Capture capture, int timeoutInMS = -1)
        {
            lock (this)
            {
                if (disposedValue)
                    throw new ObjectDisposedException(nameof(Device));

                NativeMethods.k4a_wait_result_t result = BodyTrackingNativeMethods.k4abt_tracker_enqueue_capture(handle, capture.DangerousGetHandle(), timeoutInMS);

                if (result == NativeMethods.k4a_wait_result_t.K4A_WAIT_RESULT_TIMEOUT)
                {
                    throw new TimeoutException("Timed out waiting for capture");
                }

                AzureKinectException.ThrowIfNotSuccess(result);
            }
        }

        public BodyFrame PopResult(int timeoutInMS = -1)
        {
            lock (this)
            {
                if (disposedValue)
                    throw new ObjectDisposedException(nameof(Device));

                NativeMethods.k4a_wait_result_t result = BodyTrackingNativeMethods.k4abt_tracker_pop_result(handle, out BodyTrackingNativeMethods.k4abt_frame_t frame, timeoutInMS);

                if (result == NativeMethods.k4a_wait_result_t.K4A_WAIT_RESULT_TIMEOUT)
                {
                    throw new TimeoutException("Timed out waiting for capture");
                }

                AzureKinectException.ThrowIfNotSuccess(result);

                return new BodyFrame(frame);
            }
        }

        private BodyTrackingNativeMethods.k4abt_tracker_t handle;

        #region IDisposable Support
        private bool disposedValue = false; // To detect redundant calls

        protected virtual void Dispose(bool disposing)
        {
            lock (this)
            {
                if (!disposedValue)
                {
                    if (disposing)
                    {
                    }

                    // TODO: free unmanaged resources (unmanaged objects) and override a finalizer below.
                    // TODO: set large fields to null.
                    handle.Close();
                    handle = null;

                    disposedValue = true;
                }
            }
        }

        // TODO: override a finalizer only if Dispose(bool disposing) above has code to free unmanaged resources.
        ~BodyTracker()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            Dispose(false);
        }

        // This code added to correctly implement the disposable pattern.
        public void Dispose()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            Dispose(true);
            // TODO: uncomment the following line if the finalizer is overridden above.
            GC.SuppressFinalize(this);
        }
        #endregion

    }
}

