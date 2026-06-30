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
    public partial class BallotsListDisplay : Form
    {
        public bool _default = false;
        public enum displayMode
        {
            Display,
            Report,
            Reset
        }
        public displayMode mode;
        public List<VotRiteBallotDataManager.AppCode.Ballot> selectedBallots = new List<VotRiteBallotDataManager.AppCode.Ballot>();
        public object _ballotMode;
        public int copy = 1;

        public BallotsListDisplay()
        {
            mode = displayMode.Display;
            InitializeComponent();
        }
        public BallotsListDisplay(displayMode _mode)
        {
            mode = _mode;
            InitializeComponent();
        }

        private void BallotsListDisplay_Load(object sender, EventArgs e)
        {
            if (mode == displayMode.Report || mode == displayMode.Reset)
            {
                groupBox1.Text = "Choose Report Printing Option(s)";
                
                btnBallotMode.Visible = false;
                panelFont.Visible = true;
                comboFont.SelectedIndex = 0;
                chk_showRecordtool.Visible = true;
            }
            if (mode == displayMode.Reset)
            {
                groupBox1.Text = "Choose ballot(s) to reset";
                btnBallotMode.Visible = false;
                panelFont.Visible = false;
               // comboFont.SelectedIndex = 0;
                chk_showRecordtool.Visible = false;
            }
            var list = VotRiteBallotDataManager.AppCode.Ballot.GetBallots();
            var mc = VotRiteBallotDataManager.AppCode.MachineConfig.GetMachineConfig(AppManager.Configuration["System"]["Machine"]);
            if (mc.Count == 0)
            {
                MessageBox.Show("Machine configuration is not found or not accessible, please use Configuration menu to update!!", "Error",
                                                      MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            var mblist = VotRiteBallotDataManager.AppCode.MachineBallot.GetMachineBallots(mc[0].SN);
            if (mode == displayMode.Report)
            {
               // chklist_ballot.Items.Add(new VotRiteBallotDataManager.AppCode.Ballot() { Id = -1, ElectionName = "ALL BALLOT REPORTS" });
               // chklist_ballot.Items.Add(new VotRiteBallotDataManager.AppCode.Ballot() { Id = -2, ElectionName = "CONSOLIDATED REPORT" });

                //chklist_ballot.Items.
            }
            //this.chklist_ballot.DataSource = list;
            foreach (var item in list)
            {
                if(mode == displayMode.Display)
                {
                    if (item.EndTime <= DateTime.Now)
                        continue;
                }

                var cnt = DataManager.VotingResultsDataInstance.GetScalarData("select sum(cnt) from cast where ballotId="+item.Id);
                if (cnt == null)
                    cnt = 0;
                item.ElectionName = (mode == displayMode.Report ? "Ballot: " : "")+ item.ElectionName + " (Total Cast: " + (cnt.ToString() == "" ? "0" : cnt.ToString()) + ")";
                if (mode == displayMode.Display)
                {
                    if (mblist.FindAll(a => a.Ballot_ID == item.Id).Count > 0)
                    {
                        chklist_ballot.Items.Add(item);
                        if (item.IsDefaultVote)
                            chklist_ballot.SetItemChecked(chklist_ballot.Items.Count - 1, true);
                    }
                }
                else
                {
                    chklist_ballot.Items.Add(item);
                }
            }
            this.chklist_ballot.DisplayMember = "ElectionName";
            this.comboCopy.SelectedIndex = 0;
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            var items = chklist_ballot.CheckedItems;
            if (mode == displayMode.Display)
            {
                if (items.Count > 0)
                {
                    var item = (VotRiteBallotDataManager.AppCode.Ballot)items[0];
                    var updated = VotRiteBallotDataManager.AppCode.Ballot.UpdateBallotDefault(item.Id);
                    if (updated)
                        _default = true;
                    else
                        _default = false;
                    this.Close();
                }
                else
                    MessageBox.Show("Please choose a Ballot to open!");
            }
            else
            {
                if (mode == displayMode.Report)
                {
                    float fctr = 0f;
                    switch (comboFont.SelectedIndex)
                    {
                        case 1:
                            fctr = +3f;
                            break;
                        case 2:
                            fctr = +2f;
                            break;
                        case 3:
                            fctr = +4f;
                            break;
                        case 4:
                            fctr = -2f;
                            break;
                        case 5:
                            fctr = -4f;
                            break;
                        case 6:
                            fctr = -6f;
                            break;

                        default:
                            break;
                    }
                    AppManager.Instance.reportFontSizeFactor = fctr;

                    copy = Convert.ToInt32(comboCopy.Text);
                }
                foreach (var item in items)
                    selectedBallots.Add((VotRiteBallotDataManager.AppCode.Ballot)item);
                this.Close();
            }
                                        //_default = true;

        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            _default = false;
            this.Close();
        }

        private void chklist_ballot_SelectedIndexChanged(object sender, EventArgs e)
        {
            
        }

        private void unselectall()
        {

        }

        private void chklist_ballot_ItemCheck(object sender, ItemCheckEventArgs e)
        {
            if (mode == displayMode.Display)
            {
                if (e.NewValue == CheckState.Checked && chklist_ballot.CheckedItems.Count > 0)
                {
                    chklist_ballot.ItemCheck -= chklist_ballot_ItemCheck;
                    chklist_ballot.SetItemChecked(chklist_ballot.CheckedIndices[0], false);
                    chklist_ballot.ItemCheck += chklist_ballot_ItemCheck;
                }
            }
        }

        private void chklist_ballot_KeyPress(object sender, KeyPressEventArgs e)
        {
            if(e.KeyChar == '\r')
            {
                btnSave_Click(null, null);
            }
        }

        private void btnBallotMode_Click(object sender, EventArgs e)
        {
            var items = chklist_ballot.CheckedItems;
            
                if (items.Count > 0)
                {
                    var item = (VotRiteBallotDataManager.AppCode.Ballot)items[0];
                    var updated = VotRiteBallotDataManager.AppCode.Ballot.UpdateBallotDefault(item.Id);
                    if (updated)
                        _default = true;
                    else
                        _default = false;
                BallotMode frmmode = new BallotMode();
                frmmode.ShowDialog();
                if (frmmode.normal)
                    _ballotMode = Session.BallotModes.Normal;
                else
                    _ballotMode = Session.BallotModes.Audio;
                this.Close();
                }
                else
                    MessageBox.Show("Please choose a Ballot to open!");
            

        }

        private void btnSettings_Click(object sender, EventArgs e)
        {
            AppManager.Instance.SetScreen("settingTools","Review");
        }

        private void chk_showRecordtool_CheckedChanged(object sender, EventArgs e)
        {
            if(chk_showRecordtool.Checked)
            {
                RecordReportPrint frmRecord = new RecordReportPrint();
                frmRecord.StartPosition = FormStartPosition.CenterParent;
                frmRecord.ShowDialog();
                chk_showRecordtool.Checked = false;
            }
        }
    }
}
