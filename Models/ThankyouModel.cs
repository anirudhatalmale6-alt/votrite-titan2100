// Product:	VotRite
// Module:  ThankyouModel.cs
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

namespace VotRite.Models
{
    class ThankyouModel : ScreenModel
    {
        private Hashtable vars;
        private Session session;

        public Hashtable Vars
        {
            get { return vars; }
        }

        public ThankyouModel()
        {
            vars = new Hashtable();
            session = AppManager.Instance.Session;

            Initialize();
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                vars.Clear();
                session.Dispose();
            }
            base.Dispose(disposing);
        }

        private void Initialize()
        {
            vars.Add("done", session.CurrentLocale.LblDone);
			vars.Add("thankyou", session.CurrentLocale.LblThankyou);
        }
    }
}
