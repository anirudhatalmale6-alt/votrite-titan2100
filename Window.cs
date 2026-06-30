// Product:	VotRite
// Module:  Window.cs
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
using System.Windows.Forms;
using VotRite.Util;

namespace VotRite
{
	class Window : Form
    {
        public static Window Instance;
        private PictureBox pictureBox1;
        private Panel panel1;
        private Label label1;
        private static Size displaySize;

        public static Size DisplaySize 
		{ 
			get { return displaySize; } set { displaySize = value; } 
		}
		
		/// <summary>
        /// Default contsructor
        /// </summary>
        /// <param name="wx"></param>
        /// <param name="wy"></param>
        /// <param name="ww"></param>
        /// <param name="wh"></param>
        /// <param name="wn"></param>
        public Window()
        {
			Instance = this;
            DoubleBuffered = true;
            ShowInTaskbar = false;
            MaximizeBox = false;
            MinimizeBox = false;
            ControlBox = false;
            FormBorderStyle = FormBorderStyle.None;
			Visible = false;
            this.BackgroundImage = Image.FromFile("CommonFiles\\graphics\\initialBG_f.png");
            
            //this.BackColor = Color.FromName("#1C0F98");
            //this.BackgroundImage = global::VotRite.Properties.Resources.initialBG_f;
            this.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.ClientSize = new System.Drawing.Size(1740, 991);

            this.Refresh();
            displaySize = Screen.PrimaryScreen.Bounds.Size;
			
			Width = displaySize.Width;
			Height = displaySize.Height;
			Left = Top = 0;
			
            MouseDown += HandleTouch;
            KeyUp += HandleKey;

            Logger.Instance.Write("Window initialized");
            if (Driver.TestMode)
                Logger.Instance.Write(string.Format("Test mode ballot id : {0}", AppManager.BallotId));
			AppManager.Instance.InitializeApp(this);
            Visible = true;
            
            //vModel.Definition.BgColor = "#1C0F98";
        }

        /// <summary>
        /// Handle on-screen touch
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="evt"></param>
        private void HandleTouch(object sender, MouseEventArgs evt) 
		{
            if (AppManager.Instance.ballot != null)
            {
                if (AppManager.Instance.ballot.BallotMode != Session.BallotModes.Audio)
                {
                    if (AppManager.Instance.ActiveScreens.Count > 0)
                        AppManager.Instance.GetActiveScreen().HandleTouch(evt.X, evt.Y);
                }
            }
		}


        /// <summary>
        /// Handle keyboard key
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="evt"></param>
        public void HandleKey(object sender, KeyEventArgs evt)
        {
            if (AppManager.Instance.ActiveScreens.Count > 0)
            {
                switch (evt.KeyCode)
                {
                    case Keys.Escape:
                        if (AppManager.Instance.ballot.BallotMode == Session.BallotModes.Audio)
                        {
                            JimForms.exitoptions frmoption = new JimForms.exitoptions();
                            frmoption.ShowDialog();
                            if (frmoption._chosenoption == JimForms.exitoptions.chosenoption.none)
                                break;
                            else if (frmoption._chosenoption == JimForms.exitoptions.chosenoption.ballot)
                            {
                                JimForms.PollworkerCred frmPollcred = new JimForms.PollworkerCred();
                                frmPollcred.ShowDialog();
                                if (frmPollcred.ok)
                                {
                                    AppManager.Instance.StopSpeaker();
                                    AppManager.Instance.ShowSoftPinpad("main", true);
                                }
                            }
                            else
                            {
                                if (JimForms.MenuForm.adminaccess())
                                    Close();
                            }
                            //break;

                        }
                        else
                        {
                            if (JimForms.MenuForm.adminaccess())
                                Close();
                        }
                        break;
                    //case Keys.F5: AppManager.Instance.ShowSoftPinpad("main", true); break;
                    default:
                        if (AppManager.Instance.ballot.BallotMode == Session.BallotModes.Audio)
                        {
                            switch (evt.KeyCode)
                            {
                                case Keys.Enter:
                                case Keys.Space:
                                    AppManager.Instance.keyboardButton = AppManager.KeyboardButton.select;
                                    break;
                                case Keys.Right:
                                case Keys.Down:
                                case Keys.Tab:
                                    AppManager.Instance.keyboardButton = AppManager.KeyboardButton.next;
                                    break;
                                default:
                                    AppManager.Instance.GetActiveScreen().HandleKey(evt.KeyCode.ToString());
                                    break;
                            }
                        }
                        else
                        {
                            AppManager.Instance.GetActiveScreen().HandleKey(evt.KeyCode.ToString());
                        }
                        break;
                }
            }
        }

