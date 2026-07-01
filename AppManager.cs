// Product:	VotRite
// Module:  AppManager.cs
// Author:  Dmitriy Slipak
// 
// Copyright (c) 2017 - VOTRITE INTERNATIONAL LLC. All rights reserved.
// 
// THIS PRODUCT INCLUDES SOFTWARE AS A PART OF VOTRITE VOTING 
// SYSTEM DEVELOPED AT VOTRITE INTERNATIONAL LLC (http://www.votrite.com).
// THE SOURCE CODE FOR THIS PRODUCT IS SUBJECT TO THE VOTRITE INTERNATIONAL LLC 
// LICENSE. NO ANY PORTION OF THIS PRODUCT OR SOURCE CODE CAN BE 
// REDISTRIBUTED UNDER ANY CIRCUMSTANCES.
// 

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Timers;
using System.Windows.Forms;
using VotRite.JimForms;

using SpeechLib;

using VotRite.Controllers;
using VotRite.Definition;
using VotRite.Forms;
using VotRite.Models;
using VotRite.UI;
using VotRite.Util;
using VotRite.Views;
using VotRiteBallotDataManager.AppCode;
using DBBallot = VotRiteBallotDataManager.AppCode.Ballot;
using View = VotRite.MVC.View;

using System.Speech;
using System.Speech.Recognition;

namespace VotRite
{
    internal class AppManager : IDisposable
    {
        private static readonly List<string> CharactersToRemove = new List<string> { "(", ")", "*" };

        private static AppManager instance;

        private static HardPinPad hPinPad;
        //        private static HardCounter counter;
        public static readonly CultureInfo DefaultCultureInfo = new CultureInfo("en-US");

        private string OnScreenKeyboadApplication = "osk.exe";
        private List<VrScreen> activeScreens;
        private AppAgent agent = null;
        private Global.AppState appState;
        private UInt64 _castCounter;
        private UInt64 _castTotalCounter;
        private UInt64 _castedBallotCounter;
        private UInt64 _logCounter;
        private ScreenController controller = null;
       
        private ScreenModel model = null;
        private PrinterHandler ph = null;
        public SpVoice speaker = null;
        private static int _ballotId = -1;

        /// Jim Kapsis 18-06-2016
        /// printer settings        
        private static bool _printerEnabled;
        private static bool _votedSoundEnabled;
        private static bool _showPrintReview;
        private static bool _optionInited = false;

        private static bool printFinalReportStarted = false;

        //private string prevScreen;

        private System.Timers.Timer initialTimer;

        private static SortedList<string, SortedList<string, string>> config = IOHandler.DecryptConfig();

        private int finalReportPrintCounter = 0;

        private int selectedCountyId = 0;

        public int reviewHeaderFontSize = 22;
        public int reviewResultFontSize = 18;
        public int reviewControlHeight = 30;
        public float reportFontSizeFactor = 0f;
        public ReviewController vController = null;
        public VotRite.MVC.Controller controllerForHearingMode = null;
        public int choicesLeft = 0;
        public bool writeinModel = false;
        public bool reviewprinted = false;
        public enum colorTheme
        {
            White,
            Yellow,
            LightYellow,
            Blue,
            LightBlue,
            Green,
            Contrast
        }
        public colorTheme backgroundTheme = colorTheme.White;

        public enum repeatInstructionCommand
        {
            none,repeat, norepeat
        }

        public repeatInstructionCommand repeatInstruction = repeatInstructionCommand.none;

        public enum MouseButton
        {
            none, left, right
        }

        public MouseButton mouseButton = MouseButton.none;

        public enum KeyboardButton
        {
            none, select, next
        }

        public KeyboardButton keyboardButton = KeyboardButton.none;
        public bool reviewOnConfirm = true;
        public bool headerInRecordImage = false;

        public bool PrintFinalReportStarted
        {
            get { return printFinalReportStarted; }
            set { printFinalReportStarted = value; }
        }

        public int FinalReportPrintCounter
        {
            get { return finalReportPrintCounter; }
        }

        public int SelectedCountyId
        {
            get { return selectedCountyId; }
            set { selectedCountyId = value; }
        }

        public HardPinPad HPinPad
        {
            get { return hPinPad;  }
            set { hPinPad = value; }
        }

        /// Jim Kapsis 18-06-2016
        /// Printer Settings
        public static bool PrinterEnabled
        {
            get
            {
                if (!_optionInited)
                {
                    if (!printFinalReportStarted)
                    {
                        if (!bool.TryParse(AppManager.Configuration["Printer"]["Enabled"], out _printerEnabled))
                            _printerEnabled = true;
                    }

                    _optionInited = true;
                }
                return _printerEnabled;
            }
            set { _printerEnabled = value; }
        }

        public static bool VoteSoundEnabled
        {
            get
            {
                ////if (!_optionInited)
                ////{
                _votedSoundEnabled = true;
                if (!bool.TryParse(AppManager.Configuration["VotedSound"]["Enabled"], out _votedSoundEnabled))
                {
                    _votedSoundEnabled = false;
                }

                ////_optionInited = true;
                ////}
                return _votedSoundEnabled;
            }
        }

        public static bool ShowPrintReview
        {
            get
            {
                _showPrintReview = true;
                if (!bool.TryParse(AppManager.Configuration["PrintReview"]["Enabled"], out _showPrintReview))
                {
                    _showPrintReview = false;
                }

                return _showPrintReview;
            }
        }

        public static SortedList<string, SortedList<string, string>> Configuration
        {
            get
            {
                return config;
            }
            set
            {
                config = value;
            }
        }

        public void Dispose()
        {
            DataManager.VotingResultsDataInstance.CloseConnection();
            activeScreens.Clear();
            activeScreens = null;
            if (agent != null) agent.Dispose();
            ph = null;
            model = null;
            speaker = null;
            controller = null;
            hPinPad = null;
            instance = null;
        }

        public static int BallotId
        {
            get
            {
                return _ballotId > 0 ? _ballotId : Ballot.GetDefaultBallotId();
            }
            set
            {
                _ballotId = value;
            }
        }
        /* not needed 
        public string PreviousScreen
        {
            get { return prevScreen; }
            set { prevScreen = value; }
        }
        */
        public AppManager()
        {
            TenantVoiceWeight = 0f;
            activeScreens = new List<VrScreen>();
        }

        public List<VrScreen> ActiveScreens
        {
            get { return activeScreens; }
            set { activeScreens = value; }
        }

        public Session Session { get; private set; }

        // public BallotDefinition ballot { get; private set; }
        public BallotDefinition ballot { get; private set; }

        public static AppManager Instance
        {
            get { return instance ?? (instance = new AppManager()); }
        }

        public Global.AppState State
        {
            get { return appState; }
            set { appState = value; }
        }

        public UInt64 CastCounter
        {
            get { return _castCounter; }
        }

        public UInt64 CastTotalCounter
        {
            get { return _castTotalCounter; }
        }

        public UInt64 CastedBallotsCounter
        {
            get { return _castedBallotCounter; }
        }

        public void ReadingLogFilesCount()
        {
            string[] oFiles = Directory.GetFiles(Application.StartupPath + @"\log");
            _logCounter = (ulong)oFiles.Length;
        }

        public UInt64 LogCounter
        {
            get { return _logCounter; }
        }

        public UInt64 GetCastCount()
        {
            return Convert.ToUInt64(DataManager.VotingResultsDataInstance.GetScalarData("select ifnull(sum(cnt),0) from cast where ballotId = " + ballot.Id + ";"));
        }

        public UInt64 GetTotalCastCount()
        {
            return Convert.ToUInt64(DataManager.VotingResultsDataInstance.GetScalarData("select ifnull(sum(cnt),0) from cast")); // where ballotId = " + ballot.Id + ";"));
        }

        public UInt64 GetVoteCount()
        {
            //select sum(cnt),bid from counter group by bid
            return Convert.ToUInt64(DataManager.VotingResultsDataInstance.GetScalarData("select ifnull(sum(cnt),0) from counter where bid = " + ballot.Id + ";"));
        }

        public UInt64 GetTotalVoteCount()
        {
            return Convert.ToUInt64(DataManager.VotingResultsDataInstance.GetScalarData("select ifnull(sum(cnt),0) from counter where bid in (select distinct ballotId from cast)")); // where ballotId = " + ballot.Id + ";"));
        }

        public ISpeechRecoGrammar Grammar { get; private set; }
        private SpSharedRecoContext SharedRecoContext { get; set; }

        public SpeechRecognitionEngine speechRecognitionEngine { get; set; }

        private ISpeechGrammarRule _grammarRule;

        public ISpeechGrammarRule GrammarRule { get { return _grammarRule; } }

        public bool isDoubleSpacing { get; set; }

        public List<string> _possibleWords=new List<string>();
        public List<string> _speechWords = new List<string>();
        public List<string[]> _commandList = new List<string[]>();
        public bool instructionSpoken = false;
        public System.Timers.Timer hearingtimer;
        public List<ContestDefinition> reviewSelectionList;
        public ReviewModel reviewModel;
        public List<string[]> _reviewSelections = new List<string[]>();
        public List<string> _contestSelections = new List<string>();
        public bool printRecordImageonly = false;

        public Form mainwindow = null;

        public static bool Is64BitOperatingSystem
        {
            get
            {
                bool flag;
                //return (Is64BitProcess ||
                //        ((ModuleContainsFunction("kernel32.dll", "IsWow64Process") &&
                //          IsWow64Process(GetCurrentProcess(), out flag)) && flag));
                return Environment.Is64BitOperatingSystem;
            }
        }

        public static bool Is64BitProcess
        {
            get { return (IntPtr.Size == 8); }
        }

        public static bool useExternalKeyBoard { get; set; }

        public float TenantVoiceWeight { get; private set; }

        public static string GetPathToCommonFile(string path)
        {
            //return string.Format("..{0}CommonFiles{0}{1}", Global.Instance.SLASH, (string.IsNullOrEmpty(path) ? string.Empty : path));
            return string.Format("CommonFiles{0}{1}", Global.Instance.SLASH, (string.IsNullOrEmpty(path) ? string.Empty : path), Global.Instance.APP_PATH);
        }

