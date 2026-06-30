using System;
using System.Collections.Generic;

using VotRiteBallotDataManager.AppCode;

namespace VotRite.Definition
{
    class CountyDefinition : IDisposable
    {
        public CountyDefinition()
        {
            Counties = new List<County>();
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
                if (Counties != null) Counties.Clear();
            }
        }

        public List<County> Counties;
    }
}