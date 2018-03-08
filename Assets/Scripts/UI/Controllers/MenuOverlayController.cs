using System.Collections;
using System.Collections.Generic;
using PirateGame.Managers;
using UnityEngine;

namespace PirateGame.UI.Controllers
{
    public class MenuOverlayController : Controller
    {

        public PirateGame.UI.Views.MenuOverlayView overlayView;

        void Update()
        {
            overlayView.rankImage.sprite = IconManager.instance.rankSprites[PlayerManager.instance.user.rank];

            overlayView.rankText.text = PlayerManager.instance.user.rank.ToString();
            overlayView.usernameText.text = PlayerManager.instance.user.username;

            overlayView.xpBarImage.fillAmount = PlayerManager.instance.user.xp / PlayerManager.instance.user.xpToRank;

            overlayView.xpText.text = PlayerManager.instance.user.xp + " / " + PlayerManager.instance.user.xpToRank;
        }

    }
}