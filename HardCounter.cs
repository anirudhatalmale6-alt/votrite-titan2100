/*
* Product:      VotRite
* Module:       HardCounter.cs
* Purpose:	    The VotRite application hardware counter handler
* Description:  Contains set of hardware counter functions 
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
using System.IO.Ports;
using System.Threading;
using VotRite.Util;

namespace VotRite
{
    /*class HardCounter
    {
        private const int BAUD_RATE = 19200;
        private const Parity PARITY = Parity.None;
        private const int DATA_BITS = 8;
        private const StopBits STOP_BITS = StopBits.One;
        private const char FF = (char)12;
        private const char EOT = (char)4;
        private const int START_DELAY = 3000;

        private ComPort port;

        public HardCounter() { Activate(); }

        private void Activate()
        {
            try
            {
                port = new ComPort(IOHandler.ReadConfig("System",
                    "CounterPort"), BAUD_RATE, PARITY, DATA_BITS,
                    STOP_BITS, this);
#if !DEBUG
                port.Open();

                Thread.Sleep(START_DELAY);
                port.Write(Convert.ToString(EOT));
                ClearLCD();
                port.Write("Initialized");
                Logger.Instance.Write("counter is initialized");
#endif
            }
            catch (Exception e) { Logger.Instance.Write(e); }
        }

        public void Deactivate()
        {
			try {
#if !DEBUG
            ClearLCD();
            port.Close();
#endif
			}
			catch (Exception e) { Logger.Instance.Write(e); }
        }

        public void DataReceived(string data)
        {
            Logger.Instance.Write("counter received a data:\n" + data);
        }

        private void ClearLCD()
        {
			try {
            port.Write(Convert.ToString(FF));
			}
			catch (Exception e) { Logger.Instance.Write(e); }
        }

        public void Write(string text)
        {
			try {
            ClearLCD();
            port.Write(text);
			}
			catch (Exception e) { Logger.Instance.Write(e); }
        }
    }*/
}
