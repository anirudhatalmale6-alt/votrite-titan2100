// Product:	VotRite
// Module:  DataDefinition.cs
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
using VotRite.UI;

namespace VotRite.Definition
{
    class DataDefinition: System.IDisposable
    {
        //private string title;
        private string text;
        private string group;
        private string photo;
        private string partyLogo;
        private string tag;
        private bool voteable;
        private bool isWritten;

        //public string Title { get { return title; } set { title = value.Trim(); } }
        public int Id { get; set; }
        public int GlobalId { get; set; }
        public string Text { get { return text; } set { text = value.Trim(); } }
        public string Group { get { return group; } set { group = value.Trim(); } }
        public string Photo { get { return photo; } set { photo = value.Trim(); } }
        public string PartyLogo { get { return partyLogo; } set { partyLogo = value.Trim(); } }
		public bool IncumbentFlag {get;set;}
        public VrSelection.SelectionState State { get; set; }

        public bool ReadOnly { get; set; }

        public bool WriteIn { get; set; }

        public string Tag { get { return tag; } set { tag = value.Trim(); } }

        public int ButtonGroup { get; set; }

        public bool Voteable { get { return voteable; } set { voteable = value; } }

        public bool IsWritten { get { return isWritten; } set { isWritten = value; } }

        public DataDefinition() { ReadOnly = false; }

        // Jim Kapsis.
        public void Dispose()
        {
            Dispose(true);
            System.GC.SuppressFinalize(this);
        }

        // Jim Kapsis.
        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {                
            }
        }
        
        public int SlateId { get; set; }

        public int PartyId { get; set; }

        public int Preference { get; set; }
    }
}
