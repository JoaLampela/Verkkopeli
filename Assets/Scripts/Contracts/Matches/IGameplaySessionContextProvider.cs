public interface IGameplaySessionContextProvider
{
    public bool TryGetSessionContext(out IGameplaySessionContext sessionContext);
}
