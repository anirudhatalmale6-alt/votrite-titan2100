using VotRite.Models;
using VotRite.UI;
using VotRiteBallotDataManager.AppCode;

namespace VotRite.Controllers
{
    class CountyController : ScreenController
    {
        private new CountyModel model;

        public CountyController(CountyModel m) : base(m)
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
            var scrObj = model.FindScreenObject(x, y);
            HandleButtonClick(scrObj);
        }

        private static void HandleButtonClick(ScreenObject scrObj)
        {
            if (scrObj != null) AppManager.Instance.SetCounty(scrObj.ObjectData as County);
        }

        public override void HandleSpeech(string recogWord)
        {
            var srcObject = model.FindClickableObjectByTextOrTag(recogWord);
            if (srcObject != null)
            {
                HandleButtonClick(srcObject);
            }
        }
    }
}
