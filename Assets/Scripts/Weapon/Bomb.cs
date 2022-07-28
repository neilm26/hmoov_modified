using UnityEngine;
using Photon.Bolt;

public class Bomb : Weapon
{


    private GameController _gameController = null;

    public override void Init(PlayerWeapons pw, int index) {
        base.Init(pw, index);
                
        if (_gameController == null) {
            _gameController = FindObjectOfType<GameController>();
        }
    }
    private void OnEnable()
    {
        if(_playerMotor)
        {
            if (_playerMotor.entity.HasControl) {
                GUI_Controller.Current.HideAmmo();
            }
        }
    }

    protected override void _Fire(int seed) {

    }
}
