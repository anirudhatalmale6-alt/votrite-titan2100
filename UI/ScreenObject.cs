// Product:	VotRite
// Module:  ScreenObject.cs
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
using System.Drawing.Text;
using VotRite.Util;

namespace VotRite.UI
{
    class ScreenObject: IDisposable
    {
		public const int SCREEN_V_SPACE = 20;
		public const int BORDER_PATH_LEN = 5;
		
		public enum ScreenObjectType
        {
            IMAGE,
            BUTTON,
            SELECTION,
            LABEL,
            CONTAINER,
            SCROLL,
            DROPDOWN
        }
		
		public enum ScreenObjectAction
        {
            NONE,
            SET_LOCALE,
            GET_SCREEN,
            GET_CONTEST,
            GET_CONTEST_BY_ID,
            SET_SELECTION,
            SET_WRITEIN,
            SET_TEXT,
            CANCEL,
            ACCEPT,
            CONTINUE,
            GO_BACK,
            SPEAK_TO_POLL_WORKER,
            PRINT_RECORD,
            CONFIRM,
            CANCEL_CONFIRM,
            CAST_BALLOT,
            SCROLL_CONTEST,
            SET_VOLUME,
            SET_TEXT_SIZE,
            SAVE_PREFERENCES
        }
		
		public enum ScreenObjectGraphicsState { NOT_CHANGED=0, CHANGED=1 }
		
        public enum ScreenObjectState { INACTIVE, ACTIVE }
		
        private const int DEFAULT_TEXT_SIZE = 8;
        private const int BUTTON_TEXT_MARGIN_LEFT = 20;
        private const int SELECTION_TEXT_MARGIN_LEFT = 70;
        private const int LABEL_TEXT_MARGIN_LEFT = 5;
        private const int LABEL_TEXT_MARGIN_TOP = 5;

        private string name;
        private ScreenObjectType type;
        private ScreenObjectAction action;
        private ScreenObjectState state;
        private string align;
        private string data;
        private int width;
        private int left;
        private int top;
        private string text;
        private string textAlign;
        private int textSize;
        private string fontName;
        private Font textFont;
        private string foreColor;
        private string bgColor;
        private bool resized;
        private bool visible;
        private SizeF scale;
        private string tag;
        private bool enabled;
		private ScreenObjectGraphicsState graphicsState;

        public string Name { get { return name; } set { name = value; } }
        public virtual ScreenObjectType Type
        {
            get { return type; }
            set { type = value; }
        }
        public ScreenObjectAction Action
        {
            get { return action; }
            set { action = value; }
        }
        public virtual ScreenObjectState ObjectState
        {
            get { return state; }
            set { state = value; }
        }
        public string Align { get { return align; } set { align = value; } }
        public string Data { get { return data; } set { data = value; } }
		public object ObjectData {get;set;}
        public int Width { get { return width; } set { width = value; } }

        public int Height { get; set; }

        public int Left { get { return left; } set { left = value; } }

        public int Top
        {
            get { return top; }
            set { top = value; }
        }

        public string Text 
        { 
            get { return text; } 
            set { text = value; } 
        }
        public string TextAlign
        {
            get { return textAlign; }
            set { textAlign = value; }
        }
        public int TextSize
        {
            get { return textSize; }
            set { textSize = value; }
        }
        public Font TextFont
        {
            get { return textFont; }
            set { textFont = value; }
        }
        public string FontName
        {
            get { return fontName; }
            set
            {
                
                fontName = value;
                textFont = new Font(new Font(fontName,
                    textSize), FontStyle.Bold);
            }
        }
        public string ForeColor
        {
            get { return foreColor; }
            set { foreColor = value; }
        }
        public string BgColor
        {
            get { return bgColor; }
            set { bgColor = value; }
        }
        public Boolean Resized { get { return resized; } }
        public Boolean Visible 
        { 
            get { return visible; } set { visible = value; } 
        }
        public SizeF Scale 
        { 
            get { return scale; } set { scale = value; } 
        }
        public string Tag { get { return tag; } set { tag = value; } }
		public ScreenObjectGraphicsState GraphicsState
		{
			get { return graphicsState; }
			set { graphicsState = value; }
		}

		protected int maxWidth = 0;
		public int MaxWidth
		{
			get { return maxWidth > 0 ? maxWidth : Width; }
			set { maxWidth = value; }
		}

		public int LeftPadding { get; set; }
		public int RightPadding { get; set; }
		public bool IsRightSide { get; set; }

		protected bool fitToText = false;
		public bool FitToText
		{
			get { return fitToText; }
			set { fitToText = value; }
		}

