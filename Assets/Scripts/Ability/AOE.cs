using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Bolt;

public class AOE : MonoBehaviour
{
    private List<PlayerMotor> _playersInside = new List<PlayerMotor>();
    [SerializeField]
    private int _dmg = 0;
    private float _time = 0.2f;
    private float _lastCycle = 0.2f;

    private float _interval
    {
        get => _time * BoltNetwork.FramesPerSecond;
    }

    private PlayerMotor _launcher = null;

    public PlayerMotor launcher { set => _launcher = value; }

    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<PlayerMotor>())
            _playersInside.Add(other.GetComponent<PlayerMotor>());
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.GetComponent<PlayerMotor>())
            _playersInside.Remove(other.GetComponent<PlayerMotor>());
    }

    private void FixedUpdate()
    {
        if (_launcher != null)
        {
            if (_lastCycle + _interval <= BoltNetwork.ServerFrame)
            {
                for(int i = 0; i < _playersInside.Count; i++)
                {
                    if (_playersInside[i] == null)
                        _playersInside.RemoveAt(i);
                }
                _playersInside.TrimExcess();

                _lastCycle = BoltNetwork.ServerFrame;
                foreach (PlayerMotor player in _playersInside)
                {
                    if (player)
                        player.Life(_launcher, -_dmg);
                }
            }
        }
    }
}
