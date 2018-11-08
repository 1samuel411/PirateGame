using UnityEngine;
using System.Collections;
using UnityEditor;

namespace FAE
{
    [CustomEditor(typeof(WindController))]
    public class WindControllerInspector : Editor
    {

        WindController wc;
        private bool showHelp = false;
        private GameObject selection;
        private bool visualizeVectors;

        new SerializedObject serializedObject;

        SerializedProperty windSubstance;
        SerializedProperty windVectors;


        SerializedProperty windDirection;
        SerializedProperty windSpeed;
        SerializedProperty windStrength;
        SerializedProperty windAmplitude;

        SerializedProperty trunkWindSpeed;
        SerializedProperty trunkWindWeight;
        SerializedProperty trunkWindSwinging;


#if UNITY_EDITOR
        void OnEnable()
        {
            selection = Selection.activeGameObject;
            if (selection)
            {
                wc = Selection.activeGameObject.GetComponent<WindController>();
            }

            serializedObject = new SerializedObject(wc);
            windSubstance = serializedObject.FindProperty("windSubstance");
            windVectors = serializedObject.FindProperty("windVectors");

            windDirection = serializedObject.FindProperty("windDirection");
            windSpeed = serializedObject.FindProperty("windSpeed");
            windStrength = serializedObject.FindProperty("windStrength");
            windAmplitude = serializedObject.FindProperty("windAmplitude");

            trunkWindSpeed = serializedObject.FindProperty("trunkWindSpeed");
            trunkWindWeight = serializedObject.FindProperty("trunkWindWeight");
            trunkWindSwinging = serializedObject.FindProperty("trunkWindSwinging");

        }

        public override void OnInspectorGUI()
        {

            EditorGUI.BeginChangeCheck();

            //Sync inspector var to static class var
            visualizeVectors = WindController._visualizeVectors;

            Undo.RecordObject(this, "Component");
            if (selection) Undo.RecordObject(selection, "WindController");

            serializedObject.Update();

            DrawFields();

            serializedObject.ApplyModifiedProperties();

            if (GUI.changed || EditorGUI.EndChangeCheck())
            {
                EditorUtility.SetDirty(selection);
                EditorUtility.SetDirty((WindController)target);
                EditorUtility.SetDirty(this);
                wc.Apply();

                //Set the static var
                WindController.VisualizeVectors(visualizeVectors);
            }

        }


        private void DrawFields()
        {
            DoHeader();

            EditorGUILayout.Space();

            if (!windSubstance.objectReferenceValue)
            {
                EditorGUILayout.HelpBox("The \"FAE_WindVectors\" Substance material could not be located in your project. Was it removed or renamed?", MessageType.Error);

                EditorGUILayout.LabelField("FAE_WindVectors substance");
                EditorGUILayout.PropertyField(windSubstance);
                return;
            }

            visualizeVectors = EditorGUILayout.Toggle("Visualize wind", visualizeVectors);
            if (showHelp) EditorGUILayout.HelpBox("Toggle a visualisation of the wind vectors on all the objects that use FAE shaders featuring wind.\n\nThis allows you to more clearly see the effects of the settings.", MessageType.Info);

            EditorGUI.BeginDisabledGroup(true);
            EditorGUILayout.PropertyField(windVectors);
            EditorGUI.EndDisabledGroup();


            EditorGUILayout.Space();

            EditorGUILayout.BeginVertical(EditorStyles.helpBox);

            EditorGUILayout.LabelField("Wind settings", EditorStyles.toolbarButton);
            EditorGUILayout.Space();

            EditorGUILayout.PropertyField(windDirection, new GUIContent("Direction"));
            if (showHelp) EditorGUILayout.HelpBox("The direction of the wind is equal to the Y-rotation of this object, moving in the postive Z-axis direction.", MessageType.Info);
            EditorGUILayout.PropertyField(windSpeed, new GUIContent("Speed"));
            if (showHelp) EditorGUILayout.HelpBox("The overall speed of the wind.", MessageType.Info);
            EditorGUILayout.PropertyField(windStrength, new GUIContent("Strength"));
            if (showHelp) EditorGUILayout.HelpBox("The overall strength of the wind.", MessageType.Info);
            EditorGUILayout.PropertyField(windAmplitude, new GUIContent("Amplitude"));
            if (showHelp) EditorGUILayout.HelpBox("The overall amplitude of the wind, essentially the size of wind waves.\n\nThe shader have a \"WindAmplitudeMultiplier\" parameter which multiplies this value in the material.", MessageType.Info);

            EditorGUILayout.Space();
            EditorGUILayout.EndVertical();
            
            EditorGUILayout.Space();

            EditorGUILayout.BeginVertical(EditorStyles.helpBox);

            EditorGUILayout.LabelField("Tree trunks", EditorStyles.toolbarButton);
            EditorGUILayout.Space();

            EditorGUILayout.PropertyField(trunkWindSpeed, new GUIContent("Speed"));
            if (showHelp) EditorGUILayout.HelpBox("The speed by which the tree moves.", MessageType.Info);
            EditorGUILayout.PropertyField(trunkWindWeight, new GUIContent("Weight"));
            if (showHelp) EditorGUILayout.HelpBox("The amount of influence the wind has on a tree.", MessageType.Info);
            EditorGUILayout.PropertyField(trunkWindSwinging, new GUIContent("Swinging"));
            if (showHelp) EditorGUILayout.HelpBox("A value higher than 0 means the trees will also move against the wind direction.", MessageType.Info);

            EditorGUILayout.Space();
            EditorGUILayout.EndVertical();


            DoFooter();
        }

        private void DoHeader()
        {

            EditorGUILayout.BeginHorizontal();
            showHelp = GUILayout.Toggle(showHelp, "Toggle help", "Button");
            GUILayout.Label("FAE Wind Controller", new GUIStyle(EditorStyles.centeredGreyMiniLabel)
            {
                alignment = TextAnchor.MiddleCenter,
                wordWrap = true,
                fontSize = 12
            });
            EditorGUILayout.EndHorizontal();
            if (showHelp) EditorGUILayout.HelpBox("This script drives the wind parameters of the Foliage, Grass, Tree Branch and Tree Trunk shaders.", MessageType.Info);

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

#endif
    }
}