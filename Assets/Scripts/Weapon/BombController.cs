using Photon.Bolt;

public class BombController : EntityBehaviour<IBombState>
{
    public override void Attached()
    {
        state.SetTransforms(state.Transform, transform);
    }
}
