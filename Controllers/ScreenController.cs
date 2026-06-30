// Product:	VotRite
// Module:  ScreenController.cs
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
using VotRite.MVC;
using VotRite.Models;
using VotRite.UI;
using VotRite.Util;

namespace VotRite.Controllers
{
	class ScreenController : Controller
	{
		public ScreenController(ScreenModel m) : base(m) { }
		
		public override void Update (ISubject subj)
		{
			base.Update(subj);
		}	
		
		public override void HandleTouch (int x, int y)
		{
			base.HandleTouch(x, y);
			AppManager.Instance.UpdateSession();
		}

        public void ScrollContainer(ScreenModel model, VrContainer container, ScrollStepTypes scrollStepType, string dir, string buttonText)
        {
            try
            {
                var upScroll = model.Definition.ScreenObjects[model.Definition.ScreenObjects.Count - 2];
                var downScroll = model.Definition.ScreenObjects[model.Definition.ScreenObjects.Count - 1];

                var upScrollStateOld = upScroll.ObjectState;
                var downScrollStateOld = downScroll.ObjectState;

                if ((dir == "down" && downScroll.ObjectState == ScreenObject.ScreenObjectState.ACTIVE) ||
                    (dir == "up" && upScroll.ObjectState == ScreenObject.ScreenObjectState.ACTIVE))
                {
                    container.ScrollContest(dir, scrollStepType);
                }

                if (AppManager.Instance.backgroundTheme != AppManager.colorTheme.White)
                {
                    var img = ((VrButton)upScroll).BgImage;
                    var nimg = img.Substring(0, img.Length - 4) + "_" + AppManager.Instance.backgroundTheme + ".png";
                    ((VrButton)upScroll).BgImage = nimg;

                    img = ((VrButton)downScroll).BgImage;
                     nimg = img.Substring(0, img.Length - 4) + "_" + AppManager.Instance.backgroundTheme + ".png";
                    ((VrButton)downScroll).BgImage = nimg;
                }

                if (upScrollStateOld != upScroll.ObjectState)
                {
                    if (upScroll.ObjectState == ScreenObject.ScreenObjectState.ACTIVE)
                        upScroll.Text = buttonText;
                    model.UpdateObject(upScroll);
                }

                if (downScrollStateOld != downScroll.ObjectState)
                {
                    if (downScroll.ObjectState == ScreenObject.ScreenObjectState.ACTIVE)
                        downScroll.Text = buttonText;
                    model.UpdateObject(downScroll);
                }
            }
            catch (Exception e)
            {
                Logger.Instance.Write(e);
            }
        }

        public void ScrollContainerToTop(ScreenModel model, VrContainer container, ScrollStepTypes scrollStepType, string dir, string buttonText)
        {
            try
            {
                var upScroll = model.Definition.ScreenObjects[model.Definition.ScreenObjects.Count - 2];
                var downScroll = model.Definition.ScreenObjects[model.Definition.ScreenObjects.Count - 1];

                var upScrollStateOld = upScroll.ObjectState;
                var downScrollStateOld = downScroll.ObjectState;

                if ((dir == "down" && downScroll.ObjectState == ScreenObject.ScreenObjectState.ACTIVE) ||
                    (dir == "up" && upScroll.ObjectState == ScreenObject.ScreenObjectState.ACTIVE))
                {
                    container.ScrollContestToTop(dir, scrollStepType);
                }

                if (AppManager.Instance.backgroundTheme != AppManager.colorTheme.White)
                {
                    var img = ((VrButton)upScroll).BgImage;
                    var nimg = img.Substring(0, img.Length - 4) + "_" + AppManager.Instance.backgroundTheme + ".png";
                    ((VrButton)upScroll).BgImage = nimg;

                    img = ((VrButton)downScroll).BgImage;
                    nimg = img.Substring(0, img.Length - 4) + "_" + AppManager.Instance.backgroundTheme + ".png";
                    ((VrButton)downScroll).BgImage = nimg;
                }

                if (upScrollStateOld != upScroll.ObjectState)
                {
                    if (upScroll.ObjectState == ScreenObject.ScreenObjectState.ACTIVE)
                        upScroll.Text = buttonText;
                    model.UpdateObject(upScroll);
                }

                if (downScrollStateOld != downScroll.ObjectState)
                {
                    if (downScroll.ObjectState == ScreenObject.ScreenObjectState.ACTIVE)
                        downScroll.Text = buttonText;
                    model.UpdateObject(downScroll);
                }
            }
            catch (Exception e)
            {
                Logger.Instance.Write(e);
            }
        }


    }
}
