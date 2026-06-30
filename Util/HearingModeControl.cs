using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.IO;
using System.Timers;
using SpeechLib;
using VotRite.MVC;
using VotRite.Models;

namespace VotRite.Util
{
    public class HearingModeControl
    {
        
        private List<String[]> _commandlist = null;

        Process p = new Process();

        //public string Name { get { return name; } set { name = value; } }

        System.Timers.Timer timer = new System.Timers.Timer();
       private int spokencount_sel = 0;
        private int spokencount_com = 0;
        private int choicesLeft = 0;
        private string[] currentcommand;
        
        private DateTime time_of_timerStarted = new DateTime();
        private bool repeatcandidatenow = true;
        private bool reviewOrConfirm = false;
        bool reviewListspeak = false;
        enum listeningmode
        {
            waitingvoter, notwaitingvoter
        }
        private listeningmode _listeningmode = listeningmode.notwaitingvoter;

        public HearingModeControl()
        {

        }

        public HearingModeControl(bool _reviewOrconfirm)
        {
            reviewOrConfirm = _reviewOrconfirm;
            reviewListspeak = true;
        }

        public void initiateSpeech()
        {
            _commandlist = AppManager.Instance._commandList;
            choicesLeft = AppManager.Instance.choicesLeft;
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
                AppManager.Instance.hearingtimer = timer;
                timer.Elapsed += Timer_Elapsed;
                timer.Interval = 1000;
                timer.Start();
            }
            catch (Exception)
            {
            }
        }

        private void Timer_Elapsed_yesN0(object sender, ElapsedEventArgs e)
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
                if(spantime == 1)
                {
                    File.WriteAllText(filepath, "");
                    return;
                }
                if ((_listeningmode == listeningmode.waitingvoter) && spantime >= 15)
                {
                    _listeningmode = listeningmode.notwaitingvoter;
                    if (AppManager.Instance.choicesLeft > 0)
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

                if (_listeningmode == listeningmode.notwaitingvoter && AppManager.Instance.choicesLeft >= 0)
                {
                    if (AppManager.Instance.choicesLeft > 0)
                    {
                        //selection here
                        _listeningmode = listeningmode.waitingvoter;
                        time_of_timerStarted = DateTime.Now;
                        var sels = _commandlist.Where(a => a[1] == "instruction").ToList();
                        if (sels.Count > 0 && (AppManager.Instance.repeatInstruction == AppManager.repeatInstructionCommand.none || AppManager.Instance.repeatInstruction == AppManager.repeatInstructionCommand.repeat ))
                        {
                            if (spokencount_sel >= sels.Count)
                                spokencount_sel = 0;
                            currentcommand = sels[spokencount_sel];
                            AppManager.Instance.StartSpeaker("say yes to repeat instruction.");

                            AppManager.Instance.StartSpeaker(".. Say no to not repeat instruction again.");
                            File.WriteAllText(filepath, "");
                            spokencount_sel++;
                            return;
                        }
                        sels = _commandlist.Where(a => a[1] == "selection").ToList();
                        if (sels.Count > 0)
                        {
                            if (spokencount_sel >= sels.Count)
                                spokencount_sel = 0;
                            currentcommand = sels[spokencount_sel];
                            AppManager.Instance.StartSpeaker(".. Say yes to choose "+currentcommand[0] + ".");
                            
                            AppManager.Instance.StartSpeaker(".. Say no for other options.");
                        }
                        File.WriteAllText(filepath, "");
                        spokencount_sel++;
                        return;
                    }

                    else
                    {
                        //unselection here
                        var result = Enumerable.Range(0, _commandlist.Count).Where(i => _commandlist[i].Length == 3).ToList();
                        if (result.Count > 0)
                        {
                            _listeningmode = listeningmode.waitingvoter;
                            time_of_timerStarted = DateTime.Now;
                            currentcommand = _commandlist[result[0]];
                            AppManager.Instance.StartSpeaker(currentcommand[0] + " is selected. Say yes to unselect. ");
                            AppManager.Instance.StartSpeaker(".. Say no for other options.");
                            spokencount_sel = result[0];
                            spokencount_sel++;
                            return;
                        }

                        //buttons action here
                        _listeningmode = listeningmode.waitingvoter;
                        time_of_timerStarted = DateTime.Now;
                        var sels = _commandlist.Where(a => a[1] == "button").ToList();
                        if (sels.Count > 0)
                        {
                            if (spokencount_com >= sels.Count)
                                spokencount_com = 0;
                            currentcommand = sels[spokencount_com];
                            AppManager.Instance.StartSpeaker(".. Say yes to choose "+currentcommand[0] + ".");
                            
                            AppManager.Instance.StartSpeaker(".. Say no for other options.");
                        }
                        File.WriteAllText(filepath, "");
                        spokencount_com++;
                        return;
                    }
                }

                string word = File.ReadAllText(filepath);
                File.WriteAllText(filepath, "");

                if (word != "" && _listeningmode == listeningmode.waitingvoter)
                {
                    //string[] yess = { "yes", "yeah", "yep", "yis", "use","this", "his", "use of","used","as","you said","you some","use them" };
                    //string[] nos = { "no", "know", "known", "nah", "not", "note", "knot" };
                    string[] yess = File.ReadAllLines(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "yess.txt"));
                    string[] nos = File.ReadAllLines(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "nos.txt"));

