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
    public partial class resetoptions : Form
    {
        public resetoptions()
        {
            InitializeComponent();
        }
        public int options = 0;

        private void resetoptions_Load(object sender, EventArgs e)
        {
            string img = "\\CommonFiles\\graphics\\button_green.jpg";

            //string path = Application.StartupPath;
            btnAll.BackgroundImage = btnList.BackgroundImage =
                Image.FromFile(Global.Instance.APP_PATH+ img);
            btnAll.ForeColor = btnList.ForeColor = Color.White;
            img = "\\CommonFiles\\graphics\\button_red.jpg";
            btnExit.BackgroundImage = Image.FromFile(Global.Instance.APP_PATH + img);
            btnExit.ForeColor = btnList.ForeColor = Color.White;
        }

        private void btnAll_Click(object sender, EventArgs e)
        {
            options = 1;
            this.Close();
        }

        private void btnList_Click(object sender, EventArgs e)
        {
            options = 2;
            this.Close();
        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
