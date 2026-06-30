// Product:	VotRite
// Module:  ReviewView.cs
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
using VotRite.UI;
using VotRite.Models;
using VotRite.Controllers;
using VotRite.Definition;

using System.Data;
using VotRite.DBClasses;
using VotRiteBallotDataManager.AppCode;
using System.Drawing.Printing;
using System.Drawing;

namespace VotRite.Views
{
    class ReviewView : VrScreen
    {
        private const int StartTick = 5;
        private ReviewModel vModel { get { return model as ReviewModel; } }
        private ReviewController vController { get { return controller as ReviewController; } }
        private VrLabel lblBallotName;
        private VrLabel lblBallotTitle;
        private VrLabel lblBallotAddress;
        private VrLabel lblBallotDate;
        private VrLabel lblGroupName;
        private VrLabel lblReviewTitle;
        private VrLabel lblReviewTip;

        private VrButton btnSettingTools;
        private VrButton btnCallHelp;
        private VrButton btnPrintReview;
        private VrButton btnBackToVote;
        private VrButton btnResetBallot;
        private VrButton btnAcceptPrint;
        private VrContainer container;
        private VrLabel lblUnderVote;
        private const int SelectionsSpace = 20;
        private Timer _timer;
        protected internal int TimerCount { get; set; }
        private List<string> _textsToSpeak;
        private static List<string> mListReviewDetail = new List<string>();
        
            private static List<string> mListPrint_Final = new List<string>();

        public VrContainer Container { get { return container; } }
        private VrButton btnFontPlus;
        private VrButton btnFontMinus;
        private VrButton btnSelections;
        public static int linesprinted = 0;

        public Timer GetTimer()
        {
            return _timer;
        }

        public ReviewView(ReviewModel m, ReviewController c)
            : base(m, c)
        {
            Name = "review";
            model = m;
            controller = c;
            //vModel.Definition.Background = "graphics\\initialBG_f1.png";
            lblBallotName = (VrLabel)vModel.FindScreenObject("lbl_blt_name");
            lblBallotTitle = (VrLabel)vModel.FindScreenObject("lbl_blt_title");
            lblBallotAddress = (VrLabel)vModel.FindScreenObject("lbl_blt_address");
            lblBallotDate = (VrLabel)vModel.FindScreenObject("lbl_blt_date");
            lblGroupName = (VrLabel)vModel.FindScreenObject("lbl_group_name");
            lblReviewTitle = (VrLabel)vModel.FindScreenObject("lbl_review_title");
            lblReviewTip = (VrLabel)vModel.FindScreenObject("lbl_review_tip");
            AppManager.Instance.ChangeTextSize(lblReviewTip);
            btnSettingTools = (VrButton)vModel.FindScreenObject("btn_setting_tools");
            btnCallHelp = (VrButton)vModel.FindScreenObject("btn_callhelp");
            btnPrintReview = (VrButton)vModel.FindScreenObject("btn_print_review");
            btnBackToVote = (VrButton)vModel.FindScreenObject("btn_back_to_vote");
            btnResetBallot = (VrButton)vModel.FindScreenObject("btn_reset_ballot");
            btnAcceptPrint = (VrButton)vModel.FindScreenObject("btn_accept_print");
            container = (VrContainer)vModel.FindScreenObject("ctr_review");
            lblUnderVote = (VrLabel)vModel.FindScreenObject("lbl_under_vote");
            //btnSettingTools = null;
            // container.BgColor = "#FFFFFF";
            AppManager.Instance.reviewprinted = false;
            Initialize();
            setBallotTextForLocale();
            AppManager.Instance.checkDoubleSpaceSetting(vModel.Definition.ScreenObjects, container);

            if (lblReviewTip.TextSize > 18)
            {
                lblReviewTip.Top -= 40;
                lblReviewTip.Height += (lblReviewTip.TextSize - 18)*15;
                lblReviewTip.Width += 50;
                lblUnderVote.Top += (lblReviewTip.TextSize - 18) * 15;
                lblUnderVote.Height += (lblReviewTip.TextSize - 18)*2;

                lblUnderVote.Text = lblUnderVote.Text.Replace("containes","contains" );

                lblUnderVote.TextSize = lblReviewTip.TextSize;
                btnBackToVote.TextSize = lblReviewTip.TextSize;
                btnAcceptPrint.TextSize = lblReviewTip.TextSize;
                btnResetBallot.TextSize = lblReviewTip.TextSize;
                btnFontPlus.TextSize = lblReviewTip.TextSize;
                btnFontMinus.TextSize = lblReviewTip.TextSize;
                btnPrintReview.TextSize = lblReviewTip.TextSize;
            }
            //this.container.Height = 700;
            lblUnderVote.Text = lblUnderVote.Text.Replace("containes", "contains");

            int toppos = 235;
            bool advanceHeight = false;
            foreach (var ctrl in this.container.Controls)
            {
                
                if (ctrl != null)
                {
                    if (AppManager.Instance.reviewResultFontSize > 20)
                    {
                        if (ctrl.Type == ScreenObject.ScreenObjectType.SELECTION)
                        {
                            //if (ctrl.Tag == "Proposition")
                            //    ctrl.Top += (AppManager.Instance.reviewResultFontSize - 20) * 4;
                            //else
                            //    ctrl.Top += (AppManager.Instance.reviewResultFontSize - 20) * 3;
                            ctrl.Left += 10;
                            ctrl.Height += (AppManager.Instance.reviewResultFontSize - 20) * 3;
                            // ctrl.Width = 700;
                        }
                        //else if (ctrl.Type == ScreenObject.ScreenObjectType.SCROLL && ctrl.Data == "Down")
                        //    ctrl.Top += 50;
                        else
                        {
                            //if (ctrl.Tag != "slate")
                            //{
                            //    if (ctrl.Tag == "PropositionHead") 
                            //        ctrl.Top += (AppManager.Instance.reviewResultFontSize - 20) * 3;
                            //    else
                            //        ctrl.Top += (AppManager.Instance.reviewResultFontSize - 20) * 2;
                            //}
                            ctrl.Height += 10 + (AppManager.Instance.reviewResultFontSize - 20) * 2;
                            ctrl.Width += (AppManager.Instance.reviewResultFontSize - 20) * 2;

                            if (ctrl.Text != null)
                            {
                                if (ctrl.Text.Length > 50 )
                                {
                                    //ctrl.TextSize = 26;
                                    //ctrl.Height += 10 + (AppManager.Instance.reviewResultFontSize - 20) * 2;
                                    ctrl.Height += (10 + (AppManager.Instance.reviewResultFontSize - 20) * 2) * (ctrl.Text.Length / 50);
                                    advanceHeight = true;
                                }
                                
                            }
                        }

                        if (advanceHeight)// && ctrl.Type != ScreenObject.ScreenObjectType.SELECTION)
                        {
                            ctrl.Top = toppos + 10;
                            advanceHeight = false;
                        }
                        else
                            ctrl.Top = toppos+10;

                        toppos = ctrl.Top + ctrl.Height-9;
                    }

                    if (AppManager.Instance.backgroundTheme == AppManager.colorTheme.Contrast
                                || AppManager.Instance.backgroundTheme == AppManager.colorTheme.Blue )
                    {
                        if (ctrl.Type == ScreenObject.ScreenObjectType.LABEL)
                        {
                            if (ctrl.ForeColor == "#000000" && ctrl.BgColor != "#fff")
                                ctrl.ForeColor = "#FFFFFF";
                        }
                        if (ctrl.Type == ScreenObject.ScreenObjectType.SELECTION && ctrl is VrSelection )
                        {
                            if (((VrSelection)ctrl).State == VrSelection.SelectionState.SELECTED)
                                ctrl.ForeColor = "#000000";
                        }
                            

                    }
                }

                if (ctrl.Text != null)
                {
                    int ix = ctrl.Text.IndexOf("  ");
                    if (ctrl.Type == ScreenObject.ScreenObjectType.SELECTION)
                    {
                        ctrl.Width = 700;
                        if (ix > 0)
                            ctrl.Text = ctrl.Text.Insert(ix, " / ");
                    }
                    var words = ctrl.Text.Split(' ');
                    string txt = "";
                    foreach (string word in words)
                    {
                        txt += (word == " " || word == "" ? "" : word + " ");
                    }

                    ctrl.Text = txt.Trim();
                }
              
            }
            var additionalSet = new HashSet<string> { "return to voting", "print review in normal text size", "print review in zoom text size", "accept and print ballot, you will have a second opportunity to review on the next page", "Repeat review of candidates",  "cancel ballot" };
            AppManager.Instance._contestSelections.Clear();
            CreateListOfCommands(additionalSet);
            foreach (var text in _textsToSpeak.Where(text => text != null))
            {
                AppManager.Instance.StartSpeaker(text);
            }

            

            AppManager.Instance.initiateHearingMode(true);
            AppManager.Instance.controllerForHearingMode = vController;
            AppManager.Instance.reviewModel = vModel;
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _timer.Dispose();
                _textsToSpeak.Clear();

                if (lblBallotName != null) lblBallotName.Dispose();
                if (lblBallotTitle != null) lblBallotTitle.Dispose();
                if (lblBallotAddress != null) lblBallotAddress.Dispose();
                if (lblBallotDate != null) lblBallotDate.Dispose();
                if (lblGroupName != null) lblGroupName.Dispose();
                //if (btnSettingTools != null) btnSettingTools.Dispose();
                if (btnCallHelp != null) btnCallHelp.Dispose();
                if (btnBackToVote != null) btnBackToVote.Dispose();
                if (btnResetBallot != null) btnResetBallot.Dispose();
                if (btnAcceptPrint != null) btnAcceptPrint.Dispose();
                if (btnPrintReview != null) btnPrintReview.Dispose();
                if (lblReviewTitle != null) lblReviewTitle.Dispose();
                if (lblReviewTip != null) lblReviewTip.Dispose();
                if (container != null) container.Dispose();
                if (lblUnderVote != null) lblUnderVote.Dispose();               
            }
            base.Dispose(disposing);
        }

