// Fantasy Adventure Environment
// Copyright Staggart Creations
// staggart.xyz

using UnityEngine;
using System.Collections;

namespace FAE
{
#if UNITY_EDITOR
    using UnityEditor;
    [ExecuteInEditMode]
#endif

    /// <summary>
    /// Sets the wind properties of the FAE shaders
    /// </summary>
    public class WindController : MonoBehaviour
    {

        //[Header("Vector map")]
        public ProceduralMaterial windSubstance;
        public Texture windVectors;
        public bool visualizeVectors = false;
        public static bool _visualizeVectors;

        //[Header("Wind settings")]
        [Range(0f, 1f)]
        public float windDirection = 0f;
        [Range(0f, 1f)]
        public float windSpeed = 0.5f;
        [Range(0f, 16f)]
        public float windStrength = 8f;
        [Range(0f, 32f)]
        public float windAmplitude = 14f;

        //[Header("Tree trunks")]
        [Range(0f, 150f)]
        public float trunkWindSpeed = 10f;
        [Range(0f, 30f)]
        public float trunkWindWeight = 2f;
        [Range(0f, 0.99f)]
        public float trunkWindSwinging = 0.5f;

        private float m_windStrength = 0f;
        private float m_windAmplitude = 0f;

        void OnEnable()
        {
            windDirection = this.transform.localEulerAngles.y / 360f;
            m_windStrength = windStrength;
            m_windAmplitude = windAmplitude;

#if UNITY_EDITOR
            FindSubstance();

#if UNITY_5_5_OR_NEWER
            visualizeVectors = (Shader.GetGlobalFloat("_WindDebug") == 1) ? true : false;
#endif

#endif

            if (windVectors == null)
            {
                GetSubstanceOutput();
            }

            SetShaderParameters();
        }

        public void Apply()
        {
#if UNITY_EDITOR
            this.transform.localEulerAngles = new Vector3(0f, windDirection * 360f, 0f);

            //Sync the static var to the local var
            visualizeVectors = _visualizeVectors;
            VisualizeVectors(visualizeVectors);

            SetShaderParameters();

            SetSubstanceParameters();
#endif
        }

        private void SetShaderParameters()
        {
            Shader.SetGlobalFloat("_WindSpeed", windSpeed);
            Shader.SetGlobalVector("_WindDirection", this.transform.rotation * Vector3.back);
            Shader.SetGlobalTexture("_WindVectors", windVectors);

            Shader.SetGlobalFloat("_TrunkWindSpeed", trunkWindSpeed);
            Shader.SetGlobalFloat("_TrunkWindWeight", trunkWindWeight);
            Shader.SetGlobalFloat("_TrunkWindSwinging", trunkWindSwinging);

        }

#if UNITY_EDITOR

        private void OnDisable()
        {
            VisualizeVectors(false);
        }

        //Avoid constantly rebuilding the Substance by checking if the values have changed
        private void SetSubstanceParameters()
        {

            //Wind strength
            if (m_windStrength != windStrength)
            {
                windSubstance.SetProceduralFloat("windStrength", windStrength);
                if (!windSubstance.isProcessing) windSubstance.RebuildTexturesImmediately();
            }
            m_windStrength = windStrength;

            //Wind amplitude
            if (m_windAmplitude != windAmplitude)
            {
                windSubstance.SetProceduralFloat("windAmplitude", windAmplitude);
                if (!windSubstance.isProcessing) windSubstance.RebuildTexturesImmediately();
            }
            m_windAmplitude = windAmplitude;

        }

        //Looks for the FAE_WindVectors substance in the project
        public void FindSubstance()
        {
            //Substance already assigned, no need to find it
            if (windSubstance) return;

            string[] assets = AssetDatabase.FindAssets("t:ProceduralMaterial FAE_WindVectors");
            string assetPath = AssetDatabase.GUIDToAssetPath(assets[0]);

            SubstanceImporter si = AssetImporter.GetAtPath(assetPath) as SubstanceImporter; //Substance .sbsar container
            ProceduralMaterial[] substanceContainer = si.GetMaterials();

            //Look for the substance instance matching the material name we're looking for
            foreach (ProceduralMaterial substanceInstance in substanceContainer)
            {
                if (substanceInstance.name == "FAE_WindVectors")
                {
                    windSubstance = substanceInstance; //Gotcha

                    GetSubstanceOutput();
                }
            }

            //Debug.Log("Found substance: " + windSubstance.name);
        }
#endif

        //Used by ShaderGUI's
        public static void VisualizeVectors(bool state)
        {
            _visualizeVectors = state;
            Shader.SetGlobalFloat("_WindDebug", state ? 1f : 0f);
        }

        private void GetSubstanceOutput()
        {

            if (!windSubstance)
            {
                Debug.Log(this.name + ": FAE_WindVectors Substance material is not set!");
                this.enabled = false;
                return;
            }

            windSubstance.RebuildTexturesImmediately();
            Texture[] substanceOutputs = windSubstance.GetGeneratedTextures();
            windVectors = substanceOutputs[0];

        }


    }
}