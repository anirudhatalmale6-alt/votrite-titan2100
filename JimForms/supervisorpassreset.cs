using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace VotRite.JimForms
{
    public partial class supervisorpassreset : Form
    {
        public supervisorpassreset()
        {
            InitializeComponent();
        }

        private void supervisorpassreset_Load(object sender, EventArgs e)
        {
            try
            {
                txtCurrent.Text = AppManager.Configuration["Security"]["SupervisorPassword"].ToString();
            }
            catch (Exception)
            {

            }
            try
            {
                txtCurr_poll.Text = AppManager.Configuration["Security"]["bpwd"].ToString();
            }
            catch (Exception)
            {

            }
            
        }

        private void btn_Proceed_Click(object sender, EventArgs e)
        {
            if(txtNew.Text == txtNewRe.Text)
            {
                AppManager.Configuration["Security"]["SupervisorPassword"] = txtNew.Text;
                Util.IOHandler.SaveConfig("Security", "SupervisorPassword", txtNew.Text);
                AppManager.Configuration = Util.IOHandler.DecryptConfig();
                MessageBox.Show("Supervisor password is udpated.", "Votrite Supervisor Password", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
                MessageBox.Show("New password and Re-entered password did not matched.", "Votrite Supervisor Password", MessageBoxButtons.OK, MessageBoxIcon.Stop);
        }

        private void btn_Cancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btn_proceed_admin_Click(object sender, EventArgs e)
        {
            if(this.txtCurrent_admin.Text != AppManager.Configuration["Security"]["pwd"])
            {
                MessageBox.Show("Incorrect Admin password entered.", "Votrite Administrator Password", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            if (txtNew_admin.Text.Length != 11)
            {
                //MessageBox.Show("Please provide Administrator Password to proceed!!", "Votrite Console", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                MessageBox.Show("Please 11 character Administrator Password to proceed!!", "Votrite Administrator Password", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            if (txtNew_admin.Text == txtNewRe_admin.Text)
            {
                AppManager.Configuration["Security"]["pwd"] = txtNew_admin.Text;
                Util.IOHandler.SaveConfig("Security", "pwd", txtNew_admin.Text);
                AppManager.Configuration = Util.IOHandler.DecryptConfig();
                MessageBox.Show("Administrator password is udpated.", "Votrite Administrator Password", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
                MessageBox.Show("New password and Re-entered password did not matched.", "Votrite Administrator Password", MessageBoxButtons.OK, MessageBoxIcon.Stop);
        }

        private void btn_proceed_poll_Click(object sender, EventArgs e)
        {
            if (txtNew_poll.Text == txtNewRe_poll.Text)
            {
                AppManager.Configuration["Security"]["bpws"] = txtNew_poll.Text;
                Util.IOHandler.SaveConfig("Security", "bpwd", txtNew_poll.Text);
                AppManager.Configuration = Util.IOHandler.DecryptConfig();
                MessageBox.Show("Pollworker password is udpated.", "Votrite Pollworker Password", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
                MessageBox.Show("New password and Re-entered password did not matched.", "Votrite Pollworker Password", MessageBoxButtons.OK, MessageBoxIcon.Stop);
        }
    }
}
