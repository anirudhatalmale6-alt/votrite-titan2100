using System.Collections.Generic;
using System.Data;
using VotRite;
using System;
using VotRite.Util;
using System.Text;
using System.Data.SQLite;
using System.Linq;

namespace VotRiteBallotDataManager.AppCode
{
    public enum ElectionTypes
    {
        standard,
        shares,
        percent,
        shares_times,
        ranking_choice
    }

	public class Race: IDisposable
    {
		public const string TableName = "race";
		public const string IdField = "race_loc_id";
		public const string TitleField = "race_loc_title";
		public const string VotedPositionField = "loc_voted_position";
		public const string ContestIdField = "contest_id";
		public const string LocaleIdField = "locale_id";

		public int Id { get; set; }
		public int ContestId { get; set; }
		public int LocaleId { get; set; }
		public string Title { get; set; }
		public string VotedPosition { get; set; }

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

		public static Race GetRace(int contestId, int localeId) {
			Race _race = null;
			try
			{
				DataTable dtRace = DataManager.VotingContentDataInstance.GetDataV2(string.Format("SELECT * FROM {0} WHERE {1}={2} AND {3}={4}", 
																				Race.TableName,
																				Race.ContestIdField,
																				contestId,
																				Race.LocaleIdField,
																				localeId));
				if (dtRace != null)
				{
					if (dtRace.Rows.Count > 0)
					{
						DataRow dataRow = dtRace.Rows[0];
						_race = new Race();
						_race.Id = Helper.Cast(dataRow[Race.IdField], -1);
						_race.ContestId = contestId;
						_race.LocaleId = localeId;
						_race.Title = VotriteCrypto.Decrypt(Helper.GetStringValue(dataRow[Race.TitleField]));
						_race.VotedPosition = VotriteCrypto.Decrypt(Helper.GetStringValue(dataRow[Race.VotedPositionField]));
					}                    
				}
			}
			catch (Exception e) { Logger.Instance.Write(e); }

			return _race;
		}

        public static List<Race> GetAllRaces(int localeId, int? ballotId)
        {
            var races = new List<Race>();
            try
            {
                var sql = "SELECT * FROM race r ";
                var parameters = new List<SQLiteParameter> { new SQLiteParameter("@localeid", localeId) };
                if (ballotId != null)
                {
                    sql += "join contest c on c.contest_id = r.contest_id and ballot_id = @ballotid ";
                    parameters.Add(new SQLiteParameter("@ballotid", ballotId));
                }
                sql += "WHERE locale_id = @localeid";
                var dtRace = DataManager.VotingContentDataInstance.GetDataV2(sql, parameters);
                if (dtRace != null)
                {
                    if (dtRace.Rows.Count > 0)
                    {
                        races.AddRange(from DataRow dataRow in dtRace.Rows
                                       select new Race
                                       {
                                           Id = Helper.Cast(dataRow[Race.IdField], -1),
                                           ContestId = Helper.Cast(dataRow[Race.ContestIdField], -1),
                                           LocaleId = localeId,
                                           Title = VotriteCrypto.Decrypt(Helper.GetStringValue(dataRow[Race.TitleField])),
                                           VotedPosition =
                                               VotriteCrypto.Decrypt(Helper.GetStringValue(dataRow[Race.VotedPositionField]))
                                       });
                    }
                }
            }
            catch (Exception e) { Logger.Instance.Write(e); }

            return races;
        }

        public static bool CopyRacesFromDefaultLocale(int localeId, int? ballotId)
        {
            var races = GetAllRaces(localeId, ballotId);
            var defaultRaces = GetAllRaces(Locale.DefaultLocale.Id, ballotId);
            var result = true;
            foreach (var defaultRace in defaultRaces)
            {
                var idx = races.FindIndex(r => r.ContestId == defaultRace.ContestId);
                Race race = null;
                if (idx > -1)
                    race = races[idx];
                if (race == null)
                {
                    race = defaultRace;
                    race.LocaleId = localeId;
                    result &= InsertRace(race) != null;
                }
            }
            return result;
        }

        public static Race InsertRace(Race race)
        {
            var success = DataManager.VotingContentDataInstance.SetData(string.Format("INSERT INTO {0} ({1}, {2}, {3}, {4}) values ({5}, {6}, '{7}', '{8}');",
                                                                        Race.TableName,
                                                                        Race.ContestIdField,
                                                                        Race.LocaleIdField,
                                                                        VotriteCrypto.Encrypt(Race.TitleField),
                                                                        VotriteCrypto.Encrypt(Race.VotedPositionField),
                                                                        race.ContestId,
                                                                        race.LocaleId,
                                                                        Helper.EscapeStringData(race.Title),
                                                                        Helper.EscapeStringData(race.VotedPosition)
                                                                        ));

            if (!success)
                return null;

            race.Id = Helper.GetMaxId(TableName, IdField);

            return race;
        }

        public static List<Race> GetAllRaces(int localeId, int ballotId)
        {
            var races = new List<Race>();
            try
            {
                var sql = "SELECT * FROM Race r ";
                var sqlParameters = new List<SQLiteParameter> { new SQLiteParameter("@localeid", localeId) };

                sql += "join contest c on c.contest_id = r.contest_id and ballot_id = @ballotid ";
                sqlParameters.Add(new SQLiteParameter("@ballotid", ballotId));

                sql += "WHERE locale_id = @localeid";
                var dtRace = DataManager.VotingContentDataInstance.GetDataV2(sql, sqlParameters);

                if (dtRace != null)
                {
                    if (dtRace.Rows.Count > 0)
                    {
                        races.AddRange(from DataRow dataRow in dtRace.Rows
                                       select new Race
                                       {
                                           Id = Helper.Cast(dataRow[IdField], -1),
                                           ContestId = Helper.Cast(dataRow[ContestIdField], -1),
                                           LocaleId = localeId,
                                           Title = VotriteCrypto.Decrypt(Helper.GetStringValue(dataRow[TitleField])),
                                           VotedPosition =
                                               VotriteCrypto.Decrypt(Helper.GetStringValue(dataRow[VotedPositionField]))
                                       });
                    }
                }
            }
            catch (Exception e)
            {
                Logger.Instance.Write(e);
            }

            return races;
        }
	}
}
