using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using VotRite.UI;
using VotRite.Controllers;
using VotRite.Models;
using VotRite.MVC;
using VotRite.Definition;
using VotRiteBallotDataManager.AppCode;

namespace VotRite.Views
{

	class RankingChoiceView : VrScreen
	{
		private VrLabel lblBallotName;

		private VrLabel lblBallotTitle;

		private VrLabel lblBallotAddress;

		private VrLabel lblBallotDate;

		private VrLabel lblGroupName;

		private VrLabel lblContestName;

		private VrLabel lblContestTitle;

		private VrLabel lblChoicesLeft;

		private VrLabel lblContestTip;

		private VrLabel lblCount;

		private VrContainer container;

		private VrContainer buttoncontainer;

		private VrButton btnNext;

		private VrButton btnBack;

		private VrButton btnReview;

		private VrButton btnSettingTools;

		private const int SCREEN_V_SPACE = 20;

		private const int selectionsSpace = 20;

		private static int _textSize;

		private Timer _timer;

		private const int StartTick = 5;

		private List<string> _textsToSpeak;

		private RaceViewOptions raceViewOptions = null;

		private bool _hasScroll = false;

		private int candCount;

		public VrContainer Container
		{
			get
			{
				return this.container;
			}
		}

		protected internal int TimerCount
		{
			get;
			set;
		}

		private RankingChoiceController vController
		{
			get
			{
				return this.controller as RankingChoiceController;
			}
		}

		private RankingChoiceModel vModel
		{
			get
			{
				return this.model as RankingChoiceModel;
			}
		}

		static RankingChoiceView()
		{
			RankingChoiceView._textSize = 12;
		}

		public RankingChoiceView(RankingChoiceModel m, RankingChoiceController c) : base(m, c)
		{
			this.model = m;
			this.controller = c;
			this.lblBallotName = (VrLabel)this.vModel.FindScreenObject("lbl_blt_name");
			this.lblBallotTitle = (VrLabel)this.vModel.FindScreenObject("lbl_blt_title");
			this.lblBallotAddress = (VrLabel)this.vModel.FindScreenObject("lbl_blt_address");
			this.lblBallotDate = (VrLabel)this.vModel.FindScreenObject("lbl_blt_date");
			this.lblGroupName = (VrLabel)this.vModel.FindScreenObject("lbl_group_name");
			this.lblContestName = (VrLabel)this.vModel.FindScreenObject("lbl_contest_name");
			this.lblContestTitle = (VrLabel)this.vModel.FindScreenObject("lbl_contest_title");
			this.lblChoicesLeft = (VrLabel)this.vModel.FindScreenObject("lbl_choices_left");
			this.lblContestTip = (VrLabel)this.vModel.FindScreenObject("lbl_contest_tip");
			this.lblCount = (VrLabel)this.vModel.FindScreenObject("lbl_contest_count");
			this.container = (VrContainer)this.vModel.FindScreenObject("ctr_contest");
			this.buttoncontainer = (VrContainer)vModel.FindScreenObject("ctr_buttons");
			this.btnNext = (VrButton)this.vModel.FindScreenObject("btn_next");
			this.btnBack = (VrButton)this.vModel.FindScreenObject("btn_back");
			this.btnReview = (VrButton)this.vModel.FindScreenObject("btn_review");
			this.btnSettingTools = (VrButton)this.vModel.FindScreenObject("btn_setting_tools");
			this.raceViewOptions = new RaceViewOptions(new int?(this.vModel.Contest.Id));
			RankingChoiceView._textSize = (this.raceViewOptions.UseDefaultSettings ? Convert.ToInt32(AppManager.Configuration["Contest"]["RaceFontSize"]) : this.raceViewOptions.FontSize);
			
			this.Initialize();

			setBallotTextForLocale();

			AppManager.Instance.checkDoubleSpaceSetting(vModel.Definition.ScreenObjects, container);

			//foreach (var cntrl in this.vModel.Definition.ScreenObjects)

			//{
			//	if (cntrl != null)
			//	{
			//		if (screenObj != null)
			//		{
			//			cntrl.TextSize = screenObj.TextSize;
			//			if (cntrl == lblChoicesLeft)
			//				lblChoicesLeft.Top += (screenObj.TextSize - 18) * 10;
			//			if (cntrl == lblContestTip)
			//			{
			//				lblContestTip.Top += (screenObj.TextSize - 18) * 10;
			//				lblContestTip.Height += (screenObj.TextSize - 18);
			//				if (AppManager.Instance.isDoubleSpacing)
			//					lblContestTip.Height += 50;
			//			}
			//			if (cntrl == lblContestTitle)
			//			{
			//				lblContestTitle.Top += (screenObj.TextSize - 18) * 5;
			//				lblContestTitle.Height += (screenObj.TextSize - 18);
			//			}
			//			if (cntrl == lblCount)
			//			{
			//				lblCount.Top += (screenObj.TextSize - 18) * 10;
			//				// lblCount.Height += (screenObj.TextSize - 18);
			//			}

			//		}
			//		cntrl.Resize(vModel.Scale);
			//	}
			//}

			var additionalSet = new HashSet<string> { (string)vModel.Vars["next"], (string)vModel.Vars["skip"], (string)vModel.Vars["review"], (string)vModel.Vars["back"] };
			CreateListOfCommands(additionalSet);

			foreach (string str in
				from text in this._textsToSpeak
				where text != null
				select text)
			{
				AppManager.Instance.StartSpeaker(str);
			}

			AppManager.Instance.initiateHearingMode();
			AppManager.Instance.controllerForHearingMode = vController;
		}

