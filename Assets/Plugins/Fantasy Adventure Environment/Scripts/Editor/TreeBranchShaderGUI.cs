// Fantasy Adventure Environment
// Copyright Staggart Creations
// staggart.xyz

using UnityEngine;
using UnityEditor;
using System.Collections;

namespace FAE
{
    public class TreeBranchShaderGUI : ShaderGUI
    {

        MaterialProperty _MaskClipValue;

        //Main maps
        MaterialProperty _MainTex;
        MaterialProperty _BumpMap;

        //Color
        MaterialProperty _VariationColor;
        MaterialProperty _AmbientOcclusion;
        MaterialProperty _TransmissionColor;
        MaterialProperty _GradientBrightness;
        MaterialProperty _Smoothness;
        MaterialProperty _FlatLighting;

        //Animation
        MaterialProperty _WindWeight;
        MaterialProperty _WindAmplitudeMultiplier;

        MaterialEditor m_MaterialEditor;

        //Meta
        bool showHelp;
        bool showHelpColor;
        bool showHelpAnimation;

        bool hasWindController;
        WindController windController;

        GUIContent mainTexName = new GUIContent("Diffuse", "Diffuse (RGB) and Transparency (A)");
        GUIContent normalMapName = new GUIContent("Normal Map");
        private bool visualizeVectors;

        public override void OnGUI(MaterialEditor materialEditor, MaterialProperty[] props)
        {
            if (windController == null) LocateWindController();
            this.FindProperties(props);

            //Receive
            visualizeVectors = WindController._visualizeVectors;

            this.m_MaterialEditor = materialEditor;

            //Style similar to Standard shader
            m_MaterialEditor.SetDefaultGUIWidths();
            m_MaterialEditor.UseDefaultMargins();
            EditorGUIUtility.labelWidth = 0f;

            EditorGUI.BeginChangeCheck();

            //Draw fields
            DoHeader();

            DoMapsArea();
            DoColorArea();
            DoAnimationArea();

            if (EditorGUI.EndChangeCheck())
            {
                //Send
                WindController.VisualizeVectors(visualizeVectors);
            }

#if UNITY_5_5_OR_NEWER
        m_MaterialEditor.RenderQueueField();
#endif

#if UNITY_5_6_OR_NEWER
            m_MaterialEditor.EnableInstancingField();
#endif

            DoFooter();

        }

        void DoHeader()
        {
            EditorGUILayout.BeginHorizontal();
            showHelp = GUILayout.Toggle(showHelp, "Toggle help", "Button");
            GUILayout.Label("FAE Tree Branch Shader", new GUIStyle(EditorStyles.centeredGreyMiniLabel)
            {
                alignment = TextAnchor.MiddleCenter,
                wordWrap = true,
                fontSize = 12
            });
            EditorGUILayout.EndHorizontal();
            if (showHelp) EditorGUILayout.HelpBox("Please bear in mind, when using custom meshes, that most shader feature require the ambient occlusion to be baked into the vertex colors.", MessageType.Warning);
        }

        void DoMapsArea()
        {
            GUILayout.Label("Main maps", EditorStyles.boldLabel);
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.PrefixLabel(_MaskClipValue.displayName);
            _MaskClipValue.floatValue = EditorGUILayout.Slider(_MaskClipValue.floatValue, 0f, 1f);
            EditorGUILayout.EndHorizontal();
            this.m_MaterialEditor.TexturePropertySingleLine(mainTexName, this._MainTex);
            this.m_MaterialEditor.TexturePropertySingleLine(normalMapName, this._BumpMap);

            EditorGUILayout.Space();
        }

