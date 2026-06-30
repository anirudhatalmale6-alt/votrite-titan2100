using VotRite.Models;
using VotRite.UI;
using VotRiteBallotDataManager.AppCode;

namespace VotRite.Controllers
{
    class ResultsController : ScreenController
    {
        private new ResultsModel model;
        public ScreenObject ScrObject { get; private set; }

        public ResultsController(ResultsModel m) : base(m)
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

                        string text = "";

                        if (scrObj.Tag == "Scroll up")
                        {
                            text = "Touch here to see more";
                        }
                        if (scrObj.Tag == "Scroll down")
                        {
                            text = "Touch here to see less";
                        }

                        ScrollContainer(model, model.GetView().Container, ScrollStepTypes.ContestViewScrollStep,
                                        scrObj.Data, text);
                        model.GetView().TimerCount = 0;
                        model.GetView().GetTimer().Start();
                    }

                    break;
                default:
                    if (scrObj.Visible)
                    {
                        switch (scrObj.Name)
                        {
                            case "btn_back":
                                AppManager.Instance.ShowSoftPinpad(null, true);
                                AppManager.Instance.CloseSession();
                                AppManager.Instance.CloseBallot();
                                break;
                            default:
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
