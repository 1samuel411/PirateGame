#if UNITY_EDITOR
using System;
using System.Collections;
using System.Collections.Generic;
using PirateGame.ScriptableObjects;
using SNetwork;
using SNetwork.Client;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

public class ScriptableObjects : EditorWindow
{

    [MenuItem("Tools/ScriptableObjects/Character Settings")]
    public static void CharacterSettings()
    {
        CharacterSettingsScriptableObject asset = null;
        asset = AssetDatabase.LoadAssetAtPath<CharacterSettingsScriptableObject>(CharacterSettingsScriptableObject.resourcesLocation);

        if (asset == null)
        {
            asset = ScriptableObject.CreateInstance<CharacterSettingsScriptableObject>();
            AssetDatabase.CreateAsset(asset, CharacterSettingsScriptableObject.resourcesLocation);
            AssetDatabase.SaveAssets();
        }

        EditorUtility.FocusProjectWindow();

        Selection.activeObject = asset;
    }

    [MenuItem("Tools/ScriptableObjects/Weapon Settings")]
    public static void WeaponSettings()
    {
        WeaponsScriptableObject asset = null;
        asset = AssetDatabase.LoadAssetAtPath<WeaponsScriptableObject>(WeaponsScriptableObject.resourcesLocation);

        if (asset == null)
        {
            asset = ScriptableObject.CreateInstance<WeaponsScriptableObject>();
            AssetDatabase.CreateAsset(asset, WeaponsScriptableObject.resourcesLocation);
            AssetDatabase.SaveAssets();
        }

        EditorUtility.FocusProjectWindow();

        Selection.activeObject = asset;
    }
}
#endif