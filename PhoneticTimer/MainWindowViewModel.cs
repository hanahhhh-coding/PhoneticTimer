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
                if(RecordItem?.TotalPhoneticTime == null)
                {
                    return string.Empty;
                }

                return ToReadableString(RecordItem.TotalPhoneticTime);
            }
        }

        private static string ToReadableString(TimeSpan span)
        {
            string formatted = string.Format("{0}{1}{2}{3}",
                span.Duration().Days > 0 ? string.Format("{0:0} day{1}, ", span.Days, span.Days == 1 ? string.Empty : "s") : string.Empty,
                span.Duration().Hours > 0 ? string.Format("{0:0} hour{1}, ", span.Hours, span.Hours == 1 ? string.Empty : "s") : string.Empty,
                span.Duration().Minutes > 0 ? string.Format("{0:0} minute{1}, ", span.Minutes, span.Minutes == 1 ? string.Empty : "s") : string.Empty,
                span.Duration().Seconds > 0 ? string.Format("{0:0} second{1}", span.Seconds, span.Seconds == 1 ? string.Empty : "s") : string.Empty);

            if (formatted.EndsWith(", ")) formatted = formatted.Substring(0, formatted.Length - 2);

            if (string.IsNullOrEmpty(formatted)) formatted = "0 seconds";

            return formatted;
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
