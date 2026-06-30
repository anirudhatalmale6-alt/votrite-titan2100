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
			int index = -1;
			switch (key)
			{
				case "D1": case "NumPad1": index = 0; break;
				case "D2": case "NumPad2": index = 1; break;
				case "D3": case "NumPad3": index = 2; break;
				case "D4": case "NumPad4": index = 3; break;
				case "D5": case "NumPad5": index = 4; break;
				case "D6": case "NumPad6": index = 5; break;
				case "D7": case "NumPad7": index = 6; break;
				case "D8": case "NumPad8": index = 7; break;
				case "D9": case "NumPad9": index = 8; break;
				case "D0": case "NumPad0": index = 9; break;
				default: break;
			}
			if (index >= 0 && index < model.LocalesList.Count)
			{
				AppManager.Instance.SetLocale(model.LocalesList[index]);
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
