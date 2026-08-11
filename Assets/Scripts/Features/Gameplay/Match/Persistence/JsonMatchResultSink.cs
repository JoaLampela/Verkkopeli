using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

public sealed class JsonMatchResultSink : IMatchResultSink
{
    private readonly string _path;

    public JsonMatchResultSink(string path)
    {
        if (string.IsNullOrWhiteSpace(path)) throw new ArgumentException(nameof(path));

        _path = path;
    }

    public async Task HandleResultAsync(MatchResult matchResult, CancellationToken ct = default)
    {
        await UpdateMatchResults(matchResult, ct);
    }

    private async Task UpdateMatchResults(MatchResult matchResult, CancellationToken ct = default)
    {
        ct.ThrowIfCancellationRequested();
        MatchResultHistoryDto history = await LoadMatchResultHistory(ct);
        history.results ??= new List<MatchResultDto>();
        history.results.Add(matchResult.ToDto());
        string json = JsonUtility.ToJson(history, prettyPrint: true);
        ct.ThrowIfCancellationRequested();
        using FileStream stream = new(_path, FileMode.Create, FileAccess.Write, FileShare.None, bufferSize: 4096, useAsync: true);
        using StreamWriter writer = new(stream, Encoding.UTF8);
        await writer.WriteAsync(json.AsMemory());
        Debug.Log($"Updated Match History at {_path}.");
    }

    private async Task<MatchResultHistoryDto> LoadMatchResultHistory(CancellationToken ct = default)
    {
        using FileStream readStream = new(_path, FileMode.OpenOrCreate, FileAccess.Read, FileShare.None, bufferSize: 4096, useAsync: true);
        using StreamReader reader = new(readStream, Encoding.UTF8);
        string json = await reader.ReadToEndAsync();
        ct.ThrowIfCancellationRequested();
        MatchResultHistoryDto history = (string.IsNullOrWhiteSpace(json)
            ? new MatchResultHistoryDto { version = AppConstants.MatchHistory.FormatVersion, results = new List<MatchResultDto>() }
            : JsonUtility.FromJson<MatchResultHistoryDto>(json)) ?? throw new InvalidDataException("Match history could not be parsed");

        if (history.version != AppConstants.MatchHistory.FormatVersion) throw new InvalidDataException(nameof(history.version));

        return history;
    }
}
