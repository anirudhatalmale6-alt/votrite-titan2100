using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Speech;
using System.Speech.Recognition;
using System.Text;
using System.Windows.Forms;
using VotRite.Util;

namespace VotRite.JimForms
{
    public partial class BallotMode : Form
    {
        public bool normal = true;
        public BallotMode()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (radioButton2.Checked)
            {

                if (MessageBox.Show("Please confirm you want the Visually Impaired Mode?" + Environment.NewLine + Environment.NewLine +
                    "If you plan to write-in a candidate, please proceed with the normal ballot." + Environment.NewLine + " You may request a supportive person for assistance.", "Votrite Console", MessageBoxButtons.YesNo, MessageBoxIcon.Question) ==
                        DialogResult.Yes)
                {
                    IOHandler.SaveConfig("Speech", "Rate", this.hsbarSpeechRate.Value.ToString());
                    AppManager.Configuration = IOHandler.DecryptConfig();
                    normal = false;
                }
            }
            this.Close();

        }

        private void label1_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void BallotMode_Load(object sender, EventArgs e)
        {
            //SetSpeechToTextEngine();
            try
            {
                string sRate = AppManager.Configuration["Speech"]["Rate"];
                if (sRate != "" && sRate == "-1")
                {
                    hsbarSpeechRate.Value = Convert.ToInt32(sRate);
                    lblRateText.Text = "Speech Rate: "+ sRate;
                }
            }
            catch (Exception)
            {
            }
        }
        public SpeechRecognitionEngine speechRecognitionEngine { get; set; }
        public void SetSpeechToTextEngine()
        {
            try
            {
                // create the engine
                speechRecognitionEngine = createSpeechEngine("en-US");

                // hook to events
                speechRecognitionEngine.AudioLevelUpdated += new EventHandler<AudioLevelUpdatedEventArgs>(engine_AudioLevelUpdated);
                speechRecognitionEngine.SpeechRecognized += new EventHandler<SpeechRecognizedEventArgs>(engine_SpeechRecognized);
                //speechRecognitionEngine.RecognizeCompleted += SpeechRecognitionEngine_RecognizeCompleted;
                //speechRecognitionEngine.SpeechDetected += SpeechRecognitionEngine_SpeechDetected;
                //speechRecognitionEngine.SpeechHypothesized += SpeechRecognitionEngine_SpeechHypothesized;
                //speechRecognitionEngine.SpeechRecognitionRejected += SpeechRecognitionEngine_SpeechRecognitionRejected;
                // load dictionary
                loadGrammarAndCommands();
                // use the system's default microphone
                speechRecognitionEngine.SetInputToDefaultAudioDevice();

                // start listening
                speechRecognitionEngine.RecognizeAsync(RecognizeMode.Multiple);
            }
            catch (Exception ex)
            {
                //MessageBox.Show(ex.Message, "Voice recognition failed");
            }
        }

        private SpeechRecognitionEngine createSpeechEngine(string preferredCulture)
        {
            foreach (RecognizerInfo config in SpeechRecognitionEngine.InstalledRecognizers())
            {
                if (config.Culture.ToString() == preferredCulture)
                {
                    speechRecognitionEngine = new SpeechRecognitionEngine(config);
                    break;
                }
            }

            // if the desired culture is not found, then load default
            if (speechRecognitionEngine == null)
            {
                MessageBox.Show("The desired culture is not installed on this machine, the speech-engine will continue using "
                    + SpeechRecognitionEngine.InstalledRecognizers()[0].Culture.ToString() + " as the default culture.",
                    "Culture " + preferredCulture + " not found!");
                speechRecognitionEngine = new SpeechRecognitionEngine(SpeechRecognitionEngine.InstalledRecognizers()[0]);
            }

            return speechRecognitionEngine;
        }

        private void loadGrammarAndCommands()
        {
            try
            {
                Choices texts = new Choices();
                string[] lines = File.ReadAllLines(Environment.CurrentDirectory + "\\example.txt");
                foreach (string line in lines)
                {
                    //// skip commentblocks and empty lines..
                    //if (line.StartsWith("--") || line == String.Empty) continue;

                    //// split the line
                    //var parts = line.Split(new char[] { '|' });

                    // add the text to the known choices of speechengine
                    texts.Add(line);
                }
                Grammar wordsList = new Grammar(new GrammarBuilder(texts));
               // speechRecognitionEngine.LoadGrammar(wordsList);
                speechRecognitionEngine.LoadGrammar(new Grammar(new GrammarBuilder("exit")));
                speechRecognitionEngine.LoadGrammar(new DictationGrammar());
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        void engine_AudioLevelUpdated(object sender, AudioLevelUpdatedEventArgs e)
        {
            //prgLevel.Value = e.AudioLevel;
        }
        private string getKnownTextOrExecute(string command)
        {
            //try
            //{
            //    var cmd = words.Where(c => c.Text == command).First();

            //    if (cmd.IsShellCommand)
            //    {
            //        Process proc = new Process();
            //        proc.EnableRaisingEvents = false;
            //        proc.StartInfo.FileName = cmd.AttachedText;
            //        proc.Start();
            //        return "you just started : " + cmd.AttachedText;
            //    }
            //    else
            //    {
            //        return cmd.AttachedText;
            //    }
            //}
            //catch (Exception)
            //{
            //    return command;
            //}
            return command;
        }

        /// <summary>
        /// Handles the SpeechRecognized event of the engine control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Speech.Recognition.SpeechRecognizedEventArgs"/> instance containing the event data.</param>
        void engine_SpeechRecognized(object sender, SpeechRecognizedEventArgs e)
        {
            //e.Result.Text is the text that is recognized by engine ..from grammer added
            //txtSpoken.Text += "\r" + getKnownTextOrExecute(e.Result.Text);
            //scvText.ScrollToEnd();
            lblSpeak.Text = "You said" + e.Result.Text;
        }

        private void radioButton2_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButton2.Checked)
                panelSpeech.Visible = true;
            else
                panelSpeech.Visible = false;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            try
            {
                
                AppManager.Instance.StartSpeaker_preview("Speech rate "+ this.hsbarSpeechRate.Value+ ". You are testing the speech rate for the Visually Impaired Mode.");
            }
            catch (Exception)
            {
            }
        }

        private void hsbarSpeechRate_ValueChanged(object sender, EventArgs e)
        {
            lblRateText.Text = "Speech Rate: " + hsbarSpeechRate.Value.ToString();
        }
    }
}
