// Product:	VotRite
// Module:  StartUp.cs
// Author:  Dmitriy Slipak

// 
// Copyright (c) 2017 - VOTRITE INTERNATIONAL LLC. All rights reserved.
// 
// THIS PRODUCT INCLUDES SOFTWARE AS A PART OF VOTRITE VOTING 
// SYSTEM DEVELOPED AT VOTRITE INTERNATIONAL LLC (http://www.votrite.com).
// THE SOURCE CODE FOR THIS PRODUCT IS SUBJECT TO THE VOTRITE INTERNATIONAL LLC 
// LICENSE. NO ANY PORTION OF THIS PRODUCT OR SOURCE CODE CAN BE 
// REDISTRIBUTED UNDER ANY CIRCUMSTANCES.
// 
using System;
using System.Windows.Forms;
using System.Threading;
using System.Runtime.InteropServices;

namespace VotRite
{
	public class Driver
	{
        public static bool TestMode { get; set; }
        static Mutex mutex = new Mutex(true, "{C522C0F1-1AEF-4EBE-BBBE-D323CD487425}");
		/// <summary>
		///	Application entry point 
		/// </summary>
		[STAThread]
		private static void Main(string[] args)
		{   
            if (mutex.WaitOne(TimeSpan.Zero, true))
            {
                CheckTestMode(args);

                Application.Run(new Window());
                mutex.ReleaseMutex();
            }
            else
            {
                MessageBox.Show("Instance Already Running!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Hand);
            }
		}

        private static void CheckTestMode(string[] args)
        {
            TestMode = false;

            if (args != null)
                if (args.Length == 1)
                {
                    int ballotId = -1;
                    if (int.TryParse(args[0], out ballotId))
                    {
                        AppManager.BallotId = ballotId;
                        TestMode = true;
                    }
                }
        }

        public static void Exit()
        {
            Application.Exit();
        }
	}
}
