// Product:	VotRite
// Module:  ReviewController.cs
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
using System;
using VotRite.JimForms;
using VotRite.Models;
using VotRite.UI;
using VotRiteBallotDataManager.AppCode;

namespace VotRite.Controllers
{
    class ReviewController : ScreenController
    {
        private new ReviewModel model;
        public ScreenObject ScrObject { get; private set; }

        public ReviewController(ReviewModel m)
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
                        var o = ((VrContainer)scrObj).FindScreenObject(x, y);
                        HandleContainerClick(o);
                        break;
                    default:
                        HandleButtonClick(scrObj);
                        break;
                }
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
                        if (scrObj.Name == "ScrollToTop")
                            ScrollContainerToTop(model, model.GetView().Container, ScrollStepTypes.ReviewViewScrollStep,
                                        scrObj.Data, (string)model.Vars["more"]);
                        else
                            ScrollContainer(model, model.GetView().Container, ScrollStepTypes.ReviewViewScrollStep,
                                            scrObj.Data, (string)model.Vars["more"]);
                        model.GetView().TimerCount = 0;
                        model.GetView().GetTimer().Start();
                    }
                    return;
                default:
                    if (scrObj.Visible)
                    {
                        switch (scrObj.Name)
                        {
                            case "btn_back_to_vote":
                                //if (model.Ballot.ElectionType == VotRiteBallotDataManager.AppCode.ElectionTypes.ranking_choice)
                                model.Ballot.ActiveContest = 0;
                                if ((this.model.Ballot.ElectionType == VotRiteBallotDataManager.AppCode.ElectionTypes.ranking_choice ? true :
                                    this.model.Ballot.ContestsList[this.model.Ballot.ActiveContest < this.model.Ballot.ContestsList.Count ? this.model.Ballot.ActiveContest : this.model.Ballot.ActiveContest - 1].Type == ContestTypes.V))
                                {
                                    //model.Ballot.ActiveContest = 0;
                                    AppManager.Instance.SetScreen("RankingChoice");
                                }
                                else if (model.Ballot.slatesDefinition.Slates.Count > 0)
                                {
                                   // model.Ballot.ActiveContest = 0;
                                    AppManager.Instance.SetScreen("slates");
                                }
                                else
                                    AppManager.Instance.SetScreen("contest");
                                break;
                            case "btn_print_review":
                                AppManager.Instance.PrintReview();
                                break;
                            case "btn_reset_ballot":
                                AppManager.Instance.ResetBallot();
                                break;
                            case "btn_accept_print":
                                string enabl = AppManager.Configuration["Printer"]["Enabled"];
                                if (AppManager.PrinterEnabled == false)
                                {
                                    AppManager.PrinterEnabled = true;
                                    AppManager.Instance.printRecordImageonly = true;
                                }
                                AppManager.Instance.PrintRecord();
                                bool vbal = false;
                                if(bool.TryParse(AppManager.Configuration["Printer"]["Enabled"], out vbal))
                                    AppManager.PrinterEnabled = vbal;
                                AppManager.Instance.printRecordImageonly = false;
                                break;
                            case "btn_audio":
                                AppManager.Instance.SetScreen("speechRate", "Review");
                                break;
                            case "btn_textsize":
                                AppManager.Instance.SetScreen("textSize", "Review");
                                break;
                            case "btn_setting_tools":
                                AppManager.Instance.SetScreen("settingTools", "Review");
                                break;
                            case "btn_callhelp":
                                AppManager.Instance.SetScreen("ballotoverview", "Review");
                                break;

                            case "btn_Font++":
                                //if (AppManager.Instance.reviewResultFontSize < 28)
                                //{
                                //    AppManager.Instance.reviewHeaderFontSize += 2;
                                //    AppManager.Instance.reviewResultFontSize += 2;
                                //    AppManager.Instance.SetScreen("Review");
                                //}
                                AppManager.Instance.reviewResultFontSize = 28;
                                AppManager.Instance.reviewHeaderFontSize = 30;
                                AppManager.Instance.SetScreen("Review");

                                break;
                            case "btn_Font--":
                                AppManager.Instance.reviewResultFontSize = 18;
                                AppManager.Instance.reviewHeaderFontSize = 22;
                                AppManager.Instance.SetScreen("Review");
                                //if (AppManager.Instance.reviewResultFontSize > 10)
                                //{
                                //    AppManager.Instance.reviewHeaderFontSize -= 2;
                                //    AppManager.Instance.reviewResultFontSize -= 2;
                                //    AppManager.Instance.SetScreen("Review");
                                //}
                                break;
                            case "btn_Selections":
                                ReviewScreenSelections frm = new ReviewScreenSelections();
                                frm.ShowDialog();
                                break;

                        }
                    }
                    break;
            }
        }

        private void HandleContainerClick(ScreenObject item)
        {
            if (item != null)
            {
                if (item.Type == ScreenObject.ScreenObjectType.SELECTION)
                {
                    if (item.Tag == "slate")
                    {
                        model.Ballot.ActiveContest = Convert.ToInt16(item.Data);
                        AppManager.Instance.SetScreen("slates");
                    } else if (item.Data == "ranking_choice")
                    {
                        model.Ballot.ActiveContest = 0;
                        AppManager.Instance.SetScreen("RankingChoice");
                    }
                    else
                    {
                        model.Ballot.ActiveContest = Convert.ToInt16(item.Data);
                        AppManager.Instance.SetScreen("contest");
                    }
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
            if (recogWord == "ScrollToTop")
            {
                srcObject = model.FindClickableObjectByTextOrTag("Scroll up");
                if (srcObject != null)
                    srcObject.Name = "ScrollToTop";
            }
            if (srcObject != null)
            {
                HandleButtonClick(srcObject);
                return;
            }
            var container = (VrContainer)model.FindScreenObject(ScreenObject.ScreenObjectType.CONTAINER);
            if (container != null)
            {
                srcObject = container.FindClickableObjectByTextOrTag(recogWord);
                HandleContainerClick(srcObject);
            }
        }

    }
}
