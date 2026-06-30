// Product:	VotRite
// Module:  LocaleModel.cs
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
using System.Collections;
using VotRite.MVC;
using VotRite.Util;
using System.Collections.Generic;
using VotRiteBallotDataManager.AppCode;

namespace VotRite.Models
{
	class LocaleModel : ScreenModel
	{
		private List<Locale> localesList = new List<Locale>();

		public List<Locale> LocalesList
		{
			get { return localesList; }
		}
		
		public LocaleModel()
		{
			int defaultLocaleIndex;
			List<Locale> usedLocales = Locale.FetchUsedLocales(out defaultLocaleIndex, AppManager.BallotId);//IOHandler.GetSysVars();
			if(defaultLocaleIndex >= 0) {
				localesList.Add(usedLocales[defaultLocaleIndex]);
				usedLocales.RemoveAt(defaultLocaleIndex);
			}

			localesList.AddRange(usedLocales);
		}

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                localesList.ForEach(loc => loc.Dispose());
                localesList.Clear();
            }
            base.Dispose(disposing);
        }
	}
}
