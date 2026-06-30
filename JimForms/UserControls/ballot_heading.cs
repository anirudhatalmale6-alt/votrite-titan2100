using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace VotRite.JimForms
{
    public partial class ballot_heading : UserControl
    {
        public ballot_heading(string name)
        {
            InitializeComponent();
            lblBallot.Text = name;
        }

        private void ballot_heading_Load(object sender, EventArgs e)
        {
            this.Width = this.Parent.Width;
        }


        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.lblBallot = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // lblBallot
            // 
            this.lblBallot.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.lblBallot.Font = new System.Drawing.Font("Microsoft Sans Serif", 14F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblBallot.Location = new System.Drawing.Point(0, 7);
            this.lblBallot.Name = "lblBallot";
            this.lblBallot.Size = new System.Drawing.Size(796, 76);
            this.lblBallot.TabIndex = 1;
            this.lblBallot.Text = "Ballot: ";
            // 
            // label1
            // 
            this.label1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.label1.Location = new System.Drawing.Point(-2, 5);
            this.label1.MaximumSize = new System.Drawing.Size(0, 2);
            this.label1.MinimumSize = new System.Drawing.Size(800, 2);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(800, 2);
            this.label1.TabIndex = 2;
            this.label1.Text = "label1";
            // 
            // ballot_heading
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.label1);
            this.Controls.Add(this.lblBallot);
            this.Name = "ballot_heading";
            this.Size = new System.Drawing.Size(796, 83);
            this.Load += new System.EventHandler(this.ballot_heading_Load);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Label lblBallot;
        private System.Windows.Forms.Label label1;
    }
}