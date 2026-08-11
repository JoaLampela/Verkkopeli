using System;

public readonly struct MatchId : IEquatable<MatchId>
{
    public Guid Guid { get; }

    public MatchId(Guid guid)
    {
        if (guid == Guid.Empty) throw new ArgumentException(nameof(guid));

        Guid = guid;
    }

    public override bool Equals(object obj) => obj is MatchId other && Equals(other);

    public override int GetHashCode() => Guid.GetHashCode();

    public override string ToString() => Guid.ToString();

    public bool Equals(MatchId other) => Guid.Equals(other.Guid);

    public static bool operator ==(MatchId left, MatchId right) => left.Equals(right);

    public static bool operator !=(MatchId left, MatchId right) => !left.Equals(right);
}
