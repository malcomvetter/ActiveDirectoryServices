using System;
using System.Xml.Serialization;

namespace AdLdsSync
{
    [XmlRoot("OperatingSystem")]
    public class OperatingSystem : IComparable
    {
        public string Name = string.Empty;
        public int Population = 0;

        public OperatingSystem()
        {
        }

        public OperatingSystem(int population, string name)
        {
            Population = population;
            Name = name;
        }

        #region IComparable Members

        public int CompareTo(object obj)
        {
            var result = 1;
            var system = obj as OperatingSystem;
            if (system != null)
            {
                var o = system;
                result = Population.CompareTo(o.Population);
            }
            return result;
        }

        #endregion

        public override bool Equals(Object obj)
        {
            var system = obj as OperatingSystem;
            if (system != null)
            {
                var os = system;
                return (os.Name == Name);
            }
            return false;
        }

        public override string ToString()
        {
            return string.Format("{0}", Name);
        }
    }
}