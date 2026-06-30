using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;

namespace VotRite.Util
{
    internal static class ImageEncryptor
    {
        private static readonly Queue<string> _queue = new Queue<string>();
        private static string _processPath;

        public static void EncryptFolder(IEnumerable<string> folderPath, string imageType, string processToStart = null)
        {
            if (processToStart == null)
            {
                if (_processPath != null)
                {
                }
                else
                {
                    var appPath = Global.Instance.APP_PATH.TrimEnd(new[] {'\\'});
                    var index = appPath.LastIndexOf('\\');
                    _processPath = appPath.Substring(0, index) +
                                   AppManager.GetPathToCommonFile(AppManager.Configuration["System"]["EncryptRecordBatchFilePath"]).
                                       Remove(0, 2);
                }
                processToStart = _processPath;
            }
            lock (_queue)
            {
                foreach (var folder in folderPath)
                {
                    var files = Directory.GetFiles(folder, imageType);
                    foreach (var file in files)
                    {
                        _queue.Enqueue(file);
                    }
                }
            }
            StartEncryptionProcess(processToStart);
        }

        private static void StartEncryptionProcess(string processToStart)
        {
            var info = new ProcessStartInfo(processToStart)
                           {UseShellExecute = false, CreateNoWindow = true /*, RedirectStandardOutput = true*/};
            while (_queue.Count > 0)
            {
                try
                {
                    lock (_queue)
                    {
                        if (_queue.Count > 0)
                        {
                            info.Arguments = _queue.Dequeue();
                            /*var proc = */
                            Process.Start(info);

                            /*while (!proc.StandardOutput.EndOfStream)
                            {
                                string line = proc.StandardOutput.ReadLine();
                                Logger.Instance.Write(line);
                                // do something with line
                            }*/
                        }
                    }
                }
                catch (Exception ex)
                {
                    Logger.Instance.Write(ex);
                }
            }
        }
    }
}