
namespace VotRite.JimForms
{
    partial class RecordReportPrint
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
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.radioeEnglish = new System.Windows.Forms.RadioButton();
            this.radioLanguage = new System.Windows.Forms.RadioButton();
            this.btnRecord_print = new System.Windows.Forms.Button();
            this.gvRecord = new System.Windows.Forms.DataGridView();
            this.comboRecords_dates = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.label2 = new System.Windows.Forms.Label();
            this.btn_excl_browse = new System.Windows.Forms.Button();
            this.tabControl1.SuspendLayout();
            this.tabPage1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.gvRecord)).BeginInit();
            this.tabPage2.SuspendLayout();
            this.SuspendLayout();
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.tabPage1);
            this.tabControl1.Controls.Add(this.tabPage2);
            this.tabControl1.Location = new System.Drawing.Point(14, 6);
            this.tabControl1.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(1112, 959);
            this.tabControl1.TabIndex = 0;
            // 
            // tabPage1
            // 
            this.tabPage1.BackColor = System.Drawing.Color.White;
            this.tabPage1.Controls.Add(this.radioeEnglish);
            this.tabPage1.Controls.Add(this.radioLanguage);
            this.tabPage1.Controls.Add(this.btnRecord_print);
            this.tabPage1.Controls.Add(this.gvRecord);
            this.tabPage1.Controls.Add(this.comboRecords_dates);
            this.tabPage1.Controls.Add(this.label1);
            this.tabPage1.Location = new System.Drawing.Point(4, 29);
            this.tabPage1.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.tabPage1.Size = new System.Drawing.Size(1104, 926);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "PRINT RECORD";
            // 
            // radioeEnglish
            // 
            this.radioeEnglish.AutoSize = true;
            this.radioeEnglish.Font = new System.Drawing.Font("Microsoft Sans Serif", 7.8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.radioeEnglish.Location = new System.Drawing.Point(392, 11);
            this.radioeEnglish.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.radioeEnglish.Name = "radioeEnglish";
            this.radioeEnglish.Size = new System.Drawing.Size(229, 24);
            this.radioeEnglish.TabIndex = 5;
            this.radioeEnglish.TabStop = true;
            this.radioeEnglish.Text = "RECORDS IN ENGLISH";
            this.radioeEnglish.UseVisualStyleBackColor = true;
            this.radioeEnglish.CheckedChanged += new System.EventHandler(this.radioeEnglish_CheckedChanged);
            // 
            // radioLanguage
            // 
            this.radioLanguage.AutoSize = true;
            this.radioLanguage.Checked = true;
            this.radioLanguage.Font = new System.Drawing.Font("Microsoft Sans Serif", 7.8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.radioLanguage.Location = new System.Drawing.Point(22, 10);
            this.radioLanguage.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.radioLanguage.Name = "radioLanguage";
            this.radioLanguage.Size = new System.Drawing.Size(321, 24);
            this.radioLanguage.TabIndex = 4;
            this.radioLanguage.TabStop = true;
            this.radioLanguage.Text = "RECORDS IN BALLOT LANGUAGE";
            this.radioLanguage.UseVisualStyleBackColor = true;
            this.radioLanguage.CheckedChanged += new System.EventHandler(this.radioLanguage_CheckedChanged);
            // 
            // btnRecord_print
            // 
            this.btnRecord_print.BackgroundImage = global::VotRite.Properties.Resources.button_green;
            this.btnRecord_print.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.btnRecord_print.FlatAppearance.BorderSize = 0;
            this.btnRecord_print.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnRecord_print.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnRecord_print.ForeColor = System.Drawing.Color.White;
            this.btnRecord_print.Location = new System.Drawing.Point(760, 788);
            this.btnRecord_print.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.btnRecord_print.Name = "btnRecord_print";
            this.btnRecord_print.Size = new System.Drawing.Size(296, 74);
            this.btnRecord_print.TabIndex = 3;
            this.btnRecord_print.Text = "PRINT RECORD(s)";
            this.btnRecord_print.UseVisualStyleBackColor = true;
            this.btnRecord_print.Click += new System.EventHandler(this.btnRecord_print_Click);
            // 
            // gvRecord
            // 
            this.gvRecord.AllowUserToAddRows = false;
            this.gvRecord.AllowUserToDeleteRows = false;
            this.gvRecord.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.AllCells;
            this.gvRecord.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.gvRecord.EditMode = System.Windows.Forms.DataGridViewEditMode.EditProgrammatically;
            this.gvRecord.Location = new System.Drawing.Point(25, 129);
            this.gvRecord.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.gvRecord.Name = "gvRecord";
            this.gvRecord.RowHeadersVisible = false;
            this.gvRecord.RowHeadersWidth = 51;
            this.gvRecord.RowTemplate.Height = 24;
            this.gvRecord.Size = new System.Drawing.Size(1032, 619);
            this.gvRecord.TabIndex = 2;
            // 
            // comboRecords_dates
            // 
            this.comboRecords_dates.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.comboRecords_dates.FormattingEnabled = true;
            this.comboRecords_dates.Location = new System.Drawing.Point(231, 55);
            this.comboRecords_dates.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.comboRecords_dates.Name = "comboRecords_dates";
            this.comboRecords_dates.Size = new System.Drawing.Size(310, 33);
            this.comboRecords_dates.TabIndex = 1;
            this.comboRecords_dates.SelectedIndexChanged += new System.EventHandler(this.comboRecords_dates_SelectedIndexChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(20, 56);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(143, 25);
            this.label1.TabIndex = 0;
            this.label1.Text = "Voting Dates:";
            // 
            // tabPage2
            // 
            this.tabPage2.Controls.Add(this.btn_excl_browse);
            this.tabPage2.Controls.Add(this.label2);
            this.tabPage2.Location = new System.Drawing.Point(4, 29);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Size = new System.Drawing.Size(1104, 926);
            this.tabPage2.TabIndex = 1;
            this.tabPage2.Text = "EXCEL REPORT PRINT";
            this.tabPage2.UseVisualStyleBackColor = true;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(47, 40);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(209, 20);
            this.label2.TabIndex = 0;
            this.label2.Text = "CHOOSE EXCEL REPORT:";
            // 
            // btn_excl_browse
            // 
            this.btn_excl_browse.Location = new System.Drawing.Point(301, 34);
            this.btn_excl_browse.Name = "btn_excl_browse";
            this.btn_excl_browse.Size = new System.Drawing.Size(150, 34);
            this.btn_excl_browse.TabIndex = 1;
            this.btn_excl_browse.Text = "BROWSE...";
            this.btn_excl_browse.UseVisualStyleBackColor = true;
            this.btn_excl_browse.Click += new System.EventHandler(this.btn_excl_browse_Click);
            // 
            // RecordReportPrint
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.ForestGreen;
            this.ClientSize = new System.Drawing.Size(1138, 974);
            this.Controls.Add(this.tabControl1);
            this.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "RecordReportPrint";
            this.Text = "RecordReportPrint";
            this.Load += new System.EventHandler(this.RecordReportPrint_Load);
            this.tabControl1.ResumeLayout(false);
            this.tabPage1.ResumeLayout(false);
            this.tabPage1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.gvRecord)).EndInit();
            this.tabPage2.ResumeLayout(false);
            this.tabPage2.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.ComboBox comboRecords_dates;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.DataGridView gvRecord;
        private System.Windows.Forms.Button btnRecord_print;
        private System.Windows.Forms.RadioButton radioeEnglish;
        private System.Windows.Forms.RadioButton radioLanguage;
        private System.Windows.Forms.TabPage tabPage2;
        private System.Windows.Forms.Button btn_excl_browse;
        private System.Windows.Forms.Label label2;
    }
}