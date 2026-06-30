using System;
using System.ComponentModel;
using System.Drawing;
using System.Threading;
using System.Windows.Forms;
using VotRiteBallotDataManager.AppCode;

namespace VotRite.Forms
{
    public class IdentifyTenantForm : Form
    {
        // Methods
        public IdentifyTenantForm()
        {
            InitializeComponent();
            base.FormBorderStyle = FormBorderStyle.None;
            FormHelper.HideTray();
        }

        public Tenant SelectedTenant { get; set; }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
        }

        private void IdentifyTenantForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            FormHelper.ShowTray();
            AppManager.Instance.CloseOskProcess();
        }

        private void IdentifyTenantForm_Load(object sender, EventArgs e)
        {
            AppManager.Instance.RunOsk();
        }

        private void IdentifyTenantForm_VisibleChanged(object sender, EventArgs e)
        {
            FormHelper.SetWinFullScreen(base.Handle);
            FormHelper.ShowScreenKeyboard();
            SetScreenKeyboardLocationWithDelay();
        }

        private void InitializeComponent()
        {
            base.AutoScaleDimensions = new SizeF(6f, 13f);
            base.AutoScaleMode = AutoScaleMode.Font;
            base.ClientSize = new Size(0x373, 0x25f);
            base.MaximizeBox = false;
            base.MinimizeBox = false;
            base.Name = "IdentifyTenantForm";
            base.ShowIcon = false;
            base.ShowInTaskbar = false;
            Text = "Tenants";
            base.TopMost = true;
            base.FormClosed += IdentifyTenantForm_FormClosed;
            base.Load += IdentifyTenantForm_Load;
            base.VisibleChanged += IdentifyTenantForm_VisibleChanged;
            base.ResumeLayout(false);
        }

        private void SetScreenKeyboardLocationWithDelay()
        {
            var worker = new BackgroundWorker();
            worker.DoWork += delegate
                                 {
                                     Thread.Sleep(TimeSpan.FromSeconds(1.0));
                                     FormHelper.ShowScreenKeyboard();
                                 };
            worker.RunWorkerAsync();
        }
    }
}