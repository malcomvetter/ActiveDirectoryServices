namespace System.DirectoryServices.AccountManagement
{
    [Flags]
    public enum GroupType
    {
        System = 1,
        Global = 2,
        DomainLocal = 4,
        Universal = 8,
        Security = unchecked((int) 2147483648)
    }
}