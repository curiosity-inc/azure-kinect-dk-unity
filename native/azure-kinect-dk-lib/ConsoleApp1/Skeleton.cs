// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.
using System;
using System.Numerics;
using System.Runtime.InteropServices;

namespace Microsoft.Azure.Kinect.Sensor.BodyTracking
{
    [StructLayout(LayoutKind.Sequential)]
    [Native.NativeReference("k4abt_skeleton_t")]
    public class Skeleton
    {
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 26)]
        public Joint[] Joints;
    }
}
