// Product:	VotRite
// Module:  LocaleView.cs
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
using System.Threading;
using VotRite.UI;
using VotRite.MVC;
using VotRite.Models;
using VotRite.Controllers;
using VotRite.Util;
using VotRiteBallotDataManager.AppCode;
using System.Speech.Recognition;

namespace VotRite.Views
{
	class LocaleView : VrScreen
	{
        private LocaleModel vModel { get { return (LocaleModel)model; } }

        private LocaleController vController { get { return (LocaleController)controller; } set { controller = value; } }

        //public LocaleView(LocaleModel m, vController c) : base(m, c)
        public LocaleView(LocaleModel m, LocaleController c) : base(m, c)
        {
            if (m.Definition.Background.StartsWith("CommonFiles\\"))
                m.Definition.Background = m.Definition.Background.Replace("CommonFiles\\", "");
            model = m;
            vController = c;
            AppManager.Instance._contestSelections.Clear();
            Initialize();

            AppManager.Instance.checkDoubleSpaceSetting(m.Definition.ScreenObjects, null);

            var textsToSpeak = GetAllTexts_Locale(false, false, false);
            //textsToSpeak.Add("say zero to repeat instruction");
            AppManager.Instance._speechWords = textsToSpeak;

            //foreach (var text in textsToSpeak.Where(text => text != null))
            //{
            //    AppManager.Instance.StartSpeaker(text);
            //}
            //Thread.Sleep(2000);
            //AddSpeechToTextEngine();
            //CreateListOfCommands();
            AppManager.Instance.repeatInstruction = AppManager.repeatInstructionCommand.none;            

            AppManager.Instance.initiateHearingMode();
            AppManager.Instance.controllerForHearingMode = vController;
            AppManager.Instance.instructionSpoken = false;
        }
		
		private void Initialize()
		{
			int btnLocaleHeight = 71;
			int btnLocaleSpace = 30;
            int maxPerCol = 8;
            int LocalesToTake = vModel.LocalesList.Count;

            //For Audio mode condition bellow
            if (AppManager.Instance.ballot.BallotMode == Session.BallotModes.Audio)
                LocalesToTake = 2;

                int cols = (int)Math.Ceiling((double)LocalesToTake / maxPerCol);

            
                

            if (AppManager.Instance.backgroundTheme != AppManager.colorTheme.White)
            {
                vModel.Definition.Background = AppManager.GetPathToCommonFile("graphics" + Global.Instance.SLASH + "initialBG_f_" + AppManager.Instance.backgroundTheme + ".png");
            }
            if (LocalesToTake > 0)
			{
                VrLabel lblMessage = new VrLabel
                {
                    Top = 20,
                    Left = 50,
                    Height = 40,
                    Width = 300,
                    Text = "Locale",
                    ForeColor = "#fff",
                    TextSize = 22,
                    TextAlign = "left-middle"
                };
                vModel.Definition.ScreenObjects.Add(lblMessage);

                VrButton btnLocale;
                if (AppManager.Instance.ballot.BallotMode != Session.BallotModes.Audio)
                {
                    btnLocale = new VrButton
                    {
                        Name = "btn_settings",
                        BgImage = "graphics" + Global.Instance.SLASH + "bg_dark.jpg",
                        //"button_green.jpg"),
                        Text = "--Click for Ballot Display Settings--",
                        ForeColor = "#ffffff",
                        //ForeColor = "#000",
                        TextSize = 22,
                        TextAlign = "center-middle",
                        Width = 450,
                        Height = 50,
                        Left = ((int)((float)vModel.Definition.Width / (float)vModel.Scale.Width)) / 2 + 145,
                        Top = 5,

                    };
                    try
                    {
                        btnLocale.ObjectData = "Settings";
                    }
                    catch (Exception ex) { Logger.Instance.Write(ex); }
                    vModel.Definition.ScreenObjects.Add(btnLocale);
                }

                int top = ((int)((float)vModel.Definition.Height / (float)vModel.Scale.Height) - ((btnLocaleHeight + btnLocaleSpace) * LocalesToTake - btnLocaleSpace)) / 2;
                int left = 337;
                int width = 605;
                
                if (cols > 1)
                {
                    top = ((int)((float)vModel.Definition.Height / (float)vModel.Scale.Height) - ((btnLocaleHeight + btnLocaleSpace) * maxPerCol - btnLocaleSpace)) / 2;
                    width = 280;
                    left = ((int)((float)vModel.Definition.Width / (float)vModel.Scale.Width) - ((width + btnLocaleSpace) * cols)) / 2;
                }

                int col = (cols > 1) ? 1 : 0;

                foreach (var t in vModel.LocalesList)
                {
                   //For Audio mode condition bellow
                    if (AppManager.Instance.ballot.BallotMode == Session.BallotModes.Audio && (t.Name != "English" && t.Name != "Spanish"))
                        continue;

                        btnLocale = new VrButton
                        {
                            Name = t.Name,
                            BgImage = "graphics" + Global.Instance.SLASH + "button_green.jpg",
                        //    BgImage = AppManager.GetPathToCommonFile("graphics" +
                       //Global.Instance.SLASH + "bg_dark.png"),
                            Text = t.Phrase,
                            ForeColor = "#ffffff",
                            TextSize = 22,
                            TextAlign = "center-middle",
                            Width = width,
                            Height = btnLocaleHeight,
                            Left = left,
                            Top = top
                        };

                    try
                    {
                        btnLocale.ObjectData = t;
                    }
                    catch (Exception ex) { Logger.Instance.Write(ex); }

                    vModel.Definition.ScreenObjects.Add(btnLocale);

                    top += btnLocale.Height + btnLocaleSpace;

                    if (col == maxPerCol)
                    {
                        col = 1;
                        top = ((int)((float)vModel.Definition.Height / (float)vModel.Scale.Height) - ((btnLocaleHeight + btnLocaleSpace) * maxPerCol - btnLocaleSpace)) / 2;
                        left = left + width + btnLocaleSpace; 
                    } else
                    {
                        ++col;
                    }
                }
			}
            // var additionalSet = new System.Collections.Generic.HashSet<string> { "next", "review" };
            AppManager.Instance.repeatInstruction = AppManager.repeatInstructionCommand.norepeat;
            CreateListOfCommands(null);
            AddSpeechToTextEngine();
            AppManager.Instance.choicesLeft = 0;
            // AppManager.Instance.SetSpeechToTextEngine();
            //AppManager.Instance.OpenSession();
        }
		
		public override void Update(ISubject subj)
		{
			//Logger.Instance.Write("locale view updated");
            
		}

        

       
    }
}
