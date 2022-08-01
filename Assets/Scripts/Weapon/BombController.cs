using Photon.Bolt;
using UnityEngine;

public class BombController : EntityBehaviour<IBombState>
{

    static BombController instance = null;
    private static float _MAX_DISTANCE = 3f;
    private BoltEntity _diffuser = null;
    public static bool _IS_DIFFUSED = false;

    public override void Attached()
    {
        state.SetTransforms(state.Transform, transform);
        instance = this;
    }

    public static bool CheckDiffuse(Vector3 Player) {
        if (!instance) {
            return false;
        }
        if (instance._diffuser != null  || _IS_DIFFUSED) {
            return false;
        }

        return Vector3.Distance(Player, instance.transform.position) < _MAX_DISTANCE;
    }

    public static void SetDiffuser(BoltEntity be) {
        
        if (!instance) {

            instance._diffuser = be;
        }
    }
}
