// Product:	VotRite
// Module:  ContestController.cs
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
    class ContestController : ScreenController
    {
        private new ContestModel model;
        public ScreenObject ScrObject { get; private set; }

        public ContestController(ContestModel m)
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
            ScreenObject scrObj = model.FindScreenObject(x, y);

            if (scrObj != null)
            {
                switch (scrObj.Type)
                {
                    case ScreenObject.ScreenObjectType.CONTAINER:
                        ScreenObject o = ((VrContainer)scrObj).FindScreenObject(x, y);
                        SetObjectSelection(o);
                        break;
                    default:
                        HandleButtonClick(scrObj);
                        break;
                }
            }
        }

        private void SetObjectSelection(ScreenObject item)
        {
            if (item != null && item.Enabled)
            {
                if (item.Type == ScreenObject.ScreenObjectType.SELECTION)
                    model.GetView().SetSelection((VrSelection)item);
            }
        }

        private void HandleButtonClick(ScreenObject scrObj)
        {
            if (scrObj == null)
                return;
            switch (scrObj.Type)
            {
                case ScreenObject.ScreenObjectType.SCROLL:
                    if ((scrObj).ObjectState == ScreenObject.ScreenObjectState.ACTIVE)
                    {
                        ScrObject = scrObj;
                        ScrollContainer(model, model.GetView().Container, ScrollStepTypes.ContestViewScrollStep,
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
                            case "btn_next":
                                if (model.Ballot.ActiveContest < model.Ballot.ContestsList.Count - 1)
                                {
                                    ++model.Ballot.ActiveContest;
                                    AppManager.Instance.SetScreen("contest");
                                }
                                else
                                    AppManager.Instance.SetScreen("review");
                                break;
                            case "btn_back":
                                --model.Ballot.ActiveContest;
                                AppManager.Instance.SetScreen("contest");
                                break;
                            case "btn_review":
                                AppManager.Instance.SetScreen("review");
                                break;
                            case "btn_setting_tools":
                                AppManager.Instance.SetScreen("settingTools", "Contest");
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

        public override void HandleMouseUp()
        {
            var timer = model.GetView().GetTimer();
            if (timer != null)
                timer.Stop();
            ScrObject = null;
        }

        public override void HandleMouseDown_Left()
        {
            ScrObject = null;
        }

        public override void HandleMouseDown_Right()
        {
            ScrObject = null;
        }

        public override void HandleSpeech(string recogWord)
        {
            var srcObject = model.FindClickableObjectByTextOrTag(recogWord);
            if (srcObject != null)
            {
                HandleButtonClick(srcObject);
                return;
            }
            var container = (VrContainer)model.FindScreenObject(ScreenObject.ScreenObjectType.CONTAINER);
            if (container != null)
            {
                string sCorrectWord = recogWord.Replace("say here", "Touch here");
                sCorrectWord = AppManager.ReplaceNumberToEnglish(sCorrectWord, false);
                srcObject = container.FindClickableObjectByTextOrTag(sCorrectWord, true);
                SetObjectSelection(srcObject);
            }
        }
    }
}
