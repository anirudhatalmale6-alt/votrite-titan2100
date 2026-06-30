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
    public partial class ConfigForm_Machine : Form
    {
        public bool _set = false;
        public ConfigForm_Machine()
        {
            InitializeComponent();
        }

        private void ConfigForm_Machine_Load(object sender, EventArgs e)
        {
            try
            {
                var list = VotRiteBallotDataManager.AppCode.MachineConfig.GetMachineConfigs();
                cmboMachines.DataSource = list;
                cmboMachines.DisplayMember = "MachineNo";

                if (list.Count > 0)
                    cmboMachines.SelectedIndex = 0;
            }
            catch (Exception)
            {

            }
           
        }

        private void saveNewMachineConfig(VotRiteBallotDataManager.AppCode.MachineConfig mc)
        {
            Util.IOHandler.SaveConfig("System", "Machine", mc.MachineNo);
            Util.IOHandler.SaveConfig("System",
                                      "BackupFlashDrivePath",
                                      Value: mc.BackupDrive);
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            if( MessageBox.Show("Are you sure to set "+ cmboMachines.Text + " as your Machine?", "Votrite Machine Ballots", MessageBoxButtons.YesNo, MessageBoxIcon.Question) ==
                 DialogResult.Yes)
            {
                try
                {
                    lblStatus.Text = "Updating, please wait ..";
                    lblStatus.Refresh();

                    var mc = VotRiteBallotDataManager.AppCode.MachineConfig.GetMachineConfig(cmboMachines.Text);
                    saveNewMachineConfig(mc[0]);
                    //VotRiteBallotDataManager.AppCode.Ballot.UpdateMachineValue_Location(mc[0].MachineNo, mc[0].Location);
                    // string bkpval = mc[0].BackupDrive;
                    //Util.IOHandler.SaveConfig("System", "Machine", mc[0].MachineNo);

                    AppManager.Configuration = Util.IOHandler.DecryptConfig();

                    AppManager.UpdateMachineValue();
                    
                    _set = true;

                    lblStatus.Text = "Updated successfully";
                    lblStatus.Refresh();
                    this.Close();
                }
                catch (Exception)
                {
                    lblStatus.Text = "Could not update, please try again!!";
                    lblStatus.Refresh();
                }
                
            }
        }
    }
}
