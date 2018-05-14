using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PirateGame.UI.Views
{
    [System.Serializable]
    public class CharacterEditView : View
    {

        public GameObject bodySelection;
        public Transform bodySelectionHolder;
        public GameObject bodySelectionPrefab;

        public GameObject slotsSelection;
        public Transform slotsSelectionHolder;
        public GameObject slotsSelectionPrefab;

    }
}