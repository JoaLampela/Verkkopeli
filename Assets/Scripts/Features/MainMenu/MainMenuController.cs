using System;
using TMPro;
using UnityEngine;

public class MainMenuController : MonoBehaviour
{
    [SerializeField] private ScenePathSO _matchScene;
    [SerializeField] private TMP_InputField _matchIdInputField;
    [SerializeField] private TMP_Text _statusText;
    private IMatchmakingService _matchmakingService;
    private ISceneFlowController _sceneFlowController;

    public void Bind(IMatchmakingService matchmakingService, ISceneFlowController sceneFlowController)
    {
        _matchmakingService = matchmakingService ?? throw new ArgumentNullException(nameof(matchmakingService));
        _sceneFlowController = sceneFlowController ?? throw new ArgumentNullException(nameof(sceneFlowController));
    }

    public async void CreateMatchAsync()
    {
        try
        {
            MatchInfo match = await _matchmakingService.CreateMatchAsync(destroyCancellationToken);
            _matchIdInputField.text = match.MatchId.ToString();
            await _matchmakingService.ConnectAsync(match.MatchId, destroyCancellationToken);
            _statusText.text = $"Connected to Match: {match.MatchId}";
            _sceneFlowController.ChangePrimaryScene(_matchScene);
        }
        catch (Exception ex)
        {
            Debug.LogException(ex);
            _statusText.text = "Unable to create or connect";
        }
    }

    public async void JoinMatchAsync()
    {
        if (!Guid.TryParse(_matchIdInputField.text, out Guid guid))
        {
            _statusText.text = "Invalid MatchId";
            return;
        }

        try
        {
            MatchId matchId = new(guid);
            MatchInfo match = await _matchmakingService.JoinMatchAsync(matchId, destroyCancellationToken);
            await _matchmakingService.ConnectAsync(match.MatchId, destroyCancellationToken);
            _statusText.text = $"Connected to Match: {match.MatchId}";
            _sceneFlowController.ChangePrimaryScene(_matchScene);
        }
        catch (Exception ex)
        {
            Debug.LogException(ex);
            _statusText.text = "Unable to join or connect";
        }
    }
}
