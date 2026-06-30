using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using System.Data;
using VotRite.UI;
using VotRite.Controllers;
using VotRite.Models;
using VotRite.MVC;
using VotRiteBallotDataManager.AppCode;
using VotRite.Definition;
using VotRite.DBClasses;

namespace VotRite.Views
{
    class ResultsView : VrScreen
    {
        private ResultsModel vModel { get { return model as ResultsModel; } }
        private ResultsController vController { get { return controller as ResultsController; } }
        private VrLabel lblBallotName;
        private VrLabel lblBallotTitle;
        private VrLabel lblBallotAddress;
        private VrLabel lblBallotDate;
        private VrLabel lblGroupName;
        private VrLabel lblContestTitle;
        private VrContainer container;
        private VrButton btnBack;

        public VrContainer Container { get { return container; } }

        private const int SCREEN_V_SPACE = 20;
        private const int selectionsSpace = 20;

        private static int _textSize = 12;
        private Timer _timer;
        private const int StartTick = 5;
        protected internal int TimerCount { get; set; }
        public Timer GetTimer()
        {
            return _timer;
        }

        private List<string> _textsToSpeak;

        private RaceViewOptions raceViewOptions = null;
        private bool _hasScroll = false;

        public ResultsView(ResultsModel m, ResultsController c)
            : base(m, c)
        {
            model = m;
            controller = c;
			//vModel.Definition.Background = "graphics\\initialBG_f1.png";
			lblBallotName = (VrLabel)vModel.FindScreenObject("lbl_blt_name");
            lblBallotTitle = (VrLabel)vModel.FindScreenObject("lbl_blt_title");
            lblBallotAddress = (VrLabel)vModel.FindScreenObject("lbl_blt_address");
            lblBallotDate = (VrLabel)vModel.FindScreenObject("lbl_blt_date");
            lblGroupName = (VrLabel)vModel.FindScreenObject("lbl_group_name");
            lblContestTitle = (VrLabel)vModel.FindScreenObject("lbl_contest_title");
            container = (VrContainer)vModel.FindScreenObject("ctr_contest");
            btnBack = (VrButton)vModel.FindScreenObject("btn_back");

            raceViewOptions = new RaceViewOptions(vModel.Contest.Id);
            _textSize = raceViewOptions.UseDefaultSettings ?
                Convert.ToInt32(AppManager.Configuration["Contest"]["RaceFontSize"]) : raceViewOptions.FontSize;

            Initialize();

			AppManager.Instance.checkDoubleSpaceSetting(vModel.Definition.ScreenObjects, container);

			if (_textsToSpeak != null)
            {
                foreach (var text in _textsToSpeak.Where(text => text != null))
                {
                    AppManager.Instance.StartSpeaker(text);
                }
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _timer.Dispose();
                _textsToSpeak.Clear();

                if (lblBallotName != null) lblBallotName.Dispose();
                if (lblBallotTitle != null) lblBallotTitle.Dispose();
                if (lblBallotAddress != null) lblBallotAddress.Dispose();
                if (lblBallotDate != null) lblBallotDate.Dispose();
                if (lblGroupName != null) lblGroupName.Dispose();
                if (lblContestTitle != null) lblContestTitle.Dispose();
                if (btnBack != null) btnBack.Dispose();
                if (container != null) container.Dispose();
            }
            base.Dispose(disposing);
        }

