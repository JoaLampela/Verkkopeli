public static class RealtimeProtocol
{
    public const string EndpointPath = "/realtime";

    public static class MessageTypes
    {
        public const string JoinMatch = "join_match";
        public const string MatchConnected = "match_connected";
        public const string PlayerCommand = "player_command";
        public const string Error = "error";
    }
}
