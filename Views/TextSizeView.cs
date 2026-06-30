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
    class TextSizeView : VrScreen
    {
        private const int ERROR_DELAY = 2000;

        private StringBuilder buffer = new StringBuilder();
        private Timer errorDelay = new Timer();

        private TextSizeModel vModel { get { return model as TextSizeModel; } }
        private TextSizeController vController { get { return controller as TextSizeController; } }

        private VrButton _btnCancel;
        private VrButton _btnAccept;

        public VrLabel LblDisplay { get; private set; }


        public TextSizeView(ScreenModel m, ScreenController c)
            : base(m, c)
        {
            model = m as TextSizeModel;
            controller = c as TextSizeController;
            _btnCancel = (VrButton)vModel.FindScreenObject("btn_cancel");
            _btnAccept = (VrButton)vModel.FindScreenObject("btn_accept");
            LblDisplay = (VrLabel)vModel.FindScreenObject("lbl_text");

            if (controller != null)
                vController.View = this;
            Initialize();

            foreach (var cntrl in this.vModel.Definition.ScreenObjects)

            {
                if (cntrl != null)
                {
                    switch (cntrl.Text)
                    {
                        case "8":
                            cntrl.Text = "Very Small";
                            break;
                        case "9":
                            cntrl.Text = "Small";
                            break;
                        case "10":
                            cntrl.Text = "Medium (Recommended)";
                            break;
                        case "11":
                            cntrl.Text = "Large";
                            break;
                        case "12":
                            cntrl.Text = "Very Large";
                            break;
                        case "13":
                            cntrl.Text = "Extra Large";
                            break;
                        default:
                            break;
                    }
                    

                }
            }

                    var textsToSpeak = new List<string>
                                   {
                                       GetControlText(false, false, false, _btnCancel),
                                       GetControlText(false, false, false, _btnAccept)
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
                if (_btnAccept != null) _btnAccept.Dispose();
                if (LblDisplay != null) LblDisplay.Dispose();

            }

            base.Dispose(disposing);
        }

        private void Initialize()
        {
            if (_btnCancel != null)
            {
                _btnCancel.Text = (string)vModel.Vars[Locale.BtnCancelField];
                if (_btnCancel.Text == null)
                    _btnCancel.Text = "Cancel";
            }
            if (_btnAccept != null)
            {
                _btnAccept.Text = (string)vModel.Vars[Locale.BtnAcceptField];
                if (_btnAccept.Text == null)
                    _btnAccept.Text = "Accept";
                SetAcceptance();
            }
            if (LblDisplay != null)
            {
                string sRate = AppManager.Configuration["TextSize"]["Value"];
                if (sRate != "")
                {
                    LblDisplay.Text = GetTextSizeValue(sRate, true);
                }
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

        public string GetTextSizeValue(string pValue, bool pIsRead)
        {
            string sRet = "";

            if (pIsRead)
            {
                int iTmp = (int.Parse(pValue) + 10) / 3;
                sRet = iTmp.ToString();
            }
            else
            {
                sRet = string.Format("{0:0}", int.Parse(pValue) * 3 - 10);
            }

            return sRet;
        }

        public void DataReceived(string data)
        {
            LblDisplay.Text = data.Replace("(Recommended)", "");

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
