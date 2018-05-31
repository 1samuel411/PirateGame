using PirateGame.Managers;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoadScene : MonoBehaviour
{

    public string sceneToOpen;

    public void LoadSceneNow()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(sceneToOpen);
    }
}
