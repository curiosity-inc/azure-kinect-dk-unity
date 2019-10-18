// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.
using System;
using System.Numerics;
using System.Runtime.InteropServices;

namespace Microsoft.Azure.Kinect.Sensor
{
    [StructLayout(LayoutKind.Sequential)]
    [Native.NativeReference("k4abt_tracker_configuration_t ")]
    public struct TrackerCalibration
    {

        public SensorOrientation sensor_orientation;

        public int cpu_only_mode;
    }
}
