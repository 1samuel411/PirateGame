﻿using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine.UI;
using UnityEngine;
using PirateGame.UI;

namespace PirateGame.Managers
{
	public class UIManager : MonoBehaviour
    {
        
        public static UIManager instance;

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

        void Awake()
        {
            instance = this;

            FadeOut();
        }

        void Reload()
        {
            for(int i = 0; i < activeScreens.Count; i++)
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
    }
}