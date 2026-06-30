// Product:	VotRite
// Module:  ScreenDefinition.cs
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
using System.Collections.Generic;
using VotRite.UI;

namespace VotRite.Definition
{
    class ScreenDefinition: IDisposable
    {
        private string name;
        private string view;
        private string background;
        private string bgColor;
        private int borderWidth;
        private string borderColor;
        private int width;
        private int height;
        private int top;
        private int left;
        private bool dialog;
        private bool resized;
        private List<ScreenObject> screenObjects;

        public string Name { get { return name; } set { name = value; } }
        public string View { get { return view; } set { view = value; } }
        public string Background
        {
            get { return background; }
            set { background = value; }
        }
        public string BgColor
        {
            get { return bgColor; }
            set { bgColor = value; }
        }
        public int BorderWidth
        {
            get { return borderWidth; }
            set { borderWidth = value; }
        }
        public string BorderColor
        {
            get { return borderColor; }
            set { borderColor = value; }
        }
        public int Width { get { return width; } set { width = value; } }
        public int Height { get { return height; } set { height = value; } }
        public int Top { get { return top; } set { top = value; } }
        public int Left { get { return left; } set { left = value; } }
        public bool IsDialog { get { return dialog; } set { dialog = value; } }
        public bool Resized { get { return resized; } set { resized = value; } }
        public List<ScreenObject> ScreenObjects
        {
            get { return screenObjects; }
            set { screenObjects = value; }
        }

        public ScreenDefinition()
        {
            screenObjects = new List<ScreenObject>();
        }

        // Jim Kapsis.
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        // Jim Kapsis.
        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (screenObjects != null) screenObjects.Clear(); //ForEach(sObj => sObj.Dispose());
            }
        }
    }
}