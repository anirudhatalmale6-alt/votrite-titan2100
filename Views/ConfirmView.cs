// Product:	VotRite
// Module:  ConfirmView.cs
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
using System.Linq;
using System.Windows.Forms;
using System.Drawing.Printing;

using VotRite.UI;
using VotRite.Models;
using VotRite.Controllers;
using VotRite.Definition;

using VotRite.Util;
using VotRiteBallotDataManager.AppCode;
using VotRite.DBClasses;
using System.Data;

namespace VotRite.Views
{
    class ConfirmView : VrScreen
    {
        private const int StartTick = 5;
        //private ConfirmModel model;
        //private ConfirmController controller;
        private VrLabel lblBallotName;
        private VrLabel lblBallotTitle;
        private VrLabel lblBallotAddress;
        private VrLabel lblBallotDate;
        private VrLabel lblGroupName;
        private VrLabel lblConfirmTitle;
        private VrLabel lblConfirmTip;

        private VrButton btnSettingTools;
        private VrButton btnCallHelp;
        
        private VrButton btnBack;
        private VrButton btnPrintReview;
        private VrButton btnCastVote;
        private VrContainer container;
        private VrLabel _topLine;
        private VrLabel _bottomLine;
        private Timer _timer;
        protected internal int TimerCount { get; set; }
        private List<string> _textsToSpeak;

        private static List<string> mListReviewDetail = new List<string>();

        public Timer GetTimer()
        {
            return _timer;
        }

        public VrContainer Container { get { return container; } }

        public ConfirmView(ConfirmModel m, ConfirmController c)
            : base(m, c)
        {
            model = m;
            controller = c;
           // vModel.Definition.Background = "graphics\\initialBG_f1.png";
            lblBallotName = (VrLabel)((ConfirmModel)model).FindScreenObject("lbl_blt_name");
            lblBallotTitle = (VrLabel)((ConfirmModel)model).FindScreenObject("lbl_blt_title");
            lblBallotAddress = (VrLabel)((ConfirmModel)model).FindScreenObject("lbl_blt_address");
            lblBallotDate = (VrLabel)((ConfirmModel)model).FindScreenObject("lbl_blt_date");
            lblGroupName = (VrLabel)((ConfirmModel)model).FindScreenObject("lbl_group_name");
            lblConfirmTitle = (VrLabel)((ConfirmModel)model).FindScreenObject("lbl_confirm_title");
            lblConfirmTip = (VrLabel)((ConfirmModel)model).FindScreenObject("lbl_confirm_tip");
            AppManager.Instance.ChangeTextSize(lblConfirmTip);
            btnSettingTools = (VrButton)((ConfirmModel)model).FindScreenObject("btn_setting_tools");
            btnCallHelp = (VrButton)((ConfirmModel)model).FindScreenObject("btn_callhelp");

            btnBack = (VrButton)((ConfirmModel)model).FindScreenObject("btn_back");
            btnPrintReview = (VrButton)((ConfirmModel)model).FindScreenObject("btn_print_review");
            btnCastVote = (VrButton)((ConfirmModel)model).FindScreenObject("btn_cast_vote");
            container = (VrContainer)((ConfirmModel)model).FindScreenObject("ctr_confirm");
            _topLine = (VrLabel)((ConfirmModel)model).FindScreenObject("lbl_top_line");
            _bottomLine = (VrLabel)((ConfirmModel)model).FindScreenObject("lbl_bottom_line");

            Initialize();

            AppManager.Instance.checkDoubleSpaceSetting(m.Definition.ScreenObjects, container);

            if (lblConfirmTip.TextSize > 18)
            {
                lblConfirmTip.Height += (lblConfirmTip.TextSize - 18)*30;
                lblConfirmTip.Top -= 150;
                lblConfirmTip.Width -= 20;


                btnBack.TextSize = lblConfirmTip.TextSize;
                btnCastVote.TextSize = lblConfirmTip.TextSize;
                
            }
            if (AppManager.Instance.backgroundTheme != AppManager.colorTheme.White)
            {
                m.Definition.Background = AppManager.GetPathToCommonFile("graphics" + Global.Instance.SLASH + "initialBG_f1_" + AppManager.Instance.backgroundTheme + ".png");
            }

            foreach (var text in _textsToSpeak.Where(text => text != null))
            {
                AppManager.Instance.StartSpeaker(text);
            }
            AppManager.Instance.initiateHearingMode();
            AppManager.Instance.controllerForHearingMode = controller;
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _timer.Dispose();
                _textsToSpeak.Clear();
                if (_bottomLine != null) _bottomLine.Dispose();
                if (_topLine != null) _topLine.Dispose();

                if (lblBallotName != null) lblBallotName.Dispose();
                if (lblBallotTitle != null) lblBallotTitle.Dispose();
                if (lblBallotAddress != null) lblBallotAddress.Dispose();
                if (lblBallotDate != null) lblBallotDate.Dispose();
                if (lblGroupName != null) lblGroupName.Dispose();

                if (btnSettingTools != null) btnSettingTools.Dispose();
                if (btnCallHelp != null) btnCallHelp.Dispose();
                
                if (btnBack != null) btnBack.Dispose();
                if (btnPrintReview != null) btnPrintReview.Dispose();
                if (btnCastVote != null) btnCastVote.Dispose();
                if (lblConfirmTitle != null) lblConfirmTitle.Dispose();
                if (lblConfirmTip != null) lblConfirmTip.Dispose();
                if (lblConfirmTip != null) lblConfirmTip.Dispose();

            }
            base.Dispose(disposing);
        }

