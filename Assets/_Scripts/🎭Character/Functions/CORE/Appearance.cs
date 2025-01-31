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
    private Animator animator;

    private Vector3 previousPosition;
    private Vector3 localSpritePosition;
    private float waitToIdle = 0;
    string currentAnimationName="";
    string currentSocialAction="";

    private void Start()
    {
        _spriteRenderer = _spriteTransform.GetComponent<SpriteRenderer>();
        animator = _spriteTransform.GetComponent<Animator>();
        localSpritePosition = _spriteRenderer.transform.localPosition;
    }

    private void Update()
    {
        ChangeAppearanceAccordingToState();
        MovementBasedFunctions();

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

          
                    SetAnimation("dead");

            // _spriteRenderer.gameObject.transform.localRotation = Quaternion.Euler(0, 0, 90);
                break;
            case AppearanceState.lyingDown:

                _spriteRenderer.gameObject.transform.localRotation = Quaternion.Euler(0, 0, 90);
                if (_character is Player)
                { offset = 1.5f; }
                else
                { offset = 0.9f; }
                
                break;
            case AppearanceState.standing:

                _spriteRenderer.gameObject.transform.localRotation = Quaternion.Euler(0, 0, 0);
                break;

        }
        _spriteRenderer.transform.localPosition = localSpritePosition + Vector3.right * offset;
    }
    public void SetSprite(string spriteName)
    {

        _spriteRenderer.sprite = Resources.Load<Sprite>("Sprites/characters/" + spriteName);
        var tem = _spriteRenderer.sprite;
    }
    public void SetSpriteAction(string actionName)
    {
        SetAnimation(actionName);
        currentSocialAction= actionName;
    }

    public void ResetSprite()
    {

        currentSocialAction="";
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
    void MovementBasedFunctions()
    {
        Vector3 movementDirection = transform.position - previousPosition;
        FlipSpriteBasedOnMovement(movementDirection);
        WalkAnimation(movementDirection);

        previousPosition = transform.position;
    }
    void SetAnimation(string name)
    {

        var path= $"Animations/Characters/{GetCharacterName(_character)}_{name}";

        if (name=="dead")
        {
           var testc="";
        }
        animator.runtimeAnimatorController = Resources.Load<RuntimeAnimatorController>(path);
        currentAnimationName=name;
    }

    bool IsAnimation(string name)
    {
        if (currentAnimationName == name)
        { return true;}
        else
        { return false;}
    }

    void WalkAnimation(Vector3 movementDirection)
    {
        if (State==AppearanceState.dead)
        { return;}
        if (_character.Movement.IsMoving || _character.Movement.Agent.remainingDistance> _character.Movement.CurrentInteractionDistance)
        {
            if (IsAnimation("walk")==false)
            {
                SetAnimation("walk");
  
            }
            waitToIdle = 2;
        }
        else
        if(waitToIdle<0 && currentSocialAction=="" && !IsAnimation("idle"))
        {
        
            SetAnimation("idle");
        }
        if(waitToIdle>=0)
        waitToIdle-=Time.timeScale;

    
    }
    void FlipSpriteBasedOnMovement(Vector3 movementDirection)
    {




        if (movementDirection.x > 0.005f)
        {
            _spriteRenderer.flipX = false; // Facing right
        }

        if (movementDirection.x < -0.005f)
        {
            _spriteRenderer.flipX = true; // Facing left
        }


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
