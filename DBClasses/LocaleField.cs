using System.Collections.Generic;
using System.Data.SQLite;
using VotRite;

namespace VotRiteBallotDataManager.AppCode
{
    public class LocaleField: System.IDisposable
    {
        public int LocaleId { get; set; }
        public string FieldName { get; set; }
        public string FieldValue { get; set; }
        public string FieldTitle { get; set; }
        public int ShowOrder { get; set; }        

        public bool Equals(LocaleField other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return other.LocaleId == LocaleId && Equals(other.FieldName, FieldName);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != typeof(LocaleField)) return false;
            return Equals((LocaleField)obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (LocaleId * 397) ^ (FieldName != null ? FieldName.GetHashCode() : 0);
            }
        }

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
    }
}
