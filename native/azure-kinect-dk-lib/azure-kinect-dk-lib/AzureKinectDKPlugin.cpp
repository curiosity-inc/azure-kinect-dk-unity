// azure-kinect-dk-lib.cpp : DLL アプリケーション用にエクスポートされる関数を定義します。
//

#include "stdafx.h"
#include "stdlib.h"
#include "AzureKinectDKPlugin.h"
#include <k4a/k4a.h>
#include <k4abt.h>
#include <memory>
#include <thread>
#include <map>

bool flag = false;
static std::map<int, int> trackingThreadMap;

void ReadBodyTrackingData(k4a_device_t device, k4abt_tracker_t tracker, BodyTrackingCallbackPtr callback) {
	int frame_count = 0;
	while (flag) {
		k4a_capture_t sensor_capture;
		k4a_wait_result_t get_capture_result = k4a_device_get_capture(device, &sensor_capture, K4A_WAIT_INFINITE);
		if (get_capture_result == K4A_WAIT_RESULT_SUCCEEDED)
		{
			frame_count++;
			k4a_wait_result_t queue_capture_result = k4abt_tracker_enqueue_capture(tracker, sensor_capture, K4A_WAIT_INFINITE);
			k4a_capture_release(sensor_capture);
			if (queue_capture_result == K4A_WAIT_RESULT_TIMEOUT)
			{
				// It should never hit timeout when K4A_WAIT_INFINITE is set.
				printf("Error! Add capture to tracker process queue timeout!\n");
				break;
			}
			else if (queue_capture_result == K4A_WAIT_RESULT_FAILED)
			{
				printf("Error! Add capture to tracker process queue failed!\n");
				break;
			}

			k4abt_frame_t body_frame = NULL;
			k4a_wait_result_t pop_frame_result = k4abt_tracker_pop_result(tracker, &body_frame, K4A_WAIT_INFINITE);
			if (pop_frame_result == K4A_WAIT_RESULT_SUCCEEDED)
			{
				size_t num_bodies = k4abt_frame_get_num_bodies(body_frame);
				printf("%zu bodies are detected!\n", num_bodies);
				callback(body_frame);
				k4abt_frame_release(body_frame);
			}
			else if (pop_frame_result == K4A_WAIT_RESULT_TIMEOUT)
			{
				//  It should never hit timeout when K4A_WAIT_INFINITE is set.
				printf("Error! Pop body frame result timeout!\n");
				break;
			}
			else
			{
				printf("Pop body frame result failed!\n");
				break;
			}
		}
		else if (get_capture_result == K4A_WAIT_RESULT_TIMEOUT)
		{
			// It should never hit time out when K4A_WAIT_INFINITE is set.
			printf("Error! Get depth frame time out!\n");
			break;
		}
		else
		{
			printf("Get depth capture returned error: %d\n", get_capture_result);
			break;
		}
	}
}

AZUREKINECTDKLIB_API void* OpenDevice() {
	k4a_device_t device = nullptr;
	k4a_device_open(0, &device);

	// Start camera. Make sure depth camera is enabled.
	k4a_device_configuration_t deviceConfig = K4A_DEVICE_CONFIG_INIT_DISABLE_ALL;
	deviceConfig.depth_mode = K4A_DEPTH_MODE_WFOV_2X2BINNED;
	deviceConfig.color_resolution = K4A_COLOR_RESOLUTION_OFF;
	k4a_device_start_cameras(device, &deviceConfig);
	return device;
}

AZUREKINECTDKLIB_API void* CreateTracker(void* pDevice) {
	k4a_device_t device = static_cast<k4a_device_t>(pDevice);
	k4a_calibration_t sensor_calibration;
	k4a_device_get_calibration(device, K4A_DEPTH_MODE_WFOV_2X2BINNED, K4A_COLOR_RESOLUTION_OFF, &sensor_calibration);

	k4abt_tracker_t tracker = nullptr;
	k4abt_tracker_create(&sensor_calibration, &tracker);
	return tracker;
}

AZUREKINECTDKLIB_API int CloseDevice(void* pDevice, void* pTracker) {
	k4a_device_t device = static_cast<k4a_device_t>(pDevice);
	k4abt_tracker_t tracker = static_cast<k4abt_tracker_t>(pTracker);
	k4abt_tracker_shutdown(tracker);
	k4abt_tracker_destroy(tracker);
	k4a_device_stop_cameras(device);
	k4a_device_close(device);
	return 0;
}

AZUREKINECTDKLIB_API int StartTracking(void* pDevice, void* pTracker, BodyTrackingCallbackPtr callback) {
	k4a_device_t device = static_cast<k4a_device_t>(pDevice);
	k4abt_tracker_t tracker = static_cast<k4abt_tracker_t>(pTracker);
	std::thread t(ReadBodyTrackingData, device, tracker, callback);
	t.detach();
	return 0;
}

AZUREKINECTDKLIB_API int StopTracking(void* device, void* tracker) {
	return 0;
}

AZUREKINECTDKLIB_API int GetNumBodies(void* pBodyFrame) {
	k4abt_frame_t body_frame = static_cast<k4abt_frame_t>(pBodyFrame);
	return k4abt_frame_get_num_bodies(body_frame);
}

AZUREKINECTDKLIB_API void GetBodySkeleton(void* pBodyFrame, int index, void* output) {
	k4abt_frame_t body_frame = static_cast<k4abt_frame_t>(pBodyFrame);
	k4abt_skeleton_t skeleton;
	k4abt_frame_get_body_skeleton(body_frame, index, &skeleton);
	memcpy(output, &skeleton, sizeof(skeleton));
	return;
}

AZUREKINECTDKLIB_API unsigned int GetBodyId(void* pBodyFrame, int index) {
	k4abt_frame_t body_frame = static_cast<k4abt_frame_t>(pBodyFrame);
	return k4abt_frame_get_body_id(body_frame, index);
}
