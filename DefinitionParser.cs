// Product:	VotRite
// Module:  DefinitionParser.cs
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
using System.IO;
using System.Collections;
using VotRite.Definition;
using VotRite.UI;
using VotRite.Util;
using VotRiteBallotDataManager.AppCode;
using System.Collections.Generic;
using VotRite.DBClasses;

namespace VotRite
{
    class DefinitionParser
    {
        private static DefinitionParser instance;

        private Stack block;

        private struct BlockEntry { public string key; public object value; }

        public static DefinitionParser Instance
        {
            get
            {
                if (instance == null) instance = new DefinitionParser();
                return instance;
            }
        }

        protected DefinitionParser() { block = new Stack(); }

        public BallotDefinition FillBallotContent(BallotDefinition ballotDefinition, Ballot ballot = null)
        {
            //if(AppManager.Instance.ballot != null) {
            //    BallotDefinition currentBallot = AppManager.Instance.ballot;
            //    foreach(ContestDefinition contest in currentBallot.ContestsList) {
            //        contest.ActivePage = 0;
            //        contest.LastSelectedIndex = -1;
            //        contest.Selected = 0;
            //        foreach(DataDefinition data in contest.Data) {
            //            data.State = 0;
            //        }

            //    }

            //    return currentBallot;
            //}

            if (ballotDefinition != null)
                return ballotDefinition;

           // code to display list of Ballots BallotListDisplay
           //if()
            //JimForms.BallotsListDisplay frmBallots = new JimForms.BallotsListDisplay();
            //frmBallots.ShowDialog();


            ballotDefinition = new BallotDefinition();

            if (ballot == null)
                ballot = Ballot.GetBallot();

            const string ballotContentRetrieveError = "ballot content couldn't be retrieved.";

            if (ballot == null)
                throw new Exception(ballotContentRetrieveError);

            AppManager.BallotId = ballot.Id;
            ballotDefinition.Id = ballot.Id;
            ballotDefinition.Address = ballot.Location;
            ballotDefinition.Name = ballot.TopHeading;
            ballotDefinition.Board = ballot.Board;
            ballotDefinition.Title = ballot.ElectionName;
            var hasScroll = Convert.ToBoolean(AppManager.Configuration["Contest"]["scroll"]);
            ballotDefinition.ContestPageSize = hasScroll ? int.MaxValue : 14;
            ballotDefinition.ElectionType = ballot.ElectionType;
            ballotDefinition.ReportLocaleId = ballot.ReportLocaleId;
            ballotDefinition.HasParty = ballot.HasParty;
            ballotDefinition.HasSlates = ballot.HasSlates;
            ballotDefinition.HasCounties = ballot.HasCounties;
            ballotDefinition.HasOverview = ballot.HasOverview;
            ballotDefinition.StartTime = ballot.StartTime;
            ballotDefinition.EndTime = ballot.EndTime;
            ballotDefinition.TestMode = ballot.TestMode;
            ballotDefinition.ShowSettingsAndTools = ballot.ShowSettingsAndTools;

            var contestsList = Contest.GetContests(ContestTypes.All, ballot.Id, string.Format("{0} desc, {1}", Contest.TypeField, Contest.IdField));
            ContestDefinition conDef = null;
            
            //foreach (var contest in contestsList)
            //{
            //    conDef = new ContestDefinition
            //        {
            //            Id = contest.Id,
            //            Name = contest.GenericName,
            //            Choice = "CONTEST CHOICE - NOT FROM DB",
            //            MinSelection = contest.MinVotes > 0 ? contest.MinVotes : 0,
            //            MaxSelection = contest.MaxVotes > 0 ? contest.MaxVotes : 1,
            //            Type = contest.Type,
            //            Group = contest.Id,
            //            GroupName = "CONTEST GROUP NAME - NOT FROM DB",
            //            Title = "CONTEST TITLE - NOT FROM DB",
            //            CandidatesList = contest.CandidatesList,
            //            Propositions = contest is MassProposition ? (contest as MassProposition).Propositions : null,
            //            MaxWriteins = contest.MaxWriteins,
            //            CountyId = contest.CountyId
            //        };
            //   // DataDefinition

            //    ballotDefinition.ContestsList.Add(conDef);
                   
            //}

            return ballotDefinition;
        }

