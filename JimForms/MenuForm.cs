using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using VotRite.Definition;
using VotRite.Util;

namespace VotRite.JimForms
{
    public partial class MenuForm : Form
    {
        private const int ERROR_DELAY = 2000;

        private BallotDefinition ballot = null;
        private static PinPadDefinition definition;
        private StringBuilder buffer;
        private string input;
        private Constants.PinPadState state;
        private Timer errorDelay;
       private PinPadMenuDefinition menu;
        private Session.BallotModes _ballotMode;
        private Constants.keyAction keyAction;
        private System.Windows.Forms.TextBox displaybox;

        public MenuForm()
        {
            InitializeComponent();
        }

        private void MenuForm_Resize(object sender, EventArgs e)
        {
            try
            {
                labelFooter.Left = (this.Width / 2) - (labelFooter.Width / 2);
                labelFooter.Top = this.Bottom - 40;
                picLogo.Left = this.Left;
                lblVoteCounter.Left = this.Left + 50;
                lblLogCounter.Left = this.Right - lblLogCounter.Width - 150;

                lblVoteCounter.Top = lblLogCounter.Top = this.Bottom - 100;
                panelMenu.Left = (this.Width / 2) - (panelMenu.Width / 2) - 50;
                panelMenu.Top = (this.Height / 2) - (panelMenu.Height / 2) - 50;

                lblMsg.Left = lblVoteCounter.Left;
                lblMsg.Top = lblVoteCounter.Top - 100;

                lblTime.Left = this.Width - lblTime.Width - 25;
                lblTime.Top = picLogo.Top + 10;
                this.TopMost = true;     // Force on top
                this.Activate();         // Give focus
                this.TopMost = false;
            }
            catch (Exception)
            {

            }
        }

        private void lblExit_Click(object sender, EventArgs e)
        {
            try
            {
                if (!adminaccess())
                    return;

                if (MessageBox.Show("Votrite will exit now. Do you want to exit??", "Votrite Console", MessageBoxButtons.YesNo, MessageBoxIcon.Question) ==
                    DialogResult.Yes)
                {
                    try
                    {
                        Taskbar.Show();
                    }
                    catch (Exception)
                    {
                    }
                    HiddenFolder(false);
                    AppManager.Instance.CloseOskProcess();
                    Application.Exit();
                }
                //this.Close();
            }
            catch (Exception ex)
            {

            }
        }

        public void HiddenFolder(bool hide)
        {
            try
            {
                string name = Application.StartupPath;
                DirectoryInfo di = new DirectoryInfo(name);
               // name = Path.Combine(di.Parent.FullName, "Help");
                di = new DirectoryInfo(name);
                if (hide)
                    di.Attributes |= FileAttributes.Hidden;
                else
                    di.Attributes = di.Attributes & ~FileAttributes.Hidden;
            }
            catch (Exception)
            {
            }

            
        }

        private void MenuForm_Load(object sender, EventArgs e)
        {
            lblVoteCounter.Text = "VOTE CAST COUNTER: " + AppManager.Instance.CastCounter;
            lblLogCounter.Text = "LOG COUNTER: " + AppManager.Instance.LogCounter;
            lblMachine.Text = "MACHINE: " + AppManager.Configuration["System"]["Machine"];
            lblBackup.Text = "BACKUP FILE: " + AppManager.Configuration["System"]["BackupFlashDrivePath"];


            //if (AppManager.Configuration["System"]["Machine"] == "")
            //{
            //    lblBallotMode.Visible = true;
            //    lblCurrentBallot.Visible = false;
            //}
            //else
            //{
            //    lblBallotMode.Visible = false;
            //    lblCurrentBallot.Visible = true;
            //}
            lblCurrentBallot.Visible = true;
            lblTime.Text = DateTime.Now.ToString("MM/dd/yyyy HH:mm:ss");
            timer1.Enabled = true;
            timer1.Start();

            try
            {
                Taskbar.Hide();
            }
            catch (Exception)
            {
            }

            try
            {
                Taskbar.CloseWinExplorers(Application.StartupPath);
            }
            catch (Exception)
            {

            }
            
        }

        public bool ExportWorkbookToPdf(string filename, string folder)
        {
            // If either required string is null or empty, stop and bail out

            
            string workbookPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, folder, filename + ".xlsx");
            string outputPath = workbookPath.Replace(".xlsx", ".pdf");
            if (string.IsNullOrEmpty(workbookPath) || string.IsNullOrEmpty(outputPath))
            {
                return false;
            }

