using System.Windows.Forms;
using System.Collections;
using VotRite.Models;
using VotRite.UI;
using VotRiteBallotDataManager.AppCode;
using VotRite.Definition;

namespace VotRite.Controllers
{
    //class RankingChoiceController : ScreenController
    //{
    //    private new RankingChoiceModel model;
    //    public ScreenObject ScrObject { get; private set; }

    //    public RankingChoiceController(RankingChoiceModel m) : base(m)
    //    {
    //        model = m;
    //    }

    //    public override void HandleKey(string key)
    //    {
    //        switch (key)
    //        {
    //            default: break;
    //        }
    //    }

    //    public override void HandleTouch(int x, int y)
    //    {
    //        ScreenObject scrObj = model.FindScreenObject(x, y);

    //        if (scrObj != null)
    //        {
    //            switch (scrObj.Type)
    //            {
    //                case ScreenObject.ScreenObjectType.CONTAINER:
    //                    ScreenObject o = ((VrContainer)scrObj).FindScreenObject(x, y);
    //                    SetObjectSelection(o);
    //                    break;
    //                default:
    //                    HandleButtonClick(scrObj);
    //                    break;
    //            }
    //        }
    //    }

    //    private void SetObjectSelection(ScreenObject item)
    //    {
    //        if (item != null)
    //        {
    //            if (item.Type == ScreenObject.ScreenObjectType.LABEL)
    //            {
    //                if (item.Data == "writein")
    //                {
    //                    model.GetView().SetSelection((VrLabel)item);
    //                    model.Ballot.ContestsList[model.Ballot.ActiveContest].LastSelectedIndex = int.Parse(item.Tag.Substring(item.Tag.LastIndexOf("_") + 1));
    //                } else
    //                {
    //                    model.GetView().SetPreference((VrLabel)item);
    //                }
    //            }

    //            /*
    //            if (item.Type == ScreenObject.ScreenObjectType.DROPDOWN)
    //            {
    //                VrDropDown dd = ((VrDropDown)item);

    //                if (dd.State == VrDropDown.DropDownState.CLOSED)
    //                {
    //                    dd.State = VrDropDown.DropDownState.OPENED;
    //                    dd.OpenDropDownList();
    //                }
    //                else
    //                {
    //                    dd.State = VrDropDown.DropDownState.CLOSED;
    //                    dd.CloseDropDownList();
    //                }

    //                model.InvalidateObject(dd);
    //            }
    //            */
    //        }
    //    }

    //    private void HandleButtonClick(ScreenObject scrObj)
    //    {
    //        if (scrObj == null)
    //            return;

    //        switch (scrObj.Type)
    //        {
    //            case ScreenObject.ScreenObjectType.SCROLL:
    //                if ((scrObj).ObjectState == ScreenObject.ScreenObjectState.ACTIVE)
    //                {
    //                    ScrObject = scrObj;
    //                    ScrollContainer(model, model.GetView().Container, ScrollStepTypes.ContestViewScrollStep,
    //                                    scrObj.Data, (string)model.Vars["more"]);
    //                    model.GetView().TimerCount = 0;
    //                    model.GetView().GetTimer().Start();
    //                }

    //                break;
    //            default:
    //                if (scrObj.Visible)
    //                {
    //                    switch (scrObj.Name)
    //                    {
    //                        case "btn_next":
    //                            ArrayList prefs = new ArrayList();

    //                            foreach(DataDefinition d in model.Contest.Data)
    //                            {
    //                                if (prefs.Count > 0)
    //                                {
    //                                    if (prefs.Contains(d.Preference))
    //                                    {
    //                                        MessageBox.Show("Two ore more candidates can not have the same preference", "Error");
    //                                        return;
    //                                    }
    //                                }

    //                                prefs.Add(d.Preference);
    //                            }

    //                            if (model.Contest.MinSelection > 0)
    //                            {
    //                                ++model.Ballot.ActiveContest;
    //                                for (int i = model.Ballot.ActiveContest; i < model.Ballot.ContestsList.Count &&
    //                                    model.Ballot.ContestsList[model.Ballot.ActiveContest].Type == VotRiteBallotDataManager.AppCode.ContestTypes.R; i++)
    //                                {
    //                                    if (model.Ballot.ActiveContest < model.Ballot.ContestsList.Count)
    //                                    { ++model.Ballot.ActiveContest; }
    //                                }
    //                            }
    //                            else
    //                            {
    //                                model.Ballot.ActiveContest = 0;
    //                            }

    //                            AppManager.Instance.SetScreen("review");
    //                            break;
    //                        case "btn_review":
    //                            AppManager.Instance.SetScreen("review");
    //                            break;
    //                    }
    //                }
    //                break;
    //        }
    //    }

    //    public override void HandleMouseUp()
    //    {
    //        var timer = model.GetView().GetTimer();
    //        if (timer != null)
    //            timer.Stop();
    //        ScrObject = null;
    //    }

