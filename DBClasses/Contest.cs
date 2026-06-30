using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using VotRite;
using VotRite.Util;

namespace VotRiteBallotDataManager.AppCode
{
    public enum ContestTypes
    {
        R,
        P,
        M,
        V,
        All
    }

    public class Contest : IDisposable
    {
        public const string TableName = "contest";
        public const string IdField = "contest_id";
        public const string TypeField = "contest_type";
        public const string GenericNameField = "contest_generic_name";
        public const string MinVotesField = "race_min_votes";
        public const string MaxVotesField = "race_max_votes";
        public const string MaxWriteinsField = "race_max_writeins";
        public const string BallotIdField = "ballot_id";
        public const string CountyIdField = "county_id";
        private List<Candidate> candidatesList = new List<Candidate>();

        public int Id { get; set; }
        public ContestTypes Type { get; set; }
        public string GenericName { get; set; }
        public int MinVotes { get; set; }
        public int MaxVotes { get; set; }
        public int MaxWriteins { get; set; }
        public int BallotId { get; set; }
        public int CountyId { get; set; }

        public List<Candidate> CandidatesList
        {
            get { return candidatesList; }
            set { candidatesList = value; }
        }

        // Jim Kapsis.
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        // Jim Kapsis.
        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                candidatesList.ForEach(cand => cand.Dispose());
                candidatesList.Clear();
            }
        }

        public override string ToString()
        {
            return GenericName;
        }


        public static DataTable GetOrderPositionRec()
        {
            DataTable dtTmp = null;
            try
            {
                dtTmp = DataManager.VotingContentDataInstance.GetDataV2(string.Format("SELECT contest_id,rvo_order_position FROM {0} ",
                                                                                "race_view_options"
                                                                                ));
            }
            catch (Exception e) { Logger.Instance.Write(e); }

            return dtTmp;
        }

        public static List<Contest> GetContests(ContestTypes contestType, int ballotId, string orderByField, int partyId = 0, int countyId = 0)
        {
            var contests = new List<Contest>();

            try
            {
                short massPropositionPerPage = Convert.ToInt16(AppManager.Configuration["Contest"]["MassPropositionPerPage"]);
                string whereCondition = string.Format("WHERE {0} = {1}", BallotIdField, ballotId);

                if (contestType != ContestTypes.All)
                    whereCondition += string.Format(" AND {0}='{1}'", TypeField, contestType.ToString());

                if (partyId > 0)
                {
                    whereCondition += string.Format(@" AND ((exists (select * from candidates where contest.contest_id = candidates.contest_id 
                        and candidates.party_id = {0})) OR contest.contest_type='P' OR contest.contest_type='M')", partyId);
                }

                if (countyId > 0)
                {
                    whereCondition += string.Format(@" AND contest.county_id={0}", countyId);
                }

                DataTable dtContests =
                    DataManager.VotingContentDataInstance.GetDataV2(
                        string.Format("SELECT {1}, {2}, {3}, {4}, {7}, {8}, {9}, {10} FROM {0} {5} ORDER BY {6}",
                                      TableName,
                                      IdField,
                                      GenericNameField,
                                      TypeField,
                                      MaxVotesField,
                                      whereCondition,
                                      orderByField,
                                      MaxWriteinsField,
                                      BallotIdField,
                                      MinVotesField,
                                      CountyIdField));
                MassProposition massPropositions = null;

                var propositionsList = new List<MassProposition>();

                if (dtContests != null)
                {
                    if (dtContests.Rows.Count > 0)
                    {
                        foreach (DataRow dataRow in dtContests.Rows)
                        {
                            var contest = new Contest
                            {
                                Id = Helper.Cast(dataRow[IdField], -1),
                                GenericName = VotriteCrypto.Decrypt(Helper.GetStringValue(dataRow[GenericNameField])),
                                MinVotes = Helper.Cast(dataRow[MinVotesField], -1),
                                MaxVotes = Helper.Cast(dataRow[MaxVotesField], -1),
                                MaxWriteins = Helper.Cast(dataRow[MaxWriteinsField], 0),
                                BallotId = Helper.Cast(dataRow[BallotIdField], 1),
                                CountyId = Helper.Cast(dataRow[CountyIdField], 1)
                            };
                            string type = Helper.GetStringValue(dataRow[TypeField]);
                            //contest.Type = type == ContestTypes.R.ToString()
                            //                   ? ContestTypes.R
                            //                   : (type == ContestTypes.M.ToString() ? ContestTypes.M : ContestTypes.P);

                            string stringValue = Helper.GetStringValue(dataRow["contest_type"]);
                            if (stringValue == "P")
                            {
                                contest.Type = ContestTypes.P;
                            }
                            else if (stringValue == "M")
                            {
                                contest.Type = ContestTypes.M;
                            }
                            else if (stringValue == "V")
                            {
                                contest.Type = ContestTypes.V;
                            }
                            else
                            {
                                contest.Type = ContestTypes.R;
                            }

                            if (contest.Type == ContestTypes.M)
                            {
                                if (massPropositions == null)
                                {
                                    massPropositions = new MassProposition(contest);
                                }
                                else if (massPropositions.Propositions != null &&
                                         massPropositions.Propositions.Count >= massPropositionPerPage)
                                {
                                    propositionsList.Add(massPropositions);
                                    massPropositions = new MassProposition(contest);
                                }
                                massPropositions.AddProposition(new Proposition { ContestId = contest.Id });
                            }
                            else
                                contests.Add(contest);
                        }
                    }
                }

                if (massPropositions != null)
                    propositionsList.Add(massPropositions);

                if (propositionsList.Count > 0)
                {
                    contests.AddRange(propositionsList.Cast<Contest>());
                }
            }
            catch (Exception e)
            {
                Logger.Instance.Write(e);
            }

            return contests;
        }

        public static List<Contest> GetContests_prev(ContestTypes contestType, int ballotId, string orderByField, int partyId = 0, int countyId = 0)
        {
            List<Contest> contests;
            List<Contest> contests1 = new List<Contest>();
            try
            {
                short num = Convert.ToInt16(AppManager.Configuration["Contest"]["MassPropositionPerPage"]);
                string str = string.Format("WHERE {0} = {1}", "ballot_id", ballotId);
                if (contestType != ContestTypes.All)
                {
                    str = string.Concat(str, string.Format(" AND {0}='{1}'", "contest_type", contestType.ToString()));
                }
                if (countyId > 0)
                {
                    str = string.Concat(str, string.Format(" AND {0}={1}", "county_id", countyId));
                }
                if (partyId > 0)
                {
                    str = string.Concat(str, string.Format(" AND ((exists (select * from candidates where contest.contest_id = candidates.contest_id \r\n                        and candidates.party_id = {0})) OR contest.contest_type='P' OR contest.contest_type='M')", partyId));
                }
                DataTable dataV2 = DataManager.VotingContentDataInstance.GetDataV2(string.Format("SELECT {1}, {2}, {3}, {4}, {7}, {8}, {9}, {10} FROM {0} {5} ORDER BY {6}", new object[] { "contest", "contest_id", "contest_generic_name", "contest_type", "race_max_votes", str, orderByField, "race_max_writeins", "ballot_id", "race_min_votes", "county_id" }));
                MassProposition massProposition = null;
                List<MassProposition> massPropositions = new List<MassProposition>();
                if (dataV2 != null)
                {
                    if (dataV2.Rows.Count > 0)
                    {
                        foreach (DataRow row in dataV2.Rows)
                        {
                            Contest contest = new Contest()
                            {
                                Id = Helper.Cast(row["contest_id"], -1),
                                GenericName = VotriteCrypto.Decrypt(Helper.GetStringValue(row["contest_generic_name"])),
                                MinVotes = Helper.Cast(row["race_min_votes"], -1),
                                MaxVotes = Helper.Cast(row["race_max_votes"], -1),
                                MaxWriteins = Helper.Cast(row["race_max_writeins"], 0),
                                BallotId = Helper.Cast(row["ballot_id"], 1),
                                CountyId = Helper.Cast(row["county_id"], 1)
                            };
                            string stringValue = Helper.GetStringValue(row["contest_type"]);
                            if (stringValue == "P")
                            {
                                contest.Type = ContestTypes.P;
                            }
                            else if (stringValue == "M")
                            {
                                contest.Type = ContestTypes.M;
                            }
                            else if (stringValue == "V")
                            {
                                contest.Type = ContestTypes.V;
                            }
                            else
                            {
                                contest.Type = ContestTypes.R;
                            }
                            if (contest.Type != ContestTypes.M)
                            {
                                contests1.Add(contest);
                            }
                            else
                            {
                                if (massProposition == null)
                                {
                                    massProposition = new MassProposition(contest);
                                }
                                else if ((massProposition.Propositions == null ? false : massProposition.Propositions.Count >= num))
                                {
                                    massPropositions.Add(massProposition);
                                    massProposition = new MassProposition(contest);
                                }
                                massProposition.AddProposition(new Proposition()
                                {
                                    ContestId = contest.Id
                                });
                            }
                        }
                    }
                }
                if (massProposition != null)
                {
                    massPropositions.Add(massProposition);
                }
                if (massPropositions.Count > 0)
                {
                    contests1.AddRange(massPropositions.Cast<Contest>());
                }
                List<Contest> contests2 = new List<Contest>();
                DataTable data = DataManager.VotingContentDataInstance.GetData(string.Concat("select * from race_view_options where contest_id in (", string.Join<int>(",",
                    from c in contests1
                    select c.Id), ") order by rvo_order_position"));
                foreach (DataRow dataRow in data.Rows)
                {
                    contests2.Add(contests1.Single<Contest>((Contest c) => c.Id.ToString() == dataRow["contest_id"].ToString()));
                }
                contests = contests2;
                return contests;
            }
            catch (Exception exception)
            {
                Logger.Instance.Write(exception);
            }
            contests = contests1;
            return contests;
        }

        public static DataTable getContest(int contestId)
        {
            DataTable dtContests =
                    DataManager.VotingContentDataInstance.GetDataV2("Select * from " + TableName + " where " + IdField + "=" + contestId);
            foreach (DataRow dataRow in dtContests.Rows)
            {
                dataRow[GenericNameField] = VotriteCrypto.Decrypt(Helper.GetStringValue(dataRow[GenericNameField]));
            }

            return dtContests;
        }
    }
} 