using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI_Timer : MonoBehaviour
{
    UnityEngine.UI.Text _text;
    float _nextTime = 0;

    private void Start()
    {
        _text = transform.GetChild(0).GetComponent<UnityEngine.UI.Text>();
    }

    public void Init(float timer)
    {
        _nextTime = Time.time + timer;
    }

    private void Update()
    {
        if (_nextTime > Time.time)
        {
            _text.text = UI_Cooldown.FloatToTime(-(Time.time - _nextTime), "#0:00");
        }
    }
}
