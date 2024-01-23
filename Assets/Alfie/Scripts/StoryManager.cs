using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class StoryManager : MonoBehaviour
{
    public List<string[]> storyList = new List<string[]>();
    public float happiness = 0.5f;
    public float insanity = 0f;
    public int currentDay = 0;

    // Start is called before the first frame update
    void Start()
    {
        FirstLoad();
        RunEvent();
    }

    void RunEvent()
    {
        bool forcePositive = false;
        bool insaneEvent = false;
        int insanityChance = 0;

        // check chances of an insane event
        if(insanity > 0.4f)
        {
            float tInsanity = insanity;

            // every 0.1 increase to insanity increase the chance by one. So 0.6 would be 3/10, 1 would be 7/10
            bool done = false;
            while(done == false)
            {
                // if insanity is greater than 0.4 do a 1 in 10 chance you get an insane event
                if((tInsanity - 0.4f) > 0f)
                {
                    insanityChance += 1;
                    tInsanity -= 0.1f;
                }
                if(tInsanity > 0.2f)
                {
                    done = false;
                }
                else
                {
                    done = true;
                }
            }

            if((insanity - 0.2f) > 0)
            {
                insanityChance += 1;
            }
        }
        else
        {
            // If happiness is below 0.2 do a 50/50 whether the option will be forced positive
            if(happiness <= 0.2f)
            {
                if(Random.Range(0,1) == 1)
                {
                    forcePositive = true;
                }
            }
        }

        if(insanityChance != 0)
        {
            // this makes it an insane event
            if(Random.Range(0, 10) <= insanityChance)
            {
                List<string[]> tempList = new List<string[]>();

                foreach(string[] storyEvent in storyList)
                {
                    if(storyEvent[6] == "TRUE")
                    {
                        tempList.Add(storyEvent);
                    }
                }

                // we now have our insane event
                Debug.Log(GetRandomEvent(tempList)[0]);
            }
        }
        else
        {
            // this makes it have to be an event marked as positive
            if(forcePositive)
            {
                List<string[]> tempList = new List<string[]>();

                foreach(string[] storyEvent in storyList)
                {
                    if(storyEvent[5] == "TRUE")
                    {
                        tempList.Add(storyEvent);
                    }
                }

                // we now have our positive event
                Debug.Log(GetRandomEvent(tempList)[0]);
            }
            // this is a regular event selection
            else
            {
                // we now have our selected event
                Debug.Log(GetRandomEvent(storyList)[0]);
            }
        }
    }

    private string[] GetRandomEvent(List<string[]> filteredStoryList)
    {
        List<string[]> tempList = new List<string[]>();
                
        foreach(string[] storyEvent in filteredStoryList)
        {
            // if for example its 0.3 thats 30 percent so we add 30 of them

            if (float.TryParse(storyEvent[7], out float result))
            {
                // turn the probability into an int out of 100
                float probability = result;
                int probabilityInt = (int)Mathf.Round(probability * 100);
                //Debug.Log("Probability is: " + probabilityInt);

                // add the required amount to the tempList
                for(int i = 0; i <= probabilityInt; i++)
                {
                    tempList.Add(storyEvent);
                }
            }
            else
            {
                Debug.Log("Invalid datatype for probability in story entry: " + storyEvent[0] + " '" + storyEvent[7] + "'" + " Is not valid");
            }
        }

        int index = Random.Range(0, tempList.Count);
        string[] selectedStoryEvent = tempList[index];

        return selectedStoryEvent;
    }


    public void ChangeHappiness(int value)
    {
        happiness += value;
        Debug.Log("Happiness is now: " + happiness);
    }

    public void ChangeInsanity(int value)
    {
        insanity += value;
        Debug.Log("Insanity is now: " + insanity);
    }

    public void TimeIncrease()
    {
        //progresses time
        currentDay += 1;

        //saves
        SavePlayer();
    }

    void LoadCSV()
    {
        // imports the csv
        TextAsset textFile = Resources.Load<TextAsset>("stories");

        // splits into lines
        string[] splittedLines = textFile.text.Split("\n");

        // splits into items
<<<<<<< Updated upstream
        for(int i = 1; i < splittedLines.Length; i++)
=======
        for(int i = 3; i < splittedLines.Length; i++)
>>>>>>> Stashed changes
        {
            //Debug.Log(splittedLines[i]);
            string[] splittedValues = ParseCSVLine(splittedLines[i]);
            // Debug.Log(splittedValues[0]);
            storyList.Add(splittedValues);
        }
    }

    public void SavePlayer()
    {
        SaveSystem.SavePlayer(this);
    }

    public void LoadPlayer()
    { 
        PlayerData data = SaveSystem.LoadPlayer();

        storyList = data.storyList;
        happiness = data.happiness;
        insanity = data.insanity;
        currentDay = data.currentDay;
    }

    void FirstLoad()
    {
        string path = Application.persistentDataPath + "/player.ezeSave";
        
        if (File.Exists(path))
        {
            //LoadPlayer();

            // REMOVE THIS IN PRODUCTION
            LoadCSV();
            TimeIncrease();
            SavePlayer();
        }
        else
        {
            LoadCSV();
            TimeIncrease();
            SavePlayer();
        }
    }

    public static string[] ParseCSVLine(string csvLine)
    {
        List<string> fields = new List<string>();
        bool insideQuotes = false;
        string currentField = "";

        foreach (char c in csvLine)
        {
            if (c == ',' && !insideQuotes)
            {
                fields.Add(currentField);
                currentField = "";
            }
            else if (c == '"')
            {
                insideQuotes = !insideQuotes;
                // Optionally skip adding the quote to the field
            }
            else
            {
                currentField += c;
            }
        }

        fields.Add(currentField); // Add the last field
        return fields.ToArray();
    }
}
