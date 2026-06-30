using System;
using System.Collections.Generic;
using VotRite.DBClasses;

namespace VotRite.Definition
{
    internal class SlatesDefinition: IDisposable
    {
        public SlatesDefinition()
        {
            Slates = new List<Slate>();
            Data = new DataDefinition();
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
                if (Slates != null)
                {
                    Slates.ForEach(s => s.Dispose());
                    Slates.Clear();
                }
                
                if (Data != null) Data.Dispose();
            }
        }

        public List<Slate> Slates { get; set; }

        public DataDefinition Data { get; set; }
    }
}