        private void Initialize_new()
        {
            if (lblBallotName != null)
                lblBallotName.Text = (string)((ConfirmModel)model).Vars["ballot_name"];
            if (lblBallotTitle != null)
                lblBallotTitle.Text = (string)((ConfirmModel)model).Vars["ballot_title"];
            if ((string)((ConfirmModel)model).Vars["ballot_address"] != "")
                lblBallotAddress.Text = (string)((ConfirmModel)model).Vars["ballot_address"];
            if (lblBallotDate != null)
                lblBallotDate.Text = (string)((ConfirmModel)model).Vars["ballot_date"];
            if (lblGroupName != null)
                lblGroupName.Text = (string)((ConfirmModel)model).Vars["group_name"];

            if (btnSettingTools != null)
            {
                btnSettingTools.Text = (string)((ConfirmModel)model).Vars["btn_setting_tools"];
            }
            if (btnCallHelp != null)
            {
                btnCallHelp.Text = (string)((ConfirmModel)model).Vars["btn_callhelp"];
            }

            if (btnBack != null)
            {
                btnBack.Text = (string)((ConfirmModel)model).Vars["back"];
                btnBack.Visible = (bool)((ConfirmModel)model).Vars["back_visible"];
            }

            if(!(bool)((ConfirmModel)model).Vars["back_visible"])
            {
                // back button is hidden, display message
                MessageBox.Show("Maximum number of attempts have been exceeded. You must proceed with your choices.", "Maximum number of attempts have been exceeded");
            }
                
            if (btnPrintReview != null)
            {
                btnPrintReview.Text = (string)((ConfirmModel)model).Vars["print_review"];
                if (!AppManager.ShowPrintReview)
                {
                    btnPrintReview.Visible = false;
                }
            }
            if (btnCastVote != null)
                btnCastVote.Text = (string)((ConfirmModel)model).Vars["cast_vote"];
            if (lblConfirmTitle != null)
                lblConfirmTitle.Text = (string)((ConfirmModel)model).Vars["confirm_title"];
            if (lblConfirmTip != null)
                lblConfirmTip.Text = (string)((ConfirmModel)model).Vars["confirm_tip"];

            int contGroup = 0;
            int contCounter = 0;
            int topHist = container.Top + 5;
            var showGroupForMassPropositions = true;

            //Jim Code -- Begin
            string[] sExtraInfo = new string[] { 
                   (string)((ConfirmModel)model).Vars["board"], 
                   (string)((ConfirmModel)model).Vars["address"], 
                   (string)((ConfirmModel)model).Vars["ballot"], 
                   (string)((ConfirmModel)model).Vars["date"], 
                   (string)((ConfirmModel)model).Vars["voter"], 
                   (string)((ConfirmModel)model).Vars["machine"] + ": " + AppManager.Configuration["System"]["Machine"]
                  
            };

            for (int i = 0; i < sExtraInfo.Length; i++)
            {
                VrLabel lbl = new VrLabel();
                lbl.Text = sExtraInfo[i];
                lbl.TextAlign = "left-middle";
                lbl.TextSize = 18;
                lbl.Height = 30;
                lbl.Width = container.Width;
                lbl.Left = container.Left;
                lbl.Top = topHist;

                topHist += lbl.Height;

                if (topHist > container.Top + container.Height)
                    lbl.Visible = false;
                if (!container.Controls.Contains(lbl))
                    container.Controls.Add(lbl);

                topHist += 10;

            }
            topHist += 20;

            //Jim Code -- End

            List<ContestDefinition> lstContest = ((ConfirmModel)model).Ballot.ContestsList;
            DBClasses.Slate slateSelected = null;
            
            if (((ConfirmModel)model).Ballot.slatesDefinition.Slates.Count > 0)
            {
                foreach (ContestDefinition contest in lstContest)
                {
                    if (contest.Selected > 0)
                    {
                        if (contest.Id == ((ConfirmModel)model).Ballot.slatesDefinition.Data.Id)
                        {
                            foreach (var slate in ((ConfirmModel)model).Ballot.slatesDefinition.Slates)
                            {
                                if (slate.Id == ((ConfirmModel)model).Ballot.slatesDefinition.Data.SlateId)
                                {
                                    slateSelected = slate;
                                    break;
                                }
                            }
                        }
                    }
                }

                if (slateSelected != null)
                {
                    VrLabel lbl = new VrLabel
                    {
                        Text = "Slates Voting",
                        TextAlign = "left-middle",
                        TextSize = 22,
                        Height = 30,
                        Width = container.Width,
                        Left = container.Left,
                        Top = topHist,
                        Speakable = true
                    };

                    topHist += lbl.Height;

                    if (topHist > container.Top + container.Height)
                        lbl.Visible = false;
                    if (!container.Controls.Contains(lbl))
                        container.Controls.Add(lbl);

                    lbl = new VrLabel();
                    lbl.Text = slateSelected.Name;
                    lbl.TextAlign = "left-middle";
                    lbl.TextSize = 22;
                    lbl.Height = 30;
                    lbl.Width = container.Width;
                    lbl.Left = container.Left;
                    lbl.Top = topHist;
                    lbl.Speakable = true;

                    topHist += lbl.Height;

                    if (topHist > container.Top + container.Height)
                        lbl.Visible = false;
                    if (!container.Controls.Contains(lbl))
                        container.Controls.Add(lbl);

                    topHist += 5;
                }
            }

            foreach (var contest in ((ConfirmModel)model).Ballot.ContestsList)
            {
                if (contest.Type == VotRiteBallotDataManager.AppCode.ContestTypes.R && slateSelected != null)
                {
                    contCounter++; continue;
                }

                VrLabel lbl;

                if (!(contGroup == contest.Group || (contest.Propositions != null && !showGroupForMassPropositions)))
                {
                    lbl = new VrLabel();
                    lbl.Text = (string)((ConfirmModel)model).Vars["$b" + ((ConfirmModel)model).Ballot.Id + "_g" + contest.Group + "_name"];
                    lbl.TextAlign = "left-middle";
                    lbl.TextSize = 22;
                    lbl.Height = 30;
                    lbl.Width = container.Width;
                    lbl.Left = container.Left;
                    lbl.Top = topHist;
                    lbl.Speakable = true;

                    topHist += lbl.Height;

                    if (topHist > container.Top + container.Height)
                        lbl.Visible = false;
                    if (!container.Controls.Contains(lbl))
                        container.Controls.Add(lbl);
                }

                topHist += 15;

                if (contest.Propositions == null)
                {
                    showGroupForMassPropositions = true;
                    lbl = new VrLabel();
                    lbl.Text = (string)((ConfirmModel)model).Vars["$b" + ((ConfirmModel)model).Ballot.Id + "_c" + contest.Id + "_name"];
                    lbl.TextAlign = "left-middle";
                    lbl.TextSize = 18;
                    lbl.Height = 30;
                    lbl.Width = container.Width;
                    lbl.Left = container.Left;
                    lbl.Top = topHist;
                    lbl.Speakable = true;

                    topHist += lbl.Height;

                    if (topHist > container.Top + container.Height)
                        lbl.Visible = false;
                    if (!container.Controls.Contains(lbl))
                        container.Controls.Add(lbl);

                    topHist += 5;

                    if (contest.Selected == 0)
                    {
                        lbl = new VrLabel();
                        lbl.Height = 30;
                        lbl.TextSize = 16;
                        lbl.Text = (string)((ConfirmModel)model).Vars["no_selection"];
                        lbl.TextAlign = "left-middle";
                        lbl.Width = container.Width;
                        lbl.Left = container.Left;
                        lbl.Top = topHist;
                        lbl.Speakable = true;

                        topHist += lbl.Height;

                        if (topHist > container.Top + container.Height)
                            lbl.Visible = false;
                        if (!container.Controls.Contains(lbl))
                            container.Controls.Add(lbl);
                    }
                    else
                    {
                        int counter = 0;

                        foreach (DataDefinition data in contest.Data)
                        {
                            if (counter == 20) break;

                            if (((ConfirmModel)model).Ballot.ElectionType == VotRiteBallotDataManager.AppCode.ElectionTypes.ranking_choice)
                            {
                                if (data.WriteIn && !data.IsWritten)
                                {
                                    continue;
                                }

                                lbl = new VrLabel();
                                lbl.Height = 30;
                                lbl.TextSize = 16;
                                lbl.Text =
                                    (string)
                                    ((ConfirmModel)model).Vars[
                                        "$b" + ((ConfirmModel)model).Ballot.Id + "_c" + contest.Id + "_g" + contest.Group + "_d" +
                                        data.Id] + "   Preference " + data.Preference.ToString();
                                lbl.TextAlign = "left-middle";
                                lbl.Width = container.Width;
                                lbl.Left = container.Left;
                                lbl.Top = topHist;
                                lbl.Speakable = true;

                                topHist += lbl.Height;

                                if (topHist > container.Top + container.Height)
                                    lbl.Visible = false;
                                if (!container.Controls.Contains(lbl))
                                    container.Controls.Add(lbl);
                                counter++;
                                topHist += 5;
                            }
                            else
                            {
                                if (data.State == VrSelection.SelectionState.SELECTED)
                                {
                                    lbl = new VrLabel();
                                    lbl.Height = 30;
                                    lbl.TextSize = 16;
                                    lbl.Text =
                                        (string)
                                        ((ConfirmModel)model).Vars[
                                            "$b" + ((ConfirmModel)model).Ballot.Id + "_c" + contest.Id + "_g" + contest.Group + "_d" +
                                            data.Id];
                                    lbl.TextAlign = "left-middle";
                                    lbl.Width = container.Width;
                                    lbl.Left = container.Left;
                                    lbl.Top = topHist;
                                    lbl.Speakable = true;

                                    topHist += lbl.Height;

                                    if (topHist > container.Top + container.Height)
                                        lbl.Visible = false;
                                    if (!container.Controls.Contains(lbl))
                                        container.Controls.Add(lbl);
                                    counter++;
                                    topHist += 5;
                                }
                            }
                        }

                    }
                }
                else
                {
                    var idx = 0;
                    showGroupForMassPropositions = false;
                    foreach (var proposition in contest.Propositions)
                    {
                        lbl = new VrLabel();
                        lbl.Text = proposition.Title;
                        lbl.TextAlign = "left-middle";
                        lbl.TextSize = 18;
                        lbl.Height = 30;
                        lbl.Width = container.Width;
                        lbl.Left = container.Left;
                        lbl.Top = topHist;
                        lbl.Speakable = true;

                        topHist += lbl.Height;

                        if (topHist > container.Top + container.Height)
                            lbl.Visible = false;
                        if (!container.Controls.Contains(lbl))
                            container.Controls.Add(lbl);

                        topHist += 5;

                        if (!contest.GroupSelection.ContainsKey(idx) || contest.GroupSelection[idx] == 0)
                        {
                            lbl = new VrLabel();
                            lbl.Height = 30;
                            lbl.TextSize = 16;
                            lbl.Text = (string)((ConfirmModel)model).Vars["no_selection"];
                            lbl.TextAlign = "left-middle";
                            lbl.Width = container.Width;
                            lbl.Left = container.Left;
                            lbl.Top = topHist;
                            lbl.Speakable = true;

                            topHist += lbl.Height;

                            if (topHist > container.Top + container.Height)
                                lbl.Visible = false;
                            if (!container.Controls.Contains(lbl))
                                container.Controls.Add(lbl);
                        }
                        else
                        {
                            var counter = 0;
                            foreach (var data in contest.Data.TakeWhile(data => counter != 10).Where(data => data.State == VrSelection.SelectionState.SELECTED && contest.Id + idx * 7 < data.Id && data.Id < contest.Id + (idx + 1) * 7))
                            {
                                lbl = new VrLabel
                                          {
                                              Height = 30,
                                              TextSize = 16,
                                              Text = (string)
                                                     ((ConfirmModel)model).Vars[
                                                         "$b" + ((ConfirmModel)model).Ballot.Id + "_c" + contest.Id + "_g" +
                                                         contest.Group + "_d" +
                                                         data.Id],
                                              TextAlign = "left-middle",
                                              Width = container.Width,
                                              Left = container.Left,
                                              Top = topHist,
                                              Speakable = true
                                          };
                                //data.Text + "   " + data.Group;

                                topHist += lbl.Height;

                                if (topHist > container.Top + container.Height)
                                    lbl.Visible = false;
                                if (!container.Controls.Contains(lbl))
                                    container.Controls.Add(lbl);
                                counter++;
                                topHist += 5;
                            }
                        }
                        idx++;
                    }
                }

                contGroup = contest.Group;
                contCounter++;

                if (contCounter != ((ConfirmModel)model).Ballot.ContestsList.Count && contest.Propositions == null)
                    topHist += 20;
            }

            _textsToSpeak = GetAllTexts(false, false, true);
            if (container.CreateScroll((ConfirmModel)model, topHist - container.Top))
            {
                var upScroll = ((ConfirmModel)model).Definition.ScreenObjects[((ConfirmModel)model).Definition.ScreenObjects.Count - 2];
                upScroll.Top -= 25;

                if (upScroll.ObjectState == ScreenObject.ScreenObjectState.ACTIVE)
                    upScroll.Text = (string)((ConfirmModel)model).Vars["more"];
                upScroll.Tag = "Scroll up";

                var downScroll = ((ConfirmModel)model).Definition.ScreenObjects[((ConfirmModel)model).Definition.ScreenObjects.Count - 1];
                if (downScroll.ObjectState == ScreenObject.ScreenObjectState.ACTIVE)
                    downScroll.Text = (string)((ConfirmModel)model).Vars["more"];
                downScroll.Tag = "Scroll down";

                container.Top += upScroll.Height + 20;
                container.Height -= upScroll.Height + downScroll.Height;
                if (_topLine != null)
                    _topLine.Top = Container.Top - 2;
                if (_bottomLine != null)
                    _bottomLine.Top = Container.Top + container.Height + 1;

                foreach (var screenObject in container.Controls)
                {
                    screenObject.Top += upScroll.Height + 20;
                    if (screenObject.Top + screenObject.Height > container.Top + container.Height)
                        screenObject.Visible = false;
                }
                _timer = new Timer { Interval = VrContainer.GetScrollTimerIn(ScrollTimerTypes.ReviewViewScrollTimerIn) };
                _timer.Stop();
                _timer.Tick += TimerTick;
            }

            CreateListOfCommands();
            AddSpeechToTextEngine();

            //Reading All Data to Form variable
            int iCount = 0;
            mListReviewDetail.Clear();
            foreach (ScreenObject ctl in container.Controls)
            {
                iCount++;
                mListReviewDetail.Add(ctl.Text);
                if (iCount == 6)
                {
                    mListReviewDetail.Add(" ");
                }
            }

            AppManager.Instance.initiateHearingMode();
            AppManager.Instance.controllerForHearingMode = controller;

        }

