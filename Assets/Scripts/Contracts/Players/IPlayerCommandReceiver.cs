public interface IPlayerCommandReceiver
{
    public PlayerId PlayerId { get; }
    public void Receive(in PlayerCommand playerCommand);
}
