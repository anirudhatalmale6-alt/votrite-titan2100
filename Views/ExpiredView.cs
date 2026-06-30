// Product:	VotRite
// Module:  ExpiredView.cs
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
using VotRite.UI;
using VotRite.MVC;
using VotRite.Models;
using VotRite.Controllers;

namespace VotRite.Views
{
	class ExpiredView : VrScreen
	{
        private ExpiredModel vModel { get { return (ExpiredModel)model; } }
		
		public ExpiredView(ExpiredModel m, ScreenController c) : base(m, c)
		{
			model = m;
			
			Initialize();
		}
		
		private void Initialize()
		{
			
		}
		
		public override void Update(ISubject subj)
		{
			//Logger.Instance.Write("expired view updated");
		}
	}
}
