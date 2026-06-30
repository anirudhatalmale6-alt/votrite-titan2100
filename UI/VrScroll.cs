// Product:	VotRite
// Module:  VrScroll.cs
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

namespace VotRite.UI
{
    class VrScroll : VrButton
    {
		public enum ScrollDirection { DOWN, UP }
		
        private const int SCROLL_WIDTH = 605;
        private const int SCROLL_HEIGHT = 70;

        private ScrollDirection direction;

        public VrScroll(ScrollDirection dir)
            : base()
        {
            Type = ScreenObject.ScreenObjectType.SCROLL;
            direction = dir;

            if (direction == VrScroll.ScrollDirection.DOWN)
                BgImage = AppManager.GetPathToCommonFile("graphics" + Global.Instance.SLASH + "more_down_a.png");
            if (direction == VrScroll.ScrollDirection.UP)
                BgImage = AppManager.GetPathToCommonFile("graphics" + Global.Instance.SLASH + "more_up_a.png");

            ForeColor = "#000000";
            Width = SCROLL_WIDTH;
            Height = SCROLL_HEIGHT;
            Text = "$more";
        }

        public override ScreenObject.ScreenObjectState ObjectState
        {
            get { return base.ObjectState; }
            set
            {
                base.ObjectState = value;

                if (value == ScreenObject.ScreenObjectState.INACTIVE)
                {
                    Text = "";
                    if (direction == VrScroll.ScrollDirection.DOWN)
                        BgImage = AppManager.GetPathToCommonFile("graphics" +
                            Global.Instance.SLASH + "more_down_d.png");
                    if (direction == VrScroll.ScrollDirection.UP)
                        BgImage = AppManager.GetPathToCommonFile("graphics" +
                            Global.Instance.SLASH + "more_up_d.png");
                }
                else
                {
                    if (direction == VrScroll.ScrollDirection.DOWN)
                        BgImage = AppManager.GetPathToCommonFile("graphics" +
                            Global.Instance.SLASH + "more_down_a.png");
                    if (direction == VrScroll.ScrollDirection.UP)
                        BgImage = AppManager.GetPathToCommonFile("graphics" +
                            Global.Instance.SLASH + "more_up_a.png");
                }
            }
        }
    }
}