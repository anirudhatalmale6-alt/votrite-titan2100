// Product:	VotRite
// Module:  VrSelection.cs
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
using System.Drawing.Drawing2D;
using System.IO;
using VotRite.Util;

namespace VotRite.UI
{
    class VrSelection : ScreenObject
    {
		public enum SelectionState { DESELECTED, SELECTED }
		
        private const int TEXT_MARGIN_LEFT = 70;
        private const float BORDER_WIDTH = 1f;
        private const float FONT_SIZE = 18;
        private const float GROUP_FONT_SIZE = 14f;
        private const float GROUP_MARGIN_RIGHT = 10f;
        private const float LOGO_MARGIN_RIGHT = 10f;
        private const float STATE_ICON_MARGIN = 5f;

        private SelectionState state;
        private Font groupFont;

        public string BgImageName { get; set; }

        public Image BgImage { get; set; }

        public string StateIconName { get; set; }

        public Image StateIcon { get; set; }

        public string Photo { get; set; }
        public string PartyLogo { get; set; }

        public string Group { get; set; }
        public int ButtonGroup { get; set; }

        public SelectionState State
        {
            get { return state; }
            set
            {
                state = value;

                if (state == SelectionState.SELECTED)
                {
                    ForeColor = "#ffffff";
                    //ForeColor = "#000000";
                    //BgImageName = BgImageName.Replace("_d", "_a");
                    BgImageName = AppManager.GetPathToCommonFile("graphics" + Global.Instance.SLASH + "bg_selection_a.png");
                    switch (AppManager.Instance.backgroundTheme)
                    { 
                         case AppManager.colorTheme.Blue:
                            BgImageName = AppManager.GetPathToCommonFile("graphics" + Global.Instance.SLASH + "bg_selection_yellow_d.png");
                            ForeColor = "#000000";
                            break;
                        default:
                                break;
                    }
                }
                else
                {
                    ForeColor = "#000000";
                    BgImageName = BgImageName.Replace("_a", "_d");

                    switch (AppManager.Instance.backgroundTheme)
                    {
                        case AppManager.colorTheme.Yellow:
                            BgImageName = AppManager.GetPathToCommonFile("graphics" + Global.Instance.SLASH + "bg_selection_yellow_d.png");
                            ForeColor = "#000000";
                            //var scroll = container.FindControlByName("");
                            break;
                        case AppManager.colorTheme.Blue:
                            BgImageName = AppManager.GetPathToCommonFile("graphics" + Global.Instance.SLASH + "bg_selection_blue_d.png");
                            ForeColor = "#ffffff";
                            break;
                        case AppManager.colorTheme.Green:
                            BgImageName = AppManager.GetPathToCommonFile("graphics" + Global.Instance.SLASH + "bg_selection_green_d.png");
                            ForeColor = "#ffffff";
                            break;
                        case AppManager.colorTheme.Contrast:
                            BgImageName = AppManager.GetPathToCommonFile("graphics" + Global.Instance.SLASH + "bg_selection_contrast_d.png");
                            ForeColor = "#ffffff";
                            break;
                        case AppManager.colorTheme.LightBlue:
                            BgImageName = AppManager.GetPathToCommonFile("graphics" + Global.Instance.SLASH + "bg_selection_lightblue_d.png");
                            ForeColor = "#000000";
                            break;
                        case AppManager.colorTheme.LightYellow:
                            BgImageName = AppManager.GetPathToCommonFile("graphics" + Global.Instance.SLASH + "bg_selection_lightyellow_d.png");
                            ForeColor = "#000000";
                            break;
                        default:
                            break;
                    }
                }
                
                   
                
            }
        }

        public VrSelection() : base() { initialize(); }

