using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine.UI;
using UnityEngine;
using PirateGame.UI;
using PirateGame.UI.Controllers;

namespace PirateGame.Managers
{
	public class UIManager : Base
    {
        
        public static UIManager instance;

        public string initialScreen = "Menu";

        [System.Serializable]
        public struct MenuScreen
        {
            public string name;
            public Controller menuScreen;
        }
        public List<MenuScreen> menuScreens = new List<MenuScreen>();

        public List<MenuScreen> activeScreens = new List<MenuScreen>();

        public float fadeSpeed;
        public Image fadeImage;

        public Controller inGameCanvas;
        public Controller mainMenuCanvas;

        public string menuScene = "Menu";

        public bool loading;
        public LoadingController loadingController;

        void Awake()
        {
            if (instance != null)
            {
                return;
            }

            instance = this;

            FadeOut();

            ScreenSwitch(initialScreen);
        }

        private bool lastLoading;
        void Update()
        {
            if (lastLoading != loading)
            {
                lastLoading = loading;
                UpdateLoading();
            }

            if (UnityEngine.SceneManagement.SceneManager.GetActiveScene().name == menuScene)
            {
                inGameCanvas.gameObject.SetActive(false);
                mainMenuCanvas.gameObject.SetActive(true);
            }
            else
            {
                inGameCanvas.gameObject.SetActive(true);
                mainMenuCanvas.gameObject.SetActive(false);
            }
        }

        public float autoStopTimer = 0;
        void UpdateLoading()
        {
            if(autoStopTimer != 0)
            {
                if(Time.time >= autoStopTimer)
                {
                    loading = false;
                    autoStopTimer = 0;
                }
            }

            if(loading)
                loadingController.ToggleOn();
            else
                loadingController.ToggleOff();
        }

        public void LoadMasterServerCall()
        {
            loading = true;
            autoStopTimer = Time.time + .5f;
        }

        void Reload()
        {
            for (int i = 0; i < menuScreens.Count; i++)
            {
                menuScreens[i].menuScreen.gameObject.SetActive(false);
            }

            for (int i = 0; i < activeScreens.Count; i++)
            {
                activeScreens[i].menuScreen.gameObject.SetActive(true);
            }
        }

        public void ScreenSwitch(string controllerToSwitchTo)
        {
            if(activeScreens.Count > 0)
            {
                activeScreens[0] = menuScreens.Find(x => x.name == controllerToSwitchTo);
            }
            else
            {
                activeScreens.Add(menuScreens.Find(x => x.name == controllerToSwitchTo));
            }
            Reload();
        }

        public void UnloadAll()
        {
            activeScreens.Clear();
        }

        public void AddScreenAdditive(string controllerToAdd)
        {
            activeScreens.Add(menuScreens.Find(x => x.name == controllerToAdd));
            Reload();
        }

        public void UnloadScreenAdditive(string controllerToAdd)
        {
            activeScreens.Remove(activeScreens.Find(x => x.name == controllerToAdd));
            Reload();
        }
        
        public void FadeOut(Action callback = null)
        {
            StartCoroutine(FadeCoroutine(false, callback));
        } 

        public void FadeIn(Action callback = null)
        {
            StartCoroutine(FadeCoroutine(true, callback));
        }

        IEnumerator FadeCoroutine(bool fadeIn, Action callback)
        {
            float alpha = 0;
            if(fadeIn)
            {
                while(alpha < 1)
                {
                    alpha += fadeSpeed * Time.deltaTime;
                    fadeImage.color = new Color(fadeImage.color.r, fadeImage.color.g, fadeImage.color.b, alpha);
                    yield return null;
                }
            }
            else
            {
                alpha = 1;
                while(alpha > 0)
                {
                    alpha -= fadeSpeed * Time.deltaTime;
                    fadeImage.color = new Color(fadeImage.color.r, fadeImage.color.g, fadeImage.color.b, alpha);
                    yield return null;
                }
            }
            if(callback != null)
            {
                callback.Invoke();
            }
        }

        public static string StrikeThrough(string s)
        {
            string strikethrough = "";
            foreach (char c in s)
            {
                strikethrough = strikethrough + c + '\u0336';
            }
            return strikethrough;
        }

        public bool IsScreenOpen(string screen)
        {
            for (int i = 0; i < UIManager.instance.activeScreens.Count; i++)
            {
                if (UIManager.instance.activeScreens[i].name == screen)
                {
                    return true;
                }
            }

            return false;
        }
    }
}