        private void Initialize()
        {
            VrLabel vrLabel;
            Func<DataDefinition, bool> func;
            bool flag;
            Func<DataDefinition, bool> func1 = null;
            Func<DataDefinition, bool> func2 = null;
            ConfirmModel confirmModel = (ConfirmModel)this.model;

            

            if (this.lblBallotName != null)
            {
                this.lblBallotName.Text = (string)confirmModel.Vars["ballot_name"];
            }
            if (this.lblBallotTitle != null)
            {
                this.lblBallotTitle.Text = (string)confirmModel.Vars["ballot_title"];
            }
            if ((string)confirmModel.Vars["ballot_address"] != "")
            {
                this.lblBallotAddress.Text = (string)confirmModel.Vars["ballot_address"];
            }
            if (this.lblBallotDate != null)
            {
                this.lblBallotDate.Text = (string)confirmModel.Vars["ballot_date"];
            }
            if (this.lblGroupName != null)
            {
                this.lblGroupName.Text = (string)confirmModel.Vars["group_name"];
            }
            if (this.btnSettingTools != null)
            {
                this.btnSettingTools.Text = (string)confirmModel.Vars["btn_setting_tools"];
            }
            if (this.btnCallHelp != null)
            {
                this.btnCallHelp.Text = (string)confirmModel.Vars["btn_callhelp"];
            }
            if (this.btnBack != null)
            {
                this.btnBack.Text = (string)confirmModel.Vars["back"];
                this.btnBack.Visible = (bool)confirmModel.Vars["back_visible"];
                this.btnBack.BgImage = "graphics\\button_red.jpg";
            }
            if (!(bool)confirmModel.Vars["back_visible"])
            {
                MessageBox.Show("Maximum number of attempts have been exceeded. You must proceed with your choices.", "Maximum number of attempts have been exceeded");
            }
            if (this.btnPrintReview != null)
            {
                this.btnPrintReview.Text = (string)confirmModel.Vars["print_review"];
                //if (!AppManager.ShowPrintReview)
                //{
                //    this.btnPrintReview.Visible = false;
                //}
            }
            if (this.btnCastVote != null)
            {
                this.btnCastVote.Text = (string)confirmModel.Vars["cast_vote"];
            }
            if (this.lblConfirmTitle != null)
            {
                this.lblConfirmTitle.Text = (string)confirmModel.Vars["confirm_title"];
            }
            if (this.lblConfirmTip != null)
            {
                this.lblConfirmTip.Text = (string)confirmModel.Vars["confirm_tip"];
                if(this.lblConfirmTip.Text != "")
                {
                    this.lblConfirmTip.Text = this.lblConfirmTip.Text.Replace("To confirm your vote read", "To confirm your vote hear");
                }
            }
            int group = 0;
            int num = 0;
            int top = this.container.Top + 5;
            bool flag1 = true;
            string[] item = new string[] { (string)confirmModel.Vars["board"], (string)confirmModel.Vars["address"], (string)confirmModel.Vars["ballot"], (string)confirmModel.Vars["date"], (string)confirmModel.Vars["voter"], string.Concat((string)confirmModel.Vars["machine"], ": ", AppManager.Configuration["System"]["Machine"]) };
            for (int i = 0; i < (int)item.Length; i++)
            {
                VrLabel vrLabel1 = new VrLabel()
                {
                    Text = item[i],
                    TextAlign = "left-middle",
                    TextSize = 18,
                    Height = 30,
                    Width = this.container.Width,
                    Left = this.container.Left,
                    Top = top
                };
                top += vrLabel1.Height;
                if (top > this.container.Top + this.container.Height)
                {
                    vrLabel1.Visible = false;
                }
                if (!this.container.Controls.Contains(vrLabel1))
                {
                    this.container.Controls.Add(vrLabel1);
                }
                top += 10;
            }
            top += 20;
            List<ContestDefinition> fitOrderContestList = this.GetFitOrderContestList();
            Slate slate = null;
            if (confirmModel.Ballot.slatesDefinition.Slates.Count > 0)
            {
                foreach (ContestDefinition contestDefinition in fitOrderContestList)
                {
                    if (contestDefinition.Selected > 0)
                    {
                        if (contestDefinition.Id == confirmModel.Ballot.slatesDefinition.Data.Id)
                        {
                            foreach (Slate slate1 in confirmModel.Ballot.slatesDefinition.Slates)
                            {
                                if (slate1.Id == confirmModel.Ballot.slatesDefinition.Data.SlateId)
                                {
                                    slate = slate1;
                                    break;
                                }
                            }
                        }
                    }
                }
                if (slate != null)
                {
                    VrLabel vrLabel2 = new VrLabel()
                    {
                        Text = "Slates Voting",
                        TextAlign = "left-middle",
                        TextSize = 22,
                        Height = 30,
                        Width = this.container.Width,
                        Left = this.container.Left,
                        Top = top,
                        Speakable = true
                    };
                    top += vrLabel2.Height;
                    if (top > this.container.Top + this.container.Height)
                    {
                        vrLabel2.Visible = false;
                    }
                    if (!this.container.Controls.Contains(vrLabel2))
                    {
                        this.container.Controls.Add(vrLabel2);
                    }
                    vrLabel2 = new VrLabel()
                    {
                        Text = slate.Name,
                        TextAlign = "left-middle",
                        TextSize = 22,
                        Height = 30,
                        Width = this.container.Width,
                        Left = this.container.Left,
                        Top = top,
                        Speakable = true
                    };
                    top += vrLabel2.Height;
                    if (top > this.container.Top + this.container.Height)
                    {
                        vrLabel2.Visible = false;
                    }
                    if (!this.container.Controls.Contains(vrLabel2))
                    {
                        this.container.Controls.Add(vrLabel2);
                    }
                    top += 5;
                }
            }
            foreach (ContestDefinition contestDefinition1 in fitOrderContestList)
            {
                if ((contestDefinition1.Type != ContestTypes.R ? true : slate == null))
                {
                    if (group == contestDefinition1.Group)
                    {
                        flag = false;
                    }
                    else
                    {
                        flag = (contestDefinition1.Propositions == null ? true : flag1);
                    }
                    if (flag)
                    {
                        vrLabel = new VrLabel()
                        {
                            Text = (string)confirmModel.Vars[string.Concat(new object[] { "$b", confirmModel.Ballot.Id, "_g", contestDefinition1.Group, "_name" })],
                            TextAlign = "left-middle",
                            TextSize = 22,
                            Height = 30,
                            Width = this.container.Width,
                            Left = this.container.Left,
                            Top = top,
                            Speakable = true
                        };
                        top += vrLabel.Height;
                        if (top > this.container.Top + this.container.Height)
                        {
                            vrLabel.Visible = false;
                        }
                        if (!this.container.Controls.Contains(vrLabel))
                        {
                            this.container.Controls.Add(vrLabel);
                        }
                    }
                    top += 15;
                    if (contestDefinition1.Propositions != null)
                    {
                        int num1 = 0;
                        flag1 = false;
                        foreach (Proposition proposition in contestDefinition1.Propositions)
                        {
                            vrLabel = new VrLabel()
                            {
                                Text = proposition.Title,
                                TextAlign = "left-middle",
                                TextSize = 18,
                                Height = 30,
                                Width = this.container.Width,
                                Left = this.container.Left,
                                Top = top,
                                Speakable = true
                            };
                            top += vrLabel.Height;
                            if (top > this.container.Top + this.container.Height)
                            {
                                vrLabel.Visible = false;
                            }
                            if (!this.container.Controls.Contains(vrLabel))
                            {
                                this.container.Controls.Add(vrLabel);
                            }
                            top += 5;
                            if ((!contestDefinition1.GroupSelection.ContainsKey(num1) ? false : contestDefinition1.GroupSelection[num1] != 0))
                            {
                                int num2 = 0;
                                List<DataDefinition> dataDefinitions = contestDefinition1.Data;
                                Func<DataDefinition, bool> func3 = func2;
                                if (func3 == null)
                                {
                                    Func<DataDefinition, bool> func4 = (DataDefinition data) => num2 != 10;
                                    func = func4;
                                    func2 = func4;
                                    func3 = func;
                                }
                                IEnumerable<DataDefinition> dataDefinitions1 = dataDefinitions.TakeWhile<DataDefinition>(func3);
                                Func<DataDefinition, bool> func5 = func1;
                                if (func5 == null)
                                {
                                    Func<DataDefinition, bool> func6 = (DataDefinition data) => (data.State != VrSelection.SelectionState.SELECTED || contestDefinition1.Id + num1 * 7 >= data.Id ? false : data.Id < contestDefinition1.Id + (num1 + 1) * 7);
                                    func = func6;
                                    func1 = func6;
                                    func5 = func;
                                }
                                foreach (DataDefinition dataDefinition in dataDefinitions1.Where<DataDefinition>(func5))
                                {
                                    VrLabel vrLabel3 = new VrLabel()
                                    {
                                        Height = 30,
                                        TextSize = 16,
                                        Text = (string)confirmModel.Vars[string.Concat(new object[] { "$b", confirmModel.Ballot.Id, "_c", contestDefinition1.Id, "_g", contestDefinition1.Group, "_d", dataDefinition.Id })],
                                        TextAlign = "left-middle",
                                        Width = this.container.Width,
                                        Left = this.container.Left,
                                        Top = top,
                                        Speakable = true
                                    };
                                    vrLabel = vrLabel3;
                                    top += vrLabel.Height;
                                    if (top > this.container.Top + this.container.Height)
                                    {
                                        vrLabel.Visible = false;
                                    }
                                    if (!this.container.Controls.Contains(vrLabel))
                                    {
                                        this.container.Controls.Add(vrLabel);
                                    }
                                    num2++;
                                    top += 5;
                                }
                            }
                            else
                            {
                                vrLabel = new VrLabel()
                                {
                                    Height = 30,
                                    TextSize = 16,
                                    Text = (string)confirmModel.Vars["no_selection"],
                                    TextAlign = "left-middle",
                                    Width = this.container.Width,
                                    Left = this.container.Left,
                                    Top = top,
                                    Speakable = true
                                };
                                top += vrLabel.Height;
                                if (top > this.container.Top + this.container.Height)
                                {
                                    vrLabel.Visible = false;
                                }
                                if (!this.container.Controls.Contains(vrLabel))
                                {
                                    this.container.Controls.Add(vrLabel);
                                }
                            }
                            num1++;
                            top += 10;
                        }
                    }
                    else
                    {
                        flag1 = true;
                        vrLabel = new VrLabel()
                        {
                            Text = (string)confirmModel.Vars[string.Concat(new object[] { "$b", confirmModel.Ballot.Id, "_c", contestDefinition1.Id, "_name" })],
                            TextAlign = "left-middle",
                            TextSize = 18,
                            Height = 30,
                            Width = this.container.Width,
                            Left = this.container.Left,
                            Top = top,
                            Speakable = true
                        };
                        top += vrLabel.Height;
                        if (top > this.container.Top + this.container.Height)
                        {
                            vrLabel.Visible = false;
                        }
                        if (!this.container.Controls.Contains(vrLabel))
                        {
                            this.container.Controls.Add(vrLabel);
                        }
                        top += 5;
                        if (contestDefinition1.Selected != 0)
                        {
                            int num3 = 0;
                            foreach (DataDefinition datum in contestDefinition1.Data)
                            {
                                if (num3 == 20)
                                {
                                    break;
                                }
                                else if ((confirmModel.Ballot.ElectionType == ElectionTypes.ranking_choice ? false : contestDefinition1.Type != ContestTypes.V))
                                {
                                    if (datum.State == VrSelection.SelectionState.SELECTED)
                                    {
                                        vrLabel = new VrLabel()
                                        {
                                            Height = 30,
                                            TextSize = 16,
                                            Text = (string)confirmModel.Vars[string.Concat(new object[] { "$b", confirmModel.Ballot.Id, "_c", contestDefinition1.Id, "_g", contestDefinition1.Group, "_d", datum.Id })],
                                            TextAlign = "left-middle",
                                            Width = this.container.Width,
                                            Left = this.container.Left,
                                            Top = top,
                                            Speakable = true
                                        };
                                        top += vrLabel.Height;
                                        if (top > this.container.Top + this.container.Height)
                                        {
                                            vrLabel.Visible = false;
                                        }
                                        if (!this.container.Controls.Contains(vrLabel))
                                        {
                                            this.container.Controls.Add(vrLabel);
                                        }
                                        num3++;
                                        top += 5;
                                    }
                                }
                                else if ((!datum.WriteIn ? false : !datum.IsWritten))
                                {
                                    continue;
                                }
                                else if (datum.Preference > 0)
                                {
                                    vrLabel = new VrLabel()
                                    {
                                        Height = 30,
                                        TextSize = 16
                                    };
                                    string str = (string)confirmModel.Vars[string.Concat(new object[] { "$b", confirmModel.Ballot.Id, "_c", contestDefinition1.Id, "_g", contestDefinition1.Group, "_d", datum.Id })];
                                    int preference = datum.Preference;
                                    vrLabel.Text = string.Concat(str, "   Preference ", preference.ToString());
                                    vrLabel.TextAlign = "left-middle";
                                    vrLabel.Width = this.container.Width;
                                    vrLabel.Left = this.container.Left;
                                    vrLabel.Top = top;
                                    vrLabel.Speakable = true;
                                    top += vrLabel.Height;
                                    if (top > this.container.Top + this.container.Height)
                                    {
                                        vrLabel.Visible = false;
                                    }
                                    if (!this.container.Controls.Contains(vrLabel))
                                    {
                                        this.container.Controls.Add(vrLabel);
                                    }
                                    num3++;
                                    top += 5;
                                }
                            }
                        }
                        else
                        {
                            vrLabel = new VrLabel()
                            {
                                Height = 30,
                                TextSize = 16,
                                Text = (string)confirmModel.Vars["no_selection"],
                                TextAlign = "left-middle",
                                Width = this.container.Width,
                                Left = this.container.Left,
                                Top = top,
                                Speakable = true
                            };
                            top += vrLabel.Height;
                            if (top > this.container.Top + this.container.Height)
                            {
                                vrLabel.Visible = false;
                            }
                            if (!this.container.Controls.Contains(vrLabel))
                            {
                                this.container.Controls.Add(vrLabel);
                            }
                        }
                    }
                    group = contestDefinition1.Group;
                    num++;
                    if ((num == confirmModel.Ballot.ContestsList.Count ? false : contestDefinition1.Propositions == null))
                    {
                        top += 5;
                        VrLabel vrLabel4 = new VrLabel()
                        {
                            Width = this.container.Width - 10,
                            Height = 2,
                            Left = this.container.Left + 5,
                            Top = top,
                            BgColor = "#000000"
                        };
                        this.container.Controls.Add(vrLabel4);
                        top += 20;
                    }
                }
                else
                {
                    num++;
                }
            }
            this._textsToSpeak = base.GetAllTexts(false, false, true);
            //_textsToSpeak.Add("say zero to repeat instruction");

            if (this.container.CreateScroll(confirmModel, top - this.container.Top))
            {
                ScreenObject screenObject = confirmModel.Definition.ScreenObjects[confirmModel.Definition.ScreenObjects.Count - 2];
                ScreenObject top1 = screenObject;
                top1.Top = top1.Top - 25;
                if (screenObject.ObjectState == ScreenObject.ScreenObjectState.ACTIVE)
                {
                    screenObject.Text = (string)confirmModel.Vars["more"];
                }
                screenObject.Tag = "Scroll up";
                ScreenObject item1 = confirmModel.Definition.ScreenObjects[confirmModel.Definition.ScreenObjects.Count - 1];
                if (item1.ObjectState == ScreenObject.ScreenObjectState.ACTIVE)
                {
                    item1.Text = (string)confirmModel.Vars["more"];
                }
                item1.Tag = "Scroll down";
                VrContainer vrContainer = this.container;
                vrContainer.Top = vrContainer.Top + screenObject.Height + 20;
                VrContainer height = this.container;
                height.Height = height.Height - (screenObject.Height + item1.Height);
                if (this._topLine != null)
                {
                    this._topLine.Top = this.Container.Top - 2;
                }
                if (this._bottomLine != null)
                {
                    this._bottomLine.Top = this.Container.Top + this.container.Height + 1;
                }
                foreach (ScreenObject control in this.container.Controls)
                {
                    ScreenObject screenObject1 = control;
                    screenObject1.Top = screenObject1.Top + screenObject.Height + 20;
                    if (control.Top + control.Height > this.container.Top + this.container.Height)
                    {
                        control.Visible = false;
                    }
                }
                this._timer = new Timer()
                {
                    Interval = VrContainer.GetScrollTimerIn(ScrollTimerTypes.ReviewViewScrollTimerIn)
                };
                this._timer.Stop();
                this._timer.Tick += new EventHandler(this.TimerTick);
            }
            base.CreateListOfCommands();
            base.AddSpeechToTextEngine();
            int num4 = 0;
            ConfirmView.mListReviewDetail.Clear();
            foreach (ScreenObject control1 in this.container.Controls)
            {
                num4++;
                ConfirmView.mListReviewDetail.Add(control1.Text);
                if (num4 == 6)
                {
                    ConfirmView.mListReviewDetail.Add(" ");
                }
            }
        }

