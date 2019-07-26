// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.
using System;
using System.Numerics;
using System.Runtime.InteropServices;

namespace Microsoft.Azure.Kinect.Sensor.BodyTracking
{
    [StructLayout(LayoutKind.Sequential)]
    [Native.NativeReference("k4abt_joint_t")]
    public class Joint
    {
        public Vector3 Position { get; set; }
        public Microsoft.Azure.Kinect.Sensor.Quaternion Orientation { get; set; }
    }
}
