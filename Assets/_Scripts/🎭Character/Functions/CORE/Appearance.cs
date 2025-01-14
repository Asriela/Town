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

    private void Start()
    {
        _spriteRenderer = _spriteTransform.GetComponent<SpriteRenderer>();
    }

    private void Update()
    {
        ChangeAppearanceAccordingToState();
        FlipSpriteBasedOnMovement();
    }

    public void ChangeColor(Color color)
    {
        _spriteRenderer.color = color;
    }
    private void ChangeAppearanceAccordingToState()
    {
        var temp = 0;

        var currentRotation = _spriteRenderer.transform.localRotation;

        switch (State)
        {
            case AppearanceState.dead:

                _spriteRenderer.gameObject.transform.localRotation = Quaternion.Euler(0, 0, 90);
                break;
            case AppearanceState.lyingDown:

                _spriteRenderer.gameObject.transform.localRotation = Quaternion.Euler(0, 0, 90);
        
                break;
            case AppearanceState.standing:

                _spriteRenderer.gameObject.transform.localRotation = Quaternion.Euler(0, 0, 0);
                break;
        }

    }

    void FlipSpriteBasedOnMovement()
    {

        Vector3 movementDirection = transform.position - previousPosition;


        if (movementDirection.x > 0.0001f)
        {
            _spriteRenderer.flipX = false; // Facing right
        }
        else if (movementDirection.x < -0.0001f)
        {
            _spriteRenderer.flipX = true; // Facing left
        }


        previousPosition = transform.position;
    }

}
