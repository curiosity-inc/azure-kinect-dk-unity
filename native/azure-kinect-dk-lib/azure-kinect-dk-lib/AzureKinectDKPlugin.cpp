// azure-kinect-dk-lib.cpp : DLL アプリケーション用にエクスポートされる関数を定義します。
//

#include "stdafx.h"
#include "stdlib.h"
#include "AzureKinectDKPlugin.h"
#include <k4a/k4a.h>
#include <k4abt.h>
#include <memory>

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