        private void initialize()
        {
            Type = ScreenObject.ScreenObjectType.SELECTION;
            state = SelectionState.DESELECTED;
            BgImageName = AppManager.GetPathToCommonFile("graphics" + 
                Global.Instance.SLASH + "bg_selection_d.png");
            StateIconName = AppManager.GetPathToCommonFile( "graphics" + 
                Global.Instance.SLASH +"checkmark.png");
            TextSize = (int)FONT_SIZE;
            ForeColor = "#000000";
            TextAlign = "left-middle";
            groupFont = new Font(new Font("Arial Unicode MS",
                GROUP_FONT_SIZE), FontStyle.Regular);
            Enabled = true;

            switch (AppManager.Instance.backgroundTheme)
            {
                case AppManager.colorTheme.Yellow:
                    BgImageName = AppManager.GetPathToCommonFile("graphics" + Global.Instance.SLASH + "bg_selection_yellow_d.png");
                    ForeColor = "#000000";
                    //var scroll = container.FindControlByName("");
                    break;
                case AppManager.colorTheme.Blue:
                    BgImageName = AppManager.GetPathToCommonFile("graphics" + Global.Instance.SLASH + "bg_selection_blue_d.png");
                    ForeColor = "#ffffff";
                    break;
                case AppManager.colorTheme.Green:
                    BgImageName = AppManager.GetPathToCommonFile("graphics" + Global.Instance.SLASH + "bg_selection_green_d.png");
                    ForeColor = "#ffffff";
                    break;
                case AppManager.colorTheme.Contrast:
                    BgImageName = AppManager.GetPathToCommonFile("graphics" + Global.Instance.SLASH + "bg_selection_contrast_d.png");
                    ForeColor = "#ffffff";
                    break;
                case AppManager.colorTheme.LightBlue:
                    BgImageName = AppManager.GetPathToCommonFile("graphics" + Global.Instance.SLASH + "bg_selection_lightblue_d.png");
                    ForeColor = "#000000";
                    break;
                case AppManager.colorTheme.LightYellow:
                    BgImageName = AppManager.GetPathToCommonFile("graphics" + Global.Instance.SLASH + "bg_selection_lightyellow_d.png");
                    ForeColor = "#000000";
                    break;
                default:
                    break;
            }
        }
		
        public override void Draw(Graphics gr)
        {
            base.Draw(gr);

            if (Visible)
            {
                gr.Clip = new Region(new Rectangle(new Point(Left - 1, Top - 1),
                    new Size(Width + 1, Height + 1)));

                try
                {
                    gr.InterpolationMode = InterpolationMode.NearestNeighbor;
                    gr.PixelOffsetMode = PixelOffsetMode.Half;

                    gr.DrawRectangle(new Pen(new SolidBrush(Color.Black)),
                        Left, Top, Width, Height);  //  border

                    using (BgImage = Image.FromFile(BgImageName))
                    {
                        RectangleF iconRect = new RectangleF(
                            new PointF((float)Left,
                                (float)Top),
                            new SizeF((float)Height - (BORDER_WIDTH * Scale.Height),
                                (float)Height - (BORDER_WIDTH * Scale.Height)));

                        gr.DrawImage(BgImage, (((float)Left + iconRect.Width) -
                            (BORDER_WIDTH * Scale.Width)),
                            (float)Top, (float)Width - iconRect.Width,
                            (float)Height - (BORDER_WIDTH * Scale.Height));
                        
                        if (state == SelectionState.SELECTED)
                        {
                            gr.InterpolationMode = InterpolationMode.Default;

                            gr.FillRectangle(new SolidBrush(Color.White), iconRect);

                            using (StateIcon = Image.FromFile(StateIconName))
                            {
                                float iconW = (float)StateIcon.Width;
                                float iconH = (float)StateIcon.Height;

                                if (iconW >= iconRect.Width)
                                    iconW = iconRect.Width - STATE_ICON_MARGIN;
                                if (iconH >= iconRect.Height)
                                    iconH = iconRect.Height - STATE_ICON_MARGIN;

                                gr.DrawImage(StateIcon, (float)Left +
                                    (iconRect.Width - iconW * Scale.Width) / 2f,
                                    (float)Top + (iconRect.Width -
                                    iconW * Scale.Width) / 2f,
                                    iconW * Scale.Width,
                                    iconH * Scale.Height);
                            }
                        }
                        else
                            gr.FillRectangle(new
                                SolidBrush(ColorTranslator.FromHtml("#cdcdcd")),
                                iconRect);
                    }

                    PrintText(gr);

                    if (!Enabled)
                    {
                        Brush brush = new SolidBrush(Color.FromArgb(128, 10, 50, 100));
                        gr.FillRectangle(brush, Left, Top, Width, Height);
                    }
                }
                catch (Exception e)
                {
                    Logger.Instance.Write(e);
                }
            }
        }

