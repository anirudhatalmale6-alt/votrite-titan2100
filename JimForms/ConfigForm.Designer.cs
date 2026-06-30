namespace VotRite.JimForms
{
    partial class ConfigForm
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
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle3 = new System.Windows.Forms.DataGridViewCellStyle();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.lblHead = new System.Windows.Forms.Label();
            this.splitContainer2 = new System.Windows.Forms.SplitContainer();
            this.chkRecordHeader = new System.Windows.Forms.CheckBox();
            this.lbl_Log = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.txtMainLocation = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.linkDelete = new System.Windows.Forms.LinkLabel();
            this.linkCancel = new System.Windows.Forms.LinkLabel();
            this.btnAdd = new System.Windows.Forms.Button();
            this.txtBackupDrive = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.txtLocation = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.txtMachineNo = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.gv = new System.Windows.Forms.DataGridView();
            this.btnEdit = new System.Windows.Forms.DataGridViewButtonColumn();
            this.btnBallot = new System.Windows.Forms.DataGridViewButtonColumn();
            this.btnHelp = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer2)).BeginInit();
            this.splitContainer2.Panel1.SuspendLayout();
            this.splitContainer2.Panel2.SuspendLayout();
            this.splitContainer2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.gv)).BeginInit();
            this.SuspendLayout();
            // 
            // splitContainer1
            // 
            this.splitContainer1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.FixedPanel = System.Windows.Forms.FixedPanel.Panel1;
            this.splitContainer1.IsSplitterFixed = true;
            this.splitContainer1.Location = new System.Drawing.Point(0, 0);
            this.splitContainer1.Name = "splitContainer1";
            this.splitContainer1.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.BackColor = System.Drawing.Color.ForestGreen;
            this.splitContainer1.Panel1.Controls.Add(this.lblHead);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.splitContainer2);
            this.splitContainer1.Size = new System.Drawing.Size(1898, 621);
            this.splitContainer1.SplitterDistance = 55;
            this.splitContainer1.TabIndex = 0;
            // 
            // lblHead
            // 
            this.lblHead.AutoSize = true;
            this.lblHead.Font = new System.Drawing.Font("Microsoft Sans Serif", 15F, ((System.Drawing.FontStyle)((System.Drawing.FontStyle.Bold | System.Drawing.FontStyle.Underline))), System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblHead.ForeColor = System.Drawing.Color.White;
            this.lblHead.Location = new System.Drawing.Point(372, 10);
            this.lblHead.Name = "lblHead";
            this.lblHead.Size = new System.Drawing.Size(370, 29);
            this.lblHead.TabIndex = 0;
            this.lblHead.Text = "MACHINE CONFIGURATION ";
            // 
            // splitContainer2
            // 
            this.splitContainer2.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.splitContainer2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer2.FixedPanel = System.Windows.Forms.FixedPanel.Panel1;
            this.splitContainer2.IsSplitterFixed = true;
            this.splitContainer2.Location = new System.Drawing.Point(0, 0);
            this.splitContainer2.Name = "splitContainer2";
            this.splitContainer2.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainer2.Panel1
            // 
            this.splitContainer2.Panel1.BackColor = System.Drawing.Color.White;
            this.splitContainer2.Panel1.Controls.Add(this.btnHelp);
            this.splitContainer2.Panel1.Controls.Add(this.chkRecordHeader);
            this.splitContainer2.Panel1.Controls.Add(this.lbl_Log);
            this.splitContainer2.Panel1.Controls.Add(this.label5);
            this.splitContainer2.Panel1.Controls.Add(this.txtMainLocation);
            this.splitContainer2.Panel1.Controls.Add(this.label1);
            this.splitContainer2.Panel1.Controls.Add(this.linkDelete);
            this.splitContainer2.Panel1.Controls.Add(this.linkCancel);
            this.splitContainer2.Panel1.Controls.Add(this.btnAdd);
            this.splitContainer2.Panel1.Controls.Add(this.txtBackupDrive);
            this.splitContainer2.Panel1.Controls.Add(this.label4);
            this.splitContainer2.Panel1.Controls.Add(this.txtLocation);
            this.splitContainer2.Panel1.Controls.Add(this.label3);
            this.splitContainer2.Panel1.Controls.Add(this.txtMachineNo);
            this.splitContainer2.Panel1.Controls.Add(this.label2);
            // 
            // splitContainer2.Panel2
            // 
            this.splitContainer2.Panel2.Controls.Add(this.gv);
            this.splitContainer2.Size = new System.Drawing.Size(1898, 562);
            this.splitContainer2.SplitterDistance = 115;
            this.splitContainer2.TabIndex = 0;
            // 
            // chkRecordHeader
            // 
            this.chkRecordHeader.AutoSize = true;
            this.chkRecordHeader.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.chkRecordHeader.Location = new System.Drawing.Point(382, 76);
            this.chkRecordHeader.Name = "chkRecordHeader";
            this.chkRecordHeader.Size = new System.Drawing.Size(323, 24);
            this.chkRecordHeader.TabIndex = 16;
            this.chkRecordHeader.Text = "Add Header in each record images";
            this.chkRecordHeader.UseVisualStyleBackColor = true;
            this.chkRecordHeader.CheckedChanged += new System.EventHandler(this.chkRecordHeader_CheckedChanged);
            // 
            // lbl_Log
            // 
            this.lbl_Log.AutoSize = true;
            this.lbl_Log.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbl_Log.ForeColor = System.Drawing.Color.Green;
            this.lbl_Log.Location = new System.Drawing.Point(18, 76);
            this.lbl_Log.Name = "lbl_Log";
            this.lbl_Log.Size = new System.Drawing.Size(106, 25);
            this.lbl_Log.TabIndex = 15;
            this.lbl_Log.Text = "LOG: 150";
            this.lbl_Log.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label5.Location = new System.Drawing.Point(623, 16);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(44, 18);
            this.label5.TabIndex = 11;
            this.label5.Text = "Main";
            // 
            // txtMainLocation
            // 
            this.txtMainLocation.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtMainLocation.Location = new System.Drawing.Point(714, 7);
            this.txtMainLocation.Multiline = true;
            this.txtMainLocation.Name = "txtMainLocation";
            this.txtMainLocation.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.txtMainLocation.Size = new System.Drawing.Size(361, 52);
            this.txtMainLocation.TabIndex = 4;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(623, 34);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(78, 18);
            this.label1.TabIndex = 9;
            this.label1.Text = "Location:";
            // 
            // linkDelete
            // 
            this.linkDelete.AutoSize = true;
            this.linkDelete.Font = new System.Drawing.Font("Microsoft Sans Serif", 7.8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.linkDelete.LinkColor = System.Drawing.Color.Red;
            this.linkDelete.Location = new System.Drawing.Point(1080, 69);
            this.linkDelete.Name = "linkDelete";
            this.linkDelete.Size = new System.Drawing.Size(146, 17);
            this.linkDelete.TabIndex = 8;
            this.linkDelete.TabStop = true;
            this.linkDelete.Text = "REMOVE MACHINE";
            this.linkDelete.Visible = false;
            this.linkDelete.VisitedLinkColor = System.Drawing.Color.Red;
            this.linkDelete.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.linkDelete_LinkClicked);
            // 
            // linkCancel
            // 
            this.linkCancel.AutoSize = true;
            this.linkCancel.Font = new System.Drawing.Font("Microsoft Sans Serif", 7.8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.linkCancel.Location = new System.Drawing.Point(1080, 92);
            this.linkCancel.Name = "linkCancel";
            this.linkCancel.Size = new System.Drawing.Size(68, 17);
            this.linkCancel.TabIndex = 7;
            this.linkCancel.TabStop = true;
            this.linkCancel.Text = "CANCEL";
            this.linkCancel.Visible = false;
            this.linkCancel.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.linkCancel_LinkClicked);
            // 
            // btnAdd
            // 
            this.btnAdd.BackgroundImage = global::VotRite.Properties.Resources.button_green;
            this.btnAdd.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.btnAdd.FlatAppearance.BorderSize = 0;
            this.btnAdd.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnAdd.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnAdd.ForeColor = System.Drawing.Color.White;
            this.btnAdd.Location = new System.Drawing.Point(1231, 65);
            this.btnAdd.Name = "btnAdd";
            this.btnAdd.Size = new System.Drawing.Size(188, 46);
            this.btnAdd.TabIndex = 6;
            this.btnAdd.Text = "ADD";
            this.btnAdd.UseVisualStyleBackColor = true;
            this.btnAdd.Click += new System.EventHandler(this.btnAdd_Click);
            // 
            // txtBackupDrive
            // 
            this.txtBackupDrive.Font = new System.Drawing.Font("Microsoft Sans Serif", 15F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtBackupDrive.Location = new System.Drawing.Point(1231, 7);
            this.txtBackupDrive.Multiline = true;
            this.txtBackupDrive.Name = "txtBackupDrive";
            this.txtBackupDrive.Size = new System.Drawing.Size(188, 50);
            this.txtBackupDrive.TabIndex = 5;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label4.Location = new System.Drawing.Point(1106, 22);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(105, 18);
            this.label4.TabIndex = 14;
            this.label4.Text = "Flash Drive*:";
            // 
            // txtLocation
            // 
            this.txtLocation.Font = new System.Drawing.Font("Microsoft Sans Serif", 15F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtLocation.Location = new System.Drawing.Point(382, 7);
            this.txtLocation.Multiline = true;
            this.txtLocation.Name = "txtLocation";
            this.txtLocation.Size = new System.Drawing.Size(223, 52);
            this.txtLocation.TabIndex = 3;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.Location = new System.Drawing.Point(291, 22);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(85, 18);
            this.label3.TabIndex = 2;
            this.label3.Text = "Location*:";
            // 
            // txtMachineNo
            // 
            this.txtMachineNo.Font = new System.Drawing.Font("Microsoft Sans Serif", 15F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtMachineNo.Location = new System.Drawing.Point(131, 16);
            this.txtMachineNo.Name = "txtMachineNo";
            this.txtMachineNo.Size = new System.Drawing.Size(135, 36);
            this.txtMachineNo.TabIndex = 1;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(12, 22);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(110, 18);
            this.label2.TabIndex = 0;
            this.label2.Text = "Machine No*:";
            // 
            // gv
            // 
            this.gv.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle1.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle1.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle1.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle1.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle1.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle1.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.gv.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle1;
            this.gv.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.gv.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.btnEdit,
            this.btnBallot});
            this.gv.Dock = System.Windows.Forms.DockStyle.Fill;
            this.gv.EditMode = System.Windows.Forms.DataGridViewEditMode.EditProgrammatically;
            this.gv.Location = new System.Drawing.Point(0, 0);
            this.gv.Name = "gv";
            this.gv.RowHeadersVisible = false;
            this.gv.RowHeadersWidth = 51;
            this.gv.RowTemplate.Height = 44;
            this.gv.Size = new System.Drawing.Size(1896, 441);
            this.gv.TabIndex = 0;
            this.gv.CellContentClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.gv_CellContentClick);
            // 
            // btnEdit
            // 
            this.btnEdit.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.ColumnHeader;
            dataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle2.Font = new System.Drawing.Font("Microsoft Sans Serif", 7.8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnEdit.DefaultCellStyle = dataGridViewCellStyle2;
            this.btnEdit.HeaderText = "";
            this.btnEdit.MinimumWidth = 170;
            this.btnEdit.Name = "btnEdit";
            this.btnEdit.Text = "Edit Machine Config";
            this.btnEdit.UseColumnTextForButtonValue = true;
            this.btnEdit.Width = 170;
            // 
            // btnBallot
            // 
            this.btnBallot.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.ColumnHeader;
            dataGridViewCellStyle3.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle3.BackColor = System.Drawing.Color.Gold;
            this.btnBallot.DefaultCellStyle = dataGridViewCellStyle3;
            this.btnBallot.HeaderText = "";
            this.btnBallot.MinimumWidth = 170;
            this.btnBallot.Name = "btnBallot";
            this.btnBallot.Text = "View Ballots";
            this.btnBallot.UseColumnTextForButtonValue = true;
            this.btnBallot.Width = 170;
            // 
            // btnHelp
            // 
            this.btnHelp.BackgroundImage = global::VotRite.Properties.Resources.button_green;
            this.btnHelp.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.btnHelp.FlatAppearance.BorderSize = 0;
            this.btnHelp.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnHelp.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnHelp.ForeColor = System.Drawing.Color.White;
            this.btnHelp.Location = new System.Drawing.Point(714, 72);
            this.btnHelp.Name = "btnHelp";
            this.btnHelp.Size = new System.Drawing.Size(340, 34);
            this.btnHelp.TabIndex = 17;
            this.btnHelp.Text = "Help Settings";
            this.btnHelp.UseVisualStyleBackColor = true;
            this.btnHelp.Visible = false;
            this.btnHelp.Click += new System.EventHandler(this.btnHelp_Click);
            // 
            // ConfigForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1898, 621);
            this.Controls.Add(this.splitContainer1);
            this.MinimumSize = new System.Drawing.Size(1916, 668);
            this.Name = "ConfigForm";
            this.ShowIcon = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Configuration Form";
            this.Load += new System.EventHandler(this.ConfigForm_Load);
            this.Resize += new System.EventHandler(this.ConfigForm_Resize);
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel1.PerformLayout();
            this.splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.splitContainer2.Panel1.ResumeLayout(false);
            this.splitContainer2.Panel1.PerformLayout();
            this.splitContainer2.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer2)).EndInit();
            this.splitContainer2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.gv)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.Label lblHead;
        private System.Windows.Forms.SplitContainer splitContainer2;
        private System.Windows.Forms.Button btnAdd;
        private System.Windows.Forms.TextBox txtBackupDrive;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox txtLocation;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox txtMachineNo;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.DataGridView gv;
        private System.Windows.Forms.LinkLabel linkCancel;
        private System.Windows.Forms.LinkLabel linkDelete;
        private System.Windows.Forms.DataGridViewButtonColumn btnEdit;
        private System.Windows.Forms.DataGridViewButtonColumn btnBallot;
        private System.Windows.Forms.TextBox txtMainLocation;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label lbl_Log;
        private System.Windows.Forms.CheckBox chkRecordHeader;
        private System.Windows.Forms.Button btnHelp;
    }
}