using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Senses : MonoBehaviour
{
    private NPC _npc;

    [Header("Senses Settings")]
    [SerializeField] private float _detectionRadius = 10f;  // How far NPC can see
    [SerializeField] private float _viewAngle = 180;  // The angle of view in degrees
    [SerializeField] private LayerMask _detectionLayer;  // What layers the NPC can detect (e.g., NPCs)
    [SerializeField] private Material _viewConeMaterial; // Material for the cone

    private Mesh _viewConeMesh;
    private MeshRenderer _viewConeRenderer;
    private List<Character> _charactersInSight = new List<Character>();

    public void Initialize(NPC npc)
    {
        _npc = npc;
    }

    public void Start()
    {
        CreateViewCone();
        StartCoroutine(DetectCharactersInSight());
    }

    private void Update()
    {
        DrawViewCone();
    }

    private void CreateViewCone()
    {
        // Create a GameObject for the cone
        GameObject coneObject = new GameObject("ViewCone");
        coneObject.transform.parent = transform;
        coneObject.transform.localPosition = Vector3.zero;

        // Add components for the cone
        _viewConeMesh = new Mesh();
        MeshFilter meshFilter = coneObject.AddComponent<MeshFilter>();
        _viewConeRenderer = coneObject.AddComponent<MeshRenderer>();

        meshFilter.mesh = _viewConeMesh;
        _viewConeRenderer.material = _viewConeMaterial;
    }

    private void DrawViewCone()
    {
        int segments = 50; // Number of segments for the cone
        float angleStep = _viewAngle / segments;
        Vector3[] vertices = new Vector3[segments + 2];
        int[] triangles = new int[segments * 3];

        // Get the movement direction from the NPC 
        Vector3 movementDirection = _npc.GetMovementDirection().normalized;

        vertices[0] = Vector3.zero; // Cone origin
        for (int i = 0; i <= segments; i++)
        {
            float angle = -_viewAngle / 2 + angleStep * i;
            Vector3 direction = Quaternion.Euler(0, 0, angle) * movementDirection;
            vertices[i + 1] = direction * _detectionRadius;
        }

        for (int i = 0; i < segments; i++)
        {
            triangles[i * 3] = 0;
            triangles[i * 3 + 1] = i + 1;
            triangles[i * 3 + 2] = i + 2;
        }

        _viewConeMesh.Clear();
        _viewConeMesh.vertices = vertices;
        _viewConeMesh.triangles = triangles;
        _viewConeMesh.RecalculateNormals();
    }

    private IEnumerator DetectCharactersInSight()
    {
        while (true)
        {
            // First, reset the 'unseen' state for characters that are no longer in sight
            /*foreach (var character in _charactersInSight.ToArray())
            {
                if (character is Player player)
                {
                    // Check if player is no longer in the view cone
                    if (!IsInLineOfSight(player))
                    {
                        player.SetSeenState(false); // Set player to unseen state
                        _charactersInSight.Remove(character); // Remove from sight list
                    }
                }
            }
            */

            // Clear the previous list and detect all new characters
            Collider2D[] collidersInRange = Physics2D.OverlapCircleAll(transform.position, _detectionRadius, _detectionLayer);

            foreach (var collider in collidersInRange)
            {
                if (collider == GetComponent<Collider2D>())
                    continue;

                if (collider.CompareTag("Character"))
                {
                    if (collider.TryGetComponent(out NPC potentialNPC))
                    {
                        Vector2 directionToTarget = (potentialNPC.transform.position - transform.position).normalized;
                        float angle = Vector2.Angle(transform.right, directionToTarget);

                        if (angle <= _viewAngle && IsInLineOfSight(potentialNPC))
                        {
                            if (!_charactersInSight.Contains(potentialNPC))
                            {
                                print("sees NPC");
                                _charactersInSight.Add(potentialNPC);
                            }
                        }
                    }
                    else if (collider.TryGetComponent(out Player potentialPlayer))
                    {
                        Vector2 directionToTarget = (potentialPlayer.transform.position - transform.position).normalized;
                        float angle = Vector2.Angle(transform.right, directionToTarget);

                        // If player is inside the view cone angle, immediately check line of sight
                        if (angle <= _viewAngle  && IsInLineOfSight(potentialPlayer))
                        {
                            if (!_charactersInSight.Contains(potentialPlayer))
                            {
                                print("sees Player");
                                _charactersInSight.Add(potentialPlayer);
                                //potentialPlayer.SetSeenState(true); // Set player to seen state
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
        // Offset the ray start position slightly to avoid hitting the NPC's own collider
        Vector2 rayStartPosition = (Vector2)transform.position + new Vector2(0, 2f); // Example offset along the y-axis

        // Cast a ray towards the target
        RaycastHit2D hit = Physics2D.Raycast(rayStartPosition, targetPosition - (Vector3)rayStartPosition, _detectionRadius, _detectionLayer);
        Debug.DrawRay(rayStartPosition, targetPosition - (Vector3)rayStartPosition, Color.red);

        // Check if the first hit is the character and not the NPC itself
        if (hit.collider != null && hit.collider == character.GetComponent<Collider2D>())
        {
            // Check if there are no walls between the NPC and the player
            return true;
        }



        // If no hits or a non-character object was hit, return false
        return false;
    }

    /* private bool IsInLineOfSight(Character character)
     {
         var targetPosition = character.transform.position;
         // Cast a ray towards the target
         RaycastHit2D[] hits = Physics2D.RaycastAll(transform.position, targetPosition - transform.position, _detectionRadius, _detectionLayer);

         float closestWall = Mathf.Infinity;
         float characterDistance = Mathf.Infinity;

         bool playerDetected = false;

         // Loop through all the hits
         foreach (var hit in hits)
         {
             // Check if the hit is the character
             if (hit.collider == character.GetComponent<Collider2D>() && hit.collider != _npc.GetComponent<Collider2D>())
             {
                 playerDetected = true;
                 characterDistance = Vector2.Distance(transform.position, targetPosition);
             }

             // Check if the hit is a wall
             if (hit.collider.CompareTag("Wall"))
             {
                 closestWall = Vector2.Distance(transform.position, hit.transform.position);
             }
         }

         // If the player is detected and not blocked by a wall, return true
         if (playerDetected && closestWall > characterDistance)
         {
             return true;
         }

         // If a wall is closer than the player, or the player is outside the detection radius, return false
         if (characterDistance > _detectionRadius || closestWall < characterDistance)
         {
             return false;
         }

         return false;
     }
    */
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