        private void Initialize_new()
        {
            if (btnBack != null)
                btnBack.Tag = (string)vModel.Vars["back"];

            if (lblBallotName != null)
                lblBallotName.Text = (string)vModel.Vars["ballot_name"];
            if (lblBallotTitle != null)
                lblBallotTitle.Text = (string)vModel.Vars["ballot_title"];
            if ((string)vModel.Vars["ballot_address"] != "")
                lblBallotAddress.Text = (string)vModel.Vars["ballot_address"];
            if (lblBallotDate != null)
                lblBallotDate.Text = (string)vModel.Vars["ballot_date"];
            if (lblGroupName != null)
                lblGroupName.Text = "";
            
            lblContestTitle.TextAlign = "left-top";

            int maxHeight = 0;
            int topHist = container.Top + 30;
            int leftHist = container.Left + 10;
            int n = 0;
            int objHeight = 80;
            ScreenObject screenObj;

            ///////
            string query;
            DataTable dt;

            if (vModel.Ballot.ElectionType == ElectionTypes.ranking_choice)
            {
                query = string.Format("select cnt from cast where ballotId={0}", vModel.Ballot.Id);
                int ballots = Helper.Cast(DataManager.VotingResultsDataInstance.GetScalarData(query), 0);
                Candidate candidate = null;
                int preference = 0;
                SortedList<string, int> iniCounters = new SortedList<string, int>();
                SortedList<string, int> counters = new SortedList<string, int>();
                SortedList<int, string> ballot = new SortedList<int, string>();
                SortedList<string, int> winners = new SortedList<string, int>();
                decimal quota = Math.Ceiling((decimal)ballots / 2);
                
                query = string.Format("select did, preference, sid from counter where bid={0}", vModel.Ballot.Id);
                dt = DataManager.VotingResultsDataInstance.GetDataV2(query);

                if (dt == null || dt.Rows.Count < 1)
                {
                    SetNavigation();
                    return;
                }

                int i = 1;
                string sid = "";

                foreach (DataRow dr in dt.Rows)
                {
                    Console.WriteLine(dr["sid"]);

                    if (sid == "")
                    {
                        sid = (string)dr["sid"];
                    }

                    if (sid != (string)dr["sid"])
                    {
                        string ballotStr = "";

                        foreach (KeyValuePair<int, string> pair in ballot)
                        {
                            ballotStr += pair.Value + ',';
                        }

                        ballotStr = ballotStr.Substring(0, ballotStr.Length - 1);

                        if (counters.ContainsKey(ballotStr))
                        {
                            counters[ballotStr]++;
                        }
                        else
                        {
                            if (!counters.ContainsKey(ballotStr))
                            {
                                counters.Add(ballotStr, 1);
                            }
                        }

                        sid = (string)dr["sid"];
                        ballot = new SortedList<int, string>();
                    }

                    if (i == dt.Rows.Count)
                    {
                        foreach (Candidate c in vModel.Contest.CandidatesList)
                        {
                            if (c.Id == (int)dr["did"])
                            {
                                candidate = c;
                                preference = Helper.Cast(dr["preference"], 0);
                                break;
                            }
                        }

                        if (!ballot.ContainsKey(preference))
                        {
                            ballot.Add(preference, candidate.Name);
                        }

                        string ballotStr = "";

                        foreach (KeyValuePair<int, string> pair in ballot)
                        {
                            ballotStr += pair.Value + ',';
                        }

                        ballotStr = ballotStr.Substring(0, ballotStr.Length - 1);

                        if (counters.ContainsKey(ballotStr))
                        {
                            counters[ballotStr]++;
                        }
                        else
                        {
                            if (!counters.ContainsKey(ballotStr))
                            {
                                counters.Add(ballotStr, 1);
                            }
                        }

                        break;
                    }

                    foreach (Candidate c in vModel.Contest.CandidatesList)
                    {
                        if (c.Id == (int)dr["did"])
                        {
                            candidate = c;
                            preference = Helper.Cast(dr["preference"], 0);
                            break;
                        }
                    }

                    if (!ballot.ContainsKey(preference))
                    {
                        ballot.Add(preference, candidate.Name);
                    }

                    ++i;
                }
                Console.WriteLine(counters);
                
                iniCounters = counters;

                bool winnerFound = false;
                int it = 0;

                while (!winnerFound)
                {
                    SortedList<string, int> firstPref = new SortedList<string, int>();

                    foreach (KeyValuePair<string, int> blt in counters)
                    {
                        string[] fp = blt.Key.Split(new char[] { ',' });

                        if (firstPref.ContainsKey(fp[0]))
                        {
                            firstPref[fp[0]] += blt.Value;
                        }
                        else
                        {
                            if (!firstPref.ContainsKey(fp[0])) {
                                firstPref.Add(fp[0], blt.Value);
                            }
                        }
                    }
                    Console.WriteLine(firstPref);
                    
                    foreach (KeyValuePair<string, int> c in firstPref)
                    {
                        if (c.Value > quota)
                        {
                            if (!winners.ContainsKey(c.Key))
                            {
                                winners.Add(c.Key, c.Value);
                            }
                        }
                    }

                    if (firstPref.Count == 1)
                    {
                        if (!winners.ContainsKey(firstPref.Keys[0]))
                        {
                            winners.Add(firstPref.Keys[0], firstPref.Values[0]);
                        }
                    }

                    if (winners.Count == 1)
                    {
                        winnerFound = true;
                        Console.WriteLine("winner: " + winners);
                        break;
                    }

                    int min = 0;
                    string minCand = "";
                    int j = 0;

                    foreach (KeyValuePair<string, int> p in firstPref)
                    {
                        if (min == 0)
                        {
                            min = p.Value;
                            minCand = p.Key;
                        } else
                        {
                            if (j == firstPref.Count - 1)
                            {
                                if (p.Value == min)
                                {
                                    minCand = p.Key;
                                }
                            }
                            else
                            {
                                if (p.Value <= min)
                                {
                                    minCand = p.Key;
                                }
                            }
                        }
                        
                        ++j;
                    }
                    Console.WriteLine(minCand);

                    SortedList<string, int> newCounters = new SortedList<string, int>();

                    foreach (KeyValuePair<string, int> blt in counters)
                    {
                        string[] fp = blt.Key.Split(new char[] { ',' });
                        string ballotStr = "";

                        foreach (string c in fp)
                        {
                            if (c != minCand)
                            {
                                ballotStr += c + ',';
                            }
                        }

                        if (ballotStr != "") {
                            ballotStr = ballotStr.Substring(0, ballotStr.Length - 1);

                            if (newCounters.ContainsKey(ballotStr))
                            {
                                newCounters[ballotStr] += blt.Value;
                            }
                            else
                            {
                                if (!newCounters.ContainsKey(ballotStr))
                                {
                                    newCounters.Add(ballotStr, blt.Value);
                                }
                            }
                        }
                    }
                    Console.WriteLine(newCounters);

                    counters = newCounters;
                    ++it;
                }

                // Display winner
                screenObj = new VrLabel()
                {
                    Width = 300,
                    Height = objHeight,
                    Left = leftHist,
                    Top = topHist,
                    Text = "Winner",
                    TextSize = _textSize,
                    TextAlign = "left-middle"
                };

                leftHist += screenObj.Width + 60;

                if (!screenObj.Resized)
                {
                    screenObj.Resize(vModel.Scale);
                }

                container.Controls.Add(screenObj);

                screenObj = new VrLabel()
                {
                    Width = 300,
                    Height = objHeight,
                    Left = leftHist,
                    Top = topHist,
                    Name = "lblWinner",
                    Text = winners.Keys[0],
                    TextSize = _textSize,
                    TextAlign = "left-middle"
                };

                if (!screenObj.Resized)
                {
                    screenObj.Resize(vModel.Scale);
                }

                container.Controls.Add(screenObj);

                leftHist = container.Left + 10;
                topHist += objHeight + 15;

                // Display total votes count
                screenObj = new VrLabel()
                {
                    Width = 300,
                    Height = objHeight,
                    Left = leftHist,
                    Top = topHist,
                    Text = "Total ballots",
                    TextSize = _textSize,
                    TextAlign = "left-middle"
                };

                leftHist += screenObj.Width + 70;

                if (!screenObj.Resized)
                {
                    screenObj.Resize(vModel.Scale);
                }

                container.Controls.Add(screenObj);

                screenObj = new VrLabel()
                {
                    Width = objHeight,
                    Height = objHeight,
                    Left = leftHist,
                    Top = topHist,
                    Name = "lblTotalVotes_" + ballots.ToString(),
                    Tag = Name,
                    Text = ballots.ToString(),
                    TextSize = _textSize,
                    TextAlign = "left-middle"
                };

                if (!screenObj.Resized)
                {
                    screenObj.Resize(vModel.Scale);
                }

                container.Controls.Add(screenObj);

                leftHist = container.Left + 10;
                topHist += objHeight + 15;

                // Display quota
                screenObj = new VrLabel()
                {
                    Width = 300,
                    Height = objHeight,
                    Left = leftHist,
                    Top = topHist,
                    Text = "Quota",
                    TextSize = _textSize,
                    TextAlign = "left-middle"
                };

                leftHist += screenObj.Width + 70;

                if (!screenObj.Resized)
                {
                    screenObj.Resize(vModel.Scale);
                }

                container.Controls.Add(screenObj);

                screenObj = new VrLabel()
                {
                    Width = objHeight,
                    Height = objHeight,
                    Left = leftHist,
                    Top = topHist,
                    Name = "lblQuota_" + quota.ToString(),
                    Tag = Name,
                    Text = quota.ToString(),
                    TextSize = _textSize,
                    TextAlign = "left-middle"
                };

                if (!screenObj.Resized)
                {
                    screenObj.Resize(vModel.Scale);
                }

                container.Controls.Add(screenObj);

                leftHist = container.Left + 10;
                topHist += objHeight + 15;

                // Header
                leftHist += 320;

                screenObj = new VrLabel()
                {
                    Width = 140,
                    Height = objHeight,
                    Left = leftHist,
                    Top = topHist,
                    Text = "Votes",
                    TextSize = _textSize
                 };

                leftHist += screenObj.Width + 10;

                if (!screenObj.Resized)
                {
                    screenObj.Resize(vModel.Scale);
                }

                container.Controls.Add(screenObj);
                
                topHist += objHeight + 20;
                leftHist = container.Left + 10;

                foreach (KeyValuePair<string, int> cand in iniCounters)
                {
                    screenObj = new VrLabel()
                    {
                        Width = 300,
                        Height = objHeight,
                        Left = leftHist,
                        Top = topHist,
                        Tag = cand.Key,
                        BgColor = "#fff",
                        Text = cand.Key,
                        TextAlign = "left-middle",
                        TextSize = _textSize,
                        Photo = candidate.Photo,
                        PartyLogo = candidate.PartyLogo
                    };

                    leftHist += screenObj.Width + 20;

                    if (!screenObj.Resized)
                    {
                        screenObj.Resize(vModel.Scale);
                    }

                    container.Controls.Add(screenObj);

                    screenObj = new VrLabel()
                    {
                        Width = 140,
                        Height = objHeight,
                        Left = leftHist,
                        Top = topHist,
                        Text = cand.Value.ToString(),
                        TextSize = _textSize
                    };

                    leftHist += screenObj.Width + 10;

                    if (!screenObj.Resized)
                    {
                        screenObj.Resize(vModel.Scale);
                    }

                    container.Controls.Add(screenObj);
                    
                    leftHist = container.Left + 10;
                    topHist += objHeight + 20;
                }

                if (topHist > container.Top + container.Height)
                {
                    if (!_hasScroll) _hasScroll = true;
                }

                maxHeight = Math.Max(maxHeight, topHist);
                n++;
            }
            ///////

            _textsToSpeak = GetAllTexts(true, false, false);

            SetNavigation();

            if (_hasScroll)
            {
                if (Convert.ToBoolean(AppManager.Configuration["Contest"]["scroll"]) && container.CreateScroll(vModel, maxHeight - container.Top))
                {
                    var upScroll = vModel.Definition.ScreenObjects[vModel.Definition.ScreenObjects.Count - 2];

                    if (upScroll.ObjectState == ScreenObject.ScreenObjectState.ACTIVE)
                        upScroll.Text = "Touch here to see less";

                    upScroll.Tag = "Scroll up";

                    var downScroll = vModel.Definition.ScreenObjects[vModel.Definition.ScreenObjects.Count - 1];

                    if (downScroll.ObjectState == ScreenObject.ScreenObjectState.ACTIVE)
                        downScroll.Text = "Touch here to see more";

                    downScroll.Tag = "Scroll down";

                    container.Top += upScroll.Height;
                    container.Height -= upScroll.Height;
                    container.Height -= downScroll.Height;

                    if (!container.Resized) container.Resize(vModel.Scale);

                    foreach (var screenObject in container.Controls)
                    {
                        screenObject.Top += upScroll.Height;
                        if (screenObject.Top + screenObject.Height > container.Top + container.Height)
                            screenObject.Visible = false;
                    }
                    _timer = new Timer { Interval = VrContainer.GetScrollTimerIn(ScrollTimerTypes.ContestViewScrollTimerIn) };
                    _timer.Stop();
                    _timer.Tick += TimerTick;
                }
            }
        }

