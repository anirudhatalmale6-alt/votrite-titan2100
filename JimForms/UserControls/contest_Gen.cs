using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SQLite;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using VotRite;
using VotRite.Util;

namespace VotRite.JimForms
{
    public partial class contest_Gen : UserControl
    {
        public string ballotid;
        public string contestid, contestname;
        public string voterid;
        public string contestType; // R = choices, V=Ranking, S=Slates
        public string chosenCandidate = "";
        public contest_Gen()
        {
            InitializeComponent();
        }
       
        private void splitContainer1_Panel1_Paint(object sender, PaintEventArgs e)
        {

        }

        private void contest_record_view_Load(object sender, EventArgs e)
        {
            lblContest.Text = contestname;
            
            var dtc = DataManager.GetData_audit("Select distinct a.cand_id,b.cand_name from candidates a inner join candidate_name b on a.cand_id=b.cand_id where a.contest_id=" + contestid);
            string cand = "";
            foreach (DataRow dr in dtc.Rows)
            {
                cand = VotriteCrypto.Decrypt(dr["cand_name"].ToString());
                if (dr["cand_id"].ToString() != chosenCandidate)
                    checkedListBoxCandidates.Items.Add(cand, false);
                else
                    checkedListBoxCandidates.Items.Add(cand, true);
            }
            //checkedListBoxCandidates.ba
            checkedListBoxCandidates.IntegralHeight = true;
            int h = checkedListBoxCandidates.ItemHeight * checkedListBoxCandidates.Items.Count;
            checkedListBoxCandidates.Height = h + checkedListBoxCandidates.Height - checkedListBoxCandidates.ClientSize.Height;
            this.Height = lblContest.Height + checkedListBoxCandidates.Height;
            //checkedListBoxCandidates.Enabled = true;
            this.Width = this.Parent.Width;
        }


        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.lblContest = new System.Windows.Forms.Label();
            this.checkedListBoxCandidates = new System.Windows.Forms.CheckedListBox();
            this.pictureBoxRecord = new System.Windows.Forms.PictureBox();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxRecord)).BeginInit();
            this.SuspendLayout();
            // 
            // splitContainer1
            // 
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.Location = new System.Drawing.Point(0, 0);
            this.splitContainer1.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.splitContainer1.Name = "splitContainer1";
            this.splitContainer1.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.lblContest);
            this.splitContainer1.Panel1.Paint += new System.Windows.Forms.PaintEventHandler(this.splitContainer1_Panel1_Paint);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.checkedListBoxCandidates);
            this.splitContainer1.Panel2.Controls.Add(this.pictureBoxRecord);
            this.splitContainer1.Size = new System.Drawing.Size(1170, 662);
            this.splitContainer1.SplitterDistance = 54;
            this.splitContainer1.SplitterWidth = 5;
            this.splitContainer1.TabIndex = 0;
            // 
            // lblContest
            // 
            this.lblContest.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblContest.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblContest.Location = new System.Drawing.Point(0, 0);
            this.lblContest.Name = "lblContest";
            this.lblContest.Size = new System.Drawing.Size(1170, 54);
            this.lblContest.TabIndex = 1;
            this.lblContest.Text = "Contest:";
            // 
            // checkedListBoxCandidates
            // 
            this.checkedListBoxCandidates.Dock = System.Windows.Forms.DockStyle.Fill;
            this.checkedListBoxCandidates.Enabled = false;
            this.checkedListBoxCandidates.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.checkedListBoxCandidates.FormattingEnabled = true;
            this.checkedListBoxCandidates.Location = new System.Drawing.Point(0, 0);
            this.checkedListBoxCandidates.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.checkedListBoxCandidates.Name = "checkedListBoxCandidates";
            this.checkedListBoxCandidates.Size = new System.Drawing.Size(1755, 905);
            this.checkedListBoxCandidates.TabIndex = 2;
            // 
            // pictureBoxRecord
            // 
            this.pictureBoxRecord.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pictureBoxRecord.Location = new System.Drawing.Point(0, 0);
            this.pictureBoxRecord.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.pictureBoxRecord.Name = "pictureBoxRecord";
            this.pictureBoxRecord.Size = new System.Drawing.Size(1170, 603);
            this.pictureBoxRecord.TabIndex = 0;
            this.pictureBoxRecord.TabStop = false;
            // 
            // contest_Gen
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.splitContainer1);
            this.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.Name = "contest_Gen";
            this.Size = new System.Drawing.Size(1170, 662);
            this.Load += new System.EventHandler(this.contest_record_view_Load);
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxRecord)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.CheckedListBox checkedListBoxCandidates;
        private System.Windows.Forms.Label lblContest;
        private System.Windows.Forms.PictureBox pictureBoxRecord;
    }
}
