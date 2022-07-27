using System.Collections;
using System.Collections.Generic;
using Photon.Bolt;
using UnityEngine;

public class Ability : EntityEventListener<IPlayerState>
{
    protected bool _pressed = false;
    protected bool _buttonUp;
    protected bool _buttonDown;

    protected int _cooldown = 0;
    protected float _timer = 0f;
    protected int _cost = 0;
    protected UI_Cooldown _UI_cooldown;

    protected int _abilityInterval
    {
        get { return _cooldown * BoltNetwork.FramesPerSecond; }
    }

    public virtual void UpdateAbility(bool button)
    {
        _buttonUp = false;
        _buttonDown = false;
        if(button)
        {
            if(_pressed == false) // im such a goober
            {
                _pressed = true;
                _buttonDown = true;
                /*
                Debug.Log("_buttonDown is: ");
                Debug.Log(_buttonDown);
                Debug.Log("_buttonUp is: ");
                Debug.Log(_buttonUp);
                Debug.Log("button is: ");
                Debug.Log(button);
                Debug.Log("_pressed is: ");
                Debug.Log(_pressed);
                */
            }
        }
        else
        {
            if (_pressed)
            {
                _pressed = false;
                _buttonUp = true;
                /*
                Debug.Log("_buttonDown is: ");
                Debug.Log(_buttonDown);
                Debug.Log("_buttonUp is: ");
                Debug.Log(_buttonUp);
                Debug.Log("button is: ");
                Debug.Log(button);
                Debug.Log("_pressed is: ");
                Debug.Log(_pressed);
                */
            }

        }
        //Debug.Log("UpdateAbility in Ability.cs has run"); //confirmed to be running
    }

    public virtual void ShowVisualEffect() { }
}
