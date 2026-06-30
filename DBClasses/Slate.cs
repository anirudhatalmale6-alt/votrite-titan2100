using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SqlClient;
using VotRiteBallotDataManager.AppCode;
using System.Data;
using VotRite.Util;
using System.Data.SQLite;

namespace VotRite.DBClasses
{
    public class CandidateDefinition : IDisposable
    {
        public int Id;
        public string Name;
        public string Position;

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
            }
        }
    }

    class Slate: IDisposable
    {
        private const string SlateTable = "slates";
        private const string SlateRelTable = "slates_candidates";
        private const string SlateIdField = "slate_id";
        private const string SlateIndexField = "slate_index";
        private const string NameField = "name";
        private const string BallotIdField = "ballot_id";
        private const string ContestIdField = "contest_id";
        private const string CandidateIdField = "candidate_id";
        private const string CandidateNameField = "cand_name";
        private const string CandidatePositionField = "loc_voted_position";
        private const string SlateLogoField = "slate_logo";

        public int Id { get; set; }
        public int Index { get; set; }
        public string Name { get; set; }
        public int BallotId { get; set; }
        public string SlateLogo { get; set; }
        public Dictionary<int, List<CandidateDefinition>> Candidates { get; set; }

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
                if (Candidates != null)
                {
                    foreach (List<CandidateDefinition> list in Candidates.Values)
                    {
                        list.ForEach(canDef => canDef.Dispose());
                        list.Clear();
                    }
                    Candidates.Clear();
                    Candidates = null;
                }
            }
        }

        public static List<Slate> GetSlates(int ballotId, int localeId, int? contestId=null)
        {
            var slates = new List<Slate>();
            try
            {
                var dtSlates = DataManager.VotingContentDataInstance.GetDataV2("SELECT * FROM slates where ballot_id = @ballotid",
                                                             new List<SQLiteParameter> { new SQLiteParameter("@ballotid", ballotId) });
                if (dtSlates != null)
                {
                    if (dtSlates.Rows.Count > 0)
                    {
                        slates.AddRange(from DataRow dataRow in dtSlates.Rows
                                        select new Slate
                                        {
                                            Id = Helper.Cast(dataRow[SlateIdField], -1),
                                            Index = Helper.Cast(dataRow[SlateIndexField], -1),
                                            Name = VotriteCrypto.Decrypt(Helper.GetStringValue(dataRow[NameField])),
                                            BallotId = Helper.Cast(dataRow[BallotIdField], -1),
                                            Candidates = new Dictionary<int, List<CandidateDefinition>>(),
                                            SlateLogo = VotriteCrypto.Decrypt(Helper.GetStringValue(dataRow[SlateLogoField]))
                                        });
                    }
                }
                if (slates.Count > 0)
                {
                    List<Race> races = Race.GetAllRaces(localeId, ballotId);
                    foreach (var sl in slates)
                    {
                        if (contestId != null)
                        {
                            sl.Candidates.Add((int)contestId, GetCandidates(sl.Id, (int)contestId, localeId));
                        } else
                        {
                            foreach (var race in races)
                            {
                                sl.Candidates.Add(race.ContestId, GetCandidates(sl.Id, race.ContestId, localeId));
                            }
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Logger.Instance.Write(e);
            }
            return slates;
        }

        public static List<CandidateDefinition> GetCandidates(int slateId, int contestId, int localeId)
        {
            var candidates = new List<CandidateDefinition>();
            try
            {
                var dtRaces = DataManager.VotingContentDataInstance.GetDataV2(string.Format(@"SELECT sc.candidate_id, cn.cand_name, r.loc_voted_position 
                                                                                                        FROM slates_candidates sc
                                                                                                        INNER JOIN candidate_name cn ON sc.candidate_id = cn.cand_id
                                                                                                        INNER JOIN race r ON r.contest_id = sc.contest_id
                                                                                                        WHERE sc.{0} = @slate_id and sc.{1} = @contest_id and cn.locale_id = @locale_id 
                                                                                                                and r.locale_id = @locale_id",
                    SlateIdField, ContestIdField),
                    new List<SQLiteParameter> { 
                        new SQLiteParameter("@slate_id", slateId), 
                        new SQLiteParameter("@contest_id", contestId), 
                        new SQLiteParameter("@locale_id", localeId)
                    });
                if (dtRaces != null)
                {
                    if (dtRaces.Rows.Count > 0)
                    {
                        candidates = (from DataRow dataRow in dtRaces.Rows
                                      select new CandidateDefinition
                                      {
                                          Id = Helper.Cast(dataRow[CandidateIdField], -1),
                                          Name = VotriteCrypto.Decrypt(Helper.GetStringValue(dataRow[CandidateNameField])),
                                          Position = VotriteCrypto.Decrypt(Helper.GetStringValue(dataRow[CandidatePositionField]))
                                      }).ToList();
                    }
                }
            }
            catch (Exception e)
            {
                Logger.Instance.Write(e);
            }

            return candidates;
        }

        public static int GetSlateId(int candidateId)
        {
            return Convert.ToInt32(DataManager.VotingContentDataInstance.GetScalarData("select slate_id from slates_candidates where candidate_id = " + candidateId + ";"));
        }
    }
}
