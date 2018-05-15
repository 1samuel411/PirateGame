using PirateGame.ScriptableObjects;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UMA.CharacterSystem;
using UMA;
using UnityEngine;

namespace PirateGame.Character
{
    public class Character : Base
    {
        public CharacterSettingsScriptableObject characterSettings;

        public UMAData umaData;
        public DynamicCharacterAvatar dynamicCharacterAvatar;

        private void Awake()
        {
            characterSettings = Resources.Load<CharacterSettingsScriptableObject>(CharacterSettingsScriptableObject.location);
        }

        public void AcceptData()
        {

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
        }

        public int gender;

        public void ChangeSelectionColor(string selection, int value)
        {
            Debug.Log("Changing color: " + selection);
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
        }

        public void ChangeGender(int gender)
        {
            this.gender = gender;

            if (gender == 0)
                dynamicCharacterAvatar.ChangeRace("HumanMale");
            else
            {
                dynamicCharacterAvatar.ChangeRace("HumanFemaleHighPoly");
            }
        }

    }
}