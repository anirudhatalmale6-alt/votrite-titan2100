using System.Collections.Generic;
using System.Data.SQLite;
using System.Linq;
using VotRite;
using System.Data;
using System;
using VotRite.Util;

namespace VotRiteBallotDataManager.AppCode
{
	public class Proposition: IDisposable 
    {
		public const string TableName = "proposition";
		public const string IdField = "prop_loc_id";
		public const string ContestIdField = "contest_id";
		public const string LocaleIdField = "locale_id";
		public const string TitleField = "prop_loc_name";
		public const string TextField = "prop_loc_text";
        public const string ChangedTextField = "new_prop_loc_text";
        public const string IsMassField = "is_mass";
        public const string PositiveAnswerField = "positive_answer";
        public const string NegativeAnswerField = "negative_answer";

		public int Id { get; set; }
		public int ContestId { get; set; }
		public int LocaleId { get; set; }
		public string Title { get; set; }
		public string Text { get; set; }
        public string ChangedText { get; set; }
        public bool IsMass { get; set; }
        public string PositiveAnswer { get; set; }
        public string NegativeAnswer { get; set; }

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

		public static Proposition GetProposition(int contestId, int localeId)
		{
			Proposition proposition = null;
			try
			{
				DataTable dtProposition = DataManager.VotingContentDataInstance.GetDataV2(
                    string.Format("SELECT * FROM {0} WHERE {1}={2} AND {3}={4}",
								    Proposition.TableName,
									Proposition.ContestIdField,
									contestId,
									Proposition.LocaleIdField,
									localeId));

				if (dtProposition != null)
				{
					if (dtProposition.Rows.Count > 0)
					{
						DataRow dataRow = dtProposition.Rows[0];
						proposition = new Proposition();
						proposition.Id = Helper.Cast(dataRow[Proposition.IdField], -1);
						proposition.ContestId = contestId;
						proposition.LocaleId = localeId;
						proposition.Title = Helper.GetStringValue(dataRow[Proposition.TitleField]);
						proposition.Text = Helper.GetStringValue(dataRow[Proposition.TextField]);
                        proposition.ChangedText = Helper.GetStringValue(dataRow[Proposition.ChangedTextField]);
                        proposition.IsMass = Helper.Cast(dataRow[Proposition.IsMassField], false);
                        proposition.PositiveAnswer = Helper.GetStringValue(dataRow[Proposition.PositiveAnswerField]);
                        proposition.NegativeAnswer = Helper.GetStringValue(dataRow[Proposition.NegativeAnswerField]);                        
					}
				}
			}
			catch (Exception e) { Logger.Instance.Write(e); }

			return proposition;
		}

        public static bool CopyPropositionsFromDefaultLocale(int localeId, int? ballotId)
        {
            var propositions = GetAllPropositions(localeId, null, null, null, ballotId);
            var defaultPropositions = GetAllPropositions(Locale.DefaultLocale.Id, null, null, null, ballotId);
            var result = true;
            foreach (var defaultProposition in defaultPropositions)
            {
                var idx = propositions.FindIndex(p => p.ContestId == defaultProposition.ContestId);
                Proposition proposition = null;
                if (idx > -1)
                    proposition = propositions[idx];
                if (proposition == null)
                {
                    proposition = defaultProposition;
                    proposition.LocaleId = localeId;
                    result &= InsertProposition(proposition) != null;
                }
            }
            return result;
        }

        public static Proposition InsertProposition(Proposition proposition)
        {
            const string sql = "INSERT INTO proposition (contest_id, locale_id, prop_loc_name, prop_loc_text, new_prop_loc_text, is_mass, positive_answer, negative_answer) values " +
                "(@contid, @locid, @propname, @proptext, @newproptext, @ismass, @positiveanswer, @negativeanswer)";
            var sqlParameters = new List<SQLiteParameter>
                                    {
                                        new SQLiteParameter("@contid", proposition.ContestId),
                                        new SQLiteParameter("@locid", proposition.LocaleId),
                                        new SQLiteParameter("@propname", proposition.Title),
                                        new SQLiteParameter("@proptext", proposition.Text),
                                        new SQLiteParameter("@newproptext", proposition.ChangedText),
                                        new SQLiteParameter("@ismass", proposition.IsMass ? 1 : 0),
                                        new SQLiteParameter("@positiveanswer", proposition.PositiveAnswer),
                                        new SQLiteParameter("@negativeanswer", proposition.NegativeAnswer)
                                    };
            var success = DataManager.VotingContentDataInstance.SetData(sql, sqlParameters);

            if (!success)
                return null;

            proposition.Id = Helper.GetMaxId(Proposition.TableName, Proposition.IdField);

            return proposition;
        }

