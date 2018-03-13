using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System;
using System.Collections.Generic;

public class Popup : MonoBehaviour
{

    [System.Serializable]
    public struct PopupButton
    {
        public Button button;
        public Text text;
    }

    public Image bgImage;
    public AnimationPlayer animationPlayer;

    public Text titleText;
    public Text descriptionText;
    public List<PopupButton> buttons = new List<PopupButton>();
    public GameObject buttonHolder;
    public string title, description;
    [HideInInspector]
    public Action<int> responseCallback;

    void Awake()
    {
        animationPlayer = GetComponent<AnimationPlayer>();
    }

    public void Press(int button)
    {
        if (responseCallback != null)
            responseCallback(button);
        GoAway();
    }

    public void Exit()
    {
        if (responseCallback != null)
            responseCallback(-1);
        GoAway();
    }

    void GoAway()
    {
        StartCoroutine(Destruct());
        animationPlayer.PlayAnimation("choose_stop");
    }

    IEnumerator Destruct()
    {
        yield return new WaitForSeconds(1);
        GameObject.Destroy(transform.parent.gameObject);
    }

    void Update()
    {
        titleText.text = title;
        descriptionText.text = description;
    }

    /// <summary>
    /// Creates a popup with a limit of 9 buttons. By default the popupObject should be Popup
    /// </summary>
    /// <param name="title">Title</param>
    /// <param name="description">Description</param>
    /// <param name="responseCallback">Callback</param>
    /// <param name="popupObject">The name of the popup object in the Resources Folder (Popup by default)</param>
    /// <param name="button1">Name of Button1</param>
    /// <param name="button2">Name of Button2</param>
    /// <param name="button3">Name of Button3</param>
    /// <param name="button4">Name of Button4</param>
    /// <param name="button5">Name of Button5</param>
    /// <param name="button6">Name of Button6</param>
    /// <param name="button7">Name of Button7</param>
    /// <param name="button8">Name of Button8</param>
    /// <param name="button9">Name of Button9</param>
    public static void Create(string title, string description, Action<int> responseCallback, string popupObject = "Popup", string button1 = "", string button2 = "", string button3 = "", string button4 = "", string button5 = "", string button6 = "", string button7 = "", string button8 = "", string button9 = "")
    {
        GameObject popupObj = Instantiate(Resources.Load("Popup/" + popupObject)) as GameObject;
        Popup popup = popupObj.GetComponentInChildren<Popup>();
        popup.title = title;
        popup.description = description;

        PopupButton popupButton = new PopupButton();
        if (button9 != "")
        {
            popupButton = CreateButton(button9, popup, 9);
            popup.buttons.Add(popupButton);
        }
        if (button8 != "")
        {
            popupButton = CreateButton(button8, popup, 8);
            popup.buttons.Add(popupButton);
        }

        if (button7 != "")
        {
            popupButton = CreateButton(button7, popup, 7);
            popup.buttons.Add(popupButton);
        }

        if (button6 != "")
        {
            popupButton = CreateButton(button6, popup, 6);
            popup.buttons.Add(popupButton);
        }

        if (button5 != "")
        {
            popupButton = CreateButton(button5, popup, 5);
            popup.buttons.Add(popupButton);
        }

        if (button4 != "")
        {
            popupButton = CreateButton(button4, popup, 4);
            popup.buttons.Add(popupButton);
        }

        if (button3 != "")
        {
            popupButton = CreateButton(button3, popup, 3);
            popup.buttons.Add(popupButton);
        }

        if (button2 != "")
        {
            popupButton = CreateButton(button2, popup, 2);
            popup.buttons.Add(popupButton);
        }

        if (button1 != "")
        {
            popupButton = CreateButton(button1, popup, 1);
            popup.buttons.Add(popupButton);
        }

        popup.responseCallback = responseCallback;
    }

    private static PopupButton CreateButton(string name, Popup popup, int id)
    {
        PopupButton popupButton = new PopupButton();
        GameObject buttonObj = (GameObject)Instantiate(Resources.Load("Popup/" + "Button"));
        buttonObj.transform.SetParent(popup.buttonHolder.transform, false);

        popupButton.text = buttonObj.GetComponentInChildren<Text>();
        popupButton.button = buttonObj.GetComponentInChildren<Button>();

        popupButton.button.onClick.AddListener(() => {
            popup.bgImage.gameObject.SetActive(false);
            popup.animationPlayer.PlayAnimation("popup_start");
            popup.Press(id);
        });

        popupButton.text.text = name;
        return popupButton;
    }
}
