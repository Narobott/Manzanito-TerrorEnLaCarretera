using UnityEditor.PackageManager;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public class SwipeDetection : MonoBehaviour
{
    public static SwipeDetection instance;
    public delegate void Swipe(Vector2 direction);
    public UnityEvent<Vector2> swipePerformed;
    public UnityEvent pressPerformed;
    [SerializeField] private InputAction position, press;

    [SerializeField] private float swipeResistance = 100;
    private Vector2 startPos;
    private Vector2 currentPos => position.ReadValue<Vector2>();

    private void Awake()
    {
        position.Enable();
        press.Enable();
        press.performed += _ => { startPos = currentPos; Debug.Log("Press performed at pos " + currentPos); pressPerformed.Invoke(); };
        press.canceled += _ => { DetectSwipe(); Debug.Log("Press canceled at pos " + currentPos); };
        instance = this;
    }

    private void DetectSwipe()
    {
        Vector2 delta = currentPos - startPos;
        Vector2 direction = Vector2.zero;

        if (Mathf.Abs(delta.x) > swipeResistance)
        {
            direction.x = Mathf.Clamp(delta.x, -1, 1);
        }
        if (Mathf.Abs(delta.y) > swipeResistance)
        {
            direction.y = Mathf.Clamp(delta.y, -1, 1);
        }
        if (direction != Vector2.zero)
        {
            swipePerformed.Invoke(direction);
            Debug.Log("Swipe detexted, direction:" + (direction.x > 0 ? "x" : "-x"));
        }

    }
}
