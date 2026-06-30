using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using VotRite.DBClasses;
using VotRite.Definition;
using VotRite.UI;
using VotRiteBallotDataManager.AppCode;

namespace VotRite.JimForms
{
    public partial class ReviewScreenSelections : Form
    {
        public ReviewScreenSelections()
        {
            InitializeComponent();
        }

        private void ReviewScreenSelections_Load(object sender, EventArgs e)
        {
            try
            {


                List<ContestDefinition> fitOrderContestList = AppManager.Instance.reviewSelectionList;
                Slate slate = null;
                int top = 5;
                int group = 0;
                int num = 0;
                int num1 = 5;
                bool flag1 = true;
                bool flag;
                Func<DataDefinition, bool> func;
                Func<DataDefinition, bool> func1 = null;
                Func<DataDefinition, bool> func2 = null;

                if (AppManager.Instance.ballot.slatesDefinition.Slates.Count > 0)
                {
                    foreach (ContestDefinition contestDefinition in fitOrderContestList)
                    {
                        if (contestDefinition.Selected > 0)
                        {
                            if (contestDefinition.Id == AppManager.Instance.ballot.slatesDefinition.Data.Id)
                            {
                                foreach (Slate slate1 in AppManager.Instance.ballot.slatesDefinition.Slates)
                                {
                                    if (slate1.Id == AppManager.Instance.ballot.slatesDefinition.Data.SlateId)
                                    {
                                        slate = slate1;
                                        break;
                                    }
                                }
                            }
                        }
                    }
                    if (slate != null)
                    {

                        Label vrLabel1 = getVrLabel(30, top, AppManager.Instance.reviewHeaderFontSize, true, "Slates Voting");
                        top += vrLabel1.Height + 5;

                        this.panel1.Controls.Add(vrLabel1);


                        Label vrSelection = getVrSelection(60, top, AppManager.Instance.reviewResultFontSize, true, slate.Name);
                        top += vrLabel1.Height;

                        this.panel1.Controls.Add(vrLabel1);
                        top += vrSelection.Height;
                        if (top > this.panel1.Top + this.panel1.Height)
                        {
                            vrSelection.Visible = false;
                        }
                        if (!this.panel1.Controls.Contains(vrSelection))
                        {
                            this.panel1.Controls.Add(vrSelection);
                        }
                        top += 5;
                    }
                }
                foreach (ContestDefinition contestDefinition1 in fitOrderContestList)
                {

                    if ((contestDefinition1.Type != ContestTypes.R ? true : slate == null))
                    {
                        if (group == contestDefinition1.Group)
                        {
                            flag = false;
                        }
                        else
                        {
                            flag = (contestDefinition1.Propositions == null ? true : flag1);
                        }
                        if (flag)
                        {
                            string Text = (string)AppManager.Instance.reviewModel.Vars[string.Concat(new object[] { "$b", AppManager.Instance.reviewModel.Ballot.Id, "_g", contestDefinition1.Group, "_name" })];

                            Label vrLabel = getVrLabel(30, top, AppManager.Instance.reviewHeaderFontSize, true, Text);
                            if (!this.panel1.Controls.Contains(vrLabel))
                            {
                                this.panel1.Controls.Add(vrLabel);
                            }
                            top += vrLabel.Height;
                        }
                        top += 15;
                        if (contestDefinition1.Propositions != null)
                        {
                            flag1 = false;
                            int num2 = 0;
                            foreach (Proposition proposition in contestDefinition1.Propositions)
                            {

                                Label vrLabel = getVrLabel(30, top, AppManager.Instance.reviewResultFontSize, true, proposition.Title);
                                top += vrLabel.Height;

                                if (!this.panel1.Controls.Contains(vrLabel))
                                {
                                    this.panel1.Controls.Add(vrLabel);
                                }
                                top += 5;
                                if ((!contestDefinition1.GroupSelection.ContainsKey(num2) ? false : contestDefinition1.GroupSelection[num2] != 0))
                                {
                                    int num3 = 0;
                                    int num4 = top;
                                    List<DataDefinition> dataDefinitions = contestDefinition1.Data;
                                    Func<DataDefinition, bool> func3 = func2;
                                    if (func3 == null)
                                    {
                                        Func<DataDefinition, bool> func4 = (DataDefinition data) => num3 != 10;
                                        func = func4;
                                        func2 = func4;
                                        func3 = func;
                                    }
                                    IEnumerable<DataDefinition> dataDefinitions1 = dataDefinitions.TakeWhile<DataDefinition>(func3);
                                    Func<DataDefinition, bool> func5 = func1;
                                    if (func5 == null)
                                    {
                                        Func<DataDefinition, bool> func6 = (DataDefinition data) => (data.State != VrSelection.SelectionState.SELECTED || contestDefinition1.Id + num2 * 7 >= data.Id ? false : data.Id < contestDefinition1.Id + (num2 + 1) * 7);
                                        func = func6;
                                        func1 = func6;
                                        func5 = func;
                                    }
                                    foreach (DataDefinition dataDefinition in dataDefinitions1.Where<DataDefinition>(func5))
                                    {
                                        Label vrSelection = getVrSelection(60, top, AppManager.Instance.reviewResultFontSize, true,
                                            (string)AppManager.Instance.reviewModel.Vars[string.Concat(new object[] { "$b", AppManager.Instance.reviewModel.Ballot.Id, "_c", contestDefinition1.Id, "_g", contestDefinition1.Group, "_d", dataDefinition.Id })]);
                                        if (num3 == 5)
                                        {
                                            top = num4;
                                        }
                                        vrSelection.Top = top;
                                        top += vrSelection.Height;

                                        if (!this.panel1.Controls.Contains(vrSelection))
                                        {
                                            this.panel1.Controls.Add(vrSelection);
                                        }
                                        num3++;
                                        top += 5;
                                    }
                                }
                                else
                                {
                                    Label vrSelection = getVrSelection(60, top, AppManager.Instance.reviewResultFontSize, true, (string)AppManager.Instance.reviewModel.Vars["no_selection"]);

                                    top += vrSelection.Height;

                                    if (!this.panel1.Controls.Contains(vrSelection))
                                    {
                                        this.panel1.Controls.Add(vrSelection);
                                    }
                                    top += 20;
                                }
                                num2++;
                            }
                        }
                        else
                        {
                            flag1 = true;

                            Label vrLabel = getVrLabel(30, top, AppManager.Instance.reviewResultFontSize, true,
                                (string)AppManager.Instance.reviewModel.Vars[string.Concat(new object[] { "$b", AppManager.Instance.reviewModel.Ballot.Id, "_c", contestDefinition1.Id, "_name" })]);
                            top += vrLabel.Height;

                            if (!this.panel1.Controls.Contains(vrLabel))
                            {
                                this.panel1.Controls.Add(vrLabel);
                            }
                            top += 5;
                            if (contestDefinition1.Selected != 0)
                            {
                                int num5 = 0;
                                int num6 = top;
                                int selected = contestDefinition1.Data.Count<DataDefinition>((DataDefinition it) => it.State == VrSelection.SelectionState.SELECTED);
                                if ((AppManager.Instance.reviewModel.Ballot.ElectionType == ElectionTypes.ranking_choice ? false : contestDefinition1.Type != ContestTypes.V))
                                {
                                    foreach (DataDefinition dataDefinition1 in
                                        from it in contestDefinition1.Data
                                        where it.State == VrSelection.SelectionState.SELECTED
                                        select it)
                                    {
                                        if (num5 == 20)
                                        {
                                            break;
                                        }
                                        else if (dataDefinition1.State == VrSelection.SelectionState.SELECTED)
                                        {
                                            Label vrSelection = getVrSelection(60, top, AppManager.Instance.reviewResultFontSize, true,
                                                (string)AppManager.Instance.reviewModel.Vars[string.Concat(new object[] { "$b", AppManager.Instance.reviewModel.Ballot.Id, "_c", contestDefinition1.Id, "_g", contestDefinition1.Group, "_d", dataDefinition1.Id })]);
                                            top += vrSelection.Height;

                                            if (!this.panel1.Controls.Contains(vrSelection))
                                            {
                                                this.panel1.Controls.Add(vrSelection);
                                            }
                                            num5++;
                                            top += 5;
                                        }
                                    }
                                }
                                else
                                {

                                    Label vrLabel2 = getVrLabel(30, top, AppManager.Instance.reviewResultFontSize, true, "Preference");

                                    top += vrLabel2.Height;
                                    if (!this.panel1.Controls.Contains(vrLabel2))
                                    {
                                        this.panel1.Controls.Add(vrLabel2);
                                    }
                                    selected = contestDefinition1.Selected;
                                    DataDefinition dataDefinition2 = null;
                                    foreach (DataDefinition datum in contestDefinition1.Data)
                                    {
                                        if ((!datum.WriteIn ? false : !datum.IsWritten))
                                        {
                                            dataDefinition2 = datum;
                                        }
                                    }
                                    if (dataDefinition2 != null)
                                    {
                                        contestDefinition1.Data.Remove(dataDefinition2);
                                    }
                                    foreach (DataDefinition dataDefinition3 in
                                        from i in contestDefinition1.Data
                                        where i.Preference > 0
                                        select i)
                                    {
                                        Label vrLabel3 = getVrLabel(60, top, AppManager.Instance.reviewResultFontSize, true,
                                             (string)AppManager.Instance.reviewModel.Vars[string.Concat(new object[] { "$b", AppManager.Instance.reviewModel.Ballot.Id, "_c", contestDefinition1.Id, "_g", contestDefinition1.Group, "_d", dataDefinition3.Id })]);
                                        top += vrLabel3.Height;

                                        if (!this.panel1.Controls.Contains(vrLabel3))
                                        {
                                            this.panel1.Controls.Add(vrLabel3);
                                        }

                                        Label vrLabel4 = getVrLabel(60, top, AppManager.Instance.reviewResultFontSize, true, dataDefinition3.Preference.ToString());
                                        top += vrLabel4.Height;

                                        if (!this.panel1.Controls.Contains(vrLabel4))
                                        {
                                            this.panel1.Controls.Add(vrLabel4);
                                        }
                                        num5++;

                                    }
                                }
                            }
                            else
                            {
                                Label vrSelection = getVrSelection(60, top, AppManager.Instance.reviewResultFontSize, true, (string)AppManager.Instance.reviewModel.Vars["no_selection"]);
                                top += vrSelection.Height;
                                if (!this.panel1.Controls.Contains(vrSelection))
                                {
                                    this.panel1.Controls.Add(vrSelection);
                                }
                                top += 5;
                            }
                        }
                        group = contestDefinition1.Group;
                        num++;
                        if ((num == AppManager.Instance.reviewModel.Ballot.ContestsList.Count ? false : contestDefinition1.Propositions == null))
                        {
                            top += 20;
                        }
                    }
                    else
                    {
                        num++;
                    }
                }
            }
            catch (Exception)
            {

            }
            this.Width = Screen.PrimaryScreen.WorkingArea.Width - 25;
            this.Height = Screen.PrimaryScreen.WorkingArea.Height - 25;
        }

        private Label getVrLabel(int height, int top, float fontsize, bool bold, string text)
        {
            Label lbl = new Label();
            Font SmallFont = new Font("Arial", fontsize, FontStyle.Bold);
            if(!bold)
                SmallFont = new Font("Arial", fontsize);
            lbl.Top = top;
            lbl.Height = height;
            lbl.Width = panel1.Width;
            lbl.Font = SmallFont;
            lbl.Text = text;
            return lbl;
        }


        private Label getVrSelection(int height, int top, float fontsize, bool bold, string text)
        {
            Label lbl = new Label();
            Font SmallFont = new Font("Arial", fontsize, FontStyle.Bold);
            if (!bold)
                SmallFont = new Font("Arial", fontsize);
            lbl.Top = top;
            lbl.Height = height;
            lbl.Width = panel1.Width;
            lbl.Font = SmallFont;
            lbl.BackColor = Color.SteelBlue;
            lbl.ForeColor = Color.White;
            lbl.Text = text;
            lbl.BorderStyle = BorderStyle.FixedSingle;
            lbl.FlatStyle = FlatStyle.Popup;
            return lbl;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
