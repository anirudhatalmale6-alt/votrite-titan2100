// Product:	VotRite
// Module:  ReviewModel.cs
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
using System.Linq;
using VotRite.MVC;
using VotRite.Definition;
using VotRite.Views;
using VotRite.Util;

namespace VotRite.Models
{
    class ReviewModel : ScreenModel
    {
        private Hashtable vars;
        private BallotDefinition ballot;

        public Hashtable Vars
        {
            get { return vars; }
        }

        public BallotDefinition Ballot { get { return ballot; } }

        public Session Session { get; private set; }

        public ReviewModel()
        {
            vars = new Hashtable();
            Session = AppManager.Instance.Session;
            ballot = Session.Ballot;

            Initialize();
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                vars.Clear();
                ballot.Dispose();
                Session.Dispose();
            }
            base.Dispose(disposing);
        }

        private void Initialize()
        {
            vars.Add("back_to_vote", Session.CurrentLocale.BtnBackToVote);

            /// Jim Kapsis 18-06-2016
            /// Printer Settings
            /// START
            if (AppManager.PrinterEnabled)
                vars.Add("accept_print", Session.CurrentLocale.BtnAcceptPrint);
            else
                vars.Add("accept_print", Session.CurrentLocale.BtnAcceptSave);
            /// END
			
            vars.Add("more", Session.CurrentLocale.BtnMoreResults);
            vars.Add("ballot_name", ballot.Name);
            vars.Add("ballot_title", ballot.Title);
            vars.Add("ballot_address", ballot.Address);
            vars.Add("ballot_date", ballot.Date);
            vars.Add("group_name", string.Empty);
            vars.Add("review_title", Session.CurrentLocale.LblReviewTitle);
            vars.Add("review_tip", Session.CurrentLocale.LblReviewTip);
            vars.Add("no_selection", Session.CurrentLocale.NoSelection);
            vars.Add("reset_ballot", Session.CurrentLocale.BtnResetBallot);
            vars.Add("back_to_vote_slate", Session.CurrentLocale.BtnBackVoteSlate);
            vars.Add("undervote_msg", Session.CurrentLocale.LblUnderVoteMsg);
            vars.Add("btn_setting_tools", Session.CurrentLocale.BtnSettingTools);
            vars.Add("btn_callhelp", Session.CurrentLocale.BtnCallHelp);
            
			string currentKey;
            foreach (ContestDefinition contest in ballot.ContestsList)
            {
				currentKey = "$b" + ballot.Id + "_g" + contest.Group + "_name";
				if(!vars.Contains(currentKey))
					vars.Add(currentKey, contest.GroupName);

				currentKey = "$b" + ballot.Id + "_c" + contest.Id + "_name";
				if(!vars.Contains(currentKey))
					vars.Add(currentKey, contest.Name);

                foreach (DataDefinition data in contest.Data)
                {
					currentKey = "$b" + ballot.Id + "_c" + contest.Id + "_g" + contest.Group + "_d" + data.Id;
					if(!vars.Contains(currentKey))
						vars.Add(currentKey, data.Text);
//					data.Tag = data.Text;
                }
            }
        }

        public ReviewView GetView()
        {
            return observers.OfType<ReviewView>().FirstOrDefault();
        }
    }
}
