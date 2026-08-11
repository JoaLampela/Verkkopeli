public readonly struct PlayerColor
{
    public byte Red { get; }
    public byte Green { get; }
    public byte Blue { get; }

    public PlayerColor(byte red, byte green, byte blue)
    {
        Red = red;
        Green = green;
        Blue = blue;
    }
}
