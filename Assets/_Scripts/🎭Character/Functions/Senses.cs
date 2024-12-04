using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Senses : MonoBehaviour
{
    private NPC _npc;

    [Header("Senses Settings")]
    [SerializeField] private float _detectionRadius = 10f;  // How far NPC can see
    [SerializeField] private float _viewAngle = 100f;  // The angle of view in degrees
    [SerializeField] private LayerMask _detectionLayer;  // What layers the NPC can detect (e.g., NPCs)

    private List<Character> _charactersInSight = new List<Character>();

    public void Initialize(NPC npc)
    {
        _npc = npc;

    }

    public void Start()
    {
        StartCoroutine(DetectCharactersInSight());
    }

    private IEnumerator DetectCharactersInSight()
    {
        while (true)
        {
            _charactersInSight.Clear(); // Clear the previous list

            // Use OverlapCircle for 2D to detect colliders within a radius
            Collider2D[] collidersInRange = Physics2D.OverlapCircleAll(transform.position, _detectionRadius, _detectionLayer);

            foreach (var collider in collidersInRange)
            {
                if (collider == GetComponent<Collider2D>())
                    continue;

                if (collider.CompareTag("Character"))  // Ensure it's a character
                {
                    // Check for NPC or Player specifically
                    if (collider.TryGetComponent(out NPC potentialNPC))
                    {
                        Vector2 directionToTarget = (potentialNPC.transform.position - transform.position).normalized;
                        float angle = Vector2.Angle(transform.right, directionToTarget);  // Use right for 2D field of view

                        if (angle <= _viewAngle / 2)
                        {
                            // Check if there's no obstruction between NPC and target
                            if (IsInLineOfSight(potentialNPC))
                            {
                                print("sees NPC");
                                _charactersInSight.Add(potentialNPC);
                            }
                        }
                    }
                    else if (collider.TryGetComponent(out Player potentialPlayer))
                    {
                        Vector2 directionToTarget = (potentialPlayer.transform.position - transform.position).normalized;
                        float angle = Vector2.Angle(transform.right, directionToTarget);  // Use right for 2D field of view

                        if (angle <= _viewAngle / 2)
                        {
                            // Check if there's no obstruction between NPC and target
                            if (IsInLineOfSight(potentialPlayer))
                            {
                                print("sees Player");
                                _charactersInSight.Add(potentialPlayer);
                                potentialPlayer.SetSeenState(true);
                            }
                        }
                    }
                }
            }

            yield return new WaitForSeconds(0.5f);  // Adjust the check interval if needed
        }
    }

    private bool IsInLineOfSight(Character character)
    {
        var targetPosition = character.transform.position;
        // Cast a ray towards the target
        RaycastHit2D[] hits = Physics2D.RaycastAll(transform.position, targetPosition - transform.position, _detectionRadius);

        // Loop through all hits
        foreach (var hit in hits)
        {

            if (hit.collider.CompareTag("Wall"))
            {
                return false;
            }
        }

        return true;
    }

    public GameObject SeeSomeoneWithTraits(params Mind.TraitType[] traits)
    {
        foreach (var character in _charactersInSight)
        {
            return character.gameObject;
        }
        return null;
    }

    // Gizmos for visualizing the field of view in the editor
    private void OnDrawGizmos()
    {
        // Draw the detection radius as a circle
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, _detectionRadius);

        if (_npc != null)
        {
            // Get the movement direction from the Movement class
            Vector3 movementDirection = _npc.GetMovementDirection();

            // Draw the field of view cone (2D)
            Gizmos.color = new Color(1f, 1f, 0f, 0.3f);  // Yellow with transparency
            Vector2 leftBoundary = Quaternion.Euler(0, 0, -_viewAngle / 2) * movementDirection * _detectionRadius;
            Vector2 rightBoundary = Quaternion.Euler(0, 0, _viewAngle / 2) * movementDirection * _detectionRadius;

            Gizmos.DrawLine(transform.position, transform.position + (Vector3)leftBoundary);
            Gizmos.DrawLine(transform.position, transform.position + (Vector3)rightBoundary);

            // Optionally, draw lines between the boundaries to complete the cone
            Gizmos.DrawLine(transform.position + (Vector3)leftBoundary, transform.position + (Vector3)rightBoundary);
        }
    }
}
