using System;
using System.Collections.Generic;
using VotRite.Util;
using System.Data.SQLite;
using System.Data;
using VotRiteBallotDataManager.AppCode;
using System.Linq;

namespace VotRite.DBClasses
{
    class Party
    {
        public const string PartiesTable = "parties";
        public const string PartyIdField = "party_id";
        public const string PartyNameField = "party_name";
        public const string BallotIdField = "ballot_id";
        public const string PartyLogoField = "party_logo";

        public int Id { get; set; }
        public string Name { get; set; }
        public int BallotId { get; set; }
        public string PartyLogo { get; set; }

        public static List<Party> GetParties(int ballotId, int localeId)
        {
            var parties = new List<Party>();

            try
            {
                var dtParties = DataManager.VotingContentDataInstance.GetData(@"SELECT p.party_id, p.ballot_id, p.party_logo, pn.party_name 
                                                                                FROM `parties` p 
                                                                                JOIN party_name pn ON pn.party_id = p.party_id and pn.locale_id = @localeid
                                                                                WHERE p.ballot_id = @ballotid",
                                                             new List<SQLiteParameter> { 
                                                                 new SQLiteParameter("@ballotid", ballotId), 
                                                                 new SQLiteParameter("@localeid", localeId) });
                if (dtParties != null)
                {
                    if (dtParties.Rows.Count > 0)
                    {
                        parties.AddRange(from DataRow dataRow in dtParties.Rows
                                         select new Party
                                         {
                                             Id = Helper.Cast(dataRow[PartyIdField], -1),
                                             Name = Helper.GetStringValue(dataRow[PartyNameField]),
                                             BallotId = Helper.Cast(dataRow[BallotIdField], -1),
                                             PartyLogo = Helper.GetStringValue(dataRow[PartyLogoField])
                                         });
                    }
                }
            }
            catch (Exception e)
            {
                Logger.Instance.Write(e);
            }

            return parties;
        }
    }
}
