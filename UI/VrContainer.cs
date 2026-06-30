// Product:	VotRite
// Module:  VrContainer.cs
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
using System.Collections.Generic;
using System.Linq;
using VotRite.Models;
using VotRite.Util;
using VotRite.UI;

namespace VotRite.UI
{
    class VrContainer : ScreenObject
    {
        private ScreenModel model = null;
        private List<ScreenObject> controls;
        VrScroll upScroll = null;
        VrScroll downScroll = null;
        private static int? scrollStep = null;
        private static readonly Dictionary<ScrollStepTypes, int> Step = new Dictionary<ScrollStepTypes, int>();
        private static readonly Dictionary<ScrollTimerTypes, int> TimerIn = new Dictionary<ScrollTimerTypes, int>();

        private const int DOWN_SCROLL_TOP = 860;

        public List<ScreenObject> Controls
        {
            get { return controls; }
            set { controls = value; }
        }

        public VrContainer()
        {
            Type = ScreenObject.ScreenObjectType.CONTAINER;
            Controls = new List<ScreenObject>();
        }

        public VrContainer(int? scrollstep)
        {
            Type = ScreenObject.ScreenObjectType.CONTAINER;
            Controls = new List<ScreenObject>();
            scrollStep = scrollstep;
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (model != null) model.Dispose();
                if (upScroll != null) upScroll.Dispose();
                if (downScroll != null) downScroll.Dispose();
                controls.ForEach(scrObj => scrObj.Dispose());
            }
            base.Dispose(disposing);
        }

        public override void Draw(Graphics gr)
        {
            base.Draw(gr);

            if (controls != null)
            {
                foreach (ScreenObject scrObj in controls)
                {
                    if (model != null)
                    {
                        if (!scrObj.Resized) scrObj.Resize(model.Scale);
                    }
                    scrObj.Draw(gr);
                }
            }
        }

