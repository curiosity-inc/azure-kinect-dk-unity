#include "pch.h"
#include <stdio.h>
#include <stdlib.h>

#include <k4a/k4a.h>
#include <k4abt.h>

#define PUB_HANDLE_TYPE(type) type##_wrapper_##_cpp

typedef enum
{
	IMAGE_TYPE_COLOR = 0,
	IMAGE_TYPE_DEPTH,
	IMAGE_TYPE_IR,
	IMAGE_TYPE_COUNT,
} image_type_index_t;

typedef void* LOCK_HANDLE;

typedef struct _capture_context_t
{
	volatile long ref_count;
	LOCK_HANDLE lock;

	k4a_image_t image[IMAGE_TYPE_COUNT];

	float temperature_c; /** Temperature in Celsius */
} capture_context_t;

typedef struct PUB_HANDLE_TYPE(k4a_capture_t)                                                               \
{                                                                                                                  \
char *handleType;                                                                                              \
capture_context_t context;                                                                               \
} PUB_HANDLE_TYPE(k4a_capture_t);

#define VERIFY(result, error)                                                                            \
    if(result != K4A_RESULT_SUCCEEDED)                                                                   \
    {                                                                                                    \
        printf("%s \n - (File: %s, Function: %s, Line: %d)\n", error, __FILE__, __FUNCTION__, __LINE__); \
        exit(1);                                                                                         \
    }                                                                                                    \

int main()
{
	k4a_device_configuration_t deviceConfig = K4A_DEVICE_CONFIG_INIT_DISABLE_ALL;
	deviceConfig.depth_mode = K4A_DEPTH_MODE_NFOV_UNBINNED;

	k4a_device_t device;
	VERIFY(k4a_device_open(0, &device), "Open K4A Device failed!");
	VERIFY(k4a_device_start_cameras(device, &deviceConfig), "Start K4A cameras failed!");

	k4a_calibration_t sensor_calibration;
	VERIFY(k4a_device_get_calibration(device, deviceConfig.depth_mode, deviceConfig.color_resolution, &sensor_calibration),
		"Get depth camera calibration failed!");

	k4abt_tracker_t tracker = NULL;
	VERIFY(k4abt_tracker_create(&sensor_calibration, &tracker), "Body tracker initialization failed!");

	int frame_count = 0;
	do
	{
		k4a_capture_t sensor_capture;
		k4a_wait_result_t get_capture_result = k4a_device_get_capture(device, &sensor_capture, K4A_WAIT_INFINITE);
		if (get_capture_result == K4A_WAIT_RESULT_SUCCEEDED)
		{
			//
			auto a = ((PUB_HANDLE_TYPE(k4a_capture_t) *)sensor_capture)->handleType;
			// k4a_capture_t_get_context(sensor_capture);
			//capture_dec_ref(sensor_capture);
			//auto img = k4a_capture_get_color_image(sensor_capture);
			//auto size = k4a_image_get_size(img);
			//printf("[%s] %d\n", a, size);
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
				if (num_bodies > 0) {
					k4abt_skeleton_t skeleton;
					k4abt_frame_get_body_skeleton(body_frame, 0, &skeleton);
					for (int i = 0; i < 26; i++) {
						auto j = skeleton.joints[i];
						printf("%f, %f, %f\n", j.position.v[0], j.position.v[1], j.position.v[2]);
					}
				}

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

	} while (frame_count < 100);

	printf("Finished body tracking processing!\n");

	k4abt_tracker_shutdown(tracker);
	k4abt_tracker_destroy(tracker);
	k4a_device_stop_cameras(device);
	k4a_device_close(device);

	return 0;
}
