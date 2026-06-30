using System.Collections;
using VotRite.Definition;
using VotRiteBallotDataManager.AppCode;
using VotRite.Views;
using VotRite.MVC;

namespace VotRite.Models
{
    class ResultsModel : ScreenModel
    {
        private Hashtable vars;
        private Session session;
        private BallotDefinition ballot;
        private ContestDefinition contest;
        private Race currentRaceInstance;
        private Proposition currentPropositionInstance;

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

        public ResultsModel()
        {
            vars = new Hashtable();
            session = AppManager.Instance.Session;
            ballot = session.Ballot;
            contest = ballot.ContestsList[ballot.ActiveContest];

            ContestDefinition.SetContestFields(ref contest, ref currentRaceInstance, ref currentPropositionInstance, session.CurrentLocale);
            ContestDefinition.FillContestData(ref contest, currentPropositionInstance, session.CurrentLocale);

            Initialize();
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                vars.Clear();
                session.Dispose();
                ballot.Dispose();
                contest.Dispose();
                if (currentRaceInstance != null) currentRaceInstance.Dispose();
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
            vars.Add("ballot_address", ballot.Address);
            vars.Add("ballot_date", ballot.Date);
            vars.Add("group_name", contest.GroupName);
            vars.Add("contest_name", contest.Name);
            vars.Add("slate_tip", session.CurrentLocale.SlateTip);

            vars.Add("contest_title", contest.Title);
            vars.Add("min_race_tip", session.CurrentLocale.MinRaceTip);
            vars.Add("lbl_multi_0_choices2", session.CurrentLocale.LblMulti0Choices2);
            vars.Add("lbl_choice1", session.CurrentLocale.LblChoice1);
            vars.Add("lbl_choice2", session.CurrentLocale.LblChoice2);
            vars.Add("contest_tip", contest.Tip);
            vars.Add("contest_count", session.CurrentLocale.LblContestCount);
            vars.Add("contest_footnote", contest.Footnote);
            vars.Add("btn_add_candidates", session.CurrentLocale.BtnAddCandidates);

            int i = 1;

            foreach (DataDefinition data in contest.Data)
            {
                vars.Add("contest_data_" + i, data.Text);
                i++;
            }
        }

        public ResultsView GetView()
        {
            ResultsView v = null;

            foreach (IObserver obs in observers)
            {
                if (obs.GetType().ToString() == "VotRite.Views.ResultsView")
                {
                    v = (ResultsView)obs;
                    break;
                }
            }

            return v;
        }
    }
}
