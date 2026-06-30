/*
* Product:      VotRite
* Module:       HardPinPad.cs
* Purpose:	    The VotRite application hardware pin pad handler
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
using System.IO.Ports;
using System.Threading;
using VotRite.Util;
using VotRite.Definition;

namespace VotRite
{
    class HardPinPad : PinPad
    {
        private const int BAUD_RATE = 9600;
        private const Parity PARITY = Parity.None;
        private const int DATA_BITS = 8;
        private const StopBits STOP_BITS = StopBits.One;
        private const int PP_ROWS = 4;
        private const int PP_CHAR_PER_ROW = 20;
        private const char NUL = (char)0;
        private const char BS = (char)8;

        private BallotDefinition ballot;
        private ComPort port;
        private PinPadMenuDefinition menu;
        private int openPortAttempts = 0;

        public HardPinPad(bool activate) {
            if (activate)
            {
                Activate();
            }
        }

        public ComPort Port
        {
            get { return port;  }
            set { port = value;  }
        }

        new public void Reset()
        {
            Definition.GetMenu("ballots");
            ClearBuffer();
            ClearLCD();
            Buffer = GetActiveMenuAsText();
            port.Write(Buffer);
        }

        private void Activate()
        {
            try
            {
                port = new ComPort(AppManager.Configuration["System"]["PinPadPort"], 
                    BAUD_RATE, 
                    PARITY, DATA_BITS,
                    STOP_BITS, 
                    this);

// #if !DEBUG
                port.Open();
                
                if (!port.Instance.IsOpen)
                {
                    Console.WriteLine("Port is closed");

                    if (openPortAttempts < 3)
                    {
                        Thread.Sleep(10000);
                        openPortAttempts++;
                        Activate();
                    }
                    
                    return;
                }

                ClearLCD();
                port.Write("Initialized");
                Logger.Instance.Write("hard pin pad is initialized");

                if (Definition.Menu.Count > 0)
                {
                    if (Definition.ActiveMenu < 0)
                        Definition.ActiveMenu = 0;

                    menu = Definition.Menu[Definition.ActiveMenu];
                    ClearLCD();
                    port.Write(GetActiveMenuAsText());
                }
// #endif
            }
            catch (Exception e) {
                Logger.Instance.Write(e);
            }
        }

        public void Deactivate()
        {
			try {
// #if !DEBUG
            Logger.Instance.Write("HardPinPad::Deactivate");
            ClearLCD();
            port.Close();
// #endif
			}
			catch (Exception e) { Logger.Instance.Write(e); }
        }

        public void DataReceived(string data)
        {
            Logger.Instance.Write("hard pin pad received a data:\n " + data);

            try
            {
                menu = Definition.Menu[Definition.ActiveMenu];
                Logger.Instance.Write("menu: " + menu.Name);
                int input = Convert.ToInt16(data);

                if ((menu.Name == "ballots") && 
                    (menu.Item[Convert.ToInt16(data.ToString()) - 1].Text != "More"))
                {
                    if (Convert.ToInt16(data.ToString()) - 1 < menu.Item.Count)
                    {
						ballot = (BallotDefinition)DefinitionParser.Instance.FillBallotContent(null);
                        Logger.Instance.Write("ballot: " + ballot.Id);
                        Logger.Instance.Write("will open ballot");
                        AppManager.Instance.OpenBallot(ballot, true);
                    }
                }

                if (menu.Item.Count == 1)
                {
                    Input += input;

                    if (!menu.Item[0].ReadOnly)
                    {
                        if (menu.Item[0].MaskText)
                            port.Write("*");
                        else
                            port.Write(Convert.ToString(input));
                    }
                    else
                    {
                        if (menu.Name == "reset") input = 0;
                        Input = Convert.ToString(input);
                        CheckInput();

                        if (State == PinPad.PinPadState.ERROR)
                        {
                            ClearLCD();
                            port.Write(Buffer);

                            while (State == PinPadState.ERROR)
                                Thread.Sleep(1);
                        }
                        else
                            Definition.GetMenu(menu.Item[0].Data);

                        ClearBuffer();
                        ClearLCD();
                        Buffer = GetActiveMenuAsText();
                        port.Write(Buffer);
                    }

                    /*if ((input - 1) < menu.Item.Count)
                    {
                        switch (menu.Item[0].PostAction)
                        {
                            case "hide":
                                Logger.Instance.Write("will open ballot");
                                AppManager.Instance.OpenBallot(ballot);
                                break;
                            default: break;
                        }
                    }*/
                }
                else
                {
                    Input = Convert.ToString(input);
                    CheckInput();

                    if (State == PinPadState.ERROR)
                    {
                        ClearLCD();
                        port.Write(Buffer);

                        while (State == PinPadState.ERROR)
                            Thread.Sleep(1);
                    }
                    else
                    {
                        menu.ActiveItem = input - 1;
                        Definition.GetMenu(menu.Item[menu.ActiveItem].Data);
                    }

                    ClearBuffer();
                    ClearLCD();
                    Buffer = GetActiveMenuAsText();
                    port.Write(Buffer);
                }

                Logger.Instance.Write("menu name-----> " + menu.Name);
                
                /*if (menu.Name == "ballots")
                {
                    if (Convert.ToInt16(data.ToString()) - 1 < menu.Item.Count)
                    {
                        ballot = (BallotDefinition)DefinitionParser.Instance.Parse(
                            "definition\\ballot\\" +
                            menu.Item[Convert.ToInt16(data.ToString()) - 1].PostData);
                        Logger.Instance.Write("ballot: " + ballot.Id);
                    }
                }*/
            }
            catch //(FormatException e)
            {
                if ((menu.Name == "usr_id") || (menu.Name == "usr_pwd") ||
                                (menu.Name == "bpwd"))
                {
                    if (Convert.ToChar(data) == '\b')   //  backspace
                    {
                        if (Input.Length > 0)
                        {
                            Input = Input.Substring(0, Input.Length - 1);
                            string tmpInput = Input;
                            if (menu.Item[0].MaskText)
                            {
                                foreach (char c in tmpInput)
                                    tmpInput = tmpInput.Replace(c, '*');
                            }
                            ClearLCD();
                            port.Write(GetActiveMenuAsText() + tmpInput);
                        }
                    }
                }

                if (Convert.ToChar(data) == '\r')   // enter
                {
                    if (menu.Item.Count == 1)
                    {
                        menu.ActiveItem = 0;

                        if (!menu.Item[0].ReadOnly)
                        {
                            switch (menu.Name)
                            {
                                case "usr_id": CheckUsr(); break;
                                case "usr_pwd": CheckPwd(); break;
                                case "bpwd": CheckBPwd(); break;
                                default: break;
                            }

                            if (State == PinPadState.ERROR)
                            {
                                ClearLCD();
                                port.Write(Buffer);

                                while (State == PinPadState.ERROR)
                                    Thread.Sleep(1);

                                ClearBuffer();
                                ClearLCD();
                                Buffer = GetActiveMenuAsText();
                                port.Write(Buffer);
                            }
                            else
                            {
                                if (menu.Name == "bpwd")
                                {
                                    menu = Definition.Menu[Definition.HistMenu];
                                    if (menu.Name == "reset") menu.ActiveItem = 0;
                                }

                                Definition.GetMenu(menu.Item[menu.ActiveItem].Data);
                                ClearBuffer();
                                ClearLCD();
                                Buffer = GetActiveMenuAsText();
                                port.Write(Buffer);

                                switch (menu.Item[menu.ActiveItem].PostAction)
                                {
                                    case "reset":
                                        Logger.Instance.Write("HardPinPad::Reset");
                                        Definition.GetMenu("main");
                                        ClearBuffer();
                                        ClearLCD();
                                        Buffer = GetActiveMenuAsText();
                                        port.Write(Buffer);
                                        AppManager.Instance.ResetBallot();
                                        break;
                                    case "close":
                                        Logger.Instance.Write("will close machine");
                                        AppManager.Instance.Terminate();
                                        break;
                                    case "report":
                                        Definition.GetMenu("main");
                                        ClearBuffer();
                                        ClearLCD();
                                        Buffer = GetActiveMenuAsText();
                                        port.Write(Buffer);
                                        AppManager.Instance.PrintReport();
                                        break;
                                    case "unlock":
                                        Logger.Instance.Write("unlock");
                                        Definition.GetMenu("ballots");
                                        ClearBuffer();
                                        ClearLCD();
                                        Buffer = GetActiveMenuAsText();
                                        port.Write(Buffer);
                                        break;
                                    default: break;
                                }
                            }
                        }
                    }
                }
            }
        }

        private void ClearLCD()
        {
            try
            {
                int len = PP_ROWS * PP_CHAR_PER_ROW;

                for (int i = 0; i < PP_ROWS; i++)
                    port.Instance.WriteLine(Convert.ToString(NUL));

                for (int i = 0; i < len; i++)
                    port.Write(Convert.ToString(BS));
            }
            catch (Exception e) {
                Logger.Instance.Write(e);
            }
        }
    }
}
