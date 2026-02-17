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
    public UnityEvent<Vector2> pressPerformed;
    public UnityEvent<Vector2> simpleSwipePerformed;

    [SerializeField] private InputAction position, press, move, parry;
    [SerializeField] private float swipeResistance = 100f;

    private Vector2 startPos;
    private Vector2 lastPos;

    private bool swipeCommitted;
    private DraggableCat catBeeingDragged;
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

            if (SwipeStart != null)
            {
                SwipeStart.text = "Swipe started at: " + startPos;
            }
            pressPerformed.Invoke(currentPos);


            Vector2 origin = Camera.main.ScreenToViewportPoint(startPos);
            Vector3 direction = Camera.main.ScreenToWorldPoint(currentPos);
            RaycastHit hit;
            bool bHit = Physics.Raycast(origin, direction, out hit);
            if (bHit)
            {
                if (hit.collider.gameObject.tag == "DragableCat")
                {
                    catBeeingDragged = hit.collider .gameObject.GetComponent<DraggableCat>();
                    catBeeingDragged.bBeeingDraged = true;
                }
            }
        };

        press.canceled += _ =>
        {
            if (SwipeEnd != null)
            {
                SwipeEnd.text = "Swipe ended at: " + currentPos;
            }

            if (catBeeingDragged != null)
            {
                catBeeingDragged.bBeeingDraged = false;
                catBeeingDragged = null;
            }

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
        if (SwipeResistence != null)
        {
            SwipeResistence.text = "Current swipe resistence: " + swipeResistance;
        }
    }

    private void DetectSwipe()
    {
        if (swipeCommitted)
            return;

        Vector2 delta = currentPos - startPos;

        if (SwipeDelta != null)
        {
            SwipeDelta.text =
                "Last delta: " +
                Mathf.Abs(delta.x).ToString("F1") + ", " +
                Mathf.Abs(delta.y).ToString("F1");
        }

        // Dead zone
        if (delta.magnitude < swipeResistance)
            return;


        executeSimpleSwipe(delta);

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

    void executeSimpleSwipe(Vector2 delta)
    {
        simpleSwipePerformed.Invoke(delta);
    }

}
