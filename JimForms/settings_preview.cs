using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using VotRite.JimForms.UserControls;
using VotRite.Models;
using VotRite.UI;
using VotRiteBallotDataManager.AppCode;

namespace VotRite.JimForms
{
    public partial class settings_preview : Form
    {
        public settings_preview()
        {
            InitializeComponent();
        }
        string img = "\\CommonFiles\\graphics\\button_green.jpg";
        string imgLogo = "\\CommonFiles\\graphics\\Logo4.png";
        string path = Directory.GetParent(Application.StartupPath).FullName;
        List<Candidate> candidates = new List<Candidate>();
        List<string> _textsToSpeak = new List<string>();
        public bool hearingmode = false;
        private void settings_preview_Load(object sender, EventArgs e)
        {
            //var img = ((VrButton)cntrlobj).BgImage;
            loadinfo();
            panelContainer.Width = this.Width - panelContainer.Left - 2;
            setDoubleSpace();
            addcandidate();
            
            
            pictureBox1.Image = Image.FromFile(path + imgLogo);
            pictureBox1.SizeMode = PictureBoxSizeMode.StretchImage;
            var nimg = img.Substring(0, img.Length - 4) + "_" + AppManager.Instance.backgroundTheme + ".png";
            if (AppManager.Instance.backgroundTheme == AppManager.colorTheme.White)
            {
                nimg = img;
                //nimg = Path.Combine(path, nimg);
                btnReveiw.BackgroundImage = Image.FromFile(path + img);
                btnNext.BackgroundImage = Image.FromFile(path + img);
                btnClose.BackgroundImage = btnBack.BackgroundImage = Image.FromFile(path + img.Replace("green", "red"));
            }
            else
            {
                btnReveiw.BackgroundImage = Image.FromFile(path + nimg);
                btnNext.BackgroundImage = Image.FromFile(path + nimg);
                btnClose.BackgroundImage = btnBack.BackgroundImage = Image.FromFile(path + img.Replace("green.jpg", "red_" + AppManager.Instance.backgroundTheme.ToString() + ".png"));
                switch (AppManager.Instance.backgroundTheme)
                {

                    case AppManager.colorTheme.Yellow:
                        this.BackColor = (Color)ColorTranslator.FromHtml("#ffeb3b");
                        //var scroll = container.FindControlByName("");
                        break;
                    case AppManager.colorTheme.Blue:
                        this.BackColor = (Color)ColorTranslator.FromHtml("#0623cf");
                        lblChoicesLeft.ForeColor = Color.Yellow;
                        setLableForecolor(Color.White);
                        break;
                    case AppManager.colorTheme.Green:
                        this.BackColor = (Color)ColorTranslator.FromHtml("#4caf50");
                        lblChoicesLeft.ForeColor = Color.Yellow;
                        setLableForecolor(Color.White);
                        break;
                    case AppManager.colorTheme.Contrast:
                        this.BackColor = (Color)ColorTranslator.FromHtml("#000000");
                        setLableForecolor(Color.White);
                        break;
                    case AppManager.colorTheme.LightBlue:
                        this.BackColor = (Color)ColorTranslator.FromHtml("#aad8e6");
                        break;
                    case AppManager.colorTheme.LightYellow:
                        this.BackColor = (Color)ColorTranslator.FromHtml("#FFFF99");// "#FFFFEO";// "#fff44f";
                        break;
                    default:
                        break;
                }
            }
            //hearingModeSpeak();

            timer1.Start();
        }

        private void loadinfo()
        {
            Type t;

            var ballot = AppManager.Instance.ballot;
            candidates = Candidate.GetCandidates(ballot.ContestsList[0].Id, 1);
            try
            {
                lblBallotAddress.Text = ballot.Address;
                lblBallotDate.Text = DateTime.Now.ToString("dddd, MMMM dd, yyyy");
                lblBallotName.Text = ballot.ContestsList[0].GroupName.ToUpper();
                lblBallotTitle.Text = ballot.Name.ToUpper();
                //lblContestTitle.Text = ballot.ContestsList[0].Title.ToUpper();
                lblCount.Text = "1 of " + ballot.ContestsList.Count;
                lblGroupName.Text = ballot.Title.ToUpper();
                lblContestName.Text = ballot.ContestsList[0].Name.ToUpper();
                //lblContestTip.Text = ballot.

               
            }
            catch (Exception ex)
            { }
        }

        private void setLableForecolor(Color color)
        {
            lblBallotAddress.ForeColor = lblBallotDate.ForeColor = lblBallotName.ForeColor = lblBallotTitle.ForeColor = 
                            lblContestName.ForeColor = lblContestTip.ForeColor = lblCount.ForeColor = lblGroupName.ForeColor =
                            color;
            foreach(Control cntrl in panelContainer.Controls)
            {
                cntrl.ForeColor = color;
            }
        }

