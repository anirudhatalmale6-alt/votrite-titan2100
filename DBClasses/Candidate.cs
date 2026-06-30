using System.Collections.Generic;
using System.Data;
using VotRite;
using System;
using VotRite.Util;
using System.Data.SQLite;

namespace VotRiteBallotDataManager.AppCode
{
	public enum IncumbentFlags {
		Y,
		N
	}

	public class Candidate : IDisposable
    {
		public const string TableName = "candidates";
		public const string IdField = "cand_id";
		public const string ContestIdField = "contest_id";
		public const string NameField = "cand_name";
		public const string IncumbentFlagField = "incumbent_flag";
		public const string CandidatePhotoField = "cand_photo";
		public const string PartyLogoField = "party_logo";
        public const string LocaleIdField = "locale_id";
        public const string PartyIdField = "party_id";
        public const string PhotoField = "cand_photo";
        public const string VoteableField = "voteable";
        public const string IsWrittenField = "is_written";

        public int Id { get; set; }
		public int ContestId { get; set; }
		public string Name { get; set; }
		public bool IncumbentFlag { get; set; }
		public string Photo { get; set; }
		public string PartyLogo { get; set; }
        public int? LocaleId { get; set; }
        public int PartyId { get; set; }
        public bool Voteable { get; set; }
        public bool IsWritten { get; set; }
        public int Preference { get; set; }

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

        public static List<Candidate> GetCandidates(int? contestId, int? localeId, int? ballotId)
        {
            var candidates = new List<Candidate>();
            try
            {
                var parameters = new List<SQLiteParameter>();
                var sql =
                    "SELECT c.cand_id, cand_name, incumbent_flag, cand_photo, cn.locale_id, c.contest_id, party_logo, c.party_id, c.voteable, c.is_written FROM candidates c " +
                    "left join candidate_name cn on c.cand_id = cn.cand_id ";
                if (localeId != null)
                {
                    sql += "and cn.locale_id = @localeid ";
                    parameters.Add(new SQLiteParameter("@localeid", localeId));
                }
                if (ballotId != null)
                {
                    sql += "join contest cont on cont.contest_id = c.contest_id and ballot_id = @ballotid ";
                    parameters.Add(new SQLiteParameter("@ballotid", ballotId));
                }
                if (contestId != null)
                {
                    sql += "WHERE c.contest_id = @contestid ";
                    parameters.Add(new SQLiteParameter("@contestid", contestId));
                }

                //sql += " AND c.is_written != 1 ";

                sql += " ORDER BY c.cand_id";

                var dtCandidates = DataManager.VotingContentDataInstance.GetDataV2(sql, parameters);

                if (dtCandidates != null)
                {
                    if (dtCandidates.Rows.Count > 0)
                    {
                        foreach (DataRow dataRow in dtCandidates.Rows)
                        {
                            var candidate = new Candidate();
                            candidate.Id = Helper.Cast(dataRow[Candidate.IdField], -1);
                            candidate.ContestId = Helper.Cast(dataRow[Candidate.ContestIdField], 0);
                            candidate.Name = VotriteCrypto.Decrypt( Helper.GetStringValue(dataRow[Candidate.NameField]));
                            candidate.IncumbentFlag = Helper.GetStringValue(dataRow[Candidate.IncumbentFlagField]) == IncumbentFlags.Y.ToString();
                            candidate.Photo = VotriteCrypto.Decrypt(Helper.GetStringValue(dataRow[Candidate.PhotoField]));
                            candidate.PartyLogo = VotriteCrypto.Decrypt(Helper.GetStringValue(dataRow[Candidate.PartyLogoField]));
                            var readLocaleId = dataRow[Candidate.LocaleIdField];
                            int vLocaleId;
                            if (Int32.TryParse(readLocaleId.ToString(), out vLocaleId))
                                candidate.LocaleId = vLocaleId;
                            else
                                candidate.LocaleId = null;
                            var readPartyId = dataRow[Candidate.PartyIdField];
                            int vPartyId;
                            if (Int32.TryParse(readPartyId.ToString(), out vPartyId))
                                candidate.PartyId = vPartyId;

                            candidate.Voteable = Helper.Cast(dataRow[Candidate.VoteableField], true);
                            candidate.IsWritten = Helper.Cast(dataRow[Candidate.IsWrittenField], false);

                            candidates.Add(candidate);
                        }
                    }
                }
            }
            catch (Exception e) { Logger.Instance.Write(e); }

            return candidates;
        }
        
