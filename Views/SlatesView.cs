using System;
using System.Collections.Generic;
using System.Linq;
using VotRite.UI;
using VotRite.Controllers;
using VotRite.Models;
using System.Windows.Forms;
using VotRite.MVC;

namespace VotRite.Views
{
    class SlatesView : VrScreen
    {
        private SlatesModel vModel { get { return model as SlatesModel; } }
        private SlatesController vController { get { return controller as SlatesController; } }
        private VrLabel lblBallotName;
        private VrLabel lblBallotTitle;
        private VrLabel lblBallotAddress;
        private VrLabel lblBallotDate;
        private VrLabel lblGroupName;
        private VrLabel lblContestName;
        private VrLabel lblContestTitle;
        private VrLabel lblChoicesLeft;
        private VrLabel lblSlateTip;
        private VrLabel lblCount;
        private VrContainer container;
        private VrContainer buttonsContainer;
        private VrButton btnBack;
        private VrButton btnNext;
        private VrButton btnReview;

        public VrContainer Container { get { return container; } }

        private const int SCREEN_V_SPACE = 20;
        private const int selectionsSpace = 20;

        private static int _textSize = 12;
        private Timer _timer;
        private const int StartTick = 5;
        protected internal int TimerCount { get; set; }
        public Timer GetTimer()
        {
            return _timer;
        }

        private List<string> _textsToSpeak;

        public SlatesView(SlatesModel m, SlatesController c)
            : base(m, c)
        {
            model = m;
            controller = c;

            lblBallotName = (VrLabel)vModel.FindScreenObject("lbl_blt_name");
            lblBallotTitle = (VrLabel)vModel.FindScreenObject("lbl_blt_title");
            lblBallotAddress = (VrLabel)vModel.FindScreenObject("lbl_blt_address");
            lblBallotDate = (VrLabel)vModel.FindScreenObject("lbl_blt_date");
            lblGroupName = (VrLabel)vModel.FindScreenObject("lbl_group_name");
            lblContestName = (VrLabel)vModel.FindScreenObject("lbl_contest_name");
            lblContestTitle = (VrLabel)vModel.FindScreenObject("lbl_contest_title");
            lblChoicesLeft = (VrLabel)vModel.FindScreenObject("lbl_choices_left");
            lblSlateTip = (VrLabel)vModel.FindScreenObject("lbl_contest_tip");
            lblCount = (VrLabel)vModel.FindScreenObject("lbl_contest_count");
            container = (VrContainer)vModel.FindScreenObject("ctr_contest");
            buttonsContainer = (VrContainer)vModel.FindScreenObject("ctr_buttons");
            btnBack = (VrButton)vModel.FindScreenObject("btn_back");
            btnNext = (VrButton)vModel.FindScreenObject("btn_next");
            btnReview = (VrButton)vModel.FindScreenObject("btn_review");

            Initialize();
            setBallotTextForLocale();

            AppManager.Instance.checkDoubleSpaceSetting(vModel.Definition.ScreenObjects, container);

            foreach (var cntrl in this.vModel.Definition.ScreenObjects)

            {
                if (cntrl != null)
                {
                    AppManager.Instance.ChangeTextSize(cntrl);

                    if (cntrl == lblBallotName)
                    {
                        lblBallotName.Top += (cntrl.TextSize - 18);
                        lblBallotName.Height += (cntrl.TextSize - 18) * 2;
                    }
                    if (cntrl == lblBallotTitle)
                    {
                        lblBallotTitle.Top += (cntrl.TextSize - 18) * 2;
                        lblBallotTitle.Height += (cntrl.TextSize - 18) * 2;
                        //if (AppManager.Instance.isDoubleSpacing)
                        //	lblContestTip.Height += 50;
                    }
                    if (cntrl == lblBallotAddress)
                    {
                        lblBallotAddress.Top += (cntrl.TextSize - 18);
                        lblBallotAddress.Height += (cntrl.TextSize - 18) * 2;
                    }
                    if (cntrl == lblBallotDate)
                    {
                        lblBallotDate.Top += (cntrl.TextSize - 18) * 2;
                        lblBallotDate.Height += (cntrl.TextSize - 18) * 2;
                        lblBallotDate.Visible = false;
                    }

                    if (cntrl == lblContestTitle)
                    {
                        lblContestTitle.Top -= (cntrl.TextSize - 18);
                        lblContestTitle.Height += (cntrl.TextSize - 18) * 2;
                        if (lblContestTitle.TextSize > 18)
                            lblContestTitle.TextSize = 22;
                    }

                    if (cntrl == lblSlateTip)
                    {
                        lblSlateTip.Left -= 30;
                        lblSlateTip.Height += (cntrl.TextSize - 18) * 15;
                        lblSlateTip.Width += 60;
                        lblSlateTip.TextSize -= 2;
                        //if (AppManager.Instance.isDoubleSpacing)
                        //	lblContestTip.Height += 50;
                        //AppManager.Instance.ChangeTextSize(lblSlateTip);
                    }

                    if (cntrl == lblChoicesLeft)
                    {
                        lblChoicesLeft.Top -= (cntrl.TextSize - 18)*4;
                        lblChoicesLeft.Height += (cntrl.TextSize - 18) * 4;
                        lblChoicesLeft.Width += 80 ;
                        lblChoicesLeft.Left -= 30;
                    }
                    if (cntrl == btnReview)
                    {
                        //btnReview.Top -= (cntrl.TextSize - 18) * 4;
                        btnReview.Height += (cntrl.TextSize - 18) * 2;
                        btnReview.Width -= (cntrl.TextSize - 18);
                    }
                    //cntrl.Resize(vModel.Scale);
                }
            }

            var additionalSet = new HashSet<string> { (string)vModel.Vars["next"], (string)vModel.Vars["skip"], (string)vModel.Vars["review"], (string)vModel.Vars["back"] };
            CreateListOfCommands(additionalSet);

            foreach (var text in _textsToSpeak.Where(text => text != null))
            {
                AppManager.Instance.StartSpeaker(text);
            }
            AppManager.Instance.initiateHearingMode();
            AppManager.Instance.controllerForHearingMode = vController;
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
                if (lblContestName != null) lblContestName.Dispose();
                if (lblContestTitle != null) lblContestTitle.Dispose();
                if (lblChoicesLeft != null) lblChoicesLeft.Dispose();
                if (lblSlateTip != null) lblSlateTip.Dispose();
                if (lblCount != null) lblCount.Dispose();
                if (btnBack != null) btnBack.Dispose();
                if (btnNext != null) btnNext.Dispose();
                if (btnReview != null) btnReview.Dispose();
                if (buttonsContainer != null) buttonsContainer.Dispose();
                if (container != null) container.Dispose();
            }
            base.Dispose(disposing);
        }

