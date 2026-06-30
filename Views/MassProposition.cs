using System.Collections.Generic;

namespace VotRiteBallotDataManager.AppCode
{
    class MassProposition : Contest
    {        
        public List<Proposition> Propositions { get; set; }

        public void AddProposition(Proposition proposition)
        {
            if (Propositions == null)
                Propositions = new List<Proposition>();
            Propositions.Add(proposition);
        }

        public MassProposition()
        {
        }
        
        public MassProposition(Contest contest)
        {
            Id = contest.Id;
		    Type = contest.Type;
		    GenericName = contest.GenericName;
		    MinVotes = contest.MinVotes;
		    MaxVotes = contest.MaxVotes;
        }

        public bool HasPropositions()
        {
            return Propositions != null && Propositions.Count > 0;
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (Propositions != null)
                {
                    Propositions.ForEach(propos => propos.Dispose());
                    Propositions.Clear();
                }
            }
            base.Dispose(disposing);
        }
    }
   
}