        void DoColorArea()
        {
            EditorGUILayout.BeginHorizontal();
            showHelpColor = GUILayout.Toggle(showHelpColor, "?", "Button", GUILayout.Width(25f)); GUILayout.Label("Color", EditorStyles.boldLabel);
            EditorGUILayout.EndHorizontal();

            m_MaterialEditor.ShaderProperty(_VariationColor, _VariationColor.displayName);
            if (showHelpColor) EditorGUILayout.HelpBox("Uses the object's world-space position to add a color variation. This effect is controlled through the alpha channel.\n\n Note: Does not work with meshes that are batched or combined", MessageType.None);
            m_MaterialEditor.ShaderProperty(_AmbientOcclusion, _AmbientOcclusion.displayName);
            if (showHelpColor) EditorGUILayout.HelpBox("Darkens the areas of the mesh where vertex colors are applied", MessageType.None);
            m_MaterialEditor.ShaderProperty(_TransmissionColor, _TransmissionColor.displayName);
            if (showHelpColor) EditorGUILayout.HelpBox("Simulates light passing through the material. This effect is controlled through the alpha channel.", MessageType.None);
            m_MaterialEditor.ShaderProperty(_GradientBrightness, _GradientBrightness.displayName);
            if (showHelpColor) EditorGUILayout.HelpBox("Adds a gradient to the branch mesh from bottom to top. This information is stored in the vertex color alpha channel.\n\nWithout this information, the parameter will have no effect.", MessageType.None);
            m_MaterialEditor.ShaderProperty(_Smoothness, _Smoothness.displayName);
            if (showHelpColor) EditorGUILayout.HelpBox("Determines the shininess of the material", MessageType.None);
            m_MaterialEditor.ShaderProperty(_FlatLighting, _FlatLighting.displayName);
            if (showHelpColor) EditorGUILayout.HelpBox("A value of 1 makes the mesh normals point upwards. For some trees this is necessary to achieve the best visual result.", MessageType.None);

            EditorGUILayout.Space();
        }

        void DoAnimationArea()
        {
            EditorGUILayout.BeginHorizontal();
            showHelpAnimation = GUILayout.Toggle(showHelpAnimation, "?", "Button", GUILayout.Width(25f)); GUILayout.Label("Wind animation", EditorStyles.boldLabel);
            EditorGUILayout.EndHorizontal();

            visualizeVectors = EditorGUILayout.Toggle("Visualize wind", visualizeVectors);

            if (!hasWindController)
            {
                EditorGUILayout.HelpBox("No \"WindController\" component was found in your scene. Please add this script to an empty GameObject\n\nA prefab can be found in the Prefabs/Effects folder.", MessageType.Warning);
                EditorGUI.BeginDisabledGroup(true);

            }

            if (showHelpAnimation) EditorGUILayout.HelpBox("Toggle a visualisation of the wind vectors on all the objects that use FAE shaders featuring wind.\n\nThis allows you to more clearly see the effects of the settings.", MessageType.None);

            m_MaterialEditor.ShaderProperty(_WindWeight, _WindWeight.displayName);
            if (showHelpAnimation) EditorGUILayout.HelpBox("Determines how much influence the wind has on the branches", MessageType.None);

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField(_WindAmplitudeMultiplier.displayName);
            _WindAmplitudeMultiplier.floatValue = EditorGUILayout.FloatField(_WindAmplitudeMultiplier.floatValue, GUILayout.Width(65f));
            EditorGUILayout.EndHorizontal();
            //m_MaterialEditor.ShaderProperty(_WindAmplitudeMultiplier, _WindAmplitudeMultiplier.displayName);
            if (showHelpAnimation) EditorGUILayout.HelpBox("Multiply the wind amplitude for this material. Essentally this is the size of the wind waves.", MessageType.None);

            EditorGUI.EndDisabledGroup();
            EditorGUILayout.Space();
        }

        void LocateWindController()
        {
            //Debug.Log("Searching scene for WindController script");
            windController = GameObject.FindObjectOfType<WindController>();
            hasWindController = (windController) ? true : false;
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

        public void FindProperties(MaterialProperty[] props)
        {
            //Rendering
            _MaskClipValue = FindProperty("_Cutoff", props);

            //Main maps
            _MainTex = FindProperty("_MainTex", props);
            _BumpMap = FindProperty("_BumpMap", props);

            //Color
            _VariationColor = FindProperty("_VariationColor", props);
            _AmbientOcclusion = FindProperty("_AmbientOcclusion", props);
            _TransmissionColor = FindProperty("_TransmissionColor", props);
            _GradientBrightness = FindProperty("_GradientBrightness", props);
            _Smoothness = FindProperty("_Smoothness", props);
            _FlatLighting = FindProperty("_FlatLighting", props);

            //Animation
            _WindWeight = FindProperty("_WindWeight", props);
            _WindAmplitudeMultiplier = FindProperty("_WindAmplitudeMultiplier", props);

        }

    }
}
