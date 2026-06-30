using System;
using System.Windows.Forms;

using VotRite.Util;

namespace VotRite.JimForms
{
    public partial class F_SpeechRate : Form
    {
        public F_SpeechRate()
        {
            InitializeComponent();

            string str = AppManager.Configuration["Speech"]["Rate"];

            if (str != "")
            {
                hsbarSpeechRate.Value = int.Parse(str);
                lblRate.Text = hsbarSpeechRate.Value.ToString();
            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            IOHandler.SaveConfig("Speech", "Rate", this.hsbarSpeechRate.Value.ToString());
            MessageBox.Show("Save Successfully!", "VotRite Warn", MessageBoxButtons.OK, MessageBoxIcon.Information);
            this.Close();
        }

        private void hsbarSpeechRate_Scroll(object sender, ScrollEventArgs e)
        {
            lblRate.Text = hsbarSpeechRate.Value.ToString();
        }
    }
}
