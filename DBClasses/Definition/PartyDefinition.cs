using System;
using System.Collections.Generic;
using VotRite.DBClasses;

namespace VotRite.Definition
{
    class PartyDefinition: IDisposable
    {
        public PartyDefinition()
        {
            Parties = new List<Party>();
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
                if ( Parties != null) Parties.Clear();                
            }
        }

        public List<Party> Parties;
    }
}
