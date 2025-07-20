using Microsoft.CognitiveServices.Speech;
using System.Threading.Tasks;
using CAFEHOLIC.Utils;

namespace CAFEHOLIC.Service
{
    public static class SpeechToText
    {
        private static readonly string AI_Key1 = AppConfig.Get("SpeechToText:Key1");
        private static readonly string region = AppConfig.Get("SpeechToText:Region");

        public static async Task<string> RecognizeOnceAsync()
        {
            var config = SpeechConfig.FromSubscription(AI_Key1, region);
            config.SpeechRecognitionLanguage = "vi-VN";
            config.SetProperty("SPEECH-SegmentationSilenceTimeoutMs", "500");

            using var recognizer = new SpeechRecognizer(config);

            var result = await recognizer.RecognizeOnceAsync();
            if (result.Reason == ResultReason.RecognizedSpeech)
                return result.Text;
            else
                return string.Empty;
        }
    }
}