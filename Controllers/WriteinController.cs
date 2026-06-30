using VotRite.Models;
using VotRite.UI;
using VotRite.Views;
using System.Data;
using VotRite.Definition;
using System.Collections.Generic;
using VotRiteBallotDataManager.AppCode;

namespace VotRite.Controllers
{
    internal class WriteinController : ScreenController
    {
        public WriteinView View { get; set; }
        public WriteinModel Model { get; set; }

        public WriteinController(WriteinModel m)
            : base(m)
        {
            Model = m;
        }

        public override void HandleTouch(int x, int y)
        {
            
            var scrObj = Model.FindScreenObject(x, y);
            if(scrObj == null)
                return;
            if (AppManager.useExternalKeyBoard && scrObj.Text != "Accept" && scrObj.Text != "Cancel")
                return;
            if (scrObj != null)
            {
                if (scrObj.Type == ScreenObject.ScreenObjectType.CONTAINER)
                    scrObj = ((VrContainer) scrObj).FindScreenObject(x, y);

                HandleButtonClick(scrObj);
            }
        }

        public override void HandleKey(string key)
        {
            string c = key;
            if (key.StartsWith("D") && key != "Delete" && key.Length != 1)
                c = key.Replace("D", "");
            if (key == "Return")
                c = "\r";
            if (key == "Back")
                c = "BKSP";

            View.DataReceived(c);
        }

        /*   Old Verson codes
        private void HandleButtonClick(ScreenObject scrObj)
        {
            if (scrObj != null && scrObj.Type == ScreenObject.ScreenObjectType.BUTTON)
            {
                if (scrObj.Visible)
                {
                    switch (scrObj.Name)
                    {
                        case "btn_accept":
                            Model.Session.Ballot.ContestsList[
                                Model.Session.Ballot.ActiveContest].Data[
                                    Model.Session.Ballot.ContestsList[
                                        Model.Session.Ballot.ActiveContest].LastSelectedIndex].
                                Text = View.LblDisplay.Text;

                            Model.Session.Ballot.ContestsList[
                                Model.Session.Ballot.ActiveContest].Data[
                                    Model.Session.Ballot.ContestsList[
                                        Model.Session.Ballot.ActiveContest].LastSelectedIndex].Group = "Write In";
                            if (Model.Session.Ballot.ContestsList[
                                Model.Session.Ballot.ActiveContest].Data[
                                    Model.Session.Ballot.ContestsList[
                                        Model.Session.Ballot.ActiveContest].LastSelectedIndex].Text.Trim() == "")
                            {
                                Model.Session.Ballot.ContestsList[
                                    Model.Session.Ballot.ActiveContest].Data[
                                        Model.Session.Ballot.ContestsList[
                                            Model.Session.Ballot.ActiveContest].LastSelectedIndex].Text =
                                    "Touch here to submit another name(s)";
                                Model.Session.Ballot.ContestsList[
                                    Model.Session.Ballot.ActiveContest].Data[
                                        Model.Session.Ballot.ContestsList[
                                            Model.Session.Ballot.ActiveContest].LastSelectedIndex].Group = "";
                                Model.Session.Ballot.ContestsList[
                                    Model.Session.Ballot.ActiveContest].Data[
                                        Model.Session.Ballot.ContestsList[
                                            Model.Session.Ballot.ActiveContest].LastSelectedIndex].State =
                                    VrSelection.SelectionState.DESELECTED;
                                Model.Session.Ballot.ContestsList[
                                    Model.Session.Ballot.ActiveContest].Selected--;
                            }
                            var container = (VrContainer)Model.FindScreenObject("ctr_contest");
                            if (container != null)
                            {
                                var btnAddCandidates =
                                    (VrSelection)container.FindControlByText("btn_add_candidates");
                                if (btnAddCandidates != null)
                                    btnAddCandidates.Visible = true;
                            }
                            AppManager.Instance.SetScreen("contest");
                            break;
                        case "btn_cancel":
                            Model.Session.Ballot.ContestsList[Model.Session.Ballot.ActiveContest].Data[
                                Model.Session.Ballot.ContestsList[Model.Session.Ballot.ActiveContest].
                                    LastSelectedIndex].State =
                                VrSelection.SelectionState.DESELECTED;
                            Model.Session.Ballot.ContestsList[Model.Session.Ballot.ActiveContest].Selected--;
                            AppManager.Instance.SetScreen("contest");
                            break;
                        default:
                            if (!string.IsNullOrEmpty(scrObj.Text) && View != null)
                                View.DataReceived(scrObj.Text);
                            break;
                    }
                }
            }
        }
        */

