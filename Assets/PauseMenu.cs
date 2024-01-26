using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    public GameObject canvas;
    // Start is called before the first frame update
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Escape))
        {
            canvas.SetActive(true);
        }
    }
    public void Resume()
    {
        canvas.SetActive(false);
    }

    public void ReturnToMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }
}