    //    public override void HandleSpeech(string recogWord)
    //    {
    //        var srcObject = model.FindClickableObjectByTextOrTag(recogWord);
    //        if (srcObject != null)
    //        {
    //            //HandleButtonClick(srcObject);
    //            return;
    //        }
    //        var container = (VrContainer)model.FindScreenObject(ScreenObject.ScreenObjectType.CONTAINER);
    //        if (container != null)
    //        {
    //            srcObject = container.FindClickableObjectByTextOrTag(recogWord);
    //            SetObjectSelection(srcObject);
    //        }
    //    }
    //}

    class RankingChoiceController : ScreenController
    {
        private RankingChoiceModel model;

        public ScreenObject ScrObject
        {
            get;
            private set;
        }

        public RankingChoiceController(RankingChoiceModel m) : base(m)
        {
            this.model = m;
        }

        private void HandleButtonClick(ScreenObject scrObj)
        {
            if (scrObj != null)
            {
                if (scrObj.Type != ScreenObject.ScreenObjectType.SCROLL)
                {
                    if (scrObj.Visible)
                    {
                        string name = scrObj.Name;
                        if (name != "btn_next")
                        {
                            if (name == "btn_review")
                            {
                                if (this.ValidateInput())
                                {
                                    AppManager.Instance.SetScreen("review");
                                }
                            }
                        }
                        else if (this.ValidateInput())
                        {
                            if (this.model.Ballot.ActiveContest >= this.model.Ballot.ContestsList.Count - 1)
                            {
                                AppManager.Instance.SetScreen("review");
                            }
                            else
                            {
                                BallotDefinition ballot = this.model.Ballot;
                                ballot.ActiveContest = ballot.ActiveContest + 1;
                                if ((this.model.Ballot.ElectionType == ElectionTypes.ranking_choice ? false : this.model.Ballot.ContestsList[this.model.Ballot.ActiveContest].Type != ContestTypes.V))
                                {
                                    AppManager.Instance.SetScreen("contest");
                                }
                                else
                                {
                                    AppManager.Instance.SetScreen("RankingChoice");
                                }
                            }
                        }
                    }
                }
                else if (scrObj.ObjectState == ScreenObject.ScreenObjectState.ACTIVE)
                {
                    this.ScrObject = scrObj;
                    base.ScrollContainer(this.model, this.model.GetView().Container, ScrollStepTypes.ContestViewScrollStep, scrObj.Data, (string)this.model.Vars["more"]);
                    this.model.GetView().TimerCount = 0;
                    this.model.GetView().GetTimer().Start();
                }
            }
        }

        public override void HandleKey(string key)
        {
        }

        public override void HandleMouseUp()
        {
            Timer timer = this.model.GetView().GetTimer();
            if (timer != null)
            {
                timer.Stop();
            }
            this.ScrObject = null;
        }

        public override void HandleSpeech(string recogWord)
        {
            if (this.model.FindClickableObjectByTextOrTag(recogWord) == null)
            {
                VrContainer vrContainer = (VrContainer)this.model.FindScreenObject(ScreenObject.ScreenObjectType.CONTAINER);
                if (vrContainer != null)
                {
                    this.SetObjectSelection(vrContainer.FindClickableObjectByTextOrTag(recogWord));
                }
            }
        }

        public override void HandleTouch(int x, int y)
        {
            ScreenObject screenObject = this.model.FindScreenObject(x, y);
            if (screenObject != null)
            {
                if (screenObject.Type == ScreenObject.ScreenObjectType.CONTAINER)
                {
                    this.SetObjectSelection(((VrContainer)screenObject).FindScreenObject(x, y));
                }
                else
                {
                    this.HandleButtonClick(screenObject);
                }
            }
        }

        private void SetObjectSelection(ScreenObject item)
        {
            if (item != null)
            {
                if (item.Type == ScreenObject.ScreenObjectType.LABEL)
                {
                    if (item.Data != "writein")
                    {
                        this.model.GetView().SetPreference((VrLabel)item);
                    }
                    else
                    {
                        this.model.GetView().SetSelection((VrLabel)item);
                    }
                }
            }
        }

        private bool ValidateInput()
        {
            bool flag;
            ArrayList arrayLists = new ArrayList();
            int num = 0;
            foreach (DataDefinition datum in this.model.Contest.Data)
            {
                if (datum.Preference > 0)
                {
                    num++;
                    if (arrayLists.Count > 0)
                    {
                        if (arrayLists.Contains(datum.Preference))
                        {
                            MessageBox.Show("Two ore more candidates can not have the same preference", "Error");
                            flag = false;
                            return flag;
                        }
                    }
                    arrayLists.Add(datum.Preference);
                }
            }
            if (num <= this.model.Contest.MaxSelection)
            {
                flag = true;
            }
            else
            {
                MessageBox.Show("Total number of candidates voted for exceeds the maximum number of candidates allowed !", "Error");
                flag = false;
            }
            return flag;
        }
    }

}
