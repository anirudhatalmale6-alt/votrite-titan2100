// Product:	VotRite
// Module:  IOHandler.cs
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
using System.Collections.Generic;
using System.Windows.Forms;
using System.Text;
using iTextSharp.text.pdf;
using iTextSharp.text.pdf.parser;

namespace VotRite.Util
{
    public sealed class IOHandler
    {
        internal class VarRecord
        {
            private enum Record { Var, Word };

            private string var;
            private string word;

            public string Var { get { return var; } set { var = value; } }
            public string Word { get { return word; } set { word = value; } }

            public VarRecord(string text) { Parse(text); }

            private void Parse(string text)
            {
                char[] sep = { ';' };
                string[] tokens = text.Split(sep[0]);

                var = tokens[(int)Record.Var].Trim();
                word = tokens[(int)Record.Word].Trim();

            }
        }

        public static bool SaveConfig(string section, string key, string Value)
        {
            string pathToCommonFile = IOHandler.GetPathToCommonFile("config" + Global.Instance.SLASH + "votrite.conf");
            List<string> list = new List<string>();
            bool flag;

            try
            {
                SortedList<string, SortedList<string, string>> decrypted = DecryptConfig();
                string sec;
                foreach (KeyValuePair<string, SortedList<string, string>> kv in decrypted)
                {
                    sec = string.Format("[{0}]", kv.Key);
                    if (!list.Contains(sec))
                    {
                        list.Add(sec);
                    }

                    foreach(KeyValuePair<string, string> kvv in kv.Value)
                    {
                        if (kv.Key == section && kvv.Key == key)
                        {
                            list.Add(string.Format("{0}={1}", kvv.Key, Value));
                        } else
                        {
                            list.Add(string.Format("{0}={1}", kvv.Key, kvv.Value));
                        }
                    }
                }

                List<string> encrypted = new List<string>();
                foreach (string line in list)
                {
                    encrypted.Add(Crypto.Instance.Encrypt(line, "votrite"));
                }

                File.WriteAllLines(pathToCommonFile, encrypted.ToArray());
                flag = true;
            }
            catch (IOException ex)
            {
                Logger.Instance.Write((Exception)ex);
                flag = false;
            }
            return flag;
        }


        public static string ReadConfig(string section, string key)
        {
            string value = "";
            StreamReader reader;

            try
            {
                reader = new StreamReader(GetPathToCommonFile(
                                          "config" +
                                          Global.Instance.SLASH +
                                          "votrite.conf"));

                lock (reader)
                {
                    string line = "";
                    bool section_found = false;
                    bool key_found = false;
                    string[] tokens;
                    char[] sep = { '=' };

                    while ((line = reader.ReadLine()) != null)
                    {
                        if (line == '[' + section + ']')
                            section_found = true;
                        if (section_found)
                        {
                            tokens = line.Split(sep, 2);
                            if (tokens[0] == key)
                            {
                                value = tokens[1];
                                key_found = true;
                            }
                        }

                        if (key_found) break;
                    }

                    reader.Close();
                }

                reader.Dispose();
            }
            catch (IOException e) { Logger.Instance.Write(e); }

            return value;
        }

        public static SortedList<string, SortedList<string, string>> ReadConfig()
        {
            StreamReader reader;
            SortedList<string, SortedList<string, string>> conf = new SortedList<string, SortedList<string, string>>();

            try
            {
                reader = new StreamReader(GetPathToCommonFile(
                                          "config" +
                                          Global.Instance.SLASH +
                                          "votrite.conf"));

                lock (reader)
                {
                    string line = "";
                    string[] tokens;
                    char[] sep = { '=' };
                    string key = "";
                    SortedList<string, string> val = new SortedList<string, string>();

                    while ((line = reader.ReadLine()) != null)
                    {
                        //Console.WriteLine(line);

                        if (line.Trim() == "")
                        {
                            continue;
                        }

                        tokens = line.Trim().Split(sep);

                        if (tokens.Length < 2)
                        {
                            if (tokens[0].StartsWith("[") && tokens[0].EndsWith("]"))
                            {
                                key = tokens[0].Substring(1, tokens[0].Length - 2);
                                val = new SortedList<string, string>();
                                conf.Add(key, val);
                            }
                        }
                        else
                        {
                            val.Add(tokens[0], tokens[1]);
                        }
                    }
                }

                reader.Dispose();
            }
            catch (IOException e) { Logger.Instance.Write(e); }

            return conf;
        }

