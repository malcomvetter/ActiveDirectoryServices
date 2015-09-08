using System;
using System.Xml.Serialization;

namespace AdLdsSync
{
    [XmlRoot("AdObject")]
    public class AdObject : IComparable
    {
        public string Cn = string.Empty;
        public string Dn = string.Empty;
        public Sid Sid;

        public AdObject()
        {
            Dn = string.Empty;
            Cn = string.Empty;
            Sid = null;
        }

        public AdObject(string dn, string cn, Sid sid)
        {
            Dn = dn;
            Cn = string.IsNullOrEmpty(cn) ? ConvertDnToCn(Dn) : cn;
            Sid = sid;
        }

        #region IComparable Members

        public int CompareTo(object obj)
        {
            var result = 1;
            var o = obj as AdObject;
            if (o != null)
            {
                var u = o;
                result = u.Sid != null ? Sid.CompareTo(u.Sid) : Dn.CompareTo(u.Dn);
            }
            return result;
        }

        #endregion

        public override bool Equals(Object obj)
        {
            var o = obj as AdObject;
            if (o != null)
            {
                var u = o;
                if (u.Sid != null)
                {
                    return (u.Sid == Sid);
                }
                return (u.Dn.ToLowerInvariant() == Dn.ToLowerInvariant());
            }
            return false;
        }

        public override string ToString()
        {
            return string.Format("{0} ({1}) ({2})", Cn, Sid, Dn);
        }

        public static string ConvertDnToCn(string objectDN)
        {
            var p = objectDN.Split(',');
            var q = p[0].Split('=');
            var cn = q[1];
            return cn;
        }
    }
}