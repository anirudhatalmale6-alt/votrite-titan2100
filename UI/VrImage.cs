// Product:	VotRite
// Module:  VrImage.cs
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
using System.IO;
using System.Drawing;
using VotRite.Util;

namespace VotRite.UI
{
    class VrImage : ScreenObject
    {
        private string imageName;
        private Image image;

        public string ImageName
        {
            get { return imageName; }
            set { imageName = value; }
        }
		
		public VrImage() : base() { Type = ScreenObject.ScreenObjectType.IMAGE; }

        public override void Draw(Graphics gr)
        {
            base.Draw(gr);

            try
            {
                using (image = Image.FromFile(AppManager.GetPathToCommonFile(imageName)))
                    gr.DrawImage(image, Left, Top, Width, Height);
            }
            catch (IOException e) { Logger.Instance.Write(e); }
        }
    }
}