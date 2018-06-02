using PirateGame.Managers;
using PirateGame.UI.Views;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PirateGame.UI.Controllers
{
    public class InGameOverlayController : Controller
    {

        public InGameOverlayView overlayView;

        public Sprite speakingIcon;
        public Sprite muteIcon;
        public Sprite notSpeakingIcon;


        private void Awake()
        {

        }

        private void Update()
        {
            UpdateUI();
        }

        void UpdateUI()
        {
            UpdateMinimapUI();

            UpdateCompassUI();

            UpdateResourcesUI();

            UpdateWeaponUI();
        }

        void UpdateMinimapUI()
        {
            overlayView.minimapMarker.localEulerAngles = new Vector3(0, 0, -PlayerManager.instance.playablePlayer.transform.eulerAngles.y);
        }

        void UpdateCompassUI()
        {
            overlayView.compassRawImage.uvRect = new Rect(CameraManager.instance.cameraObject.transform.eulerAngles.y / 360f, 0, 1, 1);
        }

        void UpdateResourcesUI()
        {

        }

        void UpdateWeaponUI()
        {
            if (PlayerManager.instance.playerWeaponManager == null)
                return;

            if(PlayerManager.instance.playerWeaponManager.curWeapon.name == "")
            {
                overlayView.equipedWeaponHolder.SetActive(false);
                overlayView.equipedWeaponImage.color = new Color(1, 1, 1, 0);
            }
            else
            {
                if(PlayerManager.instance.playerWeaponManager.curWeapon.gun == true)
                {
                    overlayView.equipedWeaponText.gameObject.SetActive(true);
                }
                else
                {
                    overlayView.equipedWeaponText.gameObject.SetActive(false);
                }

                overlayView.equipedWeaponHolder.SetActive(true);

                overlayView.equipedWeaponImage.sprite = PlayerManager.instance.playerWeaponManager.curWeapon.icon;
                overlayView.equipedWeaponImage.color = new Color(1, 1, 1, 1);
            }

            overlayView.defaultWeaponSelectedImage.gameObject.SetActive(false);
            if (PlayerManager.instance.playerWeaponManager.defaultWeapon.name != "" && PlayerManager.instance.playerWeaponManager.curWeapon.name == PlayerManager.instance.playerWeaponManager.defaultWeapon.name)
                overlayView.defaultWeaponSelectedImage.gameObject.SetActive(true);

            overlayView.weaponOneSelectedImage.gameObject.SetActive(false);
            if (PlayerManager.instance.playerWeaponManager.weaponOne.name != "" && PlayerManager.instance.playerWeaponManager.curWeapon.name == PlayerManager.instance.playerWeaponManager.weaponOne.name)
                overlayView.weaponOneSelectedImage.gameObject.SetActive(true);

            overlayView.weaponTwoSelectedImage.gameObject.SetActive(false);
            if (PlayerManager.instance.playerWeaponManager.weaponTwo.name != "" && PlayerManager.instance.playerWeaponManager.curWeapon.name == PlayerManager.instance.playerWeaponManager.weaponTwo.name)
                overlayView.weaponTwoSelectedImage.gameObject.SetActive(true);

            overlayView.weaponThreeSelectedImage.gameObject.SetActive(false);
            if (PlayerManager.instance.playerWeaponManager.weaponThree.name != "" && PlayerManager.instance.playerWeaponManager.curWeapon.name == PlayerManager.instance.playerWeaponManager.weaponThree.name)
                overlayView.weaponThreeSelectedImage.gameObject.SetActive(true);

            if (PlayerManager.instance.playerWeaponManager.defaultWeapon.name == "")
            {
                //overlayView.defaultWeaponHolder.gameObject.SetActive(false);
                overlayView.defaultWeaponImage.color = new Color(1, 1, 1, 0);
            }
            else
            {
                overlayView.defaultWeaponHolder.gameObject.SetActive(true);
                overlayView.defaultWeaponAmmoText.text = "";
                overlayView.defaultWeaponImage.sprite = PlayerManager.instance.playerWeaponManager.defaultWeapon.icon;
                overlayView.defaultWeaponImage.color = new Color(1, 1, 1, 1);
                overlayView.defaultWeaponText.text = "Q";
            }

            if (PlayerManager.instance.playerWeaponManager.weaponOne.name == "")
            {
                //overlayView.weaponOneHolder.gameObject.SetActive(false);
                overlayView.weaponOneImage.color = new Color(1, 1, 1, 0);
            }
            else
            {
                overlayView.weaponOneHolder.gameObject.SetActive(true);
                overlayView.weaponOneAmmoText.text = "";
                overlayView.weaponOneImage.sprite = PlayerManager.instance.playerWeaponManager.weaponOne.icon;
                overlayView.weaponOneImage.color = new Color(1, 1, 1, 1);
                overlayView.weaponOneText.text = "1";
            }

            if (PlayerManager.instance.playerWeaponManager.weaponTwo.name == "")
            {
                //overlayView.weaponTwoHolder.gameObject.SetActive(false);
                overlayView.weaponTwoImage.color = new Color(1, 1, 1, 0);
            }
            else
            {
                overlayView.weaponTwoHolder.gameObject.SetActive(true);
                overlayView.weaponTwoAmmoText.text = "";
                overlayView.weaponTwoImage.sprite = PlayerManager.instance.playerWeaponManager.weaponTwo.icon;
                overlayView.weaponTwoImage.color = new Color(1, 1, 1, 1);
                overlayView.weaponTwoText.text = "2";
            }

            if (PlayerManager.instance.playerWeaponManager.weaponThree.name == "")
            {
                //overlayView.weaponThreeHolder.gameObject.SetActive(false);
                overlayView.weaponThreeImage.color = new Color(1, 1, 1, 0);
            }
            else
            {
                overlayView.weaponThreeHolder.gameObject.SetActive(true);
                overlayView.weaponThreeAmmoText.text = "";
                overlayView.weaponThreeImage.sprite = PlayerManager.instance.playerWeaponManager.weaponThree.icon;
                overlayView.weaponThreeImage.color = new Color(1, 1, 1, 1);
                overlayView.weaponThreeText.text = "3";
            }
        }

    }
}