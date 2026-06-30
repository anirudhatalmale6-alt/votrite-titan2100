using VotRite.Models;
using VotRite.UI;
using VotRite.Views;
using System.Data;
using VotRite.Definition;

using System.Collections.Generic;

using VotRite.Util;

using VotRite.JimForms;
using System.Diagnostics;
using System.IO;

namespace VotRite.Controllers
{
    internal class SettingToolsController : ScreenController
    {
        public SettingToolsView View { get; set; }
        public SettingToolsModel Model { get; set; }

        //private string mSettingTools = "";
        private string mScreenName = "";

        public SettingToolsController(SettingToolsModel m, string pScreenName)
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
                        case "btn_audio":
                            AppManager.Instance.SetScreen("speechRate", mScreenName);
                            break;
                        case "btn_textsize":
                            AppManager.Instance.SetScreen("textSize", mScreenName);
                            break;
                        case "btn_language":
                            F_Language oForm = new F_Language();
                            if (oForm.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                            {
                                ////System.Windows.Forms.SendKeys.Send("{ESC}");
                               // AppManager.Instance.SetScreen(mScreenName);
                            }
                            else
                            {
                               // AppManager.Instance.SetScreen(mScreenName);
                            }
                            break;
                        case "btn_cancel":
                            AppManager.Instance.SetScreen(mScreenName);
                            break;
                        case "btn_help":
                            F_HelpSetting oHelp = new F_HelpSetting();
                            oHelp.ShowDialog();
                           // AppManager.Instance.SetScreen(mScreenName);
                            break;
                        case "btn_remote_poll_worker":
                            AppManager.Instance.SetScreen("remotePollWorker", mScreenName);
                            break;
                        case "btn_doublespace":
                            if(AppManager.Instance.isDoubleSpacing)
                            {
                                if (System.Windows.Forms.MessageBox.Show("Do you want to TURN-OFF double spacing??","Double Spacing OFF?", System.Windows.Forms.MessageBoxButtons.YesNo , System.Windows.Forms.MessageBoxIcon.Question)
                                     == System.Windows.Forms.DialogResult.Yes)
                                    AppManager.Instance.isDoubleSpacing = false;
                            }
                            else
                            {
                                if (System.Windows.Forms.MessageBox.Show("Do you want to TURN-ON double spacing??", "Double Spacing ON?", System.Windows.Forms.MessageBoxButtons.YesNo, System.Windows.Forms.MessageBoxIcon.Question)
                                     == System.Windows.Forms.DialogResult.Yes)
                                    AppManager.Instance.isDoubleSpacing = true;
                            }
                            break;
                        case "btn_brightness":
                            //AppManager.Instance.SetScreen("remotePollWorker", mScreenName);
                            try
                            {
                               // AppManager.Instance.stopVotriteSpeech();
                                // Redirect the output stream of the child process.
                                Process p = new Process();
                                p.StartInfo.CreateNoWindow = true;
                                p.StartInfo.UseShellExecute = false;
                                //p.StartInfo.RedirectStandardOutput = true;
                                p.StartInfo.FileName = Path.Combine(System.AppDomain.CurrentDomain.BaseDirectory, "BrightnessControl.exe");
                                p.Start();
                                p.WaitForExit();
                            }
                            catch (System.Exception ex)
                            {
                            }
                            break;
                        case "btn_themes":
                            themes oFrmtheme = new themes();
                            oFrmtheme.ShowDialog();
                            break;
                        case "btn_preview":
                            settings_preview oFrmsettings_preview = new settings_preview();
                            oFrmsettings_preview.ShowDialog();
                            break;
                        case "btn_preview_hearing":
                            settings_preview oFrmsettings_prev= new settings_preview();
                            oFrmsettings_prev.hearingmode = true;
                            oFrmsettings_prev.ShowDialog();
                            break;
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
