using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayButton : MonoBehaviour
{
    float happiness = 0.5f;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void PlayGame()
    {
        LoadPlayer();
        // Calculate the scene name based on the happiness value
        int sceneNumber = Mathf.Clamp(Mathf.FloorToInt(happiness * 10), 1, 10);
        string sceneName = sceneNumber + "Happiness";

        // Load the scene based on the calculated name
        SceneManager.LoadScene(sceneName);
    }

    void LoadPlayer()
    {
        string path = Application.persistentDataPath + "/player.ezeSave";

        if (File.Exists(path))
        {
            PlayerData data = SaveSystem.LoadPlayer();
            happiness = data.happiness;
        }
        else
        {
            SceneManager.LoadScene("5Happiness");
        }
    }
}