        public static void CopyCandidatesFromDefaultLocale(int localeId, int? ballotId)
        {
            var candidates = GetCandidates(null, localeId, ballotId);
            var defaultCandidates = GetCandidates(null, Locale.DefaultLocale.Id, ballotId);
           
            foreach (var candidate in candidates)
            {
                if (candidate.LocaleId == null)
                {
                    var idx = defaultCandidates.FindIndex(c => c.Id == candidate.Id);
                    Candidate defaultCandidate = null;
                    if (idx > -1)
                        defaultCandidate = defaultCandidates[idx];
                    if (defaultCandidate != null && defaultCandidate.LocaleId != null)
                    {
                        var sql = string.Format(@"INSERT INTO candidate_name (cand_id, locale_id, cand_name) 
                                                            VALUES (@candidate_id, @locale_id, @candidate_name);",
                                                   defaultCandidate.Id, localeId,
                                                   Helper.EscapeStringData(defaultCandidate.Name));
                        DataManager.VotingContentDataInstance.SetData(sql, new List<SQLiteParameter> { 
                            new SQLiteParameter("@candidate_id", defaultCandidate.Id),
                            new SQLiteParameter("@locale_id", localeId), 
                            new SQLiteParameter("@candidate_name", Helper.EscapeStringData(defaultCandidate.Name))});
                    }
                }
            }
        }

        public static List<Candidate> GetCandidates(int contestId, int localeId, int? partyId = null)
        {
			List<Candidate> _candidates = new List<Candidate>();

			try
			{
                var prms = new List<SQLiteParameter>();
                var query = "SELECT c.cand_id, cand_name, incumbent_flag, cand_photo, cn.locale_id, party_logo, c.party_id, c.voteable, c.is_written FROM candidates c " +
                    "left join candidate_name cn on c.cand_id = cn.cand_id and cn.locale_id = @localeId " +
                    "WHERE c.contest_id = @contestId ";

                if (partyId > 0)
                {
                    query += " AND c.party_id = @partyId ";
                    prms.Add(new SQLiteParameter("@partyId", partyId));
                }
                /*
                if (countyId > 0)
                {
                    query += " AND c.county_id = @countyId ";
                    prms.Add(new SQLiteParameter("@countyId", countyId));
                }
                */
                //query += " AND c.is_written != 1 ";

                query += " ORDER BY c.cand_id";

                prms.Add(new SQLiteParameter("@localeId", localeId));
                prms.Add(new SQLiteParameter("@contestId", contestId));

                //string q = "SELECT c.cand_id, cand_name, incumbent_flag, cand_photo, cn.locale_id, party_logo, c.party_id FROM candidates c left join candidate_name cn on c.cand_id = cn.cand_id and cn.locale_id = 1 WHERE c.contest_id = 9  AND c.county_id = 1858  ORDER BY c.cand_id";
                //DataTable dt = DataManager.VotingContentDataInstance.GetDataTest(query, prms);

                DataTable dtCandidates = DataManager.VotingContentDataInstance.GetDataV2(query, prms);

                if (dtCandidates != null)
				{
					if (dtCandidates.Rows.Count > 0)
					{
						foreach (DataRow dataRow in dtCandidates.Rows)
						{
							Candidate candidate = new Candidate();
							candidate.Id = Helper.Cast(dataRow[Candidate.IdField], -1);
							candidate.ContestId = contestId;
							candidate.Name = VotriteCrypto.Decrypt(Helper.GetStringValue(dataRow[Candidate.NameField]));
							candidate.IncumbentFlag = Helper.GetStringValue(dataRow[Candidate.IncumbentFlagField]) == IncumbentFlags.Y.ToString();
                            candidate.Photo = VotriteCrypto.Decrypt(Helper.GetStringValue(dataRow[Candidate.CandidatePhotoField]));
                            candidate.PartyLogo = VotriteCrypto.Decrypt(Helper.GetStringValue(dataRow[Candidate.PartyLogoField]));
                            candidate.PartyId = Helper.Cast(dataRow[PartyIdField], 0);
                            object readLocaleId = dataRow[Candidate.LocaleIdField];
                            int vLocaleId;
                            if (Int32.TryParse(readLocaleId.ToString(), out vLocaleId))
                                candidate.LocaleId = vLocaleId;
                            else
                                candidate.LocaleId = null;

                            candidate.Voteable = Helper.Cast(dataRow[Candidate.VoteableField], true);
                            candidate.IsWritten = Helper.Cast(dataRow[Candidate.IsWrittenField], false);

                            _candidates.Add(candidate);
						}
					}
				}
			}
			catch (Exception e) { Logger.Instance.Write(e); }

			return _candidates;
		}
        
