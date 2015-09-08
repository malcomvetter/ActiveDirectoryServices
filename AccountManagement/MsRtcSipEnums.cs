namespace System.DirectoryServices.AccountManagement
{
    public enum MsRtcSipArchivingEnabledOptions
    {
        DoNotArchive = 0,
        ArchiveInternalCommunications = 4,
        AchiveFederatedCommunications = 8,
        ArchiveAllCommunications = 12
    }

    public enum MsRtcSipOptionFlagOptions
    {
        None = 0,
        EnablePublicIM = 1,
        RemoteCallControl = 16,
        AllowOrganizeMeetingWithAnonymousParticipants = 64,
        UnifiedCommunicationsEnabled = 128,
        EnhancedPresenceEnabled = 256,
        EnhancedPresenceAndPublicIM = 257,
        RemoveCallControlDualMode = 512,
        AutoAttendantEnabled = 1024
    }
}