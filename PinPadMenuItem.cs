// Product:	VotRite
// Module:  PinPadMenuItem.cs
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

namespace VotRite
{
    class PinPadMenuItem: IDisposable
    {
        private int key;
        private string text;
        private string action;
        private string data;
        private bool pwdProtected;
        private bool readOnly;
        private bool maskText;
        private string postAction;
        private string postData;

        public int Key { get { return key; } set { key = value; } }
        public string Text { get { return text; } set { text = value; } }
        public string Action { get { return action; } set { action = value; } }
        public string PostAction
        {
            get { return postAction; }
            set { postAction = value; }
        }
        public string Data { get { return data; } set { data = value; } }
        public string PostData { get { return postData; } set { postData = value; } }
        public bool Protected
        {
            get { return pwdProtected; }
            set { pwdProtected = value; }
        }
        public bool ReadOnly
        {
            get { return readOnly; }
            set { readOnly = value; }
        }
        public bool MaskText
        {
            get { return maskText; }
            set { maskText = value; }
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
            }
        }
    }
}