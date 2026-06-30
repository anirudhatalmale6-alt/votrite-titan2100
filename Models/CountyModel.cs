using System.Collections.Generic;
using VotRiteBallotDataManager.AppCode;

namespace VotRite.Models
{
    class CountyModel : ScreenModel
    {
        private County county;
        private List<County> counties = new List<County>();

        public List<County> Counties
        {
            get { return counties; }
        }

        public CountyModel()
        {
            county = new County();
            counties = county.GetCounties(AppManager.BallotId);
        }

    }
}
