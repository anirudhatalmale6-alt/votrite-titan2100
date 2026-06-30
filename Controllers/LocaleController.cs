// Product:	VotRite
// Module:  LocaleController.cs
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
using VotRiteBallotDataManager.AppCode;

namespace VotRite.Controllers
{
	class LocaleController : ScreenController
	{
		private new LocaleModel model;
		
		public LocaleController(LocaleModel m) : base(m)
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
		
		public override void HandleTouch (int x, int y)
		{
		    var scrObj = model.FindScreenObject(x, y);
		    HandleButtonClick(scrObj);
		}

	    private static void HandleButtonClick(ScreenObject scrObj)
	    {
			if (scrObj != null)
			{
				if (scrObj.ObjectData.ToString() == "Settings")
					AppManager.Instance.SetScreen("settingTools", "Locale");
				else
					AppManager.Instance.SetLocale(scrObj.ObjectData as Locale);
			}
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
