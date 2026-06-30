// Product:	VotRite
// Module:  InitialController.cs
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
using VotRite.Models;

namespace VotRite.Controllers
{
	class InitialController : ScreenController
	{
        public InitialController(InitialModel m) : base(m)
		{
            Console.WriteLine(AppManager.Instance.GetActiveScreen());
            if (Convert.ToBoolean(AppManager.Configuration["System"]["RemotePollWorker"]) && 
                AppManager.Instance.GetActiveScreen() != null && 
                AppManager.Instance.GetActiveScreen().Name == "remotePollWorker")
            {
                AppManager.Instance.HPinPad = new HardPinPad(true);
            }

            if (Convert.ToBoolean(AppManager.Configuration["System"]["RemotePollWorker"]) && 
                AppManager.Instance.GetActiveScreen() != null && 
                (AppManager.Instance.GetActiveScreen().Name == "thankyou" || 
                AppManager.Instance.GetActiveScreen().Name == "review"))
            {
                AppManager.Instance.HPinPad.Port.Open();
                AppManager.Instance.HPinPad.Reset();
            }
        }

        public override void HandleKey(string key)
        {
            switch (key)
            {
                case "F1":
                    AppManager.Instance.OpenBallot(null, true);
                    break;
                case "F2":
                    AppManager.Instance.PrintReport();
                    break;
                case "F3":
                    AppManager.Instance.ResetBallot();
                    break;
                case "F4":
                    AppManager.Instance.Terminate();
                    break;
                case "F5":
                    AppManager.Instance.ShowTenants2();
                    break;
                default:
                    break;
            }
        }
	}
}
