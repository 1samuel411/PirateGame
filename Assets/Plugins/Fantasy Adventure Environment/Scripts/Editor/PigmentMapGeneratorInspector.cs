// Fantasy Adventure Environment
// Copyright Staggart Creations
// staggart.xyz

using UnityEngine;
using System.Collections;
using UnityEditor;
using UnityEditor.Callbacks;
using FAE;

namespace FAE
{
    [CustomEditor(typeof(PigmentMapGenerator))]
    public class PigmentMapGeneratorInspector : Editor
    {
        PigmentMapGenerator pmg;

        new SerializedObject serializedObject;

        SerializedProperty useCustomPigmentMap;
        SerializedProperty inputPigmentMap;

        // Use this for initialization
        void OnEnable()
        {
            pmg = (PigmentMapGenerator)target;

            GetProperties();
        }

        private void GetProperties()
        {
            serializedObject = new SerializedObject(pmg);

            useCustomPigmentMap = serializedObject.FindProperty("useCustomPigmentMap");
            inputPigmentMap = serializedObject.FindProperty("inputPigmentMap");
        }

        private string terrainInfo;
        private bool showHelp;

        // Update is called once per frame
        public override void OnInspectorGUI()
        {
            DoHeader();

            if(pmg.workflow == TerrainUVUtil.Workflow.None)
            {
                EditorGUILayout.HelpBox("Current object is neither a terrain or a mesh!", MessageType.Error);
                return;
            }

            EditorGUI.BeginChangeCheck();

            serializedObject.Update();

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.PrefixLabel("Custom pigment map");
            EditorGUILayout.PropertyField(useCustomPigmentMap, new GUIContent(""));
            EditorGUILayout.EndHorizontal();
            if(showHelp) EditorGUILayout.HelpBox("This option allows you to assign a custom pigment map, rather than rendering one.", MessageType.Info);

            EditorGUILayout.Space();

            if (useCustomPigmentMap.boolValue)
            {
                EditorGUILayout.PropertyField(inputPigmentMap, new GUIContent("Input"));

                if (showHelp) EditorGUILayout.HelpBox("Grass heightmap should be stored in the alpha channel.", MessageType.Info);


                if (pmg.inputPigmentMap)
                {
                    //Check if input heightmap is readable
                    try
                    {
                        pmg.inputPigmentMap.GetPixel(0, 0);
                    }
                    catch (UnityException e)
                    {
                        if (e.Message.StartsWith("Texture '" + pmg.inputPigmentMap.name + "' is not readable"))
                        {

                            EditorGUILayout.HelpBox("Please enable Read/Write on texture \"" + pmg.inputPigmentMap.name + "\"\n\nWith the texture selected, choose the \"Advanced\" texture type to show this option.", MessageType.Error);
                        }
                    }
                }

            }
            else
            {

                if (pmg.workflow == TerrainUVUtil.Workflow.Terrain && pmg.terrain.terrainData.splatPrototypes.Length == 0)
                {
                    EditorGUILayout.HelpBox("Assign at least one texture to your terrain", MessageType.Error);
                    return;
                }

                if (pmg.workflow == TerrainUVUtil.Workflow.Terrain)
                {
                    EditorGUILayout.BeginVertical(EditorStyles.helpBox);

                    EditorGUILayout.LabelField("Grass heightmap", EditorStyles.toolbarButton);
                    EditorGUILayout.Space();

                    if(pmg.terrain.terrainData.alphamapResolution >= 2048)
                    {
                        EditorGUILayout.HelpBox("Your splatmap size is 2048px or larger, this can cause severe slowdowns", MessageType.Warning);
                    }

                    //Functionality to be implemented in a later update
                    pmg.heightmapChannel = (PigmentMapGenerator.HeightmapChannel)EditorGUILayout.EnumPopup("Height source material", pmg.heightmapChannel);
                    if(showHelp) EditorGUILayout.HelpBox("This is the texture whose painted weight will determine the grass height \n\nThe effect can be controlled through the \"Heightmap influence\" parameter on the FAE/Grass shader", MessageType.None);

                    EditorGUILayout.Space();
                    EditorGUILayout.EndVertical();

                    EditorGUILayout.Space();
                }
                else if (pmg.workflow == TerrainUVUtil.Workflow.Mesh)
                {
                    EditorGUILayout.BeginVertical(EditorStyles.helpBox);

                    //Field to assign heightmap texture
                    EditorGUILayout.LabelField("Input grass heightmap (optional)", EditorStyles.toolbarButton);
                    EditorGUILayout.Space();

                    pmg.inputHeightmap = EditorGUILayout.ObjectField("Heightmap", pmg.inputHeightmap, typeof(Texture2D), false) as Texture2D;

                    if (showHelp) EditorGUILayout.HelpBox("This information is used in the \"FAE/Grass\" shader to make the grass shorter where desired", MessageType.Info);

                    if (pmg.inputHeightmap)
                    {
                        //Check if input heightmap is readable
                        try
                        {
                            pmg.inputHeightmap.GetPixel(0, 0);
                        }
                        catch (UnityException e)
                        {
                            if (e.Message.StartsWith("Texture '" + pmg.inputHeightmap.name + "' is not readable"))
                            {

                                EditorGUILayout.HelpBox("Please enable Read/Write on texture \"" + pmg.inputHeightmap.name + "\"\n\nWith the texture selected, choose the \"Advanced\" texture type to show this option.", MessageType.Error);
                            }
                        }
                    }
                    //Channel dropdown?
                    EditorGUILayout.Space();
                    EditorGUILayout.EndVertical();

                    EditorGUILayout.Space();
                }

                terrainInfo = string.Format("Terrain size: {0}x{1} \nTerrain position: X: {2} Z: {3}", pmg.targetSize.x, pmg.targetSize.z, pmg.targetPosition.x, pmg.targetPosition.z);
                terrainInfo.Replace("\\n", "\n"); //Break lines

                //EditorGUILayout.HelpBox(terrainInfo, MessageType.Info);

                if (pmg.workflow == TerrainUVUtil.Workflow.Mesh)
                {
                    EditorGUILayout.BeginVertical(EditorStyles.helpBox);

                    EditorGUILayout.LabelField("Texture transformation", EditorStyles.toolbarButton);
                    EditorGUILayout.Space();
                    if (showHelp) EditorGUILayout.HelpBox("The UV's of your mesh terrain may differ, so these options allow you to compensate for this.", MessageType.Info);

                    pmg.flipHortizontally = EditorGUILayout.Toggle("Flip horizontally", pmg.flipHortizontally);
                    pmg.flipVertically = EditorGUILayout.Toggle("Flip vertically", pmg.flipVertically);
                    pmg.textureRotation = (PigmentMapGenerator.TextureRotation)EditorGUILayout.EnumPopup("Rotation", pmg.textureRotation);

                    EditorGUILayout.Space();
                    EditorGUILayout.EndVertical();

                    EditorGUILayout.Space();
                }

                //Button
                EditorGUILayout.BeginVertical(EditorStyles.helpBox);

                EditorGUILayout.LabelField("Renderer", EditorStyles.toolbarButton);
                EditorGUILayout.Space();

                pmg.useAlternativeRenderer = EditorGUILayout.ToggleLeft("Use alternative renderer", pmg.useAlternativeRenderer);

                if (showHelp) EditorGUILayout.HelpBox("Some third-party terrain shaders require you to use this, otherwise the result may be black.", MessageType.Info);

                if (pmg.useAlternativeRenderer) pmg.renderLightBrightness = EditorGUILayout.Slider("Brightness adjustment", pmg.renderLightBrightness, 0f, 1f);
                if (showHelp) EditorGUILayout.HelpBox("To compensate for any shader variations on the terrain, you can use this to increase the brightness of the pigment map, in case it turns out too dark.", MessageType.Info);

                EditorGUILayout.Space();
                EditorGUILayout.EndVertical();

            }//if custom pigment map

            EditorGUILayout.Space();

            string buttonLabel = (useCustomPigmentMap.boolValue) ? "Assign" : "Generate";

            if (GUILayout.Button(buttonLabel, GUILayout.Height(40f)))
            {
                pmg.Generate();
            }
            EditorGUILayout.Space();

            if (!useCustomPigmentMap.boolValue)
            {
                //Pigment map preview
                EditorGUILayout.BeginHorizontal();
                if (pmg.pigmentMap)
                {
                    EditorGUILayout.LabelField(string.Format("Output pigment map ({0}x{0})", pmg.pigmentMap.height), EditorStyles.boldLabel);
                }
                EditorGUILayout.EndHorizontal();

                if (pmg.pigmentMap)
                {
                    Texture nothing = EditorGUILayout.ObjectField(pmg.pigmentMap, typeof(Texture), false, GUILayout.Width(150f), GUILayout.Height(150f)) as Texture;
                    nothing.name = "Pigmentmap";
                }

                if (pmg.workflow == TerrainUVUtil.Workflow.Terrain)
                {
                    if (pmg.hasTerrainData)
                    {
                        EditorGUILayout.LabelField("The output texture file is stored next to the TerrainData asset", EditorStyles.helpBox);
                    }
                    else
                    {
                        EditorGUILayout.LabelField("The output texture file is stored next to the scene file", EditorStyles.helpBox);
                    }
                }
                else if (pmg.workflow == TerrainUVUtil.Workflow.Mesh)
                {
                    EditorGUILayout.LabelField("The output texture file is stored next to the material file", EditorStyles.helpBox);
                }
            }


            DoFooter();

            //Placeholder for realtime updates functionality
            if (GUI.changed)
            {
                Apply();
            }

            serializedObject.ApplyModifiedProperties();

            if (GUI.changed || EditorGUI.EndChangeCheck())
            {
                EditorUtility.SetDirty((PigmentMapGenerator)target);
                EditorUtility.SetDirty(this);
                Apply();
            }
        }

        private void DoHeader()
        {

            EditorGUILayout.BeginHorizontal();
            showHelp = GUILayout.Toggle(showHelp, "Toggle help", "Button");
            GUILayout.Label("FAE Pigmentmap Generator", new GUIStyle(EditorStyles.centeredGreyMiniLabel)
            {
                alignment = TextAnchor.MiddleCenter,
                wordWrap = true,
                fontSize = 12
            });
            EditorGUILayout.EndHorizontal();
            if (showHelp) EditorGUILayout.HelpBox("This renders a color map from your terrain, which is used by the \"FAE/Grass\" shader to blend the grass color with the terrain.\n\nIf you'd like some objects to be included, like cliffs, parent them under your terrain object.", MessageType.Info);

            EditorGUILayout.Space();
        }

        void DoFooter()
        {
            GUILayout.Label("- Staggart Creations -", new GUIStyle(EditorStyles.centeredGreyMiniLabel)
            {
                alignment = TextAnchor.MiddleCenter,
                wordWrap = true,
                fontSize = 12
            });
        }

        private void Apply()
        {

        }

    }
}