        private List<ContestDefinition> GetFitOrderContestList()
        {
            List<ContestDefinition> lstRet = new List<ContestDefinition>();

            DataTable tbPosition = DataManager.VotingContentDataInstance.GetData(string.Format("SELECT contest_id,rvo_order_position FROM {0} ",
                                                                "race_view_options"
                                                                ));

            tbPosition.DefaultView.Sort = "rvo_order_position";
            DataTable tbStart = tbPosition.DefaultView.ToTable();

            for (int k = 0; k < tbStart.Rows.Count; k++)
            {
               
                for (int i = 0; i < Model.Session.Ballot.ContestsList.Count; i++)
                {
                    if (int.Parse(tbStart.Rows[k]["contest_id"].ToString()) == Model.Session.Ballot.ContestsList[i].Id)
                    {
                        lstRet.Add(Model.Session.Ballot.ContestsList[i]);
                        break;
                    }
                }
            }

            if (lstRet.Count < Model.Session.Ballot.ContestsList.Count)
            {
                lstRet = Model.Session.Ballot.ContestsList;
            }

            return lstRet;
        }

        //New Verson Code (Modified By Jim On 9/16/2015)
        private void HandleButtonClick(ScreenObject scrObj)
        {
            List<ContestDefinition> lstContest = GetFitOrderContestList();
            int iActiveContest = Model.Session.Ballot.ActiveContest;

            if (scrObj != null && scrObj.Type == ScreenObject.ScreenObjectType.BUTTON)
            {
                if (scrObj.Visible)
                {
                    switch (scrObj.Name)
                    {
                        case "btn_accept":
                            //if (Model.Ballot.ElectionType != VotRiteBallotDataManager.AppCode.ElectionTypes.ranking_choice)
                            if (Model.Ballot.ContestsList[Model.Ballot.ActiveContest].Type != ContestTypes.V)
                            {
                                lstContest[iActiveContest].Data[lstContest[iActiveContest].LastSelectedIndex].Text = 
                                    View.LblDisplay.Text.Replace("     ", " ").Replace("    ", " ").Replace("   ", " ").Replace("  ", " ").Trim();
                                lstContest[iActiveContest].Data[lstContest[iActiveContest].LastSelectedIndex].Group = "Write In";
                                lstContest[iActiveContest].Data[lstContest[iActiveContest].LastSelectedIndex].IsWritten = true;

                                if (lstContest[
                                    iActiveContest].Data[
                                        lstContest[
                                            iActiveContest].LastSelectedIndex].Text.Trim() == "")
                                {
                                    lstContest[
                                        iActiveContest].Data[
                                            lstContest[
                                                iActiveContest].LastSelectedIndex].Text =
                                        "Touch here to submit another name(s)";
                                    lstContest[
                                        iActiveContest].Data[
                                            lstContest[
                                                iActiveContest].LastSelectedIndex].Group = "";
                                    lstContest[
                                        iActiveContest].Data[
                                            lstContest[
                                                iActiveContest].LastSelectedIndex].State =
                                        VrSelection.SelectionState.DESELECTED;
                                    lstContest[
                                        iActiveContest].Selected--;
                                }

                                var container = (VrContainer)Model.FindScreenObject("ctr_contest");

                                if (container != null)
                                {
                                    var btnAddCandidates =
                                        (VrSelection)container.FindControlByText("btn_add_candidates");
                                    if (btnAddCandidates != null)
                                        btnAddCandidates.Visible = true;
                                }

                                AppManager.Instance.SetScreen("contest");
                            } else
                            {
                                DataDefinition def = null;
                                Candidate c = null;

                                foreach(DataDefinition d in lstContest[iActiveContest].Data)
                                {
                                    if (d.WriteIn)
                                    {
                                        int inx = int.Parse(d.Tag.Substring(d.Tag.LastIndexOf("_") + 1));

                                        if (inx != lstContest[iActiveContest].LastSelectedIndex)
                                        {
                                            continue;
                                        }

                                        d.Text = View.LblDisplay.Text.Replace("     ", " ").Replace("    ", " ").Replace("   ", " ").Replace("  ", " ").Trim();
                                        d.IsWritten = true;
                                        d.State = VrSelection.SelectionState.SELECTED;

                                        c = new Candidate() {
                                            Id = d.Id,
                                            Name = d.Text,
                                            PartyId = d.PartyId,
                                            PartyLogo = d.PartyLogo,
                                            Photo = d.Photo,
                                            Preference = d.Preference,
                                            IncumbentFlag = d.IncumbentFlag,
                                            ContestId = lstContest[iActiveContest].Id,
                                            IsWritten = false
                                        };
                                        def = d;

                                        break;
                                    }
                                }

                                if (c != null) {
                                    //string query = string.Format("select count(*) from candidates where cand_id={0}", c.Id);
                                    //int cnt = Helper.Cast(DataManager.VotingContentDataInstance.GetScalarData(query), 0);

                                    //query = string.Format("select * from candidates where cand_id={0}", c.Id);
                                    //var tesTab = DataManager.VotingContentDataInstance.GetData(query);

                                    //if (cnt == 0)
                                    //{
                                        string query = string.Format("select count(*) from candidate_name where cand_name='{0}'", Util.VotriteCrypto.Encrypt( c.Name));
                                        int cand = Helper.Cast(DataManager.VotingContentDataInstance.GetScalarData(query), 0);

                                    if (cand == 0)
                                    {
                                        query = string.Format("select max(cand_id) from candidate_name");
                                        int mxid = Helper.Cast(DataManager.VotingContentDataInstance.GetScalarData(query), 0);
                                        if (mxid > 0)
                                            mxid = mxid + 1;
                                        query = string.Format("insert into candidate_name (cand_id, locale_id, cand_name) values ({0}, {1}, '{2}')", mxid, 1, Util.VotriteCrypto.Encrypt(c.Name));
                                        bool res = DataManager.VotingContentDataInstance.SetData(query);

                                        if (res)
                                        {
                                            def.Id = mxid;
                                            c.Id = mxid;
                                            //query = string.Format("insert into candidates (cand_id, incumbent_flag, contest_id, cand_photo, party_logo, party_id, county_id, voteable, is_written) values ({0}, '{1}', {2}, '{3}', '{4}', {5}, {6}, {7}, {8})",
                                            //    c.Id,
                                            //    c.IncumbentFlag,
                                            //    c.ContestId,
                                            //    c.Photo,
                                            //    c.PartyLogo,
                                            //    c.PartyId,
                                            //    Model.Session.Ballot.CountyId,
                                            //    0,
                                            //    1
                                            //);
                                            query = string.Format("insert into candidates (cand_id, incumbent_flag, contest_id, cand_photo, party_logo, party_id, voteable, is_written) values ({0}, '{1}', {2}, '{3}', '{4}', {5}, {6}, {7})",
                                                c.Id,
                                                c.IncumbentFlag,
                                                c.ContestId,
                                                c.Photo,
                                                c.PartyLogo,
                                                c.PartyId,
                                                0,
                                                1
                                            );
                                            res = DataManager.VotingContentDataInstance.SetData(query);
                                        }
                                    }
                                    else
                                    {
                                        query = string.Format("select cand_id from candidate_name where cand_name='{0}'", Util.VotriteCrypto.Encrypt( c.Name));
                                        int id = Helper.Cast(DataManager.VotingContentDataInstance.GetScalarData(query), 0);
                                        c.Id = id;

                                        if (def != null)
                                        {
                                            def.Id = id;
                                        }
                                    }
                                   // }
                                }

                                c.IsWritten = true;
                                lstContest[iActiveContest].CandidatesList.Add(c);

                                AppManager.Instance.SetScreen("RankingChoice");
                            }
                            
                            break;
                        case "btn_cancel":
                            
                           //if (Model.Ballot.ElectionType != VotRiteBallotDataManager.AppCode.ElectionTypes.ranking_choice)
                           if(Model.Ballot.ContestsList[Model.Ballot.ActiveContest].Type != ContestTypes.V)
                            {
                                lstContest[iActiveContest].Data[
                                lstContest[iActiveContest].
                                    LastSelectedIndex].State =
                                VrSelection.SelectionState.DESELECTED;
                                lstContest[iActiveContest].Selected--;
                                AppManager.Instance.SetScreen("contest");
                            } else
                            {
                                AppManager.Instance.SetScreen("RankingChoice");
                            }
                            
                            break;
                        default:
                            if (!string.IsNullOrEmpty(scrObj.Text) && View != null)
                                View.DataReceived(scrObj.Text);
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
                var container = (VrContainer) Model.FindScreenObject(ScreenObject.ScreenObjectType.CONTAINER);
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
