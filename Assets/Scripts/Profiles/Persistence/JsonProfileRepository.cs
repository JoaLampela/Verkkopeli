using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

public sealed class JsonProfileRepository : IPlayerProfileRepository
{
    private readonly string _path;

    public JsonProfileRepository(string path)
    {
        if (string.IsNullOrWhiteSpace(path)) throw new ArgumentException(nameof(path));

        _path = path;
    }

    public async Task<IReadOnlyList<PlayerProfile>> LoadProfilesAsync(CancellationToken ct = default)
    {
        if (!File.Exists(_path)) return new List<PlayerProfile>();

        using FileStream stream = new(_path, FileMode.Open, FileAccess.Read, FileShare.None, bufferSize: 4096, useAsync: true);
        using StreamReader reader = new(stream, Encoding.UTF8);
        string json = await reader.ReadToEndAsync(); // Cancellation Token cannot be passed to reader in .NET 2.0
        ct.ThrowIfCancellationRequested(); // This is why we do this ugliness before .NET 5
        Debug.Log($"Loaded Profile from {_path}.");
        return JsonUtility.FromJson<PlayerProfilesDto>(json).FromDto();
    }

    public async Task SaveProfilesAsync(IReadOnlyCollection<PlayerProfile> playerProfiles, CancellationToken ct = default)
    {
        string json = JsonUtility.ToJson(playerProfiles.ToDto(), prettyPrint: true); // formats the JSON
        using FileStream stream = new(_path, FileMode.Create, FileAccess.Write, FileShare.None, bufferSize: 4096, useAsync: true);
        using StreamWriter writer = new(stream, Encoding.UTF8);
        await writer.WriteAsync(json.AsMemory(), ct);
        Debug.Log($"Saved Profile to {_path}.");
    }
}
