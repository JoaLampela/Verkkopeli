public readonly struct PlayerLookAngles
{
    public float YawDegrees { get; }
    public float PitchDegrees { get; }

    public PlayerLookAngles(float yawDegrees, float pitchDegrees)
    {
        YawDegrees = yawDegrees;
        PitchDegrees = pitchDegrees;
    }
}
