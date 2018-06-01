using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using UnityEngine.UI;

namespace PirateGame.UI.Views
{
    [System.Serializable]
	public class InGameOverlayView : View 
	{

        [Title("Kill Feed")]
        public Transform killFeedHolder;
        public GameObject killFeedPrefab;

        [Title("Minimap")]
        public Transform minimapMarker;
        public Transform minimapMarker1;
        public Transform minimapMarker2;
        public Transform minimapMarker3;
        public Transform minimap;
        public Transform distanceToCircleMarker;
        public Transform circleMarker;

        public Text timeText;
        public Text userLeftText;

        [Title("Compass")]
        public RawImage compassRawImage;

        [Title("Resources")]
        public Text resourceOneText;
        public Text resourceTwoText;
        public Text resourceThreeText;

        [Title("Weapons")]
        public Text defaultWeaponText;
        public Text weaponOneText;
        public Text weaponTwoText;
        public Text weaponThreeText;

        public Text defaultWeaponAmmoText;
        public Text weaponOneAmmoText;
        public Text weaponTwoAmmoText;
        public Text weaponThreeAmmoText;

        public Image defaultWeaponImage;
        public Image weaponOneImage;
        public Image weaponTwoImage;
        public Image weaponThreeImage;

        public Image defaultWeaponSelectedImage;
        public Image weaponOneSelectedImage;
        public Image weaponTwoSelectedImage;
        public Image weaponThreeSelectedImage;

        public GameObject defaultWeaponHolder;
        public GameObject weaponOneHolder;
        public GameObject weaponTwoHolder;
        public GameObject weaponThreeHolder;

        public GameObject equipedWeaponHolder;
        public Image equipedWeaponImage;
        public Text equipedWeaponText;

        [Title("Elimination")]
        public Text eliminationText;

        [Title("Health")]
        public Image healthImage;
        public Text healthText;
        public Image shieldBuffImage;

        [Title("Party")]
        public PartyStruct partyMemberOne;
        public PartyStruct partyMemberTwo;
        public PartyStruct partyMemberThree;

        [System.Serializable]
        public struct PartyStruct
        {
            public GameObject holder;
            public Image speakerImage;
            public Image rankImage;
            public Text nameText;
            public Image healthImage;
        }

    }
}