//------------------------------------------------------------------------------
// <copyright file="BodyTrackingNativeMethods.cs" company="Microsoft">
// Copyright (c) Takahiro Horikawa. All rights reserved.
// Licensed under the MIT License.
// </copyright>
//------------------------------------------------------------------------------
using System;
using System.Numerics;
using System.Runtime.InteropServices;
using System.Text;
using Microsoft.Azure.Kinect.Sensor.Native;

namespace Microsoft.Azure.Kinect.Sensor.BodyTracking
{
#pragma warning disable IDE1006 // Naming Styles

    internal class BodyTrackingNativeMethods
    {
        // These types are used internally by the interop dll for marshaling purposes and are not exposed
        // over the public surface of the managed dll.

        #region Handle Types

        public class k4abt_tracker_t : Microsoft.Win32.SafeHandles.SafeHandleZeroOrMinusOneIsInvalid
        {
            private k4abt_tracker_t() : base(true)
            {
            }

            protected override bool ReleaseHandle()
            {
                BodyTrackingNativeMethods.k4abt_tracker_destroy(handle);
                return true;
            }
        }

        public class k4abt_frame_t : Microsoft.Win32.SafeHandles.SafeHandleZeroOrMinusOneIsInvalid
        {
            private k4abt_frame_t() : base(true)
            {
            }

            public k4abt_frame_t DuplicateReference()
            {
                k4abt_frame_t duplicate = new k4abt_frame_t();

                BodyTrackingNativeMethods.k4abt_frame_reference(handle);
                duplicate.handle = this.handle;
                return duplicate;
            }
            protected override bool ReleaseHandle()
            {
                BodyTrackingNativeMethods.k4abt_frame_release(handle);
                return true;
            }
        }

        #endregion

        #region Enumerations
        #endregion

        #region Structures

        #endregion

        #region Functions

        [DllImport("k4abt", CallingConvention = CallingConvention.Cdecl)]
        [NativeReference]
        public static extern NativeMethods.k4a_result_t k4abt_tracker_create(Calibration sensorCalibration, out k4abt_tracker_t tracker_handle);

        [DllImport("k4abt", CallingConvention = CallingConvention.Cdecl)]
        [NativeReference]
        public static extern void k4abt_tracker_destroy(IntPtr tracker_handle);

        [DllImport("k4abt", CallingConvention = CallingConvention.Cdecl)]
        [NativeReference]
        public static extern NativeMethods.k4a_wait_result_t k4abt_tracker_enqueue_capture(k4abt_tracker_t tracker_handle, NativeMethods.k4a_capture_t sensor_capture_handle, Int32 timeout_in_ms);

        [DllImport("k4abt", CallingConvention = CallingConvention.Cdecl)]
        [NativeReference]
        public static extern NativeMethods.k4a_wait_result_t k4abt_tracker_pop_result(k4abt_tracker_t tracker_handle, out k4abt_frame_t body_frame_handle, Int32 timeout_in_ms);

        [DllImport("k4abt", CallingConvention = CallingConvention.Cdecl)]
        [NativeReference]
        public static extern void k4abt_frame_release(IntPtr body_frame_handle);

        [DllImport("k4abt", CallingConvention = CallingConvention.Cdecl)]
        [NativeReference]
        public static extern void k4abt_frame_reference(IntPtr body_frame_handle);

        [DllImport("k4abt", CallingConvention = CallingConvention.Cdecl)]
        [NativeReference]
        public static extern UInt32 k4abt_frame_get_num_bodies(k4abt_frame_t body_frame_handle);

        [DllImport("k4abt", CallingConvention = CallingConvention.Cdecl)]
        [NativeReference]
        public static extern NativeMethods.k4a_result_t k4abt_frame_get_body_skeleton(k4abt_frame_t body_frame_handle, UInt32 index, out Skeleton skeleton);

        [DllImport("k4abt", CallingConvention = CallingConvention.Cdecl)]
        [NativeReference]
        public static extern UInt32 k4abt_frame_get_body_id(k4abt_frame_t body_frame_handle, UInt32 index);

        #endregion

    }
#pragma warning restore IDE1006 // Naming Styles
}
