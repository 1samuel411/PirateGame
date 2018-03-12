using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System;

public class Popup : MonoBehaviour
{

    public Text titleText;
    public Text descriptionText;
    public Text acceptText;
    public Text declineText;
    public Button acceptButton;
    public Button declineButton;

    public string title, description, accept, decline;
    [HideInInspector]
    public Action<ResponseTypes> responseCallback;
    [HideInInspector]
    public bool acceptOnly;
    [HideInInspector]
    public AnimationPlayer animationPlayer;

    void Awake()
    {
        animationPlayer = GetComponent<AnimationPlayer>();
    }

    public void Accept()
    {
        responseCallback(ResponseTypes.Accepted);
        GoAway();
    }

    public void Decline()
    {
        responseCallback(ResponseTypes.Declined);
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
        acceptText.text = accept;
        declineText.text = decline;
        if (acceptOnly)
        {
            acceptButton.transform.localPosition = new Vector3(0, acceptButton.transform.localPosition.y);
            declineButton.gameObject.SetActive(false);
        }
    }

    public static void Create(string title, string description, string accept, string decline = "", bool acceptOnly = false, Action<ResponseTypes> responseCallback = null)
    {
        GameObject popupObj = Instantiate(Resources.Load("Popup")) as GameObject;
        Popup popup = popupObj.GetComponentInChildren<Popup>();
        popup.title = title;
        popup.description = description;
        popup.accept = accept;
        popup.decline = decline;
        popup.acceptOnly = acceptOnly;
        popup.responseCallback = responseCallback;
    }

    public enum ResponseTypes { Accepted, Declined, Exited, None };
}
