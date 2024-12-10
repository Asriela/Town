using UnityEngine;
using UnityEngine.AI;

public class Movement : MonoBehaviour
{
    protected NavMeshAgent agent;
    private Vector3 _currentTarget;

    public Mind.LocationName CurrentLocation { get; set; }

    protected virtual void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        agent.updateRotation = false;
        agent.updateUpAxis = false;
    }

    public void MoveTo(Vector3 targetPosition)
    {
        agent.SetDestination(targetPosition);
    }
    public void MoveToRandomPoints()
    {

        if (agent.pathPending || agent.remainingDistance > agent.stoppingDistance)
            return;

        float randomRange = 10f;
        // Generate a random point around the NPC's current position
        Vector3 randomDirection = Random.insideUnitSphere * randomRange;
        randomDirection += transform.position;

        // Ensure the random point is on the NavMesh
        if (NavMesh.SamplePosition(randomDirection, out NavMeshHit hit, randomRange, NavMesh.AllAreas))
        {
            _currentTarget = hit.position; // Store the current target
            agent.SetDestination(_currentTarget);
        }
        else
        {
            Debug.LogWarning("Failed to find a valid NavMesh point. Retrying...");
        }
    }

    public Vector3 GetMovementDirection() => agent.velocity.normalized;  // Returns the normalized movement direction

    public Vector3 GetPosition() => transform.position;
}