        public List<ContestDefinition> FillContestsContent(BallotDefinition ballotDefinition)
        {
            List<ContestDefinition> list_cd = new List<ContestDefinition>();
            int partyId = 0;
            int countyId = 0;

            if (ballotDefinition.HasParty)
            {
                partyId = (int)ballotDefinition.PartyId;
            }

            if (ballotDefinition.HasCounties)
            {
                countyId = (int)ballotDefinition.CountyId;
            }

            var contestsList = Contest.GetContests(ContestTypes.All, ballotDefinition.Id, string.Format("{0} desc, {1}", Contest.TypeField, Contest.IdField), partyId, countyId);

            //foreach (var contest in contestsList)
            //{
            //    list_cd.Add(
            //        new ContestDefinition
            //        {
            //            Id = contest.Id,
            //            Name = contest.GenericName,
            //            Choice = "CONTEST CHOICE - NOT FROM DB",
            //            MinSelection = contest.MinVotes > 0 ? contest.MinVotes : 0,
            //            MaxSelection = contest.MaxVotes > 0 ? contest.MaxVotes : 1,
            //            Type = contest.Type,
            //            Group = contest.Id,
            //            GroupName = "CONTEST GROUP NAME - NOT FROM DB",
            //            Title = "CONTEST TITLE - NOT FROM DB",
            //            CandidatesList = contest.CandidatesList,
            //            Propositions = contest is MassProposition ? (contest as MassProposition).Propositions : null,
            //            MaxWriteins = contest.MaxWriteins,
            //            CountyId = contest.CountyId
            //        });
            //}
            return list_cd;
        }

        public BallotDefinition FillSlatesContent(BallotDefinition ballotDefinition, Locale locale, ContestDefinition contest=null)
        {
            if (locale == null)
            {
                locale = Locale.GetDefaultLocale(ballotDefinition.Id);
            }

            if (contest != null)
            {
                ballotDefinition.slatesDefinition.Slates = Slate.GetSlates(ballotDefinition.Id, locale.Id, contest.Id);
            } else
            {
                ballotDefinition.slatesDefinition.Slates = Slate.GetSlates(ballotDefinition.Id, locale.Id, null);
            }
            

            return ballotDefinition;
        }

        public BallotDefinition FillPartiesContent(BallotDefinition ballotDefinition, Locale locale)
        {
            if (locale == null)
            {
                locale = Locale.GetDefaultLocale(ballotDefinition.Id);
            }

            ballotDefinition.partiesDefinition.Parties = Party.GetParties(ballotDefinition.Id, locale.Id);
            
            return ballotDefinition;
        }

        public BallotDefinition FillCountiesContent(BallotDefinition ballotDefinition, Locale locale)
        {
            if (locale == null)
            {
                locale = Locale.GetDefaultLocale(ballotDefinition.Id);
            }

            ballotDefinition.countiesDefinition.Counties = County.GetCounties(ballotDefinition.Id, locale.Id);

            return ballotDefinition;
        }

        public object Parse(string definition)
        {
            object container = new object();
            string line;
            string word = "";
            string tmpWord = "";

            try
            {
                using (StreamReader reader = new StreamReader(definition))
                {
                    while ((line = reader.ReadLine()) != null)
                    {
                        if (line.Length == 0) continue;

                        foreach (char ch in line)
                        {
                            switch (ch)
                            {
                                case '{': word = OpenBlock(word, ref container); break;
                                case '}': CloseBlock(ref container); break;
                                case ';': ParsePair(tmpWord, word); tmpWord = word = ""; break;
                                case '=': tmpWord = word; word = ""; break;
                                case '"': break;    //  skip quote
                                case ' ':
                                    if (tmpWord != "") word += ch; break;
                                default: word += ch; break;
                            }
                        }
                    }
                }
            }
            catch (Exception e) { Logger.Instance.Write(e); }

            return container;
        }

        private string OpenBlock(string keyWord, ref object container)
        {
            if (keyWord != "")
            {
                BlockEntry be = new BlockEntry();

                be.key = keyWord;

                switch (keyWord)
                {
                    case "ballot":
                        container = be.value = new BallotDefinition(); break;
                    case "contest": be.value = new ContestDefinition(); break;
                    case "data": be.value = new DataDefinition(); break;
                    case "screen":
                        container = be.value = new ScreenDefinition(); break;
                    case "button": be.value = new VrButton(); break;
                    case "label": be.value = new VrLabel(); break;
                    case "image": be.value = new VrImage(); break;
                    case "container": be.value = new VrContainer(); break;
                    case "pinpad": container = new PinPadDefinition(); break;
                    case "pp_menu": be.value = new PinPadMenuDefinition(); break;
                    case "pp_menu_item": be.value = new PinPadMenuItem(); break;
                    default: break;
                }

                if (!block.Contains(be)) block.Push(be);

                keyWord = "";
            }

            return keyWord;
        }

