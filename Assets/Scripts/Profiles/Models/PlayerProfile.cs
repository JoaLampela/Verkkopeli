public readonly struct PlayerProfile
{
    public PlayerId PlayerId { get; }
    public string DisplayName { get; }
    public PlayerColor PlayerColor {get; }

    public PlayerProfile(PlayerId playerId, string displayName, PlayerColor playerColor)
    {
        PlayerId = playerId;
        DisplayName = displayName;
        PlayerColor = playerColor;
    }
}
