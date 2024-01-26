using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System;

using Random=UnityEngine.Random;
using UnityEngine.SceneManagement;

public class StoryManager : MonoBehaviour
{
    // character prefabs
    [SerializeField] GameObject phone, computer, mail, kitchen, bed, money,initCamera;

    // events and story
    public List<string[]> storyList = new List<string[]>();
    EventPlayer[] eventPlayers;
    string[] currentEvent;

    // game stats
    public float happiness = 0.5f;
    public float insanity = 0f;

    // time keeping
    public int currentDay = 0;
    public int remainingEvents = 0;

    // event delays
    [SerializeField] private int maxEventDelay = 7;
    [SerializeField] private int minEventDelay = 5;

    // daily events
    [SerializeField] private int maxDailyEvents = 7;
    [SerializeField] private int minDailyEvents = 5;

    private LevelLoader levelLoader;

    // Start is called before the first frame update
    void Start()
    {
        eventPlayers = FindObjectsOfType<EventPlayer>();
        levelLoader = FindObjectOfType<LevelLoader>().GetComponent<LevelLoader>();
        

        // loads save file etc
        FirstLoad();

        // begins time cycle
        remainingEvents = Random.Range(minDailyEvents, maxDailyEvents);
        QueueTime();
        // RunEvent();
    }

    void QueueTime()
    {
        int delay = Random.Range(minEventDelay, maxEventDelay);
        Invoke("WaitTime", delay);
    }

    void WaitTime()
    {
        // time has now progressed so now we do the next event
        if(remainingEvents != 0)
        {
            RunEvent();
        }
        else
        {


            // do day ending (animation or something idk)
            //instantiate bed player object and fade out or something - dexter
            //GameObject bedPlayer = Instantiate(bed);
            ResetPlayerStates();

            bed.SetActive(true);

            // also here if we want events to happen specifically at the end of the day like scrolling shorts we can have it always happen if the remaining events is less than 2 but not 0

            Debug.Log("Day ended");

            // progresses to the next day

            // if the outcome of the day is sadness become mildly insane
            if(happiness < 0.3)
            {
                insanity += 0.1f;
            }

            if(happiness > 0.6)
            {
                insanity -= 0.1f;
            }

            // if happiness les than 0 set it to 0
            if(happiness < 0)
            {
                happiness = 0f;
            }

            // if happiness greater than 1 set it to 1
            if(happiness > 1)
            {
                happiness = 1f;
            }

            if(currentDay == 7)
            {
                // run an ending here
                EndingManager manager = GetComponent<EndingManager>();
                manager.DoEnding();
            }
            else
            {
                currentDay += 1;

                // DEBUGGING CODE REMOVE IN PROD
                // remainingEvents = Random.Range(minDailyEvents, maxDailyEvents);
                // QueueTime();
                // DEBUGGING CODE REMOVE IN PROD

                SavePlayer();

                // load scene based on happiness
                LoadSceneBasedOnHappiness();
            }
        }
    }

    private void LoadSceneBasedOnHappiness()
    {
        // Calculate the scene name based on the happiness value
        int sceneNumber = Mathf.Clamp(Mathf.FloorToInt(happiness * 10), 1, 10);
        string sceneName = sceneNumber + "Happiness";

        // Load the scene based on the calculated name
        levelLoader.LoadNextScene(sceneName);
    }

    void ResetPlayerStates()
    {
        initCamera.SetActive(false);
        phone.SetActive(false);
        computer.SetActive(false);
        mail.SetActive(false);
        kitchen.SetActive(false);
        bed.SetActive(false);
        money.SetActive(false);
    }


