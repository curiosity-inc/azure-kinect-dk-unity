// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.
using System;
using System.Numerics;
using System.Runtime.InteropServices;

namespace Microsoft.Azure.Kinect.Sensor
{
    [Native.NativeReference("k4abt_sensor_orientation_t ")]
    public enum SensorOrientation
    {
        /// <summary>
        /// Mount the sensor at its default orientation.
        /// </summary>
        K4ABT_SENSOR_ORIENTATION_DEFAULT = 0,
        /// <summary>
        /// Clockwisely rotate the sensor 90 degree.
        /// </summary>
        K4ABT_SENSOR_ORIENTATION_CLOCKWISE90,
        /// <summary>
        /// Counter-clockwisely rotate the sensor 90 degrees.
        /// </summary>
        K4ABT_SENSOR_ORIENTATION_COUNTERCLOCKWISE90,
        /// <summary>
        /// Mount the sensor upside-down.
        /// </summary>
        K4ABT_SENSOR_ORIENTATION_FLIP180,

    }
}
