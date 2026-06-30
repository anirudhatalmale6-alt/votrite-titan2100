using System.Collections.Generic;
using System.Linq;
using VotRite.Models;
using VotRite.Controllers;
using VotRite.UI;
using VotRiteBallotDataManager.AppCode;

namespace VotRite.Views
{
    class RemotePollWorkerView : VrScreen
    {
        private RemotePollWorkerModel vModel { get { return model as RemotePollWorkerModel; } }
        private RemotePollWorkerController vController { get { return controller as RemotePollWorkerController; } }
        private VrButton _btnCancel;
        private VrButton _btnAccept;
        private VrSelection btnEnableRemotePollWorker;
        private VrContainer container;

        public RemotePollWorkerView(ScreenModel m, ScreenController c)
            : base(m, c)
        {
            model = m as RemotePollWorkerModel;
            controller = c as RemotePollWorkerController;
            _btnCancel = (VrButton)vModel.FindScreenObject("btn_cancel");
            _btnAccept = (VrButton)vModel.FindScreenObject("btn_accept");
            container = (VrContainer)vModel.FindScreenObject("ctr_contest");
            
            if (controller != null)
                vController.View = this;
            Initialize();

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
                if (_btnCancel != null) _btnCancel.Dispose();
                if (_btnAccept != null) _btnAccept.Dispose();
            }
            base.Dispose(disposing);
        }

        private void Initialize()
        {
            Name = "remotePollWorker";
            if (_btnCancel != null)
            {
                _btnCancel.Text = (string)vModel.Vars[Locale.BtnCancelField];
                if (_btnCancel.Text == null)
                    _btnCancel.Text = "Cancel";
                _btnCancel.Resize(vModel.Scale);
                container.Controls.Remove(_btnCancel);
                container.Controls.Add(_btnCancel);
            }
            if (_btnAccept != null)
            {
                _btnAccept.Text = (string)vModel.Vars[Locale.BtnAcceptField];
                if (_btnAccept.Text == null)
                    _btnAccept.Text = "Accept";
                _btnAccept.Resize(vModel.Scale);
                container.Controls.Remove(_btnAccept);
                container.Controls.Add(_btnAccept);
            }

            string settings = AppManager.Configuration["System"]["RemotePollWorker"];

            btnEnableRemotePollWorker = new VrSelection();
            btnEnableRemotePollWorker.Name = "sel_remote_poll_worker";
            btnEnableRemotePollWorker.State = VrSelection.SelectionState.DESELECTED;
            btnEnableRemotePollWorker.Text = "Remote Poll Worker Disabled";
            btnEnableRemotePollWorker.TextSize = 20;
            btnEnableRemotePollWorker.Width = 360;
            btnEnableRemotePollWorker.Height = 70;
            btnEnableRemotePollWorker.Left = 400;
            btnEnableRemotePollWorker.Top = 380;

            if (settings == "True")
            {
                SetSelection(btnEnableRemotePollWorker);
            }

            if (!container.Controls.Contains(btnEnableRemotePollWorker))
            {
                btnEnableRemotePollWorker.Resize(vModel.Scale);
                container.Controls.Add(btnEnableRemotePollWorker);
            }

            CreateListOfCommands();
            AddSpeechToTextEngine();
        }

        public void SetSelection(VrSelection selection)
        {
            if (selection.State == VrSelection.SelectionState.SELECTED)
            {
                selection.State = VrSelection.SelectionState.DESELECTED;
                selection.Text = "Remote Poll Worker Disabled";
                _btnAccept.Data = "disabled";
            } else
            {
                selection.State = VrSelection.SelectionState.SELECTED;
                selection.Text = "Remote Poll Worker Enabled";
                _btnAccept.Data = "enabled";
            }

            vModel.UpdateObject(selection);
        }
    }
}
