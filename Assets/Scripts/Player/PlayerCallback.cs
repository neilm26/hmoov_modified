using UnityEngine;
using Photon.Bolt;

public class NewBehaviourScript : EntityBehaviour<IPlayerState>
{

    private PlayerMotor _playerMotor;

    private void Awake()
    {
        _playerMotor = GetComponent<PlayerMotor>();
    }

    public override void Attached()
    {
        state.AddCallback("LifePoints", UpdatePlayerLife);

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

    public void UpdatePlayerLife()
    {
        if(entity.HasControl)
            GUI_Ctroller.Current.UpdateLife(state.LifePoints, _playerMotor.TotalLife);
    }
}
