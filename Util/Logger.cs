// Product:	VotRite
// Module:  Logger.cs
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
using System.IO;
using System.Text;

namespace VotRite.Util
{
    enum LogLevel
    {
        Debug,
        Info,
        Warn,
        Error
    }

    class Logger
    {
        private const string DATE_TIME_FORMAT = "MM-dd-yyyy HH:mm:ss.fff";
        private static string logName;
        private static Logger instance;

        protected Logger()
        {
            string app_path = Global.Instance.APP_PATH;

            if (!Directory.Exists(app_path + "log"))
                Directory.CreateDirectory(app_path + "log");

            logName = app_path +
                "log" + Global.Instance.SLASH + "vr_" +
                DateTime.Now.ToString(DATE_TIME_FORMAT).
                Replace("-", "").Replace(":", "").
                Replace(" ", "_").Replace(".", "") +
                ".log";
        }

        public static Logger Instance
        {
            get
            {
                if (instance == null) instance = new Logger();
                return instance;
            }
        }

        public void Write(string text)
        {
            Write(text, LogLevel.Info);
        }

        public void Write(string text, LogLevel level)
        {
            StreamWriter sw;
            lock (this)
            {
                if (!File.Exists(logName))
                {
                    sw = File.CreateText(logName);
                    Log("New log file created", sw);
                }
                else
                    sw = File.AppendText(logName);

                Log(text, sw, level);
                sw.Close();
                sw.Dispose();
            }
        }
		
		public void Write(Exception e) {
			int counter = 1;
			StringBuilder sb = new StringBuilder(); 
			while (e != null && counter < 100) {
				sb.AppendLine(string.Format("{0} - {1}", counter, e.ToString()));
				e = e.InnerException;
				counter++;
			}
			
			Write(sb.ToString(), LogLevel.Error);
		}

        private static void Log(string text, TextWriter writer, LogLevel level = LogLevel.Info)
        {
#if DEBUG
            //nothing
#else
            if (level == LogLevel.Debug) return;
#endif
			try
			{
	            writer.WriteLine("[ {0:" + DATE_TIME_FORMAT + "} ][{1}][ {2} ]",
	                    DateTime.Now, level, text);
	
	            // Update the underlying file.
	            writer.Flush();
			}
			catch (Exception ex) { Console.WriteLine(ex); }
        }
    }
}