        public static List<Candidate> GetSlatesCandidates(int contestId, int localeId, int? partyId = null)
        {
            List<Candidate> _candidates = new List<Candidate>();

            try
            {
                var prms = new List<SQLiteParameter>();
                var query = "SELECT sc.id, sc.slate_id, c.cand_id, cand_name, incumbent_flag, cand_photo, cn.locale_id, party_logo, c.party_id, c.voteable, c.is_written FROM slates_candidates sc " +
                    "inner join candidates c on sc.candidate_id=c.cand_id " +
                    "left join candidate_name cn on c.cand_id = cn.cand_id and cn.locale_id = @localeId " +
                    "WHERE sc.contest_id = @contestId";

                if (partyId > 0)
                {
                    query += " AND c.party_id = @partyId ";
                    prms.Add(new SQLiteParameter("@partyId", partyId));
                };

                query += " ORDER BY c.cand_id";

                prms.Add(new SQLiteParameter("@localeId", localeId));
                prms.Add(new SQLiteParameter("@contestId", contestId));

                DataTable dtCandidates = DataManager.VotingContentDataInstance.GetDataV2(query, prms);

                if (dtCandidates != null)
                {
                    if (dtCandidates.Rows.Count > 0)
                    {
                        foreach (DataRow dataRow in dtCandidates.Rows)
                        {
                            Candidate candidate = new Candidate();
                            candidate.Id = Helper.Cast(dataRow[Candidate.IdField], -1);
                            candidate.ContestId = contestId;
                            candidate.Name = VotriteCrypto.Decrypt(Helper.GetStringValue(dataRow[Candidate.NameField]));
                            candidate.IncumbentFlag = Helper.GetStringValue(dataRow[Candidate.IncumbentFlagField]) == IncumbentFlags.Y.ToString();
                            candidate.Photo = VotriteCrypto.Decrypt(Helper.GetStringValue(dataRow[Candidate.CandidatePhotoField]));
                            candidate.PartyLogo = VotriteCrypto.Decrypt(Helper.GetStringValue(dataRow[Candidate.PartyLogoField]));
                            candidate.PartyId = Helper.Cast(dataRow[PartyIdField], 0);
                            object readLocaleId = dataRow[Candidate.LocaleIdField];
                            int vLocaleId;
                            if (Int32.TryParse(readLocaleId.ToString(), out vLocaleId))
                                candidate.LocaleId = vLocaleId;
                            else
                                candidate.LocaleId = null;

                            candidate.Voteable = Helper.Cast(dataRow[Candidate.VoteableField], true);
                            candidate.IsWritten = Helper.Cast(dataRow[Candidate.IsWrittenField], false);

                            _candidates.Add(candidate);
                        }
                    }
                }
            }
            catch (Exception e) { Logger.Instance.Write(e); }

            return _candidates;
        }
        
        public static bool UpdateCandidatesInfo(string query) {
			return DataManager.VotingContentDataInstance.SetData(query);
		}
	}
}
