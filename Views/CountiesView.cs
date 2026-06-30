using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using VotRite.UI;
using VotRite.MVC;
using VotRite.Models;
using VotRite.Controllers;

namespace VotRite.Views
{
    class CountiesView : VrScreen
    {
        private CountiesModel vModel { get { return (CountiesModel)model; } }
        private CountiesController vController { get { return (CountiesController)controller; } }
        private VrLabel lblBallotName;
        private VrLabel lblBallotTitle;
        private VrLabel lblBallotAddress;
        private VrLabel lblBallotDate;
        private VrLabel lblGroupName;
        private VrLabel lblCountiesName;
        private VrLabel lblCountiesTitle;
        private VrLabel lblChoicesLeft;
        private VrLabel lblCountiesTip;
        private VrLabel lblCount;
        private VrContainer container;
        private VrButton btnNext;
        private VrButton btnSettingTools;
        private VrButton btnCallHelp;

        public VrContainer Container { get { return container; } }

        //private readonly int SCREEN_V_SPACE = 20;
        private const int selectionsSpace = 20;
        //private bool _hasScroll = false;

        private static int _textSize = 18;
        private Timer _timer = new Timer();
        private const int StartTick = 5;
        protected internal int TimerCount { get; set; }
        public Timer GetTimer()
        {
            return _timer;
        }

        private List<string> _textsToSpeak;

        public CountiesView(CountiesModel m, CountiesController c)
            : base(m, c)
        {
            model = m;
            controller = c;

            lblBallotName = (VrLabel)vModel.FindScreenObject("lbl_blt_name");
            lblBallotTitle = (VrLabel)vModel.FindScreenObject("lbl_blt_title");
            lblBallotAddress = (VrLabel)vModel.FindScreenObject("lbl_blt_address");
            lblBallotDate = (VrLabel)vModel.FindScreenObject("lbl_blt_date");
            lblGroupName = (VrLabel)vModel.FindScreenObject("lbl_group_name");
            lblCountiesName = (VrLabel)vModel.FindScreenObject("lbl_contest_name");
            lblCountiesTitle = (VrLabel)vModel.FindScreenObject("lbl_contest_title");
            lblChoicesLeft = (VrLabel)vModel.FindScreenObject("lbl_choices_left");
            lblCountiesTip = (VrLabel)vModel.FindScreenObject("lbl_county_tip");
            AppManager.Instance.ChangeTextSize(lblCountiesTip);
            lblCount = (VrLabel)vModel.FindScreenObject("lbl_contest_count");


            container = (VrContainer)vModel.FindScreenObject("ctr_contest");
            btnNext = (VrButton)vModel.FindScreenObject("btn_next");
            btnSettingTools = (VrButton)vModel.FindScreenObject("btn_setting_tools");
            btnCallHelp = (VrButton)vModel.FindScreenObject("btn_callhelp");

            Initialize();

            AppManager.Instance.checkDoubleSpaceSetting(vModel.Definition.ScreenObjects, container);

            foreach (var text in _textsToSpeak.Where(text => text != null))
            {
                AppManager.Instance.StartSpeaker(text);
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _timer.Dispose();
                _textsToSpeak.Clear();

                if (btnNext != null) btnNext.Dispose();
                if (btnSettingTools != null) btnSettingTools.Dispose();
                if (btnCallHelp != null) btnCallHelp.Dispose();

                if (lblBallotName != null) lblBallotName.Dispose();
                if (lblBallotTitle != null) lblBallotTitle.Dispose();
                if (lblBallotAddress != null) lblBallotAddress.Dispose();
                if (lblBallotDate != null) lblBallotDate.Dispose();
                if (lblGroupName != null) lblGroupName.Dispose();
                if (lblCountiesName != null) lblCountiesName.Dispose();
                if (lblCountiesTitle != null) lblCountiesTitle.Dispose();
                if (lblChoicesLeft != null) lblChoicesLeft.Dispose();
                if (lblCountiesTip != null) container.Dispose();
                if (lblCount != null) lblCount.Dispose();
                if (container != null) container.Dispose();
            }
            base.Dispose(disposing);
        }

