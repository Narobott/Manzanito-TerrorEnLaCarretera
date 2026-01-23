using UnityEngine;

public class GameManager : MonoBehaviour
{

    [SerializeField]
    private Vector3Int[] horizontalSnapPositions = null;

    public Vector3Int[] GetHorizontalSnapPositions()
    {
        return horizontalSnapPositions;
    }
}
