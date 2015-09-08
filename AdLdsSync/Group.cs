using System;
using System.Security.Principal;
using System.Xml.Serialization;

namespace AdLdsSync
{
    [XmlRoot("Group")]
    public class Group : IComparable
    {
        public string Cn = string.Empty;
        public string Description = string.Empty;
        public string DisplayName = string.Empty;
        public string Dn = string.Empty;
        public string Email = string.Empty;
        public SecurityIdentifier Sid;
        public DateTime WhenCreated = DateTime.MinValue, WhenModified = DateTime.MinValue;

        #region IComparable Members

        public int CompareTo(object obj)
        {
            var result = 1;
            var @group = obj as Group;
            if (@group != null)
            {
                var g = @group;
                result = String.Compare(Cn, g.Cn, StringComparison.Ordinal);
            }
            return result;
        }

        #endregion

        public override bool Equals(Object obj)
        {
            var @group = obj as Group;
            if (@group != null)
            {
                var g = @group;
                return ((g.Cn == Cn) && (g.Dn == Dn));
            }
            return false;
        }

        public override string ToString()
        {
            return string.Format("{0}", Dn);
        }
    }
}