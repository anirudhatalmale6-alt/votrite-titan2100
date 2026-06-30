namespace VotRite.JimForms
{
    partial class BallotsListDisplay
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(BallotsListDisplay));
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.chklist_ballot = new System.Windows.Forms.CheckedListBox();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.panel1 = new System.Windows.Forms.Panel();
            this.chk_showRecordtool = new System.Windows.Forms.CheckBox();
            this.panelFont = new System.Windows.Forms.Panel();
            this.label1 = new System.Windows.Forms.Label();
            this.comboFont = new System.Windows.Forms.ComboBox();
            this.btnBallotMode = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.btnSave = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.comboCopy = new System.Windows.Forms.ComboBox();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.panel1.SuspendLayout();
            this.panelFont.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.chklist_ballot);
            this.groupBox1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBox1.Font = new System.Drawing.Font("Microsoft Sans Serif", 14F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.groupBox1.ForeColor = System.Drawing.SystemColors.HighlightText;
            this.groupBox1.Location = new System.Drawing.Point(0, 0);
            this.groupBox1.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Padding = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.groupBox1.Size = new System.Drawing.Size(1267, 815);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Active Ballots";
            // 
            // chklist_ballot
            // 
            this.chklist_ballot.BackColor = System.Drawing.Color.White;
            this.chklist_ballot.CheckOnClick = true;
            this.chklist_ballot.Dock = System.Windows.Forms.DockStyle.Fill;
            this.chklist_ballot.FormattingEnabled = true;
            this.chklist_ballot.HorizontalScrollbar = true;
            this.chklist_ballot.Location = new System.Drawing.Point(3, 36);
            this.chklist_ballot.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.chklist_ballot.Name = "chklist_ballot";
            this.chklist_ballot.Size = new System.Drawing.Size(1261, 775);
            this.chklist_ballot.TabIndex = 0;
            this.chklist_ballot.UseCompatibleTextRendering = true;
            this.chklist_ballot.ItemCheck += new System.Windows.Forms.ItemCheckEventHandler(this.chklist_ballot_ItemCheck);
            this.chklist_ballot.SelectedIndexChanged += new System.EventHandler(this.chklist_ballot_SelectedIndexChanged);
            this.chklist_ballot.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.chklist_ballot_KeyPress);
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
            this.splitContainer1.Panel1.Controls.Add(this.groupBox1);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.BackColor = System.Drawing.Color.White;
            this.splitContainer1.Panel2.Controls.Add(this.panel1);
            this.splitContainer1.Panel2.Controls.Add(this.panelFont);
            this.splitContainer1.Panel2.Controls.Add(this.btnBallotMode);
            this.splitContainer1.Panel2.Controls.Add(this.btnCancel);
            this.splitContainer1.Panel2.Controls.Add(this.btnSave);
            this.splitContainer1.Size = new System.Drawing.Size(1267, 938);
            this.splitContainer1.SplitterDistance = 815;
            this.splitContainer1.SplitterWidth = 5;
            this.splitContainer1.TabIndex = 1;
            // 
            // panel1
            // 
            this.panel1.BackColor = System.Drawing.Color.ForestGreen;
            this.panel1.Controls.Add(this.chk_showRecordtool);
            this.panel1.Location = new System.Drawing.Point(2, 78);
            this.panel1.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(1293, 40);
            this.panel1.TabIndex = 22;
            // 
            // chk_showRecordtool
            // 
            this.chk_showRecordtool.AutoSize = true;
            this.chk_showRecordtool.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.chk_showRecordtool.ForeColor = System.Drawing.Color.White;
            this.chk_showRecordtool.Location = new System.Drawing.Point(6, 8);
            this.chk_showRecordtool.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.chk_showRecordtool.Name = "chk_showRecordtool";
            this.chk_showRecordtool.Size = new System.Drawing.Size(277, 29);
            this.chk_showRecordtool.TabIndex = 21;
            this.chk_showRecordtool.Text = "Show RECORD print tool";
            this.chk_showRecordtool.UseVisualStyleBackColor = true;
            this.chk_showRecordtool.Visible = false;
            this.chk_showRecordtool.CheckedChanged += new System.EventHandler(this.chk_showRecordtool_CheckedChanged);
            // 
            // panelFont
            // 
            this.panelFont.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panelFont.Controls.Add(this.label2);
            this.panelFont.Controls.Add(this.comboCopy);
            this.panelFont.Controls.Add(this.label1);
            this.panelFont.Controls.Add(this.comboFont);
            this.panelFont.Location = new System.Drawing.Point(584, 8);
            this.panelFont.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.panelFont.Name = "panelFont";
            this.panelFont.Size = new System.Drawing.Size(671, 62);
            this.panelFont.TabIndex = 20;
            this.panelFont.Visible = false;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(370, 22);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(111, 20);
            this.label1.TabIndex = 1;
            this.label1.Text = "Report Font:";
            // 
            // comboFont
            // 
            this.comboFont.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboFont.Font = new System.Drawing.Font("Microsoft Sans Serif", 14F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.comboFont.FormattingEnabled = true;
            this.comboFont.Items.AddRange(new object[] {
            "Default",
            "Large"});
            this.comboFont.Location = new System.Drawing.Point(498, 9);
            this.comboFont.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.comboFont.Name = "comboFont";
            this.comboFont.Size = new System.Drawing.Size(160, 40);
            this.comboFont.TabIndex = 0;
            // 
            // btnBallotMode
            // 
            this.btnBallotMode.BackColor = System.Drawing.Color.Lime;
            this.btnBallotMode.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("btnBallotMode.BackgroundImage")));
            this.btnBallotMode.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.btnBallotMode.FlatAppearance.BorderSize = 0;
            this.btnBallotMode.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnBallotMode.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.btnBallotMode.ForeColor = System.Drawing.SystemColors.ButtonHighlight;
            this.btnBallotMode.Location = new System.Drawing.Point(166, 6);
            this.btnBallotMode.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.btnBallotMode.Name = "btnBallotMode";
            this.btnBallotMode.Size = new System.Drawing.Size(224, 66);
            this.btnBallotMode.TabIndex = 19;
            this.btnBallotMode.Text = "BALLOT MODE";
            this.btnBallotMode.UseVisualStyleBackColor = false;
            this.btnBallotMode.Click += new System.EventHandler(this.btnBallotMode_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.BackColor = System.Drawing.Color.Transparent;
            this.btnCancel.BackgroundImage = global::VotRite.Properties.Resources.button_red1;
            this.btnCancel.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.btnCancel.FlatAppearance.BorderSize = 0;
            this.btnCancel.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnCancel.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.btnCancel.ForeColor = System.Drawing.SystemColors.ButtonHighlight;
            this.btnCancel.Location = new System.Drawing.Point(431, 6);
            this.btnCancel.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(130, 66);
            this.btnCancel.TabIndex = 18;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = false;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // btnSave
            // 
            this.btnSave.BackColor = System.Drawing.Color.Lime;
            this.btnSave.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("btnSave.BackgroundImage")));
            this.btnSave.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.btnSave.FlatAppearance.BorderSize = 0;
            this.btnSave.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnSave.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.btnSave.ForeColor = System.Drawing.SystemColors.ButtonHighlight;
            this.btnSave.Location = new System.Drawing.Point(4, 6);
            this.btnSave.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(130, 66);
            this.btnSave.TabIndex = 17;
            this.btnSave.Text = "OK";
            this.btnSave.UseVisualStyleBackColor = false;
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(14, 23);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(114, 20);
            this.label2.TabIndex = 3;
            this.label2.Text = "Report Copy:";
            // 
            // comboCopy
            // 
            this.comboCopy.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboCopy.Font = new System.Drawing.Font("Microsoft Sans Serif", 14F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.comboCopy.FormattingEnabled = true;
            this.comboCopy.Items.AddRange(new object[] {
            "1",
            "2",
            "3",
            "4",
            "5",
            "6",
            "7",
            "8",
            "9",
            "10"});
            this.comboCopy.Location = new System.Drawing.Point(142, 10);
            this.comboCopy.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.comboCopy.Name = "comboCopy";
            this.comboCopy.Size = new System.Drawing.Size(160, 40);
            this.comboCopy.TabIndex = 2;
            // 
            // BallotsListDisplay
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.ForestGreen;
            this.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.ClientSize = new System.Drawing.Size(1267, 938);
            this.ControlBox = false;
            this.Controls.Add(this.splitContainer1);
            this.DoubleBuffered = true;
            this.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.MinimumSize = new System.Drawing.Size(1284, 954);
            this.Name = "BallotsListDisplay";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Load += new System.EventHandler(this.BallotsListDisplay_Load);
            this.groupBox1.ResumeLayout(false);
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.panelFont.ResumeLayout(false);
            this.panelFont.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Button btnSave;
        private System.Windows.Forms.CheckedListBox chklist_ballot;
        private System.Windows.Forms.Button btnBallotMode;
        private System.Windows.Forms.Panel panelFont;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ComboBox comboFont;
        private System.Windows.Forms.CheckBox chk_showRecordtool;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.ComboBox comboCopy;
    }
}