using System;

public readonly struct PlayerId : IEquatable<PlayerId>
{
    public Guid Guid { get; }

    public PlayerId(Guid guid)
    {
        if (guid == Guid.Empty) throw new ArgumentException(nameof(guid));

        Guid = guid;
    }

    public override bool Equals(object obj) => obj is PlayerId other && Equals(other);

    public override int GetHashCode() => Guid.GetHashCode();

    public override string ToString() => Guid.ToString();

    public bool Equals(PlayerId other) => Guid.Equals(other.Guid);

    public static bool operator ==(PlayerId left, PlayerId right) => left.Equals(right);

    public static bool operator !=(PlayerId left, PlayerId right) => !left.Equals(right);
}
