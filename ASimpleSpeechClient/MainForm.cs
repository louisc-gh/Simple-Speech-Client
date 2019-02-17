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

namespace SimpleSpeechClient
{

    public partial class MainForm : Form
    {

        private ISimpleSpeechRecognizer rec = null;

        /// <summary>
        /// 
        /// </summary>
        public MainForm()
        {
            // this is a new comment
            InitializeComponent();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void OnStartRecognitionClicked(object sender, EventArgs e)
        {
            try
            {
                // create an instance of the speech recognizer
                rec = new SimpleSpeechRecognizer();

                // start the speech client
                var startClientResult = rec.StartSpeechClient();

                if (0 != startClientResult)
                    return;

                transcriptTextControl.Text = "";

                rec.TranscriptAvailable += OnTranscriptAvaible;

                // configure the speech request
                await rec.ConfigureSpeechRequest(RecognitionConfig.Types.AudioEncoding.Linear16, 16000, "en", false);

                // configure the audio device (mic.)
                var configAudioResult = rec.ConfigureAudioDevice(0, 16000, 1);

                if (0 != configAudioResult)
                    return;

                // start the speech recognition
                var beginResult = rec.BeginSpeechRecognition();

#pragma warning disable CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
                rec.ProcessResponses();
#pragma warning restore CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed

                startRecognitionButton.Enabled = false;
            }
            catch (Exception)
            {
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void OnTranscriptAvaible(object sender, TranscriptAvailableEventArgs e)
        {
            transcriptTextControl.Text += e.Transcript;
            transcriptTextControl.Text += ". ";
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Form1_FormClosing(object sender, System.Windows.Forms.FormClosingEventArgs e)
        {
            // prevent close if we're interacting
            if (!startRecognitionButton.Enabled)
                e.Cancel = true;
        }

        private async void stopRecognitionButton_Click(object sender, EventArgs e)
        {
            if (null == rec)
                return;

            await rec.StopSpeechRecognition();
            await rec.StopSpeechClient();
            startRecognitionButton.Enabled = true;
        }
    }
}
