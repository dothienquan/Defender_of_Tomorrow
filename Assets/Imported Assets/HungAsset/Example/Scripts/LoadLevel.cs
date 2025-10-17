using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadLevel : MonoBehaviour {

    public float time;

    public void LoadlLevel(int levelIndex) {
        SceneManager.LoadScene(levelIndex, LoadSceneMode.Single);
    }
    public void LoadlLevelAfterTime(int levelIndex) {
        StartCoroutine(LoadLevelAfterTimeCoroutine(levelIndex));
    }

    IEnumerator LoadLevelAfterTimeCoroutine(int levelIndex) {
        yield return new WaitForSeconds(time);
        SceneManager.LoadScene(levelIndex, LoadSceneMode.Single);
    }
}
