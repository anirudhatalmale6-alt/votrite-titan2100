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
using VotRite.MVC;
using VotRite.Models;
using VotRite.UI;
using VotRite.Views;

namespace VotRite.Controllers
{

	class PinpadController : ScreenController
	{
		public PinpadView ViewInstance { get; set; }
		public PinpadModel ModelInstance { get; set; }

		public PinpadController(PinpadModel m)
			: base(m)
		{
			ModelInstance = m;
		}

		public override void HandleTouch(int x, int y)
		{
			if (AppManager.useExternalKeyBoard)
				return;
			ScreenObject scrObj = ModelInstance.FindScreenObject(x, y);

			if (scrObj != null)
			{
				if (scrObj.Type == ScreenObject.ScreenObjectType.CONTAINER)
					scrObj = ((VrContainer)scrObj).FindScreenObject(x, y);

				if (scrObj != null) {
					if (!string.IsNullOrEmpty(scrObj.Data) && ViewInstance != null)
						ViewInstance.DataReceived(Convert.ToChar(Convert.ToInt32(scrObj.Data)));
				}
			}
		}

        public override void HandleMouseUp()
        {
            base.HandleMouseUp();
        }

        public override void HandleKey(string key)
        {
			char c = '\r';
			if (key.StartsWith("D"))
				c = Convert.ToChar(key.Replace("D", ""));
			if (key == "Return")
				c = '\r';
			if (key == "Back")
				c = '\b';

			ViewInstance.DataReceived( c);
		}

    }
}
