using System;
using System.Drawing;
using System.Drawing.Text;
using System.Drawing.Drawing2D;
using VotRite.Util;

namespace VotRite.UI
{
    class VrDropDown : ScreenObject
    {
        public enum DropDownState { OPENED, CLOSED }

        private const int TEXT_MARGIN_LEFT = 70;
        private const float BORDER_WIDTH = 1f;
        private const float FONT_SIZE = 18;
        private const float GROUP_FONT_SIZE = 14f;
        private const float GROUP_MARGIN_RIGHT = 10f;
        private const float STATE_ICON_MARGIN = 5f;

        private DropDownState state;
        private Font groupFont;

        public string StateIconName { get; set; }

        public Image StateIcon { get; set; }

        public ScreenObject Parent { get; set; }

        RectangleF listRectangle;

        public DropDownState State
        {
            get { return state; }
            set
            {
                state = value;

                if (state == DropDownState.OPENED)
                {
                    StateIconName = StateIconName.Replace("collapse", "expand");
                }
                else
                {
                    StateIconName = StateIconName.Replace("expand", "collapse");
                }
            }
        }

        public VrDropDown() : base()
        {
            Type = ScreenObject.ScreenObjectType.DROPDOWN;
            state = DropDownState.CLOSED;
            StateIconName = AppManager.GetPathToCommonFile("graphics" +
                Global.Instance.SLASH + "collapse.png");
            TextSize = (int)FONT_SIZE;
            ForeColor = "#000000";
            BgColor = "#fff";
            TextAlign = "left-middle";
            groupFont = new Font(new Font("Arial Unicode MS",
                GROUP_FONT_SIZE), FontStyle.Regular);
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

                    RectangleF iconRect = new RectangleF(
                        new PointF((float)Left,
                            (float)Top),
                        new SizeF((float)Height - (BORDER_WIDTH * Scale.Height),
                            (float)Height - (BORDER_WIDTH * Scale.Height)));

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

                        gr.DrawImage(StateIcon, ((float)Left + (float)Width) -
                            (iconRect.Width - iconW * Scale.Width),
                            (float)Top + (iconRect.Height -
                            iconW * Scale.Width) / 2f,
                            iconW * Scale.Width,
                            iconH * Scale.Height);
                    }

                    //PrintText(gr);
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

            gr.TextRenderingHint = TextRenderingHint.AntiAlias;

            gr.DrawString(Text, TextFont,
                new SolidBrush(ColorTranslator.FromHtml(ForeColor)),
                textRect,
                textFormat);
        }

        public void OpenDropDownList()
        {
            Graphics gr = Window.Instance.CreateGraphics();

            gr.InterpolationMode = InterpolationMode.NearestNeighbor;
            gr.PixelOffsetMode = PixelOffsetMode.Half;

            gr.DrawRectangle(new Pen(new SolidBrush(Color.Black)),
               Left, Top + Height, Width, 200);

            listRectangle = new RectangleF(
                new PointF((float)Left, (float)Top + Height),
                new SizeF((float)Width - (BORDER_WIDTH * Scale.Width),
                    200 - (BORDER_WIDTH * Scale.Height)));
            gr.InterpolationMode = InterpolationMode.Default;
            gr.FillRectangle(new SolidBrush(Color.White), listRectangle);
        }

        public void CloseDropDownList()
        {
            Graphics gr = Window.Instance.CreateGraphics();
            Parent.Draw(gr);
        }
    }
}