        public static string ReplaceNumberToEnglish(string pWord, bool pIsNumberToEnglish)
        {
            string sRet = pWord;

            string[] sNumber = new string[] { "1.", "2.", "3.", "4.", "5.", "6.", "7.", "8.", "9.", "10.", "11.", "12.", "13.", "14.", "15.", "16.", "17.", "18.", "19.", "20.", "21.", "22.", "23.", "24.", "25." };
            string[] sEnglish = new string[] { "One", "Two", "Three", "Four", "Five", "Six", "Seven", "Eight", "Nine", "Ten", "Eleven", "Twelve", "Thirteen", "Fourteen", "Fifteen", "Sixteen", "Seventeen", "Eighteen", "Nineteen", "Twenty", "Twenty-one", "Twenty-two", "Twenty-three", "Twenty-four", "Twenty-five" };

            if (pIsNumberToEnglish)
            {
                for (int i = sNumber.Length - 1; i >= 0; i--)
                {
                    if (sRet.IndexOf(sNumber[i]) > -1)
                    {
                        sRet = sRet.Replace(sNumber[i], sEnglish[i]).ToLower(); 
                        break;
                    }
                }
            }
            else
            {
                for (int i = 0; i < sNumber.Length; i++)
                {
                    if (sRet.IndexOf(sEnglish[i] + " ") > -1)
                    {
                        sRet = sRet.Replace(sEnglish[i] + " ", sNumber[i] + "  ");
                        break;
                    }
                }
            }

            return sRet;
        }

        public static string GetPathToScreens()
        {
            return GetPathToCommonFile(string.Format("definition{0}screens{0}", Global.Instance.SLASH));
        }

        public static string GetPathToScreen(int? ballotId)
        {
            return string.Format("{0}{1}{2}", GetPathToScreens(), (ballotId == null ? "default_screen" : "screen" + ballotId), Global.Instance.SLASH);
        }

        private void InitialScreenTime_Elapsed(object sender, ElapsedEventArgs args)
        {
            initialTimer.Stop();
            initialTimer.Enabled = false;
            //var sc = GetActiveScreen();
            //UnsetScreen(sc);
            if (Convert.ToBoolean(AppManager.Configuration["System"]["RemotePollWorker"]))
                SetScreen("remotePollWorker");
            else
                ShowSoftPinpad(null, true);

                // SetTextToSpeechEngine(); checked 

                appState = Global.AppState.STARTED;

            //AppManager.Instance.mainwindow.WindowState = FormWindowState.Minimized;
            //AppManager.Instance.mainwindow.Refresh();

        }

        public void InitializeApp(Form window)
        {
            //var chnge = DataManager.VotingResultsDataInstance.ChangePass("");
            //chnge = DataManager.VotingContentDataInstance.ChangePass("");
            //var chnge = DataManager.VotingResultsDataInstance.ChangePass("VotRite2017");//VotRite2017
            //var chng = DataManager.VotingContentDataInstance.ChangePass("VotRite2017");

            AdminCred frmCred = new AdminCred();
            frmCred.ShowDialog();
            useExternalKeyBoard = frmCred.useExternalKeyboard;
            if (!frmCred.verified)
            {
                Environment.Exit(1);
                //return;
            }
            else
            {
                window.Visible = true;
                window.WindowState = FormWindowState.Normal;
                window.Refresh();
                AppManager.Instance.mainwindow = window;
                _castCounter = Convert.ToUInt64(DataManager.VotingResultsDataInstance.GetScalarData("select cnt from cast where ballotId = " + BallotId + ";"));
                //var sc =GetActiveScreen();
                SetScreen("initial");
                agent = new AppAgent();               

                UpdateMachineValue();
                VerifyFlashDrive(true);
               
               // _castTotalCounter = Convert.ToUInt64(DataManager.VotingResultsDataInstance.GetScalarData("select ifnull(sum(cnt),0) from cast"));// where ballotId = " + BallotId + ";"));
               // _castedBallotCounter = Convert.ToUInt64(DataManager.VotingResultsDataInstance.GetScalarData("select ifnull(count(distinct ballotId), 0) from cast"));// where ballotId = " + BallotId + ";"));
                ReadingLogFilesCount();

                if (Convert.ToBoolean(AppManager.Configuration["System"]["RemotePollWorker"]))
                {
                    hPinPad = new HardPinPad(false);
                }

                //added on 01/12/2019 for resolving remotepollworker screen initialization issues
                ballot = new BallotDefinition();
                

                //Session.SetLocale(Locale.DefaultLocale);

                //-------------------------------------------------------------------------------

                //SetScreen("pinpad");
                initialTimer = new System.Timers.Timer(100);
                initialTimer.Elapsed += new ElapsedEventHandler(InitialScreenTime_Elapsed);
                initialTimer.Enabled = true;

                isDoubleSpacing = false;

                HiddenFolder(true);
                Logger.Instance.Write("Hide folder");
                //IOHandler.SaveConfig("Speech", "Rate", "2");
                Logger.Instance.Write("Speech rate complete");
                string currentmachinebackup = "";
                //if(AppManager.Configuration["System"]["Machine"] != "")
                //{
                //    var macCon = MachineConfig.GetMachineConfig(AppManager.Configuration["System"]["Machine"]);
                //    if (macCon.Count > 0)
                //    {
                //        IOHandler.SaveConfig("System", "BackupFlashDrivePath", macCon[0].BackupDrive);
                //    }
                //        //AppManager.Configuration["System"]["BackupFlashDrivePath"] = macCon[0].BackupDrive;
                //}

                //Configuration = IOHandler.DecryptConfig();
                Logger.Instance.Write("Configuration read complete");
                //SetScreen("pinpad");
                window.Visible = false;
                //window.WindowState = FormWindowState.Minimized;
                window.Refresh();
                // UnsetScreen(GetActiveScreen());
                // initialTimer.Stop();
                Logger.Instance.Write("Initiation complete");
            }
        }

        public void HiddenFolder( bool hide)
        {
            try
            {
                string name = Application.StartupPath;
                DirectoryInfo di = new DirectoryInfo(name);
                name = Path.Combine(di.Parent.FullName, "Help");
                di = new DirectoryInfo(name);
                if (hide)
                    di.Attributes |= FileAttributes.Hidden;
                else
                    di.Attributes = di.Attributes & ~FileAttributes.Hidden;
            }
            catch
            {

            }
        }

        public void SetTextToSpeechEngine()
        {
            try
            {
                // init SAPI 5.1 speech recognition
                /*
                SharedRecoContext = new SpSharedRecoContext();
                SharedRecoContext.Recognition += HandleSpeech;
                SharedRecoContext.FalseRecognition += SharedRecoContext_FalseRecognition;

                Grammar = SharedRecoContext.CreateGrammar(0);
                Grammar.DictationSetState(SpeechRuleState.SGDSInactive);
                // Without this, the recognition is not very good

                // english commands
                ISpeechGrammarRule grammarRule = null;
                grammarRule = Grammar.Rules.Add("English", SpeechRuleAttributes.SRATopLevel, 1);

                Grammar.DictationLoad("", SpeechLoadOption.SLOStatic);
                //TSpSharedRecognizer 
                //Grammar.CmdSetRuleState("English", SpeechRuleState.SGDSActive);
                SharedRecoContext.State = SpeechRecoContextState.SRCS_Enabled;

                object propValue = "";
                
                grammarRule.InitialState.AddWordTransition(null, "yes", " ", SpeechGrammarWordType.SGLexical, "yes", 1, ref propValue, 1.0F);
                grammarRule.InitialState.AddWordTransition(null, "no", " ", SpeechGrammarWordType.SGLexical, "no", 2, ref propValue, 1.0F);
                grammarRule.InitialState.AddWordTransition(null, "continue", " ", SpeechGrammarWordType.SGLexical, "continue", 3, ref propValue, 1.0F);
                grammarRule.InitialState.AddWordTransition(null, "finish", " ", SpeechGrammarWordType.SGLexical, "finish", 4, ref propValue, 1.0F);
                grammarRule.InitialState.AddWordTransition(null, "skip", " ", SpeechGrammarWordType.SGLexical, "skip", 5, ref propValue, 1.0F);
                grammarRule.InitialState.AddWordTransition(null, "Next", " ", SpeechGrammarWordType.SGLexical, "Next", 6, ref propValue, 1.0F);

                //todo:
                // spanish command
                Session.GrammarRule = Grammar.Rules.Add("Spanish", SpeechRuleAttributes.SRATopLevel, 2);
                Session.GrammarRule.InitialState.AddWordTransition(null, "see", " ", SpeechGrammarWordType.SGLexical, "yes", 1, ref propValue, 1.0F);
                //gRule.InitialState.AddWordTransition(null,"no"," ",SpeechGrammarWordType.SGLexical ,"no", 2, ref propValue, 1.0F );
                Session.GrammarRule.InitialState.AddWordTransition(null, "continue are", " ", SpeechGrammarWordType.SGLexical, "continue", 3, ref propValue, 1.0F);
                Session.GrammarRule.InitialState.AddWordTransition(null, "term in are", " ", SpeechGrammarWordType.SGLexical, "finish", 4, ref propValue, 1.0F);
                Session.GrammarRule.InitialState.AddWordTransition(null, "pass are", " ", SpeechGrammarWordType.SGLexical, "skip", 4, ref propValue, 1.0F);*/

                /*//chinese commands
                _gRule = _grammar.Rules.Add("Chinese", SpeechRuleAttributes.SRATopLevel, 3);
                _gRule.InitialState.AddWordTransition(null, "she", " ", SpeechGrammarWordType.SGLexical, "yes", 1, ref propValue, 5.0F);
                _gRule.InitialState.AddWordTransition(null, "foo", " ", SpeechGrammarWordType.SGLexical, "no", 2, ref propValue, 5.0F);
                _gRule.InitialState.AddWordTransition(null, "gee shoo", " ", SpeechGrammarWordType.SGLexical, "continue", 3, ref propValue, 5.0F);
                _gRule.InitialState.AddWordTransition(null, "wan cheng", " ", SpeechGrammarWordType.SGLexical, "finish", 4, ref propValue, 5.0F);
                _gRule.InitialState.AddWordTransition(null, "chow goo oh", " ", SpeechGrammarWordType.SGLexical, "skip", 4, ref propValue, 5.0F);

                // korean commands
                _gRule = _grammar.Rules.Add("Korean", SpeechRuleAttributes.SRATopLevel, 4);
                _gRule.InitialState.AddWordTransition(null, "yeah", " ", SpeechGrammarWordType.SGLexical, "yes", 1, ref propValue, 1.0F);
                _gRule.InitialState.AddWordTransition(null, "ahkneeoh", " ", SpeechGrammarWordType.SGLexical, "no", 2, ref propValue, 1.0F);
                _gRule.InitialState.AddWordTransition(null, "key soak", " ", SpeechGrammarWordType.SGLexical, "continue", 3, ref propValue, 1.0F);
                _gRule.InitialState.AddWordTransition(null, "jong yo", " ", SpeechGrammarWordType.SGLexical, "finish", 4, ref propValue, 1.0F);
                _gRule.InitialState.AddWordTransition(null, "gunortiki", " ", SpeechGrammarWordType.SGLexical, "skip", 4, ref propValue, 1.0F);*/

                //sr.EventInterests = SpeechRecoEvents.SRERecognition;
                //                Grammar.Rules.Commit();
                //                Grammar.CmdSetRuleIdState(1, SpeechRuleState.SGDSInactive);
                //                Grammar.CmdSetRuleIdState(2, SpeechRuleState.SGDSInactive);
                //                _grammar.CmdSetRuleIdState(3, SpeechRuleState.SGDSInactive);
                //                _grammar.CmdSetRuleIdState(4, SpeechRuleState.SGDSInactive);

                //_grammarRule = grammarRule;
               

                // init the SAPI 5.1 interface for simplified chinese voice
                speaker = new SpVoice();
                //speaker.Voice = speaker.GetVoices("Name=Microsoft mike", "Language=409").Item(0);
                speaker.Voice = speaker.GetVoices().Item(0);
                speaker.EndStream += speaker_EndStream;

                //SetSpeechToTextEngine();
            }
            catch (Exception e)
            {
                Logger.Instance.Write(e);
            }
        }

