/*
* Product:      VotRite
* Module:       ComPort.cs
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
using System.IO.Ports;
using VotRite.Util;

namespace VotRite
{
    class ComPort
    {
        private SerialPort port;
        private string name;
        private int rate;
        private Parity parity;
        private int dataBits;
        private StopBits stopBits;
        private object hostDevice;

        public SerialPort Instance { get { return port; } set { port = value; } }

        public ComPort(string portName, int bRate, Parity prty,
            int dtBits, StopBits stBits, object device)
        {
            name = portName;
            rate = bRate;
            parity = prty;
            dataBits = dtBits;
            stopBits = stBits;
            hostDevice = device;

            try
            {
                port = new SerialPort(name, rate, parity, dataBits, stopBits);
                port.Handshake = Handshake.None;
                port.RtsEnable = true;
                port.DtrEnable = true;
                port.DataReceived +=
                    new SerialDataReceivedEventHandler(DataReceived);
                port.ErrorReceived += 
                    new SerialErrorReceivedEventHandler(ErrorReceived);
            }
            catch (Exception e) {
                Logger.Instance.Write(e);
            }
        }

        public void Open()
        {
            try
            {
                if (!port.IsOpen)
                {
                    port.Open();
                    Logger.Instance.Write("com port " + name + " is opened");
                }
            }
            catch (Exception e) {
                Logger.Instance.Write(e);
            }
        }

        public void Close()
        {
            try
            {
                if (port.IsOpen)
                {
                    port.Close();
                    Logger.Instance.Write("com port " + name + " is closed");
                }
            }
            catch (Exception e) {
                Logger.Instance.Write(e);
            }
        }

        public void DataReceived(object sender, SerialDataReceivedEventArgs args)
        {
            switch (hostDevice.GetType().Name)
            {
                /*case "HardCounter":
                    ((HardCounter)hostDevice).DataReceived(port.ReadExisting());
                    break;*/
                case "HardPinPad":
                    ((HardPinPad)hostDevice).DataReceived(port.ReadExisting());
                    break;
                default: break;
            }
        }

        public void ErrorReceived(object sender, SerialErrorReceivedEventArgs args)
        {
            Logger.Instance.Write(args.ToString());
        }

        public void Write(string text)
        {
            if (port.IsOpen)
            {
                try
                {
                    port.Write(text);
                }
                catch (Exception e) {
                    Logger.Instance.Write(e);
                }
            }
            else
                Logger.Instance.Write("pin pad port is closed");
        }
    }
}