        public override void PrintText(Graphics gr)
        {
            var textFormat = new StringFormat(StringFormatFlags.NoClip);
            RectangleF textRect;

            var rightTextBorder = 0;
            if (this.PartyLogo != null && this.PartyLogo.Trim() != "")
            { rightTextBorder += 40; }
            if (this.Photo != null && this.Photo.Trim() != "")
            { rightTextBorder += 40; }

            switch (TextAlign)
            {
                case "left-middle":
                    textFormat.LineAlignment = StringAlignment.Center;
                    textFormat.Alignment = StringAlignment.Near;
                    textRect = new RectangleF(new PointF(Left +
                        TEXT_MARGIN_LEFT, Top),
                        new SizeF(Width - rightTextBorder - TEXT_MARGIN_LEFT,
                            Height));
                    break;
                case "left-middle++":
                    textFormat.LineAlignment = StringAlignment.Center;
                    textFormat.Alignment = StringAlignment.Near;
                    textRect = new RectangleF(new PointF(Left+10 +
                        TEXT_MARGIN_LEFT, Top),
                        new SizeF(Width - rightTextBorder - TEXT_MARGIN_LEFT,
                            Height));
                    break;
                case "right-middle":
                    textFormat.LineAlignment = StringAlignment.Center;
                    textFormat.Alignment = StringAlignment.Far;
                    textRect = new RectangleF(new PointF(Left, Top),
                        new SizeF(Width, Height));
                    break;
                default:
                    textFormat.LineAlignment = StringAlignment.Center;
                    textFormat.Alignment = StringAlignment.Center;
                    textRect = new RectangleF(new PointF(Left, Top),
                        new SizeF(Width, Height));
                    break;
            }

            gr.TextRenderingHint = TextRenderingHint.AntiAlias;

            gr.DrawString(Text, TextFont,
                new SolidBrush(ColorTranslator.FromHtml(ForeColor)),
                textRect,
                textFormat);

            groupFont = new Font(new Font("Arial Unicode MS",
                    GROUP_FONT_SIZE * Scale.Height), FontStyle.Regular);
            var groupStrLen = gr.MeasureString(Group, groupFont);

            float photoShift = 0;
            if ((PartyLogo != null) && (PartyLogo.Trim() != ""))
            {                
                var pathToPartyLogo = AppManager.GetPathToCommonFile("graphics" + Global.Instance.SLASH + "ballot" + Global.Instance.SLASH + PartyLogo);
                if (File.Exists(pathToPartyLogo))
                {
                    gr.InterpolationMode = InterpolationMode.High;
                    using (var logo = Image.FromFile(pathToPartyLogo))
                    {
                        var logoW = logo.Width*Scale.Width;
                        var logoH = logo.Height*Scale.Height;

                        if (logoW >= Height)
                            logoW = Height - 10f;
                        if (logoH >= Height)
                            logoH = Height - 10f;

                        var logoRect = new RectangleF(
                            new PointF((Left + (float) Width) -
                                       (logoW + LOGO_MARGIN_RIGHT), Top + 5f),
                            new SizeF(logoW, logoH));
                        photoShift = logoW + LOGO_MARGIN_RIGHT;

                        gr.DrawImage(logo, logoRect.X + 2f,
                                     logoRect.Y + 2f, logoRect.Width - 4f, logoRect.Height - 4f);

                        textRect = new RectangleF(new PointF(((Left +
                                                               (float) Width) - (logoRect.Width + LOGO_MARGIN_RIGHT)) -
                                                             (groupStrLen.Width + GROUP_MARGIN_RIGHT),
                                                             Top + ((Height - groupStrLen.Height)/2f)),
                                                  new SizeF(groupStrLen.Width, groupStrLen.Height));
                    }
                }
            }
            
            if ((Photo != null) && (Photo.Trim() != ""))
            {                
                var pathToPhoto = AppManager.GetPathToCommonFile("graphics" + Global.Instance.SLASH + "ballot" + Global.Instance.SLASH + Photo);
                if (File.Exists(pathToPhoto))
                {
                    using (var logo = Image.FromFile(pathToPhoto))
                    {
                        gr.InterpolationMode = InterpolationMode.High;
                        var logoW = logo.Width*Scale.Width;
                        var logoH = logo.Height*Scale.Height;

                        if (logoW >= Height)
                            logoW = Height - 10f;
                        if (logoH >= Height)
                            logoH = Height - 10f;

                        var logoRect = new RectangleF(
                            new PointF((Left + (float) Width) -
                                       (logoW + LOGO_MARGIN_RIGHT + photoShift), Top + 5f),
                            new SizeF(logoW, logoH));

                        gr.DrawImage(logo, logoRect.X + 2f,
                                     logoRect.Y + 2f, logoRect.Width - 4f, logoRect.Height - 4f);

                        textRect = new RectangleF(new PointF(((Left +
                                                               (float) Width) - (logoRect.Width + LOGO_MARGIN_RIGHT)) -
                                                             (groupStrLen.Width + GROUP_MARGIN_RIGHT),
                                                             Top + ((Height - groupStrLen.Height)/2f)),
                                                  new SizeF(groupStrLen.Width, groupStrLen.Height));
                    }
                }
            }

            if (Group != null)
            {
                if ((Photo == null) || (Photo.Trim() == ""))
                    textRect = new RectangleF(new PointF((Left + (float)Width) -
                        (groupStrLen.Width + GROUP_MARGIN_RIGHT),
                        Top + ((Height - groupStrLen.Height) / 2f)),
                        new SizeF(groupStrLen.Width, groupStrLen.Height));

                gr.DrawString(Group, groupFont,
                    new SolidBrush(ColorTranslator.FromHtml(ForeColor)),
                    textRect,
                    textFormat);
            }
        }
    }
}