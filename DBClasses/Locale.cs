using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using VotRite;
using VotRite.Util;
using System.Linq;

namespace VotRiteBallotDataManager.AppCode
{
    public enum LocaleUseFlags
    {
        Y,
        N
    }

    public class Locale: IDisposable
    {
        public const string DefaultLanguage = "English";

        public const string TableName = "locale";
        private const string IdField = "locale_id";
        private const string CodeField = "locale_code";
        public const string NameField = "locale_name";
        private const string PhraseField = "locale_phrase";
        private const string BtnSkipField = "btn_skip";
        private const string BtnNextField = "btn_next";
        private const string BtnBackField = "btn_back";
        private const string BtnPrintReviewField = "btn_print_review";
        private const string BtnMoreCandidatesField = "btn_more_candidates";
        private const string BtnMoreResultsField = "btn_more_results";
        private const string LblContestCountField = "lbl_contest_count";
        private const string BtnReviewField = "btn_review";
        private const string LblContestTipField = "lbl_contest_tip";
        private const string PropositionTipField = "proposition_tip";
        private const string LblMulti0Choices2Field = "lbl_multi_0_choices2";
        private const string LblChoice1Field = "lbl_choice1";
        private const string LblChoice2Field = "lbl_choice2";
        private const string BtnBackToVoteField = "btn_back_to_vote";
        private const string BtnResetBallotField = "btn_reset_ballot";
        private const string BtnAcceptPrintField = "btn_accept_print";
        private const string BtnAcceptSaveField = "btn_accept_save";
        private const string BtnCastVoteField = "btn_cast_vote";
        private const string LblDoneField = "lbl_done";
        private const string LblThankyouField = "lbl_thankyou";
        private const string LblReviewTitleField = "lbl_review_title";
        private const string LblReviewTipField = "lbl_review_tip";
        private const string LblUnderVoteMsgField = "lbl_undervote_msg";
        private const string LblSpeechRateDescField = "lbl_speech_rate_desc";
        private const string LblSettingToolsDescField = "lbl_setting_tools_desc";
        
        private const string NoSelectionField = "no_selection";
        private const string VoterField = "voter";
        private const string MachineField = "machine";
        private const string VoidField = "void";
        private const string LblConfirmTitleField = "lbl_confirm_title";
        private const string LblConfirmTipField = "lbl_confirm_tip";
        private const string RaceTipField = "race_tip";
        private const string PropositionForField = "proposition_for";
        private const string PropositionAgainstField = "proposition_against";
        private const string PropositionYesField = "proposition_yes";
        private const string PropositionNoField = "proposition_no";
        private const string IncumbentFootnoteField = "incumbent_footnote";
        private const string PropositionSubheadingField = "proposition_subheading";
        public const string WriteinHelp1Field = "writein_help1";
        public const string WriteinHelp2Field = "writein_help2";
        public const string WriteinHelp3Field = "writein_help3";
        public const string WriteinHelp4Field = "writein_help4";
        public const string WriteinHelp5Field = "writein_help5";
        public const string BtnAcceptField = "btn_accept";
        public const string BtnCancelField = "btn_cancel";
        public const string BtnAddCandidatesField = "btn_add_candidates";
        public const string BtnAudioField = "btn_audio";
        public const string BtnTextSizeField = "btn_textsize";
        public const string BtnLanguageField = "btn_language";
        public const string BtnHelpField = "btn_help";
        public const string BtnRemotePollWorkerField = "btn_remote_poll_worker";

        public const string BtnSettingToolsField = "btn_setting_tools";
        public const string BtnCallHelpField = "btn_callhelp";
        
        public const string MinRaceTipField = "min_race_tip";
        public const string PartyTipField = "party_tip";
        public const string CountyTipField = "county_tip";
        public const string SlateTipField = "slate_tip";
        public const string BtnBackSlateField = "btn_back_slate";

        private Dictionary<string, LocaleField> _localeFields;

