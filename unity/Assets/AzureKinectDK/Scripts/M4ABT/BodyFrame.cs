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

        // This function retrieves a native image handle from the native API.
        //
        // If this is the first time calling the property, we construct a new wrapper
        // If the handle is for an Image we have already constructed a wrapper for, we return the existing wrapper
        // If the handle is for a different Image, or if the wrapper has already been disposed, we construct a new wrapper and dispose the old one
        private Image GetImageWrapperAndDisposePrevious(Func<BodyTrackingNativeMethods.k4abt_frame_t, NativeMethods.k4a_image_t> nativeMethod,
            ref Image cache)
        {
            // Lock must be held to ensure the Image in cache is not replaced while we are inspecting it
            // It is still possible for that Image to be accessed or Disposed while this lock is held
            lock (this)
            {
                if (this.disposedValue)
                {
                    throw new ObjectDisposedException(nameof(Capture));
                }

                NativeMethods.k4a_image_t image = nativeMethod(this.handle);
                // This try block ensures that we close image
                try
                {
                    if (cache != null)
                    {
                        IntPtr imageHandle = image.DangerousGetHandle();

                        // Only attempt to access cache in this try block
                        try
                        {
                            // If cache was disposed before we called the native accessor (by the caller or another thread), 
                            // the handle could have been reused and the values would incorrectly match. However, cache.DangerousGetHandle()
                            // will throw an exception, and we will correctly construct a new image wrapper.
                            if (cache.DangerousGetHandle().DangerousGetHandle() != imageHandle)
                            {
                                // The image has changed, invalidate the current image and construct a new wrapper
                                cache.Dispose();
                                cache = null;
                            }
                        }
                        catch (ObjectDisposedException)
                        {
                            // If cache has been disposed by the caller or another thread we will discard
                            // it and construct a new wrapper
                            cache = null;
                        }
                    }

                    if (cache == null && !image.IsInvalid)
                    {
                        // Construct a new wrapper and return it
                        // The native function may have returned 
                        cache = new Image(image);

                        // Since we have wrapped image, it is now owned by the UnsafeImage object and we should no longer close it
                        image = null;
                    }
                }
                finally
                {
                    // Ensure the native handle is closed if we have a failure creating the Image object
                    if (image != null)
                    {
                        image.Close();
                    }
                }

                return cache;
            }
        }

        private Image _BodyIndexMap;

        public Image BodyIndexMap
        {
            get
            {
                return this.GetImageWrapperAndDisposePrevious(BodyTrackingNativeMethods.k4abt_frame_get_body_index_map, ref this._BodyIndexMap);
            }
            set
            {
                lock (this)
                {
                    if (this.disposedValue)
                    {
                        throw new ObjectDisposedException(nameof(BodyFrame));
                    }

                    // If the assignment is a new managed wrapper we need
                    // to release the reference to the old wrapper.
                    // If it is the same object though, we should not dispose it.
                    if (this._BodyIndexMap != null && !object.ReferenceEquals(
                        this._BodyIndexMap, value))
                    {
                        this._BodyIndexMap.Dispose();
                    }
                    this._BodyIndexMap = value;
                }
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
