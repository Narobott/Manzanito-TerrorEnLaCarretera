using UnityEngine;

public class LoadCharacterSkin : MonoBehaviour
{
    [SerializeField] GameObject[] AvailableSkins;

    private void Start()
    {
        Instantiate(AvailableSkins[PlayerPrefs.GetInt("Skin")],
            transform.position,
            Quaternion.identity,
            transform);

    }
}
