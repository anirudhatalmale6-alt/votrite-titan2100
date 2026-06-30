// Product:	VotRite
// Module:  ContestModel.cs
// Author:  Dmitriy Slipak

// 
// Copyright (c) 2017 - VOTRITE INTERNATIONAL LLC. All rights reserved.
// 
// THIS PRODUCT INCLUDES SOFTWARE AS A PART OF VOTRITE VOTING 
// SYSTEM DEVELOPED AT VOTRITE INTERNATIONAL LLC (http://www.votrite.com).
// THE SOURCE CODE FOR THIS PRODUCT IS SUBJECT TO THE VOTRITE INTERNATIONAL LLC 
// LICENSE. NO ANY PORTION OF THIS PRODUCT OR SOURCE CODE CAN BE 
// REDISTRIBUTED UNDER ANY CIRCUMSTANCES.
// 
using System;
using System.Collections;
using VotRite.MVC;
using VotRite.Definition;
using VotRite.Views;
using VotRiteBallotDataManager.AppCode;

using System.Data;

namespace VotRite.Models
{
    class ContestModel : ScreenModel
    {
        private Hashtable vars;
        private Session session;
        private BallotDefinition ballot;
        private ContestDefinition contest;
        private Race currentRaceInstance = null;
        private Proposition currentPropositionInstance = null;        

        public Hashtable Vars
        {
            get { return vars; }
        }

        public BallotDefinition Ballot { get { return ballot; } }

        public Session Session { get { return session; } }

        public ContestDefinition Contest { get { return contest; } }

        public Proposition Proposition
        {
            get { return currentPropositionInstance; }
        }

        public ContestModel()
        {
            vars = new Hashtable();
            session = AppManager.Instance.Session;
            ballot = session.Ballot;

            //Jim Code -- Begin
            DataTable tbCurrent = GetFitOrderDataRec();

            int iLoc = ballot.ActiveContest;

            if (tbCurrent.Rows.Count > 0)
            {
                try
                {
                    iLoc = int.Parse(tbCurrent.Rows[ballot.ActiveContest]["ID"].ToString());
                } catch (Exception e)
                {
                    Console.WriteLine(e);
                }
            }
            contest = ballot.ContestsList[iLoc];
            ////Jim Code -- End
            
            ContestDefinition.SetContestFields(ref contest, ref currentRaceInstance, ref currentPropositionInstance, session.CurrentLocale, ballot.PartyId);
            ContestDefinition.FillContestData(ref contest, currentPropositionInstance, session.CurrentLocale);

            Initialize();
        }

        private DataTable GetFitOrderDataRec()
        {
            DataTable tbCurrent = new DataTable();
            tbCurrent.Columns.Add(new DataColumn("ID", typeof(System.Int32)));
            tbCurrent.Columns.Add(new DataColumn("PosID", typeof(System.Int32)));

            string query = string.Format("SELECT contest_id, rvo_order_position FROM {0} ",
                "race_view_options");

            if (ballot.HasCounties)
            {
                query = string.Format("SELECT rvo.contest_id, rvo.rvo_order_position FROM race_view_options rvo " +
                    "inner join contest c on rvo.contest_id = c.contest_id " +
                    "where c.county_id={0}", session.Ballot.CountyId);
            }

            DataTable tbPosition = DataManager.VotingContentDataInstance.GetDataV2(query);

            //int iLoc = -1;
            int iPosition = 99;
            for (int i = 0; i < ballot.ContestsList.Count; i++)
            {
                int iTmpID = ballot.ContestsList[i].Id;
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
                vars.Clear();
                session.Dispose();
                ballot.Dispose();
                contest.Dispose();

                if (currentRaceInstance != null) currentRaceInstance.Dispose();
                if (currentPropositionInstance != null) currentPropositionInstance.Dispose();
            }
            base.Dispose(disposing);
        }

        private void Initialize()
        {
            vars.Add("review", session.CurrentLocale.BtnReview);
            vars.Add("back", session.CurrentLocale.BtnBack);
            vars.Add("next", session.CurrentLocale.BtnNext);
            vars.Add("skip", session.CurrentLocale.BtnSkip);
            vars.Add("more", session.CurrentLocale.BtnMoreCandidates);
            vars.Add("ballot_board", ballot.Board);
            vars.Add("ballot_name", ballot.Name);
            vars.Add("ballot_title", ballot.Title);
            //vars.Add("ballot_title", AppManager.Instance.GetVar("$b" + ballot.Id + "_title"));
            vars.Add("ballot_address", ballot.Address);
            vars.Add("ballot_date", ballot.Date);
            vars.Add("group_name", contest.GroupName);
            vars.Add("contest_name", contest.Name);

            vars.Add("contest_title", contest.Title);
            vars.Add("min_race_tip", session.CurrentLocale.MinRaceTip);
            vars.Add("lbl_multi_0_choices2", session.CurrentLocale.LblMulti0Choices2);
            vars.Add("lbl_choice1", session.CurrentLocale.LblChoice1);
            vars.Add("lbl_choice2", session.CurrentLocale.LblChoice2);
            vars.Add("contest_tip", contest.Tip);
            vars.Add("contest_count", session.CurrentLocale.LblContestCount);
            vars.Add("contest_footnote", contest.Footnote);
            vars.Add("btn_add_candidates", session.CurrentLocale.BtnAddCandidates);
            vars.Add("btn_setting_tools", session.CurrentLocale.BtnSettingTools);
            vars.Add("btn_callhelp", session.CurrentLocale.BtnCallHelp);

            int i = 1;

            foreach (DataDefinition data in contest.Data)
            {
                vars.Add("contest_data_" + i, data.Text);
                i++;
            }

            //session.ballot.Board = (string)vars["ballot_board"];
            //session.ballot.Name = (string)vars["ballot_name"];// +"\n" + (string)vars["ballot_title"];
            //session.ballot.Address = (string)vars["ballot_address"];
        }

        public ContestView GetView()
        {
            ContestView v = null;

            foreach (IObserver obs in observers)
            {
                if (obs.GetType().ToString() == "VotRite.Views.ContestView")
                {
                    v = (ContestView)obs;
                    break;
                }
            }

            return v;
        }
    }
}
