using PirateGame.ScriptableObjects;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UMA.CharacterSystem;
using UMA;
using UnityEngine;
using Newtonsoft.Json;
using PirateGame.Managers;

namespace PirateGame.Character
{

    [System.Serializable]
    public class CharacterSettings
    {
        public BodySelection[] bodySelections;
    }

    [System.Serializable]
    public class BodySelection
    {
        public string name;
        public float value;

        public BodySelection(string name, float value)
        {
            this.name = name;
            this.value = value;
        }
    }

    public class Character : Base
    {
        public CharacterSettingsScriptableObject characterSettings;

        public UMAData umaData;
        public DynamicCharacterAvatar dynamicCharacterAvatar;

        public static string GetDefaultCharacter()
        {
            CharacterSettingsScriptableObject characterSettings = Resources.Load<CharacterSettingsScriptableObject>(CharacterSettingsScriptableObject.location);
            CharacterSettings characterSettingsDefault = new CharacterSettings();
            List<BodySelection> bodySelections = new List<BodySelection>();

            for(int i = 0; i < characterSettings.bodyHolders.Length; i++)
            {
                for(int x = 0; x < characterSettings.bodyHolders[i].bodyComponents.Length; x++)
                {
                    string description = characterSettings.bodyHolders[i].bodyComponents[x].description;
                    float value = 0;
                    if(characterSettings.bodyHolders[i].bodyComponents[x].mainType == BodyComponent.MainType.Selectable)
                        value = characterSettings.bodyHolders[i].bodyComponents[x].defaultSelection;
                    if (characterSettings.bodyHolders[i].bodyComponents[x].mainType == BodyComponent.MainType.Slider)
                        value = characterSettings.bodyHolders[i].bodyComponents[x].defaultValue;
                    bodySelections.Add(new BodySelection(description, value));
                }
            }

            characterSettingsDefault.bodySelections = bodySelections.ToArray();

            return JsonConvert.SerializeObject(characterSettingsDefault);
        }

        private void Awake()
        {
            characterSettings = Resources.Load<CharacterSettingsScriptableObject>(CharacterSettingsScriptableObject.location);
        }

        public void SetCharacter(CharacterSettings newSettings)
        {
            if (newSettings == null)
                return;
            if (!gameObject.activeSelf)
                return;

            Debug.Log("Setting Character: " + transform.name);

            StartCoroutine(SetCharacterCoroutine(newSettings));
        }
        IEnumerator SetCharacterCoroutine(CharacterSettings newSettings)
        {
            if (!gameObject.activeSelf)
                yield break;
            yield return null;
            if (!gameObject.activeSelf)
                yield break;

            characterSet = newSettings;

            yield return null;

            bool foundGender = false;
            for (int i = 0; i < characterSettings.bodyHolders.Length; i++)
            {
                for (int x = 0; x < characterSettings.bodyHolders[i].bodyComponents.Length; x++)
                {
                    for (int y = 0; y < newSettings.bodySelections.Length; y++)
                    {
                        // Found
                        if (newSettings.bodySelections[y].name == "Gender")
                        {
                            ApplySetting(characterSettings.bodyHolders[i].bodyComponents[x], newSettings.bodySelections[y].value);
                            yield return null;
                            foundGender = true;
                            break;
                        }
                    }
                    if (foundGender)
                        break;
                }
                if (foundGender)
                    break;
            }

            yield return null;
            yield return null;

            for (int i = 0; i < characterSettings.bodyHolders.Length; i++)
            {
                for (int x = 0; x < characterSettings.bodyHolders[i].bodyComponents.Length; x++)
                {
                    for (int y = 0; y < newSettings.bodySelections.Length; y++)
                    {
                        // Found
                        if (newSettings.bodySelections[y].name != "Gender" && newSettings.bodySelections[y].name == characterSettings.bodyHolders[i].bodyComponents[x].description)
                        {
                            ApplySetting(characterSettings.bodyHolders[i].bodyComponents[x], newSettings.bodySelections[y].value);
                            break;
                        }
                    }
                }
            }
        }

