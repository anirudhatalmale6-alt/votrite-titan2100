using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace VotRite.JimForms
{
    public partial class themes : Form
    {
        public themes()
        {
            InitializeComponent();
        }

        private void themes_Load(object sender, EventArgs e)
        {
            switch (AppManager.Instance.backgroundTheme)
            {
                case AppManager.colorTheme.White:
                    radioWhite.Checked = true;
                    break;
                case AppManager.colorTheme.Yellow:
                    radioYellow.Checked = true;
                    break;
                case AppManager.colorTheme.Blue:
                    radioBlue.Checked = true;
                    break;
                case AppManager.colorTheme.Green:
                    radioGreen.Checked = true;
                    break;
                case AppManager.colorTheme.Contrast:
                    radioBlack.Checked = true;
                    break;
                default:
                    break;
            }

            this.StartPosition = FormStartPosition.CenterScreen;
        }

        private void btnUpdate_Click(object sender, EventArgs e)
        {
            if (radioWhite.Checked == true)
                AppManager.Instance.backgroundTheme = AppManager.colorTheme.White;
            else if (radioYellow.Checked == true)
                AppManager.Instance.backgroundTheme = AppManager.colorTheme.Yellow;
            else if (radioGreen.Checked == true)
                AppManager.Instance.backgroundTheme = AppManager.colorTheme.Green;
            else if (radioBlue.Checked == true)
                AppManager.Instance.backgroundTheme = AppManager.colorTheme.Blue;
            else if (radioBlack.Checked == true)
                AppManager.Instance.backgroundTheme = AppManager.colorTheme.Contrast;
            else if (radioLightBlue.Checked == true)
                AppManager.Instance.backgroundTheme = AppManager.colorTheme.LightBlue;
            else if (radioLightYellow.Checked == true)
                AppManager.Instance.backgroundTheme = AppManager.colorTheme.LightYellow;

            this.Close();
        }
    }
}
