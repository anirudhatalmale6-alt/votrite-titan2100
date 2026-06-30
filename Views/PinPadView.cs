using System;
using System.Text;
using System.Timers;
using VotRite.Models;
using VotRite.Controllers;
using VotRite.Definition;
using VotRite.Util;
using VotRite.UI;
using VotRite.JimForms;

namespace VotRite.Views
{
    class PinpadView : VrScreen
    {
        private const int ERROR_DELAY = 2000;

        private BallotDefinition ballot = null;
        private static PinPadDefinition definition;
        private StringBuilder buffer;
        private string input;
        private Constants.PinPadState state;
        private Timer errorDelay;
        private PinpadModel vModel { get { return model as PinpadModel; } }
        private PinpadController vController { get { return controller as PinpadController; } }
        private PinPadMenuDefinition menu;
        private ScreenObject lblDisplay;
        private Session.BallotModes _ballotMode;
        private Constants.keyAction keyAction;
        private System.Windows.Forms.TextBox displaybox;
        public static PinPadDefinition Menu
        {
            get { return definition; }
            set { definition = value; }
        }
        public PinpadView(ScreenModel m, ScreenController c)
            : base(m, c)
        {
            Window.Instance.Invalidate();
            MenuForm frm = new MenuForm();
            frm.ShowDialog();
            return;
            model = m as PinpadModel;
            vModel.Definition.Height = System.Windows.Forms.Screen.PrimaryScreen.WorkingArea.Height;
            vModel.Definition.Width = System.Windows.Forms.Screen.PrimaryScreen.WorkingArea.Width;
            vModel.Definition.Left = 0;
            vModel.Definition.Top = 0;
            vModel.Definition.Background = "graphics\\bg_dark.png";
            controller = c as PinpadController;
            Name = "pinpad";
            lblDisplay = vModel.FindScreenObject("lbl_display");
            displaybox = new System.Windows.Forms.TextBox();
            if (definition == null)
                definition = (PinPadDefinition)DefinitionParser.Instance.Parse(
                    AppManager.GetPathToCommonFile("definition" + Constants.SLASH +
                    "pinpad" + Constants.SLASH + "pinpad_menu.mdf"));
            else
                vModel.ReDraw(Window.Instance.CreateGraphics());

            if (definition.Menu.Count > 0)
            {
                if (definition.ActiveMenu < 0)
                    definition.ActiveMenu = 0;
                menu = definition.Menu[definition.ActiveMenu];
            }
            buffer = new StringBuilder();
            errorDelay = new Timer(ERROR_DELAY);

            errorDelay.Elapsed += new ElapsedEventHandler(ErrorDelay_Elapsed);

            lblDisplay.Text = GetActiveMenuAsText();
            if(AppManager.useExternalKeyBoard)
            {
                foreach (var item in vModel.Definition.ScreenObjects)
                {
                    if (item.Type == ScreenObject.ScreenObjectType.BUTTON)
                        //item.Visible = false;

                    item.ObjectState = ScreenObject.ScreenObjectState.INACTIVE;
                }
               
            }
            
            if (controller != null)
                vController.ViewInstance = this;

            keyAction = Constants.keyAction.none;

            vModel.Definition.Background = "graphics\\bg_dark.png";
            //vModel.Definition.BgColor = "#1C0F98";
            
        }

