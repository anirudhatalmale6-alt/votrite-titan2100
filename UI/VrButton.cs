// Product:	VotRite
// Module:  VrButton.cs
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
using System.Drawing;
using VotRite.Util;

namespace VotRite.UI
{
    class VrButton : ScreenObject
    {
        private string bgImageName;
        private Image bgImage;
        private ScreenObjectState state;

        public string BgImage
        {
            get { return bgImageName; }
            set { bgImageName = value; }
        }

        public ScreenObjectState State
        {
            get { return state; }
            set
            {
                if (value == ScreenObject.ScreenObjectState.ACTIVE)
                    bgImageName = bgImageName.Replace("_d.", "_a.");
                else
                    bgImageName = bgImageName.Replace("_a.", "_d.");
                state = value;
            }
		}

        public VrButton() : base() { Type = ScreenObject.ScreenObjectType.BUTTON; }

        public override void Draw(Graphics gr)
        {
			if (FitToText) {
				var textFont = new Font(TextFont.FontFamily, TextSize, FontStyle.Regular);
				var oldWidth = Width;
				int textSize = (int)gr.MeasureString(Text, textFont).Width;
				int textSizeWithPaddings = textSize + LeftPadding + RightPadding;
				Width = ScreenObject.GenerateTextObjectWidth(Width, MaxWidth, textSizeWithPaddings);

				if (IsRightSide)
					Left -= (Width - oldWidth);
			}

            base.Draw(gr);
            try
            {
                var bgpath = AppManager.GetPathToCommonFile(bgImageName);
                using (bgImage = Image.FromFile(bgpath))
                    gr.DrawImage(bgImage, Left, Top, Width, Height);

                PrintText(gr);
            }
            catch (Exception e)
            {
                Logger.Instance.Write(e);
            }
        }
    }
}