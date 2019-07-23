#pragma once
#ifdef AZUREKINECTDKLIB_EXPORTS
#define AZUREKINECTDKLIB_API __declspec(dllexport)
#else
#define AZUREKINECTDKLIB_API __declspec(dllimport)
#endif

extern "C" {
	AZUREKINECTDKLIB_API void* OpenDevice();
	AZUREKINECTDKLIB_API void* CreateTracker(void* device);
	AZUREKINECTDKLIB_API int CloseDevice(void* device, void* tracker);
}
