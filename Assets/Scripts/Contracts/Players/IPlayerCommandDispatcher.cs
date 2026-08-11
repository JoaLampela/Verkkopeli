public interface IPlayerCommandDispatcher
{
    public bool Dispatch(PlayerId playerId, in PlayerCommand playerCommand);
}
