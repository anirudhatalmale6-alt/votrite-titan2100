using System;

using VotRite.UI;
using VotRite.MVC;
using VotRite.Models;
using VotRite.Controllers;
using VotRite.Util;

namespace VotRite.Views
{
    class CountyView : VrScreen
    {
        private CountyModel vModel { get { return (CountyModel)model; } }

        public CountyView(CountyModel m, ScreenController c) : base(m, c)
		{
            model = m;

            Initialize();

            if (AppManager.Instance.backgroundTheme != AppManager.colorTheme.White)
            {
                vModel.Definition.Background = AppManager.GetPathToCommonFile("graphics" + Global.Instance.SLASH + "initialBG_f1_" + AppManager.Instance.backgroundTheme + ".png");
            }
            /*var textsToSpeak = GetAllTexts(false, false, false);
            foreach (var text in textsToSpeak.Where(text => text != null))
            {
                AppManager.Instance.StartSpeaker(text);
            }*/
        }

        private void Initialize()
        {
            int btnCountyHeight = 71;
            int btnCountySpace = 30;

            if (vModel.Counties.Count > 0)
            {
                VrLabel lblMessage = new VrLabel
                {
                    Top = 20,
                    Left = 50,
                    Height = 40,
                    Width = 300,
                    Text = "Counties",
                    ForeColor = "#fff",
                    TextSize = 22,
                    TextAlign = "left-middle"
                };
                vModel.Definition.ScreenObjects.Add(lblMessage);

                VrButton btnCounty;
                int top = ((int)((float)vModel.Definition.Height / (float)vModel.Scale.Height) - 
                    ((btnCountyHeight + btnCountySpace) * vModel.Counties.Count - btnCountySpace)) / 2;

                foreach (var t in vModel.Counties)
                {
                    btnCounty = new VrButton
                    {
                        Name = t.Name,
                        BgImage = AppManager.GetPathToCommonFile("graphics" + Global.Instance.SLASH +
                                                                                 "option_dark_d.png"),
                        Text = t.Name,
                        ForeColor = "#000000",
                        TextSize = 22,
                        TextAlign = "left-middle",
                        Width = 605,
                        Height = btnCountyHeight,
                        Left = 337,
                        Top = top,
                        Tag = t.Id.ToString()
                    };

                    try
                    {
                        btnCounty.ObjectData = t;
                    }
                    catch (Exception ex) { Logger.Instance.Write(ex); }

                    vModel.Definition.ScreenObjects.Add(btnCounty);

                    top += btnCounty.Height + btnCountySpace;
                }
            }

            CreateListOfCommands();
            AddSpeechToTextEngine();

        }

        public override void Update(ISubject subj)
        {
            //Logger.Instance.Write("county view updated");
        }
    }
}