        private void Initialize()
        {
            if (AppManager.Instance.backgroundTheme != AppManager.colorTheme.White)
            {
                vModel.Definition.Background = AppManager.GetPathToCommonFile("graphics" + Global.Instance.SLASH + "initialBG_f1_" + AppManager.Instance.backgroundTheme + ".png");
                buttonsContainer.BgColor = null;
            }
            if (btnBack != null) {
                btnBack.Text = (string)vModel.Vars["back"];
                btnBack.Tag = (string)vModel.Vars["back"];
            }
            if (lblChoicesLeft != null)
                lblChoicesLeft.BgColor = null;
            if (btnReview != null)
            {
                btnReview.Text = (string)vModel.Vars["review"];
                btnReview.Top -= 10;
            }

            if (btnNext != null)
                btnNext.Tag = (string)vModel.Vars["next"];

            if (lblBallotName != null)
                lblBallotName.Text = (string)vModel.Vars["ballot_name"];
            if (lblBallotTitle != null)
                lblBallotTitle.Text = (string)vModel.Vars["ballot_title"];
            if (lblBallotAddress != null)
            {
                if ((string)vModel.Vars["ballot_address"] != "")
                    lblBallotAddress.Text = (string)vModel.Vars["ballot_address"];
            }
            if (lblBallotDate != null)
                lblBallotDate.Text = (string)vModel.Vars["ballot_date"];
            if (lblGroupName != null)
                lblGroupName.Text = "";
            if (lblContestName != null)
                lblContestName.Text = "";

            lblContestTitle.TextAlign = "left-top";

            UpdateLblChoicesLeft(false);

            if (lblSlateTip != null)
                lblSlateTip.Text = (string)vModel.Vars["slate_tip"];
            if (lblCount != null && vModel.Vars["contest_count"] != null)
                lblCount.Text = (vModel.Ballot.ActiveContest + 1) +
                    " " + vModel.Vars["contest_count"].ToString() +
                    " " + (vModel.Ballot.ContestsList.Count+1);

            lblCount.ForeColor = "#000";

            int topHist = container.Top + 30;
            int leftHist = container.Left + 10;
            int lblHeight = 30;
            int lblWidth = 175;
            int col = 0;
            int posLeft = 0;
            ScreenObject lbl_slate;
            ScreenObject screenObj;
            
            foreach (var slate in vModel.Ballot.slatesDefinition.Slates)
            {
                if (slate.Name != null && slate.Name.Trim() != "")
                {
                    lbl_slate = new VrLabel() { TextSize = _textSize };
                    lbl_slate.Width = lblWidth;
                    lbl_slate.Height = lblHeight;
                    lbl_slate.Top = topHist;
                    lbl_slate.Left = leftHist - 30;
                    lbl_slate.Text = vModel.Ballot.slatesDefinition.Slates[col].Name;
                    lbl_slate.BgColor = "#FFFFFF";

                    container.Controls.Add(lbl_slate);

                    AppManager.Instance.ChangeTextSize(lbl_slate); //Adjustment for header selection objects
                    if (lbl_slate.TextSize > 18)
                    {//    screenObj.Height = lbl_slate.Height * 3;
                        lbl_slate.Height += (lbl_slate.TextSize - 18) * 10;
                        lbl_slate.Width -= (lbl_slate.TextSize - 18);
                        //lbl_slate.Top -= (lbl_slate.TextSize - 18) * 2;
                        lbl_slate.TextSize = 22;
                    }

                    if (!lbl_slate.Resized) lbl_slate.Resize(vModel.Scale);

                    // Add slate selection
                    screenObj = new VrSelection
                    {
                        Tag = slate.Id.ToString(),
                        Text = slate.Name,
                        Name = "sel_" + col,
                        Top = buttonsContainer.Top + 5,
                        Left = leftHist,
                        ObjectState = ScreenObject.ScreenObjectState.ACTIVE,
                        TextSize = _textSize
                    };
                    ((VrSelection)screenObj).Width = 175;
                    ((VrSelection)screenObj).Height = 60;
                    ((VrSelection)screenObj).PartyLogo = slate.SlateLogo;

                    if (slate.Id == vModel.Ballot.slatesDefinition.Data.SlateId)
                    {
                        ((VrSelection)screenObj).State = VrSelection.SelectionState.SELECTED;
                    }

                    buttonsContainer.Controls.Add(screenObj);

                    AppManager.Instance.ChangeTextSize(screenObj); //Adjustment for footer selection objects
                    if (screenObj.TextSize > 18) 
                    {//    screenObj.Height = lbl_slate.Height * 3;
                        screenObj.TextSize = 22;
                        screenObj.Height += (screenObj.TextSize - 18) * 2;
                        screenObj.Width += (screenObj.TextSize - 18) * 10;
                        screenObj.Top -= (screenObj.TextSize - 18) * 2;
                    }

                    if (!screenObj.Resized) screenObj.Resize(vModel.Scale);

                    leftHist += lblWidth ;
                    ++col;

                    if (col == vModel.Ballot.slatesDefinition.Slates.Count)
                    {
                        lbl_slate = new VrLabel() { TextSize = _textSize };
                        lbl_slate.Width = lblWidth;
                        lbl_slate.Height = lblHeight;
                        lbl_slate.Top = topHist;
                        lbl_slate.Left = posLeft = leftHist;
                        lbl_slate.Text = "Position";
                        lbl_slate.BgColor = "#FFFFFF";

                        container.Controls.Add(lbl_slate);
                        AppManager.Instance.ChangeTextSize(lbl_slate); //Position header
                        if (lbl_slate.TextSize > 18)
                        {
                            
                            lbl_slate.Height += (lbl_slate.TextSize - 18) * 5;
                            //lbl_slate.Width += (lbl_slate.TextSize - 18);
                            lbl_slate.Top += (lbl_slate.TextSize - 18) * 2;
                            lbl_slate.TextSize = 22;
                        }
                        if (!lbl_slate.Resized) lbl_slate.Resize(vModel.Scale);
                    }
                }
            }

            List<string> positions = new List<string>();

            foreach (var slate in vModel.Ballot.slatesDefinition.Slates)
            {
                if (slate.Name.Trim() == "")
                {
                    continue;
                }
                foreach (var race in slate.Candidates)
                {
                    if (race.Value != null)
                    {
                        foreach(var val in race.Value)
                        {
                            if (!positions.Contains(val.Position))
                            {
                                positions.Add(val.Position);
                            }
                        }
                    }
                }
            }

            topHist += lblHeight + 5;

            int raceTopHist = topHist;
            int slateTopHist = raceTopHist;
            int r = 0;
            int topposit = 0;
            foreach (string pos in positions)
            {
                int n = 0;

                leftHist = container.Left + 10;
                
                foreach (var slate in vModel.Ballot.slatesDefinition.Slates)
                {
                    if (slate.Name.Trim() == "")
                    {
                        continue;
                    }
                    foreach (var race in slate.Candidates)
                    {
                        if (race.Value != null)
                        {
                            if (race.Value.Count > 0)
                            {
                                slateTopHist = raceTopHist;
                            }
                            
                            foreach (var cand in race.Value)
                            {
                                if (cand.Position != pos)
                                {
                                    continue;
                                }

                                // Print candidate's name
                                lbl_slate = new VrLabel() { TextSize = _textSize };
                                lbl_slate.Width = lblWidth;
                                lbl_slate.Height = lblHeight;
                                lbl_slate.Top = slateTopHist + 5;
                                lbl_slate.Left = leftHist;
                                lbl_slate.Text = cand.Name;
                                lbl_slate.TextAlign = "left-middle";

                                container.Controls.Add(lbl_slate);

                                AppManager.Instance.ChangeTextSize(lbl_slate); //Slate Candidate Names Adjustment
                                if (lbl_slate.TextSize > 18)
                                {
                                    //
                                    lbl_slate.Height += (lbl_slate.TextSize - 18) * 5;
                                    lbl_slate.Width += (lbl_slate.TextSize - 18) ;
                                    lbl_slate.Top += ((lbl_slate.TextSize - 18) * 6);
                                    lbl_slate.TextSize = 22;
                                }

                                if (!lbl_slate.Resized) lbl_slate.Resize(vModel.Scale);


                                if (n == vModel.Ballot.slatesDefinition.Slates.Count - 1)
                                {
                                    // Print candidate's position
                                    lbl_slate = new VrLabel() { TextSize = _textSize };
                                    lbl_slate.Width = lblWidth;
                                    lbl_slate.Height = lblHeight;
                                    lbl_slate.Top = slateTopHist + 5;
                                    lbl_slate.Left = posLeft;
                                    lbl_slate.Text = pos;
                                    lbl_slate.TextAlign = "left-middle";

                                    container.Controls.Add(lbl_slate);

                                    AppManager.Instance.ChangeTextSize(lbl_slate); //Position Items Adjustment
                                    if (lbl_slate.TextSize > 18)
                                    {

                                        lbl_slate.Height += lbl_slate.Height ;
                                        lbl_slate.Width += (lbl_slate.TextSize - 18) * 6;
                                        if (r > 0)
                                            lbl_slate.Top = topposit + 20;
                                        else
                                            lbl_slate.Top += 70;
                                        r++;
                                        lbl_slate.TextSize = 20;
                                        topposit = lbl_slate.Top + lbl_slate.Height;
                                    }

                                    if (!lbl_slate.Resized) lbl_slate.Resize(vModel.Scale);
                                }

                                slateTopHist += lblHeight + 5;
                            }
                        }
                    }

                    leftHist += lblWidth + 10;
                    ++n;
                }

                if (slateTopHist > raceTopHist)
                {
                    raceTopHist = slateTopHist;
                } else if (slateTopHist == raceTopHist)
                {
                    raceTopHist = slateTopHist + 40;
                }
            }

            SetNavigation();

            btnNext.Visible = true;
            btnReview.Visible = IsReviewVisible();

            _textsToSpeak = GetAllTexts(true, false, false);
            //_textsToSpeak.Add("say check to review your choices");
            //_textsToSpeak.Add("say zero to repeat instruction");
            if (Convert.ToBoolean(AppManager.Configuration["Contest"]["scroll"]) && container.CreateScroll(vModel, topHist - container.Top))
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
           
            AddSpeechToTextEngine();
        }

