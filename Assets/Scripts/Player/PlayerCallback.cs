using UnityEngine;
using Photon.Bolt;

public class PlayerCallback : EntityEventListener<IPlayerState>
{

    private PlayerMotor _playerMotor;
    private PlayerWeapons _playerWeapons;

    private void Awake()
    {
        _playerMotor = GetComponent<PlayerMotor>();
        _playerWeapons = GetComponent<PlayerWeapons>();
    }

    public override void Attached()
    {
        state.AddCallback("LifePoints", UpdatePlayerLife);
        state.AddCallback("Pitch", _playerMotor.SetPitch);

        if(entity.IsOwner)
        {
            state.LifePoints = _playerMotor.TotalLife;
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.UpArrow))
            state.LifePoints += 10;
        if (Input.GetKeyDown(KeyCode.DownArrow))
            state.LifePoints -= 10;
    }

    public void FireEffect(float precision, int seed)
    {
        FireEffectEvent evnt = FireEffectEvent.Create(entity, EntityTargets.EveryoneExceptOwnerAndController);
        evnt.Precision = precision;
        evnt.Seed = seed;
        evnt.Send();
    }

    public void UpdatePlayerLife()
    {
        if(entity.HasControl)
            GUI_Controller.Current.UpdateLife(state.LifePoints, _playerMotor.TotalLife);
    }
}
