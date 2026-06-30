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
    class SpeechRateView : VrScreen
    {
        private const int ERROR_DELAY = 2000;

        private StringBuilder buffer = new StringBuilder();
        private Timer errorDelay = new Timer();

        private SpeechRateModel vModel { get { return model as SpeechRateModel; } }
        private SpeechRateController vController { get { return controller as SpeechRateController; } }

        private VrButton _btnCancel;
        private VrButton _btnAccept;
        private VrLabel _lblDesc;

        public VrLabel LblDisplay { get; private set; }


        public SpeechRateView(ScreenModel m, ScreenController c)
            : base(m, c)
        {
            model = m as SpeechRateModel;
            controller = c as SpeechRateController;
            _lblDesc = (VrLabel)vModel.FindScreenObject("lbl_speech_rate_desc");
            _btnCancel = (VrButton)vModel.FindScreenObject("btn_cancel");
            _btnAccept = (VrButton)vModel.FindScreenObject("btn_accept");
            LblDisplay = (VrLabel)vModel.FindScreenObject("lbl_text");

            if (controller != null)
                vController.View = this;
            Initialize();

            var textsToSpeak = new List<string>
                                   {
                                       GetControlText(false, false, false, _lblDesc),
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
                if (_lblDesc != null) _lblDesc.Dispose();

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
                string sRate = AppManager.Configuration["Speech"]["Rate"];
                if (sRate == "" || sRate == "-1")
                {
                    LblDisplay.Text = "2";
                    
                }
                else
                    LblDisplay.Text = sRate;// GetSpeechRateValue(sRate, true);

            }
            if (_lblDesc != null)
            {
               // _lblDesc.Text = (string)vModel.Vars["speech_rate_desc"];
                //if (_lblDesc.Text == null)
                    _lblDesc.Text = "One - lowest rate, Five - highest rate, Two - Normal rate.";
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

        public string GetSpeechRateValue(string pValue, bool pIsRead)
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
            LblDisplay.Text = data;

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
