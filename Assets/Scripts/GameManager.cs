using UnityEngine;

public class GameManager : MonoBehaviour
{

    [SerializeField]
    private Vector3Int[] horizontalSnapPositions = null;

    public Vector3Int[] GetHorizontalSnapPositions()
    {
        return horizontalSnapPositions;
    }

    [SerializeField]
    public int enemyDodgePoints = 300;
}