        private void setDoubleSpace()
        {
            
            {
                foreach (Control ctrl in this.Controls)
                {
                    if (ctrl.Text != null)
                    {
                        if (AppManager.Instance.isDoubleSpacing)
                            ctrl.Text = ctrl.Text.Replace(" ", "  ");
                    }
                    ChangeTextSize(ctrl);
                }
                //if(lblBallotTitle.is)
            }
        }

        internal void ChangeTextSize(Control pObject)
        {
            if (pObject.Name != "btnClose" && pObject.Name != "lblBallotAddress" && pObject.Name != "lblBallotDate")
            {
                string sTextSize = AppManager.Configuration["TextSize"]["Value"];
                if (sTextSize != "")
                {
                    if (float.Parse(sTextSize) > 26)
                        sTextSize = "28";
                    Font font = new Font(pObject.Font.FontFamily, float.Parse(sTextSize));
                    pObject.Font = font;

                }
            }
        }

        private void addcandidate()
        {
            panelContainer.Width = this.Width - panelContainer.Left - 20;
            panelContainer.AutoScrollMinSize = panelContainer.Size;
            
            foreach (var cand in candidates)
            {
                int top = 0;
                if (panelContainer.Controls.Count > 0)
                    top = panelContainer.Controls[panelContainer.Controls.Count - 1].Top + panelContainer.Controls[panelContainer.Controls.Count - 1].Height + 5;

                var sel = new vrSelectionPreview(cand.Name);
                sel.BorderStyle = BorderStyle.FixedSingle;
                panelContainer.Controls.Add(sel);
                panelContainer.Controls[panelContainer.Controls.Count - 1].Width = panelContainer.Width-60;
                panelContainer.Controls[panelContainer.Controls.Count - 1].Height = 85;
                panelContainer.Controls[panelContainer.Controls.Count - 1].Top = top;
                //panelContainer.Controls[panelContainer.Controls.Count - 1].bo = BorderStyle.FixedSingle;
            }
            
            panelContainer.Refresh();
            lblBallotAddress.Left = panelContainer.Right - lblBallotAddress.Width;
            lblBallotDate.Left = panelContainer.Right - lblBallotDate.Width;
            btnClose.Left = panelContainer.Right - btnClose.Width;
        }

        private void hearingModeSpeak()
        {
            if (hearingmode)
            {
              
                 words.Add(lblBallotName.Text);
                 words.Add(lblContestName.Text);
                 words.Add(lblContestTitle.Text);
                 words.Add(lblChoicesLeft.Text);
                 words.Add(lblContestTip.Text);

                foreach (var cand in candidates)
                {
                     words.Add("Left click to choose " + cand.Name + ". Right click for next option.");
                    //System.Threading.Thread.Sleep(10000);
                }

                 words.Add("Left click to choose " + btnNext.Text + ". Right click for next option.");
                //System.Threading.Thread.Sleep(10000);
                 words.Add("Left click to choose " + btnReveiw.Text + ". Right click for next option.");
                //System.Threading.Thread.Sleep(10000);
                 words.Add("Left click to choose " + btnBack.Text + ". Right click for next option.");
                //System.Threading.Thread.Sleep(10000);

                // hearingModeSpeak();
                AppManager.Instance.StartSpeaker_preview(words[speaked]);
                speaked++;
                System.Threading.Thread.Sleep(1000);
                timer2.Enabled = true;
                timer2.Start();
            }
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void panelContainer_MouseClick(object sender, MouseEventArgs e)
        {

        }

        private void panelContainer_MouseDown(object sender, MouseEventArgs e)
        {

        }
        public void setchoice(bool zero)
        {
            if (zero)
                lblChoicesLeft.Text = "You have 0 choices remaining.";
            else
                lblChoicesLeft.Text = "You have 1 choices remaining.";
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            timer1.Stop();
            hearingModeSpeak();
        }

        private void settings_preview_KeyPress(object sender, KeyPressEventArgs e)
        {
            
        }

        private void settings_preview_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Escape)
                {
                    //if (MessageBox.Show("Do you want to close preview?", "Close Preview?", MessageBoxButtons.YesNo) == DialogResult.Yes)
                        this.Close();
                }
            }
            catch (Exception)
            {


            }
        }

        private void settings_preview_FormClosing(object sender, FormClosingEventArgs e)
        {
            try
            {
                stop = true;
            }
            catch (Exception)
            {
            }

        }
        int speaked = 0;
        List<string> words = new List<string>();
        bool stop = false;

        private void timer2_Tick(object sender, EventArgs e)
        {
            if(stop)
            {                
                timer2.Stop();
                timer2.Enabled = false;
            }
            if (AppManager.Instance.speaker.Status.RunningState != SpeechLib.SpeechRunState.SRSEIsSpeaking)
            {
                if (speaked < words.Count)
                {
                    AppManager.Instance.StartSpeaker_preview(words[speaked]);
                    speaked++;
                }
            }
        }
    }
}
