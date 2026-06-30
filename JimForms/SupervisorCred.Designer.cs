
namespace VotRite.JimForms
{
    partial class SupervisorCred
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
            this.panel1 = new System.Windows.Forms.Panel();
            this.lblMessage = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.txt_Cred = new System.Windows.Forms.TextBox();
            this.btn_Proceed = new System.Windows.Forms.Button();
            this.btn_Cancel = new System.Windows.Forms.Button();
            this.chkExternalKey = new System.Windows.Forms.CheckBox();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.BackColor = System.Drawing.Color.White;
            this.panel1.Controls.Add(this.lblMessage);
            this.panel1.Controls.Add(this.label1);
            this.panel1.Controls.Add(this.txt_Cred);
            this.panel1.Controls.Add(this.btn_Proceed);
            this.panel1.Controls.Add(this.btn_Cancel);
            this.panel1.Controls.Add(this.chkExternalKey);
            this.panel1.Location = new System.Drawing.Point(10, 8);
            this.panel1.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(737, 228);
            this.panel1.TabIndex = 15;
            // 
            // lblMessage
            // 
            this.lblMessage.AutoSize = true;
            this.lblMessage.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblMessage.ForeColor = System.Drawing.Color.Red;
            this.lblMessage.Location = new System.Drawing.Point(28, 186);
            this.lblMessage.Name = "lblMessage";
            this.lblMessage.Size = new System.Drawing.Size(0, 25);
            this.lblMessage.TabIndex = 7;
            // 
            // label1
            // 
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 15F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.ForeColor = System.Drawing.SystemColors.ActiveCaptionText;
            this.label1.Location = new System.Drawing.Point(12, 50);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(342, 68);
            this.label1.TabIndex = 0;
            this.label1.Text = "Supervisor Password:";
            // 
            // txt_Cred
            // 
            this.txt_Cred.Font = new System.Drawing.Font("Microsoft Sans Serif", 15F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txt_Cred.Location = new System.Drawing.Point(364, 48);
            this.txt_Cred.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.txt_Cred.MaxLength = 12;
            this.txt_Cred.Name = "txt_Cred";
            this.txt_Cred.PasswordChar = '*';
            this.txt_Cred.Size = new System.Drawing.Size(307, 41);
            this.txt_Cred.TabIndex = 1;
            this.txt_Cred.Enter += new System.EventHandler(this.txt_Cred_Enter);
            this.txt_Cred.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.txt_Cred_KeyPress);
            this.txt_Cred.Leave += new System.EventHandler(this.txt_Cred_Leave);
            // 
            // btn_Proceed
            // 
            this.btn_Proceed.BackColor = System.Drawing.SystemColors.ButtonFace;
            this.btn_Proceed.BackgroundImage = global::VotRite.Properties.Resources.button_green;
            this.btn_Proceed.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.btn_Proceed.FlatAppearance.BorderSize = 0;
            this.btn_Proceed.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btn_Proceed.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btn_Proceed.ForeColor = System.Drawing.SystemColors.ButtonHighlight;
            this.btn_Proceed.Location = new System.Drawing.Point(364, 131);
            this.btn_Proceed.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.btn_Proceed.Name = "btn_Proceed";
            this.btn_Proceed.Size = new System.Drawing.Size(136, 46);
            this.btn_Proceed.TabIndex = 2;
            this.btn_Proceed.Text = "PROCEED";
            this.btn_Proceed.UseVisualStyleBackColor = false;
            this.btn_Proceed.Click += new System.EventHandler(this.btn_Proceed_Click);
            // 
            // btn_Cancel
            // 
            this.btn_Cancel.BackColor = System.Drawing.SystemColors.Control;
            this.btn_Cancel.BackgroundImage = global::VotRite.Properties.Resources.button_red1;
            this.btn_Cancel.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.btn_Cancel.FlatAppearance.BorderSize = 0;
            this.btn_Cancel.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btn_Cancel.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btn_Cancel.ForeColor = System.Drawing.SystemColors.ButtonHighlight;
            this.btn_Cancel.Location = new System.Drawing.Point(557, 131);
            this.btn_Cancel.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.btn_Cancel.Name = "btn_Cancel";
            this.btn_Cancel.Size = new System.Drawing.Size(115, 46);
            this.btn_Cancel.TabIndex = 3;
            this.btn_Cancel.Text = "CANCEL";
            this.btn_Cancel.UseVisualStyleBackColor = false;
            this.btn_Cancel.Click += new System.EventHandler(this.btn_Cancel_Click);
            // 
            // chkExternalKey
            // 
            this.chkExternalKey.AutoSize = true;
            this.chkExternalKey.Font = new System.Drawing.Font("Microsoft Sans Serif", 7.8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.chkExternalKey.ForeColor = System.Drawing.SystemColors.ActiveCaptionText;
            this.chkExternalKey.Location = new System.Drawing.Point(364, 96);
            this.chkExternalKey.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.chkExternalKey.Name = "chkExternalKey";
            this.chkExternalKey.Size = new System.Drawing.Size(218, 24);
            this.chkExternalKey.TabIndex = 6;
            this.chkExternalKey.Text = "Use External Keyboard";
            this.chkExternalKey.UseVisualStyleBackColor = true;
            this.chkExternalKey.Visible = false;
            // 
            // SupervisorCred
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(746, 230);
            this.ControlBox = false;
            this.Controls.Add(this.panel1);
            this.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.MaximumSize = new System.Drawing.Size(768, 286);
            this.MinimumSize = new System.Drawing.Size(768, 286);
            this.Name = "SupervisorCred";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Supervisor Access Grant";
            this.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.AdminAccessCred_KeyPress);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Label lblMessage;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox txt_Cred;
        private System.Windows.Forms.Button btn_Proceed;
        private System.Windows.Forms.Button btn_Cancel;
        private System.Windows.Forms.CheckBox chkExternalKey;
    }
}