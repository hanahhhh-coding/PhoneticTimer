using NAudio.Wave;
using System;
using System.Collections.Generic;

namespace PhoneticTimer.Audio
{
    public static class AudioDeviceManager
    {
        public static List<AudioDevice> GetAllAudioDevices()
        {
            var devices = new List<AudioDevice>();
            int deviceCount = WaveInEvent.DeviceCount;

            for (int i = 0; i < deviceCount; i++)
            {
                var capabilities = WaveInEvent.GetCapabilities(i);
                var device = new AudioDevice(
                    deviceNumber: i,
                    productName: capabilities.ProductName,
                    channels: capabilities.Channels
                );
                devices.Add(device);
            }

            return devices;
        }
    }
}
