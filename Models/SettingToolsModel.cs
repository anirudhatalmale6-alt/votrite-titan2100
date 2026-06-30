using System.Collections;
using VotRite.Definition;
using VotRiteBallotDataManager.AppCode;

namespace VotRite.Models
{
    internal class SettingToolsModel : ScreenModel
    {
        public Hashtable Vars { get; private set; }

        public Session Session { get; private set; }

        public SettingToolsModel()
        {
            Vars = new Hashtable();
            Session = AppManager.Instance.Session;

            Initialize();
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                Vars.Clear();
                Session.Dispose();
            }
            base.Dispose(disposing);
        }

        private void Initialize()
        {
            if (Session != null)
            {
                Vars.Add(Locale.BtnAudioField, Session.CurrentLocale.BtnAudio);
                Vars.Add(Locale.BtnTextSizeField, Session.CurrentLocale.BtnTextSize);
                Vars.Add(Locale.BtnLanguageField, Session.CurrentLocale.BtnLanguage);
                Vars.Add(Locale.BtnHelpField, Session.CurrentLocale.BtnHelp);
                Vars.Add(Locale.BtnCancelField, Session.CurrentLocale.BtnCancel);
                Vars.Add(Locale.BtnRemotePollWorkerField, Session.CurrentLocale.BtnRemotePollWorker);
                Vars.Add("setting_tools_desc", Session.CurrentLocale.LblSettingToolsDesc);
            }
        }
    }
}
