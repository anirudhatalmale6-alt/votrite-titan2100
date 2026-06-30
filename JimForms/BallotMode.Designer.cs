
namespace VotRite.JimForms
{
    partial class BallotMode
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
            this.radioButton1 = new System.Windows.Forms.RadioButton();
            this.radioButton2 = new System.Windows.Forms.RadioButton();
            this.button1 = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.lblSpeak = new System.Windows.Forms.Label();
            this.panelSpeech = new System.Windows.Forms.Panel();
            this.button2 = new System.Windows.Forms.Button();
            this.lblRate = new System.Windows.Forms.Label();
            this.hsbarSpeechRate = new System.Windows.Forms.HScrollBar();
            this.lblRateText = new System.Windows.Forms.Label();
            this.panelSpeech.SuspendLayout();
            this.SuspendLayout();
            // 
            // radioButton1
            // 
            this.radioButton1.AutoSize = true;
            this.radioButton1.BackColor = System.Drawing.Color.Transparent;
            this.radioButton1.Checked = true;
            this.radioButton1.Font = new System.Drawing.Font("Microsoft Sans Serif", 15F, ((System.Drawing.FontStyle)((System.Drawing.FontStyle.Bold | System.Drawing.FontStyle.Underline))), System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.radioButton1.ForeColor = System.Drawing.SystemColors.ActiveCaptionText;
            this.radioButton1.Location = new System.Drawing.Point(14, 18);
            this.radioButton1.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.radioButton1.Name = "radioButton1";
            this.radioButton1.Size = new System.Drawing.Size(276, 40);
            this.radioButton1.TabIndex = 0;
            this.radioButton1.TabStop = true;
            this.radioButton1.Text = "NORMAL MODE";
            this.radioButton1.UseVisualStyleBackColor = false;
            // 
            // radioButton2
            // 
            this.radioButton2.AutoSize = true;
            this.radioButton2.BackColor = System.Drawing.Color.Transparent;
            this.radioButton2.Font = new System.Drawing.Font("Microsoft Sans Serif", 15F, ((System.Drawing.FontStyle)((System.Drawing.FontStyle.Bold | System.Drawing.FontStyle.Underline))), System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.radioButton2.ForeColor = System.Drawing.SystemColors.ActiveCaptionText;
            this.radioButton2.Location = new System.Drawing.Point(346, 18);
            this.radioButton2.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.radioButton2.Name = "radioButton2";
            this.radioButton2.Size = new System.Drawing.Size(457, 40);
            this.radioButton2.TabIndex = 1;
            this.radioButton2.Text = "VISUALLY IMPAIRED MODE";
            this.radioButton2.UseVisualStyleBackColor = false;
            this.radioButton2.CheckedChanged += new System.EventHandler(this.radioButton2_CheckedChanged);
            // 
            // button1
            // 
            this.button1.BackColor = System.Drawing.Color.LimeGreen;
            this.button1.Font = new System.Drawing.Font("Microsoft Sans Serif", 15F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.button1.ForeColor = System.Drawing.SystemColors.InfoText;
            this.button1.Location = new System.Drawing.Point(402, 150);
            this.button1.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(259, 49);
            this.button1.TabIndex = 2;
            this.button1.Text = "Set Mode";
            this.button1.UseVisualStyleBackColor = false;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.BackColor = System.Drawing.Color.Transparent;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, ((System.Drawing.FontStyle)((System.Drawing.FontStyle.Bold | System.Drawing.FontStyle.Underline))), System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.ForeColor = System.Drawing.Color.DeepPink;
            this.label1.Location = new System.Drawing.Point(706, 160);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(100, 25);
            this.label1.TabIndex = 3;
            this.label1.Text = "CANCEL";
            this.label1.Click += new System.EventHandler(this.label1_Click);
            // 
            // lblSpeak
            // 
            this.lblSpeak.AutoSize = true;
            this.lblSpeak.Location = new System.Drawing.Point(14, 140);
            this.lblSpeak.Name = "lblSpeak";
            this.lblSpeak.Size = new System.Drawing.Size(0, 20);
            this.lblSpeak.TabIndex = 4;
            // 
            // panelSpeech
            // 
            this.panelSpeech.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panelSpeech.Controls.Add(this.lblRateText);
            this.panelSpeech.Controls.Add(this.button2);
            this.panelSpeech.Controls.Add(this.lblRate);
            this.panelSpeech.Controls.Add(this.hsbarSpeechRate);
            this.panelSpeech.Location = new System.Drawing.Point(121, 65);
            this.panelSpeech.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.panelSpeech.Name = "panelSpeech";
            this.panelSpeech.Size = new System.Drawing.Size(688, 77);
            this.panelSpeech.TabIndex = 5;
            this.panelSpeech.Visible = false;
            // 
            // button2
            // 
            this.button2.BackColor = System.Drawing.Color.LimeGreen;
            this.button2.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.button2.ForeColor = System.Drawing.SystemColors.InfoText;
            this.button2.Location = new System.Drawing.Point(533, 5);
            this.button2.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(148, 36);
            this.button2.TabIndex = 26;
            this.button2.Text = "Test Speech";
            this.button2.UseVisualStyleBackColor = false;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // lblRate
            // 
            this.lblRate.AutoSize = true;
            this.lblRate.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.lblRate.Location = new System.Drawing.Point(7, 9);
            this.lblRate.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblRate.Name = "lblRate";
            this.lblRate.Size = new System.Drawing.Size(131, 25);
            this.lblRate.TabIndex = 25;
            this.lblRate.Text = "Speech Rate:";
            // 
            // hsbarSpeechRate
            // 
            this.hsbarSpeechRate.LargeChange = 1;
            this.hsbarSpeechRate.Location = new System.Drawing.Point(151, 6);
            this.hsbarSpeechRate.Maximum = 10;
            this.hsbarSpeechRate.Minimum = 1;
            this.hsbarSpeechRate.Name = "hsbarSpeechRate";
            this.hsbarSpeechRate.Size = new System.Drawing.Size(373, 28);
            this.hsbarSpeechRate.TabIndex = 24;
            this.hsbarSpeechRate.Value = 2;
            this.hsbarSpeechRate.ValueChanged += new System.EventHandler(this.hsbarSpeechRate_ValueChanged);
            // 
            // lblRateText
            // 
            this.lblRateText.AutoSize = true;
            this.lblRateText.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.lblRateText.Location = new System.Drawing.Point(149, 47);
            this.lblRateText.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblRateText.Name = "lblRateText";
            this.lblRateText.Size = new System.Drawing.Size(119, 22);
            this.lblRateText.TabIndex = 27;
            this.lblRateText.Text = "Speech Rate:";
            // 
            // BallotMode
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.ButtonHighlight;
            this.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.ClientSize = new System.Drawing.Size(836, 218);
            this.ControlBox = false;
            this.Controls.Add(this.panelSpeech);
            this.Controls.Add(this.lblSpeak);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.radioButton2);
            this.Controls.Add(this.radioButton1);
            this.DoubleBuffered = true;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.Fixed3D;
            this.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.Name = "BallotMode";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Load += new System.EventHandler(this.BallotMode_Load);
            this.panelSpeech.ResumeLayout(false);
            this.panelSpeech.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.RadioButton radioButton1;
        private System.Windows.Forms.RadioButton radioButton2;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label lblSpeak;
        private System.Windows.Forms.Panel panelSpeech;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.Label lblRate;
        private System.Windows.Forms.HScrollBar hsbarSpeechRate;
        private System.Windows.Forms.Label lblRateText;
    }
}