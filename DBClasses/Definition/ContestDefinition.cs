// Product:	VotRite
// Module:  ContestDefinition.cs
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
using System.Linq;
using VotRiteBallotDataManager.AppCode;
using VotRite.DBClasses;

namespace VotRite.Definition
{
    class ContestDefinition : IDisposable
    {
        private int group;
        private string groupName;
        private int id;
        private string name;
        private string title;
        private int selected;
        //private int type;        
        private int activePage;
        private int lastSelectedIndex;
        private int countyId;

        public int Group { get { return group; } set { group = value; } }
        public string GroupName
        {
            get { return groupName; }
            set { groupName = value; }
        }
        public int Id { get { return id; } set { id = value; } }
        public string Name { get { return name; } set { name = value; } }
        public string Title { get { return title; } set { title = value; } }
        public string Choice { get; set; }
        public string Tip { get; set; }
        public int Selected
        {
            get { return selected; }
            set { selected = value; }
        }

        public int MinSelection { get; set; }
        public int MaxSelection { get; set; }
        public int MaxWriteins { get; set; }
        public int MaxSelectionPerGroup { get; set; }

        public int ActivePage
        {
            get { return activePage; }
            set { activePage = value; }
        }
        public int LastSelectedIndex
        {
            get { return lastSelectedIndex; }
            set { lastSelectedIndex = value; }
        }

        public ContestTypes Type { get; set; }
        public string Footnote { get; set; }

        public string GenericName
        {
            get;
            set;
        }

        private List<DataDefinition> data = null;
        public List<DataDefinition> Data
        {
            get { return data; }
            set { data = value; }
        }

        private Dictionary<int, int> groupSelection = null;
        public Dictionary<int, int> GroupSelection
        {
            get { return groupSelection; }
            set { groupSelection = value; }
        }

        private List<Candidate> candidatesList = null;
        public List<Candidate> CandidatesList
        {
            get { return candidatesList; }
            set { candidatesList = value; }
        }

        private List<Proposition> propositions = null;
        public List<Proposition> Propositions { get { return propositions; } set { propositions = value; } }

        public int CountyId
        {
            get { return countyId; }
            set { countyId = value; }
        }

        public ContestDefinition()
        {
            data = new List<DataDefinition>();
            groupSelection = new Dictionary<int, int>();
            candidatesList = new List<Candidate>();
            propositions = new List<Proposition>();
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
                if (data != null)
                {
                    data.ForEach(ddef => ddef.Dispose());
                    data.Clear();
                }

                if (candidatesList != null)
                {
                    candidatesList.ForEach(cand => cand.Dispose());
                    candidatesList.Clear();
                }

                if (groupSelection != null)
                    groupSelection.Clear();

                if (propositions != null)
                {
                    propositions.ForEach(propos => propos.Dispose());
                    propositions.Clear();
                }
            }
        }

        public DataDefinition FindData(string search)
        {
            DataDefinition searchData = data.Find(
                delegate (DataDefinition tmpData)
                {
                    if (tmpData.Text == search) return true;
                    return false;
                });
            return searchData;
        }

        public DataDefinition FindDataByTag(string search)
        {
            DataDefinition searchData = data.Find(
                delegate (DataDefinition tmpData)
                {
                    if (tmpData.Tag == search) return true;
                    return false;
                });
            return searchData;
        }

        public int FindDataIndex(string search)
        {
            int index = data.FindIndex(
                delegate (DataDefinition tmpData)
                {
                    if (tmpData.Text == search) return true;
                    return false;
                });
            return index;
        }

        public int FindDataIndexByTag(string search)
        {
            int index = data.FindIndex(
                delegate (DataDefinition tmpData)
                {
                    if (tmpData.Tag == search) return true;
                    return false;
                });
            return index;
        }

