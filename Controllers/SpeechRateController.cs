using VotRite.Models;
using VotRite.UI;
using VotRite.Views;
using System.Data;
using VotRite.Definition;

using System.Collections.Generic;

using VotRite.Util;

namespace VotRite.Controllers
{
    internal class SpeechRateController : ScreenController
    {
        public SpeechRateView View { get; set; }
        public SpeechRateModel Model { get; set; }

        private string mSpeechRate = "";
        private string mScreenName = "";

        public SpeechRateController(SpeechRateModel m, string pScreenName)
            : base(m)
        {
            Model = m;

            mScreenName = pScreenName;
        }

        public override void HandleTouch(int x, int y)
        {
            var scrObj = Model.FindScreenObject(x, y);

            if (scrObj != null)
            {
                if (scrObj.Type == ScreenObject.ScreenObjectType.CONTAINER)
                    scrObj = ((VrContainer)scrObj).FindScreenObject(x, y);

                HandleButtonClick(scrObj);
            }
        }

        //New Verson Code (Modified By Jim On 9/16/2015)
        private void HandleButtonClick(ScreenObject scrObj)
        {
            if (scrObj != null && scrObj.Type == ScreenObject.ScreenObjectType.BUTTON)
            {
                if (scrObj.Visible)
                {
                    switch (scrObj.Name)
                    {
                        case "btn_accept":
                            IOHandler.SaveConfig("Speech", "Rate", mSpeechRate);
                            AppManager.Configuration = IOHandler.DecryptConfig();
                            //AppManager.Instance.SetScreen(mScreenName);
                            AppManager.Instance.SetScreen("settingTools", "Locale");
                            break;
                        case "btn_cancel":
                            //AppManager.Instance.SetScreen( mScreenName);
                            //AppManager.Instance.SetScreen("settingtools");
                            AppManager.Instance.SetScreen("settingTools", "Locale");
                            break;
                        default:
                            mSpeechRate = View.GetSpeechRateValue(scrObj.Data, false);
                            int rate = -1;
                            if (int.TryParse(scrObj.Data, out rate))
                                mSpeechRate = scrObj.Data;
                            if (!string.IsNullOrEmpty(scrObj.Data) && View != null)
                                View.DataReceived(scrObj.Data);
                            break;
                    }
                }
            }
        }

        public override void HandleSpeech(string recogWord)
        {
            var srcObject = Model.FindClickableObjectByTextOrTag(recogWord);
            if (srcObject == null)
            {
                var container = (VrContainer)Model.FindScreenObject(ScreenObject.ScreenObjectType.CONTAINER);
                if (container != null)
                {
                    srcObject = container.FindClickableObjectByTextOrTag(recogWord);
                }
            }
            if (srcObject != null)
            {
                HandleButtonClick(srcObject);
            }
        }

    }
}