        private void CloseBlock(ref object container)
        {
            BlockEntry be = (BlockEntry)block.Pop();
            BlockEntry child;

            switch (be.key)
            {
                case "contest":
                    ((BallotDefinition)container).ContestsList.Add(
                        (ContestDefinition)be.value);
                    break;
                case "data":
                    child = FindBlock("contest");

                    if (child.value != null)
                        ((ContestDefinition)child.value).Data.Add(
                            ((DataDefinition)be.value));
                    break;
                case "button":
                    ((ScreenDefinition)container).ScreenObjects.Add(
                        (VrButton)be.value);
                    break;
                case "label":
                    ((ScreenDefinition)container).ScreenObjects.Add(
                        (ScreenObject)be.value);
                    break;
                case "image":
                    ((ScreenDefinition)container).ScreenObjects.Add(
                        (ScreenObject)be.value);
                    break;
                case "container":
                    ((ScreenDefinition)container).ScreenObjects.Add(
                        (ScreenObject)be.value);
                    break;
                case "pp_menu":
                    ((PinPadDefinition)container).Menu.Add(
                        (PinPadMenuDefinition)be.value);
                    break;
                case "pp_menu_item":
                    child = FindBlock("pp_menu");

                    if (child.value != null)
                        ((PinPadMenuDefinition)child.value).Item.Add(
                            (PinPadMenuItem)be.value);
                    break;
                default: break;
            }
        }

