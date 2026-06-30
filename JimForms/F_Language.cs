using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

using VotRiteBallotDataManager.AppCode;
using VotRite.Definition;

namespace VotRite.JimForms
{
    public partial class F_Language : Form
    {
        public F_Language()
        {
            InitializeComponent();

            FillBallotForm(AppManager.Instance.ballot);
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void FillBallotForm(BallotDefinition ballot)
        {
            List<Locale> locales = Locale.FetchLocales(ballot.Id);
            List<Locale> localesAll = Locale.FetchLocales(ballot.Id, true);
            
            var top = 20;
            var left = 10;
            var columnWidth = 0;
            int col_count = 0;

            grbxLocales.Controls.Clear();

            foreach (var locale in localesAll)
            {
                var chbLocale = new CheckBox();
                if (Locale.IsDefaultLocale(locale))
                {
                    chbLocale.Enabled = false;
                }
                chbLocale.Text = locale.Name;

                for (int k = 0; k < locales.Count; k++)
                {
                    if (locales[k].Name == locale.Name)
                    {
                        chbLocale.Checked = true;
                        break;
                    }
                }

                chbLocale.AutoSize = true;
                chbLocale.Padding = new Padding(0);
                columnWidth = Math.Max(columnWidth, chbLocale.PreferredSize.Width + 30);

                if (col_count >= Math.Ceiling((double)localesAll.Count / 3))
                {
                    top = 20;
                    left += columnWidth + 40;
                    columnWidth = chbLocale.PreferredSize.Width + 30;
                    col_count = 0;
                }

                chbLocale.Location = new Point(left, top);
                chbLocale.CheckedChanged += new EventHandler(chbLocale_CheckedChanged);
                chbLocale.Tag = locale;

                grbxLocales.Controls.Add(chbLocale);

                top += chbLocale.PreferredSize.Height;
                col_count++;
            }
        }

        void chbLocale_CheckedChanged(object sender, EventArgs e)
        {
            ActivateChangedStatus();
        }

        private void ActivateChangedStatus()
        {
            btnSave.Enabled = true;
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            var updated = true;
            var sbLocales = new StringBuilder();
            sbLocales.AppendFormat("delete from ballot_locale where ballot_id = {0};", AppManager.Instance.ballot.Id);
            var uncheckedLocales = new StringBuilder();

            foreach (Control child in grbxLocales.Controls)
            {
                var chbxLocale = child as CheckBox;
                if (chbxLocale != null)
                {
                    var locale = chbxLocale.Tag as Locale;
                    if (locale != null && chbxLocale.Checked)
                    {
                        sbLocales.AppendFormat("INSERT INTO ballot_locale (ballot_id, locale_id) values ({0}, {1});",
                                               AppManager.Instance.ballot.Id,
                                               locale.Id);
                        if (chbxLocale.Checked)
                        {
                            Candidate.CopyCandidatesFromDefaultLocale(locale.Id, AppManager.Instance.ballot.Id);
                            updated &= Race.CopyRacesFromDefaultLocale(locale.Id, AppManager.Instance.ballot.Id);
                            updated &= Proposition.CopyPropositionsFromDefaultLocale(locale.Id, AppManager.Instance.ballot.Id);
                        }
                    }
                    else if (locale != null)
                    {
                        uncheckedLocales.Append(locale.Id).Append(", ");
                    }
                }
            }
            uncheckedLocales = uncheckedLocales.Remove(uncheckedLocales.Length - 2, 2);
            sbLocales.AppendFormat("DELETE FROM candidate_name WHERE locale_id in ({0}) and cand_id in " +
                                   "(SELECT cand_id FROM candidates WHERE contest_id IN (SELECT contest_id FROM contest WHERE ballot_id = {1}));",
                                   uncheckedLocales, AppManager.Instance.ballot.Id);
            sbLocales.AppendFormat(
                "DELETE FROM proposition WHERE locale_id in ({0}) and contest_id IN (SELECT contest_id FROM contest WHERE ballot_id = {1});",
                uncheckedLocales, AppManager.Instance.ballot.Id);
            sbLocales.AppendFormat(
                "DELETE FROM race WHERE locale_id in ({0}) and contest_id IN (SELECT contest_id FROM contest WHERE ballot_id = {1});",
                uncheckedLocales, AppManager.Instance.ballot.Id);

            if (sbLocales.Length > 0)
                updated = updated && DataManager.VotingContentDataInstance.SetData(sbLocales.ToString());
            if (updated)
            {
                MessageBox.Show("Saved successfully,please restart voting system!", "Success", MessageBoxButtons.OK,
                                MessageBoxIcon.Information);

                DialogResult = System.Windows.Forms.DialogResult.OK;
            }
            else
            {
                DialogResult dlgResult = MessageBox.Show("saved failed!", "Error",
                                                         MessageBoxButtons.RetryCancel, MessageBoxIcon.Error);
            }
        }
    }
}
