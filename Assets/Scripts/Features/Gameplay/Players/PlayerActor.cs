using UnityEngine;

public sealed class PlayerActor : MonoBehaviour, IPlayerActor
{
    [SerializeField] private RigidbodyPlayerMotor _rbPlayerMotor;
    [SerializeField] private PlayerLookController _localLookController;
    [SerializeField] private PlayerProfilePresenter _presenter;
    public PlayerId PlayerId { get; private set; }
    public GameObject RootObject => gameObject;
    private IPlayerMotor _playerMotor;
    private IPlayerLookController _playerLookController;

    public void Initialize(PlayerProfile profile)
    {
        PlayerId = profile.PlayerId;
        _presenter.Apply(profile);
        _playerMotor = _rbPlayerMotor;
        _playerLookController = _localLookController;
    }

    public void Receive(in PlayerCommand cmd)
    {
        _playerLookController.SetLookDirection(cmd.Look);
        _playerMotor.SetMovementIntent(cmd.MoveDir, cmd.Look.YawDegrees);

        if (cmd.JumpPressed) _playerMotor.RequestJump();
    }
}
