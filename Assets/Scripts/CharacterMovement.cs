using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class CharacterMovement : MonoBehaviour
{

    [SerializeField] private GameObject Systems;
    private GameManager gameManager;
    private SwipeDetection swipeDetection;
    private GameState gameState;


    private Vector3Int startPosition = new (0, -6, 0);
    private int positionIndex = 2;

    private Vector3Int[] HorizontalSnapPoints;

    private GameObject parent;

    private void Awake()
    {
        gameManager = Systems.GetComponent<GameManager>();
        swipeDetection = Systems.GetComponent<SwipeDetection>();
        gameState = Systems.GetComponent<GameState>();

        HorizontalSnapPoints = gameManager.GetHorizontalSnapPositions();

        swipeDetection.swipePerformed.AddListener(Move);
        parent = gameObject.transform.parent.gameObject;
    }

    void Start()
    {
        parent.transform.position = startPosition;
        gameState.setPlayerPositionIndex(positionIndex);
    }

    public void ResetCharacterPosition()
    {
        for (int i = 0; i < HorizontalSnapPoints.Length; i++)
        {
            Move(new Vector2(-1, 0));
        }
        Move(new Vector2(1, 0));
        Move(new Vector2(1, 0));
    }

    void Move(Vector2 Direction)
    {
        if (gameState.gameState == GameState.GameStateEnum.Game || gameState.gameState == GameState.GameStateEnum.StartScreen)
        {

            if (Direction.x > 0)
            {
                if (positionIndex + 1 < HorizontalSnapPoints.Length)
                {
                    // In the future this will play the move animation
                    Vector3 currPos = parent.transform.position;
                    Vector3 nextPos = new(HorizontalSnapPoints[positionIndex + 1].x, currPos.y, currPos.z);
                    parent.transform.position = nextPos;
                    positionIndex++;
                }
                else
                {
                    // In the future this will play the bounds bounce animation
                    return;
                }
            }

            else if (Direction.x < 0)
            {
                if (positionIndex - 1 != -1)
                {
                    // In the future this will play the move animation
                    Vector3 currPos = parent.transform.position;
                    Vector3 nextPos = new(HorizontalSnapPoints[positionIndex - 1].x, currPos.y, currPos.z);
                    parent.transform.position = nextPos;
                    positionIndex--;
                }
                else
                {
                    // In the future this will play the bounds bounce animation
                    return;
                }
            }

            gameState.setPlayerPositionIndex(positionIndex);

            if (Direction.x == 0)
            {
                return;
            }

            Debug.Log("Current character position index:" + positionIndex);

        }
        }




}