        private void setBallotTextForLocale()
        {
            if (AppManager.Instance.Session.CurrentLocale.Code != "en-US")
            {
                var tranBallot = VotRiteBallotDataManager.AppCode.Ballot.GetBallotTranslated(AppManager.Instance.Session.Ballot.Id, AppManager.Instance.Session.CurrentLocale.Id);
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
            return vModel.Ballot.ContestsList.Any(contest => contest.Selected > 0) || vModel.Ballot.ContestsList[vModel.Ballot.ActiveContest].Selected > 0;
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
            if (selection.State == VrSelection.SelectionState.SELECTED)
            {
                selection.State = VrSelection.SelectionState.DESELECTED;

                vModel.Ballot.ContestsList[vModel.Ballot.ActiveContest].Selected = 0;

                vModel.Ballot.slatesDefinition.Data.Id = 0;
                vModel.Ballot.slatesDefinition.Data.SlateId = 0;
            }
            else
            {
                foreach (var item in buttonsContainer.Controls)
                {
                    if (item.Type == ScreenObject.ScreenObjectType.SELECTION)
                    {
                        ((VrSelection)item).State = VrSelection.SelectionState.DESELECTED;
                        vModel.UpdateObject(item);
                    }
                }
                selection.State = VrSelection.SelectionState.SELECTED;

                vModel.Ballot.ContestsList[vModel.Ballot.ActiveContest].Selected = Convert.ToInt32(selection.Tag);

                vModel.Ballot.slatesDefinition.Data.Id = vModel.Ballot.ContestsList[vModel.Ballot.ActiveContest].Id;
                vModel.Ballot.slatesDefinition.Data.SlateId = Convert.ToInt32(selection.Tag);
            }

            if (buttonsContainer.Controls.Where(it => it.Type == ScreenObject.ScreenObjectType.SELECTION)
                .Count(it => ((VrSelection)it).State == VrSelection.SelectionState.SELECTED) > 0)
            {
                btnReview.Visible = true;
            }
            else
            {
                btnReview.Visible = false;
            }

            UpdateLblChoicesLeft(true);
            SetNavigation();

            vModel.InvalidateObject(btnNext);
            vModel.InvalidateObject(btnReview);
            vModel.UpdateObject(selection);
        }

        private void UpdateLblChoicesLeft(bool updateObject)
        {
            if (lblChoicesLeft != null)
            {
                var choicesLeft = vModel.Ballot.ContestsList[vModel.Ballot.ActiveContest].Selected > 0 ? 0 : 1;
                string choicesText;

                choicesText = string.Format("{0} {1} {2}.", vModel.Vars["lbl_choice1"], Convert.ToString(choicesLeft),
                    choicesLeft != 1 ? vModel.Vars["lbl_multi_0_choices2"] : vModel.Vars["lbl_choice2"]);

                lblChoicesLeft.Text = choicesText;
                this.lblChoicesLeft.ForeColor = "#347409";

                if (updateObject)
                {
                    if (choicesLeft <= 0)
                        AppManager.Instance.StartSpeaker(choicesText + ". Say " + vModel.Vars["next"] + " to continue");
                    else
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
                        this.lblChoicesLeft.ForeColor = "#347409";
                        lblChoicesLeft.Text = choicesText;
                    }
                }
            }
        }

