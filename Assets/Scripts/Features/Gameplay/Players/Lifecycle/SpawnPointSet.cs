using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public readonly struct SpawnPointSet
{
    public IReadOnlyList<Transform> Points { get; }

    public SpawnPointSet(IEnumerable<Transform> transforms)
    {
        if (transforms == null) throw new ArgumentNullException(nameof(transforms));

        List<Transform> points = transforms.ToList();

        if (points.Count == 0
        || points.Any(point => point == null))
            throw new ArgumentException(nameof(transforms));

        Points = points;
    }
}
