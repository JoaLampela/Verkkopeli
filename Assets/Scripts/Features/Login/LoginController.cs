using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public sealed class LoginController : MonoBehaviour
{
    [SerializeField] private ScenePathSO _mainMenuScenePathSO;
    [SerializeField] private TMP_InputField _displayNameField;
    [SerializeField] private Color _playerColor = Color.yellow;
    [SerializeField] private Button _submitButton;
    private IPlayerProfileService _playerProfileService;
    private ISceneFlowController _sceneFlowController;

    public void Bind(IPlayerProfileService pps, ISceneFlowController sfc)
    {
        _playerProfileService = pps ?? throw new ArgumentNullException(nameof(pps));
        _sceneFlowController = sfc ?? throw new ArgumentNullException(nameof(sfc));
    }

    public void Submit()
    {
        SubmitAsync().LogExceptionsAndForget();
    }
    
    private async Awaitable SubmitAsync()
    {
        if (_playerProfileService == null
        || _sceneFlowController == null
        || _mainMenuScenePathSO == null) throw new ArgumentException(nameof(SubmitAsync));

        _submitButton.interactable = false;

        try
        {
            Color32 color = _playerColor;
            PlayerColor playerColor = new(color.r, color.g, color.b);
            string displayName = string.IsNullOrWhiteSpace(_displayNameField.text)
            ? AppConstants.ProfileConfig.DefaultName
            : _displayNameField.text.Trim();

            await _playerProfileService.CreateProfileAsync(displayName, playerColor, destroyCancellationToken);
            if (_playerProfileService.HasProfile) _sceneFlowController.ChangePrimaryScene(_mainMenuScenePathSO);
        }
        finally
        {
            _submitButton.interactable = true;
        }
    }
}
