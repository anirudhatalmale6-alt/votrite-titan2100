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
using VotRite.DBClasses;
using VotRite.Util;

namespace VotRite.JimForms
{
    public partial class contest_Slate : UserControl
    {
        public string ballotid;
        public string contestid, contestname;
        public string voterid;
        public string contestType; // R = choices, V=Ranking, S=Slates
        public string chosenCandidate = "";
        public int localeid;
        public contest_Slate()
        {
            InitializeComponent();
        }
       
        private void splitContainer1_Panel1_Paint(object sender, PaintEventArgs e)
        {

        }

        private void contest_record_view_Load(object sender, EventArgs e)
        {
            lblContest.Text = contestname;

            var slates = Slate.GetSlates(Convert.ToInt32(ballotid),localeid);

            foreach (Slate dr in slates)
            {
                checkedListBoxCandidates.Items.Add(dr.Name);
                if (dr.Id.ToString() == chosenCandidate)
                    checkedListBoxCandidates.SelectedIndex = checkedListBoxCandidates.Items.Count - 1;
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
            this.pictureBoxRecord = new System.Windows.Forms.PictureBox();
            this.checkedListBoxCandidates = new System.Windows.Forms.ListBox();
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
            this.splitContainer1.Size = new System.Drawing.Size(1040, 530);
            this.splitContainer1.SplitterDistance = 44;
            this.splitContainer1.TabIndex = 0;
            // 
            // lblContest
            // 
            this.lblContest.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblContest.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblContest.Location = new System.Drawing.Point(0, 0);
            this.lblContest.Name = "lblContest";
            this.lblContest.Size = new System.Drawing.Size(1040, 44);
            this.lblContest.TabIndex = 1;
            this.lblContest.Text = "Contest:";
            // 
            // pictureBoxRecord
            // 
            this.pictureBoxRecord.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pictureBoxRecord.Location = new System.Drawing.Point(0, 0);
            this.pictureBoxRecord.Name = "pictureBoxRecord";
            this.pictureBoxRecord.Size = new System.Drawing.Size(1040, 482);
            this.pictureBoxRecord.TabIndex = 0;
            this.pictureBoxRecord.TabStop = false;
            // 
            // checkedListBoxCandidates
            // 
            this.checkedListBoxCandidates.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.checkedListBoxCandidates.FormattingEnabled = true;
            this.checkedListBoxCandidates.ItemHeight = 25;
            this.checkedListBoxCandidates.Location = new System.Drawing.Point(5, 3);
            this.checkedListBoxCandidates.Name = "checkedListBoxCandidates";
            this.checkedListBoxCandidates.Size = new System.Drawing.Size(567, 329);
            this.checkedListBoxCandidates.TabIndex = 1;
            // 
            // contest_Slate
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.splitContainer1);
            this.Name = "contest_Slate";
            this.Size = new System.Drawing.Size(1040, 530);
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
        private System.Windows.Forms.Label lblContest;
        private System.Windows.Forms.PictureBox pictureBoxRecord;
        private System.Windows.Forms.ListBox checkedListBoxCandidates;
    }
}