        public void stopVotriteSpeech()
        {
            try
            {
                var procs = Process.GetProcessesByName("VotriteSpeech");//The Votrite voting software
                foreach (var proc in procs)
                {
                    try
                    {
                        proc.Kill();
                    }
                    catch (Exception)
                    {
                    }
                }
            }
            catch (Exception)
            {
            }

            try
            {
                var procs = Process.GetProcessesByName("The Votrite voting software");//
                foreach (var proc in procs)
                {
                    try
                    {
                        proc.Kill();
                    }
                    catch (Exception)
                    {
                    }
                }
            }
            catch (Exception)
            {
            }
        }

        private void SharedRecoContext_FalseRecognition(int StreamNumber, object StreamPosition, ISpeechRecoResult Result)
        {
            //throw new NotImplementedException();
        }

        public void StartSpeaker(string sSpeech)
        {
            //if (Session != null && Session.BallotMode == Session.BallotModes.Audio ||
            //    Session == null && ballot != null && ballot.BallotMode == Session.BallotModes.Audio)
            if ( AppManager.Instance.ballot.BallotMode == Session.BallotModes.Audio)
            {
                int num2 = 2;
                string sRate = AppManager.Configuration["Speech"]["Rate"];
                if (sRate != "")
                {
                    num2 = int.Parse(sRate);
                }
                try
                {
                    if (sSpeech == null)
                        return;
                    speaker.Rate = num2;
                    sSpeech = CharactersToRemove.Aggregate(sSpeech,
                                                           (current, character) => current.Replace(character, ""));
                    //sSpeech = sSpeech.Replace("touch", "say");
                    //sSpeech = sSpeech.Replace("Touch", "Say");
                    sSpeech = sSpeech.Replace("touch", "");
                    sSpeech = sSpeech.Replace("Touch", "");
                    speaker.Speak(sSpeech, SpeechVoiceSpeakFlags.SVSFlagsAsync);
                }
                catch (Exception exception1)
                {
                    Trace.WriteLine(DateTime.Now.ToString() + " -- " + exception1.ToString());
                }
            }
        }

        public void StartSpeaker_preview(string sSpeech)
        {
            //if (Session != null && Session.BallotMode == Session.BallotModes.Audio ||
            //    Session == null && ballot != null && ballot.BallotMode == Session.BallotModes.Audio)
            SetTextToSpeechEngine();
            int num2 = 2;
            string sRate = AppManager.Configuration["Speech"]["Rate"];
            if (sRate != "")
            {
                num2 = int.Parse(sRate);
            }
            try
            {
                speaker.Rate = num2;
                sSpeech = CharactersToRemove.Aggregate(sSpeech,
                                                       (current, character) => current.Replace(character, ""));
                //sSpeech = sSpeech.Replace("touch", "say");
                //sSpeech = sSpeech.Replace("Touch", "Say");
                sSpeech = sSpeech.Replace("touch", "");
                sSpeech = sSpeech.Replace("Touch", "");
                speaker.Speak(sSpeech, SpeechVoiceSpeakFlags.SVSFlagsAsync);
            }
            catch (Exception exception1)
            {
                Trace.WriteLine(DateTime.Now.ToString() + " -- " + exception1.ToString());
            }

        }

        public void StopSpeaker()
        {
            try
            {
                if (AppManager.Instance.hearingtimer != null)
                {
                    AppManager.Instance.hearingtimer.Stop();
                    AppManager.Instance.hearingtimer.Dispose();
                }
                speaker.Speak("", SpeechVoiceSpeakFlags.SVSFPurgeBeforeSpeak);
            }
            catch (Exception exception1)
            {
                Trace.WriteLine(DateTime.Now.ToString() + " -- " + exception1.ToString());
            }
        }

        private void speaker_EndStream(int StreamNumber, object StreamPosition)
        {
            // Triggers when SAPI 5 is done speaking
            if (speaker.Status.RunningState == SpeechRunState.SRSEDone)
            {
                Thread.Sleep(10);
                DoneSpeaking();
            }
        }

        private void DoneSpeaking()
        {
            //TODO:
        }



        public void SetSpeechToTextEngine()
        {
            try
            {
                // create the engine
                speechRecognitionEngine = createSpeechEngine("en-US");

                // hook to events
                speechRecognitionEngine.AudioLevelUpdated += new EventHandler<AudioLevelUpdatedEventArgs>(engine_AudioLevelUpdated);
                speechRecognitionEngine.SpeechRecognized += new EventHandler<SpeechRecognizedEventArgs>(engine_SpeechRecognized);
                speechRecognitionEngine.RecognizeCompleted += SpeechRecognitionEngine_RecognizeCompleted;
                speechRecognitionEngine.SpeechDetected += SpeechRecognitionEngine_SpeechDetected;
                speechRecognitionEngine.SpeechHypothesized += SpeechRecognitionEngine_SpeechHypothesized;
                speechRecognitionEngine.SpeechRecognitionRejected += SpeechRecognitionEngine_SpeechRecognitionRejected;
                // load dictionary
                loadGrammarAndCommands();
                // use the system's default microphone
                speechRecognitionEngine.SetInputToDefaultAudioDevice();

                // start listening
                //speechRecognitionEngine.RecognizeAsync(RecognizeMode.Multiple);
            }
            catch (Exception ex)
            {
                //MessageBox.Show(ex.Message, "Voice recognition failed");
            }
        }

