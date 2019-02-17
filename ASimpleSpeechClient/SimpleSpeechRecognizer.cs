using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Google.Cloud.Speech.V1;
using Google.Apis.Auth.OAuth2;
using Grpc.Auth;
using System.IO;
using System.Threading;
using NAudio.Wave;

namespace SimpleSpeechClient
{
    interface ISimpleSpeechRecognizer
    {
        int StartSpeechClient();
        Task ConfigureSpeechRequest(RecognitionConfig.Types.AudioEncoding encoding, int sampleRateHertz, string languageCode, bool interimResults);
        int ConfigureAudioDevice(int deviceNumber, int waveFormatSampleRate, int numChannels);
        Task BeginSpeechRecognition();
        Task StopSpeechRecognition();
        void OnDataAvailable(object sender, NAudio.Wave.WaveInEventArgs args);
        Task ProcessResponses();
        event EventHandler<TranscriptAvailableEventArgs> TranscriptAvailable;
        Task StopSpeechClient();
    }

    /// <summary>
    /// 
    /// </summary>
    public class TranscriptAvailableEventArgs : EventArgs
    {
        public string Transcript { get; set; }
    }

    /// <summary>
    /// 
    /// </summary>
    class SimpleSpeechRecognizer : ISimpleSpeechRecognizer
    {
        private object writeLock = new object();

        /// <summary>
        /// 
        /// </summary>
        private bool writeMore = true;

        /// <summary>
        /// 
        /// </summary>
        private SpeechClient.StreamingRecognizeStream streamingCall = null;

        /// <summary>
        /// 
        /// </summary>
        private NAudio.Wave.WaveInEvent waveIn = null;
        
        /// <summary>
        /// 
        /// </summary>
        private SpeechClient speechClient = null;

        /// <summary>
        /// request
        /// </summary>
        private StreamingRecognizeRequest request = null;

        /// <summary>
        /// 
        /// </summary>
        public event EventHandler<TranscriptAvailableEventArgs> TranscriptAvailable;

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public int StartSpeechClient()
        {
            var retVal = -1;
            try
            {
                speechClient = SpeechClient.Create();
                retVal = 0;
            }
            catch(Exception)
            {
                retVal = 1;
            }

            return retVal;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="encoding"></param>
        /// <param name="sampleRateHertz"></param>
        /// <param name="languageCode"></param>
        /// <param name="intermResults"></param>
        public async Task ConfigureSpeechRequest(
            RecognitionConfig.Types.AudioEncoding encoding,
            int sampleRateHertz,
            string languageCode,
            bool intermResults)
        {

            if (null == speechClient)
                return;

            streamingCall = speechClient.StreamingRecognize();

            request = new StreamingRecognizeRequest()
            {
                StreamingConfig = new StreamingRecognitionConfig()
                {
                    Config = new RecognitionConfig()
                    {
                        Encoding = encoding,
                        SampleRateHertz = sampleRateHertz,
                        LanguageCode = languageCode,
                    },

                    InterimResults = intermResults,
                }
            };

            // Write the initial request with the config.
            await streamingCall.WriteAsync(request);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="deviceNumber"></param>
        /// <param name="waveFormatSampleRate"></param>
        /// <param name="numChannels"></param>
        /// <returns></returns>
        public int ConfigureAudioDevice(int deviceNumber, int waveFormatSampleRate, int numChannels)
        {
            if (NAudio.Wave.WaveIn.DeviceCount < 1)
            {
                Console.WriteLine("No microphone!");
                return -1;
            }

            waveIn = new NAudio.Wave.WaveInEvent();
            waveIn.DeviceNumber = deviceNumber;
            waveIn.WaveFormat = new NAudio.Wave.WaveFormat(waveFormatSampleRate, numChannels);
            waveIn.DataAvailable += this.OnDataAvailable;

            return 0;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public async Task BeginSpeechRecognition()
        {
            if (null == waveIn)
                return;

            waveIn.StartRecording();

            await Task.Run(() =>
                {
                    var startTicks = DateTime.Now;

                    do
                    {
                        var currentTime = DateTime.Now;
                        TimeSpan interval = currentTime - startTicks;

                        if (interval.TotalSeconds >= 55)
                        {
                            lock (writeLock)
                            {
                                streamingCall.WriteCompleteAsync();
#pragma warning disable CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
                                ConfigureSpeechRequest(RecognitionConfig.Types.AudioEncoding.Linear16, 16000, "en", false);
                                ProcessResponses();
#pragma warning restore CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
                            }

                            startTicks = DateTime.Now; 
                        }
                    }
                    while (true);
                }
            );

            return;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public async Task StopSpeechRecognition()
        {
            if (null == waveIn)
                return;

            waveIn.StopRecording();

            lock (writeLock) writeMore = false;
            await streamingCall.WriteCompleteAsync();
        }

        /// <summary>
        /// 
        /// </summary>
        public async Task StopSpeechClient()
        {
            if (null != speechClient)
            {
                await SpeechClient.ShutdownDefaultChannelsAsync();
                speechClient = null;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        public void OnDataAvailable(object sender, WaveInEventArgs args)
        {
            lock (writeLock)
            {
                if (!writeMore) return;

                streamingCall.WriteAsync(
                    new StreamingRecognizeRequest()
                    {
                        AudioContent = Google.Protobuf.ByteString.CopyFrom(args.Buffer, 0, args.BytesRecorded)
                    }).Wait();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public async Task ProcessResponses()
        {
            while (await streamingCall.ResponseStream.MoveNext(default(CancellationToken)))
            {
                foreach (var result in streamingCall.ResponseStream.Current.Results)
                {
                    foreach (var alternative in result.Alternatives)
                    {
                        TranscriptAvailableEventArgs e = new TranscriptAvailableEventArgs { Transcript = alternative.Transcript };
                        TranscriptAvailable?.Invoke(this, e);
                    }
                }
            }
        }
    }
}
