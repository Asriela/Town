using UnityEngine;

public class RentFeature : MonoBehaviour, IRentable
{
    private WorldObject _worldObject;

    public float RentHoursLeft { get; set; } = 0;
    public Character RentedBy { get; set; } = null;
    public Location RentedFrom { get; set; } = null;

    // Constructor to inject the parent WorldObject
    public void Setup(WorldObject worldObject) => _worldObject = worldObject;



    public void StartRenting(Character character, Location location, float hours)
    {
        RentedFrom = location;
        RentedBy = character;
        RentHoursLeft = hours;
    }

    public void UpdateRent()
    {
        if (RentedBy == null)
        {
            RentHoursLeft = 0;
            return;
        }

        RentHoursLeft -= WorldManager.Instance.TimeThatsChanged;
        if (RentHoursLeft <= 0)
        {
            RentedBy.Memory.RemoveObjectFromPossessions(_worldObject);
            RentedFrom.AddPosession(_worldObject.ObjectType, _worldObject);
            RentedBy = null;
        }
    }
}
