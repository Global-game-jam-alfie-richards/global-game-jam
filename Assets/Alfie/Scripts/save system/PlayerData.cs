using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[System.Serializable]
public class PlayerData
{
    //simple values
    public List<string[]> storyList = new List<string[]>();
    public float happiness = 0.5f;
    public float insanity = 0f;
    public int currentDay = 0;

    //arrays
    public string[] buildsProgress; //3 can be saved

    public PlayerData (StoryManager storyManager)
    {
        storyList = storyManager.storyList;
        happiness = storyManager.happiness;
        insanity = storyManager.insanity;
        currentDay = storyManager.currentDay;
    }
}
