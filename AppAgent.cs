// Product:	VotRite
// Module:  AppAgent.cs
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
using System.Threading;
using System.Diagnostics;
using VotRite.Util;

namespace VotRite
{
	public class AppAgent: IDisposable
	{
		internal class SysChecker: IDisposable
		{
			private PerformanceCounter cpuCounter = null;
			private PerformanceCounter proCounter = null;
			private string cpuCategory;
			private string cpuCounterName;
			private string proCategory;
			private Process process = null; 
			
			public SysChecker()
			{
				cpuCategory = "Processor";//
				cpuCounterName = "% Processor Time";
				process = Process.GetCurrentProcess();
				proCategory = "Process";
				
				try
				{
					cpuCounter = new PerformanceCounter(cpuCategory, cpuCounterName);
					proCounter = new PerformanceCounter(proCategory, cpuCounterName);
					cpuCounter.InstanceName = "_Total";
					proCounter.InstanceName = process.ProcessName;
				}
				catch (Exception ex) { Logger.Instance.Write(ex); }
			}
			
			public void SysCheck(object state)
			{
				Logger.Instance.Write(
                    string.Format("Total CPU usage: {0}% , Process CPU usage: {1}%", 
                                    cpuCounter.NextValue(), proCounter.NextValue()));
			}

            public void Dispose()
            {
                process = null;
                proCounter.Dispose();
                cpuCounter.Dispose();
            }
		}
		
		private const int SYS_CHECK_INTERVAL = 60000;
		
		private AutoResetEvent autoEvent = null;
		private SysChecker sysChecker = null;
		private TimerCallback sysCheckerCb = null;
		private Timer sysCheckerTimer = null;
		
		public AppAgent()
		{
			autoEvent = new AutoResetEvent(false);
			sysChecker = new SysChecker();
			sysCheckerCb = sysChecker.SysCheck;
			sysCheckerTimer = new Timer(sysCheckerCb, autoEvent, 
			                            0, SYS_CHECK_INTERVAL);
		}

        public void Dispose()
        {
            sysCheckerTimer.Dispose();
            sysCheckerCb = null;
            sysChecker.Dispose();
            autoEvent.Close();
        }
	}
}
