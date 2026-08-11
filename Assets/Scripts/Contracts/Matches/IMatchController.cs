using System;

public interface IMatchController : IMatchResultSource
{
    public bool TryDeclareWinner(PlayerId playerId);
}
