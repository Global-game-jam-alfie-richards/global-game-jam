using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class DeleteSave : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Delete()
    {
        string path = Application.persistentDataPath + "/player.ezeSave";

        if (File.Exists(path))
        {
            // if theres a save file delete it
            File.Delete(path);
        }
    }
}
