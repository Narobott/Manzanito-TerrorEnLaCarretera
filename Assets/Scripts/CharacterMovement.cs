using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class CharacterController : MonoBehaviour
{

    [SerializeField] private GameObject Systems;
    private GameManager gameManager;
    private SwipeDetection swipeDetection;


    private Vector3Int startPosition = new (0, -6, 0);
    private int positionIndex = 2;

    private Vector3Int[] HorizontalSnapPoints;

    private void Awake()
    {
        gameManager = Systems.GetComponent<GameManager>();
        swipeDetection = Systems.GetComponent<SwipeDetection>();

        HorizontalSnapPoints = gameManager.GetHorizontalSnapPositions();

        swipeDetection.swipePerformed.AddListener(Move);
    }

    void Start()
    {
        gameObject.transform.position = startPosition;

    }

    void Move(Vector2 Direction)
    {
        if (Direction.x > 0)
        {
            if (positionIndex + 1 < HorizontalSnapPoints.Length)
            {
                // In the future this will play the move animation
                Vector3 currPos = gameObject.transform.position;
                Vector3 nextPos = new(HorizontalSnapPoints[positionIndex + 1].x, currPos.y, currPos.z);
                gameObject.transform.position = nextPos;
                positionIndex++;
            }
            else
            {
                // In the future this will play the bounds bounce animation
                return;
            }
        }

        if (Direction.x < 0)
        {
            if (positionIndex - 1 != -1)
            {
                // In the future this will play the move animation
                Vector3 currPos = gameObject.transform.position;
                Vector3 nextPos = new(HorizontalSnapPoints[positionIndex - 1].x, currPos.y, currPos.z);
                gameObject.transform.position = nextPos;
                positionIndex--;
            }
            else
            {
                // In the future this will play the bounds bounce animation
                return;
            }
        }

        if (Direction.x == 0)
        {
            return;
        }

        Debug.Log("Current character position index:" + positionIndex);

    }




}
