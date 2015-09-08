using System.Globalization;
using System.Linq;
using System.Security.Principal;
using System.Xml.Serialization;

namespace System.DirectoryServices.AccountManagement
{
    [XmlRoot("SID")]
    public class Sid : IComparable
    {
        //A wrapper of the SecurityIdentifier class, since that class does not work well with XML Serialization

        public readonly String SecurityIdentifier = String.Empty;

        [XmlIgnore] public int BinaryLength = 0;

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
                if (s.sid != null)
                {
                    result = String.Compare(SecurityIdentifier.ToLowerInvariant(),
                        s.SecurityIdentifier.ToLowerInvariant(), StringComparison.Ordinal);
                }
            }
            var identifier = obj as SecurityIdentifier;
            if (identifier != null)
            {
                var s = identifier;
                result = String.Compare(SecurityIdentifier.ToLowerInvariant(), s.ToString().ToLowerInvariant(),
                    StringComparison.Ordinal);
            }
            var s1 = obj as string;
            if (s1 != null)
            {
                var s = s1;
                if (!String.IsNullOrEmpty(s))
                {
                    result = String.Compare(SecurityIdentifier.ToLowerInvariant(), s.ToLowerInvariant(),
                        StringComparison.Ordinal);
                }
            }
            return result;
        }

        #endregion

        public override int GetHashCode()
        {
            return SecurityIdentifier.GetHashCode();
        }

        public override bool Equals(Object obj)
        {
            var sid1 = obj as Sid;
            if (sid1 != null)
            {
                var s = sid1;
                if (s.sid != null)
                {
                    return (s.SecurityIdentifier.ToLowerInvariant() == SecurityIdentifier.ToLowerInvariant());
                }
            }
            var identifier = obj as SecurityIdentifier;
            if (identifier != null)
            {
                var s = identifier;
                return (s.ToString().ToLowerInvariant() == SecurityIdentifier.ToLowerInvariant());
            }
            var s1 = obj as string;
            if (s1 != null)
            {
                var s = s1;
                if (!String.IsNullOrEmpty(s))
                {
                    return (s.ToLowerInvariant() == SecurityIdentifier.ToLowerInvariant());
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

        public bool IsSidBinaryMatch(byte[] a, byte[] b)
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