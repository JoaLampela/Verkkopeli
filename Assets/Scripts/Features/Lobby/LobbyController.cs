using System;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public sealed class LobbyController : MonoBehaviour
{
    [SerializeField] private ScenePathSO gameplaySceneSO;
    [SerializeField] private ScenePathSO mainMenuSceneSO;
    [SerializeField] private TMP_Text participantsText;
    [SerializeField] private Button startButton;
    private ISceneFlowController _sceneFlowController;
    private IMatchmakingService _matchmakingService;
    private IMatchContext _matchContext;
    private IMatchSession _matchSession;
    private IMatchRealtimeEventSource _matchRealtimeEventSource;
    private IPlayerProfileContext _profileContext;
    private bool _isBound;
    private bool _isLeaving;

    private void OnDestroy()
    {
        Unbind();
    }

    public void Bind(ISceneFlowController sfc, IMatchmakingService mms, IMatchContext mc, IMatchSession ms, IMatchRealtimeEventSource mres, IPlayerProfileContext ppc)
    {
        Unbind();
        _sceneFlowController = sfc ?? throw new ArgumentNullException(nameof(sfc));
        _matchmakingService = mms ?? throw new ArgumentNullException(nameof(mms));
        _matchContext = mc ?? throw new ArgumentNullException(nameof(mc));
        _matchSession = ms ?? throw new ArgumentNullException(nameof(ms));
        _matchRealtimeEventSource = mres ?? throw new ArgumentNullException(nameof(mres));
        _profileContext = ppc ?? throw new ArgumentNullException(nameof(ppc));
        _matchRealtimeEventSource.MatchStarted += HandleMatchStarted;
        _matchRealtimeEventSource.MatchCancelled += HandleMatchCancelled;
        _matchRealtimeEventSource.ParticipantJoined += UpdateParticipants;
        _matchRealtimeEventSource.ParticipantLeft += UpdateParticipants;
        _isBound = true;
        _isLeaving = false;

        if (!_matchContext.TryGetCurrentMatch(out MatchInfo matchInfo))
            throw new InvalidOperationException("No matchId in memory!");

        if (!_profileContext.TryGetCurrentProfile(out PlayerProfile profile))
            throw new InvalidOperationException("No local profile found!");

        if (!_matchContext.TryGetCurrentMatch(out MatchInfo info))
            throw new InvalidOperationException("No local match found!");

        startButton.gameObject.SetActive(profile.PlayerId == info.HostPlayerId);

        RenderParticipants(matchInfo);
    }

    public void Unbind()
    {
        if (!_isBound) return;

        _matchRealtimeEventSource.MatchStarted -= HandleMatchStarted;
        _matchRealtimeEventSource.MatchCancelled -= HandleMatchCancelled;
        _matchRealtimeEventSource.ParticipantJoined -= UpdateParticipants;
        _matchRealtimeEventSource.ParticipantLeft -= UpdateParticipants;
        _sceneFlowController = null;
        _matchmakingService = null;
        _matchContext = null;
        _matchRealtimeEventSource = null;
        participantsText.text = string.Empty;
        _isBound = false;
    }

    public async void StartMatchAsHost()
    {
        try
        {
            if (!_matchContext.TryGetCurrentMatch(out MatchInfo matchInfo))
                throw new InvalidOperationException(nameof(StartMatchAsHost));
            
            await _matchmakingService.StartMatchAsync(matchInfo.MatchId);
        }
        catch (Exception ex)
        {
            Debug.LogException(ex);
        }
    }

    public async void LeaveLobby()
    {
        try
        {
            if (!_matchContext.TryGetCurrentMatch(out MatchInfo matchInfo))
                throw new InvalidOperationException(nameof(LeaveLobby));

            _isLeaving = true;
            await _matchmakingService.LeaveMatchAsync(matchInfo.MatchId, destroyCancellationToken);
            await _matchmakingService.DisconnectAsync(destroyCancellationToken);
            _sceneFlowController.ChangePrimaryScene(mainMenuSceneSO);
        }
        catch (Exception ex)
        {
            Debug.LogException(ex);
        }
    }

    private async void HandleMatchCancelled(MatchId matchId)
    {
        if (_isLeaving) return;

        if (!_matchContext.TryGetCurrentMatch(out MatchInfo matchInfo)) return;

        if (matchInfo.MatchId != matchId) return;

        try
        {   
            _matchSession.ClearCurrentMatch();
            await _matchmakingService.DisconnectAsync(destroyCancellationToken);
            _sceneFlowController.ChangePrimaryScene(mainMenuSceneSO);
        }
        catch (Exception ex)
        {
            Debug.LogException(ex);
        }
    }

    private async void HandleMatchStarted(MatchId matchId)
    {
        try
        {
            if (!_matchContext.TryGetCurrentMatch(out MatchInfo matchInfo))
                throw new InvalidOperationException("No active match found in-memory!");

            if (matchInfo.MatchId != matchId)
                throw new InvalidOperationException("Started matchId doesn't match current matchId");

            MatchInfo refreshedInfo = await _matchmakingService.RefreshCurrentMatchAsync();

            if (refreshedInfo.MatchStatus != MatchStatus.Running)
                throw new InvalidOperationException("The provided match is not in the state \"running\"!");

            _sceneFlowController.ChangePrimaryScene(gameplaySceneSO);
        }
        catch (Exception ex)
        {
            Debug.LogException(ex);
        }
    }

    private async void UpdateParticipants(MatchId matchId)
    {
        if (_isLeaving) return;

        try
        {
            if (!_matchContext.TryGetCurrentMatch(out MatchInfo info))
                throw new InvalidOperationException(nameof(UpdateParticipants));
            
            if (info.MatchId != matchId)
                throw new InvalidOperationException("IDs don't match!");

            MatchInfo matchInfo = await _matchmakingService.RefreshCurrentMatchAsync();

            if (matchId != matchInfo.MatchId)
                throw new InvalidOperationException("MatchId of updated game doesn't match in-memory matchId!");

            RenderParticipants(matchInfo);
        }
        catch (Exception ex)
        {
            Debug.LogException(ex);
        }
    }

    private void RenderParticipants(MatchInfo matchInfo)
    {
        participantsText.text = string.Join("\n", matchInfo.Participants.Select(player => player.DisplayName));
    }
}
