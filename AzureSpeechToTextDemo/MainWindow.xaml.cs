using AzureSpeechToTextDemo.Code;
using Microsoft.CognitiveServices.Speech;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace AzureSpeechToTextDemo
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private bool _recording = false;
        private SpeechRecognizer _recognizer;

        public MainWindow()
        {
            InitializeComponent();
            InitializeSpeechRecognizer();
        }

        private void InitializeSpeechRecognizer()
        {
            // Set up Azure Speech API key and region
            string apiKey = GlobalConst.AzureSpeechKey;
            string region = GlobalConst.AzureSpeechRegion;

            var speechConfig = SpeechConfig.FromSubscription(apiKey, region);

            // Set the language to German
            speechConfig.SpeechRecognitionLanguage = GlobalConst.AzureSpeechLanguage;

            // Create recognizer
            _recognizer = new SpeechRecognizer(speechConfig);
            _recognizer.Recognized += Recognizer_Recognized;
            _recognizer.Canceled += Recognizer_Canceled;
            _recognizer.SessionStopped += Recognizer_SessionStopped;
        }

        private void StartStopButton_Click(object sender, RoutedEventArgs e)
        {
            _recording = !_recording;

            if (_recording)
            {
                // Start recording
                StartStopButton.Content = "Stop";
                StatusLabel.Content = "RECORDING";
                StatusLabel.Foreground = GlobalConst.RedBrush;
                StartRecording();
            }
            else
            {
                // Stop recording
                StartStopButton.Content = "Start";
                StatusLabel.Content = "Converting";
                StatusLabel.Foreground = GlobalConst.BlackBrush;
                StopRecording();
            }
        }

        private async void StartRecording()
        {
            // Start Speech recognition
            await _recognizer.StartContinuousRecognitionAsync();
        }

        private void StopRecording()
        {
            // Stop Speech recognition
            _recognizer.StopContinuousRecognitionAsync();
        }

        private void Recognizer_Recognized(object sender, SpeechRecognitionEventArgs e)
        {
            if (e.Result.Reason == ResultReason.RecognizedSpeech)
            {
                // Update the TextBox with recognized speech
                Dispatcher.Invoke(() =>
                {
                    Result.Text = e.Result.Text;
                });
            }
        }

        private void Recognizer_Canceled(object sender, SpeechRecognitionCanceledEventArgs e)
        {
            if (e.Reason == CancellationReason.Error)
            {
                MessageBox.Show($"Speech Recognition Error: {e.ErrorDetails}");
            }
        }

        private void Recognizer_SessionStopped(object sender, SessionEventArgs e)
        {
            Dispatcher.Invoke(() =>
            {
                StatusLabel.Content = "Ready";
            });
        }
    }
}
