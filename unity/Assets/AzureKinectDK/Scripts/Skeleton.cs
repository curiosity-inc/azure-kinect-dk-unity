// Copyright (c) Takahiro Horikawa. All rights reserved.
// Licensed under the MIT License.
using System;
using System.Numerics;
using System.Runtime.InteropServices;

namespace Microsoft.Azure.Kinect.Sensor.BodyTracking
{
    [StructLayout(LayoutKind.Sequential)]
    [Native.NativeReference("k4abt_skeleton_t")]
    public struct Skeleton
    {
        [MarshalAs(UnmanagedType.ByValArray, ArraySubType = UnmanagedType.Struct, SizeConst = (int)JointId.Count)]
        public Joint[] Joints;
    }

    [StructLayout(LayoutKind.Sequential)]
    [Native.NativeReference("k4abt_joint_t")]
    public struct Joint
    {
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)]
        public float[] Position;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
        public float[] Orientation;
    }
}
