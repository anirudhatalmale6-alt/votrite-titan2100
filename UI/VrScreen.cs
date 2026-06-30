// Product:	VotRite
// Module:  VrScreen.cs
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
using System.IO;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Collections.Generic;
using System.Linq;
using SpeechLib;
using VotRite.MVC;
using VotRite.Models;
using VotRite.Util;
//
using System.Speech;
using System.Diagnostics;
using System.Timers;

namespace VotRite.UI
{
    class VrScreen : View
    {
        private const int ALPHA = 220;
        private const int RGB_R = 10;
        private const int RGB_G = 10;
        private const int RGB_B = 10;

        private string name;
        private SizeF scale;

        private ScreenModel Model { get { return (ScreenModel)model; } }
        //private new ScreenModel model ;
        private Controller controller;
        private VotRite.Controllers.ReviewController vController { get { return controller as VotRite.Controllers.ReviewController; } }
        private HashSet<String> _possibleWords = null;
        private IEnumerable<string> _additionalSet;
        private List<String[]> _commandlist = null;

        Process p = new Process();

        public string Name { get { return name; } set { name = value; } }

        Timer timer = new Timer();
        private static string modelname;
        private int spokencount_sel = 0;
        private int spokencount_com = 0;
        private int choicesLeft = 0;
        private string currentcommand = "";
        private DateTime time_of_timerStarted = new DateTime();

        enum listeningmode
        {
            waitingvoter, notwaitingvoter
        }
        private listeningmode _listeningmode = listeningmode.notwaitingvoter;
        public VrScreen(Model m, Controller c)
            : base(m, c)
        {
            model = (ScreenModel)m;
            controller = c;

            SizeF defSz = new Size(
                Convert.ToInt32(AppManager.Configuration["Display"]["width"]),
                Convert.ToInt32(AppManager.Configuration["Display"]["height"]));

            scale = new SizeF(Window.DisplaySize.Width / defSz.Width,
                              Window.DisplaySize.Height / defSz.Height);

            Model.Definition.Width = (int)(Model.Definition.Width * scale.Width);
            Model.Definition.Height = (int)(Model.Definition.Height * scale.Height);
            Model.Definition.Left = (int)(Model.Definition.Left * scale.Width);
            Model.Definition.Top = (int)(Model.Definition.Top * scale.Height);

            if (!Model.Definition.IsDialog)
            {
                Window.Instance.Width = Model.Definition.Width;
                Window.Instance.Height = Model.Definition.Height;
                Window.Instance.Left = Model.Definition.Left;
                Window.Instance.Top = Model.Definition.Top;
            }
            modelname = Model.Definition.Name;
            Model.Scale = scale;
            _commandlist = new List<string[]>();
            Model.Notify();

            Logger.Instance.Write("Screen " + Model.Definition.Name + " initialized");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (_possibleWords != null) _possibleWords.Clear();
                
                try
                {
                    // if (p != null) p.Kill();
                    timer.Dispose();

                }
                catch (Exception)
                {

                }
               
            }

            base.Dispose(disposing);
        }

        public void HandleTouch(int x, int y)
        {
            //Console.WriteLine("x=" + x + ":y=" + y);
            controller.HandleTouch(x, y);
        }

        public void HandleSpeech(int streamNumber, object streamPosition, SpeechRecognitionType recognitionType, ISpeechRecoResult result)
        {
            // if it's low confidence level on the command, just leave
            ////if (result.PhraseInfo.Rule.Confidence == SpeechEngineConfidence.SECLowConfidence)
            ////    return;

            // VoiceCommands vc = VoiceCommands.NONE;
            //var recogWord = result.PhraseInfo.GetText(0, result.PhraseInfo.Elements.Count, false);
            var recogWord = result.PhraseInfo.GetText(0, -1, true);

            controller.HandleSpeech(recogWord);
        }

