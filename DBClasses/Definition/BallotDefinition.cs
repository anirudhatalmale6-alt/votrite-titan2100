// Product:	VotRite
// Module:  BallotDefinition.cs
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
using System.Collections.Generic;
using VotRiteBallotDataManager.AppCode;

namespace VotRite.Definition
{
    /// <summary>
    /// BallotDefinition class
    /// Represents a ballot structure
    /// Implemented as singleton
    /// </summary>
    class BallotDefinition: IDisposable
    {
		public enum BallotTextSize { SMALL = 12, NORMAL = 18, LARGE = 24 }        
		
        //  Members
		private int id;	//	ballot id
        private string name;    //  ballot name
        private string board;   //  board name
        private string address; //  board address
        private int countyId; // county id
        private string countyName; // county
		private string logo;	//	board logo
        private string date;  //  current date
        private BallotTextSize textSize;
        private int contestPageSize;    //  number of selection displayed
        private List<ContestDefinition> contest;    //  contest instances
        private int activeContest;  //  current contest
        
        //  Set of getters/setters for members
        public int Id { get { return id; } set { id = value; } }
		public string Name { get { return name; } set { name = value; } }
        public string Board { get { return board; } set { board = value; } }
        public string Address { get { return address; } set { address = value; } }
        public string CountyName { get { return countyName; } set { countyName = value; } }
        public int CountyId { get { return countyId; } set { countyId = value; } }
        public string Logo { get { return logo; } set { logo = value; } }
        public string Date { get { return date; } set { date = value; } }
        public ElectionTypes ElectionType { get; set; }
        public string Title {get;set;}
        public int? ReportLocaleId { get; set; }
        public bool HasParty { get; set; }
        public bool HasCounties { get; set; }
        public bool HasSlates { get; set; }
        public bool HasOverview { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public int? PartyId { get; set; }
        public bool TestMode { get; set; }
        public bool ShowBallotOverview { get; set; }
        public bool ShowSettingsAndTools { get; set; }

        public BallotTextSize TextSize
        {
            get { return textSize; } set { textSize = value; }
        }
        public int ContestPageSize
        {
            get { return contestPageSize; }
            set { contestPageSize = value; }
        }
        public List<ContestDefinition> ContestsList
        {
            get { return contest; } 
            set { contest = value; }
        }
        public int ActiveContest
        {
            get { return activeContest; }
            set { activeContest = value; }
        }

        public Session.BallotModes BallotMode { get; set; }

        public SlatesDefinition slatesDefinition { get; set; }

        public PartyDefinition partiesDefinition { get; set; }

        public CountyDefinition countiesDefinition { get; set; }

        /// <summary>
        /// Default ctor
        /// </summary>
        public BallotDefinition()
        {
            ElectionType = ElectionTypes.standard;
            contest = new List<ContestDefinition>();
            slatesDefinition = new SlatesDefinition();
            partiesDefinition = new PartyDefinition();
            countiesDefinition = new CountyDefinition();
            //activeContest = -1;
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
                if (partiesDefinition != null) partiesDefinition.Dispose();
                if (countiesDefinition != null) countiesDefinition.Dispose();
                if (slatesDefinition != null)
                {
                    slatesDefinition.Dispose();
                }
                if (contest != null)
                {
                    contest.ForEach(cont => cont.Dispose());
                    contest.Clear();
                }                
            }
        }
    }
}
