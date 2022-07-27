using Photon.Bolt;
using UnityEngine;

[BoltGlobalBehaviour]

public class NetworkCallbacks : GlobalEventListener
{
    public override void BoltStartBegin()
    {
        BoltNetwork.RegisterTokenClass<PhotonRoomProperties>();
        BoltNetwork.RegisterTokenClass<WeaponDropToken>();
        BoltNetwork.RegisterTokenClass<PlayerToken>();
    }

    public override void SceneLoadLocalDone(string scene, IProtocolToken token)
    {
        if (BoltNetwork.IsServer)
        {
            if (scene == HeadlessServerManager.Map())
            {
                if (!GameController.Current)
                    BoltNetwork.Instantiate(BoltPrefabs.GameController);
            }
        }
    }
}
