using System;
using UnityEngine;

public class CharacterStats : MonoBehaviour
{
    [SerializeField] private int CharacterHealth;

    private void SetCharacterHealth(int characterHealth)
    {
        CharacterHealth = characterHealth;
        if (CharacterHealth == 0)
        {
            Die();
        }
    }

    public void ReduceCharacterHealth()
    {
        SetCharacterHealth(CharacterHealth - 1);
    }

    public void IncreaseCharacterHealth()
    {
        SetCharacterHealth(CharacterHealth + 1);
    }

    private void Die()
    {
        Destroy(gameObject);
        GameObject.Find("Systems").GetComponent<GameState>().SetGameState(GameState.GameStateEnum.LoseScreen);


    }

}
