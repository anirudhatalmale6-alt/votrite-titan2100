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
    public partial class contest_Rank : UserControl
    {
        public string ballotid;
        public string contestid, contestname;
        public string voterid;
        public string contestType; // R = choices, V=Ranking, S=Slates
        public string chosenCandidate = "";
        public contest_Rank()
        {
            InitializeComponent();
        }
       
        private void splitContainer1_Panel1_Paint(object sender, PaintEventArgs e)
        {

        }

        private void contest_record_view_Load(object sender, EventArgs e)
        {
            lblContest.Text = contestname;
            string qry = "Select did,preference from counter where voter='" + voterid + "' and bid='"+ballotid+"' and cid='"+contestid+"'";
            var dt = DataManager.executeResultDBQuery(qry);
            DataTable dtcand = new DataTable();
            dtcand.Columns.Add("Candidate");
            dtcand.Columns.Add("Preference");
            foreach (DataRow drow in dt.Rows)
            {
                var dtc = DataManager.GetData_audit("Select distinct b.cand_name from candidate_name b where b.cand_id=" + drow["did"]);
                foreach (DataRow dr in dtc.Rows)
                {
                    var nrow = dtcand.NewRow();
                    nrow["Candidate"] = VotriteCrypto.Decrypt( dr["cand_name"].ToString());
                    nrow["Preference"] = drow["preference"];
                    dtcand.Rows.Add(nrow);
                }
            }
            dataGridView1.DataSource = dtcand;
            sizeDGV(dataGridView1);

            this.Height = lblContest.Height + dataGridView1.Height;
            this.Width = this.Parent.Width;
        }

        void sizeDGV(DataGridView dgv)
        {
            DataGridViewElementStates states = DataGridViewElementStates.None;
            dgv.ScrollBars = ScrollBars.None;
            var totalHeight = dgv.Rows.GetRowsHeight(states);// + (dgv.ColumnHeadersHeight/2);
            totalHeight += dgv.Rows.Count * 4;  // a correction I need
            var totalWidth = dgv.Columns.GetColumnsWidth(states);// + dgv.RowHeadersWidth;
            dgv.ClientSize = new Size(totalWidth, totalHeight);
        }


        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle5 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle6 = new System.Windows.Forms.DataGridViewCellStyle();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.dataGridView1 = new System.Windows.Forms.DataGridView();
            this.lblContest = new System.Windows.Forms.Label();
            this.pictureBoxRecord = new System.Windows.Forms.PictureBox();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).BeginInit();
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
            this.splitContainer1.Panel2.Controls.Add(this.dataGridView1);
            this.splitContainer1.Panel2.Controls.Add(this.pictureBoxRecord);
            this.splitContainer1.Size = new System.Drawing.Size(1040, 485);
            this.splitContainer1.SplitterDistance = 45;
            this.splitContainer1.TabIndex = 0;
            // 
            // dataGridView1
            // 
            this.dataGridView1.AllowUserToAddRows = false;
            this.dataGridView1.AllowUserToDeleteRows = false;
            this.dataGridView1.AllowUserToResizeColumns = false;
            this.dataGridView1.AllowUserToResizeRows = false;
            dataGridViewCellStyle5.Font = new System.Drawing.Font("Microsoft Sans Serif", 13.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle5.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dataGridView1.AlternatingRowsDefaultCellStyle = dataGridViewCellStyle5;
            this.dataGridView1.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.dataGridView1.AutoSizeRowsMode = System.Windows.Forms.DataGridViewAutoSizeRowsMode.DisplayedCells;
            this.dataGridView1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView1.Location = new System.Drawing.Point(3, 3);
            this.dataGridView1.Name = "dataGridView1";
            this.dataGridView1.RowHeadersVisible = false;
            this.dataGridView1.RowHeadersWidth = 51;
            dataGridViewCellStyle6.Font = new System.Drawing.Font("Microsoft Sans Serif", 13.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle6.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dataGridView1.RowsDefaultCellStyle = dataGridViewCellStyle6;
            this.dataGridView1.RowTemplate.Height = 24;
            this.dataGridView1.Size = new System.Drawing.Size(495, 391);
            this.dataGridView1.TabIndex = 2;
            // 
            // lblContest
            // 
            this.lblContest.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblContest.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblContest.Location = new System.Drawing.Point(0, 0);
            this.lblContest.Name = "lblContest";
            this.lblContest.Size = new System.Drawing.Size(1040, 45);
            this.lblContest.TabIndex = 1;
            this.lblContest.Text = "Contest:";
            // 
            // pictureBoxRecord
            // 
            this.pictureBoxRecord.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pictureBoxRecord.Location = new System.Drawing.Point(0, 0);
            this.pictureBoxRecord.Name = "pictureBoxRecord";
            this.pictureBoxRecord.Size = new System.Drawing.Size(1040, 436);
            this.pictureBoxRecord.TabIndex = 0;
            this.pictureBoxRecord.TabStop = false;
            // 
            // contest_Rank
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.splitContainer1);
            this.Name = "contest_Rank";
            this.Size = new System.Drawing.Size(1040, 485);
            this.Load += new System.EventHandler(this.contest_record_view_Load);
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxRecord)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.Label lblContest;
        private System.Windows.Forms.PictureBox pictureBoxRecord;
        private System.Windows.Forms.DataGridView dataGridView1;
    }
}
