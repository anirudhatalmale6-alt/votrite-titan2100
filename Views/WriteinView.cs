using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Timers;
using VotRite.Models;
using VotRite.Controllers;
using VotRite.UI;
using VotRiteBallotDataManager.AppCode;

namespace VotRite.Views
{
    class WriteinView : VrScreen
    {
        private const int ERROR_DELAY = 2000;
        
        private StringBuilder buffer = new StringBuilder();
        private Timer errorDelay = new Timer();

        private WriteinModel vModel { get { return model as WriteinModel; } }
        private WriteinController vController { get { return controller as WriteinController; } }

        private VrLabel _lblBallotName;
        private VrLabel _lblBallotTitle;
        private VrLabel _lblBallotAddress;
        private VrLabel _lblBallotDate;
        private VrLabel _lblContestTitle;
        private VrLabel _lblWriteinHelp1;
        private VrLabel _lblWriteinHelp2;
        private VrLabel _lblWriteinHelp3;
        private VrLabel _lblWriteinHelp4;
        private VrLabel _lblWriteinHelp5;
        private VrButton _btnCancel;
        private VrButton _btnAccept;
        private VrLabel _lblCount;

        public VrLabel LblDisplay { get; private set; }

        public WriteinView(ScreenModel m, ScreenController c)
            : base(m, c)
        {
            model = m as WriteinModel;
            controller = c as WriteinController;

            _lblBallotName = (VrLabel)vModel.FindScreenObject("lbl_blt_name");
            _lblBallotTitle = (VrLabel)vModel.FindScreenObject("lbl_blt_title");
            _lblBallotAddress = (VrLabel)vModel.FindScreenObject("lbl_blt_address");
            _lblBallotDate = (VrLabel)vModel.FindScreenObject("lbl_blt_date");
            _lblContestTitle = (VrLabel)vModel.FindScreenObject("lbl_blt_contest");
            _lblWriteinHelp1 = (VrLabel)vModel.FindScreenObject("lbl_wi_help1");
            _lblWriteinHelp2 = (VrLabel)vModel.FindScreenObject("lbl_wi_help2");
            _lblWriteinHelp3 = (VrLabel)vModel.FindScreenObject("lbl_wi_help3");
            _lblWriteinHelp4 = (VrLabel)vModel.FindScreenObject("lbl_wi_help4");
            _lblWriteinHelp5 = (VrLabel)vModel.FindScreenObject("lbl_wi_help5");
            _btnCancel = (VrButton)vModel.FindScreenObject("btn_cancel");
            _btnAccept = (VrButton)vModel.FindScreenObject("btn_accept");
            _lblCount = (VrLabel)vModel.FindScreenObject("lbl_contest_count");
            LblDisplay = (VrLabel)vModel.FindScreenObject("lbl_text");

            if (controller != null)
                vController.View = this;
            Initialize();

            AppManager.Instance.writeinModel = true;
            AppManager.Instance.checkDoubleSpaceSetting(vModel.Definition.ScreenObjects, null);
            AppManager.Instance.writeinModel = false;

            var textsToSpeak = new List<string>
                                   {
                                       GetControlText(false, false, false, _lblContestTitle),
                                       GetControlText(false, false, false, _lblWriteinHelp1),
                                       GetControlText(false, false, false, _lblWriteinHelp2),
                                       GetControlText(false, false, false, _lblWriteinHelp3),
                                       GetControlText(false, false, false, _lblWriteinHelp4),
                                       GetControlText(false, false, false, _lblWriteinHelp5),
                                       GetControlText(false, false, false, _btnCancel),
                                       GetControlText(false, false, false, _btnAccept)
                                   };
            //////textsToSpeak.Add("dash");
            //////textsToSpeak.Add("A");
            //////textsToSpeak.Add("D");
            //////textsToSpeak.Add("back space");

            foreach (var text in textsToSpeak.Where(text => text != null))
            {
                AppManager.Instance.StartSpeaker(text);
            }

            if (AppManager.useExternalKeyBoard)
            {
                foreach (var item in vModel.Definition.ScreenObjects)
                {
                    if (item.Type == ScreenObject.ScreenObjectType.BUTTON && item.Text != "Accept" && item.Text != "Cancel")
                        //item.Visible = false;

                        item.ObjectState = ScreenObject.ScreenObjectState.INACTIVE;
                }

            }
            if (AppManager.Instance.backgroundTheme == AppManager.colorTheme.Contrast)
            {
                var buttons = new string[] { "btn_cancel", "btn_accept" };
                foreach (var cntrl in this.vModel.Definition.ScreenObjects)

                {
                    if (cntrl != null)
                    {
                        if (cntrl is VrButton)
                        {
                            if(cntrl.Name != "btn_cancel" && cntrl.Name != "btn_accept")
                            {
                                ((VrButton)cntrl).BgImage = "CommonFiles\\graphics\\bg_key_contrast.png";
                                cntrl.ForeColor = "#FFFFFF";
                            }
                        }
                    }
                }
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (errorDelay != null) errorDelay.Dispose();
                if (_lblBallotName != null) _lblBallotName.Dispose();
                if (_lblBallotTitle != null) _lblBallotTitle.Dispose();
                if (_lblBallotAddress != null) _lblBallotAddress.Dispose();
                if (_lblBallotDate != null) _lblBallotDate.Dispose();
                if (_lblContestTitle != null) _lblContestTitle.Dispose();
                if (_lblWriteinHelp1 != null) _lblWriteinHelp1.Dispose();
                if (_lblWriteinHelp2 != null) _lblWriteinHelp2.Dispose();
                if (_lblWriteinHelp3 != null) _lblWriteinHelp3.Dispose();
                if (_lblWriteinHelp4 != null) _lblWriteinHelp4.Dispose();
                if (_lblWriteinHelp5 != null) _lblWriteinHelp5.Dispose();
                if (_btnCancel != null) _btnCancel.Dispose();
                if (_btnAccept != null) _btnAccept.Dispose();
                if (_lblCount != null) _lblCount.Dispose();
                if (LblDisplay != null) LblDisplay.Dispose();
            }
            base.Dispose(disposing);
        }