            // Create COM Objects
            Microsoft.Office.Interop.Excel.Application excelApplication;
            Microsoft.Office.Interop.Excel.Workbook excelWorkbook;

            // Create new instance of Excel
            excelApplication = new Microsoft.Office.Interop.Excel.Application();

            // Make the process invisible to the user
            excelApplication.ScreenUpdating = false;

            // Make the process silent
            excelApplication.DisplayAlerts = false;

            // Open the workbook that you wish to export to PDF
            excelWorkbook = excelApplication.Workbooks.Open(workbookPath);

            // If the workbook failed to open, stop, clean up, and bail out
            if (excelWorkbook == null)
            {
                excelApplication.Quit();

                excelApplication = null;
                excelWorkbook = null;

                return false;
            }

            var exportSuccessful = true;
            try
            {
                // Call Excel's native export function (valid in Office 2007 and Office 2010, AFAIK)
                excelWorkbook.ExportAsFixedFormat(Microsoft.Office.Interop.Excel.XlFixedFormatType.xlTypePDF, outputPath);
            }
            catch (System.Exception ex)
            {
                // Mark the export as failed for the return value...
                exportSuccessful = false;

                // Do something with any exceptions here, if you wish...
                // MessageBox.Show...        
            }
            finally
            {
                // Close the workbook, quit the Excel, and clean up regardless of the results...
                excelWorkbook.Close();
                excelApplication.Quit();

                excelApplication = null;
                excelWorkbook = null;
            }

            // You can use the following method to automatically open the PDF after export if you wish
            // Make sure that the file actually exists first...
            

            return exportSuccessful;
        }

