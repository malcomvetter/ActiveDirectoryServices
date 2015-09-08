using System;
using System.Xml.Serialization;

namespace AdLdsSync
{
    [XmlRoot("Computer")]
    public class Computer : IComparable
    {
        public string Cn = string.Empty;
        public string Description = string.Empty;
        public string Dn = string.Empty;
        public DateTime LastLogonTimestamp = DateTime.MinValue;
        public string OperatingSystem = string.Empty;
        public DateTime WhenCreated = DateTime.MinValue, WhenModified = DateTime.MinValue;

        #region IComparable Members

        public int CompareTo(object obj)
        {
            var result = 1;
            var computer = obj as Computer;
            if (computer != null)
            {
                var c = computer;
                result = String.Compare(OperatingSystem, c.OperatingSystem, StringComparison.Ordinal);
            }
            return result;
        }

        #endregion

        public override bool Equals(Object obj)
        {
            var computer = obj as Computer;
            if (computer != null)
            {
                var c = computer;
                return ((c.Cn == Cn) && (c.Dn == Dn));
            }
            return false;
        }

        public override string ToString()
        {
            return string.Format("{0}", Dn);
        }
    }
}