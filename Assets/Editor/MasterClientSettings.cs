#if UNITY_EDITOR
using System;
using System.Collections;
using System.Collections.Generic;
using SNetwork;
using SNetwork.Client;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

public class MasterClientSettings : EditorWindow
{

    [MenuItem("Tools/SNetworking/Master Client Settings")]
    public static void Init()
    {
        MasterClientSettingsScriptableObject asset = null;
        asset = AssetDatabase.LoadAssetAtPath<MasterClientSettingsScriptableObject>(MasterClientSettingsScriptableObject.resourcesLocation);

        if (asset == null)
        {
            asset = ScriptableObject.CreateInstance<MasterClientSettingsScriptableObject>();
            AssetDatabase.CreateAsset(asset, MasterClientSettingsScriptableObject.resourcesLocation);
            AssetDatabase.SaveAssets();
        }

        EditorUtility.FocusProjectWindow();

        Selection.activeObject = asset;
    }
}
#endif