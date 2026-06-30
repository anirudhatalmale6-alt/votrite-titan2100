using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace VotRite.JimForms.UserControls
{
    public partial class vrSelectionPreview : UserControl
    {
        public bool selected = false;
        string img = "\\CommonFiles\\graphics\\checkmark.png";
        
        string path = Directory.GetParent(Application.StartupPath).FullName;
        Color lblfontcolor;
        Color bckcolor;
        public vrSelectionPreview()
        {
            InitializeComponent();
        }

        public vrSelectionPreview(string text)
        {
            InitializeComponent();
            lblcandidate.Text = text;
        }

        private void vrSelectionPreview_Load(object sender, EventArgs e)
        {
            picsign.SizeMode = PictureBoxSizeMode.StretchImage;
            lblfontcolor = lblcandidate.ForeColor;
            bckcolor = picsign.BackColor;
        }

        private void lblcandidate_Click(object sender, EventArgs e)
        {
            //checkall();
            if(lblcandidate.BackColor == Color.Transparent)
            {
                lblcandidate.BackColor = Color.Blue;
                lblcandidate.ForeColor = Color.White;
                if(AppManager.Instance.backgroundTheme == AppManager.colorTheme.Blue)
                {
                    lblcandidate.BackColor = Color.White;
                    lblcandidate.ForeColor = Color.Blue;
                }
                picsign.Image = Image.FromFile(path + img);
                picsign.BackColor = Color.White;
                selected = true;
                ((settings_preview)this.Parent.Parent).setchoice(true);
            }
            else
            {
                lblcandidate.BackColor = Color.Transparent;
                lblcandidate.ForeColor = lblfontcolor;
                picsign.Image = null;
                picsign.BackColor = bckcolor;
                selected = false;
                ((settings_preview)this.Parent.Parent).setchoice(false);
            }
            lblcandidate.Refresh();
            picsign.Refresh();
        }

        private void checkall()
        {
            foreach(var ctrl in this.Parent.Controls)
            {
                if(ctrl is vrSelectionPreview)
                {
                    if (((vrSelectionPreview)ctrl).selected)
                    {
                        ((vrSelectionPreview)ctrl).selected = false;
                        ((vrSelectionPreview)ctrl).lblcandidate.BackColor = Color.Transparent;
                        ((vrSelectionPreview)ctrl).lblcandidate.ForeColor = lblfontcolor;
                        ((vrSelectionPreview)ctrl).picsign.Image = null;
                        ((vrSelectionPreview)ctrl).picsign.BackColor = bckcolor;
                        ((vrSelectionPreview)ctrl).Refresh();


                    }
                }
            }
        }
    }
}
