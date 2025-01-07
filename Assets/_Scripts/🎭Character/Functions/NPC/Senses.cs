using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditorInternal;
using UnityEngine;

public class Senses : MonoBehaviour
{
    private NPC _npc;

    [Header("Senses Settings")]
    [SerializeField] private float _detectionRadius = 10f;
    [SerializeField] private float _viewAngle = 100;
    [SerializeField] private LayerMask _characterDetectionLayer;
    [SerializeField] private LayerMask _objectDetectionLayer;
    [SerializeField] private Material _viewConeMaterial;
    private Vector3 _lookDirection;

    private Mesh _viewConeMesh;

    private MeshRenderer _viewConeRenderer;
    private List<Character> _charactersInSight = new();
    private List<WorldObject> _objectsInSight = new();

    public void Initialize(NPC npc)
    {
        _npc = npc;
        StartCoroutine(DetectCharactersInSight());
        StartCoroutine(DetectObjectsInSight());

        CreateViewCone();
    }



    private void Update()
    {
        DrawViewCone();
        SetViewDirection();
        SetLoggersCharactersInSight();
    }

    private void SetLoggersCharactersInSight()
    {
        _npc.Ui.CharactersInSight = "";
        foreach (var character in _charactersInSight)
        {
            _npc.Ui.CharactersInSight += @$"{character.CharacterName}\n";
        }
    }
    private void SetViewDirection() => _lookDirection = _npc.Movement.GetMovementDirection().normalized;

    private void CreateViewCone()
    {

        GameObject coneObject = new("ViewCone");
        coneObject.transform.parent = transform;
        coneObject.transform.localPosition = Vector3.zero;


        _viewConeMesh = new Mesh();
        var meshFilter = coneObject.AddComponent<MeshFilter>();
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


        var movementDirection = _lookDirection;

        vertices[0] = Vector3.zero;
        for (int i = 0; i <= segments; i++)
        {
            float angle = (-_viewAngle / 2) + (angleStep * i);
            var direction = Quaternion.Euler(0, 0, angle) * movementDirection;
            vertices[i + 1] = direction * _detectionRadius;
        }

        for (int i = 0; i < segments; i++)
        {
            triangles[i * 3] = 0;
            triangles[(i * 3) + 1] = i + 1;
            triangles[(i * 3) + 2] = i + 2;
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

            List<Character> currentCharactersInSight = new();

            var collidersInRange = Physics2D.OverlapCircleAll(transform.position, _detectionRadius, _characterDetectionLayer);

            foreach (var collider in collidersInRange)
            {
                if (collider == GetComponent<Collider2D>())
                { continue; }

                if (collider.CompareTag("Character"))
                {
                    if (collider.TryGetComponent(out NPC potentialNPC))
                    {
                        if (CharacterIsInLineOfSight((Character)potentialNPC))
                        {
                            currentCharactersInSight.Add(potentialNPC);
                        }
                    }
                    else if (collider.TryGetComponent(out Player potentialPlayer))
                    {
                        if (CharacterIsInLineOfSight((Character)potentialPlayer))
                        {
                            currentCharactersInSight.Add(potentialPlayer);
                            potentialPlayer.SetSeenState(true);
                        }
                    }
                }
            }

            foreach (var character in _charactersInSight)
            {
                if (!currentCharactersInSight.Contains(character))
                {
                    if (character is Player player)
                    {
                        player.SetSeenState(false);
                    }
                }
            }

            _charactersInSight = currentCharactersInSight;

            yield return new WaitForSeconds(0.5f);
        }
    }


    private IEnumerator DetectObjectsInSight()
    {

        while (true)
        {




            var collidersInRange = Physics2D.OverlapCircleAll(transform.position, _detectionRadius, _objectDetectionLayer);

            foreach (var collider in collidersInRange)
            {
                if (collider == GetComponent<Collider2D>())
                { continue; }


                if (collider.TryGetComponent(out WorldObject potentialObject))
                {


                    if (ObjectIsInLineOfSight(potentialObject))
                    {
                        if (!_objectsInSight.Contains(potentialObject))
                        {
                            _objectsInSight.Add(potentialObject);
                        }
                    }
                }

            }

            yield return new WaitForSeconds(0.5f);
        }
    }

    private bool CharacterIsInLineOfSight(Character character)
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
        var raycastHit2D = Physics2D.Raycast(ourPosition, dirToTarget, _detectionRadius, _characterDetectionLayer);

        _npc.gameObject.layer = oldLayer;

        if (raycastHit2D.collider != null && raycastHit2D.collider.gameObject == character.gameObject)
        { return true; }

        return false;
    }

    private bool ObjectIsInLineOfSight(WorldObject worldObject)
    {

        var ourPosition = _npc.Movement.GetPosition();
        var targetPosition = worldObject.GetPosition();
        if (Vector3.Distance(ourPosition, targetPosition) > _detectionRadius)
        { return false; }

        var dirToTarget = (targetPosition - ourPosition).normalized;

        if (Vector3.Angle(_lookDirection, dirToTarget) > _viewAngle / 2f)
        { return false; }

        var raycastHit2D = Physics2D.Raycast(ourPosition, dirToTarget, _detectionRadius, _objectDetectionLayer);

        if (raycastHit2D.collider != null && raycastHit2D.collider.gameObject == worldObject.gameObject)
        { return true; }

        return false;
    }

    //TODO: add looking for object with traits, its not required yet in this demo so i didnt add it
    public GameObject SeeSomeoneWithTraits(params Mind.TraitType[] traits)
    {
        foreach (var character in _charactersInSight)
        {
            return character.gameObject;
        }
        return null;
    }

    public WorldObject SeeObjectOfType(Mind.ObjectType objectType)
    {
        var value =
        _objectsInSight.FirstOrDefault(anObject => anObject.ObjectType == objectType);
        return value;
    }




}
