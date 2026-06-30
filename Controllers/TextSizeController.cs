using VotRite.Models;
using VotRite.UI;
using VotRite.Views;
using System.Data;
using VotRite.Definition;

using System.Collections.Generic;

using VotRite.Util;

namespace VotRite.Controllers
{
    internal class TextSizeController : ScreenController
    {
        public TextSizeView View { get; set; }
        public TextSizeModel Model { get; set; }

        private string mTextSize = "";
        private string mScreenName = "";

        public TextSizeController(TextSizeModel m, string pScreenName)
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
                            IOHandler.SaveConfig("TextSize", "Value", mTextSize);
                            AppManager.Configuration = IOHandler.DecryptConfig();
                            //AppManager.Instance.SetScreen(mScreenName);
                            AppManager.Instance.SetScreen("settingTools", "Locale");
                            break;
                        case "btn_cancel":
                            //AppManager.Instance.SetScreen(mScreenName);
                            AppManager.Instance.SetScreen("settingTools", "Locale");
                            break;
                        default:
                            mTextSize = View.GetTextSizeValue(scrObj.Data, false);
                            if (!string.IsNullOrEmpty(scrObj.Data) && View != null)
                                View.DataReceived(scrObj.Text);
                            break;
                            //scrObj.ForeColor = System.Drawing.Color.Red.ToArgb().ToString();
                            //scrObj.Draw(Window.Instance.CreateGraphics());

                            //if (!string.IsNullOrEmpty(scrObj.Text) && View != null)
                            //    View.DataReceived(scrObj.Text);
                            //break;
                    }
                }
            }
        }

        //private void SetAllButtonsDefaultForecolor()
        //{
        //    //foreach (ScreenObject oCtl in this.View)
        //    //{
        //    //}
        //}

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
