
namespace VotRite.JimForms.UserControls
{
    partial class vrSelectionPreview
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.picsign = new System.Windows.Forms.PictureBox();
            this.lblcandidate = new System.Windows.Forms.Label();
            this.tableLayoutPanel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.picsign)).BeginInit();
            this.SuspendLayout();
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 2;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 10F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 90F));
            this.tableLayoutPanel1.Controls.Add(this.picsign, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.lblcandidate, 1, 0);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 1;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 79F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(773, 91);
            this.tableLayoutPanel1.TabIndex = 0;
            // 
            // picsign
            // 
            this.picsign.BackColor = System.Drawing.SystemColors.ActiveBorder;
            this.picsign.Dock = System.Windows.Forms.DockStyle.Fill;
            this.picsign.Location = new System.Drawing.Point(3, 3);
            this.picsign.Name = "picsign";
            this.picsign.Size = new System.Drawing.Size(71, 85);
            this.picsign.TabIndex = 0;
            this.picsign.TabStop = false;
            // 
            // lblcandidate
            // 
            this.lblcandidate.BackColor = System.Drawing.Color.Transparent;
            this.lblcandidate.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblcandidate.Location = new System.Drawing.Point(80, 0);
            this.lblcandidate.Name = "lblcandidate";
            this.lblcandidate.Size = new System.Drawing.Size(690, 91);
            this.lblcandidate.TabIndex = 1;
            this.lblcandidate.Text = "label1";
            this.lblcandidate.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.lblcandidate.Click += new System.EventHandler(this.lblcandidate_Click);
            // 
            // vrSelectionPreview
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSize = true;
            this.BackColor = System.Drawing.Color.Transparent;
            this.Controls.Add(this.tableLayoutPanel1);
            this.Name = "vrSelectionPreview";
            this.Size = new System.Drawing.Size(773, 91);
            this.Load += new System.EventHandler(this.vrSelectionPreview_Load);
            this.tableLayoutPanel1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.picsign)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        public System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        public System.Windows.Forms.PictureBox picsign;
        public System.Windows.Forms.Label lblcandidate;
    }
}
