// Product:	VotRite
// Module:  ConfirmController.cs
// Author:  Dmitriy Slipak

// 
// Copyright (c) 2017 - VOTRITE INTERNATIONAL LLC. All rights reserved.
// 
// THIS PRODUCT INCLUDES SOFTWARE AS A PART OF VOTRITE VOTING 
// SYSTEM DEVELOPED AT VOTRITE INTERNATIONAL LLC (http://www.votrite.com).
// THE SOURCE CODE FOR THIS PRODUCT IS SUBJECT TO THE VOTRITE INTERNATIONAL LLC 
// LICENSE. NO ANY PORTION OF THIS PRODUCT OR SOURCE CODE CAN BE 
// REDISTRIBUTED UNDER ANY CIRCUMSTANCES.
// 
using VotRite.Models;
using VotRite.UI;

namespace VotRite.Controllers
{
    class ConfirmController : ScreenController
    {
        private new ConfirmModel model;
        public ScreenObject ScrObject { get; private set; }

        public ConfirmController(ConfirmModel m)
            : base(m)
        {
            model = m;
        }

        public override void HandleKey(string key)
        {
            switch (key)
            {
                default: break;
            }
        }

        public override void HandleTouch(int x, int y)
        {
            var scrObj = model.FindScreenObject(x, y);

            HandleButtonClick(scrObj);
        }

        private void HandleButtonClick(ScreenObject scrObj)
        {
            if (scrObj != null)
            {
                switch (scrObj.Type)
                {
                    case ScreenObject.ScreenObjectType.SCROLL:
                        if ((scrObj).ObjectState == ScreenObject.ScreenObjectState.ACTIVE)
                        {
                            ScrObject = scrObj;
                            ScrollContainer(model, model.GetView().Container, ScrollStepTypes.ConfirmViewScrollStep,
                                            scrObj.Data, (string)model.Vars["more"]);
                            model.GetView().TimerCount = 0;
                            model.GetView().GetTimer().Start();
                        }
                        break;
                    default:
                        if (scrObj.Visible)
                        {
                            switch (scrObj.Name)
                            {
                                case "btn_back":
                                    AppManager.Instance.CancelVote();
                                    break;
                                case "btn_print_review":
                                    AppManager.Instance.PrintReview();
                                    break;
                                case "btn_cast_vote":
                                    AppManager.Instance.SetScreen("thankyou");
                                    break;
                                case "btn_setting_tools":
                                    AppManager.Instance.SetScreen("settingTools", "Confirm");
                                    break;
                                case "btn_callhelp":
                                    string sHelpPath = AppManager.Configuration["HelpFile"]["Path"];

                                    if (sHelpPath != "")
                                    {
                                        try
                                        {
                                            System.Diagnostics.Process.Start(sHelpPath);
                                        }
                                        catch
                                        {
                                        }
                                    }

                                    break;
                            }
                        }

                        break;
                }
            }
        }

        public override void HandleMouseUp()
        {
            var timer = model.GetView().GetTimer();
            if (timer != null)
                timer.Stop();
            ScrObject = null;
        }

        public override void HandleSpeech(string recogWord)
        {
            var srcObject = model.FindClickableObjectByTextOrTag(recogWord);
            if (srcObject != null)
            {
                HandleButtonClick(srcObject);
            }            
        }
    }
}
