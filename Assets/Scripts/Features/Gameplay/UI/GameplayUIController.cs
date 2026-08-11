using System;
using TMPro;
using UnityEngine;

public sealed class GameplayUIController : MonoBehaviour
{
    [SerializeField] private TMP_Text _winnerIdText;
    [SerializeField] private TMP_Text _timerText;
    [SerializeField] private TMP_Text _displayNameText;
    [SerializeField] private TMP_Text _playerIdText;
    [SerializeField] private GameObject _postGamePanel;
    private IPlayerProfileContext _playerProfileContext;
    private IMatchResultSource _matchResultSource;
    private IPostMatchFlowContext _postMatchContext;
    private float _timer;

    private void Update()
    {
        if (_timer <= 0f) return;

        _timer = Mathf.Max(0f, _timer -= Time.unscaledDeltaTime);
        _timerText.text = Mathf.CeilToInt(_timer).ToString();
    }

    private void OnDestroy()
    {
        Unbind();
    }

    public void Bind(IPlayerProfileContext playerProfileContext, IMatchResultSource matchResults, IPostMatchFlowContext postMatchContext)
    {
        Unbind();

        _matchResultSource = matchResults ?? throw new ArgumentNullException(nameof(matchResults));
        _postMatchContext = postMatchContext ?? throw new ArgumentNullException(nameof(postMatchContext));
        _playerProfileContext = playerProfileContext ?? throw new ArgumentNullException(nameof(playerProfileContext));
        _matchResultSource.MatchCompleted += SetWinnerText;
        _postMatchContext.PostMatchFlowSource.PostMatchStarted += HandlePostMatch;
        _displayNameText.text = _playerProfileContext.CurrentProfile.DisplayName;
        _playerIdText.text = _playerProfileContext.CurrentProfile.PlayerId.Guid.ToString();
    }

    public void Unbind()
    {
        if (_matchResultSource != null) _matchResultSource.MatchCompleted -= SetWinnerText;

        if (_postMatchContext != null) _postMatchContext.PostMatchFlowSource.PostMatchStarted -= HandlePostMatch;

        _matchResultSource = null;
        _postMatchContext = null;
        _playerProfileContext = null;
        _postGamePanel.SetActive(false);

        if (_winnerIdText != null) _winnerIdText.text = "";

        if (_timerText != null) _timerText.text = "";

        if (_displayNameText != null) _displayNameText.text = "";

        if (_playerIdText != null) _playerIdText.text = "";
    }

    private void SetWinnerText(MatchResult matchResult)
    {
        _winnerIdText.text = matchResult.WinnerId.Guid.ToString();
    }

    private void HandlePostMatch(PostMatchFlowInfo info)
    {
        _postGamePanel.SetActive(true);
        _timer = (float)info.MinimumPostMatchTime.TotalSeconds;
    }
}
