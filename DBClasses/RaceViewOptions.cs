using System;
using System.Data;
using System.Data.SQLite;
using System.Reflection;
using System.ComponentModel;
using System.Collections.Generic;
using System.Text;
using VotRite;
using VotRite.Util;

namespace VotRiteBallotDataManager.AppCode
{
    class RaceViewOptions
    {
        public const string TableName = "race_view_options";
        public const string RaceViewOptionsIdField = "rvo_id";
        public const string ContestIdField = "contest_id";
        public const string UseDefaultSettingsField = "rvo_def_settings";
        public const string CandidateAtRowField = "rvo_cand_at_row";
        public const string RowsToScrollField = "rvo_rows_to_scroll";
        public const string FontSizeField = "rvo_font_size";
        public const string ScrollStepField = "rvo_scroll_step";
        public const string RowHeightField = "rvo_row_height";
        public const string ColumnCountField = "rvo_col_count";

        public const string UseDefaultContestSizeSettingsField = "rvo_def_contest_size_settings";
        public const string ContestWidthField = "rvo_contest_width";
        public const string ContestHeightField = "rvo_contest_height";
        public const string ContestTopField = "rvo_contest_top";
        public const string ContestLeftField = "rvo_contest_left";

        [DefaultValue(null)]
        public int? RaceViewOptionsId { get; private set; }
        [DefaultValue(null)]
        public int? ContestId { get; private set; }

        [DefaultValue(true)]
        public bool UseDefaultSettings { get; set; }
        [DefaultValue(false)]
        public bool CandidateAtRow { get; set; }

        [DefaultValue(7)]
        public int RowsToScroll { get; set; }
        [DefaultValue(18)]
        public int FontSize { get; set; }
        [DefaultValue(30)]
        public int ScrollStep { get; set; }
        [DefaultValue(60)]
        public int RowHeight { get; set; }
        [DefaultValue(2)]
        public int ColumnCount { get; set; }

        [DefaultValue(true)]
        public bool UseDefaultContestSizeSettings { get; set; }
        [DefaultValue(780)]
        public int ContestWidth { get; set; }
        [DefaultValue(650)]
        public int ContestHeight { get; set; }
        [DefaultValue(220)]
        public int ContestTop { get; set; }
        [DefaultValue(470)]
        public int ContestLeft { get; set; }

        public RaceViewOptions(int? contestId)
        {
            ResetPropsUsingDefaultAttributes(false);
            if (contestId == null) return;
            ContestId = contestId;
            Load();
        }

        private void Load()
        {
            try
            {
                var dtROV = DataManager.VotingContentDataInstance.GetDataV2(string.Format("SELECT * FROM {0} WHERE {1}={2}",
                                                                                RaceViewOptions.TableName,
                                                                                RaceViewOptions.ContestIdField,
                                                                                ContestId));
                if (dtROV != null)
                {
                    if (dtROV.Rows.Count > 0)
                    {
                        var dataRow = dtROV.Rows[0];
                        RaceViewOptionsId = Helper.Cast(dataRow[RaceViewOptions.RaceViewOptionsIdField], -1);
                        UseDefaultSettings = Helper.Cast(dataRow[RaceViewOptions.UseDefaultSettingsField], true);
                        CandidateAtRow = Helper.Cast(dataRow[RaceViewOptions.CandidateAtRowField], false);
                        RowsToScroll = Helper.Cast(dataRow[RaceViewOptions.RowsToScrollField], 7);
                        FontSize = Helper.Cast(dataRow[RaceViewOptions.FontSizeField], 18);
                        ScrollStep = Helper.Cast(dataRow[RaceViewOptions.ScrollStepField], 30);
                        RowHeight = Helper.Cast(dataRow[RaceViewOptions.RowHeightField], 60);
                        ColumnCount = Helper.Cast(dataRow[RaceViewOptions.ColumnCountField], 2);
                        UseDefaultContestSizeSettings = Helper.Cast(dataRow[RaceViewOptions.UseDefaultContestSizeSettingsField], true);
                        ContestHeight = Helper.Cast(dataRow[RaceViewOptions.ContestHeightField], 650);
                        ContestWidth = Helper.Cast(dataRow[RaceViewOptions.ContestWidthField], 780);
                        ContestTop = Helper.Cast(dataRow[RaceViewOptions.ContestTopField], 290);
                        ContestLeft = Helper.Cast(dataRow[RaceViewOptions.ContestLeftField], 470);
                    }
                }
            }
            catch (Exception e) { Logger.Instance.Write(e); }
        }

        private int ResetPropsUsingDefaultAttributes(bool initInheritedProperties)
        {
            //Object oThis = this;
            int count = 0;
            //BindingFlags.Static
            Type oType = this.GetType();
            PropertyInfo[] infos = oType.GetProperties(BindingFlags.NonPublic |
                           BindingFlags.Public | BindingFlags.Instance);
            foreach (PropertyInfo inf in infos)
            {
                if (initInheritedProperties || inf.DeclaringType.Equals(oType))
                {
                    object[] oDefAtts = inf.GetCustomAttributes(
                      typeof(DefaultValueAttribute), initInheritedProperties);
                    if (oDefAtts.Length > 0)
                    {
                        DefaultValueAttribute defAtt =
                             oDefAtts[oDefAtts.Length - 1] as DefaultValueAttribute;
                        if (defAtt != null && defAtt.Value != null &&
                            !defAtt.Value.Equals(inf.GetValue(this,
                             BindingFlags.GetProperty, null, null, null)))
                        {
                            inf.SetValue(this, defAtt.Value, BindingFlags.SetProperty,
                                         null, null, null);
                            count++;
                        }
                    }
                }
            }
            return count;
        }

    }
}
