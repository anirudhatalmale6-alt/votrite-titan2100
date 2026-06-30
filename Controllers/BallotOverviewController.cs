// Product:	VotRite
// Module:  BallotoverviewController.cs
// 
// Copyright (c) 2017 - VOTRITE INTERNATIONAL LLC. All rights reserved.
// 
// THIS PRODUCT INCLUDES SOFTWARE AS A PART OF VOTRITE VOTING 
// SYSTEM DEVELOPED AT VOTRITE INTERNATIONAL LLC (http://www.votrite.com).
// THE SOURCE CODE FOR THIS PRODUCT IS SUBJECT TO THE VOTRITE INTERNATIONAL LLC 
// LICENSE. NO ANY PORTION OF THIS PRODUCT OR SOURCE CODE CAN BE 
// REDISTRIBUTED UNDER ANY CIRCUMSTANCES.
// 

using System.Windows.Forms;

using VotRite.Models;
using VotRite.UI;

namespace VotRite.Controllers
{
    class BallotoverviewController : ScreenController
    {
        private new BallotoverviewModel model;
        private string mScreenName = "";

        public BallotoverviewController(BallotoverviewModel m, string pScreenName) : base(m)
        {
            model = m;
            mScreenName = pScreenName;
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
            if (scrObj == null) return;
            if (scrObj.Data == null) return;

            if (scrObj.Name == "btnBack")
            {
                scrObj.Data = mScreenName;
            }

            foreach (Control obj in Window.Instance.Controls)
            {
                if (obj.Name == "pdfViewer")
                {
                    Window.Instance.Controls.Remove((Control)obj);
                }
            }

            AppManager.Instance.SetScreen(scrObj.Data);
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
