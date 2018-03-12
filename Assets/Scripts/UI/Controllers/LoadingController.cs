using System.Collections;
using System.Collections.Generic;
using PirateGame.UI.Views;
using UnityEngine;

namespace PirateGame.UI.Controllers
{
    public class LoadingController : Controller
    {

        public float activateSpeed = 2.0f;

        public LoadingView loadingView;

        private IEnumerator curEnumerator;

        public void ToggleOn()
        {
            if (curEnumerator != null)
            {
                StopCoroutine(curEnumerator);
            }
            curEnumerator = ToggleOnCoroutine();
            StartCoroutine(curEnumerator);
        }

        IEnumerator ToggleOnCoroutine()
        {
            loadingView.canvasGroup.blocksRaycasts = true;
            while (loadingView.canvasGroup.alpha < 1)
            {
                loadingView.canvasGroup.alpha += Time.deltaTime * activateSpeed;
                yield return null;
            }
            curEnumerator = null;
        }

        public void ToggleOff()
        {
            if (curEnumerator != null)
            {
                StopCoroutine(curEnumerator);
            }
            curEnumerator = ToggleOffCoroutine();
            StartCoroutine(curEnumerator);
        }

        IEnumerator ToggleOffCoroutine()
        {
            while (loadingView.canvasGroup.alpha > 0)
            {
                loadingView.canvasGroup.alpha -= Time.deltaTime * activateSpeed;
                yield return null;
            }
            curEnumerator = null;
            loadingView.canvasGroup.blocksRaycasts = false;
        }

    }
}