        public bool CheckIfVoiceCommand( string command, out string exactcommand)
        {
            bool ret = false;
            exactcommand = command;
            string[] twos = {"two", "to", "too", "through","tough","true","blue","thru"};
            string[] fours = { "four", "fore", "for", "fire" };
            string[] threes = { "three", "free", "tree", "there","their" };
            string[] tens = { "ten", "then", "than"};
            string[] zeeros = { "zero", "hero" };
            string[] reviews = { "check","cheque", "jet", "jack","jag" };

            if (zeeros.Contains(command))
            {
                exactcommand = "Repeat Instruction";
                return true;
            }
            if (reviews.Contains(command))
            {
                exactcommand = "Review Your Choices";
                return true;
            }

            if (twos.Contains(command))
                command = "two";
            if (threes.Contains(command))
                command = "three";
            if (fours.Contains(command))
                command = "four";
            if (tens.Contains(command))
                command = "ten";

            var checkstr = "say " + command.ToLower() + " for ";
            if (_additionalSet.Contains(command.ToUpper()))
                checkstr = command;
            AppManager.Instance._possibleWords = AppManager.Instance._possibleWords.Distinct().ToList();
            AppManager.Instance._possibleWords.RemoveAll(s => s == null);
            var exactComm = AppManager.Instance._possibleWords.Where(a => a.ToLower().StartsWith(checkstr)).ToList();
            if(exactComm.Count() > 0)
            {
                if (_additionalSet.Contains(command.ToUpper()))
                    exactcommand = exactComm[0];
                else
                    exactcommand = exactComm[0].Replace(checkstr, "");
                ret = true;
            }
            return ret;
        }

        public void HandleMouseUp()
        {
            controller.HandleMouseUp();
        }

        public void HandleMouseDown_Left()
        {
            controller.HandleMouseDown_Left();
        }
        public void HandleMouseDown_Right()
        {
            controller.HandleMouseDown_Right();
        }

        public void HandleKey(string key)
        {
            /*switch (key)
            {
                case "F1": break;
                default: break;	
            }*/

            controller.HandleKey(key);
        }

        

        public void ReDraw(Graphics gr)
        {
            if (true)
            {
                if (Model.Definition.IsDialog)
                {
                    Size ws = Window.Instance.Size;
                    
                    if (Model.ScreenShot == null) CreateScreenShot(ws);
                    Rectangle scrRect = new Rectangle(0, 0, ws.Width, ws.Height);

                    gr.DrawImage(Model.ScreenShot, scrRect);
                }

                Rectangle screenRect = new Rectangle(Model.Definition.Left,
                                                     Model.Definition.Top,
                                                     Model.Definition.Width,
                                                     Model.Definition.Height);

                if (Model.Definition.Background != null)
                {
                    Image bgImage;

                    try
                    {
                        using (bgImage = Image.FromFile(
                            AppManager.GetPathToCommonFile(Model.Definition.Background)))
                        {
                            gr.InterpolationMode = InterpolationMode.NearestNeighbor;
                            gr.PixelOffsetMode = PixelOffsetMode.Half;
                            gr.DrawImage(bgImage,
                                screenRect, 0, 0,
                                bgImage.Width,
                                bgImage.Height,
                                GraphicsUnit.Pixel);
                            gr.InterpolationMode = InterpolationMode.Default;   //  reset interpolation
                            //  for screen objects
                        }
                    }
                    catch (IOException e) { Logger.Instance.Write(e); }
                }

                if (Model.Definition.BgColor != null)
                {
                    gr.FillRectangle(
                         new SolidBrush(
                             ColorTranslator.FromHtml(Model.Definition.BgColor)),
                             screenRect);
                }

                //created = true;
            }

            foreach (ScreenObject obj in Model.Definition.ScreenObjects)
            {
                if (obj != null)
                {
                    if (!obj.Resized) obj.Resize(this.scale);

                    if (obj.GraphicsState == ScreenObject.ScreenObjectGraphicsState.CHANGED)
                    {
                        if (obj.Text != null)
                        {
                            //if (obj.Text.Contains("$"))
                            //{
                            //    if (obj.Text.Contains("$sys_"))
                            //        obj.Text = IOHandler.GetSysVar(obj.Text);
                            //    else
                            //        obj.Text = IOHandler.GetVar(obj.Text, AppManager.Instance.GetLocale());
                            //}
                        }

                        obj.Draw(gr);
                    }
                }
            }
        }

