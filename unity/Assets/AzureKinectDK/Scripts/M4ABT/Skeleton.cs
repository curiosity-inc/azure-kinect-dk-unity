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


    [Native.NativeReference("k4abt_joint_confidence_level_t")]
    public enum JointConfidenceLevel
    {
        /// <summary>
        /// The joint is out of range (too far from depth camera)
        /// </summary>
        K4ABT_JOINT_CONFIDENCE_NONE = 0,
        /// <summary>
        /// The joint is not observed (likely due to occlusion), predicted joint pose.
        /// </summary>
        K4ABT_JOINT_CONFIDENCE_LOW,
        /// <summary>
        /// Medium confidence in joint pose.
        /// 
        /// Current SDK will only provide joints up to this confidence level
        /// </summary>
        K4ABT_JOINT_CONFIDENCE_MEDIUM,
        /// <summary>
        /// High confidence in joint pose.
        /// 
        /// Placeholder for future SDK
        /// </summary>
        K4ABT_JOINT_CONFIDENCE_HIGH,
        /// <summary>
        /// The total number of confidence levels.
        /// </summary>
        K4ABT_JOINT_CONFIDENCE_LEVELS_COUNT
    }

    [StructLayout(LayoutKind.Sequential)]
    [Native.NativeReference("k4abt_joint_t")]
    public struct Joint
    {
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)]
        public float[] Position;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
        public float[] Orientation;

        public JointConfidenceLevel confidence_level;
    }
}
