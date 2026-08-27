public interface IPlayerInputSource
{
    public void Enable();
    public void Disable();
    public bool TryGetPlayerInputFrame(out PlayerInputFrame frame);
}