        private void SetNavigation()
        {
            if (vModel.Ballot.ActiveContest < 1)
            {
                if(btnBack != null)
                btnBack.Visible = false;
            }
            if (vModel.Ballot.ActiveContest > -1)
            {
                int n = 1;

                if (vModel.Ballot.ContestsList[vModel.Ballot.ActiveContest].Selected > 0)
                {
                    for (int i = vModel.Ballot.ActiveContest; i <= vModel.Ballot.ContestsList.Count; i++)
                    {
                        if (vModel.Ballot.ContestsList[vModel.Ballot.ActiveContest].Type != VotRiteBallotDataManager.AppCode.ContestTypes.R)
                        {
                            n++;
                        }
                    }

                    btnNext.Text = (string)vModel.Vars["next"];
                    btnNext.Tag = (string)vModel.Vars["skip"];
                    btnNext.BgImage = btnNext.BgImage.Replace("_d", "_a");
                }
                else
                {
                    n = vModel.Ballot.ContestsList.Count + 1;
                    btnNext.Text = (string)vModel.Vars["skip"];
                    btnNext.Tag = (string)vModel.Vars["next"];
                    btnNext.BgImage = btnNext.BgImage.Replace("_a", "_d");
                }

                if (lblCount != null && vModel.Vars["contest_count"] != null)
                    lblCount.Text = (vModel.Ballot.ActiveContest + 1) +
                        " " + vModel.Vars["contest_count"].ToString() +
                        " " + n;
                vModel.InvalidateObject(lblCount);
            }
        }
    }
}
