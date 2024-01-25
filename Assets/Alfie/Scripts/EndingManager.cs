using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using TMPro;

public class EndingManager : MonoBehaviour
{
    public List<string[]> endingList = new List<string[]>();

    public Volume volume;
    float timeElapsed;
    float lerpDuration = 3;
    public bool enableBlur = false;
    public TextMeshProUGUI endingText;
    public GameObject canvas;

    Color32 textColor = new Color32((byte)255, (byte)255, (byte)255, (byte)0);

    public void DoEnding()
    {
        StoryManager manager = GetComponent<StoryManager>();
        LoadCSV();

        // pick one based on happiness

        // makes sure it is in bounds
        if(manager.happiness >= 1){manager.happiness = 1;}
        if(manager.happiness <= 0){manager.happiness = 0;}

        // selects the ending with the closest value to happiness
        string[] selectedEnding = FindClosestItem(endingList, manager.happiness);

        // somhow runs the ending??
        enableBlur = true;
        endingText.text = selectedEnding[1];

    }

    public void MoneyEnding()
    {
        // do money
        // somhow runs the ending??
        LoadCSV();
        enableBlur = true;
        endingText.text = endingList[0][1];

    }

    void Update()
    {
        if(enableBlur)
        {
            if (timeElapsed < lerpDuration)
            {
                volume.weight = Mathf.Lerp(0.5f, 1, timeElapsed / lerpDuration);
                textColor.a = (byte)Mathf.Lerp(0, 255, timeElapsed / lerpDuration);
                endingText.color = textColor;

                timeElapsed += Time.deltaTime;
            }
        }
    }

    static string[] FindClosestItem(List<string[]> data, float target)
    {
        if (data.Count == 0)
        {
            Debug.Log("Cant have an empty list mate");
        }

        string[] closestItem = data[0];
        float minDifference = Mathf.Abs(target - float.Parse(data[0][2]));

        for (int i = 1; i < data.Count; i++)
        {
            float difference = Mathf.Abs(target - float.Parse(data[i][2]));
            if (difference < minDifference)
            {
                closestItem = data[i];
                minDifference = difference;
            }
        }

        return closestItem;
    }

    void LoadCSV()
    {
        // imports the csv
        TextAsset textFile = Resources.Load<TextAsset>("endings");

        // splits into lines
        string[] splittedLines = textFile.text.Split("\n");

        // splits into items
        for(int i = 3; i < splittedLines.Length; i++)
        {
            //Debug.Log(splittedLines[i]);
            string[] splittedValues = ParseCSVLine(splittedLines[i]);
            // Debug.Log(splittedValues[0]);
            endingList.Add(splittedValues);
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
