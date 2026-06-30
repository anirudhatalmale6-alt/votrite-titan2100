using System;
using VotRite.Models;
using VotRite.UI;
using VotRite.Views;

using VotRite.Util;

using VotRite.Definition;
using VotRiteBallotDataManager.AppCode;

namespace VotRite.Controllers
{
    internal class RemotePollWorkerController : ScreenController
    {
        public RemotePollWorkerView View { get; set; }
        public RemotePollWorkerModel Model { get; set; }

        private string mScreenName = "";

        public RemotePollWorkerController(RemotePollWorkerModel m, string pScreenName)
            : base(m)
        {
            Model = m;

            mScreenName = pScreenName;
        }

        public RemotePollWorkerController(RemotePollWorkerModel m)
            : base(m)
        {
            Model = m;

            if (m.Session != null && m.Session.Ballot != null)
            {
                if (m.Session.Ballot.ElectionType == ElectionTypes.ranking_choice)
                {
                    mScreenName = "RankingChoice";
                } else if (m.Session.Ballot.HasParty)
                {
                    m.Session.Ballot = DefinitionParser.Instance.FillPartiesContent(m.Session.Ballot, m.Session.CurrentLocale);
                    mScreenName = "parties";
                } else if (m.Session.Ballot.HasCounties)
                {
                    BallotDefinition bd = new BallotDefinition();
                    bd.countiesDefinition.Counties = County.GetCounties(m.Session.Ballot.Id, m.Session.CurrentLocale.Id);
                    m.Session.Ballot.countiesDefinition.Counties = bd.countiesDefinition.Counties;
                    mScreenName = "counties";
                } else if (m.Session.Ballot.HasSlates)
                {
                    mScreenName = "slates";
                } else
                {
                    mScreenName = "contest";
                }
            } else
            {
                if (!Convert.ToBoolean(AppManager.Configuration["System"]["RemotePollWorker"]))
                {
                    mScreenName = "pinpad";
                } else
                {
                    mScreenName = "initial";
                }            
            }
        }

        public override void HandleTouch(int x, int y)
        {
            var scrObj = Model.FindScreenObject(x, y);

            if (scrObj != null)
            {
                if (scrObj.Type == ScreenObject.ScreenObjectType.CONTAINER)
                {
                    scrObj = ((VrContainer)scrObj).FindScreenObject(x, y);
                    SetObjectSelection(scrObj);
                } else
                {
                    HandleButtonClick(scrObj);
                }
            }
        }

        private void SetObjectSelection(ScreenObject item)
        {
            if (item != null && item.Enabled)
            {
                if (item.Type == ScreenObject.ScreenObjectType.SELECTION)
                    Model.GetView().SetSelection((VrSelection)item);
            }
        }

        private void HandleButtonClick(ScreenObject scrObj)
        {
            if (scrObj != null && scrObj.Type == ScreenObject.ScreenObjectType.BUTTON)
            {
                if (scrObj.Visible)
                {
                    switch (scrObj.Name)
                    {
                        case "btn_accept":
                            bool enabled = false;
                            if (scrObj.Data == "enabled")
                            {
                                enabled = true;
                                mScreenName = "initial";
                            } else {
                                mScreenName = "pinpad";
                            }
                            IOHandler.SaveConfig("System", "RemotePollWorker", enabled.ToString());
                            AppManager.Configuration = IOHandler.DecryptConfig();
                            //AppManager.Instance.SetScreen(mScreenName);
                            AppManager.Instance.SetScreen("settingTools", "Locale");
                            break;
                        case "btn_cancel":
                            //added on 01/12/2019 for resolving remotepollworker screen initialization issues
                            AppManager.Configuration["System"]["RemotePollWorker"] = "False";
                            mScreenName = "pinpad";
                            //-------------------------------------------------------------------------------
                            //AppManager.Instance.SetScreen(mScreenName);
                            AppManager.Instance.SetScreen("settingTools", "Locale");
                            break;
                        default:
                            break;
                    }
                }
            }
        }

        public override void HandleSpeech(string recogWord)
        {
            var srcObject = Model.FindClickableObjectByTextOrTag(recogWord);
            if (srcObject == null)
            {
                var container = (VrContainer)Model.FindScreenObject(ScreenObject.ScreenObjectType.CONTAINER);
                if (container != null)
                {
                    srcObject = container.FindClickableObjectByTextOrTag(recogWord);
                }
            }
            if (srcObject != null)
            {
                HandleButtonClick(srcObject);
            }
        }

    }
}