        private void Initialize()
        {
            if (AppManager.Instance.backgroundTheme != AppManager.colorTheme.White)
            {
                vModel.Definition.Background = AppManager.GetPathToCommonFile("graphics" + Global.Instance.SLASH + "initialBG_f1_" + AppManager.Instance.backgroundTheme + ".png");
            }

            if (_lblBallotName != null)
                _lblBallotName.Text = (string)vModel.Vars["ballot_name"];
            if (_lblBallotTitle != null)
                _lblBallotTitle.Text = (string)vModel.Vars["ballot_title"];
            if ((string)vModel.Vars["ballot_address"] != "")
                _lblBallotAddress.Text = (string)vModel.Vars["ballot_address"];
            if (_lblBallotDate != null)
                _lblBallotDate.Text = (string)vModel.Vars["ballot_date"];
            if (_lblContestTitle != null)
                _lblContestTitle.Text = (string)vModel.Vars["contest_name"];
            if (_lblWriteinHelp1 != null)
                _lblWriteinHelp1.Text = (string)vModel.Vars[Locale.WriteinHelp1Field];
            if (_lblWriteinHelp2 != null)
                _lblWriteinHelp2.Text = (string)vModel.Vars[Locale.WriteinHelp2Field];
            if (_lblWriteinHelp3 != null)
                _lblWriteinHelp3.Text = (string)vModel.Vars[Locale.WriteinHelp3Field];
            if (_lblWriteinHelp4 != null)
                _lblWriteinHelp4.Text = (string)vModel.Vars[Locale.WriteinHelp4Field];
            if (_lblWriteinHelp5 != null)
                _lblWriteinHelp5.Text = (string)vModel.Vars[Locale.WriteinHelp5Field];
            if (_btnCancel != null)
                _btnCancel.Text = (string)vModel.Vars[Locale.BtnCancelField];
            if (_btnAccept != null)
                _btnAccept.Text = (string)vModel.Vars[Locale.BtnAcceptField];
            if (_lblCount != null)
                _lblCount.Text = (vModel.Ballot.ActiveContest + 1) + " " + vModel.Vars["contest_count"] + " " + vModel.Ballot.ContestsList.Count;

            CreateListOfCommands();
            AddSpeechToTextEngine();
        }

        public void ClearBuffer() {
            buffer.Remove(0, buffer.Length);
        }

        private void Error(string text)
        {
            ClearBuffer();
            buffer.Append(text);
            errorDelay.Enabled = true;
        }

        private void ErrorDelay_Elapsed(object sender, ElapsedEventArgs args)
        {
            ClearBuffer();
            errorDelay.Enabled = false;
        }

        public void DataReceived(string data)
        {
            switch (data)
            {
                case "Space":
                //case "Space Key":
                    LblDisplay.Text += " ";
                    break;
                case "BKSP":
                    if (LblDisplay.Text.Length > 0)
                        LblDisplay.Text = LblDisplay.Text.Substring(0,
                                                                    LblDisplay.Text.Length - 1);
                    break;
                case "Delete":
                //case "Deleting":
                    if (LblDisplay.Text.Length > 0)
                        LblDisplay.Text = LblDisplay.Text.Substring(0,
                                                                    LblDisplay.Text.Length - 1);
                    break;
                default:
                    LblDisplay.Text += data;
                    break;
            }

            vModel.UpdateObject(LblDisplay);
            SetAcceptance();
        }

        private void SetAcceptance()
        {
            _btnAccept.State = LblDisplay.Text.Trim() != "" ? ScreenObject.ScreenObjectState.ACTIVE : ScreenObject.ScreenObjectState.INACTIVE;
            vModel.UpdateObject(_btnAccept);
        }
    }
}
