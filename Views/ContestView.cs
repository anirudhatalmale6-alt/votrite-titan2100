// Product:	VotRite
// Module:  ContestView.cs
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
using VotRite.MVC;
using VotRite.Models;
using VotRite.Controllers;
using VotRite.Definition;
using VotRite.Util;
using VotRiteBallotDataManager.AppCode;


namespace VotRite.Views
{
    class ContestView : VrScreen
    {
        private RaceViewOptions raceViewOptions = null;
        private ContestModel vModel { get { return (ContestModel)model; } set { model = value; } }
        private ContestController vController { get { return (ContestController)controller; } set { controller = value; } }
        private VrLabel lblBallotName;
        private VrLabel lblBallotTitle;
        private VrLabel lblBallotAddress;
        private VrLabel lblBallotDate;
        private VrLabel lblGroupName;
        private VrLabel lblContestName;
        private VrLabel lblContestTitle;
        private VrLabel lblChoicesLeft;
        private VrLabel lblContestTip;
        private VrLabel lblCount;
        private VrContainer container;
        private VrButton btnNext;
        private VrButton btnBack;
        private VrButton btnReview;
        private VrLabel lblMassHeader1;
        private VrLabel lblMassHeader2;

        private VrButton btnSettingTools;
        private VrButton btnCallHelp;

        public VrContainer Container { get { return container; } }

        private readonly int SCREEN_V_SPACE = 20;
        private const int selectionsSpace = 20;
        int leftPadding = 5;
        bool showInOneColumn = Convert.ToBoolean(AppManager.Configuration["Contest"]["ShowInOneColumn"]);
        int selectionMultiplier = 0;
        int selectionControlWidth = 0;
        private bool _hasScroll = false;

        private static int _textSize = 0;
        private Timer _timer;
        private const int StartTick = 5;
        protected internal int TimerCount { get; set; }
        public Timer GetTimer()
        {
            return _timer;
        }

        private List<string> _textsToSpeak;

