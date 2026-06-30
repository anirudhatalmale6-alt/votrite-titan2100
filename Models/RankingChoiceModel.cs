using System.Collections;
using System.Collections.Generic;
using VotRite.Definition;
using VotRiteBallotDataManager.AppCode;
using VotRite.Views;
using VotRite.MVC;

namespace VotRite.Models
{
	//class RankingChoiceModel : ScreenModel
	//{
	//    private Hashtable vars;
	//    private Session session;
	//    private BallotDefinition ballot;
	//    private ContestDefinition contest;
	//    private Race currentRaceInstance;
	//    private Proposition currentPropositionInstance;

	//    public Hashtable Vars
	//    {
	//        get { return vars; }
	//    }

	//    public BallotDefinition Ballot { get { return ballot; } }

	//    public Session Session { get { return session; } }

	//    public ContestDefinition Contest { get { return contest; } }

	//    public Proposition Proposition
	//    {
	//        get { return currentPropositionInstance; }
	//    }

	//    public RankingChoiceModel()
	//    {
	//        vars = new Hashtable();
	//        session = AppManager.Instance.Session;
	//        ballot = session.Ballot;
	//        contest = ballot.ContestsList[ballot.ActiveContest];

	//        if (contest.Data.Count == 0)
	//        {
	//            ContestDefinition.SetContestFields(ref contest, ref currentRaceInstance, ref currentPropositionInstance, session.CurrentLocale);
	//            ContestDefinition.FillContestData(ref contest, currentPropositionInstance, session.CurrentLocale);
	//        }

	//        Initialize();
	//    }

	//    protected override void Dispose(bool disposing)
	//    {
	//        if (disposing)
	//        {
	//            vars.Clear();
	//            session.Dispose();
	//            ballot.Dispose();
	//            contest.Dispose();
	//            if (currentRaceInstance != null) currentRaceInstance.Dispose();
	//            if (currentPropositionInstance != null) currentPropositionInstance.Dispose();
	//        }
	//        base.Dispose(disposing);
	//    }

	//    private void Initialize()
	//    {
	//        vars.Add("review", session.CurrentLocale.BtnReview);
	//        vars.Add("back", session.CurrentLocale.BtnBack);
	//        vars.Add("next", session.CurrentLocale.BtnNext);
	//        vars.Add("skip", session.CurrentLocale.BtnSkip);
	//        vars.Add("more", session.CurrentLocale.BtnMoreCandidates);
	//        vars.Add("ballot_board", ballot.Board);
	//        vars.Add("ballot_name", ballot.Name);
	//        vars.Add("ballot_title", ballot.Title);
	//        vars.Add("ballot_address", ballot.Address);
	//        vars.Add("ballot_date", ballot.Date);
	//        vars.Add("group_name", contest.GroupName);
	//        vars.Add("contest_name", contest.Name);
	//        vars.Add("slate_tip", session.CurrentLocale.SlateTip);
	//        vars.Add("contest_title", contest.Title);
	//        vars.Add("min_race_tip", session.CurrentLocale.MinRaceTip);
	//        vars.Add("lbl_multi_0_choices2", session.CurrentLocale.LblMulti0Choices2);
	//        vars.Add("lbl_choice1", session.CurrentLocale.LblChoice1);
	//        vars.Add("lbl_choice2", session.CurrentLocale.LblChoice2);
	//        vars.Add("contest_tip", contest.Tip);
	//        vars.Add("contest_count", session.CurrentLocale.LblContestCount);
	//        vars.Add("contest_footnote", contest.Footnote);
	//        vars.Add("btn_add_candidates", session.CurrentLocale.BtnAddCandidates);

	//        int i = 1;

	//        foreach (DataDefinition data in contest.Data)
	//        {
	//            vars.Add("contest_data_" + i, data.Text);
	//            i++;
	//        }
	//    }

	//    public RankingChoiceView GetView()
	//    {
	//        RankingChoiceView v = null;

	//        foreach (IObserver obs in observers)
	//        {
	//            if (obs.GetType().ToString() == "VotRite.Views.RankingChoiceView")
	//            {
	//                v = (RankingChoiceView)obs;
	//                break;
	//            }
	//        }

	//        return v;
	//    }
	//}