        public ScreenObject FindScreenObject(int x, int y)
        {
            ScreenObject scrObject = controls.Find(
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

        public ScreenObject FindControlByText(string search)
        {
            var scrObject = Controls.Find(
                delegate(ScreenObject target)
                    {
                        if (target != null)
                        {
                            if (target.Text == search) return true;
                        }
                        return false;
                    });
            return scrObject;
        }

        public ScreenObject FindControlByName(string search)
        {
            var scrObject = Controls.Find(
                delegate (ScreenObject target)
                {
                    if (target != null)
                    {
                        if (target.Name == search) return true;
                    }
                    return false;
                });
            return scrObject;
        }

        public ScreenObject FindClickableObjectByTextOrTag(string name)
        {
            var scrObject = Controls.Find(
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

        public ScreenObject FindClickableObjectByTextOrTag(string name, bool pIsHavingNumberToEnglish)
        {
            //foreach (ScreenObject so in controls)
            //{
            //    string sName = so.Text;
            //}

            var scrObject = Controls.Find(
                delegate(ScreenObject target)
                {
                    if (target != null && (target is VrButton || target is VrSelection))
                    {
                        if (target.Text.Replace(" ", "").Replace("-", "") == name.Replace(" ", "").Replace("-", "") || target.Tag.Replace(" ", "").Replace("-", "") == name.Replace(" ", "").Replace("-", "")) return true;
                    }
                    return false;
                });
            return scrObject;
        }

		public bool CreateScroll(ScreenModel m, int totalHeight)
        {
            model = m;
            //int totalHeight = GetTotalHeight();

            if (totalHeight > Height)
            {
                int contHeight = Height;
                int leftPos = Left;

                upScroll = new VrScroll(VrScroll.ScrollDirection.UP);
                upScroll.Name = "upScroll";
                upScroll.Data = "up";
                upScroll.ObjectState = ScreenObject.ScreenObjectState.INACTIVE;
                upScroll.Top = Top; //210;
				upScroll.Width = Width;

                if (Align == "center-middle")
                    leftPos = (Width - upScroll.Width) / 2;

                upScroll.Left = leftPos;
                upScroll.TextSize = 22;
                model.InvalidateObject(upScroll);

                downScroll = new VrScroll(VrScroll.ScrollDirection.DOWN);
                downScroll.BgColor = "#cccccc";
                downScroll.Data = "down";
                downScroll.Name = "downScroll";
                downScroll.ObjectState = ScreenObject.ScreenObjectState.ACTIVE;
                downScroll.Top = DOWN_SCROLL_TOP;
                downScroll.Left = leftPos;
				downScroll.TextSize = 22;
				downScroll.Width = Width;

                model.Definition.ScreenObjects.Add(upScroll);
                model.Definition.ScreenObjects.Add(downScroll);

				return true;
            }

			return false;
        }

        private int GetTotalHeight()
        {
            var totalHeight = controls.Sum(control => control.Height + ScreenObject.SCREEN_V_SPACE);
            totalHeight -= ScreenObject.SCREEN_V_SPACE;
            return totalHeight;
        }

        public void ScrollContest(string dir, ScrollStepTypes scrollType)
        {
            try
            {                
                var step = GetScrollStep(scrollType);

                if (step <= 0 && controls.Count > 0) {
                    ScreenObject firstInvisible = null;
                    if (dir == "down") {
                        var lastVisible = controls.FindLast(el => el.Visible);
                        if (lastVisible != null && controls.IndexOf(lastVisible) < controls.Count - 1)
                            firstInvisible = controls[controls.IndexOf(lastVisible) + 1];

                        if(firstInvisible != null)
                            step = firstInvisible.Top - (Top + 5);
                    }
                    else {
                        var firstVisible = controls.Find(el => el.Visible);
                        if (firstVisible != null && controls.IndexOf(firstVisible) > 0)
                            firstInvisible = controls[controls.IndexOf(firstVisible) - 1];

                        if(firstInvisible != null)
                            step = Top + Height - 5 - firstInvisible.Height - firstInvisible.Top;

                        if(controls[0].Top + step > Top + 5)
                            step = controls[0].Top - (Top + 5);
                    }
                }

                if (step < 0)
                    step *= -1;

                var cnt = 0;
                int MaxH = controls.Count > 0 ? controls[0].Top : 0 ;
                int MinH = controls.Count > 0 ? controls[0].Top : 0 ;
                ScreenObject maxObj = null, minObj = null;

                foreach (var obj in controls)
                {

                    /*if (dir == "down")
                        obj.Top -= step;
                    else
                        obj.Top += step;
                    */
                    obj.Top += (dir == "down") ? -step : step;
                    obj.Visible = ((obj.Top < Top) || (obj.Top + obj.Height > Top + Height)) ? false : true;
                    
                    MaxH = Math.Max(MaxH, obj.Top);
                    MinH = Math.Min(MinH, obj.Top);
                    minObj = (MinH == obj.Top) ? obj : minObj ;
                    maxObj = (MaxH == obj.Top) ? obj : maxObj ;

                    //obj.Width = this.Width;

                    /*if ((obj.Top < Top) ||
                        (obj.Top + obj.Height > Top + Height))
                        obj.Visible = false;
                    else
                        obj.Visible = true;*/

                    if (cnt == 0)
                    {
                        upScroll.ObjectState = obj.Top < Top ? ScreenObjectState.ACTIVE : ScreenObjectState.INACTIVE;

                        //if (model != null)
                        //    model.UpdateObject(upScroll);
                    }

                    if ( cnt == controls.Count - 1)
                    {
                        downScroll.ObjectState = maxObj.Top + maxObj.Height + 15 > Top + Height ? ScreenObjectState.ACTIVE : ScreenObjectState.INACTIVE;

                        //if (model != null)
                        //    model.UpdateObject(downScroll);
                    }
                    
                    cnt++;
                }

                //if (maxObj.Visible) downScroll.ObjectState = maxObj.Top + maxObj.Height + 15 > Top + Height ? ScreenObjectState.ACTIVE : ScreenObjectState.INACTIVE;
                //if (minObj.Visible) upScroll.ObjectState = minObj.Top < Top ? ScreenObjectState.ACTIVE : ScreenObjectState.INACTIVE;

                if (dir == "down")
                {
                    
                }
                else
                {
                    
                }
                
                if (model != null)
                    model.InvalidateObject(this);
            }
            catch (Exception e)
            {
                Logger.Instance.Write(e);
            }
        }


        public void ScrollContestToTop(string dir, ScrollStepTypes scrollType)
        {
            try
            {
                var step = GetScrollStep(scrollType);

                if (step <= 0 && controls.Count > 0)
                {
                    ScreenObject firstInvisible = null;
                    if (dir == "down")
                    {
                        var lastVisible = controls.FindLast(el => el.Visible);
                        if (lastVisible != null && controls.IndexOf(lastVisible) < controls.Count - 1)
                            firstInvisible = controls[controls.IndexOf(lastVisible) + 1];

                        if (firstInvisible != null)
                            step = firstInvisible.Top - (Top + 5);
                    }
                    else
                    {
                        var firstVisible = controls.Find(el => el.Visible);
                        if (firstVisible != null && controls.IndexOf(firstVisible) > 0)
                            firstInvisible = controls[controls.IndexOf(firstVisible) - 1];

                        if (firstInvisible != null)
                            step = Top + Height - 5 - firstInvisible.Height - firstInvisible.Top;

                        if (controls[0].Top + step > Top + 5)
                            step = controls[0].Top - (Top + 5);
                    }
                }

                if (step < 0)
                    step *= -1;

                var cnt = 0;
                int MaxH = controls.Count > 0 ? controls[0].Top : 0;
                int MinH = controls.Count > 0 ? controls[0].Top : 0;
                ScreenObject maxObj = null, minObj = null;
                int objTop = Top + step;

                foreach (var obj in controls)
                {
                    //if (cnt == 0)                    
                    obj.Top = objTop;
                    objTop += step;
                    if (obj is VrSelection)
                        objTop += obj.Height;
                    //else
                    //    obj.Top += (dir == "down") ? -step : step;

                    obj.Visible = ((obj.Top < Top) || (obj.Top + obj.Height > Top + Height)) ? false : true;

                    MaxH = Math.Max(MaxH, obj.Top);
                    MinH = Math.Min(MinH, obj.Top);
                    minObj = (MinH == obj.Top) ? obj : minObj;
                    maxObj = (MaxH == obj.Top) ? obj : maxObj;

                    //obj.Width = this.Width;

                    /*if ((obj.Top < Top) ||
                        (obj.Top + obj.Height > Top + Height))
                        obj.Visible = false;
                    else
                        obj.Visible = true;*/

                    if (cnt == 0)
                    {
                        upScroll.ObjectState = obj.Top < Top ? ScreenObjectState.ACTIVE : ScreenObjectState.INACTIVE;

                        //if (model != null)
                        //    model.UpdateObject(upScroll);
                    }

                    if (cnt == controls.Count - 1)
                    {
                        downScroll.ObjectState = maxObj.Top + maxObj.Height + 15 > Top + Height ? ScreenObjectState.ACTIVE : ScreenObjectState.INACTIVE;

                        //if (model != null)
                        //    model.UpdateObject(downScroll);
                    }

                    cnt++;
                }

                //if (maxObj.Visible) downScroll.ObjectState = maxObj.Top + maxObj.Height + 15 > Top + Height ? ScreenObjectState.ACTIVE : ScreenObjectState.INACTIVE;
                //if (minObj.Visible) upScroll.ObjectState = minObj.Top < Top ? ScreenObjectState.ACTIVE : ScreenObjectState.INACTIVE;

                if (dir == "down")
                {

                }
                else
                {

                }

                if (model != null)
                    model.InvalidateObject(this);
            }
            catch (Exception e)
            {
                Logger.Instance.Write(e);
            }
        }

        public static void SetScrollStep(int? step)
        {
            scrollStep = step;
        }

        private static int GetScrollStep(ScrollStepTypes paramName)
        {
            int step;
            if (Step.TryGetValue(paramName, out step))
            {
                return step;
            }
			try
            {
                step = scrollStep == null ? Convert.ToInt16(AppManager.Configuration["Contest"][paramName.ToString()]) : (int)scrollStep;
                Step[paramName] = step;
            }
            catch (Exception e)
            {
                Logger.Instance.Write(e);
            }

			return step;
		}

        public static int GetScrollTimerIn(ScrollTimerTypes paramName)
        {
            int timerIn;
            if (TimerIn.TryGetValue(paramName, out timerIn))
            {
                return timerIn;
            }
            try
            {
                timerIn = Convert.ToInt16(AppManager.Configuration["Contest"][paramName.ToString()]);
                TimerIn[paramName] = timerIn;
            }
            catch (Exception e)
            {
                Logger.Instance.Write(e);
            }

            return timerIn;
        }
    }

	public enum ScrollStepTypes {
		ContestViewScrollStep,
		ReviewViewScrollStep,
		ConfirmViewScrollStep
	}

    public enum ScrollTimerTypes
    {
        ContestViewScrollTimerIn,
        ReviewViewScrollTimerIn,
        ConfirmViewScrollTimerIn
    }
}