        public ContestView(ContestModel m, ContestController c)
            : base(m, c)
        {
            vModel = m;
            //vModel.Definition.Background = "graphics\\initialBG_f1.png";
            vController = c;
            raceViewOptions = new RaceViewOptions(vModel.Contest.Id);
            _textSize = raceViewOptions.UseDefaultSettings ?
                Convert.ToInt32(AppManager.Configuration["Contest"]["RaceFontSize"]) : raceViewOptions.FontSize;

            Logger.Instance.Write(string.Format("text size = {0}, contest id = {1}, rvo ID = {2}, use default = {3}", _textSize, raceViewOptions.ContestId, raceViewOptions.RaceViewOptionsId, raceViewOptions.UseDefaultSettings));

            lblBallotName = (VrLabel)vModel.FindScreenObject("lbl_blt_name");
            lblBallotTitle = (VrLabel)vModel.FindScreenObject("lbl_blt_title");
            lblBallotAddress = (VrLabel)vModel.FindScreenObject("lbl_blt_address");
            lblBallotDate = (VrLabel)vModel.FindScreenObject("lbl_blt_date");
            lblGroupName = (VrLabel)vModel.FindScreenObject("lbl_group_name");
            lblContestName = (VrLabel)vModel.FindScreenObject("lbl_contest_name");
            lblContestTitle = (VrLabel)vModel.FindScreenObject("lbl_contest_title");
            lblChoicesLeft = (VrLabel)vModel.FindScreenObject("lbl_choices_left");
            lblContestTip = (VrLabel)vModel.FindScreenObject("lbl_contest_tip");
            lblCount = (VrLabel)vModel.FindScreenObject("lbl_contest_count");
            container = (VrContainer)vModel.FindScreenObject("ctr_contest");
            // container.BgColor = "#FFFFFF";
            btnNext = (VrButton)vModel.FindScreenObject("btn_next");
            btnBack = (VrButton)vModel.FindScreenObject("btn_back");
            btnReview = (VrButton)vModel.FindScreenObject("btn_review");

            btnSettingTools = (VrButton)vModel.FindScreenObject("btn_setting_tools");
            btnCallHelp = (VrButton)vModel.FindScreenObject("btn_callhelp");

            lblMassHeader1 = (VrLabel)vModel.FindScreenObject("lblMassHeader1");
            lblMassHeader2 = (VrLabel)vModel.FindScreenObject("lblMassHeader2");

            if (vModel.Contest.Type == ContestTypes.R)
            {
                lblMassHeader1.Height = 0;
                lblMassHeader2.Height = 0;
            }

            if (vModel.Contest.Type == ContestTypes.R && vModel.Contest.CandidatesList.Count < 19)
            {
                SCREEN_V_SPACE = 10;
            }

            if (!raceViewOptions.UseDefaultSettings) VrContainer.SetScrollStep(raceViewOptions.ScrollStep);

            Initialize();
            setBallotTextForLocale();

            lblBallotDate.Visible = true;
            lblBallotAddress.Visible = true;

            AppManager.Instance.checkDoubleSpaceSetting(vModel.Definition.ScreenObjects, container);

            if (vModel.Contest.Data[vModel.Contest.Data.Count - 1].WriteIn)
            {
                var txt = container.Controls[container.Controls.Count - 1].Text;//
                //if (container.Controls[container.Controls.Count - 1].Text != "Touch here to submit another name")
                //{
                //    var downScroll = vModel.Definition.ScreenObjects[vModel.Definition.ScreenObjects.Count - 1];
                //    var upScroll = vModel.Definition.ScreenObjects[vModel.Definition.ScreenObjects.Count - 2];
                //    var writeinObj = container.Controls[container.Controls.Count - 1];
                //    var selectionHeight = container.Controls[0].Height + 9; //9 is space between selection controls

                //    if (writeinObj.Top + selectionHeight - downScroll.Top >= 5)
                //    {
                //        var begintop = downScroll.Top;
                //        var visiblelimit = upScroll.Top + upScroll.Height + 10;
                //        for (int i = container.Controls.Count - 1; i >= 0; i--)
                //        {
                //            container.Controls[i].Top = begintop - selectionHeight;
                //            begintop -= selectionHeight;
                //            if (begintop > visiblelimit)
                //                container.Controls[i].Visible = true;
                //            //
                //        }
                //        vModel.UpdateObject(container);
                //    }
                //}
                    if (AppManager.Instance.backgroundTheme == AppManager.colorTheme.Blue || AppManager.Instance.backgroundTheme == AppManager.colorTheme.LightBlue)
                    {
                        container.Controls[container.Controls.Count - 1].ForeColor = "#000000";
                        vModel.UpdateObject(container);
                    }
                
            }


            lblCount.Text = lblCount.Text.Replace("  ", " ");
            int left = 0;

            left = lblBallotName.Left;
            lblBallotName.Left = left - 80;
            lblBallotName.Width = 940;

            left = lblBallotAddress.Left;
            lblBallotAddress.TextSize = 18;
            lblBallotAddress.Height += 5;
            lblBallotAddress.Resize(vModel.Scale);
            lblBallotAddress.Left = 900;

            if (lblBallotName.Text.Length > 50)
            {
                lblBallotName.TextSize = 20;
                lblBallotName.Resize(vModel.Scale);
            }
            lblBallotName.Height += 5;

            left = lblBallotTitle.Left;
            lblBallotTitle.Left = left - 80;
            lblBallotTitle.Width = 940;

            left = lblBallotDate.Left;
            lblBallotDate.TextSize = 18;
            lblBallotDate.Top += 10;
            lblBallotDate.Height += 5;
            lblBallotDate.Resize(vModel.Scale);
            lblBallotDate.Left = 900;

            if (lblBallotTitle.Text.Length > 50)
            {
                lblBallotTitle.TextSize = 20;
                lblBallotTitle.Resize(vModel.Scale);
            }
            lblBallotTitle.Height += 5;

            if (lblChoicesLeft.TextSize > 16)
            {
                lblChoicesLeft.Left -= 15;
                lblChoicesLeft.Width += 35;

                lblContestTip.Left -= 15;
                lblContestTip.Width += 35;

                lblContestTitle.Left -= 15;
                lblContestTitle.Width += 35;
                
            }

            int n = 0;
            foreach (var cntrlobj in container.Controls)
            {
                if (cntrlobj != null)
                {
                    if (cntrlobj.Text != null)
                    {
                        //cntrlobj.Text = cntrlobj.Text.Replace("\r\n", "");
                        //int ix = cntrlobj.Text.IndexOf("  ");
                        if (cntrlobj.Type == ScreenObject.ScreenObjectType.SELECTION)
                        {
                            //
                            //if (n > leftColumnWidth)
                            //    cntrlobj.Width = 700;
                            //else
                            if (!showInOneColumn)
                                cntrlobj.Width = (container.Width / 2);//  - (selectionsSpace / 2);                            
                            else
                                cntrlobj.Width = container.Width - selectionsSpace;// selectionWidth;
                            if (AppManager.Instance.backgroundTheme == AppManager.colorTheme.Contrast)
                                ((VrSelection)cntrlobj).StateIconName = "CommonFiles\\graphics\\checkmark_.jpg";

                           
                            vModel.UpdateObject(cntrlobj);
                            if (cntrlobj.TextSize == 16)
                                cntrlobj.Text = "  " + cntrlobj.Text;
                        }
                        n++;
                    }
                }

            }

            foreach (var text in _textsToSpeak.Where(text => text != null))
            {
                AppManager.Instance.StartSpeaker(text);
            }
            //AppManager.Instance.repeatInstruction = AppManager.repeatInstructionCommand.norepeat;
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _textSize = 0;
                _textsToSpeak.Clear();
                AppManager.Instance._speechWords.Clear();
                _timer.Dispose();
              

                if (lblBallotName != null) lblBallotName.Dispose();
                if (btnReview != null) btnReview.Dispose();
                if (btnSettingTools != null) btnSettingTools.Dispose();
                if (btnCallHelp != null) btnCallHelp.Dispose();
                
                if (btnBack != null) btnBack.Dispose();
                if (btnNext != null) btnNext.Dispose();
                if (lblBallotTitle != null) lblBallotTitle.Dispose();
                if (lblBallotAddress != null) lblBallotAddress.Dispose();
                if (lblBallotDate != null) lblBallotDate.Dispose();
                if (lblGroupName != null) lblGroupName.Dispose();
                if (lblContestName != null) lblContestName.Dispose();
                if (lblContestTitle != null) lblContestTitle.Dispose();
                if (lblChoicesLeft != null) lblChoicesLeft.Dispose();
                if (lblContestTip != null) lblContestTip.Dispose();
                if (lblCount != null) lblCount.Dispose();

                if (container != null) container.Dispose();
            }
            base.Dispose(disposing);
        }

