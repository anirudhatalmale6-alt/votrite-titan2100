using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.SQLite;
using VotRite;
using VotRite.Util;

namespace VotRite.JimForms
{
    public partial class audit : Form
    {
        Dictionary<string, string> list = new Dictionary<string, string>();
        
        

        public audit()
        {
            InitializeComponent();
            this.Width = Screen.PrimaryScreen.WorkingArea.Width - 100;
            this.Height = Screen.PrimaryScreen.WorkingArea.Height - 100;
        }


        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.btnReset = new System.Windows.Forms.Button();
            this.lblFilterText = new System.Windows.Forms.Label();
            this.comboFilterValue = new System.Windows.Forms.ComboBox();
            this.comboFilter = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.splitContainer2 = new System.Windows.Forms.SplitContainer();
            this.splitContainer3 = new System.Windows.Forms.SplitContainer();
            this.lblCounter = new System.Windows.Forms.Label();
            this.listBoxVoters = new System.Windows.Forms.ListBox();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.panelCastVote = new System.Windows.Forms.Panel();
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.panelRecords = new System.Windows.Forms.Panel();
            this.tabPage4 = new System.Windows.Forms.TabPage();
            this.panelRecords_OrigLang = new System.Windows.Forms.Panel();
            this.tabPage3 = new System.Windows.Forms.TabPage();
            this.splitContainer4 = new System.Windows.Forms.SplitContainer();
            this.gv = new System.Windows.Forms.DataGridView();
            this.panelSearch = new System.Windows.Forms.Panel();
            this.label2 = new System.Windows.Forms.Label();
            this.txtSearch = new System.Windows.Forms.TextBox();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer2)).BeginInit();
            this.splitContainer2.Panel1.SuspendLayout();
            this.splitContainer2.Panel2.SuspendLayout();
            this.splitContainer2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer3)).BeginInit();
            this.splitContainer3.Panel1.SuspendLayout();
            this.splitContainer3.Panel2.SuspendLayout();
            this.splitContainer3.SuspendLayout();
            this.tabControl1.SuspendLayout();
            this.tabPage1.SuspendLayout();
            this.tabPage2.SuspendLayout();
            this.tabPage4.SuspendLayout();
            this.tabPage3.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer4)).BeginInit();
            this.splitContainer4.Panel1.SuspendLayout();
            this.splitContainer4.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.gv)).BeginInit();
            this.panelSearch.SuspendLayout();
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
            this.splitContainer1.Panel1.Controls.Add(this.panelSearch);
            this.splitContainer1.Panel1.Controls.Add(this.btnReset);
            this.splitContainer1.Panel1.Controls.Add(this.lblFilterText);
            this.splitContainer1.Panel1.Controls.Add(this.comboFilterValue);
            this.splitContainer1.Panel1.Controls.Add(this.comboFilter);
            this.splitContainer1.Panel1.Controls.Add(this.label1);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.splitContainer2);
            this.splitContainer1.Size = new System.Drawing.Size(1786, 946);
            this.splitContainer1.SplitterDistance = 61;
            this.splitContainer1.SplitterWidth = 5;
            this.splitContainer1.TabIndex = 0;
            // 
            // btnReset
            // 
            this.btnReset.Font = new System.Drawing.Font("Microsoft Sans Serif", 7.8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnReset.Location = new System.Drawing.Point(1278, 9);
            this.btnReset.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.btnReset.Name = "btnReset";
            this.btnReset.Size = new System.Drawing.Size(298, 42);
            this.btnReset.TabIndex = 4;
            this.btnReset.Text = "RESET ENTIRE VOTING";
            this.btnReset.UseVisualStyleBackColor = true;
            this.btnReset.Click += new System.EventHandler(this.btnReset_Click);
            // 
            // lblFilterText
            // 
            this.lblFilterText.AutoSize = true;
            this.lblFilterText.Font = new System.Drawing.Font("Microsoft Sans Serif", 7.8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblFilterText.Location = new System.Drawing.Point(1681, 0);
            this.lblFilterText.Name = "lblFilterText";
            this.lblFilterText.Size = new System.Drawing.Size(78, 20);
            this.lblFilterText.TabIndex = 3;
            this.lblFilterText.Text = "Filter by:";
            this.lblFilterText.Visible = false;
            // 
            // comboFilterValue
            // 
            this.comboFilterValue.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboFilterValue.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.comboFilterValue.FormattingEnabled = true;
            this.comboFilterValue.Items.AddRange(new object[] {
            "---SELECT---"});
            this.comboFilterValue.Location = new System.Drawing.Point(1685, 28);
            this.comboFilterValue.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.comboFilterValue.Name = "comboFilterValue";
            this.comboFilterValue.Size = new System.Drawing.Size(101, 30);
            this.comboFilterValue.TabIndex = 2;
            this.comboFilterValue.Visible = false;
            this.comboFilterValue.SelectedIndexChanged += new System.EventHandler(this.comboFilterValue_SelectedIndexChanged);
            // 
            // comboFilter
            // 
            this.comboFilter.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboFilter.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.comboFilter.FormattingEnabled = true;
            this.comboFilter.Items.AddRange(new object[] {
            "---SELECT---"});
            this.comboFilter.Location = new System.Drawing.Point(201, 18);
            this.comboFilter.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.comboFilter.Name = "comboFilter";
            this.comboFilter.Size = new System.Drawing.Size(187, 30);
            this.comboFilter.TabIndex = 1;
            this.comboFilter.SelectedIndexChanged += new System.EventHandler(this.comboFilter_SelectedIndexChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 7.8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(36, 24);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(119, 20);
            this.label1.TabIndex = 0;
            this.label1.Text = "Choose Date:";
            // 
            // splitContainer2
            // 
            this.splitContainer2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer2.FixedPanel = System.Windows.Forms.FixedPanel.Panel1;
            this.splitContainer2.Location = new System.Drawing.Point(0, 0);
            this.splitContainer2.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.splitContainer2.Name = "splitContainer2";
            // 
            // splitContainer2.Panel1
            // 
            this.splitContainer2.Panel1.Controls.Add(this.splitContainer3);
            this.splitContainer2.Panel1MinSize = 400;
            // 
            // splitContainer2.Panel2
            // 
            this.splitContainer2.Panel2.Controls.Add(this.tabControl1);
            this.splitContainer2.Panel2MinSize = 700;
            this.splitContainer2.Size = new System.Drawing.Size(1786, 880);
            this.splitContainer2.SplitterDistance = 400;
            this.splitContainer2.TabIndex = 0;
            // 
            // splitContainer3
            // 
            this.splitContainer3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer3.FixedPanel = System.Windows.Forms.FixedPanel.Panel1;
            this.splitContainer3.IsSplitterFixed = true;
            this.splitContainer3.Location = new System.Drawing.Point(0, 0);
            this.splitContainer3.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.splitContainer3.Name = "splitContainer3";
            this.splitContainer3.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainer3.Panel1
            // 
            this.splitContainer3.Panel1.Controls.Add(this.lblCounter);
            // 
            // splitContainer3.Panel2
            // 
            this.splitContainer3.Panel2.Controls.Add(this.listBoxVoters);
            this.splitContainer3.Size = new System.Drawing.Size(400, 880);
            this.splitContainer3.SplitterDistance = 25;
            this.splitContainer3.SplitterWidth = 5;
            this.splitContainer3.TabIndex = 1;
            // 
            // lblCounter
            // 
            this.lblCounter.AutoSize = true;
            this.lblCounter.Font = new System.Drawing.Font("Microsoft Sans Serif", 7.8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblCounter.Location = new System.Drawing.Point(2, 4);
            this.lblCounter.Name = "lblCounter";
            this.lblCounter.Size = new System.Drawing.Size(120, 20);
            this.lblCounter.TabIndex = 4;
            this.lblCounter.Text = "Voters Count:";
            // 
            // listBoxVoters
            // 
            this.listBoxVoters.Dock = System.Windows.Forms.DockStyle.Fill;
            this.listBoxVoters.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.listBoxVoters.FormattingEnabled = true;
            this.listBoxVoters.HorizontalScrollbar = true;
            this.listBoxVoters.ItemHeight = 25;
            this.listBoxVoters.Location = new System.Drawing.Point(0, 0);
            this.listBoxVoters.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.listBoxVoters.Name = "listBoxVoters";
            this.listBoxVoters.Size = new System.Drawing.Size(400, 850);
            this.listBoxVoters.TabIndex = 0;
            this.listBoxVoters.SelectedIndexChanged += new System.EventHandler(this.listBoxVoters_SelectedIndexChanged);
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.tabPage1);
            this.tabControl1.Controls.Add(this.tabPage2);
            this.tabControl1.Controls.Add(this.tabPage4);
            this.tabControl1.Controls.Add(this.tabPage3);
            this.tabControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControl1.Location = new System.Drawing.Point(0, 0);
            this.tabControl1.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(1382, 880);
            this.tabControl1.TabIndex = 0;
            // 
            // tabPage1
            // 
            this.tabPage1.Controls.Add(this.panelCastVote);
            this.tabPage1.Location = new System.Drawing.Point(4, 29);
            this.tabPage1.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.tabPage1.Size = new System.Drawing.Size(1374, 847);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "Cast Votes";
            this.tabPage1.UseVisualStyleBackColor = true;
            // 
            // panelCastVote
            // 
            this.panelCastVote.AutoScroll = true;
            this.panelCastVote.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelCastVote.Location = new System.Drawing.Point(3, 4);
            this.panelCastVote.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.panelCastVote.Name = "panelCastVote";
            this.panelCastVote.Size = new System.Drawing.Size(1368, 839);
            this.panelCastVote.TabIndex = 0;
            // 
            // tabPage2
            // 
            this.tabPage2.Controls.Add(this.panelRecords);
            this.tabPage2.Location = new System.Drawing.Point(4, 29);
            this.tabPage2.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Padding = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.tabPage2.Size = new System.Drawing.Size(1374, 847);
            this.tabPage2.TabIndex = 1;
            this.tabPage2.Text = "Votes Record View (English)";
            this.tabPage2.UseVisualStyleBackColor = true;
            // 
            // panelRecords
            // 
            this.panelRecords.AutoScroll = true;
            this.panelRecords.AutoSize = true;
            this.panelRecords.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelRecords.Location = new System.Drawing.Point(3, 4);
            this.panelRecords.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.panelRecords.Name = "panelRecords";
            this.panelRecords.Size = new System.Drawing.Size(1368, 839);
            this.panelRecords.TabIndex = 0;
            // 
            // tabPage4
            // 
            this.tabPage4.Controls.Add(this.panelRecords_OrigLang);
            this.tabPage4.Location = new System.Drawing.Point(4, 29);
            this.tabPage4.Name = "tabPage4";
            this.tabPage4.Size = new System.Drawing.Size(1374, 847);
            this.tabPage4.TabIndex = 3;
            this.tabPage4.Text = "Votes Record View (Original Language)";
            this.tabPage4.UseVisualStyleBackColor = true;
            // 
            // panelRecords_OrigLang
            // 
            this.panelRecords_OrigLang.AutoScroll = true;
            this.panelRecords_OrigLang.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelRecords_OrigLang.Location = new System.Drawing.Point(0, 0);
            this.panelRecords_OrigLang.Name = "panelRecords_OrigLang";
            this.panelRecords_OrigLang.Size = new System.Drawing.Size(1374, 847);
            this.panelRecords_OrigLang.TabIndex = 0;
            // 
            // tabPage3
            // 
            this.tabPage3.Controls.Add(this.splitContainer4);
            this.tabPage3.Location = new System.Drawing.Point(4, 29);
            this.tabPage3.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.tabPage3.Name = "tabPage3";
            this.tabPage3.Size = new System.Drawing.Size(1374, 847);
            this.tabPage3.TabIndex = 2;
            this.tabPage3.Text = "Report View";
            this.tabPage3.UseVisualStyleBackColor = true;
            // 
            // splitContainer4
            // 
            this.splitContainer4.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer4.Location = new System.Drawing.Point(0, 0);
            this.splitContainer4.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.splitContainer4.Name = "splitContainer4";
            this.splitContainer4.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainer4.Panel1
            // 
            this.splitContainer4.Panel1.Controls.Add(this.gv);
            this.splitContainer4.Size = new System.Drawing.Size(1374, 847);
            this.splitContainer4.SplitterDistance = 804;
            this.splitContainer4.SplitterWidth = 5;
            this.splitContainer4.TabIndex = 0;
            // 
            // gv
            // 
            this.gv.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.AllCells;
            this.gv.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.gv.Dock = System.Windows.Forms.DockStyle.Fill;
            this.gv.Location = new System.Drawing.Point(0, 0);
            this.gv.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.gv.Name = "gv";
            this.gv.RowHeadersVisible = false;
            this.gv.RowHeadersWidth = 51;
            this.gv.RowTemplate.Height = 24;
            this.gv.Size = new System.Drawing.Size(1374, 804);
            this.gv.TabIndex = 0;
            // 
            // panelSearch
            // 
            this.panelSearch.Controls.Add(this.txtSearch);
            this.panelSearch.Controls.Add(this.label2);
            this.panelSearch.Location = new System.Drawing.Point(469, 3);
            this.panelSearch.Name = "panelSearch";
            this.panelSearch.Size = new System.Drawing.Size(682, 56);
            this.panelSearch.TabIndex = 5;
            this.panelSearch.Visible = false;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 7.8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(18, 21);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(296, 30);
            this.label2.TabIndex = 1;
            this.label2.Text = "Search Voting Session:";
            // 
            // txtSearch
            // 
            this.txtSearch.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtSearch.Location = new System.Drawing.Point(225, 9);
            this.txtSearch.Multiline = true;
            this.txtSearch.Name = "txtSearch";
            this.txtSearch.Size = new System.Drawing.Size(419, 42);
            this.txtSearch.TabIndex = 2;
            this.txtSearch.TextChanged += new System.EventHandler(this.txtSearch_TextChanged);
            // 
            // audit
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1786, 946);
            this.Controls.Add(this.splitContainer1);
            this.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "audit";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "audit";
            this.Load += new System.EventHandler(this.audit_Load);
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel1.PerformLayout();
            this.splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.splitContainer2.Panel1.ResumeLayout(false);
            this.splitContainer2.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer2)).EndInit();
            this.splitContainer2.ResumeLayout(false);
            this.splitContainer3.Panel1.ResumeLayout(false);
            this.splitContainer3.Panel1.PerformLayout();
            this.splitContainer3.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer3)).EndInit();
            this.splitContainer3.ResumeLayout(false);
            this.tabControl1.ResumeLayout(false);
            this.tabPage1.ResumeLayout(false);
            this.tabPage2.ResumeLayout(false);
            this.tabPage2.PerformLayout();
            this.tabPage4.ResumeLayout(false);
            this.tabPage3.ResumeLayout(false);
            this.splitContainer4.Panel1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer4)).EndInit();
            this.splitContainer4.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.gv)).EndInit();
            this.panelSearch.ResumeLayout(false);
            this.panelSearch.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.ComboBox comboFilterValue;
        private System.Windows.Forms.ComboBox comboFilter;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.SplitContainer splitContainer2;
        private System.Windows.Forms.ListBox listBoxVoters;
        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.TabPage tabPage2;
        private System.Windows.Forms.TabPage tabPage3;
        private System.Windows.Forms.Label lblFilterText;
        private System.Windows.Forms.SplitContainer splitContainer3;
        private System.Windows.Forms.Label lblCounter;
        private System.Windows.Forms.Panel panelCastVote;
        private System.Windows.Forms.Panel panelRecords;
        private System.Windows.Forms.SplitContainer splitContainer4;
        private System.Windows.Forms.DataGridView gv;
        private TabPage tabPage4;
        private Panel panelRecords_OrigLang;
        private Panel panelSearch;
        private TextBox txtSearch;
        private Label label2;
        private System.Windows.Forms.Button btnReset;


        private void comboFilter_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                DataTable dt = new DataTable();
                if (comboFilter.SelectedIndex > 0)
                {

                    dt = DataManager.executeResultDBQuery("Select distinct voter from counter_bkup where vdate='" + comboFilter.Text + "';");
                    list = new Dictionary<string, string>();
                    list.Add("--Select--", "-1");
                    foreach (DataRow drow in dt.Rows)
                    {
                        try
                        {
                            list.Add(drow["voter"].ToString(), drow["voter"].ToString());
                        }
                        catch (Exception)
                        { }
                    }
                    listBoxVoters.DataSource = new BindingSource(list, null);
                    listBoxVoters.DisplayMember = "Key";
                    listBoxVoters.ValueMember = "Value";
                    if (list.Count > 0)
                        panelSearch.Visible = true;
                    else
                        panelSearch.Visible = false;
                }
            }
            catch (Exception ex)
            {

            }
            bindgrid();
        }

        private void comboFilterValue_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (comboFilterValue.SelectedIndex == 0)
                return;
            try
            
            {
                listBoxVoters.DataSource = null;
                listBoxVoters.Refresh();
                DataTable dt = new DataTable();
                string qry = "Select distinct voter from counter_bkup where ";
                var itm = (KeyValuePair<string,string>) comboFilterValue.SelectedItem;
                switch (comboFilter.SelectedIndex)
                {
                    case 1:
                        qry += "county='" + itm.Key + "'";
                        break;
                    case 2:
                        qry += "precint='" + itm.Value + "'";
                        break;
                    case 3:
                        qry += "district='" + itm.Value + "'";
                        break;
                    default:
                        break;
                }

                dt = DataManager.executeResultDBQuery(qry);
                list = new Dictionary<string, string>();
                list.Add("--SELECT VOTER---", "-1");
                foreach (DataRow drow in dt.Rows)
                {
                    try
                    {
                        list.Add(drow["voter"].ToString(), drow["voter"].ToString());
                    }
                    catch (Exception)
                    { }
                }
                listBoxVoters.DataSource = new BindingSource(list, null);
                listBoxVoters.DisplayMember = "Key";
                listBoxVoters.ValueMember = "Value";
            }
            catch (Exception ex)
            {

            }

            bindgrid();
        }

        private void bindgrid()
        {
            try
            {
                string query = "";
               // var itm = (KeyValuePair<string, string>)comboFilterValue.SelectedItem;
                
                    
                        query += "select vDate as VotedOn,count(DISTINCT voter) as [Voter Count], count(distinct bid) [Total Ballots], count(bid) [Total Votes] "+
                                    "from(Select distinct vDate, voter, bid FROM counter_bkup ) A group by vDate";
                       
                    
                

                var dt = DataManager.executeResultDBQuery(query);
                gv.DataSource = dt;
                gv.Refresh();
            }
            catch (Exception)
            {
            }
        }

        private void audit_Load(object sender, EventArgs e)
        {
            comboFilter.SelectedIndex = 0;
            var dt = DataManager.executeResultDBQuery("Select distinct vdate from counter_bkup");
            //var dt = DataManager.executeResultDBQuery("Select * from counter_bkup ");
            for (int d=0;d<dt.Rows.Count;d++)
            {
                comboFilter.Items.Add(dt.Rows[d][0].ToString());
            }
            
        }

        private void listBoxVoters_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (listBoxVoters.SelectedIndex <= 0)
                return;
            panelCastVote.Controls.Clear();
            panelRecords.Controls.Clear();
            panelRecords_OrigLang.Controls.Clear();
            //string reportsFolderPath = Global.Instance.APP_PATH + "records\\";
            //try
            //{
            //    foreach (var fl in Directory.GetFiles(reportsFolderPath))
            //    {
            //        try
            //        {
            //            File.Delete(fl);
            //        }
            //        catch (Exception)
            //        {

            //        }

            //    }
            //}
            //catch (Exception)
            //{

            //}

            var itm = (KeyValuePair<string, string>)listBoxVoters.SelectedItem;
            string qry = "Select distinct bid,cid,slate_id from counter_bkup where voter='" + itm.Key + "' order by bid,cid";
           
            var dt = DataManager.executeResultDBQuery(qry);

            var ballotid = "-1";

            if (dt.Rows.Count > 0)
                ballotid = dt.Rows[0]["bid"].ToString();

            qry = "Select * from contest where ballot_id='" + dt.Rows[0]["bid"] + "' ";
            var dtAll = DataManager.GetData_audit(qry);

            qry = "Select * from slates where ballot_id='" + dt.Rows[0]["bid"] + "' ";
            var dtSlates = DataManager.GetData_audit(qry);
            
           var bid = "";
            int controlbottom = 0;
            int printrecordbottom = 0;
            int printrecordbottom_origlang = 0;

            string recordsFolderPath = Global.Instance.APP_PATH + "records\\"; // Path.Combine(Application.StartupPath, "records");
            string[] files = Directory.GetFiles(recordsFolderPath, itm.Key + "*.png");
            foreach (string file in files)
            {
                try
                {
                    var fi = new FileInfo(file);
                    if (fi.Name.EndsWith("_English.png"))
                    {
                        audit_record pic = new audit_record();
                        pic.imagepath = file;

                        panelRecords.Controls.Add(pic);
                        pic.Top = printrecordbottom;
                        printrecordbottom = panelRecords.Controls[panelRecords.Controls.Count - 1].Bottom + 5;
                    }
                        
                    else
                    {
                        audit_record pic = new audit_record();
                        pic.imagepath = file;

                        panelRecords_OrigLang.Controls.Add(pic);
                        pic.Top = printrecordbottom_origlang;
                        printrecordbottom_origlang = panelRecords_OrigLang.Controls[panelRecords_OrigLang.Controls.Count - 1].Bottom + 5;
                    }
                }
                catch (Exception ex)
                {

                }
            }
            panelCastVote.Width = tabPage1.Width;

            //foreach (DataRow dr in dt.Rows)
            foreach (DataRow dr in dtAll.Rows)
            {
                //if (dr["bid"].ToString() == "" || (dr["cid"].ToString() == "" && dr["slate_id"].ToString() == ""))
                //    continue;
                if (bid != dr["ballot_id"].ToString())
                {
                    var dtb = DataManager.GetData_audit("Select * from ballot where ballot_id=" + dr["ballot_id"].ToString());
                    var name = VotriteCrypto.Decrypt(dtb.Rows[0]["top_heading"].ToString()) + Environment.NewLine + VotriteCrypto.Decrypt(dtb.Rows[0]["election_name"].ToString());
                    ballot_heading uc = new ballot_heading(name);
                    uc.Width = panelCastVote.Width;
                    panelCastVote.Controls.Add(uc);
                    bid = dr["ballot_id"].ToString();
                    uc.Top = controlbottom;
                    controlbottom = panelCastVote.Controls[panelCastVote.Controls.Count - 1].Bottom;
                    try
                    {
                        //PrinterHandler printer = new PrinterHandler();
                        //printer.ballotid = bid;
                        //printer.voterid = itm.Key;
                        //printer.bflname = bid + "_" + itm.Key + "_" + DateTime.Now.ToString("ddMMyyyyhhmmss") + ".png";
                        //printer.Print();
                       
                        //audit_record pic = new audit_record();
                        //pic.imagepath = printer.outputImage;
                        
                        //panelRecords.Controls.Add(pic);
                        //pic.Top = printrecordbottom;
                        //printrecordbottom = panelRecords.Controls[panelRecords.Controls.Count - 1].Bottom;
                    }
                    catch (Exception ex)
                    {

                    }
                }
                if (dtSlates.Rows.Count > 0)
                {
                    if (dtSlates.Select("contest_id='"+ dr["contest_id"] + "'").Length > 0)
                    {
                        contest_Slate uc = new contest_Slate();
                        uc.Width = panelCastVote.Width;
                        uc.ballotid = dr["ballot_id"].ToString();
                        uc.contestid = dr["contest_id"].ToString();
                        uc.contestname = "Slate";
                        uc.contestType = "R";
                        string qry1 = "Select distinct did from counter_bkup where voter='" + itm.Key + "' and bid='" + dr["ballot_id"] + "' and cid='" + dr["contest_id"] + "'";
                        var dt_chosen = DataManager.executeResultDBQuery(qry1);
                        string chosencandidate = "";
                        if (dt_chosen.Rows.Count > 0)
                            uc.chosenCandidate = dt_chosen.Rows[0][0].ToString();
                        //uc.chosenCandidate = dr["slate_id"].ToString();
                        uc.voterid = listBoxVoters.SelectedValue.ToString();
                        int lcid = 0;
                        try
                        {
                            Int32.TryParse(dr["report_locale_id"].ToString(), out lcid);
                        }
                        catch (Exception)
                        {
                        }

                        uc.localeid = lcid;
                        panelCastVote.Controls.Add(uc);
                        uc.Top = controlbottom;
                        controlbottom = panelCastVote.Controls[panelCastVote.Controls.Count - 1].Bottom;
                    }
                }
                    if (dr["contest_id"].ToString() != "")
                {
                    var dtctes = DataManager.GetData_audit("Select * from race where contest_id=" + dr["contest_id"].ToString()+" and locale_id=1");
                    var racename = "";
                    foreach(DataRow drr in dtctes.Rows)
                    {
                        racename = VotriteCrypto.Decrypt(drr["race_loc_title"].ToString());
                        racename += Environment.NewLine+ VotriteCrypto.Decrypt(drr["loc_voted_position"].ToString());
                    }
                    //var dtc = DataManager.GetData_audit("Select * from contest where contest_id=" + dr["contest_id"].ToString());
                    //if (dtc.Rows.Count > 0)
                    //{
                        if (dr["contest_type"].ToString() == "R")
                        {
                            string qry1 = "Select distinct did from counter_bkup where voter='" + itm.Key + "' and bid='" + dr["ballot_id"] + "' and cid='" + dr["contest_id"] + "'";
                            var dt_chosen = DataManager.executeResultDBQuery(qry1);
                            string chosencandidate = "";
                            if (dt_chosen.Rows.Count > 0)
                                chosencandidate = dt_chosen.Rows[0][0].ToString();
                            contest_Gen uc = new contest_Gen();
                            uc.Width = panelCastVote.Width;
                            uc.ballotid = dr["ballot_id"].ToString();
                            uc.contestid = dr["contest_id"].ToString();
                            uc.contestname = racename;// VotriteCrypto.Decrypt(dtc.Rows[0]["contest_generic_name"].ToString());
                            uc.contestType = "R";
                            uc.chosenCandidate = chosencandidate;
                            uc.voterid = listBoxVoters.SelectedValue.ToString();
                            panelCastVote.Controls.Add(uc);
                            uc.Top = controlbottom;
                            controlbottom = panelCastVote.Controls[panelCastVote.Controls.Count - 1].Bottom;
                        }
                    if (dr["contest_type"].ToString() == "M")
                    {
                        var prop = VotRiteBallotDataManager.AppCode.Proposition.GetProposition(Convert.ToInt32( dr["contest_id"]), 1);
                        racename = prop.Title;
                        string qry1 = "Select * from counter_bkup where voter='" + itm.Key + "' and bid='" + dr["ballot_id"] + "' and cid='" + dr["contest_id"] + "'";
                        var dt_chosen = DataManager.executeResultDBQuery(qry1);
                        string chosencandidate = "";
                        if (dt_chosen.Rows.Count > 0)
                            chosencandidate = dt_chosen.Rows[0]["preference"].ToString();
                        contest_Prop uc = new contest_Prop();
                        uc.Width = panelCastVote.Width;
                        uc.ballotid = dr["ballot_id"].ToString();
                        uc.contestid = dr["contest_id"].ToString();
                        uc.contestname = racename;// VotriteCrypto.Decrypt(dtc.Rows[0]["contest_generic_name"].ToString());
                        uc.contestType = "M";
                        uc.chosenCandidate = chosencandidate;
                        uc.voterid = listBoxVoters.SelectedValue.ToString();
                        uc.prop = prop;
                        panelCastVote.Controls.Add(uc);
                        uc.Top = controlbottom;
                        controlbottom = panelCastVote.Controls[panelCastVote.Controls.Count - 1].Bottom;
                    }
                    if (dr["contest_type"].ToString() == "V")
                        {
                            contest_Rank uc = new contest_Rank();
                            uc.Width = panelCastVote.Width;
                            uc.ballotid = dr["ballot_id"].ToString();
                            uc.contestid = dr["contest_id"].ToString();
                            uc.contestname = VotriteCrypto.Decrypt(dr["contest_generic_name"].ToString());
                            uc.contestType = "V";
                            uc.voterid = listBoxVoters.SelectedValue.ToString();
                            panelCastVote.Controls.Add(uc);
                            uc.Top = controlbottom;
                            controlbottom = panelCastVote.Controls[panelCastVote.Controls.Count - 1].Bottom;
                        }
                   // }
                }
            }
        }

        private void btnReset_Click(object sender, EventArgs e)
        {
            
            SupervisorCred frmPollcred = new SupervisorCred();
            frmPollcred.ShowDialog();
            if (!frmPollcred.ok)
                return;

            try
            {
                resetoptions frm = new resetoptions();
                frm.ShowDialog();

                //DialogResult dlgRes = MessageBox.Show("Are you sure to reset entire Voting?", "Reset Voting",
                //                                         MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question);
                //if (dlgRes == DialogResult.Cancel)
                //    return;
                if (frm.options == 0)
                    return;

                //if (dlgRes == DialogResult.Yes)
                if (frm.options == 1)
                {
                    int c = DataManager.executeResultDBNonQuery("delete from cast");
                    c = DataManager.executeResultDBNonQuery("delete from counter");
                    //c = DataManager.executeResultDBNonQuery("delete from cast_bkup");
                    //c = DataManager.executeResultDBNonQuery("delete from counter_bkpu");


                    Util.IOHandler.SaveConfig("System", "Machine", "");
                    AppManager.Configuration = Util.IOHandler.DecryptConfig();

                    AppManager.UpdateMachineValue();

                    tabPage1.Controls.Clear();
                    tabPage2.Controls.Clear();
                    tabPage3.Controls.Clear();
                    listBoxVoters.Items.Clear();

                    MessageBox.Show("Entire Voting is reset, please proceed with choosing machine in Voting menu?", "Reset Entire Voting",
                                                         MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    BallotsListDisplay frmB = new BallotsListDisplay(BallotsListDisplay.displayMode.Reset);
                    frmB.ShowDialog();
                    if (frmB.selectedBallots.Count == 0)
                    {
                        MessageBox.Show("No ballot(s) selected. Please choose atleast one option.", "Reset Voting",
                                                             MessageBoxButtons.OK, MessageBoxIcon.Information);
                        return;
                    }
                    foreach (var ballt in frmB.selectedBallots)
                    {
                        int c = DataManager.executeResultDBNonQuery("delete from cast where ballotId=" + ballt.Id);
                        c = DataManager.executeResultDBNonQuery("delete from counter where bid=" + ballt.Id);
                        //c = DataManager.executeResultDBNonQuery("delete from cast_bkup");
                        //c = DataManager.executeResultDBNonQuery("delete from counter_bkpu");
                    }

                    //Util.IOHandler.SaveConfig("System", "Machine", "");
                    //AppManager.Configuration = Util.IOHandler.DecryptConfig();

                    //AppManager.UpdateMachineValue();

                    tabPage1.Controls.Clear();
                    tabPage2.Controls.Clear();
                    tabPage3.Controls.Clear();
                    listBoxVoters.Items.Clear();
                    //audit_Load(null, null);
                }
            }
            catch (Exception ex)
            {

            }

            //}
        }

        private void txtSearch_TextChanged(object sender, EventArgs e)
        {
            try
            {
                if (txtSearch.Text.Length == 0)
                {
                    listBoxVoters.DataSource = new BindingSource(list, null);
                    listBoxVoters.DisplayMember = "Key";
                    listBoxVoters.ValueMember = "Value";
                }
                else
                {
                    try
                    {
                        var sublist = new Dictionary<string, string>();
                        sublist.Add("--Select--", "-1");
                        var searched = list.Where(d => d.Key.Contains(txtSearch.Text));

                        foreach (var lst in searched)
                            sublist.Add(lst.Key, lst.Value);

                        listBoxVoters.DataSource = new BindingSource(sublist, null);
                        listBoxVoters.DisplayMember = "Key";
                        listBoxVoters.ValueMember = "Value";
                    }
                    catch (Exception)
                    {

                    }
                }
            }
            catch (Exception)
            {
            }
        }
    }
}
