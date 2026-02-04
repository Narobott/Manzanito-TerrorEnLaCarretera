using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public class SwipeDetection : MonoBehaviour
{
    [SerializeField] TextMeshPro SwipeResistence;
    [SerializeField] TextMeshPro SwipeStart;
    [SerializeField] TextMeshPro SwipeEnd;
    [SerializeField] TextMeshPro SwipeDelta;

    public static SwipeDetection instance;
    public UnityEvent<Vector2> swipePerformed;
    public UnityEvent pressPerformed;

    [SerializeField] private InputAction position, press, move, parry;
    [SerializeField] private float swipeResistance = 100f;

    private Vector2 startPos;
    private Vector2 lastPos;

    private bool swipeCommitted;

    private Vector2 currentPos => lastPos;

    private void Awake()
    {
        position.Enable();
        press.Enable();
        move.Enable();
        parry.Enable();

        position.performed += ctx =>
        {
            lastPos = ctx.ReadValue<Vector2>();
            DetectSwipe(); // detect during movement
        };

        press.performed += _ =>
        {
            startPos = currentPos;
            swipeCommitted = false;

            SwipeStart.text = "Swipe started at: " + startPos;
            pressPerformed.Invoke();
        };

        press.canceled += _ =>
        {
            SwipeEnd.text = "Swipe ended at: " + currentPos;
        };

        move.performed += _ =>
        {
            swipePerformed.Invoke(new Vector2(move.ReadValue<float>(), 0));
        };

        parry.performed += _ =>
        {
            swipePerformed.Invoke(new Vector2(0, 1));
        };

        instance = this;
    }

    private void Start()
    {
        SwipeResistence.text = "Current swipe resistence: " + swipeResistance;
    }

    private void DetectSwipe()
    {
        if (swipeCommitted)
            return;

        Vector2 delta = currentPos - startPos;

        SwipeDelta.text =
            "Last delta: " +
            Mathf.Abs(delta.x).ToString("F1") + ", " +
            Mathf.Abs(delta.y).ToString("F1");

        // Dead zone
        if (delta.magnitude < swipeResistance)
            return;

        Vector2 direction;

        // Lock to dominant axis
        if (Mathf.Abs(delta.x) >= Mathf.Abs(delta.y))
        {
            direction = new Vector2(Mathf.Sign(delta.x), 0);
        }
        else
        {
            direction = new Vector2(0, Mathf.Sign(delta.y));
        }

        swipeCommitted = true;
        swipePerformed.Invoke(direction);

        Debug.Log(
            "Swipe detected: " +
            (direction.x != 0 ? (direction.x > 0 ? "Right" : "Left") : "Up")
        );
    }
}
