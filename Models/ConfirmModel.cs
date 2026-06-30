// Product:	VotRite
// Module:  ConfirmModel.cs
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
using VotRite.Util;

namespace VotRite.Models
{
    class ConfirmModel : ScreenModel
    {
        private Hashtable vars = null;
        private Session session = null;
        private BallotDefinition ballot = null;

        public Hashtable Vars
        {
            get { return vars; }
        }

        public BallotDefinition Ballot { get { return ballot; } }

        public Session Session { get { return session; } }

        public ConfirmModel()
        {
            vars = new Hashtable();
            session = AppManager.Instance.Session;
            ballot = session.Ballot;

            Initialize();
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                vars.Clear();
                session.Dispose();
                ballot.Dispose();
            }

            base.Dispose(disposing);
        }

        private void Initialize()
        {
            vars.Add("back", session.CurrentLocale.BtnBack);
            vars.Add("back_visible", session.CheckBackButtonState());
            vars.Add("print_review", session.CurrentLocale.BtnPrintReview);
            vars.Add("cast_vote", session.CurrentLocale.BtnCastVote);
            vars.Add("more", session.CurrentLocale.BtnMoreResults);
            vars.Add("ballot_name", ballot.Name);
            vars.Add("ballot_title", ballot.Title);
            vars.Add("ballot_address", ballot.Address);
            vars.Add("ballot_date", ballot.Date);
            vars.Add("group_name", string.Empty);
            vars.Add("confirm_title", session.CurrentLocale.LblConfirmTitle);
			vars.Add("confirm_tip", session.CurrentLocale.LblConfirmTip);
			vars.Add("no_selection", session.CurrentLocale.NoSelection);

            //Jim Adding Codes -- Begin
            vars.Add("board", session.Ballot.Board);
            vars.Add("address", session.Ballot.Address);
            vars.Add("ballot", session.Ballot.Name);
            vars.Add("date", session.Ballot.Date);
            vars.Add("voter", session.CurrentLocale.Voter + ": " + Convert.ToString(session.Id));
            vars.Add("machine", session.CurrentLocale.Machine);
            vars.Add("btn_setting_tools", session.CurrentLocale.BtnSettingTools);
            vars.Add("btn_callhelp", session.CurrentLocale.BtnCallHelp);

            //Jim Adding Codes -- End

			string currentKey;
			foreach(ContestDefinition contest in ballot.ContestsList) {
				currentKey = "$b" + ballot.Id + "_g" + contest.Group + "_name";
				if(!vars.Contains(currentKey))
					vars.Add(currentKey, contest.GroupName);

				currentKey = "$b" + ballot.Id + "_c" + contest.Id + "_name";
				if(!vars.Contains(currentKey))
					vars.Add(currentKey, contest.Name);

				foreach(DataDefinition data in contest.Data) {
					currentKey = "$b" + ballot.Id + "_c" + contest.Id + "_g" + contest.Group + "_d" + data.Id;
					if(!vars.Contains(currentKey))
						vars.Add(currentKey, data.Text);
//					data.Tag = data.Text;
				}
			}
        }

        public ConfirmView GetView()
        {
            ConfirmView v = null;

            foreach (IObserver obs in observers)
            {
                if (obs.GetType().ToString() == "VotRite.Views.ConfirmView")
                {
                    v = (ConfirmView)obs;
                    break;
                }
            }

            return v;
        }
    }
}
