using Unity.VisualScripting;
using UnityEngine;

public class CharacterCollision : MonoBehaviour
{
    CircleCollider2D Collider;
    CharacterStats characterStats;

    private void Awake()
    {
        Collider = GetComponent<CircleCollider2D>();
        characterStats = GetComponent<CharacterStats>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Enemy"))
        {
            characterStats.ReduceCharacterHealth();
        }
    }

    void Update()
    {
        
    }
}
