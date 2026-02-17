using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadGameLevel : MonoBehaviour
{

    public void LoadLevel_Game()
    {
        StartCoroutine(LoadLevelAsync(1));
    }

    public void LoadLevel_Menu()
    {
        StartCoroutine(LoadLevelAsync(0));
    }

    private IEnumerator LoadLevelAsync(int scene)
    {
        var load = SceneManager.LoadSceneAsync(scene);
        load.allowSceneActivation = true;

        yield return load;
        yield return null;
    }

}