		private void Initialize()
		{
			Locale locale;
			if (AppManager.Instance.backgroundTheme != AppManager.colorTheme.White)
			{
				vModel.Definition.Background = AppManager.GetPathToCommonFile("graphics" + Global.Instance.SLASH + "initialBG_f1_" + AppManager.Instance.backgroundTheme + ".png");
			}
			if (this.btnBack != null)
			{
				this.btnBack.Tag = (string)this.vModel.Vars["back"];
			}
			if (this.lblBallotName != null)
			{
				this.lblBallotName.Text = (string)this.vModel.Vars["ballot_name"];
			}
			if (this.lblBallotTitle != null)
			{
				this.lblBallotTitle.Text = (string)this.vModel.Vars["ballot_title"];
			}
			if ((string)this.vModel.Vars["ballot_address"] != "")
			{
				this.lblBallotAddress.Text = (string)this.vModel.Vars["ballot_address"];
			}
			if (this.lblBallotDate != null)
			{
				this.lblBallotDate.Text = (string)this.vModel.Vars["ballot_date"];
			}
			if (this.lblGroupName != null)
			{
				this.lblGroupName.Text = "";
			}
			this.lblContestTitle.TextAlign = "left-top";
			int top = this.container.Top + 20;
			int left = this.container.Left + 30;
			int num = 60;
			string str = string.Concat("select cnt from cast where ballotId=", this.vModel.Ballot.Id);
			int num1 = Helper.Cast(DataManager.VotingResultsDataInstance.GetScalarData(str), 0);
			str = string.Concat("select bid, cid, did, cand_name, vdate, cnt, slate_id, pid  from counter where bid=", this.vModel.Ballot.Id, ";");
			DataTable dataV2 = DataManager.VotingResultsDataInstance.GetDataV2(str);
			ScreenObject vrLabel = new VrLabel()
			{
				Width = 300,
				Height = num,
				Left = left - 10,
				Top = top,
				Text = "Total ballots",
				TextSize = ResultsView._textSize,
				TextAlign = "left-middle"
			};
			if (!vrLabel.Resized)
			{
				vrLabel.Resize(this.vModel.Scale);
			}
			this.container.Controls.Add(vrLabel);
			vrLabel = new VrLabel()
			{
				Width = num,
				Height = num,
				Left = left + vrLabel.Width + 20,
				Top = top,
				Name = string.Concat("lblTotalVotes_", num1.ToString()),
				Tag = base.Name,
				Text = num1.ToString(),
				TextSize = ResultsView._textSize,
				TextAlign = "left-middle"
			};
			if (!vrLabel.Resized)
			{
				vrLabel.Resize(this.vModel.Scale);
			}
			this.container.Controls.Add(vrLabel);
			top += num;
			if (this.vModel.Ballot.HasSlates)
			{
				DefinitionParser.Instance.FillSlatesContent(this.vModel.Ballot, Locale.DefaultLocale, null);
				if (this.vModel.Ballot.slatesDefinition.Slates.Count > 0)
				{
					vrLabel = new VrLabel()
					{
						Width = this.container.Width - 40 - (left - this.container.Left),
						Height = 2,
						Left = left,
						Top = top,
						BgColor = "#000000"
					};
					if (!vrLabel.Resized)
					{
						vrLabel.Resize(this.vModel.Scale);
					}
					this.container.Controls.Add(vrLabel);
					vrLabel = new VrLabel()
					{
						Width = 300,
						Height = num,
						Left = left,
						Top = top,
						Text = "SLATES",
						TextSize = ResultsView._textSize,
						TextAlign = "left-middle"
					};
					if (!vrLabel.Resized)
					{
						vrLabel.Resize(this.vModel.Scale);
					}
					this.container.Controls.Add(vrLabel);
					top += num;
					foreach (Slate slate in this.vModel.Ballot.slatesDefinition.Slates)
					{
						string str1 = string.Concat(new object[] { "bid=", this.vModel.Ballot.Id, " and slate_id=", slate.Id });
						int num2 = dataV2.Select(str1).Count<DataRow>();
						vrLabel = new VrLabel()
						{
							Width = 300,
							Height = num,
							Left = left + 10,
							Top = top,
							Text = slate.Name,
							TextSize = ResultsView._textSize,
							TextAlign = "left-middle"
						};
						if (!vrLabel.Resized)
						{
							vrLabel.Resize(this.vModel.Scale);
						}
						this.container.Controls.Add(vrLabel);
						vrLabel = new VrLabel()
						{
							Width = num,
							Height = num,
							Left = vrLabel.Width + 200,
							Top = top,
							Text = num2.ToString(AppManager.DefaultCultureInfo),
							TextSize = ResultsView._textSize,
							TextAlign = "left-middle"
						};
						if (!vrLabel.Resized)
						{
							vrLabel.Resize(this.vModel.Scale);
						}
						this.container.Controls.Add(vrLabel);
						top += num;
					}
				}
			}
			ContestDefinition item = null;
			Race race = null;
			Proposition proposition = null;
			List<Locale> locales = Locale.FetchLocales(AppManager.BallotId);
			List<Locale> locales1 = locales;
			if (locales == null)
			{
				locales1 = new List<Locale>();
			}
			int num3 = -1;
			int? nullable = this.vModel.Ballot.ReportLocaleId;
			if (nullable.HasValue)
			{
				num3 = locales1.FindIndex((Locale l) => {
					int id = l.Id;
					int? reportLocaleId = this.vModel.Ballot.ReportLocaleId;
					return id == reportLocaleId.GetValueOrDefault() & reportLocaleId.HasValue;
				});
			}
			locale = (num3 <= -1 ? locales1[0] : locales1[num3]);
			for (int i = 0; i < this.vModel.Ballot.ContestsList.Count; i++)
			{
				item = this.vModel.Ballot.ContestsList[i];
				nullable = null;
				ContestDefinition.SetContestFields(ref item, ref race, ref proposition, locale, nullable, this.vModel.Ballot.HasSlates);
				ContestDefinition.FillContestData(ref item, proposition, locale);
			}
			DataTable correctOrderBallot = this.GetCorrectOrderBallot(this.vModel.Ballot);
			for (int j = 0; j < correctOrderBallot.Rows.Count; j++)
			{
				List<ContestDefinition> contestDefinitions = this.vModel.Ballot.ContestsList.FindAll((ContestDefinition cd) => cd.GroupName == correctOrderBallot.Rows[j]["GroupName"].ToString());
				var collection = (
					from cd in contestDefinitions
					select new { gpn = cd.GroupName, gnn = cd.GenericName }).Distinct();
				foreach (var variable in collection)
				{
					List<ContestDefinition> contestDefinitions1 = contestDefinitions.FindAll((ContestDefinition cd) => cd.GenericName == variable.gnn);
					if (contestDefinitions1[0].Propositions == null)
					{
						vrLabel = new VrLabel()
						{
							Width = this.container.Width - 40 - (left - this.container.Left),
							Height = 2,
							Left = left,
							Top = top,
							BgColor = "#000000"
						};
						if (!vrLabel.Resized)
						{
							vrLabel.Resize(this.vModel.Scale);
						}
						this.container.Controls.Add(vrLabel);
						vrLabel = new VrLabel()
						{
							Width = this.container.Width - 30 - (left - this.container.Left),
							Height = num,
							Left = left,
							Top = top,
							Text = variable.gpn,
							TextSize = ResultsView._textSize,
							TextAlign = "left-middle"
						};
						if (!vrLabel.Resized)
						{
							vrLabel.Resize(this.vModel.Scale);
						}
						this.container.Controls.Add(vrLabel);
						top += num;
						vrLabel = new VrLabel()
						{
							Width = this.container.Width - 30 - (left - this.container.Left),
							Height = num,
							Left = left,
							Top = top,
							Text = variable.gnn,
							TextSize = ResultsView._textSize,
							TextAlign = "left-middle"
						};
						if (!vrLabel.Resized)
						{
							vrLabel.Resize(this.vModel.Scale);
						}
						this.container.Controls.Add(vrLabel);
						top += num;
						List<Candidate> candidates = new List<Candidate>();
						List<DataDefinition> dataDefinitions = new List<DataDefinition>();
						foreach (ContestDefinition contestDefinition in contestDefinitions1)
						{
							candidates.AddRange((
								from can in contestDefinition.CandidatesList
								where can.Voteable
								orderby can.IsWritten
								select can).ToList<Candidate>());
							dataDefinitions.AddRange((
								from can in contestDefinition.Data
								where can.Voteable
								orderby can.IsWritten
								select can).ToList<DataDefinition>());
						}
						if (candidates.Count > 0)
						{
							IEnumerable<string> strs = (
								from c in candidates
								select c.Name).Distinct<string>();
							foreach (string str2 in strs)
							{
								vrLabel = new VrLabel()
								{
									Width = 300,
									Height = num,
									Left = left + 10,
									Top = top,
									Text = str2.ToUpper(),
									TextSize = ResultsView._textSize,
									TextAlign = "left-middle"
								};
								if (!vrLabel.Resized)
								{
									vrLabel.Resize(this.vModel.Scale);
								}
								this.container.Controls.Add(vrLabel);
								DataTable dataTable = dataV2;
								string[] strArrays = new string[] { "cid IN (", null, null, null, null };
								strArrays[1] = string.Join<int>(",",
									from c in contestDefinitions1
									select c.Id);
								strArrays[2] = ") AND did IN (";
								strArrays[3] = string.Join<int>(",",
									from c in candidates
									where c.Name == str2
									select c.Id);
								strArrays[4] = ")";
								int num4 = ((IEnumerable<DataRow>)dataTable.Select(string.Concat(strArrays))).Sum<DataRow>((DataRow r) => Convert.ToInt32(r["cnt"]));
								vrLabel = new VrLabel()
								{
									Width = num,
									Height = num,
									Left = vrLabel.Width + 200,
									Top = top,
									Text = num4.ToString(AppManager.DefaultCultureInfo),
									TextSize = ResultsView._textSize,
									TextAlign = "left-middle"
								};
								if (!vrLabel.Resized)
								{
									vrLabel.Resize(this.vModel.Scale);
								}
								this.container.Controls.Add(vrLabel);
								top += num;
							}
							DataRow[] dataRowArray = dataV2.Select(string.Concat("cid IN (", string.Join<int>(",",
								from c in contestDefinitions1
								select c.Id), ") AND did IS NULL"));
							List<string> strs1 = new List<string>();
							DataRow[] dataRowArray1 = dataRowArray;
							for (int k = 0; k < (int)dataRowArray1.Length; k++)
							{
								string str3 = dataRowArray1[k]["cand_name"].ToString();
								if (!strs1.Contains(str3))
								{
									vrLabel = new VrLabel()
									{
										Width = 300,
										Height = num,
										Left = left + 10,
										Top = top,
										Text = str3.ToUpper(),
										TextSize = ResultsView._textSize,
										TextAlign = "left-middle"
									};
									if (!vrLabel.Resized)
									{
										vrLabel.Resize(this.vModel.Scale);
									}
									this.container.Controls.Add(vrLabel);
									int num5 = (
										from r in dataRowArray
										where r["cand_name"].ToString() == str3
										select r).Sum<DataRow>((DataRow r) => Convert.ToInt32(r["cnt"]));
									vrLabel = new VrLabel()
									{
										Width = num,
										Height = num,
										Left = vrLabel.Width + 200,
										Top = top,
										Text = num5.ToString(AppManager.DefaultCultureInfo),
										TextSize = ResultsView._textSize,
										TextAlign = "left-middle"
									};
									if (!vrLabel.Resized)
									{
										vrLabel.Resize(this.vModel.Scale);
									}
									this.container.Controls.Add(vrLabel);
									top += num;
									strs1.Add(str3);
								}
							}
						}
						else if (dataDefinitions.Count > 0)
						{
							IEnumerable<string> strs2 = (
								from c in dataDefinitions
								select c.Text).Distinct<string>();
							foreach (string str4 in strs2)
							{
								vrLabel = new VrLabel()
								{
									Width = 300,
									Height = num,
									Left = left + 10,
									Top = top,
									Text = str4.ToUpper(),
									TextSize = ResultsView._textSize,
									TextAlign = "left-middle"
								};
								if (!vrLabel.Resized)
								{
									vrLabel.Resize(this.vModel.Scale);
								}
								this.container.Controls.Add(vrLabel);
								DataTable dataTable1 = dataV2;
								string[] strArrays1 = new string[] { "cid IN (", null, null, null, null };
								strArrays1[1] = string.Join<int>(",",
									from c in contestDefinitions1
									select c.Id);
								strArrays1[2] = ") AND did IN (";
								strArrays1[3] = string.Join<int>(",",
									from d in dataDefinitions
									where d.Text == str4
									select d.Id);
								strArrays1[4] = ")";
								int num6 = ((IEnumerable<DataRow>)dataTable1.Select(string.Concat(strArrays1))).Sum<DataRow>((DataRow r) => Convert.ToInt32(r["cnt"]));
								vrLabel = new VrLabel()
								{
									Width = num,
									Height = num,
									Left = vrLabel.Width + 200,
									Top = top,
									Text = num6.ToString(AppManager.DefaultCultureInfo),
									TextSize = ResultsView._textSize,
									TextAlign = "left-middle"
								};
								if (!vrLabel.Resized)
								{
									vrLabel.Resize(this.vModel.Scale);
								}
								this.container.Controls.Add(vrLabel);
								top += num;
							}
						}
					}
					else
					{
						foreach (ContestDefinition contestDefinition1 in contestDefinitions1)
						{
							for (int l1 = 0; l1 < contestDefinition1.Data.Count; l1 += 5)
							{
								vrLabel = new VrLabel()
								{
									Width = this.container.Width - 40 - (left - this.container.Left),
									Height = 2,
									Left = left,
									Top = top,
									BgColor = "#000000"
								};
								if (!vrLabel.Resized)
								{
									vrLabel.Resize(this.vModel.Scale);
								}
								this.container.Controls.Add(vrLabel);
								vrLabel = new VrLabel()
								{
									Width = this.container.Width - 30 - (left - this.container.Left),
									Height = num,
									Left = left,
									Top = top,
									Text = variable.gpn,
									TextSize = ResultsView._textSize,
									TextAlign = "left-middle"
								};
								if (!vrLabel.Resized)
								{
									vrLabel.Resize(this.vModel.Scale);
								}
								this.container.Controls.Add(vrLabel);
								top += num;
								vrLabel = new VrLabel()
								{
									Width = this.container.Width - 30 - (left - this.container.Left),
									Height = num,
									Left = left,
									Top = top,
									Text = contestDefinition1.Data[l1].Text.ToUpper(),
									TextSize = ResultsView._textSize,
									TextAlign = "left-middle"
								};
								if (!vrLabel.Resized)
								{
									vrLabel.Resize(this.vModel.Scale);
								}
								this.container.Controls.Add(vrLabel);
								top += num;
								vrLabel = new VrLabel()
								{
									Width = 300,
									Height = num,
									Left = left + 10,
									Top = top,
									Text = contestDefinition1.Data[l1 + 3].Text.ToUpper(),
									TextSize = ResultsView._textSize,
									TextAlign = "left-middle"
								};
								if (!vrLabel.Resized)
								{
									vrLabel.Resize(this.vModel.Scale);
								}
								this.container.Controls.Add(vrLabel);
								int num7 = ((IEnumerable<DataRow>)dataV2.Select(string.Concat(new object[] { "cid=", contestDefinition1.Propositions[l1 / 5].ContestId, " AND did=", contestDefinition1.Data[l1 + 3].Id }))).Sum<DataRow>((DataRow r) => Convert.ToInt32(r["cnt"]));
								vrLabel = new VrLabel()
								{
									Width = num,
									Height = num,
									Left = vrLabel.Width + 200,
									Top = top,
									Text = num7.ToString(AppManager.DefaultCultureInfo),
									TextSize = ResultsView._textSize,
									TextAlign = "left-middle"
								};
								if (!vrLabel.Resized)
								{
									vrLabel.Resize(this.vModel.Scale);
								}
								this.container.Controls.Add(vrLabel);
								top += num;
								vrLabel = new VrLabel()
								{
									Width = 300,
									Height = num,
									Left = left + 10,
									Top = top,
									Text = contestDefinition1.Data[l1 + 4].Text.ToUpper(),
									TextSize = ResultsView._textSize,
									TextAlign = "left-middle"
								};
								if (!vrLabel.Resized)
								{
									vrLabel.Resize(this.vModel.Scale);
								}
								this.container.Controls.Add(vrLabel);
								num7 = ((IEnumerable<DataRow>)dataV2.Select(string.Concat(new object[] { "cid=", contestDefinition1.Propositions[l1 / 5].ContestId, " AND did=", contestDefinition1.Data[l1 + 4].Id }))).Sum<DataRow>((DataRow r) => Convert.ToInt32(r["cnt"]));
								vrLabel = new VrLabel()
								{
									Width = num,
									Height = num,
									Left = vrLabel.Width + 200,
									Top = top,
									Text = num7.ToString(AppManager.DefaultCultureInfo),
									TextSize = ResultsView._textSize,
									TextAlign = "left-middle"
								};
								if (!vrLabel.Resized)
								{
									vrLabel.Resize(this.vModel.Scale);
								}
								this.container.Controls.Add(vrLabel);
								top += num;
							}
						}
					}
				}
			}
			if (top > this.container.Top + this.container.Height)
			{
				this._hasScroll = true;
			}
			this._textsToSpeak = base.GetAllTexts(true, false, false);
			this.SetNavigation();
			if (this._hasScroll)
			{
				if (this.container.CreateScroll(this.vModel, top - this.container.Top))
				{
					ScreenObject screenObject = this.vModel.Definition.ScreenObjects[this.vModel.Definition.ScreenObjects.Count - 2];
					if (screenObject.ObjectState == ScreenObject.ScreenObjectState.ACTIVE)
					{
						screenObject.Text = "Touch here to see less";
					}
					screenObject.Tag = "Scroll up";
					ScreenObject item1 = this.vModel.Definition.ScreenObjects[this.vModel.Definition.ScreenObjects.Count - 1];
					if (item1.ObjectState == ScreenObject.ScreenObjectState.ACTIVE)
					{
						item1.Text = "Touch here to see more";
					}
					item1.Tag = "Scroll down";
					this.container.Top = screenObject.Top + screenObject.Height + 10;
					this.container.Height = item1.Top - screenObject.Top - screenObject.Height - 20;
					if (!this.container.Resized)
					{
						this.container.Resize(this.vModel.Scale);
					}
					foreach (ScreenObject control in this.container.Controls)
					{
						ScreenObject top1 = control;
						top1.Top = top1.Top + screenObject.Height;
						if (control.Top + control.Height > this.container.Top + this.container.Height)
						{
							control.Visible = false;
						}
					}
					this._timer = new Timer()
					{
						Interval = VrContainer.GetScrollTimerIn(ScrollTimerTypes.ContestViewScrollTimerIn)
					};
					this._timer.Stop();
					this._timer.Tick += new EventHandler(this.TimerTick);
				}
			}
		}

