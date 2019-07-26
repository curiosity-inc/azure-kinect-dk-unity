using Microsoft.Azure.Kinect.Sensor;
using Microsoft.Azure.Kinect.Sensor.BodyTracking;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace k4atest
{
    class Program
    {
        private static bool running;

        static async Task Main(string[] args)
        {
            System.Diagnostics.Debug.WriteLine("Open Device");
            using (Device device = Device.Open(0))
            {
                System.Diagnostics.Debug.WriteLine("Start Camera");
                var config = new DeviceConfiguration
                {
                    //ColorResolution = ColorResolution.Off,
                    DepthMode = DepthMode.NFOV_Unbinned
                };
                device.StartCameras(config);

                var calibration = device.GetCalibration(config.DepthMode, config.ColorResolution);
                using (BodyTracker tracker = BodyTracker.Create(calibration))
                {

                    int frameCount = 0;
                    while (frameCount < 3000)
                    {
                        // System.Diagnostics.Debug.WriteLine("Start Capture");
                        using (Capture capture = await Task.Run(() => device.GetCapture()).ConfigureAwait(true))
                        {
                            var img = capture.Color;
                            System.Diagnostics.Debug.WriteLine(img == null);
                            System.Diagnostics.Debug.WriteLine(capture.Temperature);
                            // capture.
                            // await Task.Run(() => tracker.EnqueueCapture(capture)).ConfigureAwait(true);
                            //using (BodyFrame frame = await Task.Run(() => tracker.PopResult()).ConfigureAwait(true))
                            //{
                            //    System.Diagnostics.Debug.WriteLine(frame.NumBodies);
                            //}
                        }
                        frameCount++;
                    }
                }
            }
        }
    }
}
