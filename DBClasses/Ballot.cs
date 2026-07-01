using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using VotRite;
using VotRite.Util;

namespace VotRiteBallotDataManager.AppCode
{
    public class Ballot: IDisposable
    {
        public const string TableName = "ballot";
        public const int DefaultId = 1;
        public const string IdField = "ballot_id";
        public const string TopHeadingField = "top_heading";
        public const string ElectionNameField = "election_name";
        public const string ElectionTypeField = "election_type";
        public const string LocationField = "location";
        public const string MachineField = "machine_election";
        public const string BoardField = "board";
        public const string ReportLocaleIdField = "report_locale_id";
        public const string IsDefaultVoteField = "is_default_vote";
        public const string HasPartyField = "has_party";
        public const string HasCountiesField = "has_counties";
        public const string HasSlatesField = "has_slates";
        public const string HasOverviewField = "has_overview";
        public const string StartTimeField = "start_time";
        public const string EndTimeField = "end_time";
        public const string TestModeField = "test_mode";
        public const string ShowSettingsAndToolsField = "show_settings_tools";

        public int Id { get; set; }
        public string TopHeading { get; set; }
        public string ElectionName { get; set; }
        public string Location { get; set; }
        public string Machine { get; set; }
        public string Board { get; set; }
        public int? ReportLocaleId { get; set; }
        public bool IsDefaultVote { get; set; }
        public bool HasParty { get; set; }
        public bool HasSlates { get; set; }
        public bool HasCounties { get; set; }
        public bool HasOverview { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public bool TestMode { get; set; }
        public bool ShowSettingsAndTools { get; set; }

        public ElectionTypes ElectionType { get; set; }

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

        public static Ballot GetBallot()
        {
            Ballot ballot = null;
            try
            {
                var dtBallot = Driver.TestMode ? 
                    DataManager.VotingContentDataInstance.GetDataV2(
                        "select * from ballot where ballot_id = @ballotid",
                        new List<SQLiteParameter> { new SQLiteParameter("@ballotid", AppManager.BallotId) })
                        :
                    DataManager.VotingContentDataInstance.GetDataV2(
                        "select * from ballot where is_default_vote = @isdefaultvote",
                        new List<SQLiteParameter> { new SQLiteParameter("@isdefaultvote", "Y") });

                if (dtBallot != null)
                {
                    if (dtBallot.Rows.Count > 0)
                    {
                        var dataRow = dtBallot.Rows[0];
                        ballot = new Ballot
                        {
                            Id = Helper.Cast(dataRow[IdField], -1),
                            TopHeading = VotriteCrypto.Decrypt(Helper.GetStringValue(dataRow[TopHeadingField])),
                            ElectionName = VotriteCrypto.Decrypt(Helper.GetStringValue(dataRow[ElectionNameField])),
                            Location = VotriteCrypto.Decrypt(Helper.GetStringValue(dataRow[LocationField])),
                            Machine = Helper.GetStringValue(dataRow[MachineField]),
                            Board = VotriteCrypto.Decrypt(Helper.GetStringValue(dataRow[BoardField])),
                            ElectionType = (ElectionTypes)
                                                        Enum.Parse(typeof(ElectionTypes),
                                                                   Helper.GetStringValue(dataRow[ElectionTypeField])),
                            HasParty = Helper.Cast(dataRow[HasPartyField], false),
                            HasCounties = Helper.Cast(dataRow[HasCountiesField], false),
                            HasSlates = isSlate(Helper.Cast(dataRow[IdField], -1)),// Helper.Cast(dataRow[HasSlatesField], false),
                            HasOverview = Helper.Cast(dataRow[HasOverviewField], false),
                            StartTime = DateTime.Parse(dataRow[StartTimeField].ToString()), // (DateTime)dataRow[StartTimeField],
                            EndTime = DateTime.Parse(dataRow[EndTimeField].ToString()), // (DateTime)dataRow[EndTimeField],
                            TestMode = Helper.Cast(dataRow[TestModeField], false),
                            ShowSettingsAndTools = Helper.Cast(dataRow[ShowSettingsAndToolsField],false),
                            IsDefaultVote = (dataRow[IsDefaultVoteField].ToString() == "Y" ? true : false)
                        };
                        /// if ballot in test mode
                        Driver.TestMode = ballot.TestMode;

                        var readLocaleId = dataRow[ReportLocaleIdField];
                        int vLocaleId;
                        if (Int32.TryParse(readLocaleId.ToString(), out vLocaleId))
                            ballot.ReportLocaleId = vLocaleId;
                        else
                            ballot.ReportLocaleId = null;
                    }
                }
            }
            catch (Exception e)
            {
                Logger.Instance.Write(e);
            }

            return ballot;
        }

        public static Ballot GetBallot(int ballotID)
        {
            Ballot ballot = null;
            try
            {
                var dtBallot = Driver.TestMode ?
                    DataManager.VotingContentDataInstance.GetDataV2(
                        "select * from ballot where ballot_id = @ballotid",
                        new List<SQLiteParameter> { new SQLiteParameter("@ballotid", ballotID) })
                        :
                    DataManager.VotingContentDataInstance.GetDataV2(
                        "select * from ballot where is_default_vote = @isdefaultvote",
                        new List<SQLiteParameter> { new SQLiteParameter("@isdefaultvote", "Y") });

                if (dtBallot != null)
                {
                    if (dtBallot.Rows.Count > 0)
                    {
                        var dataRow = dtBallot.Rows[0];
                        ballot = new Ballot
                        {
                            Id = Helper.Cast(dataRow[IdField], -1),
                            TopHeading = VotriteCrypto.Decrypt(Helper.GetStringValue(dataRow[TopHeadingField])),
                            ElectionName = VotriteCrypto.Decrypt(Helper.GetStringValue(dataRow[ElectionNameField])),
                            Location = VotriteCrypto.Decrypt(Helper.GetStringValue(dataRow[LocationField])),
                            Machine = Helper.GetStringValue(dataRow[MachineField]),
                            Board = VotriteCrypto.Decrypt(Helper.GetStringValue(dataRow[BoardField])),
                            ElectionType = (ElectionTypes)
                                                        Enum.Parse(typeof(ElectionTypes),
                                                                   Helper.GetStringValue(dataRow[ElectionTypeField])),
                            HasParty = Helper.Cast(dataRow[HasPartyField], false),
                            HasCounties = Helper.Cast(dataRow[HasCountiesField], false),
                            HasSlates = isSlate(Helper.Cast(dataRow[IdField], -1)),// Helper.Cast(dataRow[HasSlatesField], false),
                            HasOverview = Helper.Cast(dataRow[HasOverviewField], false),
                            StartTime = DateTime.Parse(dataRow[StartTimeField].ToString()), // (DateTime)dataRow[StartTimeField],
                            EndTime = DateTime.Parse(dataRow[EndTimeField].ToString()), // (DateTime)dataRow[EndTimeField],
                            TestMode = Helper.Cast(dataRow[TestModeField], false),
                            ShowSettingsAndTools = Helper.Cast(dataRow[ShowSettingsAndToolsField], false),
                            IsDefaultVote = (dataRow[IsDefaultVoteField].ToString() == "Y" ? true : false)
                        };
                        /// if ballot in test mode
                        Driver.TestMode = ballot.TestMode;

                        var readLocaleId = dataRow[ReportLocaleIdField];
                        int vLocaleId;
                        if (Int32.TryParse(readLocaleId.ToString(), out vLocaleId))
                            ballot.ReportLocaleId = vLocaleId;
                        else
                            ballot.ReportLocaleId = null;
                    }
                }
            }
            catch (Exception e)
            {
                Logger.Instance.Write(e);
            }

            return ballot;
        }

        public static Ballot GetBallotTranslated(int ballotID, int localeID)
        {
            Ballot ballot = null;
            try
            {
                var dtBallot = 
                    DataManager.VotingContentDataInstance.GetDataV2(
                        "select * from translation_ballot where ballot_id = @ballotid and locale_id = @localeid",
                        new List<SQLiteParameter> { new SQLiteParameter("@ballotid", ballotID) , new SQLiteParameter("@localeid", localeID) });

                if (dtBallot != null)
                {
                    if (dtBallot.Rows.Count > 0)
                    {
                        var dataRow = dtBallot.Rows[0];
                        ballot = new Ballot
                        {
                            //Id = Helper.Cast(dataRow[IdField], -1),
                            TopHeading = VotriteCrypto.Decrypt(Helper.GetStringValue(dataRow[TopHeadingField])),
                            ElectionName = VotriteCrypto.Decrypt(Helper.GetStringValue(dataRow[ElectionNameField])),
                            Location = VotriteCrypto.Decrypt(Helper.GetStringValue(dataRow[LocationField])),
                            //Machine = Helper.GetStringValue(dataRow[MachineField]),
                            Board = VotriteCrypto.Decrypt(Helper.GetStringValue(dataRow[BoardField])),
                            //ElectionType = (ElectionTypes)
                            //                            Enum.Parse(typeof(ElectionTypes),
                            //                                       Helper.GetStringValue(dataRow[ElectionTypeField])),
                            //HasParty = Helper.Cast(dataRow[HasPartyField], false),
                            //HasCounties = Helper.Cast(dataRow[HasCountiesField], false),
                            //HasSlates = isSlate(Helper.Cast(dataRow[IdField], -1)),// Helper.Cast(dataRow[HasSlatesField], false),
                            //HasOverview = Helper.Cast(dataRow[HasOverviewField], false),
                            //StartTime = DateTime.Parse(dataRow[StartTimeField].ToString()), // (DateTime)dataRow[StartTimeField],
                            //EndTime = DateTime.Parse(dataRow[EndTimeField].ToString()), // (DateTime)dataRow[EndTimeField],
                            //TestMode = Helper.Cast(dataRow[TestModeField], false),
                            //ShowSettingsAndTools = Helper.Cast(dataRow[ShowSettingsAndToolsField], false),
                            //IsDefaultVote = (dataRow[IsDefaultVoteField].ToString() == "Y" ? true : false)
                        };
                        /// if ballot in test mode
                        //Driver.TestMode = ballot.TestMode;

                        //var readLocaleId = dataRow[ReportLocaleIdField];
                        //int vLocaleId;
                        //if (Int32.TryParse(readLocaleId.ToString(), out vLocaleId))
                        //    ballot.ReportLocaleId = vLocaleId;
                        //else
                        //    ballot.ReportLocaleId = null;
                    }
                }
            }
            catch (Exception e)
            {
                Logger.Instance.Write(e);
            }

            return ballot;
        }


        public static bool isSlate(int ballotID)
        {
            try
            {
               var dt= DataManager.VotingContentDataInstance.GetDataV2(
                       "select count(*) from slates where ballot_id = @ballotid",
                       new List<SQLiteParameter> { new SQLiteParameter("@ballotid", ballotID) });
                if (dt.Rows[0][0].ToString() == "0" || dt.Rows[0][0].ToString() == "")
                    return false;
                else
                    return true;
            }
            catch (Exception)
            {

                return false;
            }
        }

        // Added for multiple print
        public static List<Ballot> GetBallots(DataTable dtIds)
        {
            List<int> bIds = new List<int>();
            foreach (DataRow row in dtIds.Rows)
            {
                if (row["ballotId"] != DBNull.Value)
                    bIds.Add(Convert.ToInt32(row["ballotId"]));
            }
            List<Ballot> ballots = new List<Ballot>();
            try
            {
                if (bIds.Count > 0)
                {
                    string query = "select * from ballot where ballot_id in (";
                    var sqlParameters = new List<SQLiteParameter>();
                    for (int i = 0; i < bIds.Count; i++)
                    {
                        var paramName = "@ballot_id" + (i);
                        query += (i == 0 ? "" : ", ") + paramName;
                        sqlParameters.Add(new SQLiteParameter(paramName, bIds[i]));
                    }
                    query += ")";
                    var dtBallots = DataManager.VotingContentDataInstance.GetDataV2(query, sqlParameters);
                    if (dtBallots != null)
                    {
                        if (dtBallots.Rows.Count > 0)
                        {
                            foreach (DataRow dataRow in dtBallots.Rows)
                            {
                                //var dataRow = dtBallots.Rows[0];
                                Ballot ballot = new Ballot
                                {
                                    Id = Helper.Cast(dataRow[IdField], -1),
                                    TopHeading = VotriteCrypto.Decrypt(Helper.GetStringValue(dataRow[TopHeadingField])),
                                    ElectionName = VotriteCrypto.Decrypt(Helper.GetStringValue(dataRow[ElectionNameField])),
                                    Location = VotriteCrypto.Decrypt(Helper.GetStringValue(dataRow[LocationField])),
                                    Machine = Helper.GetStringValue(dataRow[MachineField]),
                                    Board = VotriteCrypto.Decrypt(Helper.GetStringValue(dataRow[BoardField])),
                                    ElectionType = (ElectionTypes)
                                                   Enum.Parse(typeof(ElectionTypes),
                                                              Helper.GetStringValue(dataRow[ElectionTypeField])),
                                    HasParty = Helper.Cast(dataRow[HasPartyField], false),
                                    HasCounties = Helper.Cast(dataRow[HasCountiesField], false),
                                    HasSlates = isSlate(Helper.Cast(dataRow[IdField], -1)),//Helper.Cast(dataRow[HasSlatesField], false),
                                    HasOverview = Helper.Cast(dataRow[HasOverviewField], false),
                                    StartTime = Convert.ToDateTime(dataRow[StartTimeField]),
                                    EndTime = Convert.ToDateTime(dataRow[EndTimeField]),
                                    IsDefaultVote = (dataRow[IsDefaultVoteField].ToString() == "Y" ? true : false)
                                };
                                var readLocaleId = dataRow[ReportLocaleIdField];
                                int vLocaleId;
                                if (Int32.TryParse(readLocaleId.ToString(), out vLocaleId))
                                    ballot.ReportLocaleId = vLocaleId;
                                else
                                    ballot.ReportLocaleId = null;

                                ballots.Add(ballot);
                            }
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Logger.Instance.Write(e);
            }

            return ballots;
        }

        // Added for multiple selected ballots print
        public static List<Ballot> GetBallots(List<Ballot> dtIds)
        {
            List<int> bIds = new List<int>();
            //foreach (DataRow row in dtIds.Rows)
            //{
            //    if (row["ballotId"] != DBNull.Value)
            //        bIds.Add(Convert.ToInt32(row["ballotId"]));
            //}
            foreach (var bid in dtIds)
            {
                bIds.Add(bid.Id);
            }
            List<Ballot> ballots = new List<Ballot>();
            try
            {
                if (bIds.Count > 0)
                {
                    string query = "select * from ballot where ballot_id in (";
                    var sqlParameters = new List<SQLiteParameter>();
                    for (int i = 0; i < bIds.Count; i++)
                    {
                        var paramName = "@ballot_id" + (i);
                        query += (i == 0 ? "" : ", ") + paramName;
                        sqlParameters.Add(new SQLiteParameter(paramName, bIds[i]));
                    }
                    query += ")";
                    var dtBallots = DataManager.VotingContentDataInstance.GetDataV2(query, sqlParameters);
                    if (dtBallots != null)
                    {
                        if (dtBallots.Rows.Count > 0)
                        {
                            foreach (DataRow dataRow in dtBallots.Rows)
                            {
                                //var dataRow = dtBallots.Rows[0];
                                Ballot ballot = new Ballot
                                {
                                    Id = Helper.Cast(dataRow[IdField], -1),
                                    TopHeading = VotriteCrypto.Decrypt(Helper.GetStringValue(dataRow[TopHeadingField])),
                                    ElectionName = VotriteCrypto.Decrypt(Helper.GetStringValue(dataRow[ElectionNameField])),
                                    Location = VotriteCrypto.Decrypt(Helper.GetStringValue(dataRow[LocationField])),
                                    Machine = Helper.GetStringValue(dataRow[MachineField]),
                                    Board = VotriteCrypto.Decrypt(Helper.GetStringValue(dataRow[BoardField])),
                                    ElectionType = (ElectionTypes)
                                                   Enum.Parse(typeof(ElectionTypes),
                                                              Helper.GetStringValue(dataRow[ElectionTypeField])),
                                    HasParty = Helper.Cast(dataRow[HasPartyField], false),
                                    HasCounties = Helper.Cast(dataRow[HasCountiesField], false),
                                    HasSlates = isSlate(Helper.Cast(dataRow[IdField], -1)),//Helper.Cast(dataRow[HasSlatesField], false),
                                    HasOverview = Helper.Cast(dataRow[HasOverviewField], false),
                                    StartTime = Convert.ToDateTime(dataRow[StartTimeField]),
                                    EndTime = Convert.ToDateTime(dataRow[EndTimeField]),
                                    IsDefaultVote = (dataRow[IsDefaultVoteField].ToString() == "Y" ? true : false)
                                };
                                var readLocaleId = dataRow[ReportLocaleIdField];
                                int vLocaleId;
                                if (Int32.TryParse(readLocaleId.ToString(), out vLocaleId))
                                    ballot.ReportLocaleId = vLocaleId;
                                else
                                    ballot.ReportLocaleId = null;

                                ballots.Add(ballot);
                            }
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Logger.Instance.Write(e);
            }

            return ballots;
        }


        // Added for displaying list
        public static List<Ballot> GetBallots()
        {

            List<Ballot> ballots = new List<Ballot>();
            try
            {

                string query = "select * from ballot;";
                var sqlParameters = new List<SQLiteParameter>();

                var dtBallots = DataManager.VotingContentDataInstance.GetDataV2(query);
                if (dtBallots != null)
                {
                    if (dtBallots.Rows.Count > 0)
                    {
                        foreach (DataRow dataRow in dtBallots.Rows)
                        {
                            //var dataRow = dtBallots.Rows[0];
                            Ballot ballot = new Ballot
                            {
                                Id = Helper.Cast(dataRow[IdField], -1),
                                TopHeading = VotriteCrypto.Decrypt(Helper.GetStringValue(dataRow[TopHeadingField])),
                                ElectionName = VotriteCrypto.Decrypt(Helper.GetStringValue(dataRow[ElectionNameField])),
                                Location = VotriteCrypto.Decrypt(Helper.GetStringValue(dataRow[LocationField])),
                                Machine = Helper.GetStringValue(dataRow[MachineField]),
                                Board = VotriteCrypto.Decrypt(Helper.GetStringValue(dataRow[BoardField])),
                                ElectionType = (ElectionTypes)
                                               Enum.Parse(typeof(ElectionTypes),
                                                          Helper.GetStringValue(dataRow[ElectionTypeField])),
                                HasParty = Helper.Cast(dataRow[HasPartyField], false),
                                HasCounties = Helper.Cast(dataRow[HasCountiesField], false),
                                HasSlates = isSlate(Helper.Cast(dataRow[IdField], -1)),//Helper.Cast(dataRow[HasSlatesField], false),
                                HasOverview = Helper.Cast(dataRow[HasOverviewField], false),
                                StartTime = Convert.ToDateTime(dataRow[StartTimeField]),
                                EndTime = Convert.ToDateTime(dataRow[EndTimeField]),
                                IsDefaultVote = (dataRow[IsDefaultVoteField].ToString() == "Y" ? true : false)
                            };
                            var readLocaleId = dataRow[ReportLocaleIdField];
                            int vLocaleId;
                            if (Int32.TryParse(readLocaleId.ToString(), out vLocaleId))
                                ballot.ReportLocaleId = vLocaleId;
                            else
                                ballot.ReportLocaleId = null;

                            ballots.Add(ballot);
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Logger.Instance.Write(e);
            }

            return ballots;
        }


        public static void UpdateMachineValue(string machineValue)
        {
            DataManager.VotingContentDataInstance.SetData(string.Format("UPDATE {0} SET {1}='{2}'",
                                                                                       TableName,
                                                                                       MachineField,
                                                                                     VotriteCrypto.Encrypt( Helper.EscapeStringData(
                                                                                           machineValue))));
        }

        public static void UpdateMachineValue_Location(string machineValue, string locationValue)
        {
            DataManager.VotingContentDataInstance.SetData(string.Format("UPDATE {0} SET {1}='{2}',{3}='{4}'",
                                                                                       TableName,
                                                                                       MachineField,
                                                                                     VotriteCrypto.Encrypt(Helper.EscapeStringData(
                                                                                           machineValue)),
                                                                                       VotriteCrypto.Encrypt(LocationField),
                                                                                       VotriteCrypto.Encrypt(locationValue)
                                                                                       ));
        }

        public static bool UpdateBallotDefault(int BallotID)
        {
            var updated = DataManager.VotingContentDataInstance.SetData("UPDATE ballot set is_default_vote='N'");

            updated = DataManager.VotingContentDataInstance.SetData("UPDATE ballot set is_default_vote='Y' where ballot_id="+BallotID);
            
            
            if (!updated)
                throw new Exception("Can't update Ballot defaul field in data base.");
            return updated;
        }

        public static int GetDefaultBallotId()
        {
            var defaultBallotId = -1;
            try
            {
                var dtBallot =
                    DataManager.VotingContentDataInstance.GetDataV2(
                        "select ballot_id from ballot where is_default_vote = @isdefaultvote",
                        new List<SQLiteParameter> { new SQLiteParameter("@isdefaultvote", "Y") });
                if (dtBallot != null)
                {
                    if (dtBallot.Rows.Count > 0)
                    {
                        var dataRow = dtBallot.Rows[0];
                        defaultBallotId = Helper.Cast(dataRow[IdField], -1);
                    }
                }
            }
            catch (Exception e)
            {
                Logger.Instance.Write(e);
            }

            return defaultBallotId;
        }
    }
}