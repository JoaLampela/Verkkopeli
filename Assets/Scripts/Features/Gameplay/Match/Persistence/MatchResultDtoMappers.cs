public static class MatchResultDtoMappers
{
    public static MatchResultDto ToDto(this MatchResult result)
    {
        return new MatchResultDto
            {
                matchId = result.MatchId.ToString(),
                winnerId = result.WinnerId.ToString(),
                endTime = result.EndTime.ToString("O")
            };
    }
}
