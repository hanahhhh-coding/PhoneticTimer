using PhoneticTimer.Audio;
using PhoneticTimer.Common;
using PhoneticTimer.Record;

namespace PhoneticTimer
{
    internal class MainWindowViewModel : Notifiable
    {
        private List<AudioDevice> availabeDevices;
        public List<AudioDevice> AvailableDevices
        {
            get => availabeDevices;
            set
            {
                if (AvailableDevices == value)
                {
                    return;
                }

                availabeDevices = value;
                OnPropertyChanged();
            }
        }

        private AudioDevice selectedDevice;
        public AudioDevice SelectedDevice
        {
            get => selectedDevice;
            set
            {
                if (selectedDevice == value)
                    return;
                selectedDevice = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(CanStart));
            }
        }

        public AudioProvider AudioProvider { get; set; }
        public VoiceDetector VoiceDetector { get; set; }
        public RecordItem RecordItem { get; set; }

        public string? TotalPhoneticTime
        {
            get
            {
                var span = RecordItem?.TotalPhoneticTime;
                if (span == null)
                {
                    span = TimeSpan.FromTicks(0);
                }

                return ToReadableString(span.Value);
            }
        }

        private static string ToReadableString(TimeSpan span)
        {
            return string.Format("{1:D2}:{2:D2}:{3:D2}", span.Days, span.Hours, span.Minutes, span.Seconds);
        }

        public bool CanStart
        {
            get => SelectedDevice != null;
        }

        public bool CanStop
        {
            get => AudioProvider != null;
        }


        public MainWindowViewModel()
        {
            RefreshAvailableDevices();
        }

        public void Start()
        {
            if (SelectedDevice == null)
            {
                return;
            }

            if(AudioProvider != null)
            {
                Stop();
            }

            AudioProvider = new AudioProvider(SelectedDevice.DeviceNumber);
            RecordItem = new RecordItem()
            {
                SessionStart = DateTime.Now
            };
            VoiceDetector = new VoiceDetector();
            AudioProvider.AudioCaptured += AudioProvider_AudioCaptured;
            AudioProvider.Start();
            OnPropertyChanged(nameof(CanStop));
        }

        private void AudioProvider_AudioCaptured(object? sender, byte[] e)
        {
            Task.Run(() =>
            {
                if (RecordItem != null && VoiceDetector.HasVoice(e))
                {
                    RecordItem.AddSample(AudioProvider.SampleRate, AudioProvider.ChannelCount, e.Length, AudioProvider.BytesPerSample);
                    OnPropertyChanged(nameof(TotalPhoneticTime));
                }
            });
        }

        public void Stop()
        {
            try
            {
                if (AudioProvider != null)
                    AudioProvider.AudioCaptured -= AudioProvider_AudioCaptured;
                AudioProvider?.Stop();
                AudioProvider?.Dispose();
                AudioProvider = null;
                VoiceDetector?.Dispose();
                VoiceDetector = null;
            }
            catch(Exception exception)
            {
                Console.WriteLine(exception.Message);
            }
        }

        public void RefreshAvailableDevices()
        {
            Stop();
            AvailableDevices = AudioDeviceManager.GetAllAudioDevices();
            SelectedDevice = null;
        }
    }
}
