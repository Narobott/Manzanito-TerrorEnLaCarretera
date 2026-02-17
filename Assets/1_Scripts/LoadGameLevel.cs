using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadGameLevel : MonoBehaviour
{

    public void LoadLevel_Game()
    {
        StartCoroutine(LoadLevelAsync("GameScene"));
    }

    public void LoadLevel_Menu()
    {
        StartCoroutine(LoadLevelAsync("MainMenu"));
    }

    private IEnumerator LoadLevelAsync(string scene)
    {
        var load = SceneManager.LoadSceneAsync(scene);
        load.allowSceneActivation = true;

        yield return load;
        yield return null;
    }

}
