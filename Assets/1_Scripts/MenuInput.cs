// MenuInput.cs
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public class MenuInput : MonoBehaviour
{
    public static MenuInput instance;

    public UnityEvent<Vector2> pressStarted = new UnityEvent<Vector2>();
    public UnityEvent<Vector2> pressMoved = new UnityEvent<Vector2>();
    public UnityEvent<Vector2> pressEnded = new UnityEvent<Vector2>();

    [SerializeField] private InputAction position;
    [SerializeField] private InputAction press;

    private Vector2 currentPos;
    private bool isPressed;

    public Vector2 CurrentScreenPosition => currentPos;

    private void Awake()
    {
        instance = this;
        position.Enable();
        press.Enable();

        position.performed += ctx =>
        {
            currentPos = ctx.ReadValue<Vector2>();
            if (isPressed)
                pressMoved.Invoke(currentPos);
        };

        press.performed += _ =>
        {
            isPressed = true;
            pressStarted.Invoke(currentPos);
        };

        press.canceled += _ =>
        {
            isPressed = false;
            pressEnded.Invoke(currentPos);
        };
    }
}