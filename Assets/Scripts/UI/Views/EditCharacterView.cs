using UnityEngine;
using UnityEngine.UI;

namespace PirateGame.UI.Views
{
    [System.Serializable]
    public class EditCharacterView : View
    {

        public GameObject sliderGameObject;
        public Text sliderNameText;
        public Slider slider;

        public GameObject selectGameObject;
        public Text selectTextName;
        public Text selectTextDesc;

        public GameObject seperatorGameObject;
        public Text seperatorText;

    }
}