using System;

public sealed class GameplaySessionService : IGameplaySessionService
{
    private IGameplaySessionContext _currentSession;

    public void SetCurrentSession(IGameplaySessionContext sessionContext)
    {
        if (_currentSession != null) throw new InvalidOperationException(nameof(SetCurrentSession));

        _currentSession = sessionContext ?? throw new ArgumentNullException(nameof(sessionContext));
    }

    public void ClearCurrentSession()
    {
        _currentSession = null;
    }

    public bool TryGetSessionContext(out IGameplaySessionContext sessionContext)
    {
        if (_currentSession == null)
        {
            sessionContext = null;
            return false;
        }

        sessionContext = _currentSession;
        return true;
    }
}