        public static SortedList<string, SortedList<string, string>> DecryptConfig()
        {
            Crypto crypto;
            string encPwd = "votrite";
            StreamReader reader;
            SortedList<string, SortedList<string, string>> conf = new SortedList<string, SortedList<string, string>>();

            try
            {
                crypto = new Crypto();
                reader = new StreamReader(GetPathToCommonFile(
                                          "config" +
                                          Global.Instance.SLASH +
                                          "votrite.conf"));
                var configpath = GetPathToCommonFile(
                                          "config" +
                                          Global.Instance.SLASH +
                                          "votrite.conf");
                lock (reader)
                {
                    string line = "";
                    string[] tokens;
                    char[] sep = { '=' };
                    string key = "";
                    SortedList<string, string> val = new SortedList<string, string>();
                    System.Text.StringBuilder sb = new System.Text.StringBuilder();
                    while ((line = reader.ReadLine()) != null)
                    {
                        //Console.WriteLine(line);

                        if (line.Trim() == "")
                        {
                            continue;
                        }

                        line = crypto.Decrypt(line, encPwd);
                        sb.AppendLine(line);
                        tokens = line.Trim().Split(sep, 2);

                        if (tokens.Length < 2)
                        {
                            if (tokens[0].StartsWith("[") && tokens[0].EndsWith("]"))
                            {
                                key = tokens[0].Substring(1, tokens[0].Length - 2);
                                val = new SortedList<string, string>();
                                conf.Add(key, val);
                            }
                        }
                        else
                        {
                            val.Add(tokens[0], tokens[1]);
                        }
                    }
                }

                reader.Dispose();
            }
            catch (IOException e) { Logger.Instance.Write(e); }

            return conf;
        }

        public static bool WriteFile(string file, string[] text)
        {
            try
            {
                using (StreamWriter sw = new StreamWriter(file))
                {
                    foreach (string line in text)
                    {
                        sw.WriteLine(line);
                    }
                }
                return true;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return false;
            }
        }

        public static string[] ReadFile(string file)
        {
            string[] lines = null;

            try
            {
                lines = File.ReadAllLines(file);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }

            return lines;
        }

        public static string ReadPdfFile(string fileName)
        {
            StringBuilder text = new StringBuilder();

            if (File.Exists(fileName))
            {
                PdfReader pdfReader = new PdfReader(fileName);

                for (int page = 1; page <= pdfReader.NumberOfPages; page++)
                {
                    ITextExtractionStrategy strategy = new SimpleTextExtractionStrategy();
                    string currentText = PdfTextExtractor.GetTextFromPage(pdfReader, page, strategy);

                    currentText = Encoding.UTF8.GetString(ASCIIEncoding.Convert(Encoding.Default, Encoding.UTF8, Encoding.Default.GetBytes(currentText)));
                    text.Append(currentText);
                }
                pdfReader.Close();
            }
            return text.ToString();
        }
        public static void BackUpDb()
        {
            SortedList<string, SortedList<string, string>> config = IOHandler.DecryptConfig();
            string resultsDbConStr = config["System"]["ResultsDbConStr"];
            string fileName = DateTime.Now.ToString("ddMMyyyy_HHmmss") + ".s3db";
            string from = Global.Instance.APP_PATH + resultsDbConStr;

            try
            {
                File.Copy(from, Global.Instance.APP_PATH + resultsDbConStr.Replace(".s3db", string.Empty) + fileName, true);
            }
            catch (Exception e) { Logger.Instance.Write(e); }

            try
            {
                string backupFlashDriveDataPath = config["System"]["BackupFlashDrivePath"] + "data\\";

                if (!Directory.Exists(backupFlashDriveDataPath))
                    Directory.CreateDirectory(backupFlashDriveDataPath);

                File.Copy(from, backupFlashDriveDataPath + "votrite" + fileName, true);
            }
            catch (Exception e) {
               
                Logger.Instance.Write(e);
                System.Windows.Forms.MessageBox.Show("Backup drive is not available or appears to be unplugged, please connect and restart app again. Votrite will exit now",
                   "Votrite: Backup Drive error", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Stop);
                Environment.Exit(1);
            }
        }

