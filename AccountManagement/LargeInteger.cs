namespace System.DirectoryServices.AccountManagement
{
    public class LargeInteger : IComparable
    {
        public int HighPart { get; set; }
        public int LowPart { get; set; }

        #region IComparable

        //IComparable Overrides:
        public int CompareTo(object obj)
        {
            const int result = 2;
            var integer = obj as LargeInteger;
            if (integer != null)
            {
                var li = integer;
                if (HighPart > li.HighPart)
                {
                    return 1;
                }
                if (HighPart < li.HighPart)
                {
                    return -1;
                }
                if (HighPart == li.HighPart)
                {
                    if (LowPart > li.LowPart)
                    {
                        return 1;
                    }
                    if (LowPart < li.LowPart)
                    {
                        return -1;
                    }
                    if (LowPart == li.LowPart)
                    {
                        return 0;
                    }
                }
            }
            return result;
        }

        public override bool Equals(Object obj)
        {
            var integer = obj as LargeInteger;
            if (integer != null)
            {
                var li = integer;
                return ((HighPart == li.HighPart) && (LowPart == li.LowPart));
            }
            return false;
        }

        #endregion
    }
}