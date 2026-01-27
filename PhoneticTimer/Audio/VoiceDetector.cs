using WebRtcVadSharp;

namespace PhoneticTimer.Audio
{
    internal class VoiceDetector: IDisposable
    {
        private WebRtcVad rtcVad;

        public VoiceDetector()
        {
            rtcVad = new WebRtcVad();
        }

        public bool HasVoice(byte[] buffer)
        {
            try
            {
                return rtcVad.HasSpeech(buffer);
            }
            catch(Exception exc)
            {
                Console.WriteLine(exc.Message);
            }

            return false;
        }

        public void Dispose()
        {
            rtcVad?.Dispose();
            rtcVad = null;
        }
    }
}
