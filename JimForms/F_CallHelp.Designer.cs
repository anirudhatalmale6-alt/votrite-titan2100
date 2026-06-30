namespace VotRite.JimForms
{
    partial class F_CallHelp
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
            this.rTxt = new System.Windows.Forms.RichTextBox();
            this.SuspendLayout();
            // 
            // rTxt
            // 
            this.rTxt.Dock = System.Windows.Forms.DockStyle.Fill;
            this.rTxt.Location = new System.Drawing.Point(0, 0);
            this.rTxt.Name = "rTxt";
            this.rTxt.Size = new System.Drawing.Size(704, 354);
            this.rTxt.TabIndex = 0;
            this.rTxt.Text = "";
            // 
            // F_CallHelp
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(704, 354);
            this.Controls.Add(this.rTxt);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "F_CallHelp";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "F_CallHelp";
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            this.Load += new System.EventHandler(this.F_CallHelp_Load);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.RichTextBox rTxt;
    }
}