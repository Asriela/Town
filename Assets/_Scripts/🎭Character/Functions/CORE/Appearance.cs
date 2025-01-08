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




    void Update()
    {
        ChangeAppearanceAccordingToState();
    }

    private void ChangeAppearanceAccordingToState()
    {


        var currentRotation = _character.SpriteRenderer.transform.localRotation;

        switch (State)
        {
            case AppearanceState.dead:

                _character.SpriteRenderer.transform.localRotation = Quaternion.Euler(0, 0, 90);
                break;
            case AppearanceState.lyingDown:

                _character.SpriteRenderer.transform.localRotation = Quaternion.Euler(0, 0, 90);
                break;
            case AppearanceState.standing:

                _character.SpriteRenderer.transform.localRotation = Quaternion.Euler(0, 0, 0);
                break;
        }

    }

}
