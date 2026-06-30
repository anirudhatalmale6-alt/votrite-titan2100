using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using VotRite.Forms;
using VotRite.Util;

namespace VotRite.JimForms
{
    public partial class AdminCred : Form
    {
        public bool verified = false;
        public bool useExternalKeyboard = false;
        public AdminCred()
        {
            InitializeComponent();
        }

        private void btn_Proceed_Click(object sender, EventArgs e)
        {
            try
            {


                if (txt_Cred.Text == "")
                {
                    //MessageBox.Show("Please provide Administrator Password to proceed!!", "Votrite Console", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                    lblMessage.Text = "Please provide Administrator Password to proceed!!";
                    return;
                }
                if (txt_Cred.Text.Length != 11)
                {
                    //MessageBox.Show("Please provide Administrator Password to proceed!!", "Votrite Console", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                    lblMessage.Text = "Please provide correct Administrator Password to proceed!!";
                    return;
                }
                string crd = AppManager.Configuration["Security"]["pwd"];
               // if (AppManager.Configuration["Security"]["pwd"].Contains(this.txt_Cred.Text.Substring(0, this.txt_Cred.Text.Length-2)))
               if(AppManager.Configuration["Security"]["pwd"] == this.txt_Cred.Text)
                {
                    if (VotriteCrypto.Encryptelf(this.txtnopwd.Text) == "tMblctYox2xXNRwDuF22Ew==")
                    {
                        VotRite.Properties.Settings1.Default.secretCipherKey = txtnopwd.Text;
                        VotRite.Properties.Settings1.Default.Save();
                        if (VotriteCrypto.Decrypt("T6b+kbzhp8kwnffkd0qqaXb+HE2vD0esMIAU5P1kg7k=") == "UAW Local 1853 District 3F")
                        {
                            verified = true;
                            if (chkExternalKey.Checked)
                                useExternalKeyboard = true;

                            try
                            {
                                Taskbar.CloseWinExplorers(Application.StartupPath);
                            }
                            catch (Exception ex)
                            {
                                MessageBox.Show(ex.Message);
                               
                            }
                        }
                        else
                        {
                            VotRite.Properties.Settings1.Default.secretCipherKey = "";
                            VotRite.Properties.Settings1.Default.Save();
                            lblMessage.Text = "Incorrect admin Password entered!!";
                            return;
                        }
                    }
                }
                else
                {
                    // MessageBox.Show("Incorrect admin Password entered!!", "Votrite Console", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    lblMessage.Text = "Incorrect admin Password entered!!";
                    return;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                verified = false;
            }
            this.Close();
        }

        private void btn_Cancel_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Votrite will exit now. Do you want to exit??", "Votrite Console", MessageBoxButtons.YesNo, MessageBoxIcon.Question) ==
                 DialogResult.Yes)
                this.Close();
        }

        private void AdminCred_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == '\u001b')
                btn_Cancel_Click(null, null);
        }

        private void txt_Cred_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == '\u001b')
                btn_Cancel_Click(null, null);
            if (e.KeyChar == '\r')
                btn_Proceed_Click(null, null);

        }

        private void pictureBox3_Click(object sender, EventArgs e)
        {

        }

        private void pictureBox4_Click(object sender, EventArgs e)
        {

        }

        private void AdminCred_Load(object sender, EventArgs e)
        {
            txt_Cred.Focus();
           
        }

        private void txt_Cred_Enter(object sender, EventArgs e)
        {
            if (chkExternalKey.Checked)
                AppManager.Instance.RunOsk();
            txt_Cred.Focus();
        }

        private void txt_Cred_Leave(object sender, EventArgs e)
        {
            AppManager.Instance.CloseOskProcess();
        }
    }
}