        public static List<Proposition> GetAllPropositions(int localeId, int? startIndex, int? pageSize,
                                                   bool? chooseMass, int? ballotId)
        {
            var propositions = new List<Proposition>();
            try
            {
                var sql = "SELECT * FROM proposition p ";
                var sqlParameters = new List<SQLiteParameter> { new SQLiteParameter("@locid", localeId) };
                var joinSt = "where";
                if (ballotId != null)
                {
                    sql += " join contest c on c.contest_id = p.contest_id where c.ballot_id = @ballotid ";
                    sqlParameters.Add(new SQLiteParameter("@ballotid", ballotId));
                    joinSt = "and";
                }
                sql += joinSt + " locale_id = @locid";
                if (chooseMass != null)
                {
                    sql += " and is_mass = @ismass";
                    sqlParameters.Add(new SQLiteParameter("@ismass", chooseMass == true ? 1 : 0));
                }
                sql += " ORDER BY contest_id, locale_id, prop_loc_id";
                if (pageSize != null)
                {
                    sql += " LIMIT @limit";
                    sqlParameters.Add(new SQLiteParameter("@limit", pageSize));
                    if (startIndex != null)
                    {
                        sql += " OFFSET @offset";
                        sqlParameters.Add(new SQLiteParameter("@offset", startIndex));
                    }
                }
                var dtProposition = DataManager.VotingContentDataInstance.GetDataV2(sql, sqlParameters);
                if (dtProposition != null)
                {
                    if (dtProposition.Rows.Count > 0)
                    {
                        propositions.AddRange(from DataRow dataRow in dtProposition.Rows
                                              select new Proposition
                                              {
                                                  Id = Helper.Cast(dataRow[Proposition.IdField], -1),
                                                  ContestId =
                                                      Helper.Cast(dataRow[Proposition.ContestIdField], 0),
                                                  LocaleId = localeId,
                                                  Title =
                                                      Helper.GetStringValue(dataRow[Proposition.TitleField]),
                                                  Text =
                                                      Helper.GetStringValue(dataRow[Proposition.TextField]),
                                                  ChangedText =
                                                      Helper.GetStringValue(
                                                          dataRow[Proposition.ChangedTextField]),
                                                  IsMass =
                                                      Helper.Cast(dataRow[Proposition.IsMassField], false),
                                                  PositiveAnswer =
                                                      Helper.GetStringValue(
                                                          dataRow[Proposition.PositiveAnswerField]),
                                                  NegativeAnswer =
                                                      Helper.GetStringValue(
                                                          dataRow[Proposition.NegativeAnswerField])
                                              });
                    }
                }
            }
            catch (Exception e)
            {
                Logger.Instance.Write(e);
            }

            return propositions;
        }

        public static List<Proposition> GetPropositions(List<int> contestIds, int localeId)
		{
            var propositions = new List<Proposition>();
			try
			{
                var sql = "SELECT * FROM proposition WHERE locale_id=@locid";
                var sqlParameters = new List<SQLiteParameter>
                                        {                                            
                                            new SQLiteParameter("@locid", localeId),
                                        };

                if (contestIds != null && contestIds.Count > 0)
                {
                    sql += " AND contest_id IN (";
                    for (var i = 0; i < contestIds.Count; i++ )
                    {
                        sql += (i == 0 ? "" : ", ") + "@contid" + i;
                        sqlParameters.Add(new SQLiteParameter("@contid" + i, contestIds[i]));
                    }
                    sql += ")";
                }
                var dtProposition = DataManager.VotingContentDataInstance.GetDataV2(sql, sqlParameters);
				if (dtProposition != null)
				{
                    if (dtProposition.Rows.Count > 0)
                    {
                        propositions.AddRange(from DataRow dataRow in dtProposition.Rows
                                              select new Proposition
                                                         {
                                                             Id = Helper.Cast(dataRow[Proposition.IdField], -1),
                                                             ContestId =
                                                                 Helper.Cast(dataRow[Proposition.ContestIdField], 0),
                                                             LocaleId = localeId,
                                                             Title =
                                                                 Helper.GetStringValue(dataRow[Proposition.TitleField]),
                                                             Text =
                                                                 Helper.GetStringValue(dataRow[Proposition.TextField]),
                                                             ChangedText =
                                                                 Helper.GetStringValue(
                                                                     dataRow[Proposition.ChangedTextField]),
                                                             IsMass =
                                                                 Helper.Cast(dataRow[Proposition.IsMassField], false),
                                                             PositiveAnswer = Helper.GetStringValue(dataRow[Proposition.PositiveAnswerField]),
                                                             NegativeAnswer = Helper.GetStringValue(dataRow[Proposition.NegativeAnswerField])
                                                         });
                    }
				}
			}
			catch (Exception e) { Logger.Instance.Write(e); }

            return propositions;
		}

	}
}
