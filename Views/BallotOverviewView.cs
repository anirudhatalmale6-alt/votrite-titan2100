// Product:	VotRite
// Module:  BallotoverviewView.cs

// 
// Copyright (c) 2017 - VOTRITE INTERNATIONAL LLC. All rights reserved.
// 
// THIS PRODUCT INCLUDES SOFTWARE AS A PART OF VOTRITE VOTING 
// SYSTEM DEVELOPED AT VOTRITE INTERNATIONAL LLC (http://www.votrite.com).
// THE SOURCE CODE FOR THIS PRODUCT IS SUBJECT TO THE VOTRITE INTERNATIONAL LLC 
// LICENSE. NO ANY PORTION OF THIS PRODUCT OR SOURCE CODE CAN BE 
// REDISTRIBUTED UNDER ANY CIRCUMSTANCES.
// 
using System.Linq;

using PdfiumViewer;

using VotRite.UI;
using VotRite.Models;
using VotRite.Controllers;

namespace VotRite.Views
{
    class BallotoverviewView : VrScreen
    {
        private BallotoverviewModel vModel { get { return (BallotoverviewModel)model; } }

        private VrScreen prevScreen;

        public BallotoverviewView(BallotoverviewModel m, ScreenController c) : base(m, c)
        {
            model = m;

            if (AppManager.Instance.ActiveScreens.Count > 0)
            {
                prevScreen = AppManager.Instance.ActiveScreens[
                    AppManager.Instance.ActiveScreens.Count - 1];
            }

            Initialize();

            AppManager.Instance.checkDoubleSpaceSetting(vModel.Definition.ScreenObjects, null);

            var textsToSpeak = GetAllTexts(false, false, false);
            foreach (var text in textsToSpeak.Where(text => text != null))
            {
                AppManager.Instance.StartSpeaker(text);
            }
        }

        private void Initialize()
        {
            CreateListOfCommands();
            AddSpeechToTextEngine();

            PdfViewer pdf = new PdfViewer();
            pdf.Name = "pdfViewer";
            pdf.Width = 1280;
            pdf.Height = 605;
            pdf.Top = 60;
            pdf.Left = 0;
            pdf.ShowToolbar = false;
            pdf.Document = PdfDocument.Load(AppManager.Configuration["HelpFile"]["Path"]);
            pdf.ZoomMode = PdfViewerZoomMode.FitBest;
            pdf.Renderer.Zoom = 3;
            Window.Instance.Controls.Add(pdf);

            VrButton btnGoTo;

            if (prevScreen.Name == "pinpad")
            {
                btnGoTo = new VrButton
                {
                    Name = "btnNext",
                    BgImage = AppManager.GetPathToCommonFile("graphics" + Global.Instance.SLASH +
                                                                                 "forward_light_a.png"),
                    Text = "Next",
                    ForeColor = "#fff",
                    TextSize = 22,
                    TextAlign = "left-middle",
                    Width = 289,
                    Height = 63,
                    Left = 966,
                    Top = 954,
                    Action = ScreenObject.ScreenObjectAction.CONTINUE,
                    Data = "locale"
                };
            } else
            {
                btnGoTo = new VrButton
                {
                    Name = "btnBack",
                    BgImage = AppManager.GetPathToCommonFile("graphics" + Global.Instance.SLASH +
                                                                                 "back_light_d.png"),
                    Text = "Back",
                    ForeColor = "#fff",
                    TextSize = 22,
                    TextAlign = "center-middle",
                    Width = 289,
                    Height = 63,
                    Left = 30,
                    Top = 954,
                    Action = ScreenObject.ScreenObjectAction.GO_BACK,
                    Data = "empty"
                };
            }

            vModel.Definition.ScreenObjects.Add(btnGoTo);
        }

    }
}
