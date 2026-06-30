namespace VotRite.JimForms
{
    partial class AdminCred
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
            this.label1 = new System.Windows.Forms.Label();
            this.txt_Cred = new System.Windows.Forms.TextBox();
            this.chkExternalKey = new System.Windows.Forms.CheckBox();
            this.panel1 = new System.Windows.Forms.Panel();
            this.lblMessage = new System.Windows.Forms.Label();
            this.btn_Proceed = new System.Windows.Forms.Button();
            this.btn_Cancel = new System.Windows.Forms.Button();
            this.pictureBox4 = new System.Windows.Forms.PictureBox();
            this.txtnopwd = new System.Windows.Forms.TextBox();
            this.panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox4)).BeginInit();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 15F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.ForeColor = System.Drawing.SystemColors.ActiveCaptionText;
            this.label1.Location = new System.Drawing.Point(12, 50);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(339, 36);
            this.label1.TabIndex = 0;
            this.label1.Text = "Administrator Password:";
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
            this.txt_Cred.TabIndex = 0;
            this.txt_Cred.Enter += new System.EventHandler(this.txt_Cred_Enter);
            this.txt_Cred.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.txt_Cred_KeyPress);
            this.txt_Cred.Leave += new System.EventHandler(this.txt_Cred_Leave);
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
            // 
            // panel1
            // 
            this.panel1.BackColor = System.Drawing.Color.Transparent;
            this.panel1.Controls.Add(this.lblMessage);
            this.panel1.Controls.Add(this.label1);
            this.panel1.Controls.Add(this.txt_Cred);
            this.panel1.Controls.Add(this.btn_Proceed);
            this.panel1.Controls.Add(this.btn_Cancel);
            this.panel1.Controls.Add(this.chkExternalKey);
            this.panel1.Location = new System.Drawing.Point(576, 185);
            this.panel1.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(737, 228);
            this.panel1.TabIndex = 14;
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
            // btn_Proceed
            // 
            this.btn_Proceed.BackColor = System.Drawing.SystemColors.ButtonFace;
            this.btn_Proceed.BackgroundImage = global::VotRite.Properties.Resources.button_green;
            this.btn_Proceed.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.btn_Proceed.FlatAppearance.BorderSize = 0;
            this.btn_Proceed.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btn_Proceed.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btn_Proceed.ForeColor = System.Drawing.SystemColors.HighlightText;
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
            this.btn_Cancel.ForeColor = System.Drawing.SystemColors.HighlightText;
            this.btn_Cancel.Location = new System.Drawing.Point(538, 130);
            this.btn_Cancel.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.btn_Cancel.Name = "btn_Cancel";
            this.btn_Cancel.Size = new System.Drawing.Size(134, 48);
            this.btn_Cancel.TabIndex = 3;
            this.btn_Cancel.Text = "CANCEL";
            this.btn_Cancel.UseVisualStyleBackColor = false;
            this.btn_Cancel.Click += new System.EventHandler(this.btn_Cancel_Click);
            // 
            // pictureBox4
            // 
            this.pictureBox4.BackColor = System.Drawing.Color.Transparent;
            this.pictureBox4.BackgroundImage = global::VotRite.Properties.Resources.loginBg1;
            this.pictureBox4.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.pictureBox4.Location = new System.Drawing.Point(-2, -2);
            this.pictureBox4.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.pictureBox4.Name = "pictureBox4";
            this.pictureBox4.Size = new System.Drawing.Size(1334, 535);
            this.pictureBox4.TabIndex = 10;
            this.pictureBox4.TabStop = false;
            this.pictureBox4.Click += new System.EventHandler(this.pictureBox4_Click);
            // 
            // txtnopwd
            // 
            this.txtnopwd.Font = new System.Drawing.Font("Microsoft Sans Serif", 15F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtnopwd.Location = new System.Drawing.Point(818, -74);
            this.txtnopwd.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.txtnopwd.MaxLength = 12;
            this.txtnopwd.Name = "txtnopwd";
            this.txtnopwd.PasswordChar = '*';
            this.txtnopwd.Size = new System.Drawing.Size(307, 41);
            this.txtnopwd.TabIndex = 8;
            this.txtnopwd.Text = "Voting1234=";
            // 
            // AdminCred
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(1320, 529);
            this.ControlBox = false;
            this.Controls.Add(this.txtnopwd);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.pictureBox4);
            this.ForeColor = System.Drawing.SystemColors.WindowFrame;
            this.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.MaximumSize = new System.Drawing.Size(1342, 551);
            this.MinimumSize = new System.Drawing.Size(1342, 551);
            this.Name = "AdminCred";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Load += new System.EventHandler(this.AdminCred_Load);
            this.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.AdminCred_KeyPress);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox4)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox txt_Cred;
        private System.Windows.Forms.Button btn_Proceed;
        private System.Windows.Forms.Button btn_Cancel;
        private System.Windows.Forms.CheckBox chkExternalKey;
        private System.Windows.Forms.PictureBox pictureBox4;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Label lblMessage;
        private System.Windows.Forms.TextBox txtnopwd;
    }
}