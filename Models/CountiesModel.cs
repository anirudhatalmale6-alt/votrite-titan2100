using System.Collections;
using VotRite.Definition;
using VotRiteBallotDataManager.AppCode;
using VotRite.Views;
using VotRite.MVC;

namespace VotRite.Models
{
    class CountiesModel : ScreenModel
    {
        private Hashtable vars;
        private Session session;
        private BallotDefinition ballot;
        private ContestDefinition contest;
        private Proposition currentPropositionInstance = new Proposition();

        public Hashtable Vars
        {
            get { return vars; }
        }

        public BallotDefinition Ballot { get { return ballot; } }

        public Session Session { get { return session; } }

        public ContestDefinition Contest { get { return contest; } }

        public Proposition Proposition
        {
            get { return currentPropositionInstance; }
        }

        public CountiesModel()
        {
            vars = new Hashtable();
            session = AppManager.Instance.Session;
            ballot = session.Ballot;
            contest = ballot.ContestsList[ballot.ActiveContest];

            Initialize();
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                vars.Clear();
                session.Dispose();
                ballot.Dispose();
                if (contest != null) contest.Dispose();
                if (currentPropositionInstance != null) currentPropositionInstance.Dispose();
            }
            base.Dispose(disposing);
        }

        private void Initialize()
        {
            vars.Add("review", session.CurrentLocale.BtnReview);
            vars.Add("back", session.CurrentLocale.BtnBack);
            vars.Add("next", session.CurrentLocale.BtnNext);
            vars.Add("skip", session.CurrentLocale.BtnSkip);
            vars.Add("more", session.CurrentLocale.BtnMoreCandidates);
            vars.Add("ballot_board", ballot.Board);
            vars.Add("ballot_name", ballot.Name);
            vars.Add("ballot_title", ballot.Title);
            //vars.Add("ballot_title", AppManager.Instance.GetVar("$b" + ballot.Id + "_title"));
            vars.Add("ballot_address", ballot.Address);
            vars.Add("ballot_date", ballot.Date);
            vars.Add("group_name", contest.GroupName);
            vars.Add("contest_name", contest.Name);

            vars.Add("contest_title", contest.Title);
            vars.Add("min_race_tip", session.CurrentLocale.MinRaceTip);
            vars.Add("lbl_multi_0_choices2", session.CurrentLocale.LblMulti0Choices2);
            vars.Add("lbl_choice1", session.CurrentLocale.LblChoice1);
            vars.Add("lbl_choice2", session.CurrentLocale.LblChoice2);
            vars.Add("county_tip", session.CurrentLocale.CountyTip);
            vars.Add("contest_count", session.CurrentLocale.LblContestCount);
            vars.Add("contest_footnote", contest.Footnote);
            vars.Add("btn_add_candidates", session.CurrentLocale.BtnAddCandidates);
            vars.Add("btn_setting_tools", session.CurrentLocale.BtnSettingTools);
            vars.Add("btn_callhelp", session.CurrentLocale.BtnCallHelp);

            //int i = 1;
            //foreach (DataDefinition data in contest.Data)
            //{
            //    vars.Add("contest_data_" + i, data.Text);
            //    i++;
            //}

            //session.ballot.Board = (string)vars["ballot_board"];
            //session.ballot.Name = (string)vars["ballot_name"];// +"\n" + (string)vars["ballot_title"];
            //session.ballot.Address = (string)vars["ballot_address"];
        }

        public CountiesView GetView()
        {
            CountiesView v = null;

            foreach (IObserver obs in observers)
            {
                if (obs.GetType().ToString() == "VotRite.Views.CountiesView")
                {
                    v = (CountiesView)obs;
                    break;
                }
            }

            return v;
        }
    }
}
