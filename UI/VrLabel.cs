// Product:	VotRite
// Module:  VrLabel.cs
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
using System.IO;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace VotRite.UI
{
    class VrLabel : ScreenObject
    {
        private const int TEXT_MARGIN_LEFT = 70;
        private const float BORDER_WIDTH = 1f;
        private const float FONT_SIZE = 18;
        private const float GROUP_FONT_SIZE = 14f;
        private const float GROUP_MARGIN_RIGHT = 10f;
        private const float LOGO_MARGIN_RIGHT = 10f;
        private const float STATE_ICON_MARGIN = 5f;

        private SizeF labelSize;
        private SizeF visibleSize;
        private SizeF actualSize;
        private int borderWidth;
        private string borderColor;
        private Pen borderPen;
        private Point[] borderPoints = new Point[ScreenObject.BORDER_PATH_LEN];

        public SizeF VisibleTextSize { get { return visibleSize; } }
        public SizeF ActualTextSize { get { return actualSize; } }
        public bool Speakable { get; set; }
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

        public string Photo { get; set; }
        public string PartyLogo { get; set; }
        public string Group { get; set; }

        private Font groupFont;

        public VrLabel() : base()
        {
            Type = ScreenObject.ScreenObjectType.LABEL;
            ForeColor = "#000000";
            Height = TextSize;
            groupFont = new Font(new Font("Arial Unicode MS",
                GROUP_FONT_SIZE), FontStyle.Regular);
        }

        public override void Draw(Graphics gr)
        {
			base.Draw(gr);

            if (Visible)
            {
                labelSize = new SizeF((float) Width, (float) Height);
                visibleSize = gr.MeasureString(Text, TextFont, labelSize);
                actualSize = gr.MeasureString(Text, TextFont);

                for (int i = 0; i < borderPoints.Length; i++)
                {
                    switch (i)
                    {
                        case 0:
                            borderPoints[i] = new Point(Left, Top);
                            break;
                        case 1:
                            borderPoints[i] = new Point(Left, Top + Height);
                            break;
                        case 2:
                            borderPoints[i] = new Point(Left + Width,
                                                        Top + Height);
                            break;
                        case 3:
                            borderPoints[i] = new Point(Left + Width,
                                                        Top);
                            break;
                        case 4:
                            borderPoints[i] = new Point(Left, Top);
                            break;
                        default:
                            break;
                    }
                }

                if (borderWidth != 0)
                {
                    borderPen = new Pen(new SolidBrush(ColorTranslator.FromHtml(
                        borderColor)), borderWidth);
                    gr.DrawLines(borderPen, borderPoints);
                    borderPen.Dispose();
                }

                var textFormat = new StringFormat(StringFormatFlags.NoClip);
                RectangleF textRect;
                var rightTextBorder = 0;

                if (this.PartyLogo != null && this.PartyLogo.Trim() != "")
                {
                    rightTextBorder += 40;
                }

                if (this.Photo != null && this.Photo.Trim() != "")
                {
                    rightTextBorder += 40;
                }

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
                            var logoW = logo.Width * Scale.Width;
                            var logoH = logo.Height * Scale.Height;

                            if (logoW >= Height)
                                logoW = Height - 10f;
                            if (logoH >= Height)
                                logoH = Height - 10f;

                            var logoRect = new RectangleF(
                                new PointF((Left + (float)Width) -
                                           (logoW + LOGO_MARGIN_RIGHT), Top + 5f),
                                new SizeF(logoW, logoH));
                            photoShift = logoW + LOGO_MARGIN_RIGHT;

                            gr.DrawImage(logo, logoRect.X + 2f,
                                         logoRect.Y + 2f, logoRect.Width - 4f, logoRect.Height - 4f);

                            textRect = new RectangleF(new PointF(((Left +
                                                                   (float)Width) - (logoRect.Width + LOGO_MARGIN_RIGHT)) -
                                                                 (groupStrLen.Width + GROUP_MARGIN_RIGHT),
                                                                 Top + ((Height - groupStrLen.Height) / 2f)),
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
                            var logoW = logo.Width * Scale.Width;
                            var logoH = logo.Height * Scale.Height;

                            if (logoW >= Height)
                                logoW = Height - 10f;
                            if (logoH >= Height)
                                logoH = Height - 10f;

                            var logoRect = new RectangleF(
                                new PointF((Left + (float)Width) -
                                           (logoW + LOGO_MARGIN_RIGHT + photoShift), Top + 5f),
                                new SizeF(logoW, logoH));

                            gr.DrawImage(logo, logoRect.X + 2f,
                                         logoRect.Y + 2f, logoRect.Width - 4f, logoRect.Height - 4f);

                            textRect = new RectangleF(new PointF(((Left +
                                                                   (float)Width) - (logoRect.Width + LOGO_MARGIN_RIGHT)) -
                                                                 (groupStrLen.Width + GROUP_MARGIN_RIGHT),
                                                                 Top + ((Height - groupStrLen.Height) / 2f)),
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
}