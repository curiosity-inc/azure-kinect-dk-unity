#pragma once
#ifdef AZUREKINECTDKLIB_EXPORTS
#define AZUREKINECTDKLIB_API __declspec(dllexport)
#else
#define AZUREKINECTDKLIB_API __declspec(dllimport)
#endif

using BodyTrackingCallbackPtr = void(*)(void* bodyFrame);

/*
typedef struct {
	float x;
	float y;
	float z;
} k4a_interop_float3_t;

typedef struct _k4abt_joint_t
{
	k4a_interop_float3_t position;
	k4a_quaternion_t orientation;
} k4abt_joint_t;

typedef struct _k4abt_skeleton_t
{
	k4abt_joint_t joints[K4ABT_JOINT_COUNT];
} k4abt_skeleton_t;
*/

typedef struct a {

} b;

extern "C" {
	AZUREKINECTDKLIB_API void* OpenDevice();
	AZUREKINECTDKLIB_API void* CreateTracker(void* device);
	AZUREKINECTDKLIB_API int CloseDevice(void* device, void* tracker);
	AZUREKINECTDKLIB_API int StartTracking(void* device, void* tracker, BodyTrackingCallbackPtr callback);
	AZUREKINECTDKLIB_API int StopTracking(void* device, void* tracker);
	AZUREKINECTDKLIB_API int GetNumBodies(void* bodyFrame);
	AZUREKINECTDKLIB_API void GetBodySkeleton(void* pBodyFrame, int index, void* output);
	AZUREKINECTDKLIB_API unsigned int GetBodyId(void* bodyFrame, int index);
	AZUREKINECTDKLIB_API int CheckCapture(void* capture);
}
