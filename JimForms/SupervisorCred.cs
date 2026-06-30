using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using VotRite.Util;

namespace VotRite.JimForms
{
    public partial class SupervisorCred : Form
    {
        public bool ok = false;
        public SupervisorCred()
        {
            InitializeComponent();
        }

        private void AdminAccessCred_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == '\u001b')
                btn_Cancel_Click(null, null);
        }

        private void btn_Cancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void txt_Cred_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == '\u001b')
                btn_Cancel_Click(null, null);
            if (e.KeyChar == '\r')
                btn_Proceed_Click(null, null);
        }

        private void btn_Proceed_Click(object sender, EventArgs e)
        {
            try
            {


                if (txt_Cred.Text == "")
                {
                    //MessageBox.Show("Please provide Administrator Password to proceed!!", "Votrite Console", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                    lblMessage.Text = "Please provide Supervisor Password to proceed!!";
                    return;
                }

                if (this.txt_Cred.Text == AppManager.Configuration["Security"]["SupervisorPassword"])
                {
                    ok = true;
                    try
                    {
                        AppManager.Instance.CloseOskProcess();
                    }
                    catch (Exception)
                    {
                    }
                    
                }
                else
                {
                    // MessageBox.Show("Incorrect admin Password entered!!", "Votrite Console", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    lblMessage.Text = "Incorrect Supervisor Password entered!!";
                    return;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                ok = false;
            }
            this.Close();
        }

        private void txt_Cred_Enter(object sender, EventArgs e)
        {
            AppManager.Instance.RunOsk();
        }

        private void txt_Cred_Leave(object sender, EventArgs e)
        {
            AppManager.Instance.CloseOskProcess();
        }
    }
}
