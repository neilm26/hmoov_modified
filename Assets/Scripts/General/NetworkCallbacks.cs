using Photon.Bolt;
using UnityEngine;

[BoltGlobalBehaviour]

public class NetworkCallbacks : GlobalEventListener
{
    public override void BoltStartBegin()
    {
        BoltNetwork.RegisterTokenClass<PhotonRoomProperties>();
        BoltNetwork.RegisterTokenClass<WeaponDropToken>();
    }
}