        private void ParsePair(string key, string value)
        {
            BlockEntry be = (BlockEntry)block.Peek();
            BlockEntry child;

            switch (be.key)
            {
                case "ballot":
                    switch (key)
                    {
                        case "id":
                            ((BallotDefinition)be.value).Id = Convert.ToInt16(value);
                            break;
                        case "name":
                            ((BallotDefinition)be.value).Name = value;
                            break;
                        case "board":
                            ((BallotDefinition)be.value).Board = value;
                            break;
                        case "address":
                            ((BallotDefinition)be.value).Address = value;
                            break;
                        case "logo":
                            ((BallotDefinition)be.value).Logo = value;
                            break;
                        case "date":
                            ((BallotDefinition)be.value).Date = value;
                            break;
                        /*case "textSize": 
                            ((BallotDefinition)be.value).TextSize = 
                                Utilities.ValueToTextSize(value);
                            break;*/
                        case "contestPageSize":
                            ((BallotDefinition)be.value).ContestPageSize =
                                Convert.ToInt16(value);
                            break;
                        default: break;
                    }
                    break;
                case "contest":
                    child = FindBlock("contest");

                    if (be.value != null)
                    {
                        switch (key)
                        {
                            case "group":
                                ((ContestDefinition)child.value).Group = Convert.ToUInt16(value);
                                break;
                            case "groupName":
                                ((ContestDefinition)child.value).GroupName = value;
                                break;
                            case "id":
                                ((ContestDefinition)child.value).Id = Convert.ToInt16(value);
                                break;
                            case "name":
                                ((ContestDefinition)child.value).Name = value;
                                break;
                            case "title":
                                ((ContestDefinition)child.value).Title = value;
                                break;
                            case "choice":
                                ((ContestDefinition)child.value).Choice = value;
                                break;
                            case "tip":
                                ((ContestDefinition)child.value).Tip = value;
                                break;
                            case "minSelection":
                                ((ContestDefinition)child.value).MinSelection = Convert.ToInt16(value);
                                break;
                            case "maxSelection":
                                ((ContestDefinition)child.value).MaxSelection = Convert.ToInt16(value);
                                break;
                            //case "type":
                            //    ((ContestDefinition)child.value).Type = Convert.ToInt16(value);
                            //    break;
                            case "footnote":
                                ((ContestDefinition)child.value).Footnote = value;
                                break;
                            default: break;
                        }
                    }
                    break;
                case "data":
                    child = FindBlock("data");

                    if (child.value != null)
                    {
                        switch (key)
                        {
                            case "id":
                                ((DataDefinition)child.value).Id = Convert.ToInt16(value);
                                break;
                            case "globalId":
                                ((DataDefinition)child.value).GlobalId = Convert.ToInt16(value);
                                break;
                            case "text":
                                ((DataDefinition)child.value).Text = value;
                                break;
                            case "group":
                                ((DataDefinition)child.value).Group = value;
                                break;
                            case "icon":
                                ((DataDefinition)child.value).Photo = value;
                                break;
                            case "state":
                                /*((DataDefinition)child.value).State = 
                                    Utilities.ValueToState(Convert.ToInt16(value));*/
                                break;
                            case "readOnly":
                                ((DataDefinition)child.value).ReadOnly =
                                    Convert.ToBoolean(value);
                                break;
                            case "writeIn":
                                ((DataDefinition)child.value).WriteIn =
                                    Convert.ToBoolean(value); break;
                            case "tag":
                                ((DataDefinition)child.value).Tag = value;
                                break;
                            /*case "title":
                                ((DataDefinition)child.value).Title = value;
                                break;*/
                            default: break;
                        }
                    }
                    break;
                case "screen":
                    switch (key)
                    {
                        case "name":
                            ((ScreenDefinition)be.value).Name = value; break;
                        case "view":
                            ((ScreenDefinition)be.value).View = value; break;
                        case "background":
#if !WINDOWS
							value = value.Replace("\\", "/");		
#endif

                            ((ScreenDefinition)be.value).Background = value;
                            break;
                        case "bgColor":
                            ((ScreenDefinition)be.value).BgColor = value; break;
                        case "width":
                            ((ScreenDefinition)be.value).Width =
                                Convert.ToInt32(value); break;
                        case "height":
                            ((ScreenDefinition)be.value).Height =
                                Convert.ToInt32(value); break;
                        case "top":
                            ((ScreenDefinition)be.value).Top =
                                Convert.ToInt32(value); break;
                        case "left":
                            ((ScreenDefinition)be.value).Left =
                                Convert.ToInt32(value); break;
                        case "borderWidth":
                            ((ScreenDefinition)be.value).BorderWidth =
                                Convert.ToInt16(value); break;
                        case "borderColor":
                            ((ScreenDefinition)be.value).BorderColor = value;
                            break;
                        case "dialog":
                            ((ScreenDefinition)be.value).IsDialog =
                                Convert.ToBoolean(value); break;
                        default: break;
                    }
                    break;
                case "button":
                    switch (key)
                    {
                        case "name":
                            ((VrButton)be.value).Name = value; break;
                        case "action":
                            /*((VrButton)be.value).Action =
                                Utilities.TextToAction(value);*/
                            break;
                        case "bgImage":
#if !WINDOWS
							value = value.Replace("\\", "/");		
#endif

                            ((VrButton)be.value).BgImage = value; break;
                        case "width":
                            ((VrButton)be.value).Width = Convert.ToInt32(value); break;
                        case "maxwidth":
                            ((VrButton)be.value).MaxWidth = Convert.ToInt32(value); break;
                        case "leftPadding":
                            ((VrButton)be.value).LeftPadding = Convert.ToInt32(value); break;
                        case "rightPadding":
                            ((VrButton)be.value).RightPadding = Convert.ToInt32(value); break;
                        case "height":
                            ((VrButton)be.value).Height = Convert.ToInt32(value);
                            break;
                        case "left":
                            ((VrButton)be.value).Left = Convert.ToInt32(value);
                            break;
                        case "top":
                            ((VrButton)be.value).Top = Convert.ToInt32(value);
                            break;
                        case "text":
                            ((VrButton)be.value).Text = value; break;
                        case "textSize":
                            ((VrButton)be.value).TextSize = Convert.ToInt32(value);
                            break;
                        case "foreColor":
                            ((VrButton)be.value).ForeColor = value; break;
                        case "textAlign":
                            ((VrButton)be.value).TextAlign = value; break;
                        case "data":
                            ((VrButton)be.value).Data = value; break;
                        case "fitToText":
                            ((VrButton)be.value).FitToText = Convert.ToBoolean(value); break;
                        case "isRightSide":
                            ((VrButton)be.value).IsRightSide = Convert.ToBoolean(value); break;
                        case "tag":
                            ((VrButton)be.value).Tag = value; break;
                        default: break;
                    }
                    break;
                case "label":
                    switch (key)
                    {
                        case "name":
                            ((VrLabel)be.value).Name = value; break;
                        case "width":
                            ((VrLabel)be.value).Width = Convert.ToInt32(value);
                            break;
                        case "height":
                            ((VrLabel)be.value).Height = Convert.ToInt32(value);
                            break;
                        case "top":
                            ((VrLabel)be.value).Top = Convert.ToInt32(value);
                            break;
                        case "left":
                            ((VrLabel)be.value).Left = Convert.ToInt32(value);
                            break;
                        case "textSize":
                            ((VrLabel)be.value).TextSize = Convert.ToInt32(value);
                            break;
                        case "text":
                            ((VrLabel)be.value).Text = value;
                            break;
                        case "foreColor":
                            ((VrLabel)be.value).ForeColor = value; break;
                        case "bgColor":
                            ((VrLabel)be.value).BgColor = value; break;
                        case "textAlign":
                            ((VrLabel)be.value).TextAlign = value; break;
                        case "fontName":
                            ((VrLabel)be.value).FontName = value; break;
                        case "borderWidth":
                            ((VrLabel)be.value).BorderWidth = Convert.ToInt32(value);
                            break;
                        case "borderColor":
                            ((VrLabel)be.value).BorderColor = value; break;
                        case "speakable":
                            ((VrLabel)be.value).Speakable = Convert.ToBoolean(value); break;
                        default: break;
                    }
                    break;
                case "image":
                    switch (key)
                    {
                        case "image":
#if !WINDOWS
							value = value.Replace("\\", "/");		
#endif

                            ((VrImage)be.value).ImageName = value; break;
                        case "name":
                            ((VrImage)be.value).Name = value; break;
                        case "width":
                            ((VrImage)be.value).Width = Convert.ToInt32(value);
                            break;
                        case "height":
                            ((VrImage)be.value).Height = Convert.ToInt32(value);
                            break;
                        case "top":
                            ((VrImage)be.value).Top = Convert.ToInt32(value);
                            break;
                        case "left":
                            ((VrImage)be.value).Left = Convert.ToInt32(value);
                            break;
                        default: break;
                    }
                    break;
                case "container":
                    switch (key)
                    {
                        case "name":
                            ((VrContainer)be.value).Name = value; break;
                        case "width":
                            ((VrContainer)be.value).Width = Convert.ToInt32(value);
                            break;
                        case "height":
                            ((VrContainer)be.value).Height = Convert.ToInt32(value);
                            break;
                        case "top":
                            ((VrContainer)be.value).Top = Convert.ToInt32(value);
                            break;
                        case "left":
                            ((VrContainer)be.value).Left = Convert.ToInt32(value);
                            break;
                        case "align":
                            ((VrContainer)be.value).Align = value; break;
                        case "bgColor":
                            ((VrContainer)be.value).BgColor = value; break;
                        default: break;
                    }
                    break;
                case "pp_menu":
                    switch (key)
                    {
                        case "name":
                            ((PinPadMenuDefinition)be.value).Name = value;
                            break;
                        default: break;
                    }
                    break;
                case "pp_menu_item":
                    switch (key)
                    {
                        case "text": ((PinPadMenuItem)be.value).Text = value;
                            break;
                        case "action": ((PinPadMenuItem)be.value).Action = value;
                            break;
                        case "postAction":
                            ((PinPadMenuItem)be.value).PostAction = value; break;
                        case "data": ((PinPadMenuItem)be.value).Data = value;
                            break;
                        case "postData": ((PinPadMenuItem)be.value).PostData = value;
                            break;
                        case "key":
                            ((PinPadMenuItem)be.value).Key =
                                Convert.ToInt32(value); break;
                        case "readOnly":
                            ((PinPadMenuItem)be.value).ReadOnly =
                                Convert.ToBoolean(value); break;
                        case "protected":
                            ((PinPadMenuItem)be.value).Protected =
                                Convert.ToBoolean(value); break;
                        case "maskText":
                            ((PinPadMenuItem)be.value).MaskText =
                                Convert.ToBoolean(value); break;
                        default: break;
                    }
                    break;
                default: break;
            }
        }

        private BlockEntry FindBlock(string key)
        {
            BlockEntry entry = new BlockEntry();

            foreach (BlockEntry be in block)
            {
                if (be.key == key)
                {
                    entry = be; break;
                }
            }

            return entry;
        }
    }
}