        private List<ContestDefinition> GetFitOrderContestList()
        {
            List<ContestDefinition> contestDefinitions = new List<ContestDefinition>();
            DataTable dataV2 = DataManager.VotingContentDataInstance.GetDataV2(string.Format("SELECT contest_id,rvo_order_position FROM {0} ", "race_view_options"));
            dataV2.DefaultView.Sort = "rvo_order_position";
            DataTable table = dataV2.DefaultView.ToTable();
            ConfirmModel confirmModel = (ConfirmModel)this.model;
            for (int i = 0; i < table.Rows.Count; i++)
            {
                int num = 0;
                while (num < confirmModel.Ballot.ContestsList.Count)
                {
                    if (int.Parse(table.Rows[i]["contest_id"].ToString()) != confirmModel.Ballot.ContestsList[num].Id)
                    {
                        num++;
                    }
                    else
                    {
                        contestDefinitions.Add(confirmModel.Ballot.ContestsList[num]);
                        break;
                    }
                }
            }
            if (contestDefinitions.Count < confirmModel.Ballot.ContestsList.Count)
            {
                contestDefinitions = confirmModel.Ballot.ContestsList;
            }
            return contestDefinitions;
        }


        private void TimerTick(object sender, EventArgs e)
        {
            TimerCount++;
            if (TimerCount > StartTick && ((ConfirmController)controller).ScrObject != null)
                ((ConfirmController)controller).ScrollContainer((ConfirmModel)model, ((ConfirmModel)model).GetView().Container, ScrollStepTypes.ConfirmViewScrollStep,
                    ((ConfirmController)controller).ScrObject.Data, (string)((ConfirmModel)model).Vars["more"]);
        }

