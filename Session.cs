// Product:	VotRite
// Module:  Session.cs
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
using System.Timers;
using System.Globalization;
using SpeechLib;
using VotRite.Definition;
using VotRite.Util;
using VotRiteBallotDataManager.AppCode;

namespace VotRite
{
    class Session: IDisposable
    {
        public enum BallotModes
        {
            Normal = 0,
            Audio = 1
        }

		private const int SESSION_CHECK_STATE_INTERVAL = 10000;
        private const int SESSION_EXPIRE_INTERVAL = 300;
        private const int SESSION_TOTAL_TICKS = 10;
        private const int SESSION_TICKS_INTERVAL = 1000;
        private const int SECONDS_IN_MIN = 60;
        private const int BACK_BUTTON_MAX = 3;
		
        private Guid id;
        private BallotDefinition ballot = null;
        private DateTime lastUpdated;
        private Timer stateTimer = null;
		private bool expired;
        private int _backButtonCounter;

        public Guid Id { get { return id; } }
        public BallotDefinition Ballot { get { return ballot; } set { ballot = value; } }
        public DateTime LastUpdated
        {
            get { return lastUpdated; } set { lastUpdated = value; }
        }
        public bool Expired { get { return expired; } }
        public Locale CurrentLocale { get; private set; }
        public BallotModes BallotMode { get; set; }
        public ISpeechGrammarRule GrammarRule { get; set; }

        public Session(BallotDefinition b)
        {
            id = Guid.NewGuid();
            CurrentLocale = null;
            ballot = b;
            BallotMode = b.BallotMode;
            lastUpdated = DateTime.Now;
            stateTimer = new Timer(SESSION_CHECK_STATE_INTERVAL);
            stateTimer.Elapsed += new ElapsedEventHandler(CheckState_Elapsed);
            stateTimer.Enabled = true;
            _backButtonCounter = 0;
			
			Logger.Instance.Write("Session opened: " + id.ToString());
        }

        // Jim Kapsis.
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        // Jim Kapsis.
        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                ballot.Dispose();
                stateTimer.Dispose();

                if (CurrentLocale != null) CurrentLocale.Dispose();
            }
        }

        public void SetLocale(Locale loc)
        {
            CurrentLocale = loc;
			
			DateTime dt = DateTime.Now;
            CultureInfo culture;
            if(CurrentLocale.Code == "ca-CA")
                culture = CultureInfo.GetCultureInfo(0x7C04);
			else if(CurrentLocale.Code == "ma-MA")
                culture = CultureInfo.GetCultureInfo(0x0004);
            else culture = new CultureInfo(CurrentLocale.Code);
            ballot.Date = dt.ToString("dddd, MMMM dd, yyyy", culture.IsNeutralCulture ? new CultureInfo("en-US") : culture);
        }

        private void CheckState_Elapsed(object sender, ElapsedEventArgs args)
        {
            TimeSpan interval = DateTime.Now - lastUpdated;

            if (interval.TotalSeconds > SESSION_EXPIRE_INTERVAL)
            {
                Logger.Instance.Write("total seconds: " + interval.TotalSeconds);
                Logger.Instance.Write("Session " + id + " is about to expire");
                expired = true;
                stateTimer.Enabled = false;
                AppManager.Instance.SessionExpired();
            }
            else
                if (!stateTimer.Enabled) stateTimer.Enabled = true;
        }

        public void Reset() 
        {
            expired = false;
            if (!stateTimer.Enabled) stateTimer.Enabled = true; 
        }

        public void AddBackButtonCounter()
        {
            // keep count of how many times back button is pressed
            _backButtonCounter++;
        }

        public bool CheckBackButtonState()
        {
            // if user clicks back button 3 times, disable it
            // user will either reset the ballot or proceed
            if(_backButtonCounter < (BACK_BUTTON_MAX))
            {
                // button is enabled
                return true;
            }
            else
            {
                // button is disabled
                return false;
            }
        }
    }
    
}
