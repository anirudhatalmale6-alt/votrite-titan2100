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
    public partial class ConfigForm_Ballots : Form
    {
        public int SN = -1;
        public string machine = "";
        public bool _default = false;
        public enum displayMode
        {
            Display,
            Report
        }
        public displayMode mode;
        public List<VotRiteBallotDataManager.AppCode.Ballot> selectedBallots = new List<VotRiteBallotDataManager.AppCode.Ballot>();

        public ConfigForm_Ballots()
        {
            mode = displayMode.Display;
            InitializeComponent();
        }
        public ConfigForm_Ballots(displayMode _mode)
        {
            mode = _mode;
            InitializeComponent();
        }

        private void BallotsListDisplay_Load(object sender, EventArgs e)
        {
            var list = VotRiteBallotDataManager.AppCode.Ballot.GetBallots();
            var mblist = VotRiteBallotDataManager.AppCode.MachineBallot.GetMachineBallots(SN);
            if (mode == displayMode.Report)
            {
                chklist_ballot.Items.Add(new VotRiteBallotDataManager.AppCode.Ballot() { Id = -1, ElectionName = "ALL BALLOT REPORTS" });
                chklist_ballot.Items.Add(new VotRiteBallotDataManager.AppCode.Ballot() { Id = -2, ElectionName = "CONSOLIDATED REPORT" });
            }
            //this.chklist_ballot.DataSource = list;
            foreach (var item in list)
            {
                if(mode == displayMode.Display)
                {
                    if (item.EndTime <= DateTime.Now)
                        continue;
                }

                //var cnt = DataManager.VotingResultsDataInstance.GetScalarData("select sum(cnt) from cast where ballotId="+item.Id);
                item.ElectionName = item.ElectionName;// + " (Total Cast: " + (cnt.ToString() == "" ? "0" : cnt.ToString()) + ")";
                chklist_ballot.Items.Add(item);
                if ( mblist.FindAll(a => a.Ballot_ID == item.Id).Count > 0)
                    chklist_ballot.SetItemChecked(chklist_ballot.Items.Count - 1, true);
            }
            this.chklist_ballot.DisplayMember = "ElectionName";

            lblHeader.Text += machine;
            try
            {
                lblHeader.Left = this.Width / 2 - (lblHeader.Width / 2);
            }
            catch (Exception)
            {

            }
            
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            var items = chklist_ballot.CheckedItems;

            if (items.Count > 0)
            {
                lblStatus.Text = "Updating Ballots for Machine: " + machine + " ......";
                if (VotRiteBallotDataManager.AppCode.MachineBallot.RemoveMachineBallots(SN))
                {
                    foreach (VotRiteBallotDataManager.AppCode.Ballot item in items)
                    {
                        var updated = VotRiteBallotDataManager.AppCode.MachineBallot.InsertMachineBallots(new VotRiteBallotDataManager.AppCode.MachineBallot()
                        {
                            Ballot_ID = item.Id,
                            Machine_SN = SN
                        });
                    }
                }
                _default = true;
                lblStatus.Text = "Ballots are saved for Machine: " + machine;
            }
            else
                MessageBox.Show("No Ballot is selected!!", "Votrite Machine Ballots", MessageBoxButtons.OK, MessageBoxIcon.Error);
            //

        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
           // _default = false;
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
            //if (mode == displayMode.Display)
            //{
            //    if (e.NewValue == CheckState.Checked && chklist_ballot.CheckedItems.Count > 0)
            //    {
            //        chklist_ballot.ItemCheck -= chklist_ballot_ItemCheck;
            //        chklist_ballot.SetItemChecked(chklist_ballot.CheckedIndices[0], false);
            //        chklist_ballot.ItemCheck += chklist_ballot_ItemCheck;
            //    }
            //}
        }
    }
}