        private void Initialize()
        {
            selectionMultiplier = showInOneColumn ? 2 : 1;

            if (AppManager.Instance.backgroundTheme != AppManager.colorTheme.White)
            {
                vModel.Definition.Background = AppManager.GetPathToCommonFile("graphics" + Global.Instance.SLASH + "initialBG_f1_" + AppManager.Instance.backgroundTheme + ".png");
            }
            if (lblChoicesLeft != null)
                lblChoicesLeft.BgColor = null;

            if (btnReview != null)
            {
                btnReview.Text = (string)vModel.Vars["review"];
                //btnReview.Height += 10;
                //btnReview.Height += 10;
                btnReview.Top -= 10;
            }
            if (btnSettingTools != null)
            {
                btnSettingTools.Text = (string)vModel.Vars["btn_setting_tools"];
                btnSettingTools.Visible = vModel.Ballot.ShowSettingsAndTools;
            }

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

            //if (btnCallHelp != null)
            //{
            //    btnCallHelp.Text = (string)vModel.Vars["btn_callhelp"];
            //}
            
            if (btnBack != null)
                btnBack.Text = (string)vModel.Vars["back"];
            if (btnNext != null)
                btnNext.Text = (string)vModel.Vars["next"];
            if (lblBallotName != null)
                lblBallotName.Text = (string)vModel.Vars["ballot_name"].ToString().ToUpper();
            if (lblBallotTitle != null)
                lblBallotTitle.Text = (string)vModel.Vars["ballot_title"].ToString().ToUpper();
            if ((string)vModel.Vars["ballot_address"] != "")
                lblBallotAddress.Text = (string)vModel.Vars["ballot_address"];
            if (lblBallotDate != null)
                lblBallotDate.Text = (string)vModel.Vars["ballot_date"];
            if (lblGroupName != null)
                lblGroupName.Text = (string)vModel.Vars["group_name"].ToString().ToUpper();
            if (lblContestName != null)
                lblContestName.Text = (string)vModel.Vars["contest_name"].ToString().ToUpper();
            if (lblMassHeader1 != null)
                lblMassHeader1.Text = (string)vModel.Vars["lblMassHeader1"];
            if (lblMassHeader2 != null)
                lblMassHeader2.Text = (string)vModel.Vars["lblMassHeader2"];//.ToString().ToUpper();

            if (lblContestTitle != null)
            {
                lblContestTitle.TextAlign = "left-top";
                if (vModel.Contest.MinSelection > 0 && !string.IsNullOrEmpty((string)vModel.Vars["min_race_tip"]))
                {
                    lblContestTitle.Text = string.Format("{0} ({1}). ", vModel.Vars["min_race_tip"], vModel.Contest.MinSelection);
                }
                else
                {
                    lblContestTitle.Text = string.Empty;
                }
                lblContestTitle.Text += string.IsNullOrEmpty((string)vModel.Vars["contest_title"]) ?
                                                string.Empty :
                                                string.Format("{0} ({1}).", vModel.Vars["contest_title"], vModel.Contest.MaxSelection);
                //lblContestTitle.ForeColor = "#F90914";
                // lblContestTitle.Text = lblContestTitle.Text.ToUpper();
                lblContestTitle.Left = lblGroupName.Left;
                lblContestTitle.Top += 40;
            }

            UpdateLblChoicesLeft(false);

            if (lblContestTip != null)
                lblContestTip.Text = (string)vModel.Vars["contest_tip"].ToString().Replace("touch","choose");
            AppManager.Instance.ChangeTextSize(lblContestTip);

            if (lblCount != null)
                lblCount.Text = (vModel.Ballot.ActiveContest + 1) +
                    " " + vModel.Vars["contest_count"].ToString() +
                    " " + vModel.Ballot.ContestsList.Count;
            lblCount.ForeColor = "#000";

            int start = 0;
            int end = 0;

            if (vModel.Contest.Type == ContestTypes.P)
            {
                lblContestTitle.Height = 140;
            }

            if (vModel.Contest.ActivePage == 0)
                ++vModel.Contest.ActivePage;

            if (vModel.Contest.ActivePage == 1)
            {
                start = 0;
                end = vModel.Ballot.ContestPageSize;
            }
            else
            {
                start = (vModel.Contest.ActivePage *
                    vModel.Ballot.ContestPageSize) -
                    vModel.Ballot.ContestPageSize;
                end = start + vModel.Ballot.ContestPageSize;

            }
            if (end > vModel.Contest.Data.Count)
                end = vModel.Contest.Data.Count;


            if (!raceViewOptions.UseDefaultContestSizeSettings)
            {
                container.Top = raceViewOptions.ContestTop;
                container.Left = raceViewOptions.ContestLeft;
                container.Width = raceViewOptions.ContestWidth;
                container.Height = raceViewOptions.ContestHeight;
            }

            ScreenObject screenObj=  new VrSelection();
            int lowestPosition = container.Top;
            int n = 0;
            int topHist = container.Top + 5;

            if (vModel.Contest.Data.Count < 4)
            {
                topHist = container.Top + 100;
            }

            int startTopHist = topHist;
            int selHeight = raceViewOptions.UseDefaultSettings ? 60 : raceViewOptions.RowHeight;
            selHeight = selHeight + 17;


            int selectionWidth = (container.Width - leftPadding - selectionsSpace) / 2;
            int leftColumnSelectionsPosition = container.Left + leftPadding;
            int rightColumnSelectionsPosition = container.Left + leftPadding + selectionWidth * selectionMultiplier + selectionsSpace;
            int defaultColumnWidth = raceViewOptions.UseDefaultSettings ? 7 : raceViewOptions.RowsToScroll;
            int leftColumnWidth = -1;
            int rightColumnWidth = -1;
            int candidatesCount = Math.Min(end - start, vModel.Ballot.ContestPageSize);
            
            if (candidatesCount > defaultColumnWidth)
            {
                leftColumnWidth = (int)((float)candidatesCount / (float)2);
                rightColumnWidth = candidatesCount - leftColumnWidth;
                if (rightColumnWidth > leftColumnWidth)
                {
                    leftColumnWidth++;
                    rightColumnWidth--;
                }
            }
            else leftColumnWidth = defaultColumnWidth;

            if (showInOneColumn)
            {
                leftColumnWidth = candidatesCount;
                rightColumnWidth = 0;
            }

            int? numberOfColumns = null;

            if (vModel.Contest.Propositions != null && vModel.Contest.Propositions.Count > 0)
            {
                leftColumnWidth = 1;
                rightColumnWidth = 1;
                end = vModel.Contest.Data.Count;
                numberOfColumns = 4;
            }

            var multiplier = 2;

            if (numberOfColumns != null)
            {
                multiplier = Convert.ToInt16(AppManager.Configuration["Contest"]["MassPropositionPerPage"]);
            }

            var selWidth = 0;
            var rowsInColumn = 0;
            var maxHeight = 0;

            if (!showInOneColumn && !raceViewOptions.UseDefaultSettings)
            {
                rowsInColumn = (vModel.Contest.Data.Count / raceViewOptions.ColumnCount);
                rowsInColumn += ((vModel.Contest.Data.Count % raceViewOptions.ColumnCount) == 0) ? 0 : 1;
                selWidth = (container.Width / raceViewOptions.ColumnCount) - selectionsSpace;
            }

            foreach (DataDefinition data in vModel.Contest.Data)
            {
                if ((n >= start) && (n < end) || ((start == 0) && (end == 0)))
                {
                    if (data.ReadOnly)
                    {
                        screenObj = new VrLabel();
                        screenObj.TextAlign = "left-top";
                        screenObj.TextSize = 16;
                        screenObj.Height = container.Height -
                                               ((selHeight * multiplier) +
                                                ((SCREEN_V_SPACE * 3) * multiplier)) - 70;
                        (screenObj as VrLabel).Speakable = true;
                    }
                    else
                    {
                        screenObj = new VrSelection();
                        ((VrSelection)screenObj).Name = "sel_" + n;
                        ((VrSelection)screenObj).State = data.State;
                        ((VrSelection)screenObj).Group = data.Group;
                        ((VrSelection)screenObj).Photo = data.Photo;
                        ((VrSelection)screenObj).PartyLogo = data.PartyLogo;
                        ((VrSelection)screenObj).ButtonGroup = data.ButtonGroup;

                        ((VrSelection)screenObj).Enabled = data.Voteable;

                        if (data.WriteIn)
                            ((VrSelection)screenObj).Data = "writein";

                        screenObj.Height = selHeight;

                        if (data.Photo != null)
                        {
                            ((VrSelection)screenObj).Height += 8;
                        }

                        if (AppManager.Configuration["Contest"]["ImageSize"].ToLower() == "l")
                        {
                            ((VrSelection)screenObj).Height += 40;
                        }
                        if (AppManager.Configuration["Contest"]["ImageSize"].ToLower() == "s")
                        {
                            ((VrSelection)screenObj).Height -= 20;
                        }

                        screenObj.TextAlign = "left-middle";
                    }

                    screenObj.Tag = data.Text;
                    screenObj.Text = (string)vModel.Vars["contest_data_" +
                                                        Convert.ToString(n + 1)];
                    
                    if ((vModel.Contest.Type == ContestTypes.R) || (vModel.Contest.Type == ContestTypes.P))
                    {
                        _textSize = raceViewOptions.UseDefaultSettings ? 
                            Convert.ToInt32(AppManager.Configuration["Contest"]["RaceFontSize"]) : raceViewOptions.FontSize;
                    }

                    if (vModel.Contest.Type == ContestTypes.M)
                    {
                        _textSize = raceViewOptions.UseDefaultSettings ? 
                            Convert.ToInt32(AppManager.Configuration["Contest"]["MassPropositionFontSize"]) : raceViewOptions.FontSize;
                        _textSize = 11;
                        string currLocale = AppManager.Instance.GetLocale();
                        string currLocaleCode = (!currLocale.Contains("-") ? currLocale : currLocale.Substring(0, currLocale.IndexOf("-")));
                        //if (currLocale != "en-US")
                        //{
                        //    screenObj.Text = Global.Instance.TranslateText(new string[] { (string)screenObj.Text }, "en", currLocaleCode)[0];
                        //    screenObj.Tag = Global.Instance.TranslateText(new string[] { (string)screenObj.Tag }, "en", currLocaleCode)[0];
                        //}
                       // container.Top = container.Top - 6;
                    }

                    screenObj.TextSize = _textSize;
                   
                    if (!data.ReadOnly)
                    {
                        AppManager.Instance.ChangeTextSize(screenObj);
                        //if (screenObj.TextSize > 23 && screenObj.Text.Length > 28)
                        //    screenObj.TextSize = 23;
                        //if (screenObj.TextSize == 23 && screenObj.Text.Length > 38)
                        //    screenObj.TextSize = 20;
                        //if (screenObj.TextSize == 20 && screenObj.Text.Length > 41)
                        //    screenObj.TextSize = 16;
                    }

                    if (numberOfColumns == null)
                    {
                        if (raceViewOptions.UseDefaultSettings || showInOneColumn)
                        {
                            screenObj.Left = (n < leftColumnWidth) ? leftColumnSelectionsPosition : rightColumnSelectionsPosition;
                            screenObj.Width = (data.ReadOnly) ? container.Width - 5 : selectionWidth * selectionMultiplier;
                            if (n == leftColumnWidth) topHist = startTopHist;
                        }
                        else
                        {
                            screenObj.Left = (n < rowsInColumn) ? leftColumnSelectionsPosition : leftColumnSelectionsPosition + (selWidth + selectionsSpace) * (n / rowsInColumn);
                            screenObj.Width = selWidth;
                            if ((n % rowsInColumn) == 0)
                            {
                                if (vModel.Contest.Type != ContestTypes.P)
                                {
                                    topHist = startTopHist;
                                }
                                else
                                {
                                    if (n == vModel.Contest.Data.Count - 1)
                                    {
                                        topHist -= selHeight + 20;
                                    }
                                }
                            }
                        }
                    }
                    else
                    {
                        double colWidthFactor = 1.0 / Convert.ToDouble(numberOfColumns); //0.33
                        leftColumnSelectionsPosition = container.Left + leftPadding;
                        var leftMiddleColumnSelectionsPosition = container.Left + leftPadding + (int)(colWidthFactor * container.Width) - 5;//selectionsSpace * 2;
                        var rightMiddleColumnSelectionsPosition = leftMiddleColumnSelectionsPosition + (int)(colWidthFactor * container.Width) - 5;
                        rightColumnSelectionsPosition = container.Left + leftPadding + (int)(selectionWidth * 1.44) + selectionsSpace;

                        if (n % 5 == 0)
                        {
                            screenObj.Left = leftColumnSelectionsPosition;
                            screenObj.Width = (leftMiddleColumnSelectionsPosition - leftColumnSelectionsPosition - 5);// (int)(colWidthFactor * container.Width) ;//selectionsSpace * 2;
                            //                            screenObj.Height = (int)(1.5*screenObj.Height);
                        }
                        else if (n % 5 == 1)
                        {
                            screenObj.Left = leftMiddleColumnSelectionsPosition;
                            screenObj.Width = (rightMiddleColumnSelectionsPosition - leftColumnSelectionsPosition - 10);// (int)(colWidthFactor * container.Width);
                            //                            screenObj.Height = (int)(1.5 * screenObj.Height);
                        }
                        else if (n % 5 == 2)
                        {
                            screenObj.Left = rightMiddleColumnSelectionsPosition;
                            screenObj.Width = (int)(colWidthFactor * container.Width) - 10;
                            //                            screenObj.Height = (int)(1.5 * screenObj.Height);
                        }
                        else
                        {
                            screenObj.Left = rightColumnSelectionsPosition;
                            /*if (data.ReadOnly)
                                screenObj.Width = (int)(0.67 * container.Width) - 5;
                            else
                            {
                               */
                            screenObj.Width = container.Width - rightColumnSelectionsPosition - 10;// (int)(0.6 * container.Width - leftPadding - selectionsSpace) / 2;
                            topHist = startTopHist + (n % 5 == 3 ? 0 : screenObj.Height + SCREEN_V_SPACE);
                            //                            }
                            screenObj.Tag = data.Tag;
                        }
                        if (n > 0 && n % 5 == 0)
                        {
                            //                            startTopHist = topHist;
                            startTopHist += container.Height -
                                      ((selHeight * multiplier) +
                                       ((SCREEN_V_SPACE * 3) * multiplier)) - 50;
                        }

                        if (n % 5 < 3)
                            topHist = startTopHist;
                    }

                    screenObj.Top = topHist;

                    if (topHist > container.Top + container.Height)
                    {
                        // screenObj.Visible = false;
                        if (!_hasScroll) _hasScroll = true;
                    }

                    if (!container.Controls.Contains(screenObj)) {

                        if (data.WriteIn && AppManager.Instance.Session.BallotMode == Session.BallotModes.Audio)
                        {

                        }
                        else
                        {
                            if (screenObj.TextSize > 23 && screenObj.Text.Length > 28)
                                screenObj.TextSize = 23;
                            if (screenObj.TextSize == 23 && screenObj.Text.Length > 38)
                                screenObj.TextSize = 20;
                            if (screenObj.TextSize == 20 && screenObj.Text.Length > 41)
                                screenObj.TextSize = 16;

                            selectionControlWidth = screenObj.Width;
                            if (!showInOneColumn)
                            {
                                if (vModel.Contest.Type != ContestTypes.M)
                                {
                                    screenObj.Width = (container.Width / 2);//  - (selectionsSpace / 2);
                                    selectionControlWidth = screenObj.Width;
                                }
                                else
                                {
                                    if (n % 5 == 1)
                                        screenObj.TextSize += 3;
                                }
                                //if (screenObj.TextSize > 23 && screenObj.Text.Length >= 1 && screenObj.Text.Length <= 14)
                                //    screenObj.TextSize = 23;
                                //if (screenObj.TextSize == 23 && screenObj.Text.Length >= 15 && screenObj.Text.Length <= 19)
                                //    screenObj.TextSize = 20;
                                //if (screenObj.TextSize == 20 && screenObj.Text.Length >= 20 )
                                //    screenObj.TextSize = 16;
                            }

                            container.Controls.Add(screenObj);
                        }
                    }

                    if (data.ReadOnly)
                        topHist = screenObj.Top + screenObj.Height +
                            SCREEN_V_SPACE * 3;
                    else
                        topHist = screenObj.Top + screenObj.Height +
                            SCREEN_V_SPACE;

                    //if (n == leftColumnWidth - 1)
                    lowestPosition = Math.Max(lowestPosition, topHist);

                    if (!screenObj.Resized) screenObj.Resize(vModel.Scale);
                }
                maxHeight = Math.Max(maxHeight, topHist);
                n++;
            }


            SetNavigation();

            string footnote = (string)vModel.Vars["contest_footnote"];
            if (!string.IsNullOrEmpty(footnote) && !string.IsNullOrEmpty(footnote.Trim()))
            {
                VrLabel lblFootnote = new VrLabel();
                lblFootnote.TextSize = 18;
                lblFootnote.TextAlign = "left-top";
                lblFootnote.Text = footnote;
                lblFootnote.Top = lowestPosition;
                lblFootnote.Left = container.Left + leftPadding;
                lblFootnote.Height = 70;//container.Height - (lowestPosition - container.Top);
                lblFootnote.Width = container.Width - leftPadding;
                lblFootnote.Speakable = false;
                container.Controls.Add(lblFootnote);
                topHist = lblFootnote.Top + lblFootnote.Height /*+ SCREEN_V_SPACE*/;
                if (!lblFootnote.Resized) lblFootnote.Resize(vModel.Scale);
            }

            VrSelection btnAddCandidates;
            var idx = 0;
            while ((btnAddCandidates = (VrSelection)container.FindControlByText("btn_add_candidates")) != null)
            {
                btnAddCandidates.Text = (string)vModel.Vars[Locale.BtnAddCandidatesField];
                if (idx++ > 0)
                    btnAddCandidates.Visible = false;
            }
            AppManager.Instance._contestSelections = new List<string>();
            //_textsToSpeak = GetAllTexts(true, false, false);            
            _textsToSpeak = GetAllTexts(true, false, false, true);            
            //_textsToSpeak.Add("say check to review your choices");
            //_textsToSpeak.Add("say zero to repeat instruction");
            AppManager.Instance._speechWords = _textsToSpeak;
            

            if (_hasScroll)
            //if (vModel.Contest.Data.Count > 18)
            {
                if (Convert.ToBoolean(AppManager.Configuration["Contest"]["scroll"]) && container.CreateScroll(vModel, maxHeight - container.Top))
                {
                    var upScroll = vModel.Definition.ScreenObjects[vModel.Definition.ScreenObjects.Count - 2];

                    if (upScroll.ObjectState == ScreenObject.ScreenObjectState.ACTIVE)
                        upScroll.Text = (string)vModel.Vars["more"];

                    upScroll.Tag = "Scroll up";

                    var downScroll = vModel.Definition.ScreenObjects[vModel.Definition.ScreenObjects.Count - 1];

                    if (downScroll.ObjectState == ScreenObject.ScreenObjectState.ACTIVE)
                        downScroll.Text = (string)vModel.Vars["more"];

                    downScroll.Tag = "Scroll down";

                    container.Top += upScroll.Height;
                    container.Height -= upScroll.Height;
                    container.Height -= downScroll.Height;

                    if (!container.Resized) container.Resize(vModel.Scale);

                    foreach (var screenObject in container.Controls)
                    {
                        screenObject.Top += upScroll.Height;
                        if (screenObject.Top + screenObject.Height > container.Top + container.Height)
                            screenObject.Visible = false;
                    }
                    _timer = new Timer { Interval = VrContainer.GetScrollTimerIn(ScrollTimerTypes.ContestViewScrollTimerIn) };
                    _timer.Stop();
                    _timer.Tick += TimerTick;
                }
            }

            foreach (var cntrl in this.vModel.Definition.ScreenObjects)
            {
                if (cntrl != null)
                {
                    if (cntrl is VrContainer)
                    {
                        //cntrl.Resize(vModel.Scale);
                        continue;
                    }
                    if (cntrl != null)
                    {
                        cntrl.TextSize = raceViewOptions.UseDefaultSettings ?
                            Convert.ToInt32(AppManager.Configuration["Contest"]["RaceFontSize"]) : raceViewOptions.FontSize;// screenObj.TextSize;
                        AppManager.Instance.ChangeTextSize(cntrl);
                        if (cntrl == lblChoicesLeft)
                        {
                            lblChoicesLeft.Top += (cntrl.TextSize - 18) * 12;
                            lblChoicesLeft.Width = lblContestTip.Width;
                            if (AppManager.Instance.isDoubleSpacing)
                            {
                                lblChoicesLeft.Height += 50;
                            }
                        }
                        if (cntrl == lblContestTip)
                        {
                            lblContestTip.Top += (cntrl.TextSize - 18) * 15;
                            lblContestTip.Height += (cntrl.TextSize - 18);
                            if (AppManager.Instance.isDoubleSpacing)
                                lblContestTip.Height += 50;
                        }
                        if (cntrl == lblContestTitle)
                        {
                            lblContestTitle.Top += (cntrl.TextSize - 18) * 7;
                            lblContestTitle.Height += lblContestTitle.Height + 50;// (cntrl.TextSize - 18);
                            lblContestTitle.Width = 400;
                            //if (cntrl.Text.Length > 30)
                            //    cntrl.Text = cntrl.Text.Substring(0, 45) + Environment.NewLine + cntrl.Text.Substring(46, cntrl.Text.Length - 45 - 1);
                        }
                        if (cntrl == lblContestName)
                        {
                            lblContestName.Top -= 75;
                            lblContestName.Height += lblContestName.Height + 75;// (cntrl.TextSize - 18);
                            //lblContestName.Width = 400;
                            //if (cntrl.Text.Length > 30)
                            //    cntrl.Text = cntrl.Text.Substring(0, 45) + Environment.NewLine + cntrl.Text.Substring(46, cntrl.Text.Length - 45 - 1);
                        }
                        if (cntrl == lblCount)
                        {
                            lblCount.Left -= 20;
                            lblCount.Width += 20;
                        }
                        if (cntrl == btnReview)
                        {
                            btnReview.Width += (cntrl.TextSize - 18) * 5;
                            btnReview.Height += (cntrl.TextSize - 18);
                        }
                        if (cntrl == lblGroupName)
                        {
                            //if (cntrl.Text.Length > 45)
                            // cntrl.Text = cntrl.Text.Substring(0, 45) + Environment.NewLine + cntrl.Text.Substring(46, cntrl.Text.Length - 45 - 1);
                            cntrl.Top -= 50;
                            cntrl.Height += cntrl.Height + 20;// (cntrl.TextSize - 18);
                        }

                    }
                    cntrl.Resize(vModel.Scale);
                }
            }

            AppManager.Instance._commandList = new List<string[]>();
            var additionalSet = new HashSet<string> { (string)vModel.Vars["next"], (string)vModel.Vars["review"], (string)vModel.Vars["back"] };

            if(vModel.Contest.ActivePage == 1)
                additionalSet = new HashSet<string> { (string)vModel.Vars["next"], (string)vModel.Vars["review"] };

            CreateListOfCommands(additionalSet);
            //AddSpeechToTextEngine();
            AppManager.Instance.initiateHearingMode();
            AppManager.Instance.controllerForHearingMode = vController;
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
        private bool IsReviewVisible()
        {
            return !vModel.Ballot.ContestsList.Any(contest => contest.Selected < contest.MinSelection);
        }

        private void TimerTick(object sender, EventArgs e)
        {
            TimerCount++;
            if (TimerCount > StartTick && vController.ScrObject != null)
                vController.ScrollContainer(vModel, vModel.GetView().Container, ScrollStepTypes.ContestViewScrollStep,
                                            vController.ScrObject.Data, (string)vModel.Vars["more"]);
        }

        public override void Update(ISubject subj)
        {
            //Logger.Instance.Write("contest view updated");
        }

        public void SetSelection(VrSelection selection)
        {
            

            int selectionWidth = ((container.Width + selectionsSpace) / 2) * selectionMultiplier;// ((container.Width - leftPadding - selectionsSpace) / 2) * selectionMultiplier;
                                                                                                    // int controlWidth = (data.ReadOnly) ? container.Width - 5 : selectionWidth;
            int start = 0;
            int end = 0;
            if (vModel.Contest.ActivePage == 1)
            {
                start = 0;
                end = vModel.Ballot.ContestPageSize;
            }
            else
            {
                start = (vModel.Contest.ActivePage *
                    vModel.Ballot.ContestPageSize) -
                    vModel.Ballot.ContestPageSize;
                end = start + vModel.Ballot.ContestPageSize;

            }
            if (end > vModel.Contest.Data.Count)
                end = vModel.Contest.Data.Count;
            int defaultColumnWidth = raceViewOptions.UseDefaultSettings ? 7 : raceViewOptions.RowsToScroll;
            int leftColumnWidth = -1;
            int rightColumnWidth = -1;
            int candidatesCount = Math.Min(end - start, vModel.Ballot.ContestPageSize);

            if (candidatesCount > defaultColumnWidth)
            {
                leftColumnWidth = (int)((float)candidatesCount / (float)2);
                rightColumnWidth = candidatesCount - leftColumnWidth;
                if (rightColumnWidth > leftColumnWidth)
                {
                    leftColumnWidth++;
                    rightColumnWidth--;
                }
            }

            if (selection.State == VrSelection.SelectionState.SELECTED)
            {
                selection.State = VrSelection.SelectionState.DESELECTED;
                vModel.Contest.Selected--;
                var groupSelection = 0;
                vModel.Contest.GroupSelection.TryGetValue(selection.ButtonGroup, out groupSelection);
                if (groupSelection > 0)
                    vModel.Contest.GroupSelection[selection.ButtonGroup] = --groupSelection;
            }
            else
            {
                if (vModel.Contest.MaxSelectionPerGroup <= 0)
                {
                    // https://votrite.fogbugz.com/f/cases/31/Voter-can-vote-for-one-candidate-as-many-times-as-he-is-listed-in-the-ballot-in-that-race
                    // make sure candidates with the same name are not being selected multiple times
                    bool sameCandidateMultipleSelection = false;
                    sameCandidateMultipleSelection = checkCandidateMultipleSelection(selection);

                    if (sameCandidateMultipleSelection)
                    {
                        // show error message
                        MessageBox.Show("Cannot select the same candidate more than once. Please make another selection.", "Error",
                                                      MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                    else
                    { 
                    if (vModel.Contest.Selected < vModel.Contest.MaxSelection)
                    {
                        selection.State = VrSelection.SelectionState.SELECTED;
                        vModel.Contest.Selected++;
                    }
                    else
                    {
                        if (vModel.Contest.MaxSelection == 1 && vModel.Contest.MaxSelectionPerGroup == 0)
                        {
                            foreach (DataDefinition data in vModel.Contest.Data)
                            {
                                if (data.State == VrSelection.SelectionState.SELECTED)
                                {
                                    data.State = VrSelection.SelectionState.DESELECTED;
                                    vModel.Contest.Selected--;

                                    //if (container.Controls[vModel.Contest.LastSelectedIndex].Type == ScreenObject.ScreenObjectType.SELECTION)
                                    //{
                                    VrSelection prevSel =
                                        (VrSelection)container.Controls[vModel.Contest.LastSelectedIndex];

                                        if (prevSel != null)
                                        {
                                            prevSel.State = VrSelection.SelectionState.DESELECTED;

                                            //prevSel.Width = selectionWidth;
                                            vModel.UpdateObject(prevSel);
                                        }
                                    //}
                                    break;
                                }
                            }

                            selection.State = VrSelection.SelectionState.SELECTED;
                            vModel.Contest.Selected++;
                        }
                    }
                    }
                }
                else
                {
                    var groupSelection = 0;
                    vModel.Contest.GroupSelection.TryGetValue(selection.ButtonGroup, out groupSelection);

                    if (groupSelection < vModel.Contest.MaxSelectionPerGroup)
                    {
                        selection.State = VrSelection.SelectionState.SELECTED;
                        vModel.Contest.Selected++;
                        vModel.Contest.GroupSelection[selection.ButtonGroup] = ++groupSelection;
                    }
                    else
                    {
                        if (vModel.Contest.MaxSelectionPerGroup == 1)
                        {
                            foreach (
                                var selectionControl in
                                    container.Controls.OfType<VrSelection>().Select(control => control as VrSelection).
                                        Where(
                                            selectionControl =>
                                            selectionControl.State == VrSelection.SelectionState.SELECTED &&
                                            selectionControl.ButtonGroup == selection.ButtonGroup))
                            {
                                selectionControl.State = VrSelection.SelectionState.DESELECTED;
                                vModel.Contest.Selected--;
                                vModel.Contest.GroupSelection[selection.ButtonGroup] = --groupSelection;
                                int idx;
                                if (Int32.TryParse(selectionControl.Name.Replace("sel_", ""), out idx) && idx >= 0 && idx < vModel.Contest.Data.Count)
                                    vModel.Contest.Data[idx].State = selectionControl.State;
                                else
                                    vModel.Contest.FindDataByTag(selectionControl.Tag).State = selectionControl.State;
                                vModel.UpdateObject(selectionControl);
                                break;
                            }

                            selection.State = VrSelection.SelectionState.SELECTED;
                            vModel.Contest.Selected++;
                            vModel.Contest.GroupSelection[selection.ButtonGroup] = ++groupSelection;
                        }
                    }
                }
            }

            DataDefinition dataDefinition;
            int index;
            if (Int32.TryParse(selection.Name.Replace("sel_", ""), out index) && index >= 0 && index < vModel.Contest.Data.Count)
            {
                dataDefinition = vModel.Contest.Data[index];
            }
            else
            {
                index = -1;
                dataDefinition = vModel.Contest.FindData(selection.Tag) ?? vModel.Contest.FindDataByTag(selection.Tag);
            }
            if (dataDefinition != null)
                dataDefinition.State = selection.State;

            if (index < 0)
                index = vModel.Contest.FindDataIndex(selection.Tag);
            if (index < 0)
                index = vModel.Contest.FindDataIndexByTag(selection.Tag);

            vModel.Contest.LastSelectedIndex = index;

            UpdateLblChoicesLeft(true);

            if ((selection.Data == "writein") &&
                (selection.State == VrSelection.SelectionState.SELECTED))
                AppManager.Instance.SetScreen("writein");
            else
            {
                if (btnNext != null)
                {
                    SetNavigation();                    

                    if (btnNext.Visible)
                        vModel.UpdateObject(btnNext);
                    else
                    {
                        vModel.InvalidateObject(btnNext);
                    }
                }
                if (btnReview != null)
                {
                    SetNavigation();

                    if (btnReview.Visible)
                        vModel.UpdateObject(btnReview);
                    else
                    {
                        vModel.InvalidateObject(btnReview);
                    }
                }
                //selection.Width = selectionWidth;
                vModel.UpdateObject(selection);
            }

            
        }

        private bool checkCandidateMultipleSelection(VrSelection selection)
        {
            if(selection.Tag != "btn_add_candidates")
            {
                string candidateName = selection.Tag;
                var candidateNameCount = (from c in vModel.Contest.Data
                                          where c.Tag != null 
                                          && c.Tag.ToUpper() == candidateName.ToUpper()
                                          && c.State == VrSelection.SelectionState.SELECTED
                                          select c).Count();

                if(candidateNameCount >= 1)
                {
                    // this candidate is already selected
                    // multiple selection of the same candidate is not allowed
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return false;
            }
        }

        private void UpdateLblChoicesLeft(bool updateObject)
        {
            if (lblChoicesLeft != null)
            {
                int choicesLeft = vModel.Contest.MaxSelection -
                    vModel.Contest.Selected;
                //choicesleft_selection(choicesLeft);
                AppManager.Instance.choicesLeft = choicesLeft;
                string choicesText;
                if (choicesLeft != 1)
                {
                    choicesText = string.Format("{0} {1} {2}.", vModel.Vars["lbl_choice1"], Convert.ToString(choicesLeft), vModel.Vars["lbl_multi_0_choices2"]);
                }
                else
                {
                    choicesText = string.Format("{0} {1} {2}.", vModel.Vars["lbl_choice1"], Convert.ToString(choicesLeft), vModel.Vars["lbl_choice2"]);
                }
                lblChoicesLeft.Text = choicesText;
                //lblChoicesLeft.BgColor = "#FFFFFF";
                //lblChoicesLeft.ForeColor = "#347409";
                if (updateObject)
                {
                    //if (choicesLeft <= 0)
                    //    AppManager.Instance.StartSpeaker(choicesText + ". Say " + vModel.Vars["next"] + " to continue");
                    //else
                        AppManager.Instance.StartSpeaker(choicesText);

                    var currLbl = lblChoicesLeft;
                    vModel.InvalidateObject(lblChoicesLeft);
                    lblChoicesLeft = (VrLabel)vModel.FindScreenObject("lbl_choices_left");
                    if (lblChoicesLeft != null)
                    {
                        lblChoicesLeft.Left = currLbl.Left;
                        lblChoicesLeft.Top = currLbl.Top;
                        lblChoicesLeft.Height = currLbl.Height;
                        lblChoicesLeft.Width = currLbl.Width;
                        lblChoicesLeft.BgColor = null;
                        lblChoicesLeft.Text = choicesText;
                    }
                }
            }
        }

        private void SetNavigation()
        {
            /// Jim Kapsis 18-06-2016
            if (vModel.Ballot.ActiveContest < 1) btnBack.Visible = false;
            if (vModel.Contest.MinSelection == 0) { btnNext.Text = (string)vModel.Vars["skip"]; }
            
            btnReview.Visible = IsReviewVisible();

            /// if race count = 1
            if (vModel.Ballot.ContestsList.Count == 1)
            {
                btnNext.Visible = false;                
                btnReview.Text = (string)vModel.Vars["next"];
                return;
            }
            //if (vModel.Ballot.ContestsList.Count < 2) btnNext.Visible = false;
            //btnNext.Visible = vModel.Contest.Selected >= vModel.Contest.MinSelection;
            /*if (vModel.Ballot.ContestsList.Count < 2)
            {
                btnNext.Visible = false;
                return;
            }*/
            
            btnNext.Visible = vModel.Contest.Selected >= vModel.Contest.MinSelection;

            if (vModel.Ballot.ActiveContest > -1)
            {
                if (vModel.Contest.Selected > 0)
                {
                    btnNext.Text = (string)vModel.Vars["next"];
                    btnNext.Tag = (string)vModel.Vars["skip"];
                    btnNext.BgImage = btnNext.BgImage.Replace("_d", "_a");
                }
                else
                {
                    btnNext.Text = (string)vModel.Vars["skip"];
                    btnNext.Tag = (string)vModel.Vars["next"];
                    btnNext.BgImage = btnNext.BgImage.Replace("_a", "_d");
                }
            }
        }


        //private VotRite.Controllers.ReviewController vController { get { return controller as VotRite.Controllers.ReviewController; } }
        
    }
}