        public int Id { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        private bool UseFlag { get; set; }

        public static readonly Locale DefaultLocale;

        static Locale()
        {
            DefaultLocale = GetDefaultLocale();
        }

        public static Locale GetDefaultLocale()
        {
            return GetLocale(null, DefaultLanguage);
        }

        public static Locale GetLocale(int? id, string name)
        {
            Locale locale = null;

            try
            {
                var sql = "select * from locale";
                var isFirstParam = true;
                var sqlParameters = new List<SQLiteParameter>();
                if (id != null)
                {
                    sql += " where locale_id = @locid";
                    sqlParameters.Add(new SQLiteParameter("@locid", id));
                    isFirstParam = false;
                }
                if (name != null)
                {
                    sql += isFirstParam ? " where" : " and";
                    sql += " locale_name = @locname";
                    sqlParameters.Add(new SQLiteParameter("@locname", name));
                    isFirstParam = false;
                }
                var dtLocale = DataManager.VotingContentDataInstance.GetDataV2(sql, sqlParameters);
                if (dtLocale == null || dtLocale.Rows.Count == 0)
                {
                    return locale;
                }

                var dataRow = dtLocale.Rows[0];
                locale = new Locale
                {
                    Id = Helper.Cast(dataRow[Locale.IdField], -1),
                    Name = Helper.GetStringValue(dataRow[Locale.NameField]),
                    Code = Helper.GetStringValue(dataRow[Locale.CodeField])
                };


                GetLocaleFields(locale);

                return locale;
            }
            catch (Exception e)
            {
                Logger.Instance.Write(e);
            }

            return locale;
        }

        public static void GetLocaleFields(Locale locale)
        {
            var sqlParameters = new List<SQLiteParameter>();
            const string fieldsSql =
                "select locale_id, field_name, field_value, field_title, show_order from locale_fields where locale_id = @locid order by show_order";
            sqlParameters.Add(new SQLiteParameter("@locid", locale.Id));
            var dtLocale = DataManager.VotingContentDataInstance.GetDataV2(fieldsSql, sqlParameters);

            if (dtLocale != null && dtLocale.Rows.Count > 0)
            {
                //--Added & commented on 01/12/2019 for fixing issues occurring on external pinpad screen implementation--------
                locale._localeFields = new Dictionary<string, LocaleField>();
                foreach (DataRow dtRow in dtLocale.Rows)
                {
                    if (!locale._localeFields.ContainsKey(VotriteCrypto.Decrypt(dtRow["field_name"].ToString()) as string))
                    {
                        locale._localeFields.Add(VotriteCrypto.Decrypt(dtRow["field_name"].ToString()) as string, new LocaleField
                        {
                            LocaleId = locale.Id,
                            FieldName = VotriteCrypto.Decrypt(dtRow["field_name"] as string),
                            FieldValue = VotriteCrypto.Decrypt(dtRow["field_value"] as string),
                            FieldTitle = VotriteCrypto.Decrypt(dtRow["field_title"] as string),
                            ShowOrder = Helper.Cast(dtRow["show_order"], 0)
                        });
                    }
                }
                //locale.LocaleFields.AddRange(from DataRow dtRow in dtLocale.Rows
                //                             select new LocaleField
                //                             {
                //                                 LocaleId = locale.Id,
                //                                 FieldName = dtRow["field_name"] as string,
                //                                 FieldValue = dtRow["field_value"] as string,
                //                                 FieldTitle = dtRow["field_title"] as string,
                //                                 ShowOrder = Helper.Cast(dtRow["show_order"], 0)
                //                             });

                //----------------------------------------------------------------------------------------------------------
            }
        }

        public Dictionary<string, LocaleField> GetLocaleFields_loc(Locale locale)
        {
            var l_localeFields = new Dictionary<string, LocaleField>();
            var sqlParameters = new List<SQLiteParameter>();
            const string fieldsSql =
                "select locale_id, field_name, field_value, field_title, show_order from locale_fields where locale_id = @locid order by show_order";
            sqlParameters.Add(new SQLiteParameter("@locid", locale.Id));
            var dtLocale = DataManager.VotingContentDataInstance.GetDataV2(fieldsSql, sqlParameters);

            if (dtLocale != null && dtLocale.Rows.Count > 0)
            {
                //--Added & commented on 01/12/2019 for fixing issues occurring on external pinpad screen implementation--------
                
                foreach (DataRow dtRow in dtLocale.Rows)
                {
                    if (!l_localeFields.ContainsKey(VotriteCrypto.Decrypt(dtRow["field_name"].ToString()) as string))
                    {
                        l_localeFields.Add(VotriteCrypto.Decrypt(dtRow["field_name"].ToString()) as string, new LocaleField
                        {
                            LocaleId = locale.Id,
                            FieldName = VotriteCrypto.Decrypt(dtRow["field_name"] as string),
                            FieldValue = VotriteCrypto.Decrypt(dtRow["field_value"] as string),
                            FieldTitle = VotriteCrypto.Decrypt(dtRow["field_title"] as string),
                            ShowOrder = Helper.Cast(dtRow["show_order"], 0)
                        });
                    }
                }
                
                //----------------------------------------------------------------------------------------------------------
            }
            return l_localeFields;
        }


        public string Phrase
        {
            get { return GetField(PhraseField); }
        }

        public string BtnSkip
        {
            get { return GetField(BtnSkipField); }
        }

        public string BtnNext
        {
            get { return GetField(BtnNextField); }
        }

        public string BtnBack
        {
            get { return GetField(BtnBackField); }
        }

        public string BtnMoreCandidates
        {
            get { return GetField(BtnMoreCandidatesField); }
        }

        public string BtnMoreResults
        {
            get { return GetField(BtnMoreResultsField); }
        }

        public string LblContestCount
        {
            get { return GetField(LblContestCountField); }
        }

        public string BtnReview
        {
            get { return GetField(BtnReviewField); }
        }

        public string LblContestTip
        {
            get { return GetField(LblContestTipField); }
        }

        public string PropositionTip
        {
            get { return GetField(PropositionTipField); }
        }

        public string LblMulti0Choices2
        {
            get { return GetField(LblMulti0Choices2Field); }
        }

        public string LblChoice1
        {
            get { return GetField(LblChoice1Field); }
        }

        public string LblChoice2
        {
            get { return GetField(LblChoice2Field); }
        }

        public string BtnBackToVote
        {
            get { return GetField(BtnBackToVoteField); }
        }

        public string BtnResetBallot
        {
            get { return GetField(BtnResetBallotField); }
        }

        public string BtnAcceptPrint
        {
            get { return GetField(BtnAcceptPrintField); }
        }

        public string BtnAcceptSave
        {
            get { return GetField(BtnAcceptSaveField); }
        }

        public string BtnCastVote
        {
            get { return GetField(BtnCastVoteField); }
        }

        public string LblDone
        {
            get { return GetField(LblDoneField); }
        }

        public string LblThankyou
        {
            get { return GetField(LblThankyouField); }
        }

        public string LblReviewTitle
        {
            get { return GetField(LblReviewTitleField); }
        }

        public string LblReviewTip
        {
            get { return GetField(LblReviewTipField); }
        }

        public string LblUnderVoteMsg
        {
            get { return GetField(LblUnderVoteMsgField); }
        }

        public string LblSpeechRateDesc
        {
            get { return GetField(LblSpeechRateDescField); }
        }

        public string LblSettingToolsDesc
        {
            get { return GetField(LblSettingToolsDescField); }
        }

        public string NoSelection
        {
            get { return GetField(NoSelectionField); }
        }

        public string Voter
        {
            get { return GetField(VoterField); }
        }

        public string Machine
        {
            get { return GetField(MachineField); }
        }

        public string Void
        {
            get { return GetField(VoidField); }
        }

        public string LblConfirmTitle
        {
            get { return GetField(LblConfirmTitleField); }
        }

        public string LblConfirmTip
        {
            get { return GetField(LblConfirmTipField); }
        }

        public string RaceTip
        {
            get { return GetField(RaceTipField); }
        }

        public string PropositionFor
        {
            get { return GetField(PropositionForField); }
        }

        public string PropositionAgainst
        {
            get { return GetField(PropositionAgainstField); }
        }

        public string PropositionYes
        {
            get { return GetField(PropositionYesField); }
        }

        public string PropositionNo
        {
            get { return GetField(PropositionNoField); }
        }

        public string IncumbentFootnote
        {
            get { return GetField(IncumbentFootnoteField); }
        }

        public string PropositionSubheading
        {
            get { return GetField(PropositionSubheadingField); }
        }

        public string WriteinHelp1
        {
            get { return GetField(WriteinHelp1Field); }
        }

        public string WriteinHelp2
        {
            get { return GetField(WriteinHelp2Field); }
        }

        public string WriteinHelp3
        {
            get { return GetField(WriteinHelp3Field); }
        }

        public string WriteinHelp4
        {
            get { return GetField(WriteinHelp4Field); }
        }

        public string WriteinHelp5
        {
            get { return GetField(WriteinHelp5Field); }
        }

        public string BtnAccept
        {
            get { return GetField(BtnAcceptField); }
        }

        public string BtnCancel
        {
            get { return GetField(BtnCancelField); }
        }

        public string BtnAddCandidates
        {
            get { return GetField(BtnAddCandidatesField); }
        }

        public string BtnAudio
        {
            get { return GetField(BtnAudioField); }
        }

        public string BtnTextSize
        {
            get { return GetField(BtnTextSizeField); }
        }

        public string BtnLanguage
        {
            get { return GetField(BtnLanguageField); }
        }

        public string BtnHelp
        {
            get { return GetField(BtnHelpField); }
        }

        public string BtnRemotePollWorker
        {
            get { return GetField(BtnRemotePollWorkerField); }
        }

        public string BtnSettingTools
        {
            get { return GetField(BtnSettingToolsField); }
        }

        public string BtnCallHelp
        {
            get { return GetField(BtnCallHelpField); }
        }

        public string MinRaceTip
        {
            get { return GetField(MinRaceTipField); }
        }

        public string PartyTip
        {
            get { return GetField(PartyTipField); }
        }

        public string CountyTip
        {
            get { return GetField(CountyTipField); }
        }

        public string SlateTip
        {
            get { return GetField(SlateTipField); }
        }

        public string BtnBackVoteSlate
        {
            get { return GetField(BtnBackSlateField); }
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
                if (_localeFields != null)
                {
                    foreach (LocaleField lField in _localeFields.Values)
                    {
                        lField.Dispose();
                    }
                    _localeFields.Clear();
                    _localeFields = null;
                }
            }
        }

