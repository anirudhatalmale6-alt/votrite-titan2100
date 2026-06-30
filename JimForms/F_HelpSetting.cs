using System;
using System.Windows.Forms;

using VotRite.Util;

namespace VotRite.JimForms
{
    public partial class F_HelpSetting : Form
    {
        public F_HelpSetting()
        {
            InitializeComponent();

            txtHelpPath.Text = AppManager.Configuration["HelpFile"]["Path"];
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            if (txtHelpPath.Text.Trim() != "")
            {
                if (!System.IO.File.Exists(this.txtHelpPath.Text))
                {
                    MessageBox.Show("Help File:" + txtHelpPath.Text + " doesn't exist!", "VotRite Warn", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
            }

            IOHandler.SaveConfig("HelpFile", "Path", this.txtHelpPath.Text);

            MessageBox.Show("Save Successfully!", "VotRite Warn", MessageBoxButtons.OK, MessageBoxIcon.Information);
            this.Close();
        }

        private void btnBrowse_Click(object sender, EventArgs e)
        {
            OpenFileDialog dlg = new OpenFileDialog();
            dlg.Filter = "PDF Files(*.pdf)|*.pdf";

            if (dlg.ShowDialog() == DialogResult.OK)
            {
                txtHelpPath.Text = dlg.FileName;
            }
        }

        private void btnClear_Click(object sender, EventArgs e)
        {
            txtHelpPath.Text = "";
        }
    }
}
