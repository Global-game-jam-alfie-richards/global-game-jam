using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class EventPlayer : MonoBehaviour
{
    // event info
    public string location;

    // ui elements
    [SerializeField] private GameObject canvas;
    [SerializeField] private TextMeshProUGUI option1;
    [SerializeField] private TextMeshProUGUI option2;

    // animation options
    // specify the animation like clip here that needs to be played for this specific object and then used in line 28
    public float animationDelay = 0f;

    // other options
    private StoryManager storyManager;
    private string[] currentEvent;

    // Start is called before the first frame update
    void Start()
    {
        storyManager = GameObject.Find("StoryManager").GetComponent<StoryManager>();
    }

    public void RunEvent(string eventLocation, string[] storyEvent)
    {
        eventLocation = eventLocation.Trim('"');
        // Debug.Log(eventLocation);
        if(eventLocation == location)
        {
            // play animation or sound here

            

            // do the event
            StartCoroutine(DoEvent(storyEvent));
        }
    }

    IEnumerator DoEvent(string[] storyEvent)
    {
        currentEvent = storyEvent;
        yield return new WaitForSeconds(animationDelay);

        // set the option text
        string[] options = GetOptions(storyEvent);

        // Debug.Log(options[0]);

        option1.text = options[0];
        option2.text = options[1];

        // enable the canvas
        canvas.SetActive(true);
    }

    private string[] GetOptions(string[] storyEvent)
    {
        string[] tOptions = {"", ""};
        tOptions[0] = storyEvent[1];
        tOptions[1] = storyEvent[2];


        // Debug.Log(tOptions[0]);
        // Debug.Log(tOptions[1]);

        return tOptions;
    }

    public void ButtonPress(int button)
    {
        // true is 1 false is 2
        bool chosenButton = false;
        switch(button)
        {
            case 1:
                // Debug.Log("Button 1 pressed");
                chosenButton = true;
                break;

            case 2:
                // Debug.Log("Button 2 pressed");
                chosenButton = false;
                break;
        }

        float influence = 0f;
        if(chosenButton)
        {
            if (float.TryParse(currentEvent[7], out float result))
            {
                if(currentEvent[5] != "TRUE"){influence = result;}
            }
            else
            {
                Debug.Log("Invalid datatype for influence in story entry: " + currentEvent[0] + " '" + currentEvent[7] + "'" + " Is not valid");
            }
        }
        else
        {
            if (float.TryParse(currentEvent[8], out float result))
            {
                if(currentEvent[5] != "TRUE"){influence = result;}
            }
            else
            {
                Debug.Log("Invalid datatype for influence in story entry: " + currentEvent[0] + " '" + currentEvent[7] + "'" + " Is not valid");
            }
        }

        if(influence != 0f)
        {
            Debug.Log("Influence is: " + influence);
            storyManager.ChangeHappiness(influence);
        }
        else
        {
            storyManager.ChangeHappiness(0f);
        }

        // close ui and animate like exit
        EndEvent();
    }

    void EndEvent()
    {
        canvas.SetActive(false);
        // fade to black
    }

}