                    //if (CheckIfVoiceCommand(word, out word))
                    File.AppendAllText(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "sampletext.txt"), word + Environment.NewLine);
                    if (nos.Contains(word.ToLower()))
                    {
                        if (currentcommand[0] == "Repeat")
                        {
                            AppManager.Instance.repeatInstruction = AppManager.repeatInstructionCommand.norepeat;
                            AppManager.Instance.StartSpeaker("Moving to candidate choices");
                            _listeningmode = listeningmode.notwaitingvoter;
                            return;
                        }

                            if (currentcommand[1] == "selection") //..continue with selected option .. voter says no to unselect 
                        {
                            if (spokencount_sel > 0)
                            {
                                if (currentcommand.Length == 3) //if already selected then unselect
                                {
                                    _commandlist[spokencount_sel - 1] = new string[] { currentcommand[0], currentcommand[1] };                                   
                                }
                            }
                        }
                        AppManager.Instance.StartSpeaker("Moving to other option");
                        _listeningmode = listeningmode.notwaitingvoter;
                        return;
                    }
                    if (yess.Contains(word.ToLower()))
                    {
                        timer.Stop();
                        word = currentcommand[0];                      
                        
                        if (word.ToLower() == "repeat")
                        {
                            AppManager.Instance.repeatInstruction = AppManager.repeatInstructionCommand.repeat;
                            AppManager.Instance.StopSpeaker();
                            AppManager.Instance.StartSpeaker("Repeating the instructions");
                            System.Threading.Thread.Sleep(1000);                          
                            foreach (var text in AppManager.Instance._speechWords.Where(text => text != null))
                            {
                                AppManager.Instance.StartSpeaker(text);
                            }
                            timer.Start();
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
                            if (currentcommand[1] == "selection")
                            {
                                if (spokencount_sel > 0)
                                {
                                    if (currentcommand.Length == 3) //if already selected then unselect
                                    {
                                        _commandlist[spokencount_sel - 1] = new string[] { currentcommand[0], currentcommand[1] };
                                        AppManager.Instance.StartSpeaker(currentcommand[0] + " is unselected now.");
                                    }
                                    else
                                    {
                                        _commandlist[spokencount_sel - 1] = new List<string>(currentcommand) { "selected" }.ToArray();//marking option is selected
                                        AppManager.Instance.StartSpeaker(word + " is accepted");
                                    }
                                }
                            }
                            else
                                AppManager.Instance.StartSpeaker(word + " is accepted");

                            //System.Threading.Thread.Sleep(1000);
                            //timer.Stop();
                            if (word.ToLower() == "return to voting")
                            {
                                if (AppManager.Instance.vController != null)
                                    AppManager.Instance.vController.HandleSpeech(word);
                            }
                            else
                            {
                                if (AppManager.Instance.controllerForHearingMode != null)
                                    AppManager.Instance.controllerForHearingMode.HandleSpeech(word);
                               // controller.HandleSpeech(word);
                               if(currentcommand[1] != "button")
                                    timer.Start();
                            }
                            //
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
                       
                    }
                }
            }
            catch (Exception)
            {
            }
        }

        private void Timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            timer.Stop();
            string yescmd = "Left click on mouse";//"Say yes";
            string nocmd = "Right click on mouse";//"Say no";
            try
            {
                string filepath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "speechout.txt");
                if (AppManager.Instance.speaker.Status.RunningState == SpeechRunState.SRSEIsSpeaking)
                {
                    System.Threading.Thread.Sleep(500);
                    timer.Start();
                    return;
                }
                
                if(reviewOrConfirm && reviewListspeak)
                {
                    ReviewSelectionsSpeakAndScroll();
                    reviewListspeak = false;
                }
                
                double spantime = DateTime.Now.Subtract(time_of_timerStarted).TotalSeconds;

                if ((_listeningmode == listeningmode.waitingvoter) && spantime >= 10)
                {
                    _listeningmode = listeningmode.notwaitingvoter;
                    if (currentcommand[0] != "Repeat")
                    {
                        if (AppManager.Instance.choicesLeft > 0)
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
                }
                //When returned from Review
                if (_listeningmode == listeningmode.notwaitingvoter && AppManager.Instance._contestSelections.Count > 0)
                {
                    var sels = _commandlist.Where(a => a[0] == AppManager.Instance._contestSelections[0]).ToList();
                    if (sels.Count > 0)
                    {
                        currentcommand = sels[0];
                        _listeningmode = listeningmode.waitingvoter;
                        time_of_timerStarted = DateTime.Now;
                        AppManager.Instance.StartSpeaker(currentcommand[0] + " is selected. " + yescmd + " again to unselect. ");
                        AppManager.Instance.StartSpeaker(".. " + nocmd + " for next option.");
                        System.Threading.Thread.Sleep(2000);
                    }
                }

                if (_listeningmode == listeningmode.notwaitingvoter && AppManager.Instance.choicesLeft >= 0)
                {
                    AppManager.Instance.mouseButton = AppManager.MouseButton.none;
                    var sels = _commandlist.Where(a => a[1] == "selection").ToList();
                    if (AppManager.Instance.choicesLeft > 0 && spokencount_sel < sels.Count)
                    {
                        //selection here

                        _listeningmode = listeningmode.waitingvoter;
                        time_of_timerStarted = DateTime.Now;
                        //sels = _commandlist.Where(a => a[1] == "instruction").ToList();
                        //if (sels.Count > 0 && (AppManager.Instance.repeatInstruction == AppManager.repeatInstructionCommand.none || AppManager.Instance.repeatInstruction == AppManager.repeatInstructionCommand.repeat))
                        //{
                        //    if (spokencount_sel >= sels.Count)
                        //        spokencount_sel = 0;
                        //    currentcommand = sels[spokencount_sel];
                        //    AppManager.Instance.StartSpeaker(yescmd + " to repeat instruction.");

                        //    AppManager.Instance.StartSpeaker(nocmd + " to not repeat instruction again.");
                        //    System.Threading.Thread.Sleep(2000);
                        //    spokencount_sel++;
                        //    timer.Start();
                        //    return;
                        //}
                        sels = _commandlist.Where(a => a[1] == "selection").ToList();
                        if (sels.Count > 0)
                        {
                            AppManager.Instance.repeatInstruction = AppManager.repeatInstructionCommand.norepeat;
                            if (spokencount_sel >= sels.Count)
                            {
                                spokencount_sel = 0;
                                repeatcandidatenow = true;
                            }
                            currentcommand = sels[spokencount_sel];
                            AppManager.Instance.StartSpeaker(".. " + yescmd + " to choose " + currentcommand[0] + ".");

                            AppManager.Instance.StartSpeaker(".. " + nocmd + " for next options.");
                            System.Threading.Thread.Sleep(1000);
                        }
                        spokencount_sel++;
                        timer.Start();
                        return;
                    }
                    else if (AppManager.Instance.choicesLeft > 0 && spokencount_sel >= sels.Count && repeatcandidatenow)
                    {
                        _listeningmode = listeningmode.waitingvoter;
                        currentcommand =new string[] { "Repeat" };
                        time_of_timerStarted = DateTime.Now;
                        AppManager.Instance.StartSpeaker(".. " + yescmd + " to repeat choices.");

                        AppManager.Instance.StartSpeaker(".. " + nocmd + " for next options.");
                        System.Threading.Thread.Sleep(1000);
                    }
                    else
                    {
                        //unselection here
                        var result = Enumerable.Range(0, _commandlist.Count).Where(i => _commandlist[i].Length == 3).ToList();
                        if (result.Count > 0)
                        {
                            _listeningmode = listeningmode.waitingvoter;
                            time_of_timerStarted = DateTime.Now;
                            currentcommand = _commandlist[result[0]];
                            AppManager.Instance.StartSpeaker(currentcommand[0] + " is selected now. " + yescmd + " again to unselect. ");
                            AppManager.Instance.StartSpeaker(".. " + nocmd + " for next option.");
                            System.Threading.Thread.Sleep(2000);
                            //spokencount_sel = result[0];
                            // spokencount_sel++;
                            //timer.Start();
                            //return;
                        }
                        else
                        {
                            //buttons action here
                            _listeningmode = listeningmode.waitingvoter;
                            time_of_timerStarted = DateTime.Now;
                            
                            sels = _commandlist.Where(a => a[1] == "button").ToList();
                            if (spokencount_com >= sels.Count)
                                spokencount_com = 0;
                            if (sels.Count > 0 && spokencount_com < sels.Count)
                            {

                                currentcommand = sels[spokencount_com];
                                spokencount_com++;
                                //if (AppManager.Instance.choicesLeft > 0 && currentcommand[0].ToLower() == "skip")
                                //{                                    
                                //    currentcommand = sels[spokencount_com];
                                    
                                //}
                                AppManager.Instance.StartSpeaker(".. " + yescmd + " to choose " + currentcommand[0] + ".");

                                AppManager.Instance.StartSpeaker(".. " + nocmd + " for next option.");
                                System.Threading.Thread.Sleep(2000);
                            }
                            //spokencount_com++;
                            if (AppManager.Instance.choicesLeft > 0 || _commandlist.Where(a => a[1] == "selection").ToList().Count == 0)
                            {
                                if (spokencount_com >= sels.Count)
                                {
                                    spokencount_com = 0;
                                    if (spokencount_sel >= _commandlist.Where(a => a[1] == "selection").ToList().Count)
                                    {
                                        spokencount_sel = 0;
                                        repeatcandidatenow = true;
                                    }
                                }
                            }                         

                            timer.Start();
                            return;
                        }
                    }
                   
                }

                

                    string word = "";

                if (AppManager.Instance.mouseButton != AppManager.MouseButton.none && _listeningmode == listeningmode.waitingvoter)
                {

                    if (AppManager.Instance.mouseButton == AppManager.MouseButton.right)
                    {
                        timer.Stop();
                        AppManager.Instance.mouseButton = AppManager.MouseButton.none;
                        if (currentcommand[0] == "Repeat")
                        {
                            AppManager.Instance.repeatInstruction = AppManager.repeatInstructionCommand.norepeat;
                            AppManager.Instance.StartSpeaker("Moving to next option");
                            repeatcandidatenow = false;
                            System.Threading.Thread.Sleep(1000);
                            _listeningmode = listeningmode.notwaitingvoter;
                            timer.Start();
                            return;
                        }

                        if (currentcommand[1] == "selection") //..continue with selected option .. voter says no to unselect 
                        {
                            if (spokencount_sel > 0)
                            {
                                if (currentcommand.Length == 3) //if already selected then unselect
                                {
                                    var curix = _commandlist.IndexOf(currentcommand);
                                    _commandlist[curix] = new string[] { currentcommand[0], currentcommand[1] };
                                }
                            }
                        }
                        if (word == " to complete voting without hear review again in next screen")
                        {
                            word = "Accept and Print";
                            if (AppManager.Instance.controllerForHearingMode != null)
                                AppManager.Instance.controllerForHearingMode.HandleSpeech(word);
                            return;
                        }
                        AppManager.Instance.StartSpeaker("Moving to next option");
                        System.Threading.Thread.Sleep(1000);
                        _listeningmode = listeningmode.notwaitingvoter;
                        timer.Start();
                        return;
                    }
                    if (AppManager.Instance.mouseButton == AppManager.MouseButton.left)
                    {
                        timer.Stop();
                        AppManager.Instance.mouseButton = AppManager.MouseButton.none;
                        word = currentcommand[0];


                        if (word.ToLower() == "repeat" || word == "Repeat review of candidates")
                        {
                            //AppManager.Instance.repeatInstruction = AppManager.repeatInstructionCommand.repeat;
                            AppManager.Instance.StartSpeaker("Repeating the choices.");
                            //_listeningmode = listeningmode.notwaitingvoter;
                            //spokencount_sel = 0;
                            //repeatcandidatenow = true;
                            if (word == "Repeat review of candidates")
                            {
                                System.Threading.Thread.Sleep(1000);
                                AppManager.Instance.vController.HandleSpeech("ScrollToTop");
                                ReviewSelectionsSpeakAndScroll();
                            }
                            else
                                spokencount_sel = 0;
                            timer.Start();
                            //return;
                        }
                        else if (word == "to complete voting without hear review")
                        {
                            timer.Start();
                            return;
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
                            if (currentcommand[1] == "selection")
                            {
                                if (spokencount_sel > 0)
                                {
                                    var curix = _commandlist.IndexOf(currentcommand);
                                    if (currentcommand.Length == 3) //if already selected then unselect
                                    {
                                        _commandlist[curix] = new string[] { currentcommand[0], currentcommand[1] };
                                        AppManager.Instance.StartSpeaker(currentcommand[0] + " is unselected now.");
                                    }
                                    else
                                    {
                                        _commandlist[curix] = new List<string>(currentcommand) { "selected" }.ToArray();//marking option is selected
                                        AppManager.Instance.StartSpeaker(word + " is accepted");
                                        _listeningmode = listeningmode.notwaitingvoter;
                                        //timer.Start();
                                        //return;
                                    }
                                }
                            }
                            else
                            {
                                if (word == "print review in normal text size")
                                {
                                    AppManager.Instance.reviewResultFontSize = 18;
                                    AppManager.Instance.reviewHeaderFontSize = 22;
                                    word = "Print Review";
                                }
                                else if (word == "print review in zoom text size")
                                {
                                    AppManager.Instance.reviewResultFontSize = 28;
                                    AppManager.Instance.reviewHeaderFontSize = 30;
                                    word = "Print Review";
                                }
                                else if (word == "accept and print ballot, you will have a second opportunity to review on the next page")
                                    word = "Accept and Print";
                                AppManager.Instance.StartSpeaker(word + " is accepted");
                                if (word == "Accept and Print")
                                {
                                    
                                    currentcommand = new string[] { " to complete voting without hear review again in next screen", "button" };
                                    _commandlist.Add(currentcommand);
                                    spokencount_com = _commandlist.Count;
                                    word = " to complete voting without hear review again in next screen";
                                    _listeningmode = listeningmode.waitingvoter;
                                    System.Threading.Thread.Sleep(4000);
                                    AppManager.Instance.StartSpeaker(yescmd + " to complete voting, without hear review again, in next screen");
                                    //AppManager.Instance.StartSpeaker(nocmd + " to complete voting, with hear review again, in next screen");
                                    AppManager.Instance.StartSpeaker(nocmd + " to continue usual.");
                                    System.Threading.Thread.Sleep(2000);
                                    timer.Start();
                                    return;
                                }
                                if(word == " to complete voting without hear review again in next screen")
                                {
                                    word = "Accept and Print";
                                    AppManager.Instance.reviewResultFontSize = 18;
                                    AppManager.Instance.reviewHeaderFontSize = 22;
                                    AppManager.Instance.reviewOnConfirm = false;
                                }
                            }

                            System.Threading.Thread.Sleep(1000);
                            //timer.Stop();
                            if (word.ToLower() == "return to voting")
                            {
                                if (AppManager.Instance.vController != null)
                                    AppManager.Instance.vController.HandleSpeech("Return to voting");
                            }
                            else
                            {
                                if (AppManager.Instance.controllerForHearingMode != null)
                                {
                                    AppManager.Instance.controllerForHearingMode.HandleSpeech(word);
                                    if (AppManager.Instance._contestSelections.Count > 0)
                                        AppManager.Instance._contestSelections.RemoveAt(0);
                                }
                                // controller.HandleSpeech(word);
                                if (word == "Print Review" || word == "Zoom" || word == "Normal")
                                {
                                    timer.Start();
                                }
                                if (currentcommand[1] != "button")
                                    timer.Start();
                            }
                            //
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

                    }
                }
                else
                    timer.Start();
            }
            catch (Exception ex)
            {
            }
        }

        private void ReviewSelectionsSpeakAndScroll()
        {
            foreach (var texts in AppManager.Instance._reviewSelections)
            {
                if (texts[1] == "scroll")
                {
                    AppManager.Instance.vController.HandleSpeech("Scroll down");
                    AppManager.Instance.vController.HandleSpeech("Scroll down");
                }
                AppManager.Instance.StartSpeaker(texts[0]);
                while (AppManager.Instance.speaker.Status.RunningState == SpeechRunState.SRSEIsSpeaking)
                {
                    System.Threading.Thread.Sleep(1500);
                }
            }
        }

    }
}
