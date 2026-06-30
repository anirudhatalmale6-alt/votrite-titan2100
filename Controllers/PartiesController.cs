using VotRite.Models;
using VotRite.UI;

namespace VotRite.Controllers
{
    class PartiesController : ScreenController
    {
        private new PartiesModel model;
        public ScreenObject ScrObject { get; private set; }

        public PartiesController(PartiesModel m)
            : base(m)
        {
            model = m;
        }

        public override void HandleKey(string key)
        {
            switch (key)
            {
                default: break;
            }
        }

        public override void HandleTouch(int x, int y)
        {
            ScreenObject scrObj = model.FindScreenObject(x, y);

            if (scrObj != null)
            {
                switch (scrObj.Type)
                {
                    case ScreenObject.ScreenObjectType.CONTAINER:
                        ScreenObject o = ((VrContainer)scrObj).FindScreenObject(x, y);
                        SetObjectSelection(o);
                        break;
                    default:
                        HandleButtonClick(scrObj);
                        break;
                }
            }
        }

        private void SetObjectSelection(ScreenObject item)
        {
            if (item != null)
            {
                if (item.Type == ScreenObject.ScreenObjectType.SELECTION)
                    model.GetView().SetSelection((VrSelection)item);
            }
        }

        private void HandleButtonClick(ScreenObject scrObj)
        {
            if (scrObj == null)
                return;
            switch (scrObj.Type)
            {
                case ScreenObject.ScreenObjectType.SCROLL:
                    if ((scrObj).ObjectState == ScreenObject.ScreenObjectState.ACTIVE)
                    {
                        ScrObject = scrObj;
                        ScrollContainer(model, model.GetView().Container, ScrollStepTypes.ContestViewScrollStep,
                                        scrObj.Data, (string)model.Vars["more"]);
                        model.GetView().TimerCount = 0;
                        model.GetView().GetTimer().Start();
                    }
                    break;
                default:
                    if (scrObj.Visible)
                    {
                        switch (scrObj.Name)
                        {
                            case "btn_next":
                                model.Ballot.ContestsList = DefinitionParser.Instance.FillContestsContent(model.Ballot);
                                model.Ballot.ActiveContest = 0;
                                if (model.Ballot.ActiveContest < model.Ballot.ContestsList.Count)
                                {
                                    AppManager.Instance.SetScreen("contest");
                                }
                                else
                                    AppManager.Instance.SetScreen("review");
                                break;
                            case "btn_review":
                                AppManager.Instance.SetScreen("review");
                                break;
                            case "btn_setting_tools":
                                AppManager.Instance.SetScreen("settingTools", "Parties");
                                break;
                            case "btn_callhelp":
                                string sHelpPath = AppManager.Configuration["HelpFile"]["Path"];

                                if (sHelpPath != "")
                                {
                                    try
                                    {
                                        System.Diagnostics.Process.Start(sHelpPath);
                                    }
                                    catch
                                    {
                                    }
                                }

                                break;
                        }
                    }
                    break;
            }
        }

        public override void HandleMouseUp()
        {
            var timer = model.GetView().GetTimer();
            if (timer != null)
                timer.Stop();
            ScrObject = null;
        }

        public override void HandleSpeech(string recogWord)
        {
            var srcObject = model.FindClickableObjectByTextOrTag(recogWord);
            if (srcObject != null)
            {
                HandleButtonClick(srcObject);
                return;
            }
            var container = (VrContainer)model.FindScreenObject(ScreenObject.ScreenObjectType.CONTAINER);
            if (container != null)
            {
                srcObject = container.FindClickableObjectByTextOrTag(recogWord);
                SetObjectSelection(srcObject);
            }
        }
    }
}