        private void Initialize_new()
        {
            VrSelection sel;
            lblUnderVote.Visible = false;

            if (lblBallotName != null)
                lblBallotName.Text = (string)vModel.Vars["ballot_name"];
            if (lblBallotTitle != null)
                lblBallotTitle.Text = (string)vModel.Vars["ballot_title"];
            if ((string)vModel.Vars["ballot_address"] != "")
                lblBallotAddress.Text = (string)vModel.Vars["ballot_address"];
            if (lblBallotDate != null)
                lblBallotDate.Text = (string)vModel.Vars["ballot_date"];
            if (lblGroupName != null)
                lblGroupName.Text = (string)vModel.Vars["group_name"];

            //if (btnSettingTools != null)
            //{
            //    btnSettingTools.Text = (string)vModel.Vars["btn_setting_tools"];
            //    btnSettingTools.Visible = vModel.Ballot.ShowSettingsAndTools;
            //}

            if (vModel.Ballot.HasOverview)
            {
                btnCallHelp = new VrButton
                {
                    Name = "btn_callhelp",
                    BgImage = AppManager.GetPathToCommonFile("graphics" + Global.Instance.SLASH +
                                                                                 "help_light.png"),
                    Text = "     Ballot",
                    ForeColor = "#fff",
                    TextSize = 18,
                    TextAlign = "center-middle",
                    Width = 150,
                    Height = 63,
                    Left = 25,
                    Top = 780,
                    Action = ScreenObject.ScreenObjectAction.GET_SCREEN,
                    Data = "ballotoverview"
                };
                vModel.Definition.ScreenObjects.Add(btnCallHelp);
            }

            if (btnBackToVote != null)
            {
                if (vModel.Ballot.slatesDefinition.Slates.Count > 0)
                    btnBackToVote.Text = (string)vModel.Vars["back_to_vote_slate"];
                else
                    btnBackToVote.Text = (string)vModel.Vars["back_to_vote"];

                btnBackToVote.BgImage = "";
            }
            if (btnResetBallot != null)
                btnResetBallot.Text = (string)vModel.Vars["reset_ballot"];
            if (btnAcceptPrint != null)
                btnAcceptPrint.Text = (string)vModel.Vars["accept_print"];
            if (lblReviewTitle != null)
                lblReviewTitle.Text = (string)vModel.Vars["review_title"];
            if (lblReviewTip != null)
                lblReviewTip.Text = (string)vModel.Vars["review_tip"];
            if (lblUnderVote != null)
            {
                lblUnderVote.Text = (string)vModel.Vars["undervote_msg"];
            }

            int topHist = container.Top + 15;
            int contGroup = 0;
            int contCounter = 0;
            int leftPadding = 5;
            int selectionWidth = (container.Width - leftPadding - SelectionsSpace) / 2;
            int leftColumnSelectionsPosition = container.Left + leftPadding;
            int rightColumnSelectionsPosition = container.Left + leftPadding + selectionWidth + SelectionsSpace;
            var showGroupForMassPropositions = true;

            // ranking choice
            
            if (vModel.Ballot.ElectionType == VotRiteBallotDataManager.AppCode.ElectionTypes.ranking_choice)
            {
                VrLabel lbl = new VrLabel
                {
                    Text = "Ranking Choice Voting",
                    TextAlign = "left-middle",
                    TextSize = AppManager.Instance.reviewHeaderFontSize,
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
                
                
                var startTopHist = topHist;

                sel = new VrSelection
                {
                    Data = "ranking_choice",
                    Height = 60,
                    TextSize = 18,
                    Text = vModel.Ballot.slatesDefinition.Slates.FirstOrDefault(it => it.Id == vModel.Ballot.slatesDefinition.Data.SlateId).Name,
                    State = VrSelection.SelectionState.SELECTED,
                    TextAlign = "left-middle",
                    Left = leftColumnSelectionsPosition,
                    Width = selectionWidth
                };

                sel.Top = topHist;

                topHist += sel.Height;

                if (topHist > container.Top + container.Height)
                    sel.Visible = false;

                if (!container.Controls.Contains(sel))
                    container.Controls.Add(sel);
                
                topHist += 5;

            }
            
            // ranking choice end

            List<ContestDefinition> lstContest = GetFitOrderContestList();
            DBClasses.Slate slateSelected = null;
           // VrSelection sel;

            if (vModel.Ballot.slatesDefinition.Slates.Count > 0)
            {
                foreach (ContestDefinition contest in lstContest)
                {
                    if (contest.Selected > 0)
                    {
                        if (contest.Id == vModel.Ballot.slatesDefinition.Data.Id)
                        {
                            foreach (var slate in vModel.Ballot.slatesDefinition.Slates)
                            {
                                if (slate.Id == vModel.Ballot.slatesDefinition.Data.SlateId)
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
                        TextSize = AppManager.Instance.reviewHeaderFontSize,
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

                    sel = new VrSelection();
                    sel.Data = Convert.ToString(contCounter);
                    sel.Height = 60;
                    sel.TextSize = 18;
                    sel.Text = slateSelected.Name;
                    sel.Tag = "slate";
                    sel.State = VrSelection.SelectionState.SELECTED;
                    sel.TextAlign = "left-middle";

                    sel.Left = leftColumnSelectionsPosition;

                    sel.Width = selectionWidth;

                    sel.Top = topHist;

                    topHist += sel.Height;

                    if (topHist > container.Top + container.Height)
                        sel.Visible = false;

                    if (!container.Controls.Contains(sel))
                        container.Controls.Add(sel);

                    topHist += 5;
                }
            }

            foreach (ContestDefinition contest in lstContest)
            {
                if (contest.MaxSelection > contest.Selected)
                {
                    lblUnderVote.Visible = true;
                }

                if (contest.Type == VotRiteBallotDataManager.AppCode.ContestTypes.R && slateSelected != null)
                {
                    contCounter++; continue;
                }

                VrLabel lbl;

                if (!(contGroup == contest.Group || (contest.Propositions != null && !showGroupForMassPropositions)))
                {
                    lbl = new VrLabel();
                    lbl.Text = (string)vModel.Vars["$b" + vModel.Ballot.Id + "_g" + contest.Group + "_name"];
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

                //VrSelection sel;

                if (contest.Propositions == null)
                {
                    showGroupForMassPropositions = true;
                    lbl = new VrLabel();
                    lbl.Text = (string)vModel.Vars["$b" + vModel.Ballot.Id + "_c" + contest.Id + "_name"];
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
                        //Missing anything,need to how under vote
                        lblUnderVote.Visible = true;

                        sel = new VrSelection();

                        if (vModel.Ballot.ElectionType == VotRiteBallotDataManager.AppCode.ElectionTypes.ranking_choice)
                        {
                            sel.Data = "ranking_choice";
                        }
                        else
                        {
                            sel.Data = Convert.ToString(contCounter);
                        }

                        sel.Height = 60;
                        sel.TextSize = 18;
                        sel.Text = (string)vModel.Vars["no_selection"];
                        sel.TextAlign = "left-middle";
                        sel.Width = selectionWidth;
                        sel.Left = leftColumnSelectionsPosition;
                        sel.Top = topHist;

                        topHist += sel.Height;

                        if (topHist > container.Top + container.Height)
                            sel.Visible = false;
                        if (!container.Controls.Contains(sel))
                            container.Controls.Add(sel);

                        topHist += 20;
                    }
                    else
                    {
                        int maxSel = 0;
                        int startTopHist = topHist;
                        int selected_count = contest.Data.Count(it => it.State == VrSelection.SelectionState.SELECTED);

                        if (vModel.Ballot.ElectionType == VotRiteBallotDataManager.AppCode.ElectionTypes.ranking_choice)
                        {
                            VrLabel prefTitle = new VrLabel()
                            {
                                Height = 20,
                                Width = 100,
                                TextSize = 18,
                                TextAlign = "center-middle",
                                Text = "Preference",
                                Left = rightColumnSelectionsPosition,
                                Top = topHist
                            };

                            topHist += 40;

                            if (!container.Controls.Contains(prefTitle))
                                container.Controls.Add(prefTitle);

                            selected_count = contest.Selected;
                            VrLabel candidate;
                            VrLabel preference;

                            DataDefinition d = null;

                            foreach (DataDefinition data in contest.Data)
                            {
                                if (data.WriteIn && !data.IsWritten)
                                {
                                    d = data;
                                }
                            }

                            if (d != null)
                            {
                                contest.Data.Remove(d);
                            }

                            foreach (DataDefinition data in contest.Data)
                            {
                                candidate = new VrLabel();
                                candidate.Data = Convert.ToString(contCounter);
                                candidate.Height = 60;
                                candidate.TextSize = 18;
                                candidate.Text =
                                    (string)
                                    vModel.Vars[
                                        "$b" + vModel.Ballot.Id + "_c" + contest.Id + "_g" + contest.Group + "_d" +
                                        data.Id];
                                candidate.TextAlign = "left-middle";
                                candidate.BgColor = "#fff";
                                candidate.BorderWidth = 2;
                                candidate.Left = leftColumnSelectionsPosition;
                                candidate.Width = selectionWidth;
                                candidate.Top = topHist;

                                preference = new VrLabel()
                                {
                                    Height = 60,
                                    Width = 100,
                                    TextSize = 18,
                                    TextAlign = "center-middle",
                                    Text = data.Preference.ToString(),
                                    Left = rightColumnSelectionsPosition,
                                    Top = topHist
                                };

                                topHist += candidate.Height;

                                if (topHist > container.Top + container.Height)
                                {
                                    candidate.Visible = false;
                                    preference.Visible = false;
                                }

                                if (!container.Controls.Contains(candidate))
                                    container.Controls.Add(candidate);

                                if (!container.Controls.Contains(preference))
                                    container.Controls.Add(preference);

                                maxSel++;
                                topHist += 5;
                            }
                        }
                        else
                        {
                            foreach (DataDefinition data in contest.Data.Where(it => it.State == VrSelection.SelectionState.SELECTED))
                            {
                                if (maxSel == 20) break;

                                if (data.State == VrSelection.SelectionState.SELECTED)
                                {
                                    sel = new VrSelection();
                                    sel.Data = Convert.ToString(contCounter);
                                    sel.Height = 60;
                                    sel.TextSize = 18;
                                    sel.Text =
                                        (string)
                                        vModel.Vars[
                                            "$b" + vModel.Ballot.Id + "_c" + contest.Id + "_g" + contest.Group + "_d" +
                                            data.Id];
                                    sel.Group = data.Group;
                                    sel.State = VrSelection.SelectionState.SELECTED;
                                    sel.TextAlign = "left-middle";

                                    if (maxSel < (selected_count > 3 ? Math.Floor((double)selected_count / 2) : selected_count))
                                        sel.Left = leftColumnSelectionsPosition;
                                    else
                                        sel.Left = rightColumnSelectionsPosition;

                                    sel.Width = selectionWidth;

                                    if (maxSel == (selected_count > 3 ? Math.Floor((double)selected_count / 2) : selected_count))
                                        topHist = startTopHist;

                                    sel.Top = topHist;

                                    topHist += sel.Height;

                                    if (topHist > container.Top + container.Height)
                                        sel.Visible = false;

                                    if (!container.Controls.Contains(sel))
                                        container.Controls.Add(sel);
                                    maxSel++;
                                    topHist += 5;
                                }
                            }
                        }
                    }
                }
                else
                {
                    showGroupForMassPropositions = false;
                    var idx = 0;

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
                            sel = new VrSelection();
                            sel.Data = Convert.ToString(contCounter);
                            sel.Height = 60;
                            sel.TextSize = 18;
                            sel.Text = (string)vModel.Vars["no_selection"];
                            sel.TextAlign = "left-middle";
                            sel.Width = selectionWidth;
                            sel.Left = leftColumnSelectionsPosition;
                            sel.Top = topHist;

                            topHist += sel.Height;

                            if (topHist > container.Top + container.Height)
                                sel.Visible = false;
                            if (!container.Controls.Contains(sel))
                                container.Controls.Add(sel);

                            topHist += 20;
                        }
                        else
                        {
                            var maxSel = 0;
                            var startTopHist = topHist;

                            foreach (var data in contest.Data.TakeWhile(data => maxSel != 10).
                                Where(data => data.State == VrSelection.SelectionState.SELECTED && contest.Id + idx * 7 < data.Id && data.Id < contest.Id + (idx + 1) * 7))
                            {
                                sel = new VrSelection();
                                sel.Data = Convert.ToString(contCounter);
                                sel.Height = 60;
                                sel.TextSize = 18;
                                sel.Text =
                                    (string)
                                    vModel.Vars[
                                        "$b" + vModel.Ballot.Id + "_c" + contest.Id + "_g" + contest.Group + "_d" +
                                        data.Id];
                                sel.Group = data.Group;
                                sel.State = VrSelection.SelectionState.SELECTED;
                                sel.TextAlign = "left-middle";

                                sel.Left = maxSel < 5 ? leftColumnSelectionsPosition : rightColumnSelectionsPosition;

                                sel.Width = selectionWidth;

                                if (maxSel == 5)
                                    topHist = startTopHist;

                                sel.Top = topHist;

                                topHist += sel.Height;

                                if (topHist > container.Top + container.Height)
                                    sel.Visible = false;

                                if (!container.Controls.Contains(sel))
                                    container.Controls.Add(sel);
                                maxSel++;
                                topHist += 5;
                            }
                        }
                        idx++;
                    }
                }

                contGroup = contest.Group;
                contCounter++;

                if (contCounter != vModel.Ballot.ContestsList.Count && contest.Propositions == null)
                    topHist += 20;
            }

            _textsToSpeak = GetAllTexts(false, false, true);

            if (container.CreateScroll(vModel, topHist - container.Top))
            {
                var upScroll = vModel.Definition.ScreenObjects[vModel.Definition.ScreenObjects.Count - 2];
                upScroll.Top -= 25;

                if (upScroll.ObjectState == ScreenObject.ScreenObjectState.ACTIVE)
                    upScroll.Text = (string)vModel.Vars["more"];
                upScroll.Tag = "Scroll up";

                var downScroll = vModel.Definition.ScreenObjects[vModel.Definition.ScreenObjects.Count - 1];
                if (downScroll.ObjectState == ScreenObject.ScreenObjectState.ACTIVE)
                    downScroll.Text = (string)vModel.Vars["more"];
                downScroll.Tag = "Scroll down";
                container.Top += upScroll.Height;
                container.Height -= upScroll.Height + downScroll.Height + 20;

                foreach (var screenObject in container.Controls)
                {
                    screenObject.Top += upScroll.Height;
                    if (screenObject.Top + screenObject.Height > container.Top + container.Height)
                        screenObject.Visible = false;
                }
                _timer = new Timer { Interval = VrContainer.GetScrollTimerIn(ScrollTimerTypes.ReviewViewScrollTimerIn) };
                _timer.Stop();
                _timer.Tick += TimerTick;
            }

            CreateListOfCommands();
            //AddSpeechToTextEngine();
            
        }

        private void Initialize()
        {
            VrSelection vrSelection;
            VrLabel vrLabel;
            Func<DataDefinition, bool> func;
            bool flag;
            Func<DataDefinition, bool> func1 = null;
            Func<DataDefinition, bool> func2 = null;
            this.lblUnderVote.Visible = false;
            string currLocale = AppManager.Instance.GetLocale();
            string currLocaleCode = (!currLocale.Contains("-") ? currLocale : currLocale.Substring(0, currLocale.IndexOf("-")));

            if (AppManager.Instance.backgroundTheme != AppManager.colorTheme.White)
            {
                vModel.Definition.Background = AppManager.GetPathToCommonFile("graphics" + Global.Instance.SLASH + "initialBG_f1_" + AppManager.Instance.backgroundTheme + ".png");
            }

            if (this.lblBallotName != null)
            {
                if (currLocale == "en-US")
                    this.lblBallotName.Text = (string)this.vModel.Vars["ballot_name"];
                else
                {
                    // this.lblBallotName.Text = Global.Instance.TranslateText(new string[] { (string)this.vModel.Vars["ballot_name"] }, "en", currLocaleCode)[0];
                }
                this.lblBallotName.ForeColor = "#000000";
            }
            if (this.lblBallotTitle != null)
            {
                if (currLocale == "en-US")
                    this.lblBallotTitle.Text = (string)this.vModel.Vars["ballot_title"];
                else
                {
                    // this.lblBallotTitle.Text = Global.Instance.TranslateText(new string[] { (string)this.vModel.Vars["ballot_title"] }, "en", currLocaleCode)[0];
                }
                this.lblBallotTitle.ForeColor = "#000000";
            }
            if ((string)this.vModel.Vars["ballot_address"] != "")
            {
                this.lblBallotAddress.Text = (string)this.vModel.Vars["ballot_address"];
                this.lblBallotAddress.ForeColor = "#000000";
            }
            if (this.lblBallotDate != null)
            {
                this.lblBallotDate.Text = (string)this.vModel.Vars["ballot_date"];
                this.lblBallotDate.ForeColor = "#000000";
            }
            if (this.lblGroupName != null)
            {
                this.lblGroupName.Text = (string)this.vModel.Vars["group_name"];
            }
            if (this.btnSettingTools != null)
            {
                this.btnSettingTools.Text = (string)this.vModel.Vars["btn_setting_tools"];
                this.btnSettingTools.Visible = false;// this.vModel.Ballot.ShowSettingsAndTools;
                this.btnSettingTools.Top -= 400;
            }
            if (this.vModel.Ballot.HasOverview)
            {
                this.btnCallHelp = new VrButton()
                {
                    Name = "btn_callhelp",
                    BgImage = AppManager.GetPathToCommonFile(string.Concat("graphics", Global.Instance.SLASH, "help_light.png")),
                    Text = "     Ballot",
                    ForeColor = "#fff",
                    TextSize = 18,
                    TextAlign = "center-middle",
                    Width = 150,
                    Height = 63,
                    Left = 25,
                    Top = 780,
                    Action = ScreenObject.ScreenObjectAction.GET_SCREEN,
                    Data = "ballotoverview"
                };
                this.vModel.Definition.ScreenObjects.Add(this.btnCallHelp);
            }
            if (this.btnBackToVote != null)
            {
                //if (this.vModel.Ballot.slatesDefinition.Slates.Count <= 0)
                //{
                this.btnBackToVote.Text = (string)this.vModel.Vars["back_to_vote"];
                //}
                //else
                //{
                //    this.btnBackToVote.Text = (string)this.vModel.Vars["back_to_vote_slate"];
                //}
            }
            if (this.btnResetBallot != null)
            {
                this.btnResetBallot.Text = (string)this.vModel.Vars["reset_ballot"];
                this.btnResetBallot.BgImage = "graphics\\button_red.jpg";
            }
            if (this.btnAcceptPrint != null)
            {
                this.btnAcceptPrint.Text = (string)this.vModel.Vars["accept_print"];
            }
            if (btnPrintReview != null)
            {
                //btnPrintReview.BgImage = "graphics\\bg_key_pp2.png";
                btnPrintReview.ForeColor = "#000";
                btnPrintReview.Text = "Print Review";// (string)this.vModel.Vars["print_review"];
                //if (!AppManager.ShowPrintReview)
                //{
                //    btnPrintReview.Visible = false;
                //}
            }

            this.btnFontPlus = new VrButton()
            {
                Name = "btn_Font++",
                BgImage = btnPrintReview.BgImage,
                Text = "Zoom",
                ForeColor = "#000",
                TextSize = 18,
                TextAlign = "center-middle",
                Width = 100,
                Height = 63,
                Left = 25,
                Top = 720,
                // Action = ScreenObject.ScreenObjectAction.GET_SCREEN,
                // Data = "ballotoverview"
            };
            this.vModel.Definition.ScreenObjects.Add(this.btnFontPlus);

            this.btnFontMinus = new VrButton()
            {
                Name = "btn_Font--",
                BgImage = btnPrintReview.BgImage,
                Text = "Normal",
                ForeColor = "#000",
                TextSize = 18,
                TextAlign = "center-middle",
                Width = 150,
                Height = 63,
                Left = 140,
                Top = 720,
                //Action = ScreenObject.ScreenObjectAction.GET_SCREEN,
                //Data = "ballotoverview"
            };
            if (AppManager.Instance.ballot.BallotMode != Session.BallotModes.Audio)
            {
                this.btnSelections = new VrButton()
                {
                    Name = "btn_Selections",
                    BgImage = btnPrintReview.BgImage,
                    Text = "View Selections",
                    ForeColor = "#000",
                    TextSize = 18,
                    TextAlign = "center-middle",
                    Width = 250,
                    Height = 63,
                    Left = 25,
                    Top = 647,
                    //Action = ScreenObject.ScreenObjectAction.GET_SCREEN,
                    //Data = "ballotoverview"
                };
                this.vModel.Definition.ScreenObjects.Add(btnSelections);
            }
            this.vModel.Definition.ScreenObjects.Add(btnFontMinus);

            if (this.lblReviewTitle != null)
            {
                this.lblReviewTitle.Text = (string)this.vModel.Vars["review_title"];
            }
            if (this.lblReviewTip != null)
            {
                this.lblReviewTip.Text = (string)this.vModel.Vars["review_tip"];
                if (AppManager.Instance.ballot.BallotMode == Session.BallotModes.Audio)
                {
                    this.lblReviewTip.Text = this.lblReviewTip.Text.Replace("Read", "Hear").Replace("press", "choose");
                    this.lblReviewTip.Text = this.lblReviewTip.Text.Replace("If you wish you can write down your", "A").Replace("press", "choose").Replace("which ", "");
                }
                //this.lblReviewTip.Text = this.lblReviewTip.Text.Replace("If you wish you can write down your voting session number which will appear on ", "A").Replace("press", "choose");
            }
            if (this.lblUnderVote != null)
            {
                this.lblUnderVote.Text = (string)this.vModel.Vars["undervote_msg"];
            }
            int top = this.container.Top + 15;
            int group = 0;
            int num = 0;
            int num1 = 5;
            int width = (this.container.Width - num1 - 20) / 2;
            int left = this.container.Left + num1;
            int left1 = this.container.Left + num1 + width + 20;
            bool flag1 = true;
            List<ContestDefinition> fitOrderContestList = this.GetFitOrderContestList();
            AppManager.Instance.reviewSelectionList = fitOrderContestList;
            Slate slate = null;
            if (this.vModel.Ballot.slatesDefinition.Slates.Count > 0)
            {
                foreach (ContestDefinition contestDefinition in fitOrderContestList)
                {
                    if (contestDefinition.Selected > 0)
                    {
                        if (contestDefinition.Id == this.vModel.Ballot.slatesDefinition.Data.Id)
                        {
                            foreach (Slate slate1 in this.vModel.Ballot.slatesDefinition.Slates)
                            {
                                if (slate1.Id == this.vModel.Ballot.slatesDefinition.Data.SlateId)
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
                    VrLabel vrLabel1 = new VrLabel()
                    {
                        Text = "Slates Voting",
                        TextAlign = "left-middle",
                        TextSize = AppManager.Instance.reviewHeaderFontSize,
                        Height = 30,
                        Width = this.container.Width,
                        Left = this.container.Left,
                        Top = top,
                        Speakable = true
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
                    vrSelection = new VrSelection()
                    {
                        Data = Convert.ToString(num),
                        Height = 60,
                        TextSize = AppManager.Instance.reviewResultFontSize,
                        Text = slate.Name,
                        Tag = "slate",
                        State = VrSelection.SelectionState.SELECTED,
                        TextAlign = "left-middle",
                        Left = left,
                        Width = width,
                        Top = top
                    };
                    top += vrSelection.Height;
                    if (top > this.container.Top + this.container.Height)
                    {
                        vrSelection.Visible = false;
                    }
                    if (!this.container.Controls.Contains(vrSelection))
                    {
                        this.container.Controls.Add(vrSelection);
                    }
                    top += 5;
                }
            }
            foreach (ContestDefinition contestDefinition1 in fitOrderContestList)
            {
                if (contestDefinition1.MaxSelection > contestDefinition1.Selected)
                {
                    this.lblUnderVote.Visible = true;
                }
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
                            Text = (string)this.vModel.Vars[string.Concat(new object[] { "$b", this.vModel.Ballot.Id, "_g", contestDefinition1.Group, "_name" })],
                            TextAlign = "left-middle",
                            TextSize = AppManager.Instance.reviewHeaderFontSize,
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
                        flag1 = false;
                        int num2 = 0;
                        foreach (Proposition proposition in contestDefinition1.Propositions)
                        {
                            if(proposition.Title == null)
                            {
                                var currprop = Proposition.GetProposition(proposition.ContestId, AppManager.Instance.Session.CurrentLocale.Id);
                                proposition.Title = currprop.Title;
                            }
                            vrLabel = new VrLabel()
                            {
                                Text = proposition.Title,
                                TextAlign = "left-middle",
                                TextSize = AppManager.Instance.reviewResultFontSize,
                                Height = 30,
                                Width = this.container.Width,
                                Left = this.container.Left,
                                Top = top,
                                Speakable = true,
                                Tag= "PropositionHead"
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
                            if ((!contestDefinition1.GroupSelection.ContainsKey(num2) ? false : contestDefinition1.GroupSelection[num2] != 0))
                            {
                                int num3 = 0;
                                int num4 = top;
                                List<DataDefinition> dataDefinitions = contestDefinition1.Data;
                                Func<DataDefinition, bool> func3 = func2;
                                if (func3 == null)
                                {
                                    Func<DataDefinition, bool> func4 = (DataDefinition data) => num3 != 10;
                                    func = func4;
                                    func2 = func4;
                                    func3 = func;
                                }
                                IEnumerable<DataDefinition> dataDefinitions1 = dataDefinitions.TakeWhile<DataDefinition>(func3);
                                Func<DataDefinition, bool> func5 = func1;
                                if (func5 == null)
                                {
                                    Func<DataDefinition, bool> func6 = (DataDefinition data) => (data.State != VrSelection.SelectionState.SELECTED || contestDefinition1.Id + num2 * 7 >= data.Id ? false : data.Id < contestDefinition1.Id + (num2 + 1) * 7);
                                    func = func6;
                                    func1 = func6;
                                    func5 = func;
                                }
                                foreach (DataDefinition dataDefinition in dataDefinitions1.Where<DataDefinition>(func5))
                                {
                                    vrSelection = new VrSelection()
                                    {
                                        Data = Convert.ToString(num),
                                        Height = 60,
                                        TextSize = AppManager.Instance.reviewResultFontSize,
                                        Text = (string)this.vModel.Vars[string.Concat(new object[] { "$b", this.vModel.Ballot.Id, "_c", contestDefinition1.Id, "_g", contestDefinition1.Group, "_d", dataDefinition.Id })],
                                        Group = dataDefinition.Group,
                                        State = VrSelection.SelectionState.SELECTED,
                                        TextAlign = "left-middle",
                                        Left = (num3 < 5 ? left : left1),
                                        Width = width,
                                        Tag = "Proposition"
                                    };
                                    if (num3 == 5)
                                    {
                                        top = num4;
                                    }
                                    vrSelection.Top = top;
                                    top += vrSelection.Height;
                                    if (top > this.container.Top + this.container.Height)
                                    {
                                        vrSelection.Visible = false;
                                    }
                                    if (!this.container.Controls.Contains(vrSelection))
                                    {
                                        this.container.Controls.Add(vrSelection);
                                    }
                                    num3++;
                                    top += 5;
                                }
                            }
                            else
                            {
                                vrSelection = new VrSelection()
                                {
                                    Data = Convert.ToString(num),
                                    Height = 60,
                                    TextSize = AppManager.Instance.reviewResultFontSize,
                                    Text = (string)this.vModel.Vars["no_selection"],
                                    TextAlign = "left-middle",
                                    Width = width,
                                    Left = left,
                                    Top = top,
                                    Tag = "Proposition"
                                };
                                top += vrSelection.Height;
                                if (top > this.container.Top + this.container.Height)
                                {
                                    vrSelection.Visible = false;
                                }
                                if (!this.container.Controls.Contains(vrSelection))
                                {
                                    this.container.Controls.Add(vrSelection);
                                }
                                top += 20;
                            }
                            num2++;
                        }
                    }
                    else
                    {
                        flag1 = true;
                        vrLabel = new VrLabel()
                        {
                            Text = (string)this.vModel.Vars[string.Concat(new object[] { "$b", this.vModel.Ballot.Id, "_c", contestDefinition1.Id, "_name" })],
                            TextAlign = "left-middle",
                            TextSize = AppManager.Instance.reviewResultFontSize,
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
                            int num5 = 0;
                            int num6 = top;
                            int selected = contestDefinition1.Data.Count<DataDefinition>((DataDefinition it) => it.State == VrSelection.SelectionState.SELECTED);
                            if ((this.vModel.Ballot.ElectionType == ElectionTypes.ranking_choice ? false : contestDefinition1.Type != ContestTypes.V))
                            {
                                foreach (DataDefinition dataDefinition1 in
                                    from it in contestDefinition1.Data
                                    where it.State == VrSelection.SelectionState.SELECTED
                                    select it)
                                {
                                    if (num5 == 20)
                                    {
                                        break;
                                    }
                                    else if (dataDefinition1.State == VrSelection.SelectionState.SELECTED)
                                    {
                                        vrSelection = new VrSelection()
                                        {
                                            Data = Convert.ToString(num),
                                            Height = 60,
                                            TextSize = AppManager.Instance.reviewResultFontSize,
                                            Text = (string)this.vModel.Vars[string.Concat(new object[] { "$b", this.vModel.Ballot.Id, "_c", contestDefinition1.Id, "_g", contestDefinition1.Group, "_d", dataDefinition1.Id })],
                                            Group = dataDefinition1.Group,
                                            State = VrSelection.SelectionState.SELECTED,
                                            TextAlign = "left-middle"
                                        };
                                        if ((double)num5 >= (selected > 3 ? Math.Floor((double)selected / 2) : (double)selected))
                                        {
                                            vrSelection.Left = left1;
                                        }
                                        else
                                        {
                                            vrSelection.Left = left;
                                            
                                        }
                                        if (AppManager.Instance.reviewResultFontSize > 18)
                                            vrSelection.TextAlign = "left-middle++";

                                        vrSelection.Width = width;
                                        if ((double)num5 == (selected > 3 ? Math.Floor((double)selected / 2) : (double)selected))
                                        {
                                            top = num6;
                                        }
                                        vrSelection.Top = top;
                                        top += vrSelection.Height;
                                        if (top > this.container.Top + this.container.Height)
                                        {
                                            vrSelection.Visible = false;
                                        }
                                        if (!this.container.Controls.Contains(vrSelection))
                                        {
                                            this.container.Controls.Add(vrSelection);
                                        }
                                        num5++;
                                        top += 5;
                                    }
                                }
                            }
                            else
                            {
                                VrLabel vrLabel2 = new VrLabel()
                                {
                                    Height = 20,
                                    Width = 100,
                                    TextSize = AppManager.Instance.reviewResultFontSize,
                                    TextAlign = "center-middle",
                                    Text = "Preference",
                                    Left = left1,
                                    Top = top
                                };
                                top += 40;
                                if (!this.container.Controls.Contains(vrLabel2))
                                {
                                    this.container.Controls.Add(vrLabel2);
                                }
                                selected = contestDefinition1.Selected;
                                DataDefinition dataDefinition2 = null;
                                foreach (DataDefinition datum in contestDefinition1.Data)
                                {
                                    if ((!datum.WriteIn ? false : !datum.IsWritten))
                                    {
                                        dataDefinition2 = datum;
                                    }
                                }
                                if (dataDefinition2 != null)
                                {
                                    contestDefinition1.Data.Remove(dataDefinition2);
                                }
                                foreach (DataDefinition dataDefinition3 in
                                    from i in contestDefinition1.Data
                                    where i.Preference > 0
                                    select i)
                                {
                                    VrLabel vrLabel3 = new VrLabel()
                                    {
                                        Data = Convert.ToString(num),
                                        Height = 60,
                                        TextSize = AppManager.Instance.reviewResultFontSize,
                                        Text = (string)this.vModel.Vars[string.Concat(new object[] { "$b", this.vModel.Ballot.Id, "_c", contestDefinition1.Id, "_g", contestDefinition1.Group, "_d", dataDefinition3.Id })],
                                        TextAlign = "left-middle",
                                        BgColor = "#fff",
                                        BorderWidth = 2,
                                        Left = left,
                                        Width = width,
                                        Top = top
                                    };
                                    VrLabel vrLabel4 = new VrLabel()
                                    {
                                        Height = 60,
                                        Width = 100,
                                        TextSize = AppManager.Instance.reviewResultFontSize,
                                        TextAlign = "center-middle",
                                        Text = dataDefinition3.Preference.ToString(),
                                        Left = left1,
                                        Top = top
                                    };
                                    top += vrLabel3.Height;
                                    if (top > this.container.Top + this.container.Height)
                                    {
                                        vrLabel3.Visible = false;
                                        vrLabel4.Visible = false;
                                    }
                                    if (!this.container.Controls.Contains(vrLabel3))
                                    {
                                        this.container.Controls.Add(vrLabel3);
                                    }
                                    if (!this.container.Controls.Contains(vrLabel4))
                                    {
                                        this.container.Controls.Add(vrLabel4);
                                    }
                                    num5++;
                                    top += 5;
                                }
                            }
                        }
                        else
                        {
                            this.lblUnderVote.Visible = true;
                            vrSelection = new VrSelection();
                            if ((this.vModel.Ballot.ElectionType == ElectionTypes.ranking_choice ? false : contestDefinition1.Type != ContestTypes.V))
                            {
                                vrSelection.Data = Convert.ToString(num);
                            }
                            else
                            {
                                vrSelection.Data = "ranking_choice";
                            }
                            vrSelection.Height = 60;
                            vrSelection.TextSize = AppManager.Instance.reviewResultFontSize;
                            vrSelection.Text = (string)this.vModel.Vars["no_selection"];
                            vrSelection.TextAlign = "left-middle";
                            vrSelection.Width = width;
                            vrSelection.Left = left;
                            vrSelection.Top = top;
                            top += vrSelection.Height;
                            if (top > this.container.Top + this.container.Height)
                            {
                                vrSelection.Visible = false;
                            }
                            if (!this.container.Controls.Contains(vrSelection))
                            {
                                this.container.Controls.Add(vrSelection);
                            }
                            top += 20;
                        }
                    }
                    group = contestDefinition1.Group;
                    num++;
                    if ((num == this.vModel.Ballot.ContestsList.Count ? false : contestDefinition1.Propositions == null))
                    {
                        top += 20;
                    }
                }
                else
                {
                    num++;
                }
            }
            this._textsToSpeak = base.GetAllTexts(false, false, true);
            if (this.container.CreateScroll(this.vModel, top - this.container.Top))
            {
                ScreenObject item = this.vModel.Definition.ScreenObjects[this.vModel.Definition.ScreenObjects.Count - 2];
                ScreenObject screenObject = item;
                screenObject.Top = screenObject.Top - 25;
                if (item.ObjectState == ScreenObject.ScreenObjectState.ACTIVE)
                {
                    item.Text = (string)this.vModel.Vars["more"];
                }
                item.Tag = "Scroll up";
                ScreenObject item1 = this.vModel.Definition.ScreenObjects[this.vModel.Definition.ScreenObjects.Count - 1];
                if (item1.ObjectState == ScreenObject.ScreenObjectState.ACTIVE)
                {
                    item1.Text = (string)this.vModel.Vars["more"];
                }
                item1.Tag = "Scroll down";
                VrContainer vrContainer = this.container;
                vrContainer.Top = vrContainer.Top + item.Height;
                VrContainer height = this.container;
                height.Height = height.Height - (item.Height + item1.Height + 20);
                foreach (ScreenObject control in this.container.Controls)
                {
                    ScreenObject top1 = control;
                    top1.Top = top1.Top + item.Height;
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
            //base.CreateListOfCommands();
            base.AddSpeechToTextEngine();

            //Reading All Data to Form variable
            int iCount = 0;
            mListReviewDetail.Clear();
            mListReviewDetail.Add(this.vModel.Ballot.Board);
            mListReviewDetail.Add(this.vModel.Ballot.Name);
            mListReviewDetail.Add(this.vModel.Ballot.Title);
            mListReviewDetail.Add(this.vModel.Ballot.Address);
           
            mListReviewDetail.Add("Machine: "+AppManager.Configuration["System"]["Machine"]);
            mListReviewDetail.Add(" ");

            foreach (ScreenObject ctl in container.Controls)
            {
                iCount++;

                string txt = ctl.Text;
                if (txt != null)
                {
                    int ix = txt.IndexOf("  ");
                    if (ix > 0)
                    {
                        txt = ctl.Text.Insert(ix, " / ");

                        var words = txt.Split(' ');
                        txt = "";
                        foreach (string word in words)
                        {
                            txt += (word == " " || word == "" ? "" : word + " ");
                        }
                    }
                    mListReviewDetail.Add(txt);
                }
                

            }
        }
        private void setBallotTextForLocale()
        {
            if (AppManager.Instance.Session.CurrentLocale.Code != "en-US")
            {
                var tranBallot = Ballot.GetBallotTranslated(AppManager.Instance.Session.Ballot.Id, AppManager.Instance.Session.CurrentLocale.Id);
                if (tranBallot != null)
                {
                    try
                    {
                        if (this.lblBallotName != null)
                            this.lblBallotName.Text = tranBallot.TopHeading;
                        if (this.lblBallotTitle != null)
                            this.lblBallotTitle.Text = tranBallot.ElectionName;
                        if (this.lblBallotAddress != null)
                            this.lblBallotAddress.Text = tranBallot.Location;
                    }
                    catch (Exception ex)
                    {

                    }
                }
            }
        }
        private List<ContestDefinition> GetFitOrderContestList()
        {
            List<ContestDefinition> lstRet = new List<ContestDefinition>();

            DataTable tbPosition = DataManager.VotingContentDataInstance.GetDataV2(string.Format("SELECT contest_id,rvo_order_position FROM {0} ",
                                                                "race_view_options"
                                                                ));

            tbPosition.DefaultView.Sort = "rvo_order_position";
            DataTable tbStart = tbPosition.DefaultView.ToTable();

            for (int k = 0; k < tbStart.Rows.Count; k++)
            {
                for (int i = 0; i < vModel.Ballot.ContestsList.Count; i++)
                {
                    if (int.Parse(tbStart.Rows[k]["contest_id"].ToString()) == vModel.Ballot.ContestsList[i].Id)
                    {
                        lstRet.Add(vModel.Ballot.ContestsList[i]);
                        break;
                    }
                }
            }

            if (lstRet.Count < vModel.Ballot.ContestsList.Count)
            {
                lstRet = vModel.Ballot.ContestsList;
            }

            return lstRet;
        }

        private void TimerTick(object sender, EventArgs e)
        {
            TimerCount++;
            if (TimerCount > StartTick && vController.ScrObject != null)
                vController.ScrollContainer(vModel, vModel.GetView().Container, ScrollStepTypes.ReviewViewScrollStep,
                                            vController.ScrObject.Data, (string)vModel.Vars["more"]);
        }

        internal static void PrintReview()
        {
            linesprinted = 0;
            mListPrint_Final.Insert(0, "==PAPER AUDIT TRAIL==");
            PrintDocument pd = new PrintDocument();
            Margins margins = new Margins(10, 10, 10, 10);
            pd.DefaultPageSettings.Margins = margins;
            ////document.DefaultPageSettings.PaperSize = new PaperSize("Custom", document.DefaultPageSettings.Bounds.Width, nHeight);
            pd.PrintPage += new PrintPageEventHandler(pd_PrintPage);
            // Print the document.
            pd.Print();
            AppManager.Instance.reviewprinted = true;
            //System.Threading.Thread oThread = new System.Threading.Thread(document.Print);
            //oThread.Start();
            //pThread.Join();
        }

        private static void pd_PrintPage(object sender, PrintPageEventArgs e)
        {
            float linesPerPage = 0;

            float leftMargin = e.MarginBounds.Left;
            float topMargin = e.MarginBounds.Top;
            float fontheight = 0;
            String line = null;
            float yPos = topMargin;

            System.Drawing.Font printFont = new System.Drawing.Font("Arial", AppManager.Instance.reviewResultFontSize - 8);

            List<string> mListPrint_n = new List<string>();
            int inx_in = -1;
            Dictionary<int, List<string>> dictPrint = new Dictionary<int, List<string>>();

            if (linesprinted > 0)
                mListReviewDetail = mListPrint_Final;
            else
            {
                List<string> printedFormatList = new List<string>();
                foreach (var item in mListReviewDetail)
                {
                    var lines = WriteText(e.Graphics, item, printFont);
                    foreach (var nline in lines)
                    {
                        printedFormatList.Add(nline);
                    }
                }
                mListReviewDetail = mListPrint_Final = printedFormatList;
            }

            for (int i = 0; i < mListReviewDetail.Count; i++)
            {

                mListPrint_n.Add(mListReviewDetail[i]);
               
                fontheight = printFont.GetHeight(e.Graphics);
                yPos += fontheight;                
                // if last line position exceeds the visible area
                if (i + 1 < mListReviewDetail.Count)
                {
                    if (yPos + 10 + fontheight >= e.Graphics.VisibleClipBounds.Height)// && linesprinted < totallines)
                    {
                        yPos = topMargin;
                        inx_in++;
                        dictPrint.Add(inx_in, mListPrint_n);
                        mListPrint_n = new List<string>();
                    }
                }
                
            }
            if (mListPrint_n.Count > 0)
            {
                yPos = topMargin;
                inx_in++;
                dictPrint.Add(inx_in, mListPrint_n);
                mListPrint_n = new List<string>();
            }

           
            if (linesprinted > 0)
            {
                for (int d = 0; d < linesprinted; d++)
                {
                    dictPrint.Remove(dictPrint.Count - 1);
                }
            }

            mListReviewDetail = dictPrint[dictPrint.Count - 1];

            for (int i = 0; i < mListReviewDetail.Count; i++)
            {
                line = mListReviewDetail[i].Trim();

                int ix = line.IndexOf("  ");

                if (ix > 0)
                {
                    line = line.Insert(ix, " / ");

                    var words = line.Split(' ');
                    string txt = "";
                    foreach (string word in words)
                    {
                        txt += (word == " " || word == "" ? "" : word + " ");
                    }
                    line = txt;
                }

                
                e.Graphics.DrawString(line, printFont, System.Drawing.Brushes.Black, leftMargin, yPos, new System.Drawing.StringFormat());
                fontheight = printFont.GetHeight(e.Graphics);
                yPos += fontheight;
               
            }
            linesprinted += 1;
            if (inx_in + 1 > linesprinted)
            {
                e.HasMorePages = true;
                
            }
            else
            {
                e.HasMorePages = false;
                mListPrint_Final = new List<string>();
               
            }


        }

        public static List<string> WriteText( Graphics gr, string text,  Font font)
        {
            text = text.Replace("\n", " / ");
            string[] tmpText;
            List<string> nlist = new List<string>();
            string line = "";
            SizeF textSize = gr.MeasureString(text, font);
            float maxWidth = 400;

            int ix = text.IndexOf("  ");

            if (ix > 0)
            {
                line = text.Insert(ix, " / ");

                var words = line.Split(' ');
                string txt = "";
                foreach (string word in words)
                {
                    txt += (word == " " || word == "" ? "" : word + " ");
                }
                text = txt;
                line = "";
            }



            if (textSize.Width > maxWidth)
            {
                tmpText = text.Split(new char[] { ' ' });
                float lineW = 0f;

                for (int i = 0; i < tmpText.Length; i++)
                {
                    line += tmpText[i] + " ";
                    lineW = gr.MeasureString(line, font).Width;

                    float nextWordW = 0;

                    if (i < tmpText.Length - 1)
                        nextWordW = gr.MeasureString(tmpText[i + 1] + " ", font).Width;
                    else
                    {
                        
                        nlist.Add(line);
                        break;
                    }

                    if ((lineW + nextWordW) > maxWidth)
                    {
                        nlist.Add(line);
                        //top += font.Height;
                        line = "";
                    }
                }
            }
            else
            {

               
                nlist.Add(text);
            }

            return nlist;
        }


    }
}
