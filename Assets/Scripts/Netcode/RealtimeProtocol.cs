public static class RealtimeProtocol
{
    public const string EndpointPath = "/realtime";

    public static class MessageTypes
    {
        public const string JoinMatch = "join_match";
        public const string MatchConnected = "match_connected";
        public const string MatchStarted = "match_started";
        public const string MatchCancelled = "match_cancelled";
        public const string MatchCompleted = "match_completed";
        public const string ParticipantJoined = "participant_joined";
        public const string ParticipantLeft = "participant_left";
        public const string PlayerCommand = "player_command";
        public const string Error = "error";
    }
}
