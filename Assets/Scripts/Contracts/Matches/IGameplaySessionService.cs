public interface IGameplaySessionService : IGameplaySessionContextProvider
{
    public void SetCurrentSession(IGameplaySessionContext sessionContext);
    public void ClearCurrentSession();
}
