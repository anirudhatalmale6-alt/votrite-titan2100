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
    public partial class CloseOrReset : Form
    {
        public enum CloseOrResetOption
        {
            CloseApplication,
            ResetVoting
                
        }

        public CloseOrResetOption option;
        public CloseOrReset()
        {
            InitializeComponent();
        }

        private void CloseOrReset_Load(object sender, EventArgs e)
        {

        }

        private void btnResetVoting_Click(object sender, EventArgs e)
        {
            lblMsg.Text = "Please wait...";
            this.Refresh();
            option = CloseOrResetOption.ResetVoting;
            Util.IOHandler.SaveConfig("System", "Machine", "");
            AppManager.Configuration = Util.IOHandler.DecryptConfig();

            AppManager.UpdateMachineValue();
            this.Close();
        }

        private void btnExitApp_Click(object sender, EventArgs e)
        {
            option = CloseOrResetOption.CloseApplication;
            this.Close();
        }
    }
}