        public string GetField(string btnAddCandidatesField)
        {
            LocaleField field;

            if (_localeFields == null)
            {
                return null;
            }
            //if(_localeFields.Count == 0)
            //{
            //    _localeFields = GetLocaleFields_loc(AppManager.Instance.Session.CurrentLocale);
            //}

            _localeFields.TryGetValue(btnAddCandidatesField, out field);
            return field == null ? null : field.FieldValue;
        }

        private List<LocaleField> LocaleFields
        {
            get
            {
                if (_localeFields == null)
                    _localeFields = new Dictionary<string, LocaleField>();
                return new List<LocaleField>(_localeFields.Values);
            }
        }

        public override string ToString()
        {
            return Name;
        }

        public static List<Locale> FetchLocales(int ballotId, bool pIsAll)
        {
            var locales = new List<Locale>();
            try
            {
                var dtLocale =
                    DataManager.VotingContentDataInstance.GetDataV2(
                        "SELECT l.locale_id, l.locale_name, l.locale_code, bl.locale_id AS use_flag " +
                        "FROM locale l " +
                        "LEFT OUTER JOIN ballot_locale bl ON l.locale_id = bl.locale_id AND bl.ballot_id = @ballotid",
                        new List<SQLiteParameter>() { new SQLiteParameter("@ballotid", ballotId) });
                if (dtLocale != null)
                {
                    if (dtLocale.Rows.Count > 0)
                    {
                        locales.AddRange(from DataRow dataRow in dtLocale.Rows
                                         select new Locale
                                         {
                                             Id = Helper.Cast(dataRow[IdField], -1),
                                             Name = Helper.GetStringValue(dataRow[NameField]),
                                             Code = Helper.GetStringValue(dataRow[CodeField]),
                                             UseFlag = !(dataRow["use_flag"] is DBNull)
                                         });

                        //_locales.Sort(new Comparison<Locale>(LocalesComparer));
                    }
                }
            }
            catch (Exception e)
            {
                Logger.Instance.Write(e);
            }

            return locales;
        }