        public void ApplySetting(BodyComponent bodyComponent, float value)
        {
            // Gender
            if (bodyComponent.type == BodyComponent.Type.gender && bodyComponent.mainType == BodyComponent.MainType.Selectable)
            {
                ChangeGender((int)value);
            }
            // Color
            if (bodyComponent.type == BodyComponent.Type.color && bodyComponent.mainType == BodyComponent.MainType.Selectable)
            {
                ChangeSelectionColor(bodyComponent.description, (int)value);
            }
            // Wardrobe Hair/Eyes/Beard
            if (bodyComponent.type == BodyComponent.Type.wardrobeRecipe && bodyComponent.mainType == BodyComponent.MainType.Selectable)
            {
                ChangeSelectionWardrobe(bodyComponent.description, (int)value);
            }
            // Part
            if (bodyComponent.mainType == BodyComponent.MainType.Slider)
            {
                ChangePart(bodyComponent.description, value);
            }
        }


        public void ChangePart(string name, float value)
        {
            // Convert name
            string newName = name.Replace(" ", "");
            newName = name.First().ToString().ToLower() + newName.Substring(1);
            // Apply
            Dictionary<string, DnaSetter> dna = dynamicCharacterAvatar.GetDNA();
            if(dna.ContainsKey(newName) == false)
            {
                Debug.Log("Key missing: " + newName);
                return;
            }

            dna[newName].Set(value);
            dynamicCharacterAvatar.BuildCharacter();

            ChangeBodySelection(name, value);
        }

        public int gender;

        public void ChangeSelectionColor(string selection, int value)
        {
            if(selection == "Hair Color")
            {
                dynamicCharacterAvatar.SetColor("Hair", characterSettings.bodyHolders.FirstOrDefault(x => x.descriptionName == "Hair").bodyComponents.FirstOrDefault(x => x.description == selection).colors[value], new Color(), 0, true);
            }
            else if(selection == "Eye Color")
            {
                dynamicCharacterAvatar.SetColor("Eyes", characterSettings.bodyHolders.FirstOrDefault(x => x.descriptionName == "Character").bodyComponents.FirstOrDefault(x => x.description == selection).colors[value], new Color(), 0, true);
            }
            else if(selection == "Skin Color")
            {
                dynamicCharacterAvatar.SetColor("Skin", characterSettings.bodyHolders.FirstOrDefault(x => x.descriptionName == "Character").bodyComponents.FirstOrDefault(x => x.description == selection).colors[value], new Color(), 0, true);
            }

            ChangeBodySelection(selection, value);
        }

        public void ChangeSelectionWardrobe(string selection, int value)
        {
            if (selection == "Eyes")
            {
                dynamicCharacterAvatar.SetSlot(characterSettings.bodyHolders.FirstOrDefault(x => x.descriptionName == "Character").bodyComponents.FirstOrDefault(x => x.description == selection).bodyParts[value].GetRecipe(gender));
                dynamicCharacterAvatar.BuildCharacter();
            }
            else if(selection == "Hair")
            {
                dynamicCharacterAvatar.SetSlot(characterSettings.bodyHolders.FirstOrDefault(x => x.descriptionName == "Hair").bodyComponents.FirstOrDefault(x => x.description == selection).bodyParts[value].GetRecipe(gender));
                dynamicCharacterAvatar.BuildCharacter();
            }
            else if(selection == "Eyebrow")
            {
                dynamicCharacterAvatar.SetSlot(characterSettings.bodyHolders.FirstOrDefault(x => x.descriptionName == "Hair").bodyComponents.FirstOrDefault(x => x.description == selection).bodyParts[value].GetRecipe(gender));
                dynamicCharacterAvatar.BuildCharacter();
            }
            else if(selection == "Beard")
            {
                if (gender == 1)
                    return;

                dynamicCharacterAvatar.SetSlot(characterSettings.bodyHolders.FirstOrDefault(x => x.descriptionName == "Hair").bodyComponents.FirstOrDefault(x => x.description == selection).bodyParts[value].GetRecipe(gender));
                dynamicCharacterAvatar.BuildCharacter();
            }

            ChangeBodySelection(selection, value);
        }

        public void ChangeGender(int gender)
        {
            Debug.Log("Changing race");
            this.gender = gender;

            if (gender == 0)
                dynamicCharacterAvatar.ChangeRace("HumanMaleDCS");
            else
            {
                dynamicCharacterAvatar.ChangeRace("HumanFemaleHighPoly");
            }

            ChangeBodySelection("Gender", gender);
        }

        public CharacterSettings characterSet;
        public void ChangeBodySelection(string name, float newValue)
        {
            characterSet.bodySelections.FirstOrDefault(x => x.name == name).value = newValue;
        }
    }
}