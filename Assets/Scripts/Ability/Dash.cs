using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Bolt;

public class Dash : Ability
{
    private NetworkRigidbody _networkBody = null;
    [SerializeField]
    private Transform _cam = null;
    private float _dashForce = 40f;
    private float _dashDuration = 1f;
    private bool _dashing = false;

    public void Awake()
    {
        _cooldown = 2;
        _networkBody = GetComponent<NetworkRigidbody>();
        _UI_cooldown = GUI_Controller.Current.Skill;
        _UI_cooldown.InitView(_abilityInterval);
        _cost = 1;
        Debug.Log("Awake in Dash.cs has run");
    }

    public override void UpdateAbility(bool button)
    {
        base.UpdateAbility(button);//front

        if (_buttonDown && _timer + _abilityInterval <= BoltNetwork.ServerFrame && (state.Energy - _cost) >= 0)
        {
            _timer = BoltNetwork.ServerFrame;
            if (entity.HasControl)
                _UI_cooldown.StartCooldown();
            _Dash();
            Debug.Log("you should be dashing right now");
        }

        if (_dashing)
        {
            _networkBody.MoveVelocity = Vector3.Scale(_cam.forward, new Vector3(1, 0, 1)).normalized * _dashForce;
        }

        //Debug.Log("UpdateAbility in Dash.cs is running"); // confirmed to be running
    }

    private void _Dash()
    {
        if (entity.IsOwner)
            state.Energy -= _cost;
        StartCoroutine(Dashing());
    }

    IEnumerator Dashing()
    {
        _dashing = true;
        yield return new WaitForSeconds(_dashDuration);
        _dashing = false;
    }
}