// Product:	VotRite
// Module:  ThankyouView.cs
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
using System.Linq;
using System.Timers;

using VotRite.UI;
using VotRite.Models;
using VotRite.Controllers;
using VotRite.Util;
using Timer = System.Timers.Timer;

namespace VotRite.Views
{

    class ThankyouView : VrScreen
    {
        private const int CAST_TIMEOUT = 2000;// 100;// 3000;// 10000;// 5000;

        private ThankyouModel vModel { get { return model as ThankyouModel; } }
        private VrLabel lblDone;
        private VrLabel lblThankyou;
        private Timer timer;
        private Timer initialTimer;

        //[DllImport("kernel32.dll", EntryPoint = "Beep", SetLastError = true, ExactSpelling = true)]

        //public static extern bool Beep(uint frequency, uint duration);

        public ThankyouView(ThankyouModel m, ScreenController c)
            : base(m, c)
        {
            Name = "thankyou";
            //model = m;
            timer = new Timer();

            timer.Interval = CAST_TIMEOUT;
            timer.Elapsed += new ElapsedEventHandler(CastTimeOut_Elapsed);
            
            lblDone = (VrLabel)vModel.FindScreenObject("lbl_done");
            lblThankyou = (VrLabel)vModel.FindScreenObject("lbl_thankyou");
            
            Initialize();            
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                timer.Dispose();
                lblDone.Dispose();
                lblThankyou.Dispose();
            }
            base.Dispose(disposing);
        }

        private void Initialize()
        {
            try
            {
                if (AppManager.Instance.backgroundTheme != AppManager.colorTheme.White)
                {
                    vModel.Definition.Background = AppManager.GetPathToCommonFile("graphics" + Global.Instance.SLASH + "initialBG_f1_" + AppManager.Instance.backgroundTheme + ".png");
                }

                if (lblDone != null)
                    lblDone.Text = (string)vModel.Vars["done"];
                if (lblThankyou != null)
                    lblThankyou.Text = (string)vModel.Vars["thankyou"];

                var textsToSpeak = GetAllTexts(false, false, false);
                foreach (var text in textsToSpeak.Where(text => text != null))
                {
                    AppManager.Instance.StartSpeaker(text);
                }

                //Jim Code -- Begin
                if (AppManager.VoteSoundEnabled)
                {
                    PlayFinishVoteAudio();
                }
                //Jim Code -- End


                AppManager.Instance.CastVote();
            }
            catch (Exception ex)
            {
                System.Windows.Forms.MessageBox.Show("Error occured in casting vote: "+ ex.Message,"Vote Casting Error", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error);
            }

            timer.Enabled = true;
        }

        private void PlayFinishVoteAudio()
        {

            //Beep(1000, 800);
            //Thread.Sleep(2000);

            //System.Media.SystemSounds.Hand.Play();
            //System.Media.SystemSounds.Asterisk.Play();
            System.Media.SystemSounds.Exclamation.Play();
            //System.Media.SystemSounds.Beep.Play();

           // ManagementObjectSearcher objSearcher = new ManagementObjectSearcher(
           //"SELECT * FROM Win32_SoundDevice");

           // ManagementObjectCollection objCollection = objSearcher.Get();


           // foreach (ManagementObject obj in objCollection)
           // {
           //     foreach (PropertyData property in obj.Properties)
           //     {
           //         Console.Out.WriteLine(String.Format("{0}:{1}", property.Name, property.Value));

           //         try
           //         {
           //             playSound(property.Name);
           //         }
           //         catch
           //         {
           //         }
           //     }
           // }
        }

        //private void playSound(string path)
        //{
        //    System.Media.SoundPlayer player =
        //        new System.Media.SoundPlayer();
        //    player.SoundLocation = path;
        //    player.Load();
        //    player.Play();
        //}



        private void CastTimeOut_Elapsed(object source, ElapsedEventArgs e)
        {
            try
            {
                timer.Enabled = false;
                AppManager.Instance.SetScreen("initial");
                /*Thread.Sleep(500);
                AppManager.Instance.ShowSoftPinpad("ballots", false);*/

                initialTimer = new Timer(4000);
                initialTimer.Elapsed += new ElapsedEventHandler(InitialScreenTime_Elapsed);
                initialTimer.Enabled = true;
            }
            catch (Exception exception)
            {
                Logger.Instance.Write(exception);
            }
			//AppManager.Instance.CloseSession();
			//PinpadView.Menu.GetMenu("main");
//			AppManager.Instance.ShowSoftPinpad();
        }

        private void InitialScreenTime_Elapsed(object sender, ElapsedEventArgs args)
        {
            initialTimer.Enabled = false;

            if (!Convert.ToBoolean(AppManager.Configuration["System"]["RemotePollWorker"]))
            {
                //AppManager.Instance.ShowSoftPinpad("ballots", false);
                AppManager.Instance.ShowSoftPinpad("main", false);
            }
        }
    }
}
