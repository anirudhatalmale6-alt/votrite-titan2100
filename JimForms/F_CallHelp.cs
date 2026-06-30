using System;
using System.Windows.Forms;

namespace VotRite.JimForms
{
    public partial class F_CallHelp : Form
    {
        public F_CallHelp(bool f)
        {
            InitializeComponent();
        }

        private void F_CallHelp_Load(object sender, EventArgs e)
        {
            string sPath = AppManager.Configuration["HelpFile"]["Path"];

            if (sPath != "")
            {
                if (System.IO.File.Exists(sPath))
                {
                    rTxt.ReadOnly = true;
                }
            }
        }
    }
}
