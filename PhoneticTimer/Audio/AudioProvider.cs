using NAudio.Wave;
using System;
using System.Collections.Generic;
using System.Text;

namespace PhoneticTimer.Audio
{
    public class AudioProvider: IDisposable
    {
        public event EventHandler<byte[]> AudioCaptured;

        private WaveInEvent waveIn;
        private bool isRecording;

        public AudioProvider(int deviceNumber = 0)
        {
            waveIn = new WaveInEvent
            {
                DeviceNumber = deviceNumber,
                WaveFormat = new WaveFormat(16000, 1) // 16kHz, mono
            };

            waveIn.DataAvailable += OnDataAvailable;
        }

        private void OnDataAvailable(object sender, WaveInEventArgs e)
        {
            if (e.BytesRecorded > 0)
            {
                byte[] audioData = new byte[e.BytesRecorded];
                Buffer.BlockCopy(e.Buffer, 0, audioData, 0, e.BytesRecorded);
                AudioCaptured?.Invoke(this, audioData);
            }
        }

        public void Start()
        {
            if (!isRecording)
            {
                waveIn.StartRecording();
                isRecording = true;
            }
        }

        public void Stop()
        {
            if (isRecording)
            {
                waveIn.StopRecording();
                isRecording = false;
            }
        }

        public void Dispose()
        {
            Stop();

            if (waveIn != null)
            {
                waveIn.DataAvailable -= OnDataAvailable;
                waveIn.Dispose();
                waveIn = null;
            }
        }

        public static int GetDeviceCount()
        {
            return WaveInEvent.DeviceCount;
        }

        public static string GetDeviceName(int deviceNumber)
        {
            return WaveInEvent.GetCapabilities(deviceNumber).ProductName;
        }
    }
}
