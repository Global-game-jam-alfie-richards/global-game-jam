using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelLoader : MonoBehaviour
{
    public Animator transition;

    public float transitionTime = 1f;

    // Update is called once per frame
    public void LoadNextScene(string sceneName)
    {
        StartCoroutine(LoadLevel(sceneName));
    }

    public IEnumerator LoadLevel(string sceneName)
    {
        //Play animation
        transition.SetTrigger("Start");

        //Wait
        yield return new WaitForSeconds(transitionTime);

        //Load scene
        SceneManager.LoadScene(sceneName);
    }
}
