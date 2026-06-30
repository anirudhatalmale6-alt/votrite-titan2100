// Product:	VotRite
// Module:  Global.cs
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
using Google.Apis.Services;
using Google.Apis.Translate.v2;
using Google.Apis.Util;
using System.Linq;

namespace VotRite
{
	public class Global
	{
		/// <summary>
		///	Set of global application level constants 
		/// </summary>
		public string APP_PATH;
		public string SLASH;
		public string DB_CONN_STRING;

        private static Global instance;
		
		/// <summary>
		///	Singleton implementation 
		/// </summary>
		public static Global Instance
		{
			get
			{
				if (instance == null) instance = new Global();
				return instance;
			}
		}
		
		public enum AppState { STARTED=1 }
		
		protected Global() 
		{
			APP_PATH = AppDomain.CurrentDomain.BaseDirectory;
#if WINDOWS
			{
				SLASH = "\\";
				DB_CONN_STRING = "URI=file:data\\votrite.s3db,version=3";
			}
#else
			{
				SLASH = "//";
				DB_CONN_STRING = "URI=file:data//votrite.s3db,version=3";
			}
#endif
		}

        public string[] TranslateText(string[] toTranslate, string srcLocale, string tgtLocale)
        {
            string[] strArray = new string[toTranslate.Length];
            try
            {
                toTranslate.CopyTo((Array)strArray, 0);
                TranslateService translateService = new TranslateService(new BaseClientService.Initializer()
                {
                    ApiKey = "AIzaSyDwxUesKrOFYypnT73emUyfss7VH1gE8oY", //previous "AIzaSyDSswF9CPPmvoLgx -00fjF4_U3_mBw9iiM",
                    ApplicationName = "Votrite", // previous "VotRite"
                });
                for (int index = 0; index < toTranslate.Length; ++index)
                {
                    if (string.IsNullOrEmpty(toTranslate[index]))
                        toTranslate[index] = "";
                }
                toTranslate = ((System.Collections.Generic.IEnumerable<string>)toTranslate).Select<string, string>((Func<string, string>)(x => !string.IsNullOrEmpty(x) ? x : "")).ToArray<string>();
                return translateService.Translations.List((Repeatable<string>)toTranslate, tgtLocale).Execute().Translations.ToList<Google.Apis.Translate.v2.Data.TranslationsResource>().Select<Google.Apis.Translate.v2.Data.TranslationsResource, string>((Func<Google.Apis.Translate.v2.Data.TranslationsResource, string>)(x => System.Net.WebUtility.HtmlDecode(x.TranslatedText))).ToArray<string>();
            }
            catch (Exception ex)
            {
                //Logger.Error("Google Translation: " + ex.Message);
            }
            return strArray;
        }

    }
}

