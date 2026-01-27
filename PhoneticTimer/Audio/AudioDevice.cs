using System;

namespace PhoneticTimer.Audio
{
    public class AudioDevice
    {
        public int DeviceNumber { get; set; }
        public string ProductName { get; set; }
        public int Channels { get; set; }
        public string DriverVersion { get; set; }

        public AudioDevice(int deviceNumber, string productName, int channels)
        {
            DeviceNumber = deviceNumber;
            ProductName = productName;
            Channels = channels;
        }

        public override string ToString()
        {
            return $"{ProductName} (Device {DeviceNumber})";
        }
    }
}