        public void startlistening()
        {
            try
            {
                // hook to events
                //speechRecognitionEngine.AudioLevelUpdated += new EventHandler<AudioLevelUpdatedEventArgs>(engine_AudioLevelUpdated);
                //speechRecognitionEngine.SpeechRecognized += new EventHandler<SpeechRecognizedEventArgs>(engine_SpeechRecognized);
                //speechRecognitionEngine.RecognizeCompleted += SpeechRecognitionEngine_RecognizeCompleted;
                //speechRecognitionEngine.SpeechDetected += SpeechRecognitionEngine_SpeechDetected;
                //speechRecognitionEngine.SpeechHypothesized += SpeechRecognitionEngine_SpeechHypothesized;
                //speechRecognitionEngine.SpeechRecognitionRejected += SpeechRecognitionEngine_SpeechRecognitionRejected;
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

        HearingModeControl hmd;
        public void initiateHearingMode(bool reviewOrconfirm = false)
        {
            if (AppManager.Instance.ballot.BallotMode == Session.BallotModes.Audio)
            {
                if (hmd != null)
                    hmd = null;
                if (reviewOrconfirm)
                    hmd = new HearingModeControl(reviewOrconfirm);
                else
                    hmd = new HearingModeControl();
                hmd.initiateSpeech();
            }
        }

        public void checkDoubleSpaceSetting(List<ScreenObject> screenObjects, VrContainer container)
        {
            try
            {
                foreach (var cntrlobj in screenObjects)
                {
                    if (cntrlobj != null)
                    {
                        if (cntrlobj.Text != null)
                        {
                            cntrlobj.Text = cntrlobj.Text.Replace("\r\n", "");
                            int ix = cntrlobj.Text.IndexOf("  ");
                            if (cntrlobj.Type == ScreenObject.ScreenObjectType.SELECTION)
                            {
                                cntrlobj.Width = 700;
                                if (ix > 0)
                                    cntrlobj.Text = cntrlobj.Text.Insert(ix, " / ");
                            }
                            var words = cntrlobj.Text.Split(' ');
                            string txt = "";
                            foreach (string word in words)
                            {
                                txt += (word == " " || word == "" ? "" : word + " ");
                            }

                            cntrlobj.Text = txt.Trim();

                            if (AppManager.Instance.isDoubleSpacing)
                                cntrlobj.Text = cntrlobj.Text.Replace(" ", "  ");

                            if (AppManager.Instance.backgroundTheme != AppManager.colorTheme.White && cntrlobj.Name != "btn_settings"
                               && (cntrlobj.Type == ScreenObject.ScreenObjectType.BUTTON || cntrlobj.Type == ScreenObject.ScreenObjectType.SCROLL)
                               )
                            {
                                var img = ((VrButton)cntrlobj).BgImage;
                                var nimg = img.Substring(0, img.Length - 4) + "_" + AppManager.Instance.backgroundTheme + ".png";
                                if (!AppManager.instance.writeinModel)
                                {
                                    ((VrButton)cntrlobj).BgImage = nimg;
                                    if (AppManager.Instance.backgroundTheme == AppManager.colorTheme.Green && cntrlobj.Type == ScreenObject.ScreenObjectType.BUTTON)
                                    {
                                        if (((VrButton)cntrlobj).BgImage == "CommonFiles\\graphics\\button_green_Green.png")
                                        {
                                            ((VrButton)cntrlobj).BgImage = "CommonFiles\\graphics\\bottom_btn_light_d_btngreen.png";
                                            cntrlobj.ForeColor = "#FFFFFF";
                                        }
                                    }
                                    //
                                    if (AppManager.Instance.backgroundTheme == AppManager.colorTheme.Yellow || AppManager.Instance.backgroundTheme == AppManager.colorTheme.LightYellow)
                                    {
                                        if (cntrlobj.Text == "Normal" || cntrlobj.Text == "Zoom" || cntrlobj.Text == "Print Review")
                                            cntrlobj.ForeColor = "#000000";
                                        else
                                            cntrlobj.ForeColor = "#FFFFFF";
                                    }
                                }
                                else
                                {
                                    if (cntrlobj.Text == "Cancel" || cntrlobj.Text == "Accept")
                                        ((VrButton)cntrlobj).BgImage = nimg;
                                }
                            }

                            if ((AppManager.Instance.backgroundTheme == AppManager.colorTheme.Contrast
                                || AppManager.Instance.backgroundTheme == AppManager.colorTheme.Blue) && cntrlobj.Name != "btn_settings" 
                               )
                            {
                                if (cntrlobj.ForeColor == "#000000" || cntrlobj.Name == "lbl_contest_count" )
                                {
                                    cntrlobj.ForeColor = "#FFFFFF";
                                    if (AppManager.instance.writeinModel && cntrlobj.Type == ScreenObject.ScreenObjectType.BUTTON && cntrlobj.Text != "Cancel" && cntrlobj.Text != "Accept")
                                        cntrlobj.ForeColor = "#000000";
                                }
                                if (cntrlobj.Name == "lbl_choices_left")
                                    cntrlobj.ForeColor = "#ffeb3b";

                            }
                            if (( AppManager.Instance.backgroundTheme == AppManager.colorTheme.LightBlue) && cntrlobj.Name != "btn_settings"
                                && cntrlobj.Type != ScreenObject.ScreenObjectType.BUTTON )
                            {
                                if (cntrlobj.Name != "lbl_contest_title" && cntrlobj.Name != "lbl_contest_count" && cntrlobj.Name != "lbl_choices_left")
                                    cntrlobj.ForeColor = "#000000";
                                if (AppManager.instance.writeinModel && cntrlobj.Name == "lbl_contest_count")
                                    cntrlobj.ForeColor = "#FFFFFF";
                            }
                            if (AppManager.Instance.backgroundTheme == AppManager.colorTheme.LightBlue && AppManager.instance.writeinModel && cntrlobj.Name == "lbl_contest_count")
                                    cntrlobj.ForeColor = "#000000";
                            if (AppManager.Instance.backgroundTheme == AppManager.colorTheme.Green && cntrlobj.Name != "btn_settings"
                                )
                            {
                                if (cntrlobj.Name == "lbl_contest_count")
                                    cntrlobj.ForeColor = "#FFFFFF";
                                else if (cntrlobj.Name == "lbl_choices_left")
                                    cntrlobj.ForeColor = "#ffeb3b";
                                if (cntrlobj.ForeColor.ToUpper() == "#000000" && cntrlobj.Type == ScreenObject.ScreenObjectType.LABEL)
                                {
                                   
                                        cntrlobj.ForeColor = "#FFFFFF";
                                }
                            }
                            else if (( AppManager.Instance.backgroundTheme == AppManager.colorTheme.White || AppManager.Instance.backgroundTheme == AppManager.colorTheme.Yellow 
                                || AppManager.Instance.backgroundTheme == AppManager.colorTheme.LightYellow) && 
                                cntrlobj.Type == ScreenObject.ScreenObjectType.LABEL && cntrlobj.Name != "btn_settings" )
                            {
                                if (cntrlobj.ForeColor.ToUpper() == "#FFFFFF")
                                    cntrlobj.ForeColor = "#000000";
                            }

                            if(AppManager.instance.writeinModel && cntrlobj.Name == "lbl_text")
                                cntrlobj.ForeColor = "#000000";

                            if (cntrlobj.Type == ScreenObject.ScreenObjectType.SCROLL)
                            {
                                cntrlobj.ForeColor = "#ffffff";
                                switch (AppManager.Instance.backgroundTheme)
                                {

                                    case AppManager.colorTheme.Blue:
                                        cntrlobj.ForeColor = "#0623cf";
                                        break;
                                    case AppManager.colorTheme.Green:
                                        cntrlobj.ForeColor = "#000000";
                                        break;
                                    case AppManager.colorTheme.Contrast:
                                        cntrlobj.ForeColor = "#000000";
                                        break;
                                    case AppManager.colorTheme.LightBlue:
                                        cntrlobj.ForeColor = "#000000";
                                        break;
                                    case AppManager.colorTheme.White:
                                        cntrlobj.ForeColor = "#000000";
                                        break;

                                    default:
                                        break;
                                }
                            }

                        }
                    }
                }

            }
            catch (Exception)
            { }

            try
            {
                if (container == null)
                    return;
                if (container.Controls == null)
                    return;
                int x = 1;
                foreach (var cntrlobj in container.Controls)
                {
                    if (cntrlobj != null)
                    {
                        if (cntrlobj.Text != null)
                        {
                            cntrlobj.Text = cntrlobj.Text.Replace("\r\n", "");
                            int ix = cntrlobj.Text.IndexOf("  ");
                            if (cntrlobj.Type == ScreenObject.ScreenObjectType.SELECTION)
                            {
                                //cntrlobj.Width = 700;
                                if (ix > 0)
                                    cntrlobj.Text = cntrlobj.Text.Insert(ix, " / ");

                                //if(AppManager.Instance.backgroundTheme == colorTheme.Contrast)
                                //{
                                //    cntrlobj.BgColor = "#000000";
                                //    cntrlobj.ForeColor = "#FFFFFF";
                                //}
                            }
                            var words = cntrlobj.Text.Split(' ');
                            string txt = "";
                            foreach (string word in words)
                            {
                                txt += (word == " " || word == "" ? "" : word + " ");
                            }

                            cntrlobj.Text = txt.Trim();
                            if (AppManager.Instance.isDoubleSpacing)
                                cntrlobj.Text = cntrlobj.Text.Replace(" ", "  ");

                            cntrlobj.ForeColor = "#ffffff";
                            switch (AppManager.Instance.backgroundTheme)
                            {

                                
                                case AppManager.colorTheme.Yellow:
                                    cntrlobj.ForeColor = "#000000";
                                    break;
                                case AppManager.colorTheme.LightYellow:
                                    cntrlobj.ForeColor = "#000000";
                                    break;
                                case AppManager.colorTheme.LightBlue:
                                    cntrlobj.ForeColor = "#000000";
                                    break;
                                case AppManager.colorTheme.White:
                                    cntrlobj.ForeColor = "#000000";
                                    break;

                                default:
                                    break;
                            }
                        }
                    }
                }
            }
            catch (Exception)
            { }

            try
            {
                if (AppManager.Instance.backgroundTheme != AppManager.colorTheme.White && container != null)
                {
                    switch (AppManager.Instance.backgroundTheme)
                    {
                       
                        case AppManager.colorTheme.Yellow:
                            container.BgColor = "#ffeb3b";
                            //var scroll = container.FindControlByName("");
                            break;
                        case AppManager.colorTheme.Blue:
                            container.BgColor = "#0623cf";
                            break;
                        case AppManager.colorTheme.Green:
                            container.BgColor = "#4caf50";
                            break;
                        case AppManager.colorTheme.Contrast:
                            container.BgColor = "#000000";
                            break;
                        case AppManager.colorTheme.LightBlue:
                            container.BgColor = "#aad8e6";
                            break;
                        case AppManager.colorTheme.LightYellow:
                            container.BgColor = "#FFFF99";// "#FFFFEO";// "#fff44f";
                            break;
                        default:
                            break;
                    }
                }
            }
            catch (Exception)
            {
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
                speechRecognitionEngine.LoadGrammar(wordsList);
            }
            catch (Exception ex)
            {
                throw ex;
            }
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
        }

        private void SpeechRecognitionEngine_RecognizeCompleted(object sender, RecognizeCompletedEventArgs e)
        {
            throw new NotImplementedException();
        }
        private void SpeechRecognitionEngine_SpeechRecognitionRejected(object sender, SpeechRecognitionRejectedEventArgs e)
        {
            throw new NotImplementedException();
        }

        private void SpeechRecognitionEngine_SpeechHypothesized(object sender, SpeechHypothesizedEventArgs e)
        {
            throw new NotImplementedException();
        }

        private void SpeechRecognitionEngine_SpeechDetected(object sender, SpeechDetectedEventArgs e)
        {
            throw new NotImplementedException();
        }
        void engine_AudioLevelUpdated(object sender, AudioLevelUpdatedEventArgs e)
        {
            //prgLevel.Value = e.AudioLevel;
        }

        public void DisposeSpeechToTextEngine()
        {
            try
            {
                if (speechRecognitionEngine != null)
                {
                    // unhook events
                    speechRecognitionEngine.RecognizeAsyncStop();
                    // clean references
                    speechRecognitionEngine.Dispose();
                }
            }
            catch (Exception)
            {

            }

        }

        public static void UpdateMachineValue()
        {
            if (Driver.TestMode) return;

            try
            {
                DBBallot.UpdateMachineValue(AppManager.Configuration["System"]["Machine"]);
            }
            catch (Exception e)
            {
                DialogResult dlgRes = MessageBox.Show(string.Format("Data base exception. {0}", e.Message), "Error",
                                                      MessageBoxButtons.OK, MessageBoxIcon.Error);
                if (dlgRes == DialogResult.OK)
                    Environment.Exit(1);
            }
        }

        public void VerifyFlashDrive(bool initiate = false)
        {
            try
            {
                
                    var list = VotRiteBallotDataManager.AppCode.MachineConfig.GetMachineConfig(AppManager.Configuration["System"]["Machine"]);

                //MessageBox.Show("Current Flash drive:" + list[0].BackupDrive + Environment.NewLine + "From previous config flash:" + AppManager.Configuration["System"]["BackupFlashDrivePath"]);

                //AppManager.Configuration["System"]["BackupFlashDrivePath"] = list[0].BackupDrive;
                if (list[0].BackupDrive != AppManager.Configuration["System"]["BackupFlashDrivePath"])
                {
                    Util.IOHandler.SaveConfig("System", "BackupFlashDrivePath", list[0].BackupDrive);
                    AppManager.Configuration = Util.IOHandler.DecryptConfig();
                }
            }
            catch (Exception)
            {

            }

            try
            {
                //Util.IOHandler.SaveConfig("System", "BackupFlashDrivePath", "C:\\Election_Results_Backup");
               
                string flashDrivePath = AppManager.Configuration["System"]["BackupFlashDrivePath"];
                if (!Directory.Exists(flashDrivePath))
                    Directory.CreateDirectory(flashDrivePath);
            }
            catch (Exception)
            {

            }
            try
            {
                string flashDrivePath = AppManager.Configuration["System"]["BackupFlashDrivePath"];
                if (!Directory.Exists(flashDrivePath))
                    Directory.CreateDirectory(flashDrivePath);

                using (FileStream sw = File.Create(Path.Combine(flashDrivePath, "drive_test.txt"), 64, FileOptions.DeleteOnClose))
                {
                    byte[] bytes = Encoding.UTF8.GetBytes("hello world!!!");
                    sw.Write(bytes, 0, bytes.Length);
                }
                Logger.Instance.Write("Flash memory test passed!" + flashDrivePath);
            }
            catch (Exception e)
            {

                // ****************** Changed on 25 Sep 2020/*
                Logger.Instance.Write(e);
               // DialogResult dlgRes = MessageBox.Show("Internal flash test failure. Internal flash not inserted or corrupted. Proceed without Flash?", "Error",
               //                     MessageBoxButtons.AbortRetryIgnore, MessageBoxIcon.Error);
                DialogResult dlgRes = MessageBox.Show("Internal flash test failure. Internal flash not inserted or corrupted. ", "Error",
                                    MessageBoxButtons.RetryCancel, MessageBoxIcon.Error);

                if (dlgRes == DialogResult.Abort)
                {
                    Environment.Exit(1);
                }
                if (dlgRes == DialogResult.Retry)
                {
                    VerifyFlashDrive();
                }
                
                
                
            }
        }

        delegate void RefreshCallback();

        public void ShowSoftPinpad(string showMenu, bool doRefresh)
        {
            if (doRefresh)
            {
                if (Window.Instance.InvokeRequired)
                {
                    RefreshCallback d = new RefreshCallback(Window.Instance.Refresh);
                    Window.Instance.Invoke(d);
                }
            }

            if (!string.IsNullOrEmpty(showMenu) && PinpadView.Menu != null)
            {
                PinpadView.Menu.GetMenu(showMenu);
            }

            SetScreen("pinpad");
        }

        public void ShowAudioWindow()
        {
            F_SpeechRate oForm = new F_SpeechRate();
            oForm.ShowDialog();
        }


        public void ShowTenants()
        {
            var form = new IdentifyTenantForm();
            form.ShowDialog();
            Tenant selectedTenant = form.SelectedTenant;
        }

        public void ShowTenants2()
        {
            var form = new IdentifyTenantForm2();
            form.ShowDialog();
            Tenant selectedTenant = form.SelectedTenant;
        }

        public void SetScreen(string name)
        {
            while (speaker != null && speaker.Status.RunningState == SpeechRunState.SRSEIsSpeaking)
            {
                StopSpeaker();
            }

            // make sure SD card is in
            VerifyFlashDrive();

            //TODO: char first not required (below 3 lines not required)
            char first = Convert.ToChar(name.Substring(0, 1));
            string reminder = name.Substring(1);
            name = first.ToString().ToUpper() + reminder;

            if (name.ToLower() == "pinpad")
            {
                MenuForm frm = new MenuForm();
                frm.ShowDialog();
               // return;
            }
            else
            {
                Type t;
                View v = null;

                try
                {
                    t = Type.GetType("VotRite.Models." + name + "Model");

                    model = (ScreenModel)Activator.CreateInstance(t);

                    t = Type.GetType("VotRite.Controllers." + name + "Controller");

                    if (t == null)
                        controller = new ScreenController(model);
                    else
                        controller = (ScreenController)Activator.CreateInstance(t, new object[] { model });

                    if (name == "Review")
                        vController = (ReviewController)controller;

                    var pathToDefinition = GetPathToScreen(BallotId) + name.ToLower() + ".sdf";

                    if (!File.Exists(pathToDefinition))
                    {
                        pathToDefinition = GetPathToScreen(null) + name.ToLower() + ".sdf";
                    }

                    model.Definition =
                        (ScreenDefinition)DefinitionParser.Instance.Parse(pathToDefinition);

                    //if(name == "Confirm")
                    //{
                    //    model.Definition.Background = "graphics\\initialBG_f1.png"; 
                    //}

                    t = Type.GetType("VotRite.Views." + name + "View");

                    v = (View)Activator.CreateInstance(t, new object[] { model, controller });
                    /*
                    if (activeScreens.Count != 0)
                    {
                        prevScreen = GetActiveScreen().Name.ToLower();
                    }
                    */
                    if (!model.Definition.IsDialog)
                    {
                        while (activeScreens.Count > 0)
                            UnsetScreen(GetActiveScreen());
                    }

                    //((VrScreen)v).Name = name;
                    //if(name.ToLower() != "pinpad")
                    activeScreens.Add((VrScreen)v);
                }
                catch (Exception ex)
                {
                    Logger.Instance.Write(ex);
                }

                Window.Instance.Invalidate();
                if (name == "Pinpad")
                {
                    model.Definition.Background = "\\graphics\\initialBG_f.png";
                    // model.Definition.BgColor = "#000000";
                }
                if (name.ToLower() == "initial" || name.ToLower() == "pinpad" || name.ToLower() == "menu")
                {
                    try
                    {
                        IOHandler.SaveConfig("TextSize", "Value", "16");
                        AppManager.Configuration = IOHandler.DecryptConfig();
                    }
                    catch (Exception)
                    {

                    }
                }

            }
        }

        public void SetScreen(string name, string pCurrentScreenName)
        {
            while (speaker != null && speaker.Status.RunningState == SpeechRunState.SRSEIsSpeaking)
            {
                StopSpeaker();
            }

            // make sure SD card is in
            VerifyFlashDrive();

            char first = Convert.ToChar(name.Substring(0, 1));
            string reminder = name.Substring(1);
            name = first.ToString().ToUpper() + reminder;
            Type t;
            View v = null;

            try
            {
                t = Type.GetType("VotRite.Models." + name + "Model");

                model = (ScreenModel)Activator.CreateInstance(t);

                t = Type.GetType("VotRite.Controllers." + name + "Controller");

                if (t == null)
                    controller = new ScreenController(model);
                else
                    controller = (ScreenController)Activator.CreateInstance(t, new object[] { model, pCurrentScreenName });

                var pathToDefinition = GetPathToScreen(BallotId) + name.ToLower() + ".sdf";
                if (!File.Exists(pathToDefinition))
                {
                    pathToDefinition = GetPathToScreen(null) + name.ToLower() + ".sdf";
                }

                model.Definition =
                    (ScreenDefinition)DefinitionParser.Instance.Parse(pathToDefinition);

                t = Type.GetType("VotRite.Views." + name + "View");

                try
                {
                    v = (View)Activator.CreateInstance(t, new object[] { model, controller });
                }
                catch (Exception ex)
                {
                    Logger.Instance.Write(ex);
                }
                /*
                if (activeScreens.Count != 0)
                {
                    prevScreen = GetActiveScreen().Name.ToLower();
                }
                */
                if (!model.Definition.IsDialog)
                {
                    while (activeScreens.Count > 0)
                        UnsetScreen(GetActiveScreen());
                }

                //((VrScreen)v).Name = name;
                activeScreens.Add((VrScreen)v);
            }
            catch (Exception ex)
            {
                Logger.Instance.Write(ex);
            }

            Window.Instance.Invalidate();
        }

        public void UnsetScreen(VrScreen screen)
        {
            foreach (VrScreen scr in activeScreens)
            {
                if (scr.Equals(screen))
                {
                    activeScreens.Remove(screen);
                    //screen.Dispose();
                    break;
                }
            }
        }

        public VrScreen GetActiveScreen()
        {
            if (activeScreens.Count > 0)
            {
                return activeScreens[activeScreens.Count - 1];
            }

            return null;
        }

        public void SetLocale(Locale locale)
        {
            OpenSession();
            Session.SetLocale(locale);

            if (Session.BallotMode == Session.BallotModes.Audio)
            {
                try
                {
                   // Session.GrammarRule = Grammar.Rules.FindRule(1);
                }
                catch (Exception)
                {

                }
                
            }
            
            if (ballot != null)
            {
                if (ballot.ElectionType == ElectionTypes.ranking_choice || (this.ballot.ContestsList.Count > 0 && this.ballot.ContestsList[0].Type == ContestTypes.V))
                {
                    SetScreen("RankingChoice");
                    return;
                }
                else if (ballot.HasParty)
                {
                    ballot = DefinitionParser.Instance.FillPartiesContent(ballot, Session.CurrentLocale);
                    SetScreen("parties");
                    return;
                }
                else if (ballot.HasCounties)
                {
                    BallotDefinition bd = new BallotDefinition();
                    bd.countiesDefinition.Counties = County.GetCounties(BallotId, locale.Id);
                    ballot.countiesDefinition.Counties = bd.countiesDefinition.Counties;

                    SetScreen("counties");
                    return;
                }
                else if (ballot.HasSlates)
                {
                    SetScreen("slates");
                    return;
                }
            }

            if (ballot != null && ballot.ContestsList.Count == 0)
            {
                MessageBox.Show("No contests configured in this ballot. Please use Edit Ballot to add contests before starting a voting session.",
                    "No Contests", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                SetScreen("initial");
                return;
            }

            SetScreen("contest");

            // SetScreen("remotePollWorker");
        }

        public string GetLocale()
        {
            return Session.CurrentLocale.Code;
        }

        public void SetCounty(County county)
        {
            ballot.CountyId = county.Id;
            ballot.CountyName = county.Name;
            ballot.Address = ballot.CountyName + " - " + ballot.Address;
            selectedCountyId = county.Id;

            if (ballot.ElectionType == ElectionTypes.ranking_choice)
            {
                SetScreen("RankingChoice");
                return;
            }
            else if (ballot.HasParty)
            {
                SetScreen("parties");
                return;
            }
            else if (ballot.slatesDefinition.Slates.Count > 0)
            {
                SetScreen("slates");
                return;
            }
            SetScreen("contest");
        }

        [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr GetCurrentProcess();

        [DllImport("kernel32.dll", CharSet = CharSet.Auto)]
        private static extern IntPtr GetModuleHandle(string moduleName);

        [DllImport("kernel32.dll", CharSet = CharSet.Ansi, SetLastError = true)]
        private static extern IntPtr GetProcAddress(IntPtr hModule, string methodName);

        public Process GetOskProcess()
        {
            Func<Process, bool> predicate = null;
            string processName = Path.GetFileNameWithoutExtension(OnScreenKeyboadApplication);
            if (!string.IsNullOrEmpty(processName))
            {
                if (predicate == null)
                {
                    predicate = delegate(Process process) { return process.ProcessName == processName; };
                }
                return Process.GetProcesses().Where(predicate).FirstOrDefault<Process>();
            }
            return null;
        }

        public IntPtr GetOskWindowHandle()
        {
            Process oskProcess = GetOskProcess();
            if (oskProcess != null)
            {
                return oskProcess.MainWindowHandle;
            }
            return IntPtr.Zero;
        }

        [return: MarshalAs(UnmanagedType.Bool)]
        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern bool IsWow64Process(IntPtr hProcess, [MarshalAs(UnmanagedType.Bool)] out bool isWow64);

        private static bool ModuleContainsFunction(string moduleName, string methodName)
        {
            IntPtr moduleHandle = GetModuleHandle(moduleName);
            return ((moduleHandle != IntPtr.Zero) && (GetProcAddress(moduleHandle, methodName) != IntPtr.Zero));
        }

        public void OpenBallot(BallotDefinition b, bool openSession)
        {

            if (ballot != null & openSession)
            {
                ballot.Dispose();
                ballot = null;
            }

            ballot = b ?? DefinitionParser.Instance.FillBallotContent(ballot);

            if (ballot.BallotMode == Session.BallotModes.Audio)
            {
                SetTextToSpeechEngine();
            }

            Logger.Instance.Write("ballot opened: " + ballot.Id);

            if (openSession && (ballot.ElectionType != ElectionTypes.standard) && (ballot.ElectionType != ElectionTypes.ranking_choice))
            {
                Tenant selectedTenant = null;
                var form = new IdentifyTenantForm2();
                form.ShowDialog();
                if (form.DialogResult == DialogResult.OK)
                {
                    selectedTenant = form.SelectedTenant;
                    if (selectedTenant != null)
                    {
                        TenantVoiceWeight = (ballot.ElectionType == ElectionTypes.shares)
                                                ? (selectedTenant.Shares)
                                                : selectedTenant.Percent;
                        Logger.Instance.Write(
                            string.Format("Tenant info: name - {0}; apartment - {1}; voice weight - {2}.",
                                          selectedTenant.Name, selectedTenant.Apartment, TenantVoiceWeight));
                    }


                    if (ballot.HasOverview)
                    {
                        SetScreen("ballotoverview", "pinpad");
                    }
                    else
                    {
                        SetScreen("locale");
                    }
                }
                else
                {
                    SetScreen("initial");
                    Thread.Sleep(500);
                    Instance.ShowSoftPinpad(null, true);
                }
            }
            else if (openSession)
            {
                if (ballot.HasOverview)
                {
                    SetScreen("ballotoverview", "pinpad");
                }
                else
                {
                    SetScreen("locale");
                }
            }

            // if (ballot != null & openSession)
            // {
            //     ballot.Dispose();
            //     ballot = null;

            // }

            // ballot = b ?? DefinitionParser.Instance.FillBallotContent(ballot);
            // //
            //// OpenSession();
            // if (Session != null)
            // {
            //     if (ballot.BallotMode == Session.BallotModes.Audio)
            //     {
            //         SetTextToSpeechEngine();
            //         Session.BallotMode = Session.BallotModes.Audio;
            //         Session.GrammarRule = AppManager.Instance.GrammarRule;
            //     }
            //     else
            //         Session.BallotMode = Session.BallotModes.Normal;
            // }

            // Logger.Instance.Write("ballot opened: " + ballot.Id);

            // if (openSession && (ballot.ElectionType != ElectionTypes.standard) && (ballot.ElectionType != ElectionTypes.ranking_choice))
            // {
            //     Tenant selectedTenant = null;
            //     var form = new IdentifyTenantForm2();
            //     form.ShowDialog();
            //     if (form.DialogResult == DialogResult.OK)
            //     {
            //         selectedTenant = form.SelectedTenant;
            //         if (selectedTenant != null)
            //         {
            //             TenantVoiceWeight = (ballot.ElectionType == ElectionTypes.shares)
            //                                     ? (selectedTenant.Shares)
            //                                     : selectedTenant.Percent;
            //             Logger.Instance.Write(
            //                 string.Format("Tenant info: name - {0}; apartment - {1}; voice weight - {2}.",
            //                               selectedTenant.Name, selectedTenant.Apartment, TenantVoiceWeight));
            //         }


            //         if(ballot.HasOverview)
            //         {
            //             SetScreen("ballotoverview", "pinpad");
            //         }
            //         else
            //         {
            //             SetScreen("locale");
            //         }
            //     }
            //     else
            //     {
            //         SetScreen("initial");
            //         Thread.Sleep(500);
            //         Instance.ShowSoftPinpad(null, true);
            //     }
            // }
            // else if (openSession)
            // {
            //     if (ballot.HasOverview)
            //     {
            //         SetScreen("ballotoverview", "pinpad");
            //     }
            //     else
            //     {
            //         SetScreen("locale");
            //     }
            // }
        }

        public void OpenSession()
        {
            if (Session != null) Session.Dispose();
            Session = new Session(ballot);
        }

        public void CloseSession()
        {
            if (Session != null)
            {
                Logger.Instance.Write("Closing session: " + Session.Id);
                Session.Dispose();
                Session = null;
            }
        }

        public void CloseBallot()
        {
            if (ballot != null)
            {
                Logger.Instance.Write("Closing ballot: " + ballot.Id);
                ballot.Dispose();
                ballot = null;
#if !DEBUG
                hPinPad.Reset();
#endif
            }
        }

        public void CloseOskProcess()
        {
            try
            {
                Process oskProcess = GetOskProcess();
                if (oskProcess != null)
                {
                    oskProcess.Kill();
                }
            }
            catch (Exception)
            {
            }
            
        }

        public void ResetBallot()
        {
            if (AppManager.instance.reviewprinted)
            {
                //if (ph == null)
                    ph = new PrinterHandler();
                ph.Print(PrintDoc.VOID, true);
            }

                //ImageEncryptor.EncryptFolder(new List<string> { Global.Instance.APP_PATH + "records\\" }, "*.png");

                string recordsDir = Global.Instance.APP_PATH + "records";

                /*if (File.Exists(recordsDir))
                {
                    ProcessFile(recordsDir);
                }
                else if (Directory.Exists(recordsDir))
                {
                    ProcessDirectory(recordsDir);
                }
                else
                {
                    Logger.Instance.Write(recordsDir + " is not a valid file or directory.");
                }*/
            
            CloseSession();
            CloseBallot();
            //SetScreen("initial");
            IOHandler.SaveConfig("TextSize", "Value", "16");
            AppManager.Instance.isDoubleSpacing = false;
            AppManager.Instance.reviewResultFontSize = 18;
            AppManager.Instance.reviewHeaderFontSize = 22;
            AppManager.Instance.backgroundTheme = colorTheme.White;
            //Thread.Sleep(500);
            Instance.ShowSoftPinpad("main", true);
        }

        public void ResetVoting()
        {
            if (ph != null)
            {
                ph.Print(PrintDoc.VOID, true);

                string recordsDir = Global.Instance.APP_PATH + "records";
            }
            DataManager.VotingResultsDataInstance.SetData("delete from cast");
            DataManager.VotingResultsDataInstance.SetData("delete from counter");
            _castCounter = Convert.ToUInt64(DataManager.VotingResultsDataInstance.GetScalarData("select cnt from cast where ballotId = " + BallotId + ";"));
            CloseSession();
            CloseBallot();
            SetScreen("initial");
            IOHandler.SaveConfig("TextSize", "Value", "16");
            AppManager.Instance.isDoubleSpacing = false;
            AppManager.Instance.reviewResultFontSize = 18;
            AppManager.Instance.reviewHeaderFontSize = 22;
            AppManager.Instance.backgroundTheme = colorTheme.White;
            Thread.Sleep(500);
            Instance.ShowSoftPinpad("main", true);
        }

        public void RunOsk()
        {
            try
            {


                var ptr = new IntPtr();
                bool flag = false;
                if (Is64BitOperatingSystem)
                {
                    flag = Wow64DisableWow64FsRedirection(ref ptr);
                }
                using (var process = new Process())
                {
                    process.StartInfo.FileName = @"C:\Windows\System32\osk.exe";// OnScreenKeyboadApplication;
                    process.Start();
                    try
                    {
                        process.WaitForInputIdle(0);
                    }
                    catch (Exception)
                    {
                    }
                }
                if (Is64BitOperatingSystem && flag)
                {
                    Wow64RevertWow64FsRedirection(ptr);
                }
            }
            catch (Exception)
            {

            }
        }

        public void CastVote()
        {
            Logger.Instance.Write("Casting ballot for session: " + Session.Id);

           // ReadingLogFilesCount();
            Logger.Instance.Write("Test Mode1: " + Driver.TestMode.ToString());
            try
            {
                ph = new PrinterHandler();
                ph.CutPaper();
            }
            catch (Exception)
            {

            }
            
            Logger.Instance.Write("Test Mode: " + Driver.TestMode.ToString());
            //if (!Driver.TestMode)
            //{
                SetCounter();
                _castCounter = Convert.ToUInt64(DataManager.VotingResultsDataInstance.GetScalarData("select cnt from cast where ballotId = " + BallotId + ";"));
                _castCounter++;
                if (_castCounter == 1)
                {
                    DataManager.VotingResultsDataInstance.SetData(
                        "insert into cast(cnt, ballotId) values(" + _castCounter + ", " + BallotId + ");");
                    _castedBallotCounter++;
                    Logger.Instance.Write("New Cast added: " + Session.Id);
                }
                else
                {
                    DataManager.VotingResultsDataInstance.SetData("update cast set cnt=" + _castCounter + " where ballotId = " + BallotId + ";");
                    Logger.Instance.Write(" Cast updated: " + Session.Id);
                }

                _castTotalCounter = Convert.ToUInt64(DataManager.VotingResultsDataInstance.GetScalarData("select ifnull(sum(cnt),0) from cast"));
                /*#if !DEBUG
                            if (!Convert.ToBoolean(AppManager.Configuration["System"]["RemotePollWorker"]))
                                counter.Write("Total: " + castCounter);
                #endif*/

                IOHandler.UpdateBackupDbOnFlashDrive();
            //}
            //else
            //{
            //    Logger.Instance.Write("Test Mode: " + "No");
            //}

            CloseSession();
            CloseBallot();
            stopVotriteSpeech();

            IOHandler.SaveConfig("TextSize", "Value", "16");
            AppManager.Instance.isDoubleSpacing = false;
            AppManager.Instance.reviewResultFontSize = 18;
            AppManager.Instance.reviewHeaderFontSize = 22;
            AppManager.Instance.backgroundTheme = colorTheme.White;

        }
        
        public void PrintReview()
        {
            //ConfirmView.PrintReview();
            ReviewView.PrintReview();
        }

        public void CancelVote()
        {
            // keep track of how many times back button was pressed
            Session.AddBackButtonCounter();

            //PrinterHandler.Instance.Print(PrintDoc.VOID, true);
            if (ph != null)
            {
                //ph.Print(PrintDoc.VOID, false);
                //ImageEncryptor.EncryptFolder(new List<string> { Global.Instance.APP_PATH + "records\\" }, "*.png");

                string recordsDir = Global.Instance.APP_PATH + "records";

               /* if (File.Exists(recordsDir))
                {
                    ProcessFile(recordsDir);
                }
                else if (Directory.Exists(recordsDir))
                {
                    ProcessDirectory(recordsDir);
                }
                else
                {
                    Logger.Instance.Write(recordsDir + " is not a valid file or directory.");
                }*/
            }
            ph = null;
            SetScreen("review");
        }

        public void SessionExpired()
        {
            // todo
        }

        public void UpdateSession()
        {
            if (Session != null) Session.LastUpdated = DateTime.Now;
        }

        public void PrintRecord()
        {
            string recordsDir = Global.Instance.APP_PATH + "records";

            if (!Directory.Exists(recordsDir))
                Directory.CreateDirectory(recordsDir);            

            //if (AppManager.PrinterEnabled)
            //{
                ph = new PrinterHandler();
                //ph.Print(PrintDoc.RECORD, false, false);
                ph.Print(PrintDoc.RECORD, false);

                if (!ph.PrintingCompleted)
                {
                    MessageBox.Show("Printing was not complete. Please press 'Accept and print' to proceed.");
                    return;
                }
           // }
            
            SetScreen("confirm");
        }

        public void PrintReport()
        {
           
            List<Ballot> list_ballots = new List<DBBallot>();
            JimForms.BallotsListDisplay frmBallots = new JimForms.BallotsListDisplay(JimForms.BallotsListDisplay.displayMode.Report);
           
                
                frmBallots.ShowDialog();
                if (frmBallots.selectedBallots.Count == 0)
                {
                    // AppManager.Instance.ShowSoftPinpad("main", false);
                    return;
                }


                if (frmBallots.selectedBallots.FindAll(a => a.Id == -1).Count > 0)
                {

                    var dtBallots = DataManager.VotingResultsDataInstance.GetDataV2("select distinct ballotId from cast");

                    if (dtBallots == null)
                    {
                        MessageBox.Show("An error occured. Please, try again or inform your administrator.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }

                    list_ballots = Ballot.GetBallots(dtBallots);
                }
                else
                    list_ballots = frmBallots.selectedBallots;
            

                List <BallotDefinition> defBallots = new List<BallotDefinition>();

            foreach (Ballot fballot in list_ballots)
            {
                if (fballot.Id != -1 && fballot.Id != -2)
                {
                    
                    fballot.ElectionName = fballot.ElectionName.Split(new string[] { " (Total Cast:" }, StringSplitOptions.None)[0];
                    BallotDefinition bd = DefinitionParser.Instance.FillBallotContent(null, fballot);
                    // bd = DefinitionParser.Instance.FillSlatesContent(bd, Locale.GetDefaultLocale(bd.Id));

                    if (bd.HasParty)
                    {
                        bd = DefinitionParser.Instance.FillPartiesContent(bd, Locale.GetDefaultLocale(bd.Id));
                    }

                    if (bd.HasCounties)
                    {
                        bd = DefinitionParser.Instance.FillCountiesContent(bd, Locale.GetDefaultLocale(bd.Id));
                    }

                    defBallots.Add(bd);
                }
            }

            bool currentPrinterStatus = AppManager.PrinterEnabled;

            for (int j = 0; j < defBallots.Count; j++)
            {
                AppManager.PrinterEnabled = true;
                //defBallots[j].ReportLocaleId = 0;
                for (int p = 1; p <= frmBallots.copy; p++)
                {
                    AppManager.Instance.PrintFinalReportStarted = true;
                    OpenBallot(defBallots[j], false);
                    ph = new PrinterHandler();
                    ph.noVoid = true;
                    ph.Print(PrintDoc.REPORT, true);                    
                    Thread.Sleep(500);
                    ph = null;
                }
                finalReportPrintCounter++;
            }

            if (!currentPrinterStatus)
                AppManager.PrinterEnabled = false;

            finalReportPrintCounter = 0;

            string reportsDir = Global.Instance.APP_PATH + "reports";

            if (!Directory.Exists(reportsDir))
                Directory.CreateDirectory(reportsDir);

            
            ph = new PrinterHandler();
            ph.PrintConsolidated();
            AppManager.Instance.Session = null;

            IOHandler.BackUpDb();

            //resetting voting after printing
            AppManager.instance.ResetVoting();
           

            string appPath = Global.Instance.APP_PATH.TrimEnd(new[] { '\\' });
            int index = appPath.LastIndexOf('\\');

            

            string backupDir = AppManager.Configuration["System"]["BackupFlashDrivePath"];

            
            list_ballots.ForEach(ball => ball.Dispose());
            list_ballots.Clear();

            defBallots.ForEach(defBall => defBall.Dispose());
            defBallots.Clear();

        }

        public void PrintReportSmit()
        {
            //var dtBallots1 = DataManager.VotingResultsDataInstance.GetDataV2("select * from cast");
           // var bbol = DataManager.VotingResultsDataInstance.SetData("delete from cast");
           // bbol = DataManager.VotingResultsDataInstance.SetData("insert into cast(cnt, ballotId) select sum(cnt),bid from counter group by bid");

            var dtBallots = DataManager.VotingResultsDataInstance.GetDataV2("select distinct ballotId from cast");

            if (dtBallots == null)
            {
                MessageBox.Show("An error occured. Please, try again or inform your administrator.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            List<Ballot> list_ballots = Ballot.GetBallots(dtBallots);
            List<BallotDefinition> defBallots = new List<BallotDefinition>();

            foreach (Ballot fballot in list_ballots)
            {
                BallotDefinition bd = DefinitionParser.Instance.FillBallotContent(null, fballot);
                // bd = DefinitionParser.Instance.FillSlatesContent(bd, Locale.GetDefaultLocale(bd.Id));

                if (bd.HasParty)
                {
                    bd = DefinitionParser.Instance.FillPartiesContent(bd, Locale.GetDefaultLocale(bd.Id));
                }

                if (bd.HasCounties)
                {
                    bd = DefinitionParser.Instance.FillCountiesContent(bd, Locale.GetDefaultLocale(bd.Id));
                }

                defBallots.Add(bd);
            }

            if (defBallots.Count > 0)
            {
                for (int i = 0; i < 1; i++)
                {
                    for (int j = 0; j < defBallots.Count; j++)
                    {
                        OpenBallot(defBallots[j], false);
                        ph = new PrinterHandler();
                        //ph.PrintSmit(PrintDoc.REPORT, true);
                        ph.Print(PrintDoc.REPORT, true);
                        finalReportPrintCounter++;
                        Thread.Sleep(500);
                        ph = null;
                    }
                }
            }
            else
            {
                ph = new PrinterHandler();
                ph.Print(PrintDoc.REPORT, true, true);
                finalReportPrintCounter++;
                Thread.Sleep(500);
                ph = null;
            } // No Data,Print 0 and date&time

            finalReportPrintCounter = 0;

            string reportsDir = Global.Instance.APP_PATH + "reports";

            if (!Directory.Exists(reportsDir))
                Directory.CreateDirectory(reportsDir);

            //ImageEncryptor.EncryptFolder(new List<string> { Global.Instance.APP_PATH + "reports\\" }, "*.png");

            /*if (File.Exists(reportsDir))
            {
                ProcessFile(reportsDir);
            }
            else if (Directory.Exists(reportsDir))
            {
                ProcessDirectory(reportsDir);
            }
            else
            {
                Logger.Instance.Write(reportsDir + " is not a valid file or directory.");
            }*/

           // IOHandler.BackUpDb();

           // DataManager.VotingResultsDataInstance.SetData("delete from cast;");
          //  DataManager.VotingResultsDataInstance.SetData("delete from counter;");
          //  _castCounter = 0;

            string appPath = Global.Instance.APP_PATH.TrimEnd(new[] { '\\' });
            int index = appPath.LastIndexOf('\\');

            /*
            Process.Start(appPath.Substring(0, index) +
                          GetPathToCommonFile(AppManager.Configuration["System"]["EncryptBatchFilePath"]).Remove(0, 2));
            */

            string backupDir = AppManager.Configuration["System"]["BackupFlashDrivePath"];

            /*if (File.Exists(backupDir))
            {
                ProcessFile(backupDir);
            }
            else if (Directory.Exists(backupDir))
            {
                ProcessDirectory(backupDir);
            }
            else
            {
                Logger.Instance.Write(backupDir + " is not a valid file or directory.");
            }*/

            // Jim Kapsis.
            list_ballots.ForEach(ball => ball.Dispose());
            list_ballots.Clear();

            defBallots.ForEach(defBall => defBall.Dispose());
            defBallots.Clear();

            SetScreen("initial");
            Thread.Sleep(500);
            Instance.ShowSoftPinpad(null, true);
        }


        private void ProcessDirectory(string targetDirectory)
        {
            string[] fileEntries = Directory.GetFiles(targetDirectory);
            foreach (string fileName in fileEntries)
                ProcessFile(fileName);

            string[] subdirectoryEntries = Directory.GetDirectories(targetDirectory);
            foreach (string subdirectory in subdirectoryEntries)
                ProcessDirectory(subdirectory);
        }

        private void ProcessFile(string path)
        {
            string ext = Path.GetExtension(path);
            string encFile = path.Replace(ext, ext + ".aes");

            if (ext == ".aes")
            {
                return;
            }

            Crypto.Instance.EncryptFile(path, encFile, Encoding.ASCII.GetBytes("VotRite2017"));
            Logger.Instance.Write("Encrypted " + path + " file");
        }

        /*public void FinalizeApp()
        {
            DataManager.VotingResultsDataInstance.CloseConnection();
        }*/

        private void SetCounter()
        {
            string tq = "begin transaction; ";

            if (ballot.slatesDefinition.Slates.Count > 0 && ballot.slatesDefinition.Data.SlateId > 0)
            {
                tq += "insert into counter(bid, vdate, cnt, slate_id, voter) " +
                                  "values(" + ballot.Id + ", '" + DateTime.Now.Date.ToString("d") + "', " + 1 + ", " + ballot.slatesDefinition.Data.SlateId + ",'"+ Session.Id + "'); ";
            }

            foreach (ContestDefinition contest in ballot.ContestsList)
            {
                if (ballot.slatesDefinition.Slates.Count > 0 && ballot.slatesDefinition.Data.SlateId > 0 && contest.Type == ContestTypes.R)
                { continue; }
                string q;
                float count;

                if (contest.Propositions == null)
                {
                    foreach (DataDefinition data in contest.Data)
                    {
                        if ((ballot.ElectionType != ElectionTypes.ranking_choice) && (contest.Type != ContestTypes.V))
                        {
                            if (data.State == VrSelection.SelectionState.DESELECTED)
                                continue;
                        }
                        
                        Candidate candidate = contest.CandidatesList.Find(cand => cand.Id == data.Id);

                        string today = DateTime.Now.Date.ToString("d");
                        q = "select cnt from counter " +
                            "where bid=" + ballot.Id +
                            " and cid=" + contest.Id +
                            ((candidate != null || contest.Type != ContestTypes.R) 
                                 ? (" and did=" + data.Id)
                                 : " and cand_name='" + Helper.EscapeStringData(data.Text) + "'") +
                            " and vdate='" + today + "';";
                        count = Helper.Cast(DataManager.VotingResultsDataInstance.GetScalarData(q), 0f);

                        //if (count > 0f)
                        //{
                        //    count += (ballot.ElectionType == ElectionTypes.standard)
                        //                 ? 1f
                        //                 : TenantVoiceWeight;
                        //    tq += "update counter set cnt=" + count.ToString(DefaultCultureInfo) +
                        //          " where bid=" + ballot.Id +
                        //          " and cid=" + contest.Id +
                        //          (candidate != null || contest.Type != ContestTypes.R
                        //               ? (" and did=" + data.Id)
                        //               : " and cand_name='" + Helper.EscapeStringData(data.Text) + "'") +
                        //          " and vdate='" + today + "'; ";
                        //}
                        //else
                        //{
                            count = (ballot.ElectionType == ElectionTypes.standard)
                                        ? 1f
                                        : TenantVoiceWeight;
                            int iPartyID = data.PartyId;
                            if (iPartyID == 0)
                            {
                                for (int k = 0; k < contest.Data.Count; k++)
                                {
                                    if (contest.Data[k].PartyId != 0)
                                    {
                                        iPartyID = contest.Data[k].PartyId;
                                        break;
                                    }
                                }
                            }

                            if (ballot.HasCounties && ballot.CountyId > 0)
                            {
                                tq += "insert into counter(bid, cid, " +
                                  ((candidate != null || contest.Type != ContestTypes.R) && !data.IsWritten ? " did" : "cand_name") +
                                  ", vdate, cnt, pid, preference, county_id,voter) " +
                                  "values(" + ballot.Id + ", " + contest.Id + ", " +
                                  ((candidate != null || contest.Type != ContestTypes.R) && !data.IsWritten
                                       ? "" + data.Id
                                       : "'" + Helper.EscapeStringData(data.Text) + "'") +
                                  ", '" + DateTime.Now.Date.ToString("d") + "', " + count.ToString(DefaultCultureInfo) +
                                  "," + iPartyID + ", " + data.Preference + ", " + ballot.CountyId + ",'" + Session.Id + "'); ";
                            } else
                            {
                                tq += "insert into counter(bid, cid, " +
                                  ((candidate != null || contest.Type != ContestTypes.R) && !data.IsWritten ? " did" : "cand_name") +
                                  ", vdate, cnt, pid, preference,voter) " +
                                  "values(" + ballot.Id + ", " + contest.Id + ", " +
                                  ((candidate != null || contest.Type != ContestTypes.R ) && !data.IsWritten
                                       ? "" + data.Id
                                       : "'" + Helper.EscapeStringData(data.Text) + "'") +
                                  ", '" + DateTime.Now.Date.ToString("d") + "', " + count.ToString(DefaultCultureInfo) +
                                  "," + iPartyID + ", " + data.Preference + ",'" + Session.Id + "'); ";
                            }
                       // }
                    }
                }
                else
                {
                    int idx = 0;
                    foreach (Proposition proposition in contest.Propositions)
                    {
                        foreach (DataDefinition data in contest.Data)
                        {
                            if (data.State == VrSelection.SelectionState.SELECTED && contest.Id + idx * 7 < data.Id &&
                                data.Id < contest.Id + (idx + 1) * 7)
                            {
                                string today = DateTime.Now.Date.ToString("d");
                                int prefSel = (data.Text == "YES" ? 1 : 0);
                                q = "select cnt from counter " +
                                    "where bid=" + ballot.Id +
                                    " and cid=" + proposition.ContestId +
                                    " and did=" + data.Id +
                                    " and vdate=\"" + today + "\"" +
                                    " and voter='"+Session.Id+"';";

                                count = Helper.Cast(DataManager.VotingResultsDataInstance.GetScalarData(q), 0f);

                                if (count > 0f)
                                {
                                    count += (ballot.ElectionType == ElectionTypes.standard)
                                                 ? 1f
                                                 : TenantVoiceWeight;
                                    tq += "update counter set cnt=" + count.ToString(DefaultCultureInfo) + ", preference=" + prefSel + 
                                          " where bid=" + ballot.Id +
                                          " and cid=" + proposition.ContestId +
                                          " and did=" + data.Id +
                                          " and vdate=\"" + today + "\"; " ;
                                }
                                else
                                {
                                    count = (ballot.ElectionType == ElectionTypes.standard)
                                        ? 1f
                                        : TenantVoiceWeight;

                                    if (ballot.HasCounties && ballot.CountyId > 0)
                                    {
                                        tq += "insert into counter(bid, cid, did, vdate, cnt, county_id,voter,preference) " +
                                         "values(" + ballot.Id + ", " + proposition.ContestId + ", " +
                                         data.Id + ", \"" + DateTime.Now.Date.ToString("d") + "\", " +
                                         count.ToString(DefaultCultureInfo) + ", " + ballot.CountyId + ",'" + Session.Id + "',"+ prefSel + "); ";
                                    } else
                                    {
                                        tq += "insert into counter(bid, cid, did, vdate, cnt,voter,preference) " +
                                         "values(" + ballot.Id + ", " + proposition.ContestId + ", " +
                                         data.Id + ", \"" + DateTime.Now.Date.ToString("d") + "\", " +
                                         count.ToString(DefaultCultureInfo) + ",'" + Session.Id + "'," + prefSel + "); ";
                                    }                                       
                                }
                            }
                        }
                        idx++;
                    }
                }
            }

            tq += "commit;";

            DataManager.VotingResultsDataInstance.SetData(tq);
            DataManager.VotingResultsDataInstance.SetData(tq.Replace("counter", "counter_bkup"));

            try
            {
                string destFolder = new DirectoryInfo(Global.Instance.APP_PATH).Parent.FullName + "\\Ballot_Edit\\records";
                string recordsFolderPath = Global.Instance.APP_PATH + AppManager.Configuration["System"]["Machine"] + "records\\";
                string[] files = Directory.GetFiles(recordsFolderPath, Session.Id + "*.png");
                foreach (string file in files)
                {
                    try
                    {
                        var destfile = Path.Combine(destFolder, Path.GetFileName(file));
                        File.Copy(file, destfile);
                    }
                    catch (Exception ex)
                    {

                    }

                }
            }
            catch (Exception)
            {

            }
        }

        public void Terminate()
        {
            CloseSession();
            CloseBallot();
            //DisposeSpeechToTextEngine();

            if (Convert.ToBoolean(AppManager.Configuration["System"]["RemotePollWorker"]))
            {
// #if !DEBUG
                if (hPinPad != null)
                {
                    hPinPad.Deactivate();
                }

//                counter.Deactivate();
// #endif
            }
            //			else
            PinpadView.Menu = null;

            try
            {
                Taskbar.Show();
            }
            catch (Exception)
            {
            }

            HiddenFolder(false);

            Window.Instance.Terminate();
            Logger.Instance.Write("Terminating the application");
            Driver.Exit();
        }


        
        /// <summary>
        /// Handle on-screen touch
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="evt"></param>
        private void HandleSpeech(int streamNumber, object streamPosition, SpeechRecognitionType recognitionType,
                                  ISpeechRecoResult result)
        {
            if (ActiveScreens.Count > 0)
                GetActiveScreen().HandleSpeech(streamNumber, streamPosition, recognitionType, result);
        }

        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern bool Wow64DisableWow64FsRedirection(ref IntPtr ptr);

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern bool Wow64RevertWow64FsRedirection(IntPtr ptr);


        internal void ChangeTextSize(VrLabel pLabel)
        {
            string sTextSize = AppManager.Configuration["TextSize"]["Value"];

            if (sTextSize != "")
            {
                pLabel.TextSize = int.Parse(sTextSize);
                pLabel.Draw(Window.Instance.CreateGraphics());
            }
        }

        internal void ChangeTextSize(ScreenObject pObject)
        {
            string sTextSize = AppManager.Configuration["TextSize"]["Value"];
            if (sTextSize != "")
            {
                pObject.TextSize = int.Parse(sTextSize);
                pObject.Draw(Window.Instance.CreateGraphics());
            }
        }

        public void ShowResults()
        {
            Ballot b = Ballot.GetBallot();
            ballot = DefinitionParser.Instance.FillBallotContent(null, b);
            OpenBallot(ballot, false);
            OpenSession();

            List<Locale> locales = Locale.FetchLocales(b.Id);
            Locale l = null;

            foreach (Locale loc in locales)
            {
                if (loc.Id == 1)
                {
                    l = loc;
                    break;
                }
            }

            Session.SetLocale(l);
            SetScreen("results");
        }
    }
}