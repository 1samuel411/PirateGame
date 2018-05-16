using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

namespace PirateGame.ScriptableObjects
{
    public class CharacterSettingsScriptableObject : ScriptableObject
    {
        public static string location = "CharacterSettings";
	    public static string resourcesLocation = "Assets/Resources/CharacterSettings.asset";

        public BodyHolder[] bodyHolders;

        public Clothing[] clothing;
    }
}

[System.Serializable]
public class Clothing
{
    public string name;
    public Sprite sprite;
    public UMA.CharacterSystem.UMAWardrobeRecipe recipeMale;
    public UMA.CharacterSystem.UMAWardrobeRecipe recipeFemale;
}

[System.Serializable]
public class BodyComponent
{
    public string description;

    public enum MainType { Slider, Selectable };
    public MainType mainType;

    public enum Type { color, wardrobeRecipe, gender }

    public enum GenderRestriction { none, maleOnly, femaleOnly };
    public GenderRestriction genderRestriction;

    [ShowIf("mainType", MainType.Slider)]
    public float defaultValue = 0.5f;
    [ShowIf("mainType", MainType.Slider)]
    public float maxValue = 0.6f;
    [ShowIf("mainType", MainType.Slider)]
    public float minValue = 0.4f;

    [ShowIf("mainType", MainType.Selectable)]
    public int defaultSelection = 0;

    [ShowIf("mainType", MainType.Selectable)]
    public Type type;

    [ShowIf("mainType", MainType.Selectable)]
    [ShowIf("type", Type.wardrobeRecipe)]
    public BodyPart[] bodyParts;

    [ShowIf("mainType", MainType.Selectable)]
    [ShowIf("type", Type.color)]
    public Color[] colors;
}

[System.Serializable]
public class BodyPart
{
    public string name;
    public UMA.CharacterSystem.UMAWardrobeRecipe recipeMale;
    public UMA.CharacterSystem.UMAWardrobeRecipe recipeFemale;

    public UMA.CharacterSystem.UMAWardrobeRecipe GetRecipe(int gender)
    {
        if (gender == 0)
            return recipeMale;
        else
            return recipeFemale;
    }
}

[System.Serializable]
public class BodyHolder
{
    public string descriptionName;
    public BodyComponent[] bodyComponents;
}