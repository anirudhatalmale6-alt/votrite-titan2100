/*
* Product:      VotRite
* Module:       PinPad.cs
* Purpose:	    The VotRite application pin pad handler
* Description:  Contains set of pin pad functions 
* Author:       Dmitriy Slipak

* Copyright (c) 2017 VOTRITE INTERNATIONAL LLC. All rights reserved.

* THIS PRODUCT INCLUDES SOFTWARE AS A PART OF VOTRITE INTERNATIONAL LLC SYSTEM
* DEVELOPED AT VOTRITE INTERNATIONAL LLC (http://www.votrite.com).
* THE SOURCE CODE FOR THIS PRODUCT IS SUBJECT TO THE VOTRITE INTERNATIONAL LLC 
* LLC LICENSE. NO ANY PORTION OF THIS PRODUCT OR SOURCE CODE CAN BE 
* REDISTRIBUTED UNDER ANY CIRCUMSTANCES.
*/
using System;
using System.Text;
using System.Timers;
using VotRite.Util;
using VotRite.Definition;

namespace VotRite
{
    class PinPad
    {
        public enum PinPadState { OK, ERROR }
        private const int ERROR_DELAY = 2000;

        private PinPadDefinition definition;
        private StringBuilder buffer;
        private string input;
        private PinPadState state;
        private Timer errorDelay;

        public PinPad()
        {
            definition = (PinPadDefinition)DefinitionParser.Instance.Parse(
                AppManager.GetPathToCommonFile("definition" + Global.Instance.SLASH +
                "pinpad" + Global.Instance.SLASH + "pinpad_menu.mdf"));
            buffer = new StringBuilder();
            errorDelay = new Timer(ERROR_DELAY);

            errorDelay.Elapsed += new ElapsedEventHandler(ErrorDelay_Elapsed);
        }

        public PinPadDefinition Definition { get { return definition; } }

        public string Buffer
        {
            get { return buffer.ToString(); }
            set { buffer.Append(value); }
        }

        public PinPadState State
        {
            get { return state; }
            set { state = value; }
        }

        public string Input
        {
            get { return input; }
            set { input = value; }
        }

        public void Reset()
        {
            ClearBuffer();
            definition.ActiveMenu = -1;
            definition.HistMenu = -1;
            definition.Menu.Clear();

            definition = (PinPadDefinition)DefinitionParser.Instance.Parse(
                AppManager.GetPathToCommonFile("definition" + Global.Instance.SLASH +
                "pinpad" + Global.Instance.SLASH + "pinpad_menu.mdf"));
            Logger.Instance.Write("PinPad::Reset");
            Logger.Instance.Write("PinPad::Reset::ActiveMenu : " + definition.ActiveMenu);
        }

        public string GetActiveMenuAsText()
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

        public void CheckUsr()
        {
            if (input != AppManager.Configuration["Security"]["usr"])
                Error("Invalid User ID");
        }

        public void CheckPwd()
        {
            if (input != AppManager.Configuration["Security"]["pwd"])
                Error("Invalid Password");
        }

        public void CheckBPwd()
        {
            if (input != AppManager.Configuration["Security"]["bpwd"])
                Error("Invalid Password");
        }

        public void CheckInput()
        {
            if (input == null)
                Error("Invalid Choice");
            else if (Convert.ToUInt16(input) >
                definition.Menu[definition.ActiveMenu].Item.Count)
                Error("Invalid Choice");
        }

        public void ClearBuffer() { buffer.Remove(0, buffer.Length); input = ""; }

        private void Error(string text)
        {
            ClearBuffer();
            state = PinPadState.ERROR;
            Buffer = text;
            errorDelay.Enabled = true;
        }

        private void ErrorDelay_Elapsed(object sender, ElapsedEventArgs args)
        {
            ClearBuffer();
            errorDelay.Enabled = false;
            state = PinPadState.OK;
        }
    }
}
