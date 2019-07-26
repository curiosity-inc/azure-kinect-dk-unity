// Copyright (c) Takahiro Horikawa. All rights reserved.
// Licensed under the MIT License.
using System;

namespace Microsoft.Azure.Kinect.Sensor.BodyTracking
{
    public class BodyFrame : IDisposable
    {
        internal BodyFrame(BodyTrackingNativeMethods.k4abt_frame_t handle)
        {
            this.handle = handle;
        }
        
        public UInt32 NumBodies
        {
            get
            {
                lock (this)
                {
                    if (disposedValue)
                        throw new ObjectDisposedException(nameof(BodyFrame));

                    return BodyTrackingNativeMethods.k4abt_frame_get_num_bodies(handle);
                }
            }
        }

        public Skeleton GetSkeleton(UInt32 index)
        {
            lock (this)
            {
                if (disposedValue)
                    throw new ObjectDisposedException(nameof(BodyFrame));
                BodyTrackingNativeMethods.k4abt_frame_get_body_skeleton(handle, index, out Skeleton skeleton);
                return skeleton;
            }
        }

        public UInt32 GetBodyId(UInt32 index)
        {
            lock (this)
            {
                if (disposedValue)
                    throw new ObjectDisposedException(nameof(BodyFrame));
                return BodyTrackingNativeMethods.k4abt_frame_get_body_id(handle, index);
            }
        }

        public BodyFrame Reference()
        {
            lock (this)
            {
                if (disposedValue)
                    throw new ObjectDisposedException(nameof(BodyFrame));

                return new BodyFrame(handle.DuplicateReference());
            }
        }

        private BodyTrackingNativeMethods.k4abt_frame_t handle;

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
        ~BodyFrame()
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
