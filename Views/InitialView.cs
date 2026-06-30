// Product:	VotRite
// Module:  InitialView.cs
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
using System.Threading;

using VotRite.UI;
using VotRite.MVC;
using VotRite.Models;
using VotRite.Controllers;
using VotRite.Util;

namespace VotRite.Views
{
	class InitialView : VrScreen
	{
        private InitialModel vModel { get { return (InitialModel)model; } }
		private VrLabel lblMessage;
		private VrLabel lblDot;
        private VrLabel lblCounter;
        private VrLabel lblLogCounter;
		
		public InitialView(InitialModel m, ScreenController c) : base(m, c)
		{
			model = m;
			//vModel.Definition.Background = "graphics\\initialBG_f1.png";
			lblMessage = (VrLabel)vModel.FindScreenObject("lbl_vr");
			lblDot = (VrLabel)vModel.FindScreenObject("lbl_dot");
			lblDot.Text = "";
            lblCounter = (VrLabel)vModel.FindScreenObject("lbl_counter");
            lblCounter.Text += AppManager.Instance.CastCounter;//.CastTotalCounter;

           // lblCounter.Text += "      Ballots casted: "+ AppManager.Instance.CastedBallotsCounter;

            lblLogCounter = (VrLabel)vModel.FindScreenObject("lbl_log_counter");
            lblLogCounter.Width = 350;
            lblLogCounter.Text += AppManager.Instance.LogCounter;
			bool _logEnabled = false;
			bool.TryParse(AppManager.Configuration["ShowLog"]["Enabled"], out _logEnabled);
			
			lblLogCounter.Visible = _logEnabled;
			lblMessage.Visible = false;
			lblDot.Visible = false;
			lblDot.BgColor = "#FFFFFF";
			lblCounter.BgColor = "#FFFFFF";
			lblCounter.ForeColor = "#1C0F98";

			/*if (AppManager.Instance.State != Global.AppState.STARTED)
                        {
                            vModel.Message = "Initializing...";
			
                            Thread tt = new Thread(new ThreadStart(ProbeSystem));
                            tt.Start();
                        }
                        else
                            vModel.Message = "Electronic Voting";*/

			AppManager.Instance.State = Global.AppState.STARTED;
		}

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                lblMessage.Dispose();
                lblDot.Dispose();
                lblCounter.Dispose();
                lblLogCounter.Dispose();
            }
            base.Dispose(disposing);
        }
		
		private void ProbeSystem()
		{
			//Thread pt = new Thread(new ThreadStart(UpdateProgress));
			//pt.Start();
			
			//ProbeDb();
			//Thread.Sleep(2000);
			
			vModel.Message = "Electronic Voting";
			//Test();
			
			//pt.Abort();
			//lblDot.Text = "";
			//vModel.UpdateObject(lblDot);
		}
		
		/*private void ProbeDb()
		{
			vModel.Message = "Probing db...";
			vModel.Message += DataManager.VotingContentDataInstance.Probe();
		}*/
		
		private void Test()
		{
			//vModel.Message = "Test";
		}
		
		public override void Update(ISubject subj)
		{
			if (lblMessage != null) 
			{
				lblMessage.Text = vModel.Message;
				vModel.UpdateObject(lblMessage);
			}
		}
		
		private void UpdateProgress()
		{
			/*int left = lblDot.Left;
			int min = left;
			int right = left + 100;
			int dir = 0;*/
			string[] chars = { "|", "/", "--", "\\", "|", "/", "--", "\\" };
			int i = 0;
			
			while (true)
			{
				/*lblDot.Left = left;
				Thread.Sleep(10);
				
				if (dir == 1) left--;
				if (dir == 0) left++;
				
				if (left >= right) 
					dir = 1;
				else if(left <= min) 
					dir = 0;*/
				
				lblDot.Text = chars[i];
				Thread.Sleep(50);
				i++;
				if (i >= chars.Length-1) i = 0;
				
				vModel.UpdateObject(lblDot);
			}
		}
	}
}