        internal static void PrintReview()
        {
            PrintDocument pd = new PrintDocument();
            Margins margins = new Margins(10, 10, 10, 10);
            pd.DefaultPageSettings.Margins = margins;
            ////document.DefaultPageSettings.PaperSize = new PaperSize("Custom", document.DefaultPageSettings.Bounds.Width, nHeight);
            pd.PrintPage += new PrintPageEventHandler(pd_PrintPage);
            // Print the document.
            pd.Print();

            //System.Threading.Thread oThread = new System.Threading.Thread(document.Print);
            //oThread.Start();
            //pThread.Join();
        }

        private static void pd_PrintPage(object sender, PrintPageEventArgs e)
        {
            float linesPerPage = 0;
            float yPos = 0;
            float leftMargin = e.MarginBounds.Left;
            float topMargin = e.MarginBounds.Top;
            String line = null;

            System.Drawing.Font printFont = new System.Drawing.Font("Arial", 10);

            // Calculate the number of lines per page.
            linesPerPage = e.MarginBounds.Height /
               printFont.GetHeight(e.Graphics);

            for (int i = 0; i < mListReviewDetail.Count; i++)
            {
                line = mListReviewDetail[i].Trim();
                yPos = topMargin + (i * printFont.GetHeight(e.Graphics));
                e.Graphics.DrawString(line, printFont, System.Drawing.Brushes.Black, leftMargin, yPos, new System.Drawing.StringFormat());
            }

            ////// If more lines exist, print another page.
            ////if (line != null)
            ////    e.HasMorePages = true;
            ////else
            e.HasMorePages = false;
        }
    }
}
