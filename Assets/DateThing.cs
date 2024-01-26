using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DateThing : MonoBehaviour
{
    // Start is called before the first frame update
    public TextMeshProUGUI text;
    void Start()
    {
        int tDay = GameObject.FindObjectOfType<StoryManager>().currentDay;
        text.text = (tDay + 19).ToString() + "th of January 2024";
    }
}
