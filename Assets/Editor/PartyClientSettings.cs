#if UNITY_EDITOR
using System;
using System.Collections;
using System.Collections.Generic;
using SNetwork;
using SNetwork.Client;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

public class PartyClientSettings : EditorWindow
{

    [MenuItem("Tools/SNetworking/Party Client Settings")]
    public static void Init()
    {
        PartyClientSettingsScriptableObject asset = null;
        asset = AssetDatabase.LoadAssetAtPath<PartyClientSettingsScriptableObject>(PartyClientSettingsScriptableObject.resourcesLocation);

        if (asset == null)
        {
            asset = ScriptableObject.CreateInstance<PartyClientSettingsScriptableObject>();
            AssetDatabase.CreateAsset(asset, PartyClientSettingsScriptableObject.resourcesLocation);
            AssetDatabase.SaveAssets();
        }

        EditorUtility.FocusProjectWindow();

        Selection.activeObject = asset;
    }
}
#endif