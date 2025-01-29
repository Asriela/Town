using UnityEngine;
public enum AppearanceState
{
    lyingDown,
    standing,
    walking,
    dead
}
public class Appearance : MonoBehaviour
{
    private Character _character;
    public void Initialize(Character character) => _character = character;
    private AppearanceState _lastState;
    public AppearanceState State { get; set; } = AppearanceState.standing;

    [SerializeField]
    private Transform _spriteTransform;

    private SpriteRenderer _spriteRenderer;

    private Vector3 previousPosition;
    private Vector3 localSpritePosition;

    private void Start()
    {
        _spriteRenderer = _spriteTransform.GetComponent<SpriteRenderer>();
        localSpritePosition=_spriteRenderer.transform.localPosition;
    }

    private void Update()
    {
        ChangeAppearanceAccordingToState();
        FlipSpriteBasedOnMovement();
        SortByY();
    }
    private void SortByY()
    {
        _spriteRenderer.sortingOrder = Mathf.RoundToInt(-transform.position.y * 100);
    }
    public void ChangeColor(Color color)
    {
        _spriteRenderer.color = color;
    }
    private void ChangeAppearanceAccordingToState()
    {
        var temp = 0;
        var offset = 0f;
        var currentRotation = _spriteRenderer.transform.localRotation;

        switch (State)
        {
            case AppearanceState.dead:
     
                Sprite deadSprite = Resources.Load<Sprite>("Assets/Sprites/characters/TravelerDead.png");
                if (deadSprite != null)
                {
                    _spriteRenderer.sprite = deadSprite;
                }
                else
                {
                    Debug.LogWarning("Sprite not found at path: Sprites/Characters/TravelerDead");
                }
                _spriteRenderer.gameObject.transform.localRotation = Quaternion.Euler(0, 0, 90);
                break;
            case AppearanceState.lyingDown:

                _spriteRenderer.gameObject.transform.localRotation = Quaternion.Euler(0, 0, 90);
                offset=0.9f;
                break;
            case AppearanceState.standing:

                _spriteRenderer.gameObject.transform.localRotation = Quaternion.Euler(0, 0, 0);
                break;

        }
        _spriteRenderer.transform.localPosition= localSpritePosition+ Vector3.right * offset;
    }
    public void SetSprite(string spriteName)
    {

        _spriteRenderer.sprite = Resources.Load<Sprite>("Sprites/characters/"+spriteName);
        var tem = _spriteRenderer.sprite;
    }
    public void SetSpriteAction(string actionName)
    {

        _spriteRenderer.sprite = Resources.Load<Sprite>($"Sprites/characters/{GetCharacterName(_character)}_" + actionName);
        var tem = _spriteRenderer.sprite;
    }

    public void ResetSprite()
    {

        _spriteRenderer.sprite = Resources.Load<Sprite>($"Sprites/characters/{GetCharacterName(_character)}");
        var tem = _spriteRenderer.sprite;
    }

    private string GetCharacterName(Character character)
    {
        if (character == WorldManager.Instance.ThePlayer)
        {
            return "Traveler";
        }
        else
        { return _character.CharacterName.ToString(); }
    
    }
    void FlipSpriteBasedOnMovement()
    {

        Vector3 movementDirection = transform.position - previousPosition;


        if (movementDirection.x > 0.005f)
        {
            _spriteRenderer.flipX = false; // Facing right
        }

        if (movementDirection.x < -0.005f)
        {
            _spriteRenderer.flipX = true; // Facing left
        }


        previousPosition = transform.position;
    }
    public void FaceLeft()
    {
        _spriteRenderer.flipX = true;
    }
    public void FaceRight()
    {
        _spriteRenderer.flipX = false;
    }
    public void FlipSpriteToTarget(Character subject, Character target)
    {
        // Determine the direction to the target
        bool flip = target.transform.position.x < subject.transform.position.x;

        // Flip the sprite based on the direction
        _spriteRenderer.flipX = flip;
    }
}
