using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;
using PirateGame.Character;
using PirateGame.Managers;
using PirateGame.ScriptableObjects;
using PirateGame.UI.Views;
using UnityEngine;

namespace PirateGame.UI.Controllers
{
    public class CharacterEditController : Controller
    {

        public CharacterSettingsScriptableObject characterSettings;

        public CharacterEditView editView;

        public enum Selected { body, clothing, weapons, consumables, emotes };
        public Selected selected;
        private Selected lastSelected;

        public Character.Character character;

        private void OnEnable()
        {
            characterSettings = Resources.Load<CharacterSettingsScriptableObject>(CharacterSettingsScriptableObject.location);
            if(characterSettings == null)
            {
                Debug.Log("Character Settings not found! " + CharacterSettingsScriptableObject.location);
                return;
            }

            character.SetCharacter(PlayerManager.instance.user.character);

            Refresh();
        }

        private void Update()
        {
            if(lastSelected != selected)
            {
                lastSelected = selected;
                Refresh();
            }
        }

        public void Refresh()
        {
            StartCoroutine(WaitFrames());
        }

        IEnumerator WaitFrames()
        {
            yield return null;
            yield return null;
            switch (selected)
            {
                case Selected.body:
                    RefreshBody();
                    break;
                case Selected.clothing:
                    RefreshClothing();
                    break;
                case Selected.weapons:
                    RefreshWeapons();
                    break;
                case Selected.consumables:
                    RefreshConsumables();
                    break;
                case Selected.emotes:
                    RefreshEmotes();
                    break;
            }
        }

        private List<EditCharacterController> editCharacterControllers = new List<EditCharacterController>();
        void RefreshBody()
        {
            for(int i = 0; i < editCharacterControllers.Count; i++)
            {
                Destroy(editCharacterControllers[i].gameObject);
            }
            editCharacterControllers.Clear();

            for (int i = 0; i < characterSettings.bodyHolders.Length; i++)
            {
                SpawnEditCharacterController(null, characterSettings.bodyHolders[i].descriptionName);
                for(int x = 0; x < characterSettings.bodyHolders[i].bodyComponents.Length; x++)
                {
                    EditCharacterController controller = SpawnEditCharacterController(characterSettings.bodyHolders[i].bodyComponents[x]);
                    controller.value = PlayerManager.instance.user.GetCharacterSetting(characterSettings.bodyHolders[i].bodyComponents[x].description);
                    controller.maxValue = characterSettings.bodyHolders[i].bodyComponents[x].maxValue;
                    controller.minValue = characterSettings.bodyHolders[i].bodyComponents[x].minValue;
                    controller.changedDelegate += BodyChange;
                }
            }
        }

        void BodyChange(EditCharacterController controller)
        {
            StartCoroutine(BodyChangeWaitFrame(controller));
        }

        IEnumerator BodyChangeWaitFrame(EditCharacterController controller)
        {
            yield return null;
            float value = 0;
            if (controller.bodyComponent.mainType == BodyComponent.MainType.Selectable)
                value = controller.selected;
            else
                value = controller.value;

            character.ApplySetting(controller.bodyComponent, value);
            if (controller.bodyComponent.type == BodyComponent.Type.gender && controller.bodyComponent.mainType == BodyComponent.MainType.Selectable)
            {
                // Gender changed, refresh
                for (int i = 0; i < editCharacterControllers.Count; i++)
                {
                    // Seperator, return
                    if (editCharacterControllers[i].bodyComponent == null)
                        continue;

                    editCharacterControllers[i].gameObject.SetActive(true);
                    if (editCharacterControllers[i].bodyComponent.genderRestriction == BodyComponent.GenderRestriction.maleOnly)
                    {
                        if (selected != 0)
                            editCharacterControllers[i].gameObject.SetActive(false);
                    }
                    else if (editCharacterControllers[i].bodyComponent.genderRestriction == BodyComponent.GenderRestriction.femaleOnly)
                    {
                        if (selected == 0)
                            editCharacterControllers[i].gameObject.SetActive(false);
                    }

                    if (editCharacterControllers[i].bodyComponent.type != BodyComponent.Type.gender)
                        editCharacterControllers[i].changedDelegate.Invoke(editCharacterControllers[i]);
                }
            }
        }

        private EditCharacterController SpawnEditCharacterController(BodyComponent bodyComponent, string description = "")
        {
            GameObject newCharacterController = Instantiate(editView.bodySelectionPrefab);
            newCharacterController.transform.SetParent(editView.bodySelectionHolder);
            newCharacterController.transform.localScale = Vector3.one;
            newCharacterController.transform.localPosition = Vector3.one;
            newCharacterController.transform.rotation = Quaternion.identity;

            EditCharacterController controller = newCharacterController.GetComponent<EditCharacterController>();
            if(string.IsNullOrEmpty(description))
                controller.bodyComponent = bodyComponent;
            else
            {
                controller.bodyComponent = null;
                controller.description = description;
            }
            editCharacterControllers.Add(controller);

            return controller;
        }

        void RefreshClothing()
        {

        }

        void RefreshWeapons()
        {

        }

        void RefreshConsumables()
        {

        }

        void RefreshEmotes()
        {

        }

        #region Show
        public void ShowBody()
        {
            selected = Selected.body;
        }

        public void ShowClothing()
        {
            selected = Selected.clothing;
        }

        public void ShowWeapons()
        {
            selected = Selected.weapons;
        }

        public void ShowConsumables()
        {
            selected = Selected.consumables;
        }

        public void ShowEmotes()
        {
            selected = Selected.emotes;
        }

        public void Save()
        {
            PlayerManager.instance.SetValue("Character", JsonConvert.SerializeObject(character.characterSet));
            UIManager.instance.ScreenSwitch("Menu");
        }
        #endregion

    }
}
