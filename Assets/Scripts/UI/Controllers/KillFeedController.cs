using PirateGame.UI.Views;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PirateGame.UI.Controllers
{
    public class KillFeedController : Controller
    {

        public KillFeedView killFeedView;

        public string nameA;
        public string nameB;

        public Sprite icon;

        void Awake()
        {
            Update();
        }

        void Update()
        {
            killFeedView.icon.sprite = icon;
            killFeedView.nameAText.text = nameA;
            killFeedView.nameBText.text = nameB;
        }

    }
}