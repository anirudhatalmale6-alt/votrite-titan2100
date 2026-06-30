// Product:	VotRite
// Module:  ThankyouController.cs
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
using VotRite.MVC;
using VotRite.Models;

namespace VotRite.Controllers
{
    class ThankyouController : ScreenController
    {
        public ThankyouController(ThankyouModel m)
            : base(m)
        {

        }

        public override void HandleKey(string key)
        {
            switch (key)
            {
                default: break;
            }
        }
    }
}
