using System.Collections;
using System.Data;
using System;
using VotRite.Definition;
using VotRiteBallotDataManager.AppCode;

namespace VotRite.Models
{
    internal class WriteinModel : ScreenModel
    {
        private readonly BallotDefinition _ballot;
        private readonly ContestDefinition _contest;

        public Hashtable Vars { get; private set; }

        public BallotDefinition Ballot
        {
            get { return _ballot; }
        }

        public Session Session { get; private set; }

        public WriteinModel()
        {
            Vars = new Hashtable();
            Session = AppManager.Instance.Session;
            _ballot = Session.Ballot;

            DataTable tbCurrent = GetFitOrderDataRec();

            int iLoc = _ballot.ActiveContest;

            if (tbCurrent.Rows.Count > 0)
            {
                try
                {
                    iLoc = int.Parse(tbCurrent.Rows[_ballot.ActiveContest]["ID"].ToString());
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                }
            }

            // _contest = _ballot.ContestsList[_ballot.ActiveContest];
            _contest = _ballot.ContestsList[iLoc];

            Initialize();
        }

        private DataTable GetFitOrderDataRec()
        {
            DataTable tbCurrent = new DataTable();
            tbCurrent.Columns.Add(new DataColumn("ID", typeof(System.Int32)));
            tbCurrent.Columns.Add(new DataColumn("PosID", typeof(System.Int32)));

            string query = string.Format("SELECT contest_id, rvo_order_position FROM {0} ",
                "race_view_options");

            if (_ballot.HasCounties)
            {
                query = string.Format("SELECT rvo.contest_id, rvo.rvo_order_position FROM race_view_options rvo " +
                    "inner join contest c on rvo.contest_id = c.contest_id " +
                    "where c.county_id={0}", _ballot.CountyId);
            }

            DataTable tbPosition = DataManager.VotingContentDataInstance.GetDataV2(query);

            //int iLoc = -1;
            int iPosition = 99;
            for (int i = 0; i < _ballot.ContestsList.Count; i++)
            {
                int iTmpID = _ballot.ContestsList[i].Id;
                DataRow[] rowsTmp2 = tbPosition.Select("contest_id=" + iTmpID.ToString());
                if (rowsTmp2.Length > 0)
                {
                    ////if (iPosition > int.Parse(rowsTmp2[0]["rvo_order_position"].ToString()))
                    ////{
                    ////    iLoc = i;
                    iPosition = int.Parse(rowsTmp2[0]["rvo_order_position"].ToString());
                    tbCurrent.Rows.Add(new object[] { i, iPosition });
                    ////}
                }
            }

            tbCurrent.DefaultView.Sort = "PosID";
            //////DataTable tbRet = tbCurrent.DefaultView.ToTable();
            //////for (int i = 0; i < tbRet.Rows.Count; i++)
            //////{
            //////    tbRet.Rows[i]["ID"] = i;
            //////}

            return tbCurrent.DefaultView.ToTable();
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                Vars.Clear();
                Session.Dispose();
                _ballot.Dispose();
                _contest.Dispose();
            }
            base.Dispose(disposing);
        }

        private void Initialize()
        {
            Vars.Add("ballot_name", _ballot.Name);
            Vars.Add("ballot_title", _ballot.Title);
            Vars.Add("ballot_address", _ballot.Address);
            Vars.Add("ballot_date", _ballot.Date);
            Vars.Add("contest_name", _contest.Name);
            Vars.Add(Locale.WriteinHelp1Field, Session.CurrentLocale.WriteinHelp1);
            Vars.Add(Locale.WriteinHelp2Field, Session.CurrentLocale.WriteinHelp2);
            Vars.Add(Locale.WriteinHelp3Field, Session.CurrentLocale.WriteinHelp3);
            Vars.Add(Locale.WriteinHelp4Field, Session.CurrentLocale.WriteinHelp4);
            Vars.Add(Locale.WriteinHelp5Field, Session.CurrentLocale.WriteinHelp5);
            Vars.Add(Locale.BtnAcceptField, Session.CurrentLocale.BtnAccept);
            Vars.Add(Locale.BtnCancelField, Session.CurrentLocale.BtnCancel);
            Vars.Add("contest_count", Session.CurrentLocale.LblContestCount);
        }
	}
}