		protected override void Dispose(bool disposing)
		{
			if (disposing)
			{
				this._timer.Dispose();
				this._textsToSpeak.Clear();
				if (this.lblBallotName != null)
				{
					this.lblBallotName.Dispose();
				}
				if (this.lblBallotTitle != null)
				{
					this.lblBallotTitle.Dispose();
				}
				if (this.lblBallotAddress != null)
				{
					this.lblBallotAddress.Dispose();
				}
				if (this.lblBallotDate != null)
				{
					this.lblBallotDate.Dispose();
				}
				if (this.lblGroupName != null)
				{
					this.lblGroupName.Dispose();
				}
				if (this.lblContestName != null)
				{
					this.lblContestName.Dispose();
				}
				if (this.lblContestTitle != null)
				{
					this.lblContestTitle.Dispose();
				}
				if (this.lblContestTip != null)
				{
					this.lblContestTip.Dispose();
				}
				if (this.lblCount != null)
				{
					this.lblCount.Dispose();
				}
				if (this.btnNext != null)
				{
					this.btnNext.Dispose();
				}
				if (this.container != null)
				{
					this.container.Dispose();
				}
			}
			base.Dispose(disposing);
		}

		public Timer GetTimer()
		{
			return this._timer;
		}

		private void Initialize()
		{
			if (AppManager.Instance.backgroundTheme != AppManager.colorTheme.White)
			{
				vModel.Definition.Background = AppManager.GetPathToCommonFile("graphics" + Global.Instance.SLASH + "initialBG_f1_" + AppManager.Instance.backgroundTheme + ".png");
				buttoncontainer.BgColor = null;
			}
			if (this.btnReview != null)
			{
				this.btnReview.Text = (string)this.vModel.Vars["review"];
				btnReview.Top -= 10;
			}
			if (lblChoicesLeft != null)
				lblChoicesLeft.BgColor = null;
			if (this.btnSettingTools != null)
			{
				this.btnSettingTools.Text = (string)this.vModel.Vars["btn_setting_tools"];
				this.btnSettingTools.Visible = this.vModel.Ballot.ShowSettingsAndTools;
			}
			if (this.vModel.Ballot.HasOverview)
			{
				VrButton vrButton = new VrButton()
				{
					Name = "btn_callhelp",
					BgImage = AppManager.GetPathToCommonFile(string.Concat("graphics", Global.Instance.SLASH, "help_light.png")),
					Text = "     Ballot",
					ForeColor = "#fff",
					TextSize = 18,
					TextAlign = "center-middle",
					Width = 150,
					Height = 63,
					Left = 25,
					Top = 780,
					Action = ScreenObject.ScreenObjectAction.GET_SCREEN,
					Data = "ballotoverview"
				};
				this.vModel.Definition.ScreenObjects.Add(vrButton);
			}
			if (this.btnBack != null)
			{
				this.btnBack.Text = (string)this.vModel.Vars["back"];
			}
			if (this.btnNext != null)
			{
				this.btnNext.Text = (string)this.vModel.Vars["next"];
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
				this.lblGroupName.Text = (string)this.vModel.Vars["group_name"];
			}
			if (this.lblContestName != null)
			{
				this.lblContestName.Text = (string)this.vModel.Vars["contest_name"];
			}
			if (this.lblContestTitle != null)
			{
				this.lblContestTitle.TextAlign = "left-top";
				if ((this.vModel.Contest.MinSelection <= 0 ? true : string.IsNullOrEmpty((string)this.vModel.Vars["min_race_tip"])))
				{
					this.lblContestTitle.Text = string.Empty;
				}
				else
				{
					this.lblContestTitle.Text = string.Format("{0} ({1}). ", this.vModel.Vars["min_race_tip"], this.vModel.Contest.MinSelection).Replace("touch", "choose");
				}
				VrLabel vrLabel = this.lblContestTitle;
				vrLabel.Text = string.Concat(vrLabel.Text, (string.IsNullOrEmpty((string)this.vModel.Vars["contest_title"]) ? string.Empty : string.Format("{0} ({1}).", this.vModel.Vars["contest_title"], this.vModel.Contest.MaxSelection)));
				this.lblContestName.Top = lblContestName.Top - 20;
				lblContestTitle.Top = lblContestTitle.Top + 10;
				lblContestTitle.TextFont = new System.Drawing.Font("Arial Unicode MS", 6f);
			}
			this.UpdateLblChoicesLeft(false);
			if (this.lblContestTip != null)
			{
				this.lblContestTip.Text = "Please assign preferences to candidates by using '+' and '-' buttons";
			}
			if ((this.lblCount == null ? false : this.vModel.Vars["contest_count"] != null))
			{
				this.lblCount.Text = string.Concat(new object[] { this.vModel.Ballot.ActiveContest + 1, " ", this.vModel.Vars["contest_count"].ToString(), " ", this.vModel.Ballot.ContestsList.Count });
				lblCount.ForeColor = "#000";
			}
			int top = this.container.Top + 30;
			int left = this.container.Left + 10;
			int num = 0;
			int num1 = (this.raceViewOptions.UseDefaultSettings ? 60 : this.raceViewOptions.RowHeight);
			int num2 = 0;
			int num3 = 15;
			if (AppManager.Configuration["Contest"]["ImageSize"].ToLower() == "l")
			{
				num1 += 40;
				num3 = 30;
			}
			if (AppManager.Configuration["Contest"]["ImageSize"].ToLower() == "s")
			{
				num1 -= 20;
				num3 = 10;
			}
			foreach (DataDefinition datum in this.vModel.Contest.Data)
			{
				ScreenObject tag = new VrLabel()
				{
					Width = this.container.Width / 2 + this.container.Width / 2 / 2 - num1,
					Height = num1,
					Left = left,
					Top = top,
					Tag = datum.Text,
					BgColor = "#fff",
					BorderWidth = 2,
					BorderColor = "#000000",
					Text = datum.Text,
					TextAlign = "left-middle",
					TextSize = RankingChoiceView._textSize,
					Photo = datum.Photo,
					PartyLogo = datum.PartyLogo
				};
				if (AppManager.Configuration["Contest"]["ImageSize"] == "L")
				{
					ScreenObject width = tag;
					width.Width = width.Width - num1 / 2;
				}
				if (datum.WriteIn)
				{
					tag.Data = "writein";
					tag.Tag = datum.Tag;
				}
				if ((!datum.WriteIn ? true : datum.IsWritten))
				{
					this.candCount++;
				}
				else
				{
					tag.Text = (string)this.vModel.Vars["btn_add_candidates"];
					if (datum.IsWritten)
					{
						this.candCount++;
					}
				}
				left = left + tag.Width + 20;
				AppManager.Instance.ChangeTextSize(tag);
				if (!tag.Resized)
				{
					tag.Resize(this.vModel.Scale);
				}
				
				this.container.Controls.Add(tag);
				if (!datum.Voteable)
				{
					left = this.container.Left + 10;
				}
				else
				{
					VrLabel name = new VrLabel()
					{
						Width = num1,
						Height = num1,
						Left = left,
						Top = top
					};
					int id = datum.Id;
					name.Name = string.Concat("lblPreference_", id.ToString());
					name.Tag = base.Name;
					name.Text = (datum.Preference == 0 ? "" : datum.Preference.ToString());
					name.TextSize = RankingChoiceView._textSize;
					name.BgColor = "#fff";
					name.BorderColor = "#cccccc";
					name.BorderWidth = 2;
					tag = name;
					Candidate candidate = null;
					int preference = 0;
					foreach (Candidate candidatesList in this.vModel.Contest.CandidatesList)
					{
						if (candidatesList.Id != datum.Id)
						{
							preference++;
						}
						else
						{
							candidate = candidatesList;
							this.vModel.Contest.CandidatesList[preference].Preference = datum.Preference;
							break;
						}
					}
					left = left + tag.Width + 5;
					AppManager.Instance.ChangeTextSize(tag);
					if (!tag.Resized)
					{
						tag.Resize(this.vModel.Scale);
					}
					this.container.Controls.Add(tag);
					tag = new VrLabel()
					{
						Width = num1,
						Height = num1,
						Left = left,
						Top = top,
						Text = "+",
						Tag = datum.Id.ToString(),
						TextSize = 28,
						BgColor = "#cccccc",
						BorderColor = "#000000",
						BorderWidth = 2
					};
					left = left + tag.Width + 5;
					AppManager.Instance.ChangeTextSize(tag);
					if (!tag.Resized)
					{
						tag.Resize(this.vModel.Scale);
					}
					this.container.Controls.Add(tag);
					tag = new VrLabel()
					{
						Width = num1,
						Height = num1,
						Left = left,
						Top = top,
						Text = "-",
						Tag = datum.Id.ToString(),
						TextSize = 28,
						BgColor = "#cccccc",
						BorderColor = "#000000",
						BorderWidth = 2
					};
					left = this.container.Left + 10;
					AppManager.Instance.ChangeTextSize(tag);
					if (!tag.Resized)
					{
						tag.Resize(this.vModel.Scale);
					}
					this.container.Controls.Add(tag);
				}
				top = top + num1 + num3;
				num2 = Math.Max(num2, top);
				num++;
			}
			if (top > this.container.Top + this.container.Height)
			{
				this._hasScroll = true;
			}
			this.SetNavigation();

			foreach (var cntrl in this.vModel.Definition.ScreenObjects)

			{
				if (cntrl != null)
				{
					AppManager.Instance.ChangeTextSize(cntrl);
					if (cntrl is VrContainer)
						continue;
					if (cntrl == lblBallotName)
					{
						lblBallotName.Top += (cntrl.TextSize - 18);
						lblBallotName.Height += (cntrl.TextSize - 18) * 2;
					}
					if (cntrl == lblBallotTitle)
					{
						lblBallotTitle.Top += (cntrl.TextSize - 18) * 2;
						lblBallotTitle.Height += (cntrl.TextSize - 18)*2;
						//if (AppManager.Instance.isDoubleSpacing)
						//	lblContestTip.Height += 50;
					}
					if (cntrl == lblBallotAddress)
					{
						lblBallotAddress.Top += (cntrl.TextSize - 18);
						lblBallotAddress.Height += (cntrl.TextSize - 18)*2;
					}
					if (cntrl == lblBallotDate)
					{
						lblBallotDate.Top += (cntrl.TextSize - 18) * 2;
						lblBallotDate.Height += (cntrl.TextSize - 18) * 2;
						lblBallotDate.Visible = false;
					}

					if(cntrl == lblContestTitle)
                    {
						lblContestTitle.Top -= (cntrl.TextSize - 18) ;
						lblContestTitle.Height += (cntrl.TextSize - 18)*2;
					}
					if (cntrl == btnReview)
					{
						//btnReview.Top -= (cntrl.TextSize - 18) * 4;
						btnReview.Height += (cntrl.TextSize - 18) * 4;
						btnReview.Width -= (cntrl.TextSize - 18);
					}
					cntrl.Resize(vModel.Scale);
				}
			}

			this._textsToSpeak = base.GetAllTexts(true, false, false);
			//_textsToSpeak.Add("say check to review your choices");
			//_textsToSpeak.Add("say zero to repeat instruction");

			if (this._hasScroll)
			{
				if ((!Convert.ToBoolean(AppManager.Configuration["Contest"]["scroll"]) ? false : this.container.CreateScroll(this.vModel, num2 - this.container.Top)))
				{
					ScreenObject item = this.vModel.Definition.ScreenObjects[this.vModel.Definition.ScreenObjects.Count - 2];
					if (item.ObjectState == ScreenObject.ScreenObjectState.ACTIVE)
					{
						item.Text = (string)this.vModel.Vars["more"];
					}
					item.Tag = "Scroll up";
					ScreenObject screenObject = this.vModel.Definition.ScreenObjects[this.vModel.Definition.ScreenObjects.Count - 1];
					if (screenObject.ObjectState == ScreenObject.ScreenObjectState.ACTIVE)
					{
						screenObject.Text = (string)this.vModel.Vars["more"];
					}
					screenObject.Tag = "Scroll down";
					item.Top = this.lblContestTitle.Top + this.lblContestTitle.Height + 10;
					this.container.Top = item.Top + item.Height + 10;
					this.container.Height = screenObject.Top - item.Top - item.Height - 20;
					if (!this.container.Resized)
					{
						this.container.Resize(this.vModel.Scale);
					}
					foreach (ScreenObject control in this.container.Controls)
					{
						ScreenObject top1 = control;
						top1.Top = top1.Top + item.Height;
						control.Visible = control.Top + control.Height < this.container.Top + this.container.Height;
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

		private void setBallotTextForLocale()
        {
			if(AppManager.Instance.Session.CurrentLocale.Code != "en-US")
            {
				var tranBallot = Ballot.GetBallotTranslated(AppManager.Instance.Session.Ballot.Id, AppManager.Instance.Session.CurrentLocale.Id);
				if(tranBallot != null)
                {
                    try
                    {
						if (this.lblBallotName != null)
							this.lblBallotName.Text = tranBallot.TopHeading;
						if (this.lblBallotTitle != null)
							this.lblBallotTitle.Text = tranBallot.ElectionName;
						if(this.lblBallotAddress != null)
							this.lblBallotAddress.Text = tranBallot.Location;
					}
                    catch (Exception ex)
                    {
 
                    }
                }
            }
        }
		private bool IsReviewVisible()
		{
			return (this.vModel.Ballot.ContestsList.Any<ContestDefinition>((ContestDefinition contest) => contest.Selected > 0) ? true : this.vModel.Ballot.slatesDefinition.Data.SlateId > 0);
		}

		private void SetNavigation()
		{
			int num = (
				from c in this.vModel.Contest.CandidatesList
				where c.Preference > 0
				select c).Count<Candidate>();
			if ((num < this.vModel.Contest.MinSelection ? true : num > this.vModel.Contest.MaxSelection))
			{
				this.btnNext.Text = (string)this.vModel.Vars["skip"];
				this.btnNext.Tag = (string)this.vModel.Vars["next"];
				this.btnNext.BgImage = this.btnNext.BgImage.Replace("_a", "_d");
				this.btnNext.Action = ScreenObject.ScreenObjectAction.GET_SCREEN;
				this.btnNext.Data = "review";
			}
			else
			{
				this.btnNext.Text = (string)this.vModel.Vars["next"];
				this.btnNext.Tag = (string)this.vModel.Vars["skip"];
				this.btnNext.BgImage = this.btnNext.BgImage.Replace("_d", "_a");
			}
			this.vModel.InvalidateObject(this.btnNext);
		}

		public void SetPreference(VrLabel label)
		{
			if ((label.Text == "+" ? true : label.Text == "-"))
			{
				Candidate candidate = this.vModel.Contest.CandidatesList.Find((Candidate c) => c.Id.ToString() == label.Tag);
				if (candidate != null)
				{
					int num = this.vModel.Contest.Data.FindIndex((DataDefinition d) => d.Id == candidate.Id);
					if (num >= 0)
					{
						DataDefinition item = this.vModel.Contest.Data[num];
						VrLabel str = (VrLabel)this.container.FindControlByName(string.Concat("lblPreference_", label.Tag));
						if (str != null)
						{
							int num1 = (string.IsNullOrEmpty(str.Text) ? 0 : Convert.ToInt32(str.Text));
							num1 = (label.Text == "+" ? num1 + 1 : num1 - 1);
							if (num1 == 1)
							{
								ContestDefinition contest = this.vModel.Contest;
								contest.Selected = contest.Selected + 1;
							}
							if (num1 <= 0)
							{
								str.Text = "";
								if (candidate.Preference > 0)
								{
									candidate.Preference = 0;
									ContestDefinition selected = this.vModel.Contest;
									selected.Selected = selected.Selected - 1;
									this.vModel.Contest.Data[num].Preference = 0;
								}
							}
							else if (num1 <= this.candCount)
							{
								str.Text = num1.ToString();
								candidate.Preference = num1;
								this.vModel.Contest.Data[num].Preference = num1;
							}
							this.UpdateLblChoicesLeft(true);
							this.vModel.UpdateObject(str);
							this.SetNavigation();
						}
					}
				}
			}
		}

		public void SetSelection(VrLabel label)
		{
			int state;
			if (this.vModel.Contest.MaxSelectionPerGroup <= 0)
			{
				if (this.vModel.Contest.Selected < this.vModel.Contest.MaxSelection)
				{
					ContestDefinition contest = this.vModel.Contest;
					contest.Selected = contest.Selected + 1;
				}
				else if ((this.vModel.Contest.MaxSelection != 1 ? false : this.vModel.Contest.MaxSelectionPerGroup == 0))
				{
					foreach (DataDefinition datum in this.vModel.Contest.Data)
					{
						if (datum.State == VrSelection.SelectionState.SELECTED)
						{
							datum.State = VrSelection.SelectionState.DESELECTED;
							ContestDefinition selected = this.vModel.Contest;
							selected.Selected = selected.Selected - 1;
							VrSelection item = (VrSelection)this.container.Controls[this.vModel.Contest.LastSelectedIndex];
							if (item != null)
							{
								item.State = VrSelection.SelectionState.DESELECTED;
								this.vModel.UpdateObject(item);
							}
							break;
						}
					}
					ContestDefinition contestDefinition = this.vModel.Contest;
					contestDefinition.Selected = contestDefinition.Selected + 1;
				}
			}
			else if (0 < this.vModel.Contest.MaxSelectionPerGroup)
			{
				ContestDefinition contest1 = this.vModel.Contest;
				contest1.Selected = contest1.Selected + 1;
			}
			else if (this.vModel.Contest.MaxSelectionPerGroup == 1)
			{
				using (IEnumerator<VrSelection> enumerator = (
					from control in this.container.Controls.OfType<VrSelection>()
					select control into selectionControl
					where selectionControl.State == VrSelection.SelectionState.SELECTED
					select selectionControl).GetEnumerator())
				{
					if (enumerator.MoveNext())
					{
						VrSelection current = enumerator.Current;
						current.State = VrSelection.SelectionState.DESELECTED;
						ContestDefinition selected1 = this.vModel.Contest;
						selected1.Selected = selected1.Selected - 1;
						if ((!int.TryParse(current.Name.Replace("sel_", ""), out state) || state < 0 ? true : state >= this.vModel.Contest.Data.Count))
						{
							this.vModel.Contest.FindDataByTag(current.Tag).State = current.State;
						}
						else
						{
							this.vModel.Contest.Data[state].State = current.State;
						}
						this.vModel.UpdateObject(current);
					}
				}
				ContestDefinition contestDefinition1 = this.vModel.Contest;
				contestDefinition1.Selected = contestDefinition1.Selected + 1;
			}
			int num = int.Parse(label.Tag.Substring(label.Tag.LastIndexOf("_") + 1));
			if (num < 0)
			{
				num = this.vModel.Contest.FindDataIndex(label.Tag);
			}
			if (num < 0)
			{
				num = this.vModel.Contest.FindDataIndexByTag(label.Tag);
			}
			DataDefinition text = this.vModel.Contest.FindData(label.Tag) ?? this.vModel.Contest.FindDataByTag(label.Tag);
			if (text != null)
			{
				text.Text = label.Text;
			}
			this.vModel.Contest.LastSelectedIndex = num;
			this.UpdateLblChoicesLeft(true);
			if (label.Data != "writein")
			{
				if (this.btnNext != null)
				{
					this.SetNavigation();
					if (!this.btnNext.Visible)
					{
						this.vModel.InvalidateObject(this.btnNext);
					}
					else
					{
						this.vModel.UpdateObject(this.btnNext);
					}
				}
				if (this.btnReview != null)
				{
					this.SetNavigation();
					if (!this.btnReview.Visible)
					{
						this.vModel.InvalidateObject(this.btnReview);
					}
					else
					{
						this.vModel.UpdateObject(this.btnReview);
					}
				}
				this.vModel.UpdateObject(label);
			}
			else
			{
				AppManager.Instance.SetScreen("writein");
			}
		}

		private void TimerTick(object sender, EventArgs e)
		{
			this.TimerCount = this.TimerCount + 1;
			if ((this.TimerCount <= 5 ? false : this.vController.ScrObject != null))
			{
				this.vController.ScrollContainer(this.vModel, this.vModel.GetView().Container, ScrollStepTypes.ContestViewScrollStep, this.vController.ScrObject.Data, (string)this.vModel.Vars["more"]);
			}
		}

		public override void Update(ISubject subj)
		{
		}

		private void UpdateLblChoicesLeft(bool updateObject)
		{
			string str;
			if (this.lblChoicesLeft != null)
			{
				int maxSelection = this.vModel.Contest.MaxSelection - this.vModel.Contest.Selected;
				str = (maxSelection == 1 ? string.Format("{0} {1} {2}.", this.vModel.Vars["lbl_choice1"], Convert.ToString(maxSelection), this.vModel.Vars["lbl_choice2"]) : string.Format("{0} {1} {2}.", this.vModel.Vars["lbl_choice1"], Convert.ToString(maxSelection), this.vModel.Vars["lbl_multi_0_choices2"]));
				this.lblChoicesLeft.Text = str;
				this.lblChoicesLeft.ForeColor = "#347409";
				if (updateObject)
				{
					if (maxSelection <= 0)

						AppManager.Instance.StartSpeaker(string.Concat(new object[] { str, ". Say ", this.vModel.Vars["next"], " to continue" }));
					else
						AppManager.Instance.StartSpeaker(str);

					var currLbl = lblChoicesLeft;
					vModel.InvalidateObject(lblChoicesLeft);
					lblChoicesLeft = (VrLabel)vModel.FindScreenObject("lbl_choices_left");
					if (lblChoicesLeft != null)
					{
						lblChoicesLeft.Left = currLbl.Left;
						lblChoicesLeft.Top = currLbl.Top;
						lblChoicesLeft.Height = currLbl.Height;
						lblChoicesLeft.Width = currLbl.Width;
						lblChoicesLeft.BgColor = null;
						this.lblChoicesLeft.ForeColor = "#347409";
						lblChoicesLeft.Text = str;
					}
				}
				//this.lblChoicesLeft.Visible = false;
			}
		}
	}


	//class RankingChoiceView : VrScreen
	//{
	//    private RankingChoiceModel vModel { get { return model as RankingChoiceModel; } }
	//    private RankingChoiceController vController { get { return controller as RankingChoiceController; } }
	//    private VrLabel lblBallotName;
	//    private VrLabel lblBallotTitle;
	//    private VrLabel lblBallotAddress;
	//    private VrLabel lblBallotDate;
	//    private VrLabel lblGroupName;
	//    private VrLabel lblContestName;
	//    private VrLabel lblContestTitle;
	//    private VrLabel lblContestTip;
	//    private VrLabel lblCount;
	//    private VrContainer container;
	//    private VrButton btnNext;

	//    public VrContainer Container { get { return container; } }

	//    private const int SCREEN_V_SPACE = 20;
	//    private const int selectionsSpace = 20;

	//    private static int _textSize = 12;
	//    private Timer _timer;
	//    private const int StartTick = 5;
	//    protected internal int TimerCount { get; set; }
	//    public Timer GetTimer()
	//    {
	//        return _timer;
	//    }

	//    private List<string> _textsToSpeak;

	//    private RaceViewOptions raceViewOptions = null;
	//    private bool _hasScroll = false;

	//    private int candCount;

	//    public RankingChoiceView(RankingChoiceModel m, RankingChoiceController c)
	//        : base(m, c)
	//    {
	//        model = m;
	//        controller = c;

	//        lblBallotName = (VrLabel)vModel.FindScreenObject("lbl_blt_name");
	//        lblBallotTitle = (VrLabel)vModel.FindScreenObject("lbl_blt_title");
	//        lblBallotAddress = (VrLabel)vModel.FindScreenObject("lbl_blt_address");
	//        lblBallotDate = (VrLabel)vModel.FindScreenObject("lbl_blt_date");
	//        lblGroupName = (VrLabel)vModel.FindScreenObject("lbl_group_name");
	//        lblContestName = (VrLabel)vModel.FindScreenObject("lbl_contest_name");
	//        lblContestTitle = (VrLabel)vModel.FindScreenObject("lbl_contest_title");
	//        lblContestTip = (VrLabel)vModel.FindScreenObject("lbl_contest_tip");
	//        lblCount = (VrLabel)vModel.FindScreenObject("lbl_contest_count");
	//        container = (VrContainer)vModel.FindScreenObject("ctr_contest");
	//        btnNext = (VrButton)vModel.FindScreenObject("btn_next");

	//        raceViewOptions = new RaceViewOptions(vModel.Contest.Id);
	//        _textSize = raceViewOptions.UseDefaultSettings ?
	//            Convert.ToInt32(AppManager.Configuration["Contest"]["RaceFontSize"]) : raceViewOptions.FontSize;

	//        Initialize();

	//        foreach (var text in _textsToSpeak.Where(text => text != null))
	//        {
	//            AppManager.Instance.StartSpeaker(text);
	//        }
	//    }

	//    protected override void Dispose(bool disposing)
	//    {
	//        if (disposing)
	//        {
	//            _timer.Dispose();
	//            _textsToSpeak.Clear();

	//            if (lblBallotName != null) lblBallotName.Dispose();
	//            if (lblBallotTitle != null) lblBallotTitle.Dispose();
	//            if (lblBallotAddress != null) lblBallotAddress.Dispose();
	//            if (lblBallotDate != null) lblBallotDate.Dispose();
	//            if (lblGroupName != null) lblGroupName.Dispose();
	//            if (lblContestName != null) lblContestName.Dispose();
	//            if (lblContestTitle != null) lblContestTitle.Dispose();
	//            if (lblContestTip != null) lblContestTip.Dispose();
	//            if (lblCount != null) lblCount.Dispose();
	//            if (btnNext != null) btnNext.Dispose();
	//            if (container != null) container.Dispose();
	//        }
	//        base.Dispose(disposing);
	//    }

	//    private void Initialize()
	//    {
	//        if (btnNext != null)
	//            btnNext.Tag = (string)vModel.Vars["next"];

	//        if (lblBallotName != null)
	//            lblBallotName.Text = (string)vModel.Vars["ballot_name"];
	//        if (lblBallotTitle != null)
	//            lblBallotTitle.Text = (string)vModel.Vars["ballot_title"];
	//        if ((string)vModel.Vars["ballot_address"] != "")
	//            lblBallotAddress.Text = (string)vModel.Vars["ballot_address"];
	//        if (lblBallotDate != null)
	//            lblBallotDate.Text = (string)vModel.Vars["ballot_date"];
	//        if (lblGroupName != null)
	//            lblGroupName.Text = "";
	//        if (lblContestName != null)
	//            lblContestName.Text = "";

	//        lblContestTitle.TextAlign = "left-top";

	//        if (lblContestTip != null)
	//        {
	//            lblContestTip.Text = "Please assign preferences to candidates by using '+' and '-' buttons";
	//        }

	//        if (lblCount != null && vModel.Vars["contest_count"] != null)
	//            lblCount.Text = (vModel.Ballot.ActiveContest + 1) +
	//                " " + vModel.Vars["contest_count"].ToString() +
	//                " " + vModel.Ballot.ContestsList.Count;

	//        int topHist = container.Top + 30;
	//        int leftHist = container.Left + 10;
	//        int n = 0;
	//        int objHeight = raceViewOptions.UseDefaultSettings ? 80 : raceViewOptions.RowHeight;
	//        int maxHeight = 0;
	//        ScreenObject screenObj;
	//        int wiInx = 0;
	//        int vSpace = 15;

	//        if (AppManager.Configuration["Contest"]["ImageSize"].ToLower() == "l")
	//        {
	//            objHeight += 40;
	//            vSpace = 30;
	//        }
	//        if (AppManager.Configuration["Contest"]["ImageSize"].ToLower() == "s")
	//        {
	//            objHeight -= 20;
	//            vSpace = 10;
	//        }

	//        foreach (DataDefinition data in vModel.Contest.Data)
	//        {
	//            screenObj = new VrLabel()
	//            {
	//                Width = ((container.Width / 2) + ((container.Width / 2) / 2)) - objHeight,
	//                Height = objHeight,
	//                Left = leftHist,
	//                Top = topHist,
	//                Tag = data.Text,
	//                BgColor = "#fff",
	//                BorderWidth = 2,
	//                BorderColor = "#000000",
	//                Text = (string)vModel.Vars["contest_data_" + Convert.ToString(n + 1)],
	//                TextAlign = "left-middle",
	//                TextSize = _textSize,
	//                Photo = data.Photo,
	//                PartyLogo = data.PartyLogo
	//            };

	//            if (AppManager.Configuration["Contest"]["ImageSize"] == "L")
	//            {
	//                screenObj.Width -= objHeight/2;
	//            }

	//            if (data.WriteIn)
	//            {
	//                screenObj.Data = "writein";
	//                screenObj.Tag += "_" + wiInx.ToString();
	//                wiInx++;
	//            }

	//            if (data.WriteIn && !data.IsWritten)
	//            {
	//                screenObj.Text = (string)vModel.Vars[Locale.BtnAddCandidatesField];

	//                if (data.IsWritten)
	//                {
	//                    ++candCount;
	//                }
	//            } else
	//            {
	//                ++candCount;
	//            }

	//            leftHist += screenObj.Width + 20;

	//            if (!screenObj.Resized)
	//            {
	//                screenObj.Resize(vModel.Scale);
	//            }

	//            container.Controls.Add(screenObj);

	//            if (data.Voteable)
	//            {

	//                screenObj = new VrLabel()
	//                {
	//                    Width = objHeight,
	//                    Height = objHeight,
	//                    Left = leftHist,
	//                    Top = topHist,
	//                    Name = "lblPreference_" + data.Id.ToString(),
	//                    Tag = Name,
	//                    Text = (data.Preference == 0) ? "" : data.Preference.ToString(),
	//                    TextSize = _textSize,
	//                    BgColor = "#fff",
	//                    BorderColor = "#cccccc",
	//                    BorderWidth = 2
	//                };

	//                Candidate candidate = null;
	//                int inx = 0;

	//                foreach (Candidate c in vModel.Contest.CandidatesList)
	//                {
	//                    if (c.Id == data.Id)
	//                    {
	//                        candidate = c;
	//                        vModel.Contest.CandidatesList[inx].Preference = data.Preference;
	//                        break;
	//                    }
	//                    inx++;
	//                }

	//                leftHist += screenObj.Width + 5;

	//                if (!screenObj.Resized)
	//                {
	//                    screenObj.Resize(vModel.Scale);
	//                }

	//                container.Controls.Add(screenObj);

	//                screenObj = new VrLabel()
	//                {
	//                    Width = objHeight,
	//                    Height = objHeight,
	//                    Left = leftHist,
	//                    Top = topHist,
	//                    Text = "+",
	//                    Tag = data.Id.ToString(),
	//                    TextSize = 28,
	//                    BgColor = "#cccccc",
	//                    BorderColor = "#000000",
	//                    BorderWidth = 2
	//                };

	//                leftHist += screenObj.Width + 5;

	//                if (!screenObj.Resized)
	//                {
	//                    screenObj.Resize(vModel.Scale);
	//                }

	//                container.Controls.Add(screenObj);

	//                screenObj = new VrLabel()
	//                {
	//                    Width = objHeight,
	//                    Height = objHeight,
	//                    Left = leftHist,
	//                    Top = topHist,
	//                    Text = "-",
	//                    Tag = data.Id.ToString(),
	//                    TextSize = 28,
	//                    BgColor = "#cccccc",
	//                    BorderColor = "#000000",
	//                    BorderWidth = 2
	//                };

	//                leftHist = container.Left + 10;

	//                if (!screenObj.Resized)
	//                {
	//                    screenObj.Resize(vModel.Scale);
	//                }

	//                container.Controls.Add(screenObj);
	//            } else
	//            {
	//                leftHist = container.Left + 10;
	//            }

	//            topHist += objHeight + vSpace;

	//            if (topHist > container.Top + container.Height)
	//            {
	//                if (!_hasScroll) _hasScroll = true;
	//            }

	//            maxHeight = Math.Max(maxHeight, topHist);
	//            n++;
	//        }

	//        SetNavigation();

	//        _textsToSpeak = GetAllTexts(true, false, false);

	//        if (_hasScroll)
	//        {
	//            if (Convert.ToBoolean(AppManager.Configuration["Contest"]["scroll"]) && container.CreateScroll(vModel, maxHeight - container.Top))
	//            {
	//                var upScroll = vModel.Definition.ScreenObjects[vModel.Definition.ScreenObjects.Count - 2];

	//                if (upScroll.ObjectState == ScreenObject.ScreenObjectState.ACTIVE)
	//                    upScroll.Text = (string)vModel.Vars["more"];

	//                upScroll.Tag = "Scroll up";

	//                var downScroll = vModel.Definition.ScreenObjects[vModel.Definition.ScreenObjects.Count - 1];

	//                if (downScroll.ObjectState == ScreenObject.ScreenObjectState.ACTIVE)
	//                    downScroll.Text = (string)vModel.Vars["more"];

	//                downScroll.Tag = "Scroll down";

	//                container.Top += upScroll.Height;
	//                container.Height -= upScroll.Height;
	//                container.Height -= downScroll.Height;

	//                if (!container.Resized) container.Resize(vModel.Scale);

	//                foreach (var screenObject in container.Controls)
	//                {
	//                    screenObject.Top += upScroll.Height;
	//                    if (screenObject.Top + screenObject.Height > container.Top + container.Height)
	//                        screenObject.Visible = false;
	//                }
	//                _timer = new Timer { Interval = VrContainer.GetScrollTimerIn(ScrollTimerTypes.ContestViewScrollTimerIn) };
	//                _timer.Stop();
	//                _timer.Tick += TimerTick;
	//            }
	//        }
	//    }

	//    private bool IsReviewVisible()
	//    {
	//        return vModel.Ballot.ContestsList.Any(contest => contest.Selected > 0) || vModel.Ballot.slatesDefinition.Data.SlateId > 0;
	//    }

	//    private void TimerTick(object sender, EventArgs e)
	//    {
	//        TimerCount++;
	//        if (TimerCount > StartTick && vController.ScrObject != null)
	//            vController.ScrollContainer(vModel, vModel.GetView().Container, ScrollStepTypes.ContestViewScrollStep,
	//                                        vController.ScrObject.Data, (string)vModel.Vars["more"]);
	//    }

	//    public override void Update(ISubject subj)
	//    {
	//        //Logger.Instance.Write("contest view updated");
	//    }

	//    public void SetPreference(VrLabel label)
	//    {
	//        if ((label.Text == "+") || (label.Text == "-"))
	//        {
	//            Candidate candidate = null;

	//            foreach (Candidate c in vModel.Contest.CandidatesList)
	//            {
	//                if (c.Id.ToString() == label.Tag)
	//                {
	//                    candidate = c;
	//                    break;
	//                }
	//            }

	//            DataDefinition data = null;

	//            int dataIndex = 0;

	//            foreach (DataDefinition d in vModel.Contest.Data)
	//            {
	//                if (candidate != null)
	//                {
	//                    if (d.Id == candidate.Id)
	//                    {
	//                        data = d;
	//                        break;
	//                    }

	//                    dataIndex++;
	//                }
	//            }

	//            if (data == null)
	//            {
	//                return;
	//            }

	//            VrLabel lblPreference = (VrLabel)container.FindControlByName("lblPreference_" + label.Tag);

	//            if (lblPreference != null)
	//            {
	//                if (lblPreference.Text == null)
	//                {
	//                    lblPreference.Text = "";
	//                }

	//                int val = 0;

	//                if (label.Text == "+")
	//                {
	//                    if (lblPreference.Text.Trim() == "")
	//                    {
	//                        val = 1;
	//                    } else
	//                    {
	//                        val = int.Parse(lblPreference.Text) + 1;
	//                    }

	//                    if (val == 1)
	//                    {
	//                        vModel.Contest.Selected++;
	//                    }
	//                }

	//                if (label.Text == "-")
	//                {
	//                    if (lblPreference.Text.Trim() != "")
	//                    {
	//                        val = int.Parse(lblPreference.Text) - 1;
	//                    }
	//                }

	//                if (val > 0)
	//                {
	//                    if (val <= candCount)
	//                    {
	//                        lblPreference.Text = val.ToString();
	//                        candidate.Preference = val;
	//                        vModel.Contest.Data[dataIndex].Preference = val;
	//                    }
	//                } else
	//                {
	//                    lblPreference.Text = "";

	//                    if (candidate.Preference > 0)
	//                    {
	//                        candidate.Preference = 0;
	//                        vModel.Contest.Selected--;
	//                        vModel.Contest.Data[dataIndex].Preference = 0;
	//                    }
	//                }

	//                vModel.UpdateObject(lblPreference);
	//                SetNavigation();
	//            }
	//        }
	//    }

	//    private void SetNavigation()
	//    {
	//        if (vModel.Ballot.ActiveContest > -1)
	//        {
	//            if (vModel.Contest.Selected == candCount)
	//            {
	//                btnNext.Text = (string)vModel.Vars["next"];
	//                btnNext.Tag = (string)vModel.Vars["skip"];
	//                btnNext.BgImage = btnNext.BgImage.Replace("_d", "_a");
	//            }
	//            else
	//            {
	//                btnNext.Text = (string)vModel.Vars["skip"];
	//                btnNext.Tag = (string)vModel.Vars["next"];
	//                btnNext.BgImage = btnNext.BgImage.Replace("_a", "_d");
	//                btnNext.Action = ScreenObject.ScreenObjectAction.GET_SCREEN;
	//                btnNext.Data = "review";
	//            }

	//            vModel.InvalidateObject(btnNext);
	//        }
	//    }

	//    public void SetSelection(VrLabel label)
	//    {
	//        if (label.Data == "writein")
	//        {
	//            AppManager.Instance.SetScreen("writein");
	//        }
	//    }
	//}


}
