# azure-kinect-dk-unity

Unity example to use Azure Kinect DK (works with both Sensor SDK and Body Tracking SDK).

![Screenshot](screenshot.jpg?raw=true "ScreenShot")

## Tested environment
As of July 27, 2019, this repo has been tested under the following environment.
- Windows 10 Pro (ver 1803)
- [Azure Kinect Sensor SDK](https://docs.microsoft.com/ja-jp/azure/Kinect-dk/sensor-sdk-download) v1.1.0
- [Azure Kinect Body Tracking SDK](https://docs.microsoft.com/ja-jp/azure/Kinect-dk/body-sdk-download) v0.9.0
- Unity 2019.1.11f1
- CUDA v10.0
- cudnn v7.5.1.10

## Get Started
1. Clone this repo.
2. Copy following files from Azure Kinect Sensor SDK or Azure Kinect Body Tracking SDK to *directly under the unity folder*
- k4a.dll
- k4abt.dll
- onnxruntime.dll
- depthengine_1_0.dll
- dnn_model.onnx
3. Open Assets/AzureKinectDK/Example/Scenes/SampleScene
4. Play the scene and see how it works.

## License
Files under `unity/Assets/AzureKinectDK/Scripts/M4A` are copied from https://github.com/microsoft/Azure-Kinect-Sensor-SDK and modified by Takahiro Horikawa which is licensed under MIT License.
Other files are licensed under MIT license.