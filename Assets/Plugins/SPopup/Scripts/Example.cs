using UnityEngine;
using System.Collections;

public class Example : MonoBehaviour
{

    public void OneButtonExample()
    {
        Popup.Create("One Button Example", "This is the one button example. It contains only one button! It can be used to alert players of new items, updates, etc.", OneButtonCallback, "Popup", "Okay");
    }

    void OneButtonCallback(int callback)
    {
        if (callback == -1)
        {
            Debug.Log("Exited");
        }

        if (callback == 1)
        {
            Debug.Log("Pressed Okay Button");
        }
    }

    public void TwoButtonExample()
    {
        Popup.Create("Two Button Example", "This is the two button example. It contains only two buttons! It can be used to make sure an action is intended such as leaving, clicking a link, purchasing, etc", TwoButtonCallback, "Popup", "Accept", "Decline");
    }

    void TwoButtonCallback(int callback)
    {
        if (callback == -1)
        {
            Debug.Log("Exited");
        }

        if (callback == 1)
        {
            Debug.Log("Pressed Accept Button");
        }

        if (callback == 2)
        {
            Debug.Log("Pressed Decline Button");
        }
    }

    public void DialogueExample()
    {
        Popup.Create("Mr. Roboto", "This is the dialogue example. How are you?", FirstDialogue, "Dialogue", "Good", "Bad");

    }

    void FirstDialogue(int callback)
    {
        if (callback == -1)
        {
            Debug.Log("Exited");
        }

        if (callback == 1)
        {
            Debug.Log("Pressed Good Button");
            Popup.Create("Mr. Roboto", "I am glad to hear that", SecondDialogue, "Dialogue", "Okay..");
        }

        if (callback == 2)
        {
            Debug.Log("Pressed Bad Button");
            Popup.Create("Mr. Roboto", "I am sorry to hear that", SecondDialogue, "Dialogue", "Okay..");
        }
    }

    void SecondDialogue(int callback)
    {
        if(callback == -1)
        {
            Debug.Log("Exited");
        }

        if(callback == 1)
        {
            Debug.Log("Pressed okay");
        }
    }
}
