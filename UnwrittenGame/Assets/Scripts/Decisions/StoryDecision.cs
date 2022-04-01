using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class StoryDecision : MonoBehaviour
{
    public string text;
    public TextMeshProUGUI storyDisplay;

    public GameObject wall;


    private bool triggered = false;
    void Start()
    {
        wall.SetActive(false);
    }

    private void OnTriggerEnter(Collider collider)
    {
        if (!triggered)
        {
            if (collider.gameObject.CompareTag("Player"))
            {
                StartCoroutine(displayText());
            }
        }
    }

    private void OnTriggerExit()
    {
        if (!triggered)
        {
            wall.SetActive(true);
            triggered = true;
        }
    }


    private IEnumerator displayText()
    {
        if (storyDisplay.text != "")
        {
            text = " " + text;
        }
        storyDisplay.text += text;
        WaitForSeconds wait = new WaitForSeconds(5.0f);
        yield return wait;
        storyDisplay.text = storyDisplay.text.Substring(text.Length, storyDisplay.text.Length-text.Length);
        //storyDisplay.text = "";
    }
}