using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace VotRite.JimForms
{
    public partial class exitoptions : Form
    {
        public enum chosenoption
        {
            none, ballot, votrite
        }

        public chosenoption _chosenoption = chosenoption.none;
        public exitoptions()
        {
            InitializeComponent();
        }

        private void exitoptions_Load(object sender, EventArgs e)
        {
            string img = "\\CommonFiles\\graphics\\button_green.jpg";
            
            string path = Directory.GetParent(Application.StartupPath).FullName;
            btnExitballot.BackgroundImage = btnExitvotrite.BackgroundImage =
                Image.FromFile(path + img);
            btnExitballot.ForeColor = btnExitvotrite.ForeColor = Color.White;
           
        }

        private void btnExitballot_Click(object sender, EventArgs e)
        {
            _chosenoption = chosenoption.ballot;
            this.Close();
        }

        private void btnExitvotrite_Click(object sender, EventArgs e)
        {
            _chosenoption = chosenoption.votrite;
            this.Close();
        }
    }
}
