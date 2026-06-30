namespace VotRite.JimForms
{
    partial class CloseOrReset
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
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.label1 = new System.Windows.Forms.Label();
            this.btnExitApp = new System.Windows.Forms.Button();
            this.btnResetVoting = new System.Windows.Forms.Button();
            this.lblMsg = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
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
            this.splitContainer1.Panel1.BackColor = System.Drawing.SystemColors.ActiveCaption;
            this.splitContainer1.Panel1.Controls.Add(this.label1);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.lblMsg);
            this.splitContainer1.Panel2.Controls.Add(this.btnExitApp);
            this.splitContainer1.Panel2.Controls.Add(this.btnResetVoting);
            this.splitContainer1.Size = new System.Drawing.Size(542, 262);
            this.splitContainer1.SplitterDistance = 70;
            this.splitContainer1.TabIndex = 0;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 16F, ((System.Drawing.FontStyle)((System.Drawing.FontStyle.Bold | System.Drawing.FontStyle.Underline))), System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(38, 21);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(464, 31);
            this.label1.TabIndex = 0;
            this.label1.Text = "Choose Reset/Close Option Below";
            // 
            // btnExitApp
            // 
            this.btnExitApp.BackColor = System.Drawing.Color.LightBlue;
            this.btnExitApp.Font = new System.Drawing.Font("Arial Black", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnExitApp.Location = new System.Drawing.Point(276, 54);
            this.btnExitApp.Name = "btnExitApp";
            this.btnExitApp.Size = new System.Drawing.Size(266, 79);
            this.btnExitApp.TabIndex = 1;
            this.btnExitApp.Text = "EXIT VOTING APP";
            this.btnExitApp.UseVisualStyleBackColor = false;
            this.btnExitApp.Click += new System.EventHandler(this.btnExitApp_Click);
            // 
            // btnResetVoting
            // 
            this.btnResetVoting.BackColor = System.Drawing.Color.SteelBlue;
            this.btnResetVoting.Font = new System.Drawing.Font("Arial Black", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnResetVoting.ForeColor = System.Drawing.SystemColors.Control;
            this.btnResetVoting.Location = new System.Drawing.Point(3, 54);
            this.btnResetVoting.Name = "btnResetVoting";
            this.btnResetVoting.Size = new System.Drawing.Size(266, 79);
            this.btnResetVoting.TabIndex = 0;
            this.btnResetVoting.Text = "RESET ENTIRE VOTING";
            this.btnResetVoting.UseVisualStyleBackColor = false;
            this.btnResetVoting.Click += new System.EventHandler(this.btnResetVoting_Click);
            // 
            // lblMsg
            // 
            this.lblMsg.AutoSize = true;
            this.lblMsg.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblMsg.Location = new System.Drawing.Point(3, 152);
            this.lblMsg.Name = "lblMsg";
            this.lblMsg.Size = new System.Drawing.Size(0, 20);
            this.lblMsg.TabIndex = 2;
            // 
            // CloseOrReset
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(542, 262);
            this.ControlBox = false;
            this.Controls.Add(this.splitContainer1);
            this.MaximumSize = new System.Drawing.Size(560, 280);
            this.MinimumSize = new System.Drawing.Size(560, 280);
            this.Name = "CloseOrReset";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Load += new System.EventHandler(this.CloseOrReset_Load);
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel1.PerformLayout();
            this.splitContainer1.Panel2.ResumeLayout(false);
            this.splitContainer1.Panel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button btnExitApp;
        private System.Windows.Forms.Button btnResetVoting;
        private System.Windows.Forms.Label lblMsg;
    }
}