    void RunEvent()
    {
        bool forcePositive = false;
        int insanityChance = 0;

        // check chances of an insane event
        if (insanity > 0.4f)
        {
            float tInsanity = insanity;

            // every 0.1 increase to insanity increase the chance by one. So 0.6 would be 3/10, 1 would be 7/10
            bool done = false;
            while (done == false)
            {
                // if insanity is greater than 0.3 do a 1 in 10 chance you get an insane event
                if((tInsanity - 0.3f) > 0f)
                {
                    insanityChance += 1;
                    tInsanity -= 0.1f;
                }
                else
                {
                    done = true;
                }
            }
        }
        else
        {
            // If happiness is below 0.2 do a 50/50 whether the option will be forced positive
            if (happiness <= 0.2f)
            {
                if (Random.Range(0, 1) == 1)
                {
                    forcePositive = true;
                }
            }
        }

        if (insanityChance != 0)
        {
            // this makes it an insane event
            if (Random.Range(0, 10) <= insanityChance)
            {
                Debug.Log(insanityChance);
                List<string[]> tempList = new List<string[]>();

                foreach (string[] storyEvent in storyList)
                {
                    if(storyEvent[5] == "TRUE")
                    {
                        tempList.Add(storyEvent);
                    }
                }

                if(tempList.Count > 0)
                {
                    // we now have our insane event
                    currentEvent = (GetRandomEvent(tempList));
                }
            }
        }
        if(currentEvent == null)
        {
            // this makes it have to be an event marked as positive
            if (forcePositive)
            {
                List<string[]> tempList = new List<string[]>();

                foreach (string[] storyEvent in storyList)
                {
                    if(storyEvent[4] == "TRUE")
                    {
                        tempList.Add(storyEvent);
                    }
                }

                // we now have our positive event
                if(tempList.Count > 0)
                {
                    // we now have our insane event
                    currentEvent = (GetRandomEvent(tempList));
                }
                else
                {
                    // makes it so u only get insane events if youre insane
                    tempList = new List<string[]>();

                    foreach(string[] storyEvent in storyList)
                    {
                        if(storyEvent[5] != "TRUE")
                        {
                            tempList.Add(storyEvent);
                        }
                    }

                    // we now have our positive event
                    currentEvent = (GetRandomEvent(tempList));
                }
            }
            // this is a regular event selection
            else
            {
                // makes it so u only get insane events if youre insane
                List<string[]> tempList = new List<string[]>();

                foreach(string[] storyEvent in storyList)
                {
                    if(storyEvent[5] != "TRUE")
                    {
                        tempList.Add(storyEvent);
                    }
                }

                // we now have our positive event
                currentEvent = (GetRandomEvent(tempList));
            }
        }

        Debug.Log(currentEvent[0]);
        storyList.Remove(currentEvent);


        // get event location ready for players
        string eventLocation = currentEvent[3];

        Vector3 position = new Vector3(0, 0, 0);
        Quaternion rotation = Quaternion.Euler(0, 0, 0);
        Vector3 scale = new Vector3(1.35f, 1.35f, 1.35f);

        ResetPlayerStates();

        // activate the corresponding player object
        switch (eventLocation)
        {
            case "phone on desk":

                //GameObject phonePlayer = Instantiate(phone);
                ResetPlayerStates();
                phone.SetActive(true);
                break;

            case "computer screen":

                //GameObject screenPlayer = Instantiate(computer);
                ResetPlayerStates();
                computer.SetActive(true);
                break;

            case "mail":

                //GameObject mailPlayer = Instantiate(mail);
                ResetPlayerStates();
                mail.SetActive(true);
                break;

            case "kitchen":

                //GameObject kitchenPlayer = Instantiate(kitchen);
                ResetPlayerStates();
                kitchen.SetActive(true);
                break;

            case "money":

                //GameObject moneyPlayer = Instantiate(money);
                ResetPlayerStates();
                money.SetActive(true);
                break;
            default:

                //GameObject bedPlayer = Instantiate(bed);
                ResetPlayerStates();
                bed.SetActive(true);
                break;
        }

        // find all objects of type EventPlayer, call the method on them
        foreach(EventPlayer eventPlayer in eventPlayers)
        {
            eventPlayer.RunEvent(eventLocation, currentEvent);
        }
    }

    private string[] GetRandomEvent(List<string[]> filteredStoryList)
    {
        List<string[]> tempList = new List<string[]>();

        foreach (string[] storyEvent in filteredStoryList)
        {
            // if for example its 0.3 thats 30 percent so we add 30 of them

            if (float.TryParse(storyEvent[6], out float result))
            {
                // turn the probability into an int out of 100
                float probability = result;
                int probabilityInt = (int)Mathf.Round(probability * 100);
                //Debug.Log("Probability is: " + probabilityInt);

                // add the required amount to the tempList
                for (int i = 0; i <= probabilityInt; i++)
                {
                    tempList.Add(storyEvent);
                }
            }
            else
            {
                Debug.Log("Invalid datatype for probability in story entry: " + storyEvent[0] + " '" + storyEvent[6] + "'" + " Is not valid");
            }
        }

        int index = Random.Range(0, tempList.Count);
        string[] selectedStoryEvent = tempList[index];

        return selectedStoryEvent;
    }


    public void ChangeHappiness(float value)
    {
        happiness += value;
        happiness = (float)Math.Round(happiness, 2);
        Debug.Log("Happiness is now: " + happiness);

        // also probably animate the camera returning here?? or we handle all that in the event player itself which is probs a better idea

        // start the next event timer
        remainingEvents --;
        currentEvent = null;
        QueueTime();
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
        for (int i = 3; i < splittedLines.Length; i++)
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
            LoadPlayer();

            // // DEBUGGING CODE REMOVE IN PROD
            // LoadCSV();
            // TimeIncrease();
            // SavePlayer();
            // // DEBUGGING CODE REMOVE IN PROD
        }
        else
        {
            LoadCSV();
            TimeIncrease();
            SavePlayer();
        }
    }

    public string[] ParseCSVLine(string csvLine)
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
