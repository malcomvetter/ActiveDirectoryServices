using System;
using System.Security.Principal;
using System.Xml.Serialization;

namespace AdLdsSync
{
    [XmlRoot("User")]
    public class User : IComparable
    {
        public string Cn = string.Empty;
        public string Description = string.Empty;
        public string DisplayName = string.Empty;
        public string Dn = string.Empty;
        public string Email = string.Empty;
        public bool Enabled = false;
        public Guid Guid;
        public bool IsGeneric = false;
        public DateTime LastLogonTimestamp = DateTime.MinValue;
        public DateTime PasswordLastSet = DateTime.MinValue;
        public SecurityIdentifier Sid;
        public short Status = 0;

        public User()
        {
            Cn = string.Empty;
            Dn = string.Empty;
            DisplayName = string.Empty;
            Description = string.Empty;
            Email = string.Empty;
            Enabled = false;
        }

        #region IComparable Members

        public int CompareTo(object obj)
        {
            const int result = 1;
            var user = obj as User;
            if (user != null)
            {
                var u = user;
                if (u.Sid != null)
                {
                    return Sid.CompareTo(u.Sid);
                }
                return !string.IsNullOrWhiteSpace(u.Guid.ToString())
                    ? Guid.CompareTo(u.Guid)
                    : String.Compare(Cn, u.Cn, StringComparison.Ordinal);
            }
            return result;
        }

        #endregion

        public override bool Equals(Object obj)
        {
            var user = obj as User;
            if (user != null)
            {
                var u = user;
                if (u.Sid != null)
                {
                    return (u.Sid == Sid);
                }
                if (!string.IsNullOrWhiteSpace(u.Guid.ToString()))
                {
                    return (u.Guid == Guid);
                }
                return ((u.Cn.ToLowerInvariant() == Cn.ToLowerInvariant()) &&
                        (u.Dn.ToLowerInvariant() == Dn.ToLowerInvariant()));
            }
            return false;
        }

        public override string ToString()
        {
            return string.Format("{0} ({1})", Cn, Sid);
        }
    }
}