        public void CreateScreenShot(Size ws)
        {
            try
            {
                Bitmap bmp = new Bitmap(ws.Width, ws.Height);
                
                using (Graphics copyGr = Graphics.FromImage(bmp))
                {
                    Rectangle scr = new Rectangle(0, 0, ws.Width, ws.Height);
                    SizeF dpi = new SizeF(copyGr.DpiX, copyGr.DpiY);
                    SizeF scale = new SizeF()
                    {
                        Width = dpi.Width / 96f,
                        Height = dpi.Height / 96f
                    };

                    scr.Width *= (int)scale.Width;
                    scr.Height *= (int)scale.Height;

                    copyGr.CopyFromScreen(0, 0, 0, 0, scr.Size);
                    copyGr.SetClip(scr);
                    /*copyGr.FillRectangle(new SolidBrush(Color.FromArgb(
                        ALPHA, RGB_R, RGB_G, RGB_B)), scr);*/

                    copyGr.FillRectangle(new SolidBrush(Color.Black), scr);

                    Model.ScreenShot = bmp;
                }
            }
            catch (Exception e) { Logger.Instance.Write(e); }
        }

        protected void CreateListOfCommands()
        {
            CreateListOfCommands(null);
        }

        protected void CreateListOfCommands(IEnumerable<string> additionalSet)
        {
            
            var session = AppManager.Instance.Session;
            //if ((session != null && session.BallotMode == Session.BallotModes.Audio) ||
            //    (session == null && AppManager.Instance.ballot.BallotMode == Session.BallotModes.Audio))
            if (AppManager.Instance.ballot.BallotMode == Session.BallotModes.Audio)
            {
                if (AppManager.Instance.repeatInstruction == AppManager.repeatInstructionCommand.none
                || AppManager.Instance.repeatInstruction == AppManager.repeatInstructionCommand.repeat)
                {
                    _commandlist.Add(new string[] { "Repeat", "instruction" });
                }
                if (additionalSet == null)
                {
                    additionalSet = new HashSet<string>();
                }
               
                _additionalSet = additionalSet;
                
                foreach (var cmdd in additionalSet)
                {
                    _commandlist.Add(new string[] { cmdd, "button" });
                }
                _possibleWords = new HashSet<string>();
                int y = 1;
                //if (modelname.ToLower() == "confirm")
                //    _commandlist.Add(new string[] { "to complete voting without hear review", "selection" });

                foreach (var screenObject in Model.Definition.ScreenObjects)
                {
                    if (screenObject == null)
                        continue;
                   
                    //if (screenObject.Visible == false)
                    //    continue;
                        if (screenObject is VrContainer)
                    {
                        int x = 1;
                        foreach (var control in (screenObject as VrContainer).Controls.
                            Where(control => (control is VrButton || control is VrSelection) && control.Text != null))
                        {
                           
                            _possibleWords.Add("say " + AppManager.ReplaceNumberToEnglish(x.ToString() + ".", true) + " for " +control.Text);

                            string cmd = control.Text;
                            if (cmd == "btn_add_candidates")
                                continue;
                            if (!AppManager.Instance._possibleWords.Contains(cmd) && modelname.ToLower() != "review")
                            {
                                AppManager.Instance._possibleWords.Add(cmd);
                                _commandlist.Add(new string[] { cmd, "selection" });
                                if (control is VrSelection)
                                {
                                    if (((VrSelection)control).State == VrSelection.SelectionState.SELECTED)
                                    {
                                        if (!AppManager.Instance._contestSelections.Contains(control.Text))
                                            AppManager.Instance._contestSelections.Add(control.Text);
                                    }
                                }
                            }

                            if ((control.Tag != null && control.Tag != control.Text) && modelname.ToLower() != "review")
                            {
                                _possibleWords.Add(control.Tag);
                                _commandlist.Add(new string[] { control.Tag, "selection" });
                            }
                            x++;
                        }
                    }
                    else if ((screenObject is VrButton || screenObject is VrSelection) && screenObject.Text != null)
                    {
                        if (screenObject.Text == "." || screenObject.Text == "-")
                        {
                            _possibleWords.Add(screenObject.Tag);
                        }
                        else
                        {
                            //if (modelname.ToLower() != "contest" && modelname.ToLower() != "rankingchoice" && modelname.ToLower() != "slate")
                            //{
                            string cmd = screenObject.Text;
                            if (screenObject.Visible == false)
                                continue;
                            if (cmd == "")
                                continue;
                            if (additionalSet.Contains(cmd))
                                continue;
                            if (cmd == "btn_add_candidates")
                                continue;
                            //if(modelname.ToLower() == "locale")
                            //    cmd = screenObject.Tag;

                            if (!AppManager.Instance._possibleWords.Contains(cmd) && (screenObject is VrButton))
                            {
                                if (screenObject.Text != "--Click for Ballot Display Settings--" && screenObject.Text != "btn_add_candidates")
                                {
                                    AppManager.Instance._possibleWords.Add(cmd);
                                    if (modelname.ToLower() == "review")
                                    {
                                        string[] reviewbtns = {"print review in normal text size", "print review in zoom text size", "accept and print ballot, you will have a second opportunity to review on the next page",  "Repeat review of candidates", "next", "return to voting", "cancel ballot" };
                                        if (reviewbtns.Contains(cmd.ToLower()))
                                            _commandlist.Add(new string[] { cmd, (screenObject is VrButton ? "button" : "selection") });
                                    }
                                    else
                                        _commandlist.Add(new string[] { cmd, (screenObject is VrButton ? "button" : "selection") });
                                    y++;
                                }
                            }

                            if (screenObject.Tag != null && screenObject.Tag != screenObject.Text)
                            {
                                _possibleWords.Add(screenObject.Tag);
                                _commandlist.Add(new string[] { screenObject.Tag, (screenObject is VrButton ? "button" : "selection") });
                            }
                            //}
                        }
                    }
                }

                
                AppManager.Instance._possibleWords = new List<string>();
                AppManager.Instance._possibleWords.AddRange(_additionalSet);

                int repeatTry = 0;
                AppManager.Instance._commandList.Clear();
                AppManager.Instance._commandList = _commandlist;
                ResetGrammer:
                try
                {                    
                    File.WriteAllLines(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "speechgrammer.txt"), _possibleWords.ToArray());
                }
                catch (Exception)
                {
                    repeatTry++;
                    System.Threading.Thread.Sleep(500);
                    if (repeatTry < 5)
                        goto ResetGrammer;
                   // throw;
                }
            }
        }

        protected void choicesleft_selection(int count)
        {
            choicesLeft = count;
            
        }
        protected void AddSpeechToTextEngine()
        {
            try
            {
                var session = AppManager.Instance.Session;
                //if ((session != null && session.BallotMode == Session.BallotModes.Audio) ||
                //    (session == null && AppManager.Instance.ballot.BallotMode == Session.BallotModes.Audio))
                if ( AppManager.Instance.ballot.BallotMode == Session.BallotModes.Audio)
                {

                    //initiateVotriteSpeech();
                    return;

                    ISpeechGrammarRule grammarRule;
                    if (session != null)
                    {
                        grammarRule = session.GrammarRule;
                        if (grammarRule == null)
                            grammarRule = AppManager.Instance.GrammarRule;
                    }
                    else
                    {
                        if (AppManager.Instance.Grammar == null)
                        {
                            return;
                        }
                        grammarRule = AppManager.Instance.Grammar.Rules.FindRule(1);
                    }

                    object propValue = "";
                    if (grammarRule != null)
                        grammarRule.Clear();
                    else
                        return;
                    int iSerial = 0;
                    foreach (var possibleWord in _possibleWords)
                    {
                        string sTmpWord = possibleWord;
                        if (possibleWord.ToLower().IndexOf("touch here") > -1)
                        {
                            sTmpWord = possibleWord.ToLower().Replace("touch here", "say here");
                        }
                        sTmpWord = AppManager.ReplaceNumberToEnglish(sTmpWord, true).Replace("-", " ");
                        iSerial++;
                        ////grammarRule.InitialState.AddWordTransition(null, possibleWord, " ", SpeechGrammarWordType.SGLexical, possibleWord, 1, ref propValue, 1.0F);
                        //grammarRule.InitialState.AddWordTransition(null, possibleWord, " ", SpeechGrammarWordType.SGLexical, possibleWord, iSerial, ref propValue, 1.0F);
                        //grammarRule.InitialState.AddWordTransition(null, sTmpWord, " ", SpeechGrammarWordType.SGLexical, sTmpWord, iSerial, ref propValue, 1.0F);
                    }


                   // AppManager.Instance.Grammar.Rules.Commit();
                    //AppManager.Instance.Grammar.CmdSetRuleIdState(1, SpeechRuleState.SGDSActive);
                }
            }
            catch (Exception)
            {

                // throw;
            }
        }

      
        protected void initiateVotriteSpeech()
        {
            try
            {
                File.WriteAllText(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "speechout.txt"), "");
            }
            catch (Exception)
            {
            }
            try
            {
                AppManager.Instance.stopVotriteSpeech();
                // Redirect the output stream of the child process.
                p.StartInfo.CreateNoWindow = true;
                p.StartInfo.UseShellExecute = false;
                //p.StartInfo.RedirectStandardOutput = true;
                p.StartInfo.FileName = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "VotriteSpeech.exe");
                p.Start();

                timer.Elapsed += Timer_Elapsed;
                timer.Interval = 1000;
                timer.Start();
            }
            catch (Exception)
            {
            }
        }

        private void Timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            try
            {
                string filepath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "speechout.txt");
                if (AppManager.Instance.speaker.Status.RunningState == SpeechRunState.SRSEIsSpeaking)
                {
                    File.WriteAllText(filepath, "");
                    return;
                }
                double spantime = DateTime.Now.Subtract(time_of_timerStarted).TotalSeconds;
                if ((_listeningmode == listeningmode.waitingvoter) &&  spantime >= 15)
                {
                    _listeningmode = listeningmode.notwaitingvoter;
                    if(choicesLeft > 0 )
                    {
                        if (spokencount_sel > 0)
                            spokencount_sel--;
                    }
                    else
                    {
                        if (spokencount_com > 0)
                            spokencount_com--;
                    }
                }

                if (_listeningmode == listeningmode.notwaitingvoter && choicesLeft >= 0)
                {
                    if (choicesLeft > 0)
                    {
                        _listeningmode = listeningmode.waitingvoter;
                        time_of_timerStarted = DateTime.Now;
                        var sels = _commandlist.Where(a => a[1] == "selection").ToList();
                        if (sels.Count > 0)
                        {
                            if (spokencount_sel >= sels.Count)
                                spokencount_sel = 0;
                            currentcommand = sels[spokencount_sel][0];
                            AppManager.Instance.StartSpeaker(currentcommand + ".");
                            AppManager.Instance.StartSpeaker(".. Say yes to choose.");
                            AppManager.Instance.StartSpeaker(".. Say no to move next.");
                        }
                        
                        spokencount_sel++;
                        File.WriteAllText(filepath, "");
                        
                        return;
                    }

                    else
                    {
                        _listeningmode = listeningmode.waitingvoter;
                        time_of_timerStarted = DateTime.Now;
                        var sels = _commandlist.Where(a => a[1] == "button").ToList();
                        if (sels.Count > 0)
                        {
                            if (spokencount_com >= sels.Count)
                                spokencount_com = 0;
                            currentcommand = sels[spokencount_com][0];
                            AppManager.Instance.StartSpeaker(currentcommand + ".");
                            AppManager.Instance.StartSpeaker(".. Say yes to choose.");
                            AppManager.Instance.StartSpeaker(".. Say no to move next.");
                        }
                        
                        spokencount_com++;
                        File.WriteAllText(filepath, "");
                        
                        return;
                    }
                }
                
                string word = File.ReadAllText(filepath);
                File.WriteAllText(filepath, "");

                if (word != "" && _listeningmode == listeningmode.waitingvoter)
                {
                    string[] yess = { "yes", "yeah", "yep", "yis","use" };
                    string[] nos = { "no", "nah", "not","note" };

                    //if (CheckIfVoiceCommand(word, out word))
                    if (nos.Contains(word.ToLower()))
                    {
                        _listeningmode = listeningmode.notwaitingvoter;
                        return;
                    }
                        if (yess.Contains(word.ToLower()))
                    {
                        word = currentcommand;
                        timer.Stop();
                        if (word == "Repeat Instruction")
                        {
                            AppManager.Instance.StopSpeaker();
                            AppManager.Instance.StartSpeaker("Repeating the instructions");
                            System.Threading.Thread.Sleep(1000);
                            foreach (var text in AppManager.Instance._speechWords.Where(text => text != null))
                            {
                                AppManager.Instance.StartSpeaker(text);
                            }
                        }
                        else if (word.ToLower() == "review your choices")
                        {
                            AppManager.Instance.StartSpeaker("Review is accepted");
                            //System.Threading.Thread.Sleep(1000);
                            //timer.Stop();
                            //controller.HandleSpeech("Review your Choices");
                            AppManager.Instance.SetScreen("review");
                            
                        }
                        //timer.Stop();
                        else
                        {
                            AppManager.Instance.StartSpeaker(word + " is accepted");
                            //System.Threading.Thread.Sleep(1000);
                            //timer.Stop();
                            if (word.ToLower() == "return to voting")
                            {
                               if(AppManager.Instance.vController != null)
                                    AppManager.Instance.vController.HandleSpeech(word);
                            }
                            else
                                controller.HandleSpeech(word);
                            //timer.Start();
                            try
                            {
                                //p.Close();
                                //p.Kill();

                            }
                            catch (Exception)
                            {

                            }
                        }
                        //AppManager.Instance.StartSpeaker(word + " is voted");
                        timer.Start();
                    }
                }
            }
            catch (Exception)
            {
            }
        }

        protected List<string> GetAllTexts(bool useTagsForSelection, bool useTagsForButtons, bool speakInvisible)
        {
            var result = new List<string>();
            var session = AppManager.Instance.Session;
            //if ((session != null && session.BallotMode == Session.BallotModes.Audio) ||
            //    (session == null && AppManager.Instance.ballot.BallotMode == Session.BallotModes.Audio))
            if (AppManager.Instance.ballot.BallotMode == Session.BallotModes.Audio)
            {
                int y = 1;
                foreach (var screenObject in Model.Definition.ScreenObjects)
                {
                    if (screenObject == null)
                        continue;
                    if (screenObject.Visible == false)
                        continue;

                    if (screenObject is VrContainer)
                    {
                        //if (modelname.ToLower() == "review" || modelname.ToLower() == "confirm")
                        //    result.AddRange((screenObject as VrContainer).Controls.
                        //        Select(control => GetControlText(useTagsForSelection, useTagsForButtons, speakInvisible, control)).Where(text => text != null));

                        int x = 1;
                        foreach (var selObject in (screenObject as VrContainer).Controls.Where(text => text != null))
                        {
                            if (modelname.ToLower() != "review" && modelname.ToLower() != "confirm")
                            {
                                //result.Add(GetControlText(useTagsForSelection, useTagsForButtons, speakInvisible, selObject, false, x));
                                //x++;
                            }
                            else
                            {
                                if (AppManager.Instance.reviewOnConfirm)
                                {
                                    string currText = "";
                                    if (selObject is VrSelection)
                                    {
                                        currText = GetControlText_selection(useTagsForSelection, useTagsForButtons, speakInvisible, selObject);
                                    }
                                    else
                                        currText = GetControlText(useTagsForSelection, useTagsForButtons, speakInvisible, selObject);

                                    //result.Add(currText);

                                    if (selObject.Top + selObject.Height > (screenObject as VrContainer).Top + (screenObject as VrContainer).Height)
                                        AppManager.Instance._reviewSelections.Add(new string[] { currText, "scroll" });
                                    else
                                        AppManager.Instance._reviewSelections.Add(new string[] { currText, "" });
                                }
                            }
                        }
                    }
                    else
                    {
                        var text = GetControlText(useTagsForSelection, useTagsForButtons, speakInvisible, screenObject);
                        if ((modelname.ToLower() == "locale" || modelname.ToLower() == "review" || modelname.ToLower() == "confirm") && screenObject is VrButton)
                        {
                            continue;
                            //if (screenObject.Text != "--Click for Ballot Display Settings--")
                            //{
                            //    text = GetControlText(useTagsForSelection, useTagsForButtons, speakInvisible, screenObject, false, y);
                            //    y++;
                            //}
                        }

                        if (text != null)
                            result.Add(text);
                    }
                }
            }
            return result;
        }

        protected List<string> GetAllTexts_Locale(bool useTagsForSelection, bool useTagsForButtons, bool speakInvisible)
        {
            var result = new List<string>();
            var session = AppManager.Instance.Session;
            //if ((session != null && session.BallotMode == Session.BallotModes.Audio) ||
            //    (session == null && AppManager.Instance.ballot.BallotMode == Session.BallotModes.Audio))
            if (AppManager.Instance.ballot.BallotMode == Session.BallotModes.Audio)
            {
                int y = 1;
                foreach (var screenObject in Model.Definition.ScreenObjects)
                {
                    if (screenObject == null)
                        continue;
                    if (screenObject.Visible == false)
                        continue;

                    if (screenObject is VrContainer)
                    {
                        //result.AddRange((screenObject as VrContainer).Controls.
                        //    Select(control => GetControlText(useTagsForSelection, useTagsForButtons, speakInvisible, control)).Where(text => text != null));

                        //int x = 1;
                        //foreach (var selObject in (screenObject as VrContainer).Controls.Where(text => text != null))
                        //{
                        //    if (modelname.ToLower() != "review" && modelname.ToLower() != "confirm")
                        //    {
                        //        result.Add(GetControlText(useTagsForSelection, useTagsForButtons, speakInvisible, selObject, false, x));
                        //        x++;
                        //    }
                        //    else
                        //        result.Add(GetControlText(useTagsForSelection, useTagsForButtons, speakInvisible, selObject));


                        //}
                    }
                    else
                    {
                        var text = GetControlText(useTagsForSelection, useTagsForButtons, speakInvisible, screenObject);
                        if ((modelname.ToLower() == "locale" || modelname.ToLower() == "review" || modelname.ToLower() == "confirm") && screenObject is VrButton)
                        {
                            if (screenObject.Text != "--Click for Ballot Display Settings--")
                            {
                                text = GetControlText(useTagsForSelection, useTagsForButtons, speakInvisible, screenObject, false, y);
                                if (text != null)
                                {
                                    if (text.Contains("for"))
                                    {
                                        var spl = text.Split(new string[] { "for" }, StringSplitOptions.None);
                                        text = spl[0] + "for vote in " + screenObject.Name;
                                            result.Add(text);
                                    }
                                }
                                y++;
                            }
                        }

                        
                    }
                }
            }
            return result;
        }


        protected List<string> GetAllTexts(bool useTagsForSelection, bool useTagsForButtons, bool speakInvisible, bool pIsParties)
        {
            var result = new List<string>();
           
            var session = AppManager.Instance.Session;
            if ((session != null && session.BallotMode == Session.BallotModes.Audio) ||
                (session == null && AppManager.Instance.ballot.BallotMode == Session.BallotModes.Audio))
            {
                foreach (var screenObject in Model.Definition.ScreenObjects)
                {
                    if (screenObject == null)
                        continue;
                    if (screenObject.Visible == false)
                        continue;
                    if (screenObject is VrContainer)
                    {
                        //result.AddRange((screenObject as VrContainer).Controls.
                        //    Select(control => GetControlText(useTagsForSelection, useTagsForButtons, speakInvisible, control, pIsParties)).Where(text => text != null));
                        //int x = 1;
                        foreach (var selObject in (screenObject as VrContainer).Controls.Where(text => text != null))
                        {
                            if (selObject is VrSelection)
                            {
                                if (((VrSelection)selObject).State == VrSelection.SelectionState.SELECTED)
                                {
                                    if (!AppManager.Instance._contestSelections.Contains(selObject.Text))
                                        AppManager.Instance._contestSelections.Add(selObject.Text);
                                }
                            }
                        }
                    }
                    else
                    {
                        if ((screenObject is VrLabel))
                        {
                            if (screenObject.Name == "lbl_contest_tip" && (AppManager.Instance.repeatInstruction == AppManager.repeatInstructionCommand.norepeat))
                                continue;
                                var text = GetControlText(useTagsForSelection, useTagsForButtons, speakInvisible, screenObject);
                            if (text != null && text.IndexOf("_") == -1)
                            {
                                if (screenObject.Name == "lbl_contest_tip")
                                {
                                    if (!AppManager.Instance.instructionSpoken)
                                        result.Add(text);
                                }
                                else
                                    result.Add(text);
                            }
                        }
                    }
                }
            }
            return result;
        }

        protected static string GetControlText(bool useTagsForSelection, bool useTagsForButtons, bool speakInvisible, ScreenObject control)
        {
            if (control == null || control.Text == null || (!control.Visible && !speakInvisible) ||
                ((control is VrLabel) && !(control as VrLabel).Speakable))
            {
                return null;
            }
            var text = "";
            if (control is VrButton || control is VrSelection)
            {
                return null;
                //if (modelname.ToLower() != "review")
                //    text += "Say ";
            }

            if (control.Tag != null &&
                (useTagsForSelection && control is VrSelection || useTagsForButtons && control is VrButton))
            {
                text += control.Tag;
            }
            else
            {
                text += control.Text;
            }
            return text.Replace("touch", "choose");
        }

        protected static string GetControlText(bool useTagsForSelection, bool useTagsForButtons, bool speakInvisible, ScreenObject control, bool pIsParties)
        {
            if (control == null || control.Text == null || (!control.Visible && !speakInvisible) ||
                ((control is VrLabel) && !(control as VrLabel).Speakable))
            {
                return null;
            }
            var text = "";
            if (control is VrButton || control is VrSelection)
            {
                text += "Say ";
            }

            text += control.Text;

            return text.Replace("touch", "choose");
        }

        protected static string GetControlText_selection(bool useTagsForSelection, bool useTagsForButtons, bool speakInvisible, ScreenObject control)
        {
            if (control == null || control.Text == null || (!control.Visible && !speakInvisible) ||
                ((control is VrLabel) && !(control as VrLabel).Speakable))
            {
                return null;
            }
            var text = "";
            

            text += control.Text;

            if (control is VrSelection)
            {
                if (((VrSelection)control).State == VrSelection.SelectionState.SELECTED)
                    text += " is voted";
            }

            return text.Replace("touch", "choose");
        }


        protected static string GetControlText(bool useTagsForSelection, bool useTagsForButtons, bool speakInvisible, ScreenObject control, bool pIsParties, int controlIndex)
        {
            if (control == null || control.Text == null || (!control.Visible && !speakInvisible) ||
                ((control is VrLabel) && !(control as VrLabel).Speakable))
            {
                return null;
            }
            var text = "";
            if (control is VrButton || control is VrSelection)
            {
                text += "Say "+ AppManager.ReplaceNumberToEnglish(controlIndex.ToString()+".",true) + " for ";
            }

            text += control.Text;

            return text.Replace("touch", "choose");
        }

    }
}