using Mind;
using UnityEngine;

public class UpkeepFeature : MonoBehaviour, IUpkeepable
{
    private WorldObject _worldObject;





    public void Setup(WorldObject worldObject) => _worldObject = worldObject;







    public void Degenerate()
    {
        if (_worldObject.Maintenance < 10)
        {
            _worldObject.Integrity -= 1;
        }
    }

    public void Maintain(Character userOfObject)
    {
        foreach (var trait in _worldObject.ObjectTraits)
        {
            switch (trait)
            {
                case ObjectTrait.crop:
                    _worldObject.Maintenance += 100;
                    break;
            }
        }
    }
}