        public static void pdfCreate(string filename, string folder)
        {
            try
            {
                string path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, folder, filename + ".xlsx");

                GemBox.Spreadsheet.ExcelFile file = GemBox.Spreadsheet.ExcelFile.Load(path);

                GemBox.Spreadsheet.ExcelWorksheet excelWorksheet = file.Worksheets[0];

                excelWorksheet.PrintOptions.FitWorksheetWidthToPages = 1;
                //   excelWorksheet.PrintOptions.FitToPage = false;

                file.Save(path.Replace(".xlsx", ".pdf"), new GemBox.Spreadsheet.PdfSaveOptions()//
                {
                    SelectionType = GemBox.Spreadsheet.SelectionType.EntireFile, ConformanceLevel = GemBox.Spreadsheet.PdfConformanceLevel.None,
                     ImageDpi = 330, MetafileScaleFactor = 1
                     
                });
                //File.Delete(path);
            }
            catch (Exception ex)
            {
                System.Windows.Forms.MessageBox.Show(ex.Message);
            }
        }

        private void lblVoting_Click(object sender, EventArgs e)
        {
            //ExportWorkbookToPdf("Test", "records");
            panelVoting.Visible = true;
            panelOption.Visible = false;
            picArrow.Visible = true;
            picArrow.Top = panelVoting.Top;
        }

        private void lblOption_Click(object sender, EventArgs e)
        {
            if (adminaccess())
            {
                panelVoting.Visible = false;
                panelOption.Visible = true;
                picArrow.Visible = true;
                picArrow.Top = panelOption.Top;
            }
        }

        private void lblReport_Click(object sender, EventArgs e)
        {
            if (adminaccess())
            {
                //lblMsg.Text = "Working, please wait ...";
                //lblMsg.Refresh();
                panelVoting.Visible = false;
                panelOption.Visible = false;
                picArrow.Visible = false;
                AppManager.Instance.PrintFinalReportStarted = true;
                //AppManager.PrinterEnabled = true;
                AppManager.Instance.PrintReport();
                //AppManager.Instance.PrintReportSmit();
                AppManager.Instance.PrintFinalReportStarted = false;
                lblMsg.Text = "";
                lblMsg.Refresh();
            }

        }

        private void lblCastVote_Click(object sender, EventArgs e)
        {
            try
            {
                PollworkerCred frmPollcred = new PollworkerCred();
                frmPollcred.ShowDialog();
                if (!frmPollcred.ok)
                    return;

                if (ballot != null)
                {
                    ballot.Dispose();
                    ballot = null;
                }
                //if (keyAction == Constants.keyAction.Unlock)
                //{
                if (AppManager.Configuration["System"]["Machine"] == "")
                {
                    JimForms.ConfigForm_Machine frmMachine = new JimForms.ConfigForm_Machine();
                    frmMachine.ShowDialog();
                    if (!frmMachine._set)
                        return;
                    lblMachine.Text = "MACHINE: " + AppManager.Configuration["System"]["Machine"];
                    lblBackup.Text = "BACKUP FILE: " + AppManager.Configuration["System"]["BackupFlashDrivePath"];


                }
                //if (AppManager.Configuration["System"]["Machine"] == "")
                //{
                //    lblBallotMode.Visible = true;
                //    lblCurrentBallot.Visible = false;
                //}
                //else
                //{
                //    lblBallotMode.Visible = false;
                //    lblCurrentBallot.Visible = true;
                //}
                keyAction = Constants.keyAction.none;
                JimForms.BallotsListDisplay frmBallots = new JimForms.BallotsListDisplay();
                frmBallots.ShowDialog();
                if (!frmBallots._default)
                {
                    //AppManager.Instance.ShowSoftPinpad("main", false);
                    return;
                }
                if (frmBallots._ballotMode != null)
                    _ballotMode = (Session.BallotModes)frmBallots._ballotMode;
                //}
                AppManager.Instance.VerifyFlashDrive();
                ballot = (BallotDefinition)DefinitionParser.Instance.FillBallotContent(null, null);// .FillBallotContent(null);
                ballot.BallotMode = _ballotMode;
                // Logger.Instance.Write("ballot: " + ballot.Id);

                DateTime now = DateTime.Now;

                if (now < ballot.StartTime)
                {
                    MessageBox.Show("Ballot not started yet", "Votrite Voting Ballot", MessageBoxButtons.OK, MessageBoxIcon.Error);

                    AppManager.Instance.ResetBallot();
                    return;
                }

                if (now > ballot.EndTime)
                {

                    MessageBox.Show("Ballot Closed, can not vote for this ballot.", "Votrite Voting Ballot", MessageBoxButtons.OK, MessageBoxIcon.Error);

                    AppManager.Instance.ResetBallot();
                    return;
                }

                AppManager.Instance.OpenBallot(ballot, true);
                this.Close();
            }
            catch (Exception)
            {

            }
        }

        private void lblShowResult_Click(object sender, EventArgs e)
        {
            if (adminaccess())
            {
                AppManager.Instance.ShowResults();
                this.Close();
            }
        }

        private void lblBallotMode_Click(object sender, EventArgs e)
        {
            BallotMode frmmode = new BallotMode();
            frmmode.ShowDialog();
            if (frmmode.normal)
                _ballotMode = Session.BallotModes.Normal;
            else
                _ballotMode = Session.BallotModes.Audio;

            if (ballot != null)
                ballot.BallotMode = _ballotMode;
            if(AppManager.Instance.Session != null)
            {
                AppManager.Instance.Session.BallotMode = _ballotMode;
            }
        }

        private void lblResetVoting_Click(object sender, EventArgs e)
        {

            //if (MessageBox.Show("Are you sure to reset entire Voting??", "Votrite Console", MessageBoxButtons.YesNo, MessageBoxIcon.Question) ==
            //   DialogResult.Yes)
            //{

            //    lblMsg.Text = "Working, please wait ..";
            //    lblMsg.Refresh();
            //    Util.IOHandler.SaveConfig("System", "Machine", "");
            //    AppManager.Configuration = Util.IOHandler.DecryptConfig();

            //    AppManager.UpdateMachineValue();
            //    lblMsg.Text = "";
            //    lblMsg.Refresh();
            //    panelOption.Visible = false;
            //    picArrow.Visible = false;
            //}

            
                RecordReportPrint frmPrintTool = new RecordReportPrint();
                frmPrintTool.ShowDialog();
            
        }

        private void lblConfiguration_Click(object sender, EventArgs e)
        {
            JimForms.ConfigForm frmConfig = new JimForms.ConfigForm();
            frmConfig.ShowDialog();
            panelOption.Visible = false;
            picArrow.Visible = false;
        }

        private void MenuForm_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == '\u001b')
                lblExit_Click(null, null);
        }

        public static bool adminaccess()
        {
            bool toret = false;

            AdminAccessCred frmAdmin = new AdminAccessCred();
            frmAdmin.ShowDialog();
            toret = frmAdmin.ok;
            return toret;
        }

        private void lblVoting_MouseHover(object sender, EventArgs e)
        {
            this.Cursor = Cursors.Hand;
        }

        private void lblVoting_MouseLeave(object sender, EventArgs e)
        {
            this.Cursor = Cursors.Default;
        }

        private void lblAudit_Click(object sender, EventArgs e)
        {
            audit frmAudit = new audit();
            panelOption.Visible = false;
            picArrow.Visible = false;
            frmAudit.ShowDialog();
        }

        private void lblCurrentBallot_Click(object sender, EventArgs e)
        {
            try
            {
                PollworkerCred frmPollcred = new PollworkerCred();
                frmPollcred.ShowDialog();
                if (!frmPollcred.ok)
                    return;

                if (AppManager.Configuration["System"]["Machine"] == "")
                {
                    lblCastVote_Click(null, null);
                    return;
                }
                else
                    AppManager.Instance.VerifyFlashDrive();

                if (ballot != null)
                {
                    ballot.Dispose();
                    ballot = null;
                }
                ballot = (BallotDefinition)DefinitionParser.Instance.FillBallotContent(null, null);// .FillBallotContent(null);
                ballot.BallotMode = _ballotMode;
                // Logger.Instance.Write("ballot: " + ballot.Id);

                DateTime now = DateTime.Now;

                if (now < ballot.StartTime)
                {
                    MessageBox.Show("Ballot not started yet", "Votrite Voting Ballot", MessageBoxButtons.OK, MessageBoxIcon.Error);

                    AppManager.Instance.ResetBallot();
                    return;
                }

                if (now > ballot.EndTime)
                {

                    MessageBox.Show("Ballot Closed, can not vote for this ballot.", "Votrite Voting Ballot", MessageBoxButtons.OK, MessageBoxIcon.Error);

                    AppManager.Instance.ResetBallot();
                    return;
                }



                AppManager.Instance.OpenBallot(ballot, true);
                this.Close();
            }
            catch
            {

            }
        }

        private void labelHelp_Click(object sender, EventArgs e)
        {
            try
            {
                PollworkerCred frmPollcred = new PollworkerCred();
                frmPollcred.ShowDialog();
                if (!frmPollcred.ok)
                    return;
                //if (adminaccess())
                //{
                    lblMsg.Invoke(new MethodInvoker(delegate
                   {
                       lblMsg.Text = "Opening file ...";
                       lblMsg.Refresh();
                   }));
                    System.Diagnostics.Process.Start(AppManager.Configuration["HelpFile"]["Path"]);
                    lblMsg.Invoke(new MethodInvoker(delegate
                    {
                        lblMsg.Text = "";
                        lblMsg.Refresh();
                    }));
                //}
            }
            catch (Exception ex)
            {
                MessageBox.Show("Unable to open help file. Error: "+ex.Message);
            }
        }

        private void label1_Click(object sender, EventArgs e)
        {
            try
            {
                SupervisorCred frmPollcred = new SupervisorCred();
                frmPollcred.ShowDialog();
                if (!frmPollcred.ok)
                    return;
                //if (adminaccess())
                //{
                lblMsg.Invoke(new MethodInvoker(delegate
                {
                    lblMsg.Text = "Opening file ...";
                    lblMsg.Refresh();
                }));
                System.Diagnostics.Process.Start(AppManager.Configuration["HelpFile"]["SupervisorHelpFile"]);
                lblMsg.Invoke(new MethodInvoker(delegate
                {
                    lblMsg.Text = "";
                    lblMsg.Refresh();
                }));
                //}
            }
            catch (Exception ex)
            {
                MessageBox.Show("Unable to open help file. Error: " + ex.Message);
            }
        }

        private void label2_Click(object sender, EventArgs e)
        {
            if(adminaccess())
            {
                supervisorpassreset frm = new supervisorpassreset();
                frm.ShowDialog();
            }
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            try
            {
                lblTime.Text = DateTime.Now.ToString("MM/dd/yyyy HH:mm:ss");
            }
            catch (Exception)
            {

            }
            //lblTime.Invoke(new MethodInvoker(delegate {
            //    lblTime.Text = DateTime.Now.ToString("MM/dd/yyyy HH:mm:ss");
            //}));
        }

        private void MenuForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            try
            {
                Taskbar.Show();
            }
            catch (Exception)
            {
            }
            try
            {
                HiddenFolder(false);
            }
            catch
            { }
        }

        private void timer2_Tick(object sender, EventArgs e)
        {
            timer2.Stop();
            try
            {
                AppManager.Configuration = IOHandler.DecryptConfig();
                this.Invoke(new MethodInvoker(delegate
                {
                    lblMachine.Text = "MACHINE: " + AppManager.Configuration["System"]["Machine"];
                    lblBackup.Text = "BACKUP FILE: " + AppManager.Configuration["System"]["BackupFlashDrivePath"];
                }));
                AppManager.Instance.OpenSession();
                Logger.Instance.Write("Session complete");
                AppManager.Instance.Session.SetLocale(VotRiteBallotDataManager.AppCode.Locale.DefaultLocale);
            }
            catch (Exception)
            {
            }
            
        }
    }
}
