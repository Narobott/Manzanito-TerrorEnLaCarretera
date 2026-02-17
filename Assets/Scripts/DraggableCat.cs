using UnityEngine;

public class DraggableCat : MonoBehaviour
{
    [SerializeField]
    private SwipeDetection swipeDetection;

    public bool bBeeingDraged;
    private Vector2 fingerPosition;

    private void Start()
    {
        swipeDetection.pressPerformed.AddListener(OnPressPerformed);
        swipeDetection.simpleSwipePerformed.AddListener(ApplyForceInDirection);
    }

    // This method gets triggered when a press event occurs
    void OnPressPerformed(Vector2 pressPosition)
    {
        if (bBeeingDraged)
        {
            // Follow the touch or mouse position during drag
            UpdatePositionToFinger(pressPosition);
        }
    }

    private void UpdatePositionToFinger(Vector2 pressPosition)
    {

    }

    void ApplyForceInDirection(Vector2 direction)
    {
        // Apply force based on swipe direction (not directly related to drag, so kept as is)
        gameObject.GetComponent<Rigidbody2D>().AddForce(direction);
    }
}
