using UnityEngine;
using UnityEngine.AI;

public class Movement : MonoBehaviour
{
    private NavMeshAgent agent;
    public NavMeshAgent Agent  => agent; 
    private Vector3 _currentTarget;

    public Mind.LocationName CurrentLocation { get; set; }

    protected virtual void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        agent.updateRotation = false;
        agent.updateUpAxis = false;
    }

    public void MoveTo(Vector3 targetPosition) => agent.SetDestination(targetPosition);

    public void Stop() => agent.SetDestination(transform.position);

    public void MoveToRandomPoints()
    {

        if (agent.pathPending || agent.remainingDistance > agent.stoppingDistance)
        {
            return;
        }

        float randomRange = 10f;

        Vector3 randomDirection = Random.insideUnitSphere * randomRange;
        randomDirection += transform.position;


        if (NavMesh.SamplePosition(randomDirection, out NavMeshHit hit, randomRange, NavMesh.AllAreas))
        {
            _currentTarget = hit.position;
            agent.SetDestination(_currentTarget);
        }
        else
        {



        }
    }

    public Vector3 GetMovementDirection() => agent.velocity.normalized;  

    public Vector3 GetPosition() => transform.position;
}



