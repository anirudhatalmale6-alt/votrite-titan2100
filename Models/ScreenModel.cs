// Product:	VotRite
// Module:  ScreenModel.cs
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
using VotRite.MVC;
using VotRite.Definition;
using VotRite.UI;
using VotRite.Util;

namespace VotRite.Models
{
    class ScreenModel : Model
    {
        private ScreenDefinition definition = null;
        private SizeF scale;
		private Bitmap screenShot = null;

        public ScreenDefinition Definition
        {
            get { return definition; } set { definition = value; }
        }
        public SizeF Scale { get { return scale; } set { scale = value; } }
		public Bitmap ScreenShot { get { return screenShot; } set { screenShot = value; } }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (definition != null) definition.Dispose();
                if (screenShot != null) screenShot.Dispose();
            }
            base.Dispose(disposing);
        }
		
        public ScreenObject FindScreenObject(string search)
        {
            ScreenObject scrObject = definition.ScreenObjects.Find(
                delegate(ScreenObject target)
                {
                    if (target != null)
                    {
                        if (target.Name == search) return true;
                    }
                    return false;
                });
            return scrObject;
        }

        public ScreenObject FindScreenObject(ScreenObject.ScreenObjectType search)
        {
            ScreenObject scrObject = definition.ScreenObjects.Find(
                delegate(ScreenObject target)
                {
                    if (target != null)
                    {
                        if (target.Type == search) return true;
                    }
                    return false;
                });
            return scrObject;
        }

        public System.Collections.Generic.List<ScreenObject> FindScreenObjects(ScreenObject.ScreenObjectType search)
        {
            System.Collections.Generic.List<ScreenObject> scrObjects = definition.ScreenObjects.FindAll(
                delegate (ScreenObject target)
                {
                    if (target != null)
                    {
                        if (target.Type == search) return true;
                    }
                    return false;
                });
            return scrObjects;
        }

        public ScreenObject FindScreenObject(int x, int y)
        {
            ScreenObject scrObject = definition.ScreenObjects.Find(
                delegate(ScreenObject target)
                {
                    if (target != null)
                    {
                        if (((x >= target.Left) &&
                            (x <= (target.Left + target.Width))) &&
                            ((y >= target.Top) &&
                            (y <= target.Top + target.Height))) return true;
                    }
                    return false;
                });
            return scrObject;
        }
        
        public ScreenObject FindClickableObjectByTextOrTag(string name)
        {
            var scrObject = definition.ScreenObjects.Find(
                delegate(ScreenObject target)
                {
                    if (target != null && (target is VrButton || target is VrSelection))
                    {
                        if (target.Text == name || target.Tag == name) return true;
                    }
                    return false;
                });
            return scrObject;
        }

		public void ReDraw(Graphics gr)
		{
		    try
		    {
		        foreach (IObserver obs in observers)
		        {
		            if (obs.GetType().BaseType.Name == "VrScreen")
		            {
		                ((VrScreen)obs).ReDraw(gr);
		                break;
		            }
		        }
		    }
		    catch (Exception e)
		    {
		        Logger.Instance.Write(e);
		    }
		}
		
		public void UpdateObject(ScreenObject o) 
		{ 
            //TODO: objRect not required
			Rectangle objRect = new Rectangle(o.Left, o.Top, o.Width, o.Height);
			o.GraphicsState = ScreenObject.ScreenObjectGraphicsState.CHANGED;
			o.Draw(Window.Instance.CreateGraphics());
		}

        public void InvalidateObject(ScreenObject o)
        {
            Rectangle objRect = new Rectangle(o.Left, o.Top, o.Width, o.Height);
            o.GraphicsState = ScreenObject.ScreenObjectGraphicsState.CHANGED;
            Window.Instance.Invalidate(objRect);
        }
    }
}