        public static void UpdateBackupDbOnFlashDrive()
        {
            SortedList<string, SortedList<string, string>> config = IOHandler.DecryptConfig();
            string resultsDbConStr = config["System"]["ResultsDbConStr"];
            //resultsDbConStr = resultsDbConStr.Substring(3, resultsDbConStr.Length - 3);
            string from =  resultsDbConStr;
            string fileName = "votrite.s3db";

            try
            {
                string backupFlashDriveDataPath = config["System"]["BackupFlashDrivePath"] + "\\data\\";

                if (!Directory.Exists(backupFlashDriveDataPath))
                    Directory.CreateDirectory(backupFlashDriveDataPath);

                File.Copy(from, backupFlashDriveDataPath + fileName, true);
            }
            catch (Exception e) {
                Logger.Instance.Write(e);
                System.Windows.Forms.MessageBox.Show("Backup drive is not available or appears to be unplugged, please connect and restart app again. Votrite will exit now",
                   "Votrite: Backup Drive error", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Stop);
                Environment.Exit(1);
            }
        }

        public static string GetPathToCommonFile(string path)
        {
            string apppath = Global.Instance.APP_PATH;
            return string.Format("{2}CommonFiles{0}{1}", Global.Instance.SLASH, (string.IsNullOrEmpty(path) ? string.Empty : path), apppath);
        }

        public static string GetPathToScreens()
        {
            return GetPathToCommonFile(string.Format("definition{0}screens{0}", Global.Instance.SLASH));
        }

        public static string GetPathToScreen(int? ballotId)
        {
            return string.Format("{0}{1}{2}", GetPathToScreens(), (ballotId == null ? "default_screen" : "screen" + ballotId), Global.Instance.SLASH);
        }

        public static bool CopyDirectory(string sourceDirName, string destDirName, bool copySubDirs)
        {
            try
            {
                var dir = new DirectoryInfo(sourceDirName);
                var dirs = dir.GetDirectories();
                if (!dir.Exists)
                {
                    Logger.Instance.Write("Source directory does not exist or could not be found: " + sourceDirName);
                }

                if (!Directory.Exists(destDirName))
                {
                    Directory.CreateDirectory(destDirName);
                }

                var files = dir.GetFiles();
                foreach (var file in files)
                {
                    var temppath = System.IO.Path.Combine(destDirName, file.Name);
                    file.CopyTo(temppath, true);
                }

                if (copySubDirs)
                {
                    foreach (var subdir in dirs)
                    {
                        var temppath = System.IO.Path.Combine(destDirName, subdir.Name);
                        CopyDirectory(subdir.FullName, temppath, copySubDirs);
                    }
                }
            }
            catch (Exception e)
            {
                Logger.Instance.Write(e);
                return false;
            }
            return true;
        }

        public static bool DeleteDirectory(string path)
        {
            if (Directory.Exists(path))
            {
                try
                {
                    Directory.Delete(path, true);
                }
                catch (IOException e)
                {
                    Logger.Instance.Write(e);
                    return false;
                }
            }
            return true;
        }
    }
}
