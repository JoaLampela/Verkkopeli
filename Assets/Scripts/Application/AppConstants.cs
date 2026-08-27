public static class AppConstants
{
    public static class MatchHistory
    {
        public const string FormatVersion = "1.0.0";
        public const string DataLocation = "match_results.json";
    }

    public static class Api
    {
        public const string LoginPath = "/api/auth/login";
        public const string ProfilePath = "/api/profiles/me";
        public const string MatchPath = "/api/matches";
    }

    public static class Authentication
    {
        public const string DataLocation = "auth_data.json";
    }

    public static class Gameplay
    {
        public const string RealtimePath = "/realtime";
    }
}
