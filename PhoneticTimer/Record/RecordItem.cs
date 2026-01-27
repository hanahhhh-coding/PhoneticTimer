using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;

namespace PhoneticTimer.Record
{
    internal class RecordItem
    {
        public DateTime SessionStart { get; set; }
        public TimeSpan TotalPhoneticTime { get; set; }
        public ConcurrentStack<SampleDescriptorItem> Samples { get; set; }

        public RecordItem()
        {
            Samples = new ConcurrentStack<SampleDescriptorItem>();
        }

        public void AddSample(int sampleRate, int channelCount, int size, int bytesPerSample)
        {
            var length = GetSampleMs(sampleRate, channelCount, size, bytesPerSample);
            TotalPhoneticTime += TimeSpan.FromMilliseconds(length);
            Samples.Push(new SampleDescriptorItem()
            {
                DiffInSeconds = (DateTime.Now - SessionStart).TotalSeconds,
                SampleLengthInMs = length,
            });
        }

        private int GetSampleMs(int sampleRate, int channelCount, int size, int bytesPerSample)
        {
            // Calculate total number of samples
            int totalSamples = size / (bytesPerSample * channelCount);

            // Calculate duration in milliseconds
            double durationMs = (totalSamples / (double)sampleRate) * 1000;

            return (int)Math.Round(durationMs);
        }
    }
}