        public bool Enabled {
            get {
                return enabled;
            }
            set
            {
                enabled = value;
            }
        }

        protected ScreenObject()
        {
            textSize = DEFAULT_TEXT_SIZE;
            fontName = "Arial Unicode MS";
            textFont = new Font(new Font(fontName,
                    DEFAULT_TEXT_SIZE), FontStyle.Bold);
            resized = false;
            visible = true;
            scale = new SizeF(1f, 1f);
			graphicsState = ScreenObject.ScreenObjectGraphicsState.CHANGED;
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
                textFont.Dispose();                
            }
        }

        public virtual void Draw(Graphics gr)
        {
            try
            {
                if (visible)
                {
                    gr.Clip = new Region(new Rectangle(new Point(left, top),
                                                       new Size(width, Height)));
				
                    if (bgColor != null)
                        gr.Clear(ColorTranslator.FromHtml(bgColor));
                
                    if (text != null)
                    {
                        if ((type != ScreenObjectType.BUTTON) ||
                            (type != ScreenObjectType.SELECTION))
                            PrintText(gr);
                    }
                }
            }
            catch (Exception e)
            {
                Logger.Instance.Write(e);
            }
			
			//graphicsState = ScreenObject.ScreenObjectGraphicsState.NOT_CHANGED;
        }

        public void Resize(SizeF sz)
        {
            try
			{
				scale = sz;
	            SizeF rectSize = new SizeF(width * scale.Width,
	                Height * scale.Height);
	            PointF rectPos = new PointF(left * scale.Width,
	                top * scale.Height);
	
	            width = (int)rectSize.Width;
	            Height = (int)rectSize.Height;
	            left = (int)rectPos.X;
	            top = (int)rectPos.Y;
	
	            if ((text != null) && (textSize > 0))
	            {
	                float fSize = (float)textSize * scale.Height;
	                textFont = new Font(new Font(fontName,
	                    fSize), FontStyle.Bold);
	            }
				
				resized = true;
			}
			catch (Exception e) { Logger.Instance.Write(e); }
        }

        public virtual void PrintText(Graphics gr)
        {
            StringFormat textFormat = new StringFormat(StringFormatFlags.NoClip);
            RectangleF textRect;
            int marginLeft = 0;
            int marginTop = 0;
			
			switch (type)
            {
                case ScreenObject.ScreenObjectType.BUTTON:
                    marginLeft = BUTTON_TEXT_MARGIN_LEFT;
                    break;
                case ScreenObject.ScreenObjectType.SELECTION:
                    marginLeft = SELECTION_TEXT_MARGIN_LEFT;
                    break;
                case ScreenObject.ScreenObjectType.LABEL:
                    marginLeft = LABEL_TEXT_MARGIN_LEFT;
                    marginTop = LABEL_TEXT_MARGIN_TOP;
                    break;
                default: marginLeft = 0; break;
            }

            switch (textAlign)
            {
                case "left-middle":
                    textFormat.LineAlignment = StringAlignment.Center;
                    textFormat.Alignment = StringAlignment.Near;
                    textRect = new RectangleF(new PointF(left + marginLeft,
                        top), new SizeF(width - marginLeft,
                            Height));
                    break;
                case "right-middle":
                    textFormat.LineAlignment = StringAlignment.Center;
                    textFormat.Alignment = StringAlignment.Far;
                    textRect = new RectangleF(new PointF(left, top),
                        new SizeF(width, Height));
                    break;
                case "left-top":
                    textFormat.LineAlignment = StringAlignment.Near;
                    textFormat.Alignment = StringAlignment.Near;
                    textRect = new RectangleF(new PointF(left + marginLeft,
                        top + marginTop), new SizeF(width - marginLeft,
                            Height - marginTop));
                    break;
                default:
                    textFormat.LineAlignment = StringAlignment.Center;
                    textFormat.Alignment = StringAlignment.Center;
                    textRect = new RectangleF(new PointF(left, top),
                        new SizeF(width, Height));
                    break;
            }

            gr.TextRenderingHint = TextRenderingHint.AntiAlias;
            
            gr.DrawString(text, textFont,
                new SolidBrush(ColorTranslator.FromHtml(ForeColor)),
                textRect,
                textFormat);
        }

		public static int GenerateTextObjectWidth(int minWidth, int maxWidth, int textSize) {
			if (textSize <= 0)
				return minWidth;

			if (maxWidth <= 0)
				maxWidth = 10000;

			if (textSize <= minWidth)
				return minWidth;

			if (textSize >= maxWidth)
				return maxWidth;

			return textSize;

		}
    }
}
