public readonly struct MatchParticipant
{
    public PlayerId PlayerId { get; }
    public string DisplayName { get; }

    public MatchParticipant(PlayerId playerId, string displayName)
    {
        PlayerId = playerId;
        DisplayName = displayName;
    }
}
