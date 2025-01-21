using System.Collections.Generic;
using static InteractFeature;

public interface IGrowable
{
    void Grow();

}
public interface IUpkeepable
{

    void Degenerate();
    void Maintain(Character userOfObject);
}
public interface IRentable
{
    void StartRenting(Character character, Location location, float hours);
    void UpdateRent();
}

public interface IUseable
{
    void Use(Character user);
}
public interface IEffetable
{
    void EffectWearer(Character wearer);
}
public interface IInteractable
{
    List<InteractionOption> GetInteractionOptions(Character character);
    void Interact(Character character, ObjectInteractionType interactionType);
}
