using System.Collections;
using VotRite.Definition;
using VotRiteBallotDataManager.AppCode;

namespace VotRite.Models
{
    internal class SpeechRateModel : ScreenModel
    {
        public Hashtable Vars { get; private set; }

        public Session Session { get; private set; }

        public SpeechRateModel()
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
                Vars.Add(Locale.BtnAcceptField, Session.CurrentLocale.BtnAccept);
                Vars.Add(Locale.BtnCancelField, Session.CurrentLocale.BtnCancel);
                Vars.Add("speech_rate_desc", Session.CurrentLocale.LblSpeechRateDesc);
            }
        }
    }
}
