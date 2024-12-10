using System.Collections;
using System.Collections.Generic;
using UnityEditorInternal;
using UnityEngine;

public class Senses : MonoBehaviour
{
    private NPC _npc;

    [Header("Senses Settings")]
    [SerializeField] private float _detectionRadius = 10f;
    [SerializeField] private float _viewAngle = 100;
    [SerializeField] private LayerMask _detectionLayer;
    [SerializeField] private Material _viewConeMaterial;
    private Vector3 _lookDirection;

    private Mesh _viewConeMesh;

    private MeshRenderer _viewConeRenderer;
    private List<Character> _charactersInSight = new List<Character>();

    public void Initialize(NPC npc)
    {
        _npc = npc;
        StartCoroutine(DetectCharactersInSight());
        CreateViewCone();
    }



    private void Update()
    {
        DrawViewCone();
        SetViewDirection();
    }

    private void SetViewDirection()
    {
        _lookDirection = _npc.Movement.GetMovementDirection().normalized;
    }

    private void CreateViewCone()
    {

        GameObject coneObject = new GameObject("ViewCone");
        coneObject.transform.parent = transform;
        coneObject.transform.localPosition = Vector3.zero;


        _viewConeMesh = new Mesh();
        MeshFilter meshFilter = coneObject.AddComponent<MeshFilter>();
        _viewConeRenderer = coneObject.AddComponent<MeshRenderer>();

        meshFilter.mesh = _viewConeMesh;
        _viewConeRenderer.material = _viewConeMaterial;
    }

    private void DrawViewCone()
    {
        int segments = 50;
        float angleStep = _viewAngle / segments;
        Vector3[] vertices = new Vector3[segments + 2];
        int[] triangles = new int[segments * 3];


        Vector3 movementDirection = _lookDirection;

        vertices[0] = Vector3.zero;
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

            foreach (var character in _charactersInSight.ToArray())
            {
                if (character is Player player)
                {


                    player.SetSeenState(false);
                    _charactersInSight.Remove(character);

                }
            }



            Collider2D[] collidersInRange = Physics2D.OverlapCircleAll(transform.position, _detectionRadius, _detectionLayer);

            foreach (var collider in collidersInRange)
            {
                if (collider == GetComponent<Collider2D>())
                    continue;

                if (collider.CompareTag("Character"))
                {
                    if (collider.TryGetComponent(out NPC potentialNPC))
                    {


                        if (IsInLineOfSight((Character)potentialNPC))
                        {
                            if (!_charactersInSight.Contains(potentialNPC))
                            {
                                //  print("sees NPC");
                                _charactersInSight.Add(potentialNPC);
                            }
                        }
                    }
                    else if (collider.TryGetComponent(out Player potentialPlayer))
                    {


                        if (IsInLineOfSight((Character)potentialPlayer))
                        {
                            if (!_charactersInSight.Contains(potentialPlayer))
                            {
                                // print("sees Player");
                                _charactersInSight.Add(potentialPlayer);
                                potentialPlayer.SetSeenState(true);
                            }
                        }
                    }
                }
            }

            yield return new WaitForSeconds(0.5f);
        }
    }
    private bool IsInLineOfSight(Character character)
    {

        var ourPosition = _npc.Movement.GetPosition();
        var targetPosition = character.Movement.GetPosition();
        if (Vector3.Distance(ourPosition, targetPosition) > _detectionRadius)
        { return false; }

        var dirToTarget = (targetPosition - ourPosition).normalized;

        if (Vector3.Angle(_lookDirection, dirToTarget) > _viewAngle / 2f)
        { return false; }

        int oldLayer = _npc.gameObject.layer;
        _npc.gameObject.layer = Physics.IgnoreRaycastLayer;
        var raycastHit2D = Physics2D.Raycast(ourPosition, dirToTarget, _detectionRadius, _detectionLayer);

        _npc.gameObject.layer = oldLayer;


        if (raycastHit2D.collider != null && raycastHit2D.collider.gameObject == character.gameObject)
        { return true; }


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
            Vector3 movementDirection = _npc.Movement.GetMovementDirection();

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