        public static void SetContestFields(ref ContestDefinition contest, ref Race currentRaceInstance, ref Proposition currentPropositionInstance, Locale currentLocale, int? partyId = null, bool hasSlates=false)
        {
            //if (contest.Type == ContestTypes.R)
            if ((contest.Type == ContestTypes.R ? true : contest.Type == ContestTypes.V))
            {
                currentRaceInstance = Race.GetRace(contest.Id, currentLocale.Id);
                if (currentRaceInstance != null)
                {
                    contest.GroupName = currentRaceInstance.Title;
                    contest.Name = currentRaceInstance.VotedPosition;
                    contest.Footnote = currentLocale.IncumbentFootnote;
                    contest.Title = currentLocale.RaceTip;
                    contest.Tip = currentLocale.LblContestTip;
                }

                if (hasSlates)
                {
                    contest.CandidatesList = Candidate.GetSlatesCandidates(contest.Id, currentLocale.Id, partyId);
                } else
                {
                    contest.CandidatesList = Candidate.GetCandidates(contest.Id, currentLocale.Id, partyId);
                }
            }
            else if (contest.Type == ContestTypes.P)
            {
                currentPropositionInstance = Proposition.GetProposition(contest.Id, currentLocale.Id);
                if (currentPropositionInstance != null)
                {
                    contest.GroupName = currentLocale.PropositionSubheading;
                    contest.Name = currentPropositionInstance.Title;
                    contest.Title = string.Empty;
                    contest.Tip = currentLocale.PropositionTip;
                    contest.Footnote = string.Empty;
                }
            }
            else
            {
                if (contest.Propositions != null && contest.Propositions.Count > 0)
                {
                    contest.GroupName = currentLocale.PropositionSubheading;
                    contest.Name = "Mass Propositions";
                    contest.Title = "Mass Propositions";
                    contest.Tip = currentLocale.PropositionTip;
                    contest.Footnote = string.Empty;
                    var contestIds = new List<int>();
                    contestIds.AddRange(from Proposition proposition in contest.Propositions
                                        select proposition.ContestId);
                    contest.Propositions = Proposition.GetPropositions(contestIds, currentLocale.Id);
                    contest.MinSelection = 0;
                    contest.MaxSelection = contest.Propositions != null ? contest.Propositions.Count : 0;
                    contest.MaxSelectionPerGroup = 1;
                }
            }
        }