        private void Initialize()
        {
            if (AppManager.Instance.backgroundTheme != AppManager.colorTheme.White)
            {
                vModel.Definition.Background = AppManager.GetPathToCommonFile("graphics" + Global.Instance.SLASH + "initialBG_f1_" + AppManager.Instance.backgroundTheme + ".png");
            }
            if (lblChoicesLeft != null)
                lblChoicesLeft.BgColor = null;
            if (btnNext != null)
                btnNext.Tag = (string)vModel.Vars["next"];
            if (lblBallotName != null)
                lblBallotName.Text = (string)vModel.Vars["ballot_name"];
            if (lblBallotTitle != null)
                lblBallotTitle.Text = (string)vModel.Vars["ballot_title"];
            if ((string)vModel.Vars["ballot_address"] != "")
                lblBallotAddress.Text = (string)vModel.Vars["ballot_address"];
            if (lblBallotDate != null)
                lblBallotDate.Text = (string)vModel.Vars["ballot_date"];

            if (lblGroupName.Text == "" || lblGroupName.Text == "$blt_group_name")
            {
                lblGroupName.Text = "Counties";
            }

            if (lblCountiesName != null)
            {
                lblCountiesName.Text = (string)vModel.Vars["contest_name"];
                lblCountiesName.Visible = false;
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

            lblCountiesTitle.Text = "Choose your county";

            UpdateLblChoicesLeft(false);

            if (lblCountiesTip != null)
            {
                lblCountiesTip.Text = (string)vModel.Vars["county_tip"];
            }

            container.Top = 220;
            container.TextSize = 18;

            int lowestPosition = container.Top;
            int n = 0;
            int topHist = container.Top + 5;
            int startTopHist = topHist;
            int leftPadding = 5;
            var showInOneColumn = Convert.ToBoolean(AppManager.Configuration["Contest"]["ShowInOneColumn"]);
            int selectionWidth = (container.Width - leftPadding - selectionsSpace) / 2;
            int selectionMultiplier = showInOneColumn ? 2 : 1;
            int leftColumnSelectionsPosition = container.Left + leftPadding;
            int rightColumnSelectionsPosition = container.Left + leftPadding + selectionWidth * selectionMultiplier + selectionsSpace;

            int defaultColumnWidth = 7;
            int leftColumnWidth = -1;

            leftColumnWidth = defaultColumnWidth;
            container.TextSize = _textSize;
            int? numberOfColumns = null;

            var multiplier = 2;
            if (numberOfColumns != null)
            {
                multiplier = Convert.ToInt16(AppManager.Configuration["Counties"]["MassPropositionPerPage"]);
            }

            int top = 0;

            foreach (var county in vModel.Ballot.countiesDefinition.Counties)
            {
                // 331 45
                ScreenObject screenObj = new VrSelection
                {
                    Tag = county.Id.ToString(),
                    Text = county.Name,
                    Name = "sel_" + n,
                    Top = container.Top + top,
                    Left = container.Left + 30,
                    ObjectState = ScreenObject.ScreenObjectState.ACTIVE,
                    Width = 360//selectionWidth
                };
                ((VrSelection)screenObj).TextSize = 18;// selHeight;
                /*
                if (vModel.Ballot.countiesDefinition.Counties.Count < 6)
                {
                    ((VrSelection)screenObj).Height = 100;
                } else
                {
                    ((VrSelection)screenObj).Height = 55;// selHeight;
                }
                */
                ((VrSelection)screenObj).Height = 55;
                //((VrSelection)screenObj).CountyLogo = county.CountyLogo;
                AppManager.Instance.ChangeTextSize(screenObj);

                if (vModel.Ballot.CountyId > 0 && county.Id == vModel.Ballot.CountyId)
                { ((VrSelection)screenObj).State = VrSelection.SelectionState.SELECTED; }
                container.Controls.Add(screenObj);
                if (!screenObj.Resized) screenObj.Resize(vModel.Scale);
                /*
                if (vModel.Ballot.countiesDefinition.Counties.Count < 6)
                {
                    top += screenObj.Height + 50;
                }
                else
                {
                    top += 65;
                }
                */
                top += 65;
                n++;
            }

            SetNavigation();

            btnNext.Visible = vModel.Ballot.CountyId > 0;

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
            if (container.CreateScroll(vModel, top - container.Top))
            {
                var upScroll = vModel.Definition.ScreenObjects[vModel.Definition.ScreenObjects.Count - 2];
                upScroll.Top -= 25;

                if (upScroll.ObjectState == ScreenObject.ScreenObjectState.ACTIVE)
                    upScroll.Text = "Touch here for more counties"; //(string)vModel.Vars["more"];
                upScroll.Tag = "Scroll up";

                var downScroll = vModel.Definition.ScreenObjects[vModel.Definition.ScreenObjects.Count - 1];
                if (downScroll.ObjectState == ScreenObject.ScreenObjectState.ACTIVE)
                    downScroll.Text = "Touch here for more counties"; //(string)vModel.Vars["more"];

                downScroll.Tag = "Scroll down";
                container.Top += upScroll.Height;
                container.Height -= (upScroll.Height + downScroll.Height + 20);

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
            _textsToSpeak = GetAllTexts(true, false, false, true);
            var additionalSet = new HashSet<string> { (string)vModel.Vars["next"], (string)vModel.Vars["skip"] };

            CreateListOfCommands(additionalSet);
            AddSpeechToTextEngine();
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
            //Logger.Instance.Write("Counties view updated");
        }

        public void SetSelection(VrSelection selection)
        {
            if (selection.State == VrSelection.SelectionState.SELECTED)
            {
                selection.State = VrSelection.SelectionState.DESELECTED;
                vModel.Ballot.CountyId = 0;
            }
            else
            {
                foreach (var item in container.Controls)
                {
                    if (item.Type == ScreenObject.ScreenObjectType.SELECTION)
                    {
                        ((VrSelection)item).State = VrSelection.SelectionState.DESELECTED;
                        vModel.UpdateObject(item);
                    }
                }
                selection.State = VrSelection.SelectionState.SELECTED;
                vModel.Ballot.CountyId = Convert.ToInt32(selection.Tag);
            }

            UpdateLblChoicesLeft(true);

            btnNext.Visible = vModel.Ballot.CountyId > 0;

            SetNavigation();

            vModel.InvalidateObject(btnNext);
            vModel.UpdateObject(selection);
        }

        private void UpdateLblChoicesLeft(bool updateObject)
        {
            if (lblChoicesLeft != null)
            {
                int choicesLeft = (vModel.Ballot.CountyId > 0) ? 0 : 1; //vModel.Ballot.CountyId != null && vModel.Ballot.CountyId > 0 ? 0 : 1;
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

                if (updateObject)
                {
                    if (choicesLeft <= 0)
                        choicesText += ". Say " + vModel.Vars["next"] + " to continue";
                    AppManager.Instance.StartSpeaker(choicesText);
                    vModel.UpdateObject(lblChoicesLeft);
                }
            }
        }

        private void SetNavigation()
        {
            if (vModel.Ballot.ActiveContest > -1)
            {
                if (vModel.Ballot.CountyId > 0)
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

    }
}
