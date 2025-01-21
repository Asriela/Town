using UnityEngine;

public class GrowFeature : MonoBehaviour, IGrowable
{
    private WorldObject _worldObject;






     public void Setup(WorldObject worldObject) => _worldObject = worldObject;



    public void Grow()
    {
        if (_worldObject.Maintenance > 40)
        {
            _worldObject.Integrity += 1;
            _worldObject.Size += 1;
        }
    }


}
