using System.Collections;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using static GameState;

public class FullscreenEffectsManager : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    [SerializeField] private ScriptableRendererFeature _fullscreenImpactFrame;
    [SerializeField] private GameState _gameState;

    private void Start()
    {
        _fullscreenImpactFrame.SetActive(false);
    }

    public IEnumerator InvokeImpactFrame()
    {
        _fullscreenImpactFrame.SetActive(true);
        yield return new WaitForSeconds(.2f);
        _fullscreenImpactFrame.SetActive(false);
        _gameState.SetGameState(GameStateEnum.Game);
    }
}
