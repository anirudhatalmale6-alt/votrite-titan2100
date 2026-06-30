
namespace VotRite.JimForms
{
    partial class exitoptions
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
            this.btnExitballot = new System.Windows.Forms.Button();
            this.btnExitvotrite = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // btnExitballot
            // 
            this.btnExitballot.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.btnExitballot.FlatAppearance.BorderSize = 0;
            this.btnExitballot.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnExitballot.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnExitballot.Location = new System.Drawing.Point(27, 12);
            this.btnExitballot.Name = "btnExitballot";
            this.btnExitballot.Size = new System.Drawing.Size(230, 42);
            this.btnExitballot.TabIndex = 0;
            this.btnExitballot.Text = "CANCEL BALLOT";
            this.btnExitballot.UseVisualStyleBackColor = true;
            this.btnExitballot.Click += new System.EventHandler(this.btnExitballot_Click);
            // 
            // btnExitvotrite
            // 
            this.btnExitvotrite.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.btnExitvotrite.FlatAppearance.BorderSize = 0;
            this.btnExitvotrite.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnExitvotrite.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnExitvotrite.Location = new System.Drawing.Point(290, 12);
            this.btnExitvotrite.Name = "btnExitvotrite";
            this.btnExitvotrite.Size = new System.Drawing.Size(230, 42);
            this.btnExitvotrite.TabIndex = 1;
            this.btnExitvotrite.Text = "EXIT VOTRITE";
            this.btnExitvotrite.UseVisualStyleBackColor = true;
            this.btnExitvotrite.Click += new System.EventHandler(this.btnExitvotrite_Click);
            // 
            // exitoptions
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(548, 66);
            this.Controls.Add(this.btnExitvotrite);
            this.Controls.Add(this.btnExitballot);
            this.MaximizeBox = false;
            this.MaximumSize = new System.Drawing.Size(566, 113);
            this.MinimizeBox = false;
            this.MinimumSize = new System.Drawing.Size(566, 113);
            this.Name = "exitoptions";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Exit Options";
            this.Load += new System.EventHandler(this.exitoptions_Load);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button btnExitballot;
        private System.Windows.Forms.Button btnExitvotrite;
    }
}