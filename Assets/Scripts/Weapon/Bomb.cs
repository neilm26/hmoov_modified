using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Bolt;

public class Bomb : Weapon
{
    private GameController _gameController = null;

    private float _plantingTime = 7.5f;
    private bool _pressed = false;
    private float _plantedTime = 0;

    public override void Init(PlayerWeapons pw, int index) 
    {
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

    private void OnDisable()
    {
        _pressed = false;
        _plantedTime = 0;
        _playerMotor.IsPlanting = false;
        GUI_Controller.Current.PlantingProgressShow(false);
        GUI_Controller.Current.Show(true);
    }

    protected override void _Fire(int seed) 
    {

    }

    public override void ExecuteCommand(bool fire, bool aiming, bool reload, int seed)
    {
        if (_gameController.IsInSite)
        {
            if (fire)
            {
                if (_pressed == false)
                {
                    _playerMotor.IsPlanting = true;
                    _pressed = true;
                    _plantedTime = BoltNetwork.ServerTime + _plantingTime;
                    GUI_Controller.Current.PlantingProgressShow(true);
                } 

                if (_plantingTime < BoltNetwork.ServerTime && _plantedTime != 0)
                {
                    if (_playerWeapons.entity.IsOwner)
                    {
                        BoltNetwork.Instantiate(BoltPrefabs.BombGoal, transform.position, Quaternion.identity);
                        _gameController.Planted();
                        _playerWeapons.RemoveBomb();
                    }
                    _playerMotor.IsPlanting = false;
                    GUI_Controller.Current.PlantingProgressShow(false);
                }

                if (_playerWeapons.entity.HasControl)
                {
                    GUI_Controller.Current.PlantingProgress(1 + ((BoltNetwork.ServerTime - _plantedTime) / _plantingTime));
                }

            }
            else
            {
                if (_pressed)
                {
                    _playerMotor.IsPlanting = false;
                    _pressed = false;
                    _plantedTime = 0;
                    GUI_Controller.Current.PlantingProgressShow(false);
                }
            }
        }
        else
        {
            _plantedTime = 0;
            GUI_Controller.Current.PlantingProgressShow(false);
        }
    }
}
