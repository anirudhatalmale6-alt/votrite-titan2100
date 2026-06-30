// Product:	VotRite
// Module:  PinPadDefinition.cs
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
    class PinPadDefinition: IDisposable
    {
        private List<PinPadMenuDefinition> menu;
        private int activeMenu;
        private int histMenu;

        public List<PinPadMenuDefinition> Menu
        {
            get { return menu; }
            set { menu = value; }
        }
        public int ActiveMenu
        {
            get { return activeMenu; }
            set { histMenu = activeMenu; activeMenu = value; }
        }
        public int HistMenu
        {
            get { return histMenu; }
            set { histMenu = value; }
        }

        public PinPadDefinition()
        {
            menu = new List<PinPadMenuDefinition>();
            activeMenu = histMenu = -1;
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
                if (menu != null)
                {
                    menu.ForEach(ppmDef => ppmDef.Dispose());
                    menu.Clear();
                }
            }
        }

        public void GetMenu(string name)
        {
            int index = FindMenuIndex(name);
            ActiveMenu = index;
        }

        public int FindMenuIndex(string search)
        {
            int index = menu.FindIndex(
                delegate(PinPadMenuDefinition tmpMenu)
                {
                    if (tmpMenu.Name == search) return true;
                    return false;
                });
            return index;
        }
    }
}