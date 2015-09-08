using System;
using System.Globalization;
using System.Linq;
using System.Security.Principal;
using System.Xml.Serialization;

namespace AdLdsSync
{
    [XmlRoot("SID")]
    public class Sid : IComparable
    {
        //A wrapper of the SecurityIdentifier class, since that class does not work well with XML Serialization

        [XmlIgnore] public int BinaryLength = 0;

        public String SecurityIdentifier = String.Empty;

        //Don't want to serialize the BinaryLength field to XML, just want it to be here for interoperability with the SecurityIdentifier class
        private SecurityIdentifier sid;

        public Sid()
        {
            SecurityIdentifier = string.Empty;
            sid = null;
            BinaryLength = 0;
        }

        public Sid(SecurityIdentifier sid)
        {
            this.sid = sid;
            SecurityIdentifier = sid.ToString();
            BinaryLength = sid.BinaryLength;
        }

        public Sid(string sid)
        {
            SecurityIdentifier = sid;
            this.sid = new SecurityIdentifier(sid);
            BinaryLength = this.sid.BinaryLength;
        }

        public Sid(byte[] binaryForm, int offset)
        {
            sid = new SecurityIdentifier(binaryForm, offset);
            BinaryLength = sid.BinaryLength;
            SecurityIdentifier = sid.ToString();
        }

        #region IComparable Members

        public int CompareTo(object obj)
        {
            var result = 1;
            var sid1 = obj as Sid;
            if (sid1 != null)
            {
                var s = sid1;
                result = s.sid != null
                    ? sid.CompareTo(s.sid)
                    : String.Compare(SecurityIdentifier.ToLowerInvariant(), s.SecurityIdentifier.ToLowerInvariant(),
                        StringComparison.Ordinal);
            }
            var identifier = obj as SecurityIdentifier;
            if (identifier != null)
            {
                var s = identifier;
                result = sid.CompareTo(s);
            }
            if ((obj != null) && ((obj is String) || obj is string))
            {
                var s = (String) obj;
                if (!String.IsNullOrEmpty(s))
                {
                    result = String.Compare(SecurityIdentifier.ToLowerInvariant(), s.ToLowerInvariant(),
                        StringComparison.Ordinal);
                }
            }
            return result;
        }

        #endregion

        public override bool Equals(Object obj)
        {
            var sid1 = obj as Sid;
            if (sid1 != null)
            {
                var s = sid1;
                if (s.sid != null)
                {
                    return (s.sid == sid);
                }
                return (s.SecurityIdentifier.ToLowerInvariant() == SecurityIdentifier.ToLowerInvariant());
            }
            var identifier = obj as SecurityIdentifier;
            if (identifier != null)
            {
                var s = identifier;
                return (s == sid);
            }
            if ((obj is string) || (obj is String))
            {
                var s = (String) obj;
                if (!String.IsNullOrEmpty(s))
                {
                    return (s == sid.ToString());
                }
            }
            return false;
        }

        public override string ToString()
        {
            return SecurityIdentifier;
        }

        public string ToHexString()
        {
            var hex = string.Empty;
            if (!string.IsNullOrEmpty(SecurityIdentifier))
            {
                var sidInBytes = new byte[BinaryLength];
                GetBinaryForm(sidInBytes);
                hex = BitConverter.ToString(sidInBytes);
                hex = hex.Replace("-", " ");
            }
            return hex;
        }

        public void GetBinaryForm(byte[] binaryForm, int offset)
        {
            sid.GetBinaryForm(binaryForm, offset);
        }

        public void GetBinaryForm(byte[] binaryForm)
        {
            GetBinaryForm(binaryForm, 0);
        }

        public bool IsBinaryMatch(byte[] a, byte[] b)
        {
            if (a.Count() != b.Count())
            {
                return false;
            }
            for (var i = 0; i < a.Count(); i++)
            {
                if (a[i].ToString(CultureInfo.InvariantCulture) != b[i].ToString(CultureInfo.InvariantCulture))
                {
                    return false;
                }
            }
            return true;
        }
    }
}