        public static List<Locale> FetchLocales(int ballotId)
        {
            var locales = new List<Locale>();
            try
            {
                var dtLocale =
                    DataManager.VotingContentDataInstance.GetDataV2(
                        "select l.locale_id, l.locale_name, l.locale_code from locale l " +
                        "JOIN ballot_locale bl ON l.locale_id = bl.locale_id AND bl.ballot_id = @ballotid " +
                        "order by l.locale_id", new List<SQLiteParameter> { new SQLiteParameter("@ballotid", ballotId) });
                var fieldSql =
                    "select locale_id, field_name, field_value, field_title, show_order from locale_fields where locale_id in (";
                var sqlParameters = new List<SQLiteParameter>();
                if (dtLocale != null && dtLocale.Rows.Count > 0)
                {
                    var idx = 0;
                    foreach (DataRow dataRow in dtLocale.Rows)
                    {
                        var locale = new Locale
                        {
                            Id = Helper.Cast(dataRow[Locale.IdField], -1),
                            Name = Helper.GetStringValue(dataRow[Locale.NameField]),
                            Code = Helper.GetStringValue(dataRow[Locale.CodeField]),
                            UseFlag = true
                        };
                        var paramName = "@locid" + (idx++);
                        fieldSql += (idx == 1 ? "" : ", ") + paramName;
                        sqlParameters.Add(new SQLiteParameter(paramName, locale.Id));
                        locales.Add(locale);
                    }
                    fieldSql += ") order by locale_id";
                    var dtLocaleFields = DataManager.VotingContentDataInstance.GetDataV2(fieldSql, sqlParameters);
                    if (dtLocaleFields != null && dtLocaleFields.Rows.Count > 0)
                    {
                        var fields = new Dictionary<int, Dictionary<string, LocaleField>>();
                        foreach (DataRow dataRow in dtLocaleFields.Rows)
                        {
                            var localeId = Helper.Cast(dataRow["locale_id"], 0);
                            var localeField = new LocaleField
                            {
                                LocaleId = localeId,
                                FieldName = dataRow["field_name"] as string,
                                FieldValue = dataRow["field_value"] as string,
                                FieldTitle = dataRow["field_title"] as string,
                                ShowOrder = Helper.Cast(dataRow["show_order"], 0)
                            };
                            Dictionary<string, LocaleField> dict;
                            fields.TryGetValue(localeId, out dict);
                            if (dict == null)
                                dict = new Dictionary<string, LocaleField>();
                            dict[localeField.FieldName] = localeField;
                            fields[localeId] = dict;
                        }
                        foreach (var locale in locales)
                        {
                            locale._localeFields = fields[locale.Id];
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Logger.Instance.Write(e);
            }

            return locales;
        }

        public static List<Locale> FetchUsedLocales(int ballotId)
        {
            return FetchLocales(ballotId);
        }

        public static List<Locale> FetchUsedLocales(out int defaultLocaleIndex, int ballotId)
        {
            var locales = FetchUsedLocales(ballotId);
            defaultLocaleIndex = locales.FindIndex(l => IsDefaultLocale(l.Name));

            return locales;
        }

        public static bool IsDefaultLocale(Locale locale)
        {
            return IsDefaultLocale(locale.Name);
        }

        public static bool IsDefaultLocale(string localeName)
        {
            return string.Compare(localeName, Locale.DefaultLanguage, StringComparison.InvariantCultureIgnoreCase) == 0;
        }

        public static Locale GetLocale(int? id, string name, int ballotId)
        {
            Locale locale = null;

            try
            {
                var sql = "select l.locale_id, l.locale_name, l.locale_code, bl.locale_id AS use_flag from locale l " +
                          "left join ballot_locale bl ON l.locale_id = bl.locale_id " +
                          "WHERE bl.ballot_id = @ballotid ";
                var sqlParameters = new List<SQLiteParameter> { new SQLiteParameter("@ballotid", ballotId) };
                if (id != null)
                {
                    sql += " AND l.locale_id = @locid";
                    sqlParameters.Add(new SQLiteParameter("@locid", id));
                }
                if (name != null)
                {
                    sql += " AND l.locale_name = @locname";
                    sqlParameters.Add(new SQLiteParameter("@locname", name));
                }
                var dtLocale = DataManager.VotingContentDataInstance.GetDataV2(sql, sqlParameters);
                if (dtLocale == null || dtLocale.Rows.Count == 0)
                {
                    return locale;
                }

                var dataRow = dtLocale.Rows[0];
                locale = new Locale
                {
                    Id = Helper.Cast(dataRow[Locale.IdField], -1),
                    Name = Helper.GetStringValue(dataRow[Locale.NameField]),
                    UseFlag = !(dataRow["use_flag"] is DBNull),
                    Code = Helper.GetStringValue(dataRow[Locale.CodeField])
                };
                
                const string fieldsSql =

                    "select locale_id, field_name, field_value, field_title, show_order from locale_fields where locale_id = @locid order by show_order";
                sqlParameters.Clear();
                sqlParameters.Add(new SQLiteParameter("@locid", locale.Id));
                dtLocale = DataManager.VotingContentDataInstance.GetDataV2(fieldsSql, sqlParameters);

                if (dtLocale != null && dtLocale.Rows.Count > 0)
                {
                    locale.LocaleFields.AddRange(from DataRow dtRow in dtLocale.Rows
                                                 select new LocaleField
                                                 {
                                                     LocaleId = locale.Id,
                                                     FieldName = dtRow["field_name"] as string,
                                                     FieldValue = dtRow["field_value"] as string,
                                                     FieldTitle = dtRow["field_title"] as string,
                                                     ShowOrder = Helper.Cast(dtRow["show_order"], 0)
                                                 });
                }

                return locale;
            }
            catch (Exception e)
            {
                Logger.Instance.Write(e);
            }

            return locale;
        }

        public static Locale GetDefaultLocale(int ballotId)
        {
            // TODO
            // Retrieve id of the default locale and pass it to GetLocale instead of null
            string sql = string.Format("select locale_id from locale where locale_name='{0}'", DefaultLanguage);
            Int64 localeId = (Int64)DataManager.VotingContentDataInstance.GetScalarData(sql);
            return GetLocale((int?)localeId, DefaultLanguage, ballotId);
        }

        public string BtnPrintReview 
        {
            get { return GetField(BtnPrintReviewField); }
        }
    }
}
