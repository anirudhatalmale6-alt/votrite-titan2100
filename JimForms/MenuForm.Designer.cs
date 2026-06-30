
namespace VotRite.JimForms
{
    partial class MenuForm
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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MenuForm));
            this.labelFooter = new System.Windows.Forms.Label();
            this.picLogo = new System.Windows.Forms.PictureBox();
            this.panel1 = new System.Windows.Forms.Panel();
            this.lblExit = new System.Windows.Forms.Label();
            this.lblOption = new System.Windows.Forms.Label();
            this.lblReport = new System.Windows.Forms.Label();
            this.lblVoting = new System.Windows.Forms.Label();
            this.panelMenu = new System.Windows.Forms.Panel();
            this.lblBackup = new System.Windows.Forms.Label();
            this.lblMachine = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.labelHelp = new System.Windows.Forms.Label();
            this.picArrow = new System.Windows.Forms.PictureBox();
            this.panelOption = new System.Windows.Forms.Panel();
            this.label2 = new System.Windows.Forms.Label();
            this.lblAudit = new System.Windows.Forms.Label();
            this.lblConfiguration = new System.Windows.Forms.Label();
            this.lblResetVoting = new System.Windows.Forms.Label();
            this.panelVoting = new System.Windows.Forms.Panel();
            this.lblCurrentBallot = new System.Windows.Forms.Label();
            this.lblShowResult = new System.Windows.Forms.Label();
            this.lblBallotMode = new System.Windows.Forms.Label();
            this.lblCastVote = new System.Windows.Forms.Label();
            this.lblVoteCounter = new System.Windows.Forms.Label();
            this.lblLogCounter = new System.Windows.Forms.Label();
            this.lblMsg = new System.Windows.Forms.Label();
            this.lblTime = new System.Windows.Forms.Label();
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.timer2 = new System.Windows.Forms.Timer(this.components);
            ((System.ComponentModel.ISupportInitialize)(this.picLogo)).BeginInit();
            this.panel1.SuspendLayout();
            this.panelMenu.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.picArrow)).BeginInit();
            this.panelOption.SuspendLayout();
            this.panelVoting.SuspendLayout();
            this.SuspendLayout();
            // 
            // labelFooter
            // 
            this.labelFooter.AutoSize = true;
            this.labelFooter.BackColor = System.Drawing.Color.Transparent;
            this.labelFooter.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, ((System.Drawing.FontStyle)((System.Drawing.FontStyle.Bold | System.Drawing.FontStyle.Underline))), System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelFooter.ForeColor = System.Drawing.SystemColors.ButtonHighlight;
            this.labelFooter.Location = new System.Drawing.Point(518, 785);
            this.labelFooter.Name = "labelFooter";
            this.labelFooter.Size = new System.Drawing.Size(388, 22);
            this.labelFooter.TabIndex = 0;
            this.labelFooter.Text = "Copyright © Votrite LLC, USA 2020 - 2025";
            // 
            // picLogo
            // 
            this.picLogo.BackgroundImage = global::VotRite.Properties.Resources.logo;
            this.picLogo.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.picLogo.Location = new System.Drawing.Point(2, 74);
            this.picLogo.Margin = new System.Windows.Forms.Padding(3, 5, 3, 5);
            this.picLogo.Name = "picLogo";
            this.picLogo.Size = new System.Drawing.Size(112, 80);
            this.picLogo.TabIndex = 1;
            this.picLogo.TabStop = false;
            // 
            // panel1
            // 
            this.panel1.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("panel1.BackgroundImage")));
            this.panel1.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.panel1.Controls.Add(this.lblExit);
            this.panel1.Controls.Add(this.lblOption);
            this.panel1.Controls.Add(this.lblReport);
            this.panel1.Controls.Add(this.lblVoting);
            this.panel1.Location = new System.Drawing.Point(3, 18);
            this.panel1.Margin = new System.Windows.Forms.Padding(3, 5, 3, 5);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(710, 551);
            this.panel1.TabIndex = 2;
            // 
            // lblExit
            // 
            this.lblExit.BackColor = System.Drawing.Color.Transparent;
            this.lblExit.Font = new System.Drawing.Font("Microsoft Sans Serif", 20F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblExit.ForeColor = System.Drawing.Color.Red;
            this.lblExit.Location = new System.Drawing.Point(92, 422);
            this.lblExit.Name = "lblExit";
            this.lblExit.Size = new System.Drawing.Size(602, 49);
            this.lblExit.TabIndex = 3;
            this.lblExit.Text = "CLOSE MACHINE";
            this.lblExit.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.lblExit.Click += new System.EventHandler(this.lblExit_Click);
            this.lblExit.MouseLeave += new System.EventHandler(this.lblVoting_MouseLeave);
            this.lblExit.MouseHover += new System.EventHandler(this.lblVoting_MouseHover);
            // 
            // lblOption
            // 
            this.lblOption.BackColor = System.Drawing.Color.Transparent;
            this.lblOption.Font = new System.Drawing.Font("Microsoft Sans Serif", 20F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblOption.ForeColor = System.Drawing.Color.MidnightBlue;
            this.lblOption.Location = new System.Drawing.Point(98, 295);
            this.lblOption.Name = "lblOption";
            this.lblOption.Size = new System.Drawing.Size(596, 49);
            this.lblOption.TabIndex = 2;
            this.lblOption.Text = "OPTIONS && MORE";
            this.lblOption.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.lblOption.Click += new System.EventHandler(this.lblOption_Click);
            this.lblOption.MouseLeave += new System.EventHandler(this.lblVoting_MouseLeave);
            this.lblOption.MouseHover += new System.EventHandler(this.lblVoting_MouseHover);
            // 
            // lblReport
            // 
            this.lblReport.BackColor = System.Drawing.Color.Transparent;
            this.lblReport.Font = new System.Drawing.Font("Microsoft Sans Serif", 20F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblReport.ForeColor = System.Drawing.Color.MidnightBlue;
            this.lblReport.Location = new System.Drawing.Point(100, 158);
            this.lblReport.Name = "lblReport";
            this.lblReport.Size = new System.Drawing.Size(592, 49);
            this.lblReport.TabIndex = 1;
            this.lblReport.Text = "REPORTS";
            this.lblReport.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.lblReport.Click += new System.EventHandler(this.lblReport_Click);
            this.lblReport.MouseLeave += new System.EventHandler(this.lblVoting_MouseLeave);
            this.lblReport.MouseHover += new System.EventHandler(this.lblVoting_MouseHover);
            // 
            // lblVoting
            // 
            this.lblVoting.BackColor = System.Drawing.Color.Transparent;
            this.lblVoting.Font = new System.Drawing.Font("Microsoft Sans Serif", 20F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblVoting.ForeColor = System.Drawing.Color.MidnightBlue;
            this.lblVoting.Location = new System.Drawing.Point(96, 31);
            this.lblVoting.Name = "lblVoting";
            this.lblVoting.Size = new System.Drawing.Size(597, 49);
            this.lblVoting.TabIndex = 0;
            this.lblVoting.Text = "VOTING";
            this.lblVoting.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.lblVoting.Click += new System.EventHandler(this.lblVoting_Click);
            this.lblVoting.MouseLeave += new System.EventHandler(this.lblVoting_MouseLeave);
            this.lblVoting.MouseHover += new System.EventHandler(this.lblVoting_MouseHover);
            // 
            // panelMenu
            // 
            this.panelMenu.BackColor = System.Drawing.Color.Transparent;
            this.panelMenu.Controls.Add(this.lblBackup);
            this.panelMenu.Controls.Add(this.lblMachine);
            this.panelMenu.Controls.Add(this.label1);
            this.panelMenu.Controls.Add(this.labelHelp);
            this.panelMenu.Controls.Add(this.picArrow);
            this.panelMenu.Controls.Add(this.panelOption);
            this.panelMenu.Controls.Add(this.panelVoting);
            this.panelMenu.Controls.Add(this.panel1);
            this.panelMenu.Location = new System.Drawing.Point(254, 112);
            this.panelMenu.Margin = new System.Windows.Forms.Padding(3, 5, 3, 5);
            this.panelMenu.Name = "panelMenu";
            this.panelMenu.Size = new System.Drawing.Size(1472, 832);
            this.panelMenu.TabIndex = 3;
            // 
            // lblBackup
            // 
            this.lblBackup.BackColor = System.Drawing.Color.Transparent;
            this.lblBackup.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, ((System.Drawing.FontStyle)((System.Drawing.FontStyle.Bold | System.Drawing.FontStyle.Underline))), System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblBackup.ForeColor = System.Drawing.Color.MidnightBlue;
            this.lblBackup.Location = new System.Drawing.Point(3, 708);
            this.lblBackup.Name = "lblBackup";
            this.lblBackup.Size = new System.Drawing.Size(1243, 38);
            this.lblBackup.TabIndex = 10;
            this.lblBackup.Text = "File: ";
            // 
            // lblMachine
            // 
            this.lblMachine.BackColor = System.Drawing.Color.Transparent;
            this.lblMachine.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, ((System.Drawing.FontStyle)((System.Drawing.FontStyle.Bold | System.Drawing.FontStyle.Underline))), System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblMachine.ForeColor = System.Drawing.Color.MidnightBlue;
            this.lblMachine.Location = new System.Drawing.Point(802, 662);
            this.lblMachine.Name = "lblMachine";
            this.lblMachine.Size = new System.Drawing.Size(444, 31);
            this.lblMachine.TabIndex = 9;
            this.lblMachine.Text = "MACHINE:";
            this.lblMachine.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // label1
            // 
            this.label1.BackColor = System.Drawing.Color.Transparent;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, ((System.Drawing.FontStyle)((System.Drawing.FontStyle.Bold | System.Drawing.FontStyle.Underline))), System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.ForeColor = System.Drawing.Color.MidnightBlue;
            this.label1.Location = new System.Drawing.Point(349, 662);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(437, 31);
            this.label1.TabIndex = 8;
            this.label1.Text = "CLICK FOR HELP (Supervisor)";
            this.label1.Click += new System.EventHandler(this.label1_Click);
            // 
            // labelHelp
            // 
            this.labelHelp.BackColor = System.Drawing.Color.Transparent;
            this.labelHelp.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, ((System.Drawing.FontStyle)((System.Drawing.FontStyle.Bold | System.Drawing.FontStyle.Underline))), System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelHelp.ForeColor = System.Drawing.Color.MidnightBlue;
            this.labelHelp.Location = new System.Drawing.Point(3, 662);
            this.labelHelp.Name = "labelHelp";
            this.labelHelp.Size = new System.Drawing.Size(338, 31);
            this.labelHelp.TabIndex = 7;
            this.labelHelp.Text = "CLICK FOR HELP";
            this.labelHelp.Click += new System.EventHandler(this.labelHelp_Click);
            this.labelHelp.MouseLeave += new System.EventHandler(this.lblVoting_MouseLeave);
            this.labelHelp.MouseHover += new System.EventHandler(this.lblVoting_MouseHover);
            // 
            // picArrow
            // 
            this.picArrow.BackgroundImage = global::VotRite.Properties.Resources.connect;
            this.picArrow.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.picArrow.Location = new System.Drawing.Point(718, 69);
            this.picArrow.Margin = new System.Windows.Forms.Padding(3, 5, 3, 5);
            this.picArrow.Name = "picArrow";
            this.picArrow.Size = new System.Drawing.Size(76, 62);
            this.picArrow.TabIndex = 5;
            this.picArrow.TabStop = false;
            this.picArrow.Visible = false;
            // 
            // panelOption
            // 
            this.panelOption.BackgroundImage = global::VotRite.Properties.Resources.submenuBG;
            this.panelOption.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.panelOption.Controls.Add(this.label2);
            this.panelOption.Controls.Add(this.lblAudit);
            this.panelOption.Controls.Add(this.lblConfiguration);
            this.panelOption.Controls.Add(this.lblResetVoting);
            this.panelOption.Location = new System.Drawing.Point(802, 332);
            this.panelOption.Margin = new System.Windows.Forms.Padding(3, 5, 3, 5);
            this.panelOption.Name = "panelOption";
            this.panelOption.Size = new System.Drawing.Size(444, 290);
            this.panelOption.TabIndex = 4;
            this.panelOption.Visible = false;
            // 
            // label2
            // 
            this.label2.BackColor = System.Drawing.Color.Transparent;
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 8F, ((System.Drawing.FontStyle)((System.Drawing.FontStyle.Bold | System.Drawing.FontStyle.Underline))), System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.ForeColor = System.Drawing.Color.MidnightBlue;
            this.label2.Location = new System.Drawing.Point(94, 265);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(338, 31);
            this.label2.TabIndex = 8;
            this.label2.Text = "CHANGE PASSWORDS";
            this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.label2.Click += new System.EventHandler(this.label2_Click);
            this.label2.MouseLeave += new System.EventHandler(this.lblVoting_MouseLeave);
            this.label2.MouseHover += new System.EventHandler(this.lblVoting_MouseHover);
            // 
            // lblAudit
            // 
            this.lblAudit.BackColor = System.Drawing.Color.Transparent;
            this.lblAudit.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblAudit.ForeColor = System.Drawing.Color.DarkGreen;
            this.lblAudit.Location = new System.Drawing.Point(76, 110);
            this.lblAudit.Name = "lblAudit";
            this.lblAudit.Size = new System.Drawing.Size(348, 31);
            this.lblAudit.TabIndex = 5;
            this.lblAudit.Text = "AUDIT SECTION";
            this.lblAudit.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.lblAudit.Click += new System.EventHandler(this.lblAudit_Click);
            this.lblAudit.MouseLeave += new System.EventHandler(this.lblVoting_MouseLeave);
            this.lblAudit.MouseHover += new System.EventHandler(this.lblVoting_MouseHover);
            // 
            // lblConfiguration
            // 
            this.lblConfiguration.BackColor = System.Drawing.Color.Transparent;
            this.lblConfiguration.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblConfiguration.ForeColor = System.Drawing.Color.Chocolate;
            this.lblConfiguration.Location = new System.Drawing.Point(76, 198);
            this.lblConfiguration.Name = "lblConfiguration";
            this.lblConfiguration.Size = new System.Drawing.Size(348, 31);
            this.lblConfiguration.TabIndex = 4;
            this.lblConfiguration.Text = "MACHINE CONFIGURATION";
            this.lblConfiguration.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.lblConfiguration.Click += new System.EventHandler(this.lblConfiguration_Click);
            this.lblConfiguration.MouseLeave += new System.EventHandler(this.lblVoting_MouseLeave);
            this.lblConfiguration.MouseHover += new System.EventHandler(this.lblVoting_MouseHover);
            // 
            // lblResetVoting
            // 
            this.lblResetVoting.BackColor = System.Drawing.Color.Transparent;
            this.lblResetVoting.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblResetVoting.ForeColor = System.Drawing.Color.MidnightBlue;
            this.lblResetVoting.Location = new System.Drawing.Point(76, 25);
            this.lblResetVoting.Name = "lblResetVoting";
            this.lblResetVoting.Size = new System.Drawing.Size(348, 31);
            this.lblResetVoting.TabIndex = 1;
            this.lblResetVoting.Text = "PRINT TOOL";
            this.lblResetVoting.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.lblResetVoting.Click += new System.EventHandler(this.lblResetVoting_Click);
            this.lblResetVoting.MouseLeave += new System.EventHandler(this.lblVoting_MouseLeave);
            this.lblResetVoting.MouseHover += new System.EventHandler(this.lblVoting_MouseHover);
            // 
            // panelVoting
            // 
            this.panelVoting.BackgroundImage = global::VotRite.Properties.Resources.submenuBG;
            this.panelVoting.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.panelVoting.Controls.Add(this.lblCurrentBallot);
            this.panelVoting.Controls.Add(this.lblShowResult);
            this.panelVoting.Controls.Add(this.lblBallotMode);
            this.panelVoting.Controls.Add(this.lblCastVote);
            this.panelVoting.Location = new System.Drawing.Point(802, 55);
            this.panelVoting.Margin = new System.Windows.Forms.Padding(3, 5, 3, 5);
            this.panelVoting.Name = "panelVoting";
            this.panelVoting.Size = new System.Drawing.Size(444, 272);
            this.panelVoting.TabIndex = 3;
            this.panelVoting.Visible = false;
            // 
            // lblCurrentBallot
            // 
            this.lblCurrentBallot.BackColor = System.Drawing.Color.Transparent;
            this.lblCurrentBallot.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblCurrentBallot.ForeColor = System.Drawing.Color.MidnightBlue;
            this.lblCurrentBallot.Location = new System.Drawing.Point(78, 105);
            this.lblCurrentBallot.Name = "lblCurrentBallot";
            this.lblCurrentBallot.Size = new System.Drawing.Size(346, 31);
            this.lblCurrentBallot.TabIndex = 6;
            this.lblCurrentBallot.Text = "OPEN CURRENT BALLOT";
            this.lblCurrentBallot.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.lblCurrentBallot.Visible = false;
            this.lblCurrentBallot.Click += new System.EventHandler(this.lblCurrentBallot_Click);
            this.lblCurrentBallot.MouseLeave += new System.EventHandler(this.lblVoting_MouseLeave);
            this.lblCurrentBallot.MouseHover += new System.EventHandler(this.lblVoting_MouseHover);
            // 
            // lblShowResult
            // 
            this.lblShowResult.BackColor = System.Drawing.Color.Transparent;
            this.lblShowResult.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblShowResult.ForeColor = System.Drawing.Color.Chocolate;
            this.lblShowResult.Location = new System.Drawing.Point(70, 182);
            this.lblShowResult.Name = "lblShowResult";
            this.lblShowResult.Size = new System.Drawing.Size(352, 31);
            this.lblShowResult.TabIndex = 3;
            this.lblShowResult.Text = "SHOW LAST RESULT";
            this.lblShowResult.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.lblShowResult.Click += new System.EventHandler(this.lblShowResult_Click);
            this.lblShowResult.MouseLeave += new System.EventHandler(this.lblVoting_MouseLeave);
            this.lblShowResult.MouseHover += new System.EventHandler(this.lblVoting_MouseHover);
            // 
            // lblBallotMode
            // 
            this.lblBallotMode.AutoSize = true;
            this.lblBallotMode.BackColor = System.Drawing.Color.Transparent;
            this.lblBallotMode.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblBallotMode.ForeColor = System.Drawing.Color.MidnightBlue;
            this.lblBallotMode.Location = new System.Drawing.Point(98, 105);
            this.lblBallotMode.Name = "lblBallotMode";
            this.lblBallotMode.Size = new System.Drawing.Size(311, 29);
            this.lblBallotMode.TabIndex = 1;
            this.lblBallotMode.Text = "CHOOSE BALLOT MODE";
            this.lblBallotMode.Visible = false;
            this.lblBallotMode.Click += new System.EventHandler(this.lblBallotMode_Click);
            this.lblBallotMode.MouseLeave += new System.EventHandler(this.lblVoting_MouseLeave);
            this.lblBallotMode.MouseHover += new System.EventHandler(this.lblVoting_MouseHover);
            // 
            // lblCastVote
            // 
            this.lblCastVote.BackColor = System.Drawing.Color.Transparent;
            this.lblCastVote.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblCastVote.ForeColor = System.Drawing.Color.DarkGreen;
            this.lblCastVote.Location = new System.Drawing.Point(72, 25);
            this.lblCastVote.Name = "lblCastVote";
            this.lblCastVote.Size = new System.Drawing.Size(352, 31);
            this.lblCastVote.TabIndex = 2;
            this.lblCastVote.Text = "UNLOCK BALLOT";
            this.lblCastVote.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.lblCastVote.Click += new System.EventHandler(this.lblCastVote_Click);
            this.lblCastVote.MouseLeave += new System.EventHandler(this.lblVoting_MouseLeave);
            this.lblCastVote.MouseHover += new System.EventHandler(this.lblVoting_MouseHover);
            // 
            // lblVoteCounter
            // 
            this.lblVoteCounter.AutoSize = true;
            this.lblVoteCounter.BackColor = System.Drawing.Color.Transparent;
            this.lblVoteCounter.Font = new System.Drawing.Font("Microsoft Sans Serif", 20F, ((System.Drawing.FontStyle)((System.Drawing.FontStyle.Bold | System.Drawing.FontStyle.Underline))), System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblVoteCounter.ForeColor = System.Drawing.Color.MidnightBlue;
            this.lblVoteCounter.Location = new System.Drawing.Point(11, 969);
            this.lblVoteCounter.Name = "lblVoteCounter";
            this.lblVoteCounter.Size = new System.Drawing.Size(485, 46);
            this.lblVoteCounter.TabIndex = 4;
            this.lblVoteCounter.Text = "VOTE CAST COUNTER:";
            // 
            // lblLogCounter
            // 
            this.lblLogCounter.AutoSize = true;
            this.lblLogCounter.BackColor = System.Drawing.Color.Transparent;
            this.lblLogCounter.Font = new System.Drawing.Font("Microsoft Sans Serif", 20F, ((System.Drawing.FontStyle)((System.Drawing.FontStyle.Bold | System.Drawing.FontStyle.Underline))), System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblLogCounter.ForeColor = System.Drawing.Color.MidnightBlue;
            this.lblLogCounter.Location = new System.Drawing.Point(1088, 969);
            this.lblLogCounter.Name = "lblLogCounter";
            this.lblLogCounter.Size = new System.Drawing.Size(336, 46);
            this.lblLogCounter.TabIndex = 5;
            this.lblLogCounter.Text = "LOG COUNTER:";
            // 
            // lblMsg
            // 
            this.lblMsg.AutoSize = true;
            this.lblMsg.BackColor = System.Drawing.Color.Transparent;
            this.lblMsg.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblMsg.ForeColor = System.Drawing.Color.Red;
            this.lblMsg.Location = new System.Drawing.Point(14, 632);
            this.lblMsg.Name = "lblMsg";
            this.lblMsg.Size = new System.Drawing.Size(0, 29);
            this.lblMsg.TabIndex = 6;
            // 
            // lblTime
            // 
            this.lblTime.BackColor = System.Drawing.Color.Transparent;
            this.lblTime.Font = new System.Drawing.Font("Microsoft Sans Serif", 20F, ((System.Drawing.FontStyle)((System.Drawing.FontStyle.Bold | System.Drawing.FontStyle.Underline))), System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblTime.ForeColor = System.Drawing.Color.MidnightBlue;
            this.lblTime.Location = new System.Drawing.Point(1426, 67);
            this.lblTime.Name = "lblTime";
            this.lblTime.Size = new System.Drawing.Size(452, 40);
            this.lblTime.TabIndex = 11;
            this.lblTime.Text = "Time:";
            // 
            // timer1
            // 
            this.timer1.Interval = 1000;
            this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
            // 
            // timer2
            // 
            this.timer2.Tick += new System.EventHandler(this.timer2_Tick);
            // 
            // MenuForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("$this.BackgroundImage")));
            this.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.ClientSize = new System.Drawing.Size(1884, 1084);
            this.ControlBox = false;
            this.Controls.Add(this.lblTime);
            this.Controls.Add(this.lblMsg);
            this.Controls.Add(this.lblLogCounter);
            this.Controls.Add(this.lblVoteCounter);
            this.Controls.Add(this.panelMenu);
            this.Controls.Add(this.picLogo);
            this.Controls.Add(this.labelFooter);
            this.DoubleBuffered = true;
            this.Margin = new System.Windows.Forms.Padding(3, 5, 3, 5);
            this.Name = "MenuForm";
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.MenuForm_FormClosing);
            this.Load += new System.EventHandler(this.MenuForm_Load);
            this.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.MenuForm_KeyPress);
            this.Resize += new System.EventHandler(this.MenuForm_Resize);
            ((System.ComponentModel.ISupportInitialize)(this.picLogo)).EndInit();
            this.panel1.ResumeLayout(false);
            this.panelMenu.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.picArrow)).EndInit();
            this.panelOption.ResumeLayout(false);
            this.panelVoting.ResumeLayout(false);
            this.panelVoting.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label labelFooter;
        private System.Windows.Forms.PictureBox picLogo;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Label lblOption;
        private System.Windows.Forms.Label lblReport;
        private System.Windows.Forms.Label lblVoting;
        private System.Windows.Forms.Label lblExit;
        private System.Windows.Forms.Panel panelMenu;
        private System.Windows.Forms.Panel panelVoting;
        private System.Windows.Forms.Label lblShowResult;
        private System.Windows.Forms.Label lblCastVote;
        private System.Windows.Forms.Label lblBallotMode;
        private System.Windows.Forms.Panel panelOption;
        private System.Windows.Forms.Label lblAudit;
        private System.Windows.Forms.Label lblConfiguration;
        private System.Windows.Forms.Label lblResetVoting;
        private System.Windows.Forms.Label lblVoteCounter;
        private System.Windows.Forms.Label lblLogCounter;
        private System.Windows.Forms.PictureBox picArrow;
        private System.Windows.Forms.Label lblMsg;
        private System.Windows.Forms.Label lblCurrentBallot;
        private System.Windows.Forms.Label labelHelp;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label lblBackup;
        private System.Windows.Forms.Label lblMachine;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label lblTime;
        private System.Windows.Forms.Timer timer1;
        private System.Windows.Forms.Timer timer2;
    }
}