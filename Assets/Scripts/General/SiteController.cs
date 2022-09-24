using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SiteController : MonoBehaviour
{
    List<PlayerWeapons> _playersIn = new List<PlayerWeapons>();
    private bool _isPlayerIn = false;

    public bool IsPlayerIn { get => _isPlayerIn; }

    //bitch ass name
    void OnTriggerEnter(Collider col)
    {
        if (col.GetComponent<PlayerWeapons>())
        {
            if (!_playersIn.Contains(col.GetComponent<PlayerWeapons>()) && (col.gameObject.CompareTag("Player") || col.gameObject.CompareTag("LocalPlayer")))
            {
                _playersIn.Add(col.GetComponent<PlayerWeapons>());
            }
        }
    }

    void OnTriggerExit(Collider col)
    {
        if (col.GetComponent<PlayerWeapons>())
        {
            if (_playersIn.Contains(col.GetComponent<PlayerWeapons>()))
            {
                _playersIn.Remove(col.GetComponent<PlayerWeapons>());
            }
        }
    }

    void FixedUpdate()
    {
        //Debug.Log("FixedUpdate() in SiteController.cs is running"); //confirmed to be running
        _isPlayerIn = false;
        Debug.Log(_playersIn);
        foreach (PlayerWeapons pw in _playersIn) // this is not running??
        {
            if (pw.HasBomb)
            {
                _isPlayerIn = true;
                Debug.Log(pw);
                Debug.Log("_isPlayerIn = true");
            }
            Debug.Log("_isPlayerIn = false");
        }
    }

    public void RoundReset()
    {
        _playersIn = new List<PlayerWeapons>();
    }
}
