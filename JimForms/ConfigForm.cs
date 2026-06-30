using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using VotRiteBallotDataManager.AppCode;

namespace VotRite.JimForms
{
    public partial class ConfigForm : Form
    {
        int ed_SN = -1;
        string ed_MachineNo = "";

        public ConfigForm()
        {
            InitializeComponent();
        }

        private void ConfigForm_Load(object sender, EventArgs e)
        {
            loadMachineConfigs();
            Resized();

            lbl_Log.Text = "LOG: "+ AppManager.Instance.LogCounter.ToString();

        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            if(txtMachineNo.Text == "" || txtLocation.Text == "" || txtBackupDrive.Text == "")
            {
                MessageBox.Show("Please enter all required fields!!", "Votrite Machine Configuration", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            if (ed_MachineNo != txtMachineNo.Text)
            {
                var list = MachineConfig.GetMachineConfig(txtMachineNo.Text);

                if (list.Count > 0)
                {
                    MessageBox.Show("Machine Number " + txtMachineNo.Text + " already exists, please choose different name!!", "Votrite Machine Configuration", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
            }
            MachineConfig mc = new MachineConfig()
            {
                
                MachineNo = txtMachineNo.Text,
                Location = txtLocation.Text,
                BackupDrive = txtBackupDrive.Text,
                MainLocation = txtMainLocation.Text
            };
            if (btnAdd.Text == "ADD")
            {
                if (MachineConfig.InsertMachineConfig(mc))
                {
                    loadMachineConfigs();
                }
            }
            else
            {
                mc.SN = ed_SN;
                if (MachineConfig.UpdateMachineConfig(mc))
                {
                    loadMachineConfigs();
                    Reset();
                }
            }
        }

        private void loadMachineConfigs()
        {
            try
            {
                gv.DataSource = null;
                gv.DataSource = MachineConfig.GetMachineConfigs();
                if (gv.Columns.Count > 0)
                    gv.Columns[2].Visible = false;
            }
            catch (Exception)
            {
            }
        }

        private void Resized()
        {
            lblHead.Left = this.Width / 2 - (lblHead.Width / 2);
        }

        private void ConfigForm_Resize(object sender, EventArgs e)
        {
            Resized();
        }

        private void Reset()
        {
            ed_SN = -1;
            ed_MachineNo = "";
            btnAdd.Text = "ADD";
            txtMachineNo.Text = "";
            txtLocation.Text = "";
            txtMainLocation.Text = "";
            txtBackupDrive.Text = "";
            linkCancel.Visible = false;
            linkDelete.Visible = false;
        }

        private void gv_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.ColumnIndex == 0)
            {
                int r = e.RowIndex;

                ed_SN = Convert.ToInt32(gv.Rows[r].Cells[2].Value);
                ed_MachineNo= txtMachineNo.Text = gv.Rows[r].Cells[3].Value.ToString();
                txtLocation.Text = gv.Rows[r].Cells[4].Value.ToString();
                txtMainLocation.Text = gv.Rows[r].Cells[5].Value.ToString();
                txtBackupDrive.Text = gv.Rows[r].Cells[6].Value.ToString();
                btnAdd.Text = "UPDATE";
                linkCancel.Visible = true;
                linkDelete.Visible = true;
            }
            if(e.ColumnIndex == 1)
            {
                int r = e.RowIndex;                
                ConfigForm_Ballots frm = new ConfigForm_Ballots();
                frm.SN = Convert.ToInt32(gv.Rows[r].Cells[2].Value);
                frm.machine = gv.Rows[r].Cells[3].Value.ToString();
                frm.ShowDialog();
                if (frm._default)
                    loadMachineConfigs();
            }
        }

        private void linkCancel_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Reset();
        }

        private void linkDelete_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            if (MessageBox.Show("Are you sure to remove machine: " + txtMachineNo.Text + " ?", "Votrite Machine Configuration", MessageBoxButtons.YesNo, MessageBoxIcon.Question)
                 == DialogResult.Yes)
            {
                if (!MachineConfig.RemoveMachineConfig(ed_SN))
                {
                    MessageBox.Show("Could not remove machine!!", "Votrite Machine Configuration", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                else
                {
                    MachineBallot.RemoveMachineBallots(ed_SN);
                    Reset();
                    loadMachineConfigs();
                }
            }
        }

        private void chkRecordHeader_CheckedChanged(object sender, EventArgs e)
        {
            if (chkRecordHeader.Checked)
                AppManager.Instance.headerInRecordImage = true;
            else
                AppManager.Instance.headerInRecordImage = false;
        }

        private void btnHelp_Click(object sender, EventArgs e)
        {
            F_HelpSetting oHelp = new F_HelpSetting();
            oHelp.ShowDialog();
        }
    }
}