	class RankingChoiceModel : ScreenModel
	{
		private Hashtable vars;

		private VotRite.Session session;

		private BallotDefinition ballot;

		private ContestDefinition contest;

		private Race currentRaceInstance;

		private VotRiteBallotDataManager.AppCode.Proposition currentPropositionInstance;

		public BallotDefinition Ballot
		{
			get
			{
				return this.ballot;
			}
		}

		public ContestDefinition Contest
		{
			get
			{
				return this.contest;
			}
		}

		public VotRiteBallotDataManager.AppCode.Proposition Proposition
		{
			get
			{
				return this.currentPropositionInstance;
			}
		}

		public VotRite.Session Session
		{
			get
			{
				return this.session;
			}
		}

		public Hashtable Vars
		{
			get
			{
				return this.vars;
			}
		}

		public RankingChoiceModel()
		{
			this.vars = new Hashtable();
			this.session = AppManager.Instance.Session;
			this.ballot = this.session.Ballot;
			this.contest = this.ballot.ContestsList[this.ballot.ActiveContest];
			if (this.contest.Data.Count == 0)
			{
				int? nullable = null;
				ContestDefinition.SetContestFields(ref this.contest, ref this.currentRaceInstance, ref this.currentPropositionInstance, this.session.CurrentLocale, nullable, false);
				ContestDefinition.FillContestData(ref this.contest, this.currentPropositionInstance, this.session.CurrentLocale);
			}
			this.Initialize();
		}

		protected override void Dispose(bool disposing)
		{
			if (disposing)
			{
				this.vars.Clear();
				this.session.Dispose();
				this.ballot.Dispose();
				this.contest.Dispose();
				if (this.currentRaceInstance != null)
				{
					this.currentRaceInstance.Dispose();
				}
				if (this.currentPropositionInstance != null)
				{
					this.currentPropositionInstance.Dispose();
				}
			}
			base.Dispose(disposing);
		}

		public RankingChoiceView GetView()
		{
			RankingChoiceView rankingChoiceView = null;
			foreach (IObserver observer in this.observers)
			{
				if (observer.GetType().ToString() == "VotRite.Views.RankingChoiceView")
				{
					rankingChoiceView = (RankingChoiceView)observer;
					break;
				}
			}
			return rankingChoiceView;
		}

		private void Initialize()
		{
			this.vars.Add("review", this.session.CurrentLocale.BtnReview);
			this.vars.Add("back", this.session.CurrentLocale.BtnBack);
			this.vars.Add("next", this.session.CurrentLocale.BtnNext);
			this.vars.Add("skip", this.session.CurrentLocale.BtnSkip);
			this.vars.Add("more", this.session.CurrentLocale.BtnMoreCandidates);
			this.vars.Add("ballot_board", this.ballot.Board);
			this.vars.Add("ballot_name", this.ballot.Name);
			this.vars.Add("ballot_title", this.ballot.Title);
			this.vars.Add("ballot_address", this.ballot.Address);
			this.vars.Add("ballot_date", this.ballot.Date);
			this.vars.Add("group_name", this.contest.GroupName);
			this.vars.Add("contest_name", this.contest.Name);
			this.vars.Add("slate_tip", this.session.CurrentLocale.SlateTip);
			this.vars.Add("contest_title", this.contest.Title);
			this.vars.Add("min_race_tip", this.session.CurrentLocale.MinRaceTip);
			this.vars.Add("lbl_multi_0_choices2", this.session.CurrentLocale.LblMulti0Choices2);
			this.vars.Add("lbl_choice1", this.session.CurrentLocale.LblChoice1);
			this.vars.Add("lbl_choice2", this.session.CurrentLocale.LblChoice2);
			this.vars.Add("contest_tip", this.contest.Tip);
			this.vars.Add("contest_count", this.session.CurrentLocale.LblContestCount);
			this.vars.Add("contest_footnote", this.contest.Footnote);
			this.vars.Add("btn_add_candidates", this.session.CurrentLocale.BtnAddCandidates);
			int num = 1;
			foreach (DataDefinition datum in this.contest.Data)
			{
				this.vars.Add(string.Concat("contest_data_", num), datum.Text);
				num++;
			}
		}
	}

}
