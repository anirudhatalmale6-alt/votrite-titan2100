using System.Collections;
using VotRiteBallotDataManager.AppCode;
using VotRite.Views;
using VotRite.MVC;

namespace VotRite.Models
{
    internal class RemotePollWorkerModel : ScreenModel
    {
        public Hashtable Vars { get; private set; }

        public Session Session { get; private set; }

        public RemotePollWorkerModel()
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
                // Vars.Add("speech_rate_desc", Session.CurrentLocale.LblSpeechRateDesc);
            }
        }

        public RemotePollWorkerView GetView()
        {
            RemotePollWorkerView v = null;

            foreach (IObserver obs in observers)
            {
                if (obs.GetType().ToString() == "VotRite.Views.RemotePollWorkerView")
                {
                    v = (RemotePollWorkerView)obs;
                    break;
                }
            }

            return v;
        }
    }
}
