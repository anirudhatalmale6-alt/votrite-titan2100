// Product:	VotRite
// Module:  PinPadMenuDefinition.cs
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
using System.Collections.Generic;

namespace VotRite.Definition
{
    class PinPadMenuDefinition: IDisposable
    {
        private string name;
        private List<PinPadMenuItem> item;
        private int activeItem;

        public string Name { get { return name; } set { name = value; } }
        public List<PinPadMenuItem> Item
        {
            get { return item; }
            set { item = value; }
        }
        public int ActiveItem
        {
            get { return activeItem >= 0 ? activeItem : (activeItem = 0); }
            set { activeItem = value; }
        }

        public PinPadMenuDefinition()
        {
            item = new List<PinPadMenuItem>();
            activeItem = -1;
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
                if (item != null)
                {
                    item.ForEach(ppmItem => ppmItem.Dispose());
                    item.Clear();
                }
            }
        }
    }
}