        void myTextBox_KeyPress(object sender, System.Windows.Forms.KeyPressEventArgs e)
        {
            //Do Key press event work here
        }


        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                errorDelay.Dispose();
                lblDisplay.Dispose();
                if (ballot != null) ballot.Dispose();
                if (menu != null) menu.Dispose();
                if (definition != null) definition.Dispose();
            }
            base.Dispose(disposing);
        }

        private string GetActiveMenuAsText()
        {
            string menu = "";

            foreach (PinPadMenuItem item in
                        definition.Menu[
                        definition.ActiveMenu].Item)
            {
                if (item.Key > 0)
                    menu += item.Key + ") " + item.Text;
                else
                    menu += item.Text;

                if (definition.Menu[definition.ActiveMenu].Item.Count > 1)
                    menu += "\n\r";
            }

            return menu;
        }

        public void CheckUsr(string usr)
        {
            if (usr != AppManager.Configuration["Security"]["usr"])
                Error("Invalid User ID");
        }

        public void CheckPwd(string pwd)
        {
            if (pwd != AppManager.Configuration["Security"]["pwd"])
                Error("Invalid Password");
        }

        public void CheckBPwd(string pwd)
        {
            if (pwd != AppManager.Configuration["Security"]["bpwd"])
                Error("Invalid Password");
        }

        public void CheckInput(string val)
        {
            bool found = false;

            foreach (PinPadMenuItem item in
                definition.Menu[definition.ActiveMenu].Item)
            {
                if (item.Key == Convert.ToInt16(val))
                {
                    found = true;
                    break;
                }
            }

            if (!found) Error("Invalid Choice");
        }

        public void ClearBuffer() { buffer.Remove(0, buffer.Length); input = ""; }

        private void Error(string text)
        {
            ClearBuffer();
            state = Constants.PinPadState.ERROR;
            buffer.Append(text);
            errorDelay.Enabled = true;
        }

        private void ErrorDelay_Elapsed(object sender, ElapsedEventArgs args)
        {
            ClearBuffer();
            errorDelay.Enabled = false;
            state = Constants.PinPadState.OK;
        }

        public void DataReceived(char data)
        {
            Logger.Instance.Write("soft pin pad received a data:\n " + data);
           // keyAction = Constants.keyAction.none;

            try
            {
                menu = definition.Menu[definition.ActiveMenu];
                Logger.Instance.Write("menu: " + menu.Name);

                switch ((int)data)
                {
                    case ((int) Constants.PinPadKey.KEY_BKSP):
                        if ((menu.Name == "usr_id") || (menu.Name == "usr_pwd") ||
                            (menu.Name == "bpwd"))
                        {
                            if (!string.IsNullOrEmpty(input))
                            {
                                lblDisplay.Text = lblDisplay.Text.Substring(0, lblDisplay.Text.Length - 1);
                                input = input.Substring(0, input.Length - 1);
                            }
                        }
                        break;
                    case ((int) Constants.PinPadKey.KEY_ENTER):
                        menu.ActiveItem = 0;
                        if (menu.Item[0].ReadOnly) break;
                        switch (menu.Name)
                        {
                            case "usr_id":
                                CheckUsr(input);
                                break;
                            case "usr_pwd":
                                CheckPwd(input);
                                break;
                            case "bpwd":
                                CheckBPwd(input);
                                break;
                            default:
                                break;
                        }
                        if (state == Constants.PinPadState.ERROR)
                        {
                            lblDisplay.Text = buffer.ToString();
                            vModel.UpdateObject(lblDisplay);

                            while (state == Constants.PinPadState.ERROR)
                                System.Threading.Thread.Sleep(1);

                            ClearBuffer();
                            buffer.Append(GetActiveMenuAsText());
                            lblDisplay.Text = buffer.ToString();
                        }
                        else
                        {
                            if (menu.Name == "bpwd")
                            {
                                menu = definition.Menu[definition.HistMenu];
                                if (menu.Name == "reset") menu.ActiveItem = 0;
                            }

                            definition.GetMenu(menu.Item[menu.ActiveItem].Data);
                            ClearBuffer();
                            buffer.Append(GetActiveMenuAsText());
                            lblDisplay.Text = buffer.ToString();

                            switch (menu.Item[menu.ActiveItem].PostAction)
                            {
                                case "reset":
                                    Logger.Instance.Write("SoftPinPad::Reset");
                                    definition.GetMenu("main");
                                    ClearBuffer();
                                    buffer.Append(GetActiveMenuAsText());
                                    lblDisplay.Text = buffer.ToString();
                                    AppManager.Instance.ResetBallot();
                                    break;
                                case "close":
                                    JimForms.CloseOrReset frmCloseReset = new JimForms.CloseOrReset();
                                    frmCloseReset.ShowDialog();
                                    if (frmCloseReset.option == JimForms.CloseOrReset.CloseOrResetOption.CloseApplication)
                                    {
                                        Logger.Instance.Write("will close machine");
                                        AppManager.Instance.Terminate();
                                        return;
                                    }
                                    else
                                    {
                                        Logger.Instance.Write("will Reset Voting");
                                        AppManager.Instance.ResetVoting();
                                        break;
                                    }
                                case "report":
                                    definition.GetMenu("main");
                                    ClearBuffer();
                                    buffer.Append(GetActiveMenuAsText());
                                    lblDisplay.Text = buffer.ToString();
                                    AppManager.Instance.PrintFinalReportStarted = true;
                                    AppManager.PrinterEnabled = true;
                                    AppManager.Instance.PrintReport();
                                    //AppManager.Instance.PrintReportSmit();
                                    AppManager.Instance.PrintFinalReportStarted = false;
                                    break;
                                case "unlock":

                                    Logger.Instance.Write("unlock");
                                    keyAction = Constants.keyAction.Unlock;
                                    definition.GetMenu("ballots");
                                    ClearBuffer();
                                    buffer.Append(GetActiveMenuAsText());
                                    lblDisplay.Text = buffer.ToString();
                                    break;
                                case "results":
                                    Logger.Instance.Write("results");
                                    definition.GetMenu("main");
                                    ClearBuffer();
                                    buffer.Append(GetActiveMenuAsText());
                                    lblDisplay.Text = buffer.ToString();
                                    AppManager.Instance.ShowResults();
                                    break;
                                case "configuration":
                                    JimForms.ConfigForm frmConfig = new JimForms.ConfigForm();
                                    frmConfig.ShowDialog();
                                    //if (frmCloseReset.option == JimForms.CloseOrReset.CloseOrResetOption.CloseApplication)
                                    //{
                                    //    Logger.Instance.Write("will close machine");
                                    //    AppManager.Instance.Terminate();
                                    //    return;
                                    //}
                                    //else
                                    //{
                                    //    Logger.Instance.Write("will Reset Voting");
                                    //    AppManager.Instance.ResetVoting();
                                    //    break;
                                    //}
                                    AppManager.Instance.ShowSoftPinpad("main", false);
                                    break;
                                    return;
                                default:
                                    break;
                            }
                        }
                        break;
                    case (int) Constants.PinPadKey.KEY_UP:
                        break;
                    case (int) Constants.PinPadKey.KEY_DOWN:
                        break;
                    case (int) Constants.PinPadKey.KEY_LEFT:
                        break;
                    case (int) Constants.PinPadKey.KEY_RIGHT:
                        break;
                    case (int) Constants.PinPadKey.KEY_DOT:
                        break;
                    case (int) Constants.PinPadKey.KEY_SPACE:
                        break;
                    default:
                        try
                        {
                            if ((menu.Name == "ballots") && (menu.Item[Convert.ToInt16(data.ToString()) - 1].Data == "open_ballot"))
                            {
                                if (Convert.ToInt16(data.ToString()) - 1 < menu.Item.Count)
                                {
                                    if (ballot != null)
                                    {
                                        ballot.Dispose();
                                        ballot = null;
                                    }
                                    if (keyAction == Constants.keyAction.Unlock)
                                    {
                                        if(AppManager.Configuration["System"]["Machine"] == "")
                                        {
                                            JimForms.ConfigForm_Machine frmMachine = new JimForms.ConfigForm_Machine();
                                            frmMachine.ShowDialog();
                                            if (!frmMachine._set)
                                                return;
                                        }

                                        keyAction = Constants.keyAction.none;
                                        JimForms.BallotsListDisplay frmBallots = new JimForms.BallotsListDisplay();
                                        frmBallots.ShowDialog();
                                        if (!frmBallots._default)
                                        {
                                            AppManager.Instance.ShowSoftPinpad("main", false);
                                            return;
                                        }
                                    }
                                    ballot = (BallotDefinition)DefinitionParser.Instance.FillBallotContent(null, null);// .FillBallotContent(null);
                                    ballot.BallotMode = _ballotMode;
                                    Logger.Instance.Write("ballot: " + ballot.Id);

                                    DateTime now = DateTime.Now;

                                    if (now < ballot.StartTime)
                                    {
                                        Error("Ballot not started yet");
                                        lblDisplay.Text = buffer.ToString();
                                        vModel.UpdateObject(lblDisplay);

                                        while (state == Constants.PinPadState.ERROR)
                                            System.Threading.Thread.Sleep(1);

                                        ClearBuffer();
                                        buffer.Append(GetActiveMenuAsText());
                                        lblDisplay.Text = buffer.ToString();
                                        AppManager.Instance.ResetBallot();
                                        return;
                                    }

                                    if (now > ballot.EndTime)
                                    {
                                        Error("Ballot closed");
                                        lblDisplay.Text = buffer.ToString();
                                        vModel.UpdateObject(lblDisplay);

                                        while (state == Constants.PinPadState.ERROR)
                                            System.Threading.Thread.Sleep(1);

                                        ClearBuffer();
                                        buffer.Append(GetActiveMenuAsText());
                                        lblDisplay.Text = buffer.ToString();
                                        AppManager.Instance.ResetBallot();
                                        return;
                                    }

                                    Logger.Instance.Write("will open ballot");

                                    AppManager.Instance.OpenBallot(ballot, true);

                                    return;
                                }
                            }
                        } catch (ArgumentOutOfRangeException e)
                        {
                            return;
                        }
                        
                        if (menu.Name == "ballot_mode")
                        {
                            switch (menu.Item[Convert.ToInt16(data.ToString()) - 1].PostAction)
                            {
                                case "setNormalMode":
                                    _ballotMode = Session.BallotModes.Normal;
                                    break;
                                case "setAudioMode":
                                    _ballotMode = Session.BallotModes.Audio;
                                    break;
                            }
                        }

                        if (menu.Item.Count == 1)
                        {
                            input += data;

                            if (!menu.Item[0].ReadOnly)
                            {
                                if (menu.Item[0].MaskText)
                                    lblDisplay.Text += '*';
                                else
                                    lblDisplay.Text += data;
                            }
                            else
                            {
                                if (menu.Name == "reset") data = '0';

                                input = Convert.ToString(data);
                                CheckInput(input);

                                if (state == Constants.PinPadState.ERROR)
                                {
                                    lblDisplay.Text = buffer.ToString();

                                    while (state == Constants.PinPadState.ERROR)
                                        System.Threading.Thread.Sleep(1);
                                }
                                else
                                    definition.GetMenu(menu.Item[0].Data);

                                ClearBuffer();
                                buffer.Append(GetActiveMenuAsText());
                                lblDisplay.Text = buffer.ToString();
                            }
                        }
                        else
                        {
                            input = Convert.ToString(data);
                            CheckInput(input);

                            if (state == Constants.PinPadState.ERROR)
                            {
                                lblDisplay.Text = buffer.ToString();

                                while (state == Constants.PinPadState.ERROR)
                                    System.Threading.Thread.Sleep(1);
                            }
                            else
                            {
                                menu.ActiveItem = Convert.ToInt32(data.ToString()) - 1;
                                definition.GetMenu(menu.Item[menu.ActiveItem].Data);
                            }

                            ClearBuffer();
                            buffer.Append(GetActiveMenuAsText());
                            lblDisplay.Text = buffer.ToString();
                        }

                        Logger.Instance.Write("menu name-----> " + menu.Name);
                        break;
                }

            }
            catch (FormatException e)
            {
                Logger.Instance.Write(e.ToString());
            }
            vModel.UpdateObject(lblDisplay);
        }
    }
}