        public static void FillContestData(ref ContestDefinition contest, Proposition currentPropositionInstance, Locale currentLocale)
        {
            if (contest.Data.Count > 0)
                return;

            //if (contest.Type == ContestTypes.R)
            if ((contest.Type == ContestTypes.R ? true : contest.Type == ContestTypes.V))
            {
                var counter = 1;
                var maxCandId = 0;

                foreach (var candidate in contest.CandidatesList)
                {
                    if (!string.IsNullOrEmpty(candidate.Name))
                    {
                        var dataDef = new DataDefinition
                        {
                            GlobalId = candidate.Id,
                            Id = candidate.Id,
                            IncumbentFlag = candidate.IncumbentFlag,
                            Photo = candidate.Photo,
                            PartyLogo = candidate.PartyLogo,
                            PartyId = candidate.PartyId,
                            Voteable = candidate.Voteable,
                            IsWritten = candidate.IsWritten,
                            SlateId = Slate.GetSlateId(candidate.Id)
                        };

                        maxCandId = Math.Max(maxCandId, candidate.Id);
                        dataDef.Tag =
                            dataDef.Text =
                            string.Format("{0} {1}", candidate.Name, candidate.IncumbentFlag ? "**" : string.Empty);
                        dataDef.State = 0;

                        if (!dataDef.IsWritten)
                        {
                            contest.Data.Add(dataDef);
                            counter++;
                        }
                    }
                }

                for (var i = 0; i < contest.MaxWriteins; i++)
                {
                    var dataDef = new DataDefinition { GlobalId = ++maxCandId, Id = maxCandId, IncumbentFlag = false, WriteIn = true };
                    dataDef.Tag = dataDef.Text = string.Format("{0} {1}", "btn_add_candidates", string.Empty);
                    dataDef.Tag += "_" + i.ToString();
                    dataDef.State = 0;
                    dataDef.Voteable = true;
                    contest.Data.Add(dataDef);
                    counter++;
                }
            }
            else if (contest.Type == ContestTypes.P)
            {
                if (currentPropositionInstance != null)
                {
                    var propositionTextData = new DataDefinition();
                    propositionTextData.Id = propositionTextData.GlobalId = contest.Id;
                    propositionTextData.Text = currentPropositionInstance.Text;
                    propositionTextData.ReadOnly = true;

                    var forLabelData = new DataDefinition();
                    var againstLabelData = new DataDefinition();

                    forLabelData.Id = forLabelData.GlobalId = contest.Id + 1;
                    againstLabelData.Id = againstLabelData.GlobalId = contest.Id + 2;

                    forLabelData.Tag = forLabelData.Text = currentLocale.GetField(currentPropositionInstance.PositiveAnswer);
                    againstLabelData.Tag = againstLabelData.Text = currentLocale.GetField(currentPropositionInstance.NegativeAnswer);

                    forLabelData.State = againstLabelData.State = 0;

                    forLabelData.Voteable = true;
                    againstLabelData.Voteable = true;

                    contest.Data.Add(propositionTextData);
                    contest.Data.Add(forLabelData);
                    contest.Data.Add(againstLabelData);
                }
            }
            else
            {
                var groupId = 0;
                if (contest.Propositions != null && contest.Propositions.Count > 0)
                {
                    var idx = 0;
                    foreach (var proposition in contest.Propositions)
                    {
                        var propositionTitle = new DataDefinition();
                        propositionTitle.Id = propositionTitle.GlobalId = contest.Id + ++idx;
                        propositionTitle.Text = proposition.Title;
                        propositionTitle.ReadOnly = true;

                        var propositionTextData = new DataDefinition();
                        propositionTextData.Id = propositionTextData.GlobalId = contest.Id + ++idx;
                        propositionTextData.Text = proposition.Text;
                        propositionTextData.ReadOnly = true;

                        var newPropositionTextData = new DataDefinition
                        {
                            Id = contest.Id + ++idx,
                            GlobalId = contest.Id + ++idx,
                            Text = proposition.ChangedText,
                            ReadOnly = true
                        };

                        var forLabelData = new DataDefinition();
                        var againstLabelData = new DataDefinition();

                        forLabelData.Id = forLabelData.GlobalId = contest.Id + ++idx;
                        againstLabelData.Id = againstLabelData.GlobalId = contest.Id + ++idx;
                        if (currentLocale.GetField(proposition.PositiveAnswer) == null)
                            forLabelData.Text = "Yes";
                        else
                        forLabelData.Text = currentLocale.GetField(proposition.PositiveAnswer);
                        //forLabelData.Tag = NumberToText.IntegerToWritten(Convert.ToInt32(proposition.Title)) + " " + currentLocale.GetField(proposition.PositiveAnswer);
                        if (currentLocale.GetField(proposition.NegativeAnswer) == null)
                            againstLabelData.Text = "No";
                        else
                        againstLabelData.Text = currentLocale.GetField(proposition.NegativeAnswer);
                        //againstLabelData.Tag = NumberToText.IntegerToWritten(Convert.ToInt32(proposition.Title)) + " " + currentLocale.GetField(proposition.NegativeAnswer);

                        forLabelData.State = againstLabelData.State = 0;
                        forLabelData.ButtonGroup = againstLabelData.ButtonGroup = groupId++;

                        forLabelData.Voteable = true;
                        againstLabelData.Voteable = true;

                        contest.Data.Add(propositionTitle);
                        contest.Data.Add(propositionTextData);
                        contest.Data.Add(newPropositionTextData);
                        contest.Data.Add(forLabelData);
                        contest.Data.Add(againstLabelData);
                    }
                }
            }
        }


    }
}