        /// <summary>
        /// Terminate window
        /// </summary>
        public virtual void Terminate() 
        {
            try
            {
                Taskbar.Show();
            }
            catch (Exception)
            {
            }
            try
            {
                Logger.Instance.Write("Window terminated");
                this.Dispose();
            }
            catch (Exception)
            {

            }
			
        }
		
        protected override void OnLoad(EventArgs e)
        {
            GemBox.Spreadsheet.SpreadsheetInfo.SetLicense("FREE-LIMITED-KEY");
            base.OnLoad(e);
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
 	         base.OnFormClosing(e);

             Terminate();
        }
		
		protected override void OnPaint(PaintEventArgs e)
		{
		    try
		    {
		        if (AppManager.Instance.ActiveScreens.Count > 0)                
		            AppManager.Instance.GetActiveScreen().ReDraw(e.Graphics);
		        else
		            Logger.Instance.Write("there is no active screen");
		    }
		    catch (Exception exc)
		    {
                Logger.Instance.Write(exc);
		    }
		}

	    protected override void OnMouseUp(MouseEventArgs e)
	    {
            if (AppManager.Instance.ActiveScreens.Count > 0)
                AppManager.Instance.GetActiveScreen().HandleMouseUp();
            if (AppManager.Instance.ballot != null)
            {
                if (AppManager.Instance.ballot.BallotMode == Session.BallotModes.Audio)
                {
                    if (e.Button == MouseButtons.Left)
                        AppManager.Instance.mouseButton = AppManager.MouseButton.left;
                    else
                        AppManager.Instance.mouseButton = AppManager.MouseButton.right;
                }
            }
        }       

        private void InitializeComponent()
        {
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.panel1 = new System.Windows.Forms.Panel();
            this.label1 = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // pictureBox1
            // 
            this.pictureBox1.Image = global::VotRite.Properties.Resources.logo;
            this.pictureBox1.Location = new System.Drawing.Point(8, 3);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(589, 496);
            this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pictureBox1.TabIndex = 0;
            this.pictureBox1.TabStop = false;
            // 
            // panel1
            // 
            this.panel1.BackColor = System.Drawing.Color.White;
            this.panel1.Controls.Add(this.label1);
            this.panel1.Controls.Add(this.pictureBox1);
            this.panel1.Location = new System.Drawing.Point(554, 189);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(609, 598);
            this.panel1.TabIndex = 1;
            // 
            // label1
            // 
            this.label1.BackColor = System.Drawing.Color.White;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 14F, ((System.Drawing.FontStyle)((System.Drawing.FontStyle.Bold | System.Drawing.FontStyle.Underline))), System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(8, 515);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(589, 58);
            this.label1.TabIndex = 1;
            this.label1.Text = "Votrite Voting App";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // Window
            // 
            this.BackgroundImage = global::VotRite.Properties.Resources.initialBG_f;
            this.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.ClientSize = new System.Drawing.Size(1740, 991);
            this.Controls.Add(this.panel1);
            this.DoubleBuffered = true;
            this.Name = "Window";
            this.Load += new System.EventHandler(this.Window_Load);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.panel1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        private void Window_Load(object sender, EventArgs e)
        {
            //pictureBox1.Image = Image.FromFile("..\\CommonFiles\\graphics\\initialBG_f1.png");
            panel1.Left = (this.Width / 2) - (panel1.Width / 2);
            panel1.Top = (this.Height / 2) - (panel1.Height / 2);
            this.Refresh();


        }
    }
}