		private bool IsReviewVisible()
        {
            return vModel.Ballot.ContestsList.Any(contest => contest.Selected > 0) || vModel.Ballot.slatesDefinition.Data.SlateId > 0;
        }

        private void TimerTick(object sender, EventArgs e)
        {
            TimerCount++;
            if (TimerCount > StartTick && vController.ScrObject != null)
                vController.ScrollContainer(vModel, vModel.GetView().Container, ScrollStepTypes.ContestViewScrollStep,
                                            vController.ScrObject.Data, (string)vModel.Vars["more"]);
        }

        public override void Update(ISubject subj)
        {
            //Logger.Instance.Write("contest view updated");
        }

        private void SetNavigation()
        {
            btnBack.Text = (string)vModel.Vars["back"];
            btnBack.Tag = (string)vModel.Vars["back"];
            btnBack.BgImage = btnBack.BgImage.Replace("_a", "_d");
            btnBack.Action = ScreenObject.ScreenObjectAction.GET_SCREEN;
            btnBack.Data = "pinpad";

            vModel.InvalidateObject(btnBack);

        }

		private DataTable GetCorrectOrderBallot(BallotDefinition pBallot)
		{
			DataTable dataTable = new DataTable();
			dataTable.Columns.Add(new DataColumn("GroupID", typeof(int)));
			dataTable.Columns.Add(new DataColumn("GroupName", typeof(string)));
			DataTable orderPositionRec = Contest.GetOrderPositionRec();
			for (int i = 0; i < pBallot.ContestsList.Count; i++)
			{
				string groupName = pBallot.ContestsList[i].GroupName;
				if (dataTable.Select(string.Concat("GroupName='", groupName, "'")).Length == 0)
				{
					DataRow dataRow = dataTable.NewRow();
					int num = -1;
					int id = pBallot.ContestsList[i].Id;
					DataRow[] dataRowArray = orderPositionRec.Select(string.Concat("contest_id=", id.ToString()));
					if (dataRowArray.Length != 0)
					{
						num = int.Parse(dataRowArray[0]["rvo_order_position"].ToString());
					}
					dataRow["GroupID"] = num;
					dataRow["GroupName"] = groupName;
					dataTable.Rows.Add(dataRow);
				}
			}
			dataTable.DefaultView.Sort = "GroupID";
			return dataTable.DefaultView.ToTable();
		}

	}
}
