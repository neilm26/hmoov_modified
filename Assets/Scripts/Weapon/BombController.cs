using Photon.Bolt;
using UnityEngine;

public class BombController : EntityBehaviour<IBombState>
{

    static BombController instance = null;
    private static float _MAX_DISTANCE = 2f;
    private BoltEntity _defuser = null;
    public static bool _IS_DEFUSED = false;

    public override void Attached()
    {
        state.SetTransforms(state.Transform, transform);
        instance = this;
    }

    public static bool CheckDefuse(Vector3 Player) {
        if (!instance) 
            return false;
        if (instance._defuser != null  || _IS_DEFUSED) 
            return false;
        return Vector3.Distance(Player, instance.transform.position) < _MAX_DISTANCE;
    }

    public static void SetDefuser(BoltEntity be) {
        
        if (!instance) {

            instance._defuser = be;
        }
    }
}
