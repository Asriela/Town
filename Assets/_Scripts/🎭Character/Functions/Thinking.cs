using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Thinking : MonoBehaviour
{
    [SerializeField]
    private List<Trait> _traits = new();

    private NPC _npc;

    public void Initialize(NPC npc) => _npc = npc;

    public List<Trait> Traits => _traits;

    // Example for controlling behavior check interval
    [SerializeField] private float checkInterval = 3f; // Time in seconds to wait between behavior checks

    private void Start()
    {
        // Start the coroutine to check the best behavior periodically
        StartCoroutine(CheckBestBehaviorCoroutine());
    }

    private IEnumerator CheckBestBehaviorCoroutine()
    {
        while (true)  // This will run indefinitely until the object is destroyed or stopped
        {
            var highestScoringBehavior = FindHighestScoringBehavior();

            if (highestScoringBehavior != null)
            {
                _npc.Acting.CurrentBehavior = highestScoringBehavior;
                print($"Selected Behavior: {highestScoringBehavior.Name}");
            }
            else
            {
                print("No valid behaviour found.");
            }

            // Wait for the specified interval before checking again
            yield return new WaitForSeconds(checkInterval);
        }
    }

    public Behavior FindHighestScoringBehavior()
    {
        Behavior highestScoringBehavior = null;
        int highestScore = 0;

        // Iterate through each trait
        for (int i = 0; i < _traits.Count; i++)
        {
            var trait = _traits[i];
            print($"inspecting trait: {trait.name}");

            // Iterate through each behavior of the trait
            for (int j = 0; j < trait.Behaviors.Count; j++)
            {

                var behavior = trait.Behaviors[j];
                print($"inspecting behaviour: {behavior.Name}");

                // Check if all conditions are met for the behavior
                print($"are conditions met: {AreConditionsMet(behavior.Conditions)}");
                if (AreConditionsMet(behavior.Conditions))
                {
                    // Calculate score
                    int score = behavior.Priority + j + 1 + i + 1;

                    // Update if this behavior has a higher score
                    if (score > highestScore)
                    {
                        highestScore = score;
                        highestScoringBehavior = behavior;
                    }
                }
            }
        }

        return highestScoringBehavior;
    }

    // Check if all conditions for a behavior are met
    private bool AreConditionsMet(List<Condition> conditions)
    {
        foreach (var condition in conditions)
        {
            if (!Conditions.CheckCondition(condition, _npc)) // Static call to Conditions.CheckCondition
            {
                return false;
            }
        }
        return true;
    }
}
