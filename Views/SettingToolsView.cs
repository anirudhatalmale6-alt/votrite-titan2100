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
    class SettingToolsView : VrScreen
    {
        private const int ERROR_DELAY = 2000;

        private StringBuilder buffer = new StringBuilder();
        private Timer errorDelay = new Timer();

        private SettingToolsModel vModel { get { return model as SettingToolsModel; } }
        private SettingToolsController vController { get { return controller as SettingToolsController; } }

        private VrButton _btnCancel;
        private VrButton btnAudio;
        private VrButton btnTextSize;
        private VrButton btnLanguage;
        private VrButton btnHelp;
        private VrButton btnRemotePollWorker;

        private VrLabel _lblDesc;
        private VrLabel _lblTitle;
        private VrButton btnDoubleSpace;
        private VrButton btnBrightness;
        private VrButton btnThemes;

        public SettingToolsView(ScreenModel m, ScreenController c)
            : base(m, c)
        {
            model = m as SettingToolsModel;
            controller = c as SettingToolsController;
            
            _lblDesc = (VrLabel)vModel.FindScreenObject("lbl_setting_tools_desc");
            _lblTitle = (VrLabel)vModel.FindScreenObject("lbl_pp_title");
            _btnCancel = (VrButton)vModel.FindScreenObject("btn_cancel");
            btnAudio = (VrButton)vModel.FindScreenObject("btn_audio");
            btnTextSize = (VrButton)vModel.FindScreenObject("btn_textsize");
            btnLanguage = (VrButton)vModel.FindScreenObject("btn_language");
            btnHelp = (VrButton)vModel.FindScreenObject("btn_help");
            btnRemotePollWorker = (VrButton)vModel.FindScreenObject("btn_remote_poll_worker");
            //btnDoubleSpace = (VrButton)vModel.FindScreenObject("btn_remote_poll_worker");

            //btnDoubleSpace.Top += 50;
            

            //btnDoubleSpace.Text = "Double Spacing";

           // vModel.Definition.ScreenObjects.Add(btnDoubleSpace);

            if (controller != null)
                vController.View = this;
            Initialize();

            var textsToSpeak = new List<string>
                                   {
                                       GetControlText(false, false, false, _lblDesc),
                                       GetControlText(false, false, false, btnAudio),
                                       GetControlText(false, false, false, btnTextSize),
                                       GetControlText(false, false, false, btnLanguage),
                                       GetControlText(false, false, false, btnHelp),
                                       GetControlText(false, false, false, btnRemotePollWorker),
                                       GetControlText(false, false, false, _btnCancel),
                                   };
            
            foreach (var text in textsToSpeak.Where(text => text != null))
            {
                AppManager.Instance.StartSpeaker(text);
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (errorDelay != null) errorDelay.Dispose();
                if (_btnCancel != null) _btnCancel.Dispose();
                if (btnAudio != null) btnAudio.Dispose();
                if (btnTextSize != null) btnTextSize.Dispose();
                if (btnLanguage != null) btnLanguage.Dispose();
                if (btnHelp != null) btnHelp.Dispose();
                if (btnRemotePollWorker != null) btnRemotePollWorker.Dispose();

                if (_lblDesc != null) _lblDesc.Dispose();
                if (_lblTitle != null) _lblTitle.Dispose();
            }
            base.Dispose(disposing);
        }

        private void Initialize()
        {
            try
            {
                btnRemotePollWorker.Top -= 2 * ( btnRemotePollWorker.Height + 15);
                btnDoubleSpace = new VrButton
                {
                    Name = "btn_doublespace",
                    BgImage = AppManager.GetPathToCommonFile("graphics" +
                       Global.Instance.SLASH +
                       "button_green.jpg"),
                    Text = "Double Spacing",
                    ForeColor = "#ffffff",
                    TextSize = btnRemotePollWorker.TextSize,
                    TextAlign = "center-middle",
                    Width = btnRemotePollWorker.Width,
                    Height = btnRemotePollWorker.Height,
                    Left = btnRemotePollWorker.Left,
                    Top = btnRemotePollWorker.Top + btnRemotePollWorker.Height+ 15
                };
                vModel.Definition.ScreenObjects.Add(btnDoubleSpace);

                btnBrightness = new VrButton
                {
                    Name = "btn_brightness",
                    BgImage = AppManager.GetPathToCommonFile("graphics" +
                       Global.Instance.SLASH +
                       "button_green.jpg"),
                    Text = "Screen Brightness",
                    ForeColor = "#ffffff",
                    TextSize = btnRemotePollWorker.TextSize,
                    TextAlign = "center-middle",
                    Width = btnRemotePollWorker.Width,
                    Height = btnRemotePollWorker.Height,
                    Left = btnRemotePollWorker.Left,
                    Top = btnDoubleSpace.Top + btnDoubleSpace.Height + 15
                };
                //vModel.Definition.ScreenObjects.Add(btnBrightness);

                btnBrightness = new VrButton
                {
                    Name = "btn_themes",
                    BgImage = AppManager.GetPathToCommonFile("graphics" +
                      Global.Instance.SLASH +
                      "button_green.jpg"),
                    Text = "Background Color",
                    ForeColor = "#ffffff",
                    TextSize = btnRemotePollWorker.TextSize,
                    TextAlign = "center-middle",
                    Width = btnRemotePollWorker.Width,
                    Height = btnRemotePollWorker.Height,
                    Left = btnRemotePollWorker.Left,
                    Top = btnDoubleSpace.Top + btnBrightness.Height + 15
                };
                vModel.Definition.ScreenObjects.Add(btnBrightness);

                btnBrightness = new VrButton
                {
                    Name = "btn_preview",
                    BgImage = AppManager.GetPathToCommonFile("graphics" +
                      Global.Instance.SLASH +
                      //"button_green.jpg"),
                      "bg_key_pp2.png"),
                    Text = "Preview Ballot",
                    ForeColor = "#000000",
                    TextSize = btnRemotePollWorker.TextSize,
                    TextAlign = "center-middle",
                    Width = btnRemotePollWorker.Width,
                    Height = btnRemotePollWorker.Height,
                    Left = btnRemotePollWorker.Left,
                    Top = btnBrightness.Top + btnBrightness.Height + 15
                };
                vModel.Definition.ScreenObjects.Add(btnBrightness);

                btnBrightness = new VrButton
                {
                    Name = "btn_preview_hearing",
                    BgImage = AppManager.GetPathToCommonFile("graphics" +
                      Global.Instance.SLASH +
                      //"button_green.jpg"),
                      "bg_key_pp2.png"),
                    Text = "Preview Visual Impaired",
                    ForeColor = "#000000",
                    TextSize = btnRemotePollWorker.TextSize,
                    TextAlign = "center-middle",
                    Width = btnRemotePollWorker.Width,
                    Height = btnRemotePollWorker.Height,
                    Left = btnRemotePollWorker.Left,
                    Top = btnBrightness.Top + btnBrightness.Height + 15
                };
                vModel.Definition.ScreenObjects.Add(btnBrightness);
            }
            catch (System.Exception)
            {

            }
            if (_btnCancel != null)
            {
                _btnCancel.Text = "Back to Voting";// (string)vModel.Vars[Locale.BtnCancelField];
                _btnCancel.BgImage = "graphics\\button_red.jpg";
                _btnCancel.Width += 105;
                _btnCancel.Left -= 50;
                _btnCancel.Left += 400;
                _btnCancel.Top = btnBrightness.Top;
            }
            if (btnAudio != null)
            {
                //btnAudio.Text = (string)vModel.Vars[Locale.BtnAudioField];
                btnAudio.Text = "Adjust Speech Rate";
            }
            if (btnTextSize != null)
            {
                btnTextSize.Text = (string)vModel.Vars[Locale.BtnTextSizeField];
                if (btnTextSize.Text == null)
                    btnTextSize.Text = "Text Size";
            }
            if (btnLanguage != null)
            {
                btnLanguage.Text = (string)vModel.Vars[Locale.BtnLanguageField];
                if (btnLanguage.Text == null)
                    btnLanguage.Text = "Language";

                btnLanguage.Visible = false;
            }
            if (btnHelp != null)
            {
                btnHelp.Text = (string)vModel.Vars[Locale.BtnHelpField];
                if (btnHelp.Text == null)
                    btnHelp.Text = "Help";
                btnHelp.Visible = false;
                btnHelp.Top = btnBrightness.Top + btnBrightness.Height + 15;
            }
            if (btnRemotePollWorker != null)
            {
                btnRemotePollWorker.Text = (string)vModel.Vars[Locale.BtnRemotePollWorkerField];
                if (btnRemotePollWorker.Text == null)
                    btnRemotePollWorker.Text = "Remote Poll Worker";
                btnRemotePollWorker.Visible = false;
            }
            vModel.Definition.Top -= 100;
             vModel.Definition.Width = vModel.Definition.Width + 200;
            vModel.Definition.Height = vModel.Definition.Height + 200;

           
            if(_lblTitle != null)
            {
                _lblTitle.Width = _lblTitle.Width + 160;
            }

            foreach(var cntrl in vModel.Definition.ScreenObjects)
            {
                cntrl.Top -= 100;
                cntrl.TextSize = 22;
            }
            if (_lblDesc != null)
            {
                _lblDesc.Text = (string)vModel.Vars["setting_tools_desc"];
                _lblDesc.TextSize = 30;
                _lblDesc.Height = _lblDesc.Height + 80;
                _lblDesc.Width = _lblDesc.Width + 150;
            }
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
    }
}
