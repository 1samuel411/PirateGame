// Stylized Water Shader by Staggart Creations http://u3d.as/A2R
// Online documentation can be found at http://staggart.xyz

// BETA VERSION

using UnityEngine;
using System.Collections;
#if UNITY_EDITOR
using UnityEditor;
#endif

#if UNITY_EDITOR
namespace Staggart.BETA
{
    [CustomEditor(typeof(StylizedWater_BETA))]
    public class StylizedWaterInspector_BETA : Editor
    {
        //Non serialized, local
        StylizedWater_BETA stylizedWater;
        Material material;
        ProceduralMaterial substance;

        //Shader variables
        #region Shader parameters
        public Color waterColor;
        public Color waterShallowColor;
        public Color fresnelColor;
        public Color rimColor;
        public bool isUnlit;

        public float transparency;
        public float glossiness;
        public float fresnel;
        public float normalStrength;
        public float reflection;

        public bool useIntersectionHighlight;
        public float surfaceHighlightTiling;

        public float depth;

        public float rimSize;
        public float rimFalloff;

        public bool worldSpaceTiling;
        public float tiling;
        public float rimTiling;
        public float rimDistance;

        public float refractionAmount;

        public float waveSpeed;
        public float waveStrength;
        public float waveTint;
        public float waveFoam;
        public float waveSize;
        public Vector3 waveDirection;

        public Texture normals;
        public Texture shadermap;

        public Texture reflectionCubemap;

        public float tessellation;
        #endregion

        #region  Substance parameters
        public float seed;

        public string[] intersectionStyleList;
        public string[] waveStyleList;

        public bool useIntersectionAsHighlight;
        public bool surfaceHighlightPanning;
        public float surfaceHighlight;
        public float surfaceHighlightSize;
        public int intersectionStyle;

        public int waveStyle;
        public string[] waveHeightmapList;
        public int waveHeightmap;
        public bool useCustomNormals;
        public bool useCustomIntersection;
        #endregion

        #region Local variables
        bool useReflection;
        public string[] reslist = new string[] { "64x64", "128x128", "256x256", "512x512", "1024x1024", "2048x2048" };
        public int res;
        int reflectionTextureSize;
        LayerMask reflectLayers;
        Texture customIntersection;
        Texture customNormal;

        GameObject selected;
        public bool isMobileAdvanced;
        public bool isMobileBasic;
        #endregion

        #region Meta
        string shaderName = null;
        bool hasSubstanceParams = false;

        //Toggle bools
        bool showColors = true;
        bool showSurface = true;
        bool showReflection;
        bool showIntersection;
        bool showHighlights;
        bool showWaves;

        //Styling
        GUIStyle groupFoldout;
        #endregion

        void OnEnable()
        {
            setStyling();
            selected = Selection.activeGameObject;

            getProperties();

        }

        public override void OnInspectorGUI()
        {

            EditorGUI.BeginChangeCheck();

            Undo.RecordObject(this, "Component");
            if (selected) Undo.RecordObject(selected, "Water");
            if (substance) Undo.RecordObject(substance, "Substance");
            if (material) Undo.RecordObject(material, "Material");

            /*-----------------*/
            drawFields();
            /*-----------------*/

            //Apply values
            if (EditorGUI.EndChangeCheck())
            {
                setScriptValues(stylizedWater); //Passes the current Substance to component

                //Shader
                setShaderValues();

                //Substance
                setSubstanceParameters();

                //Changes applied, grab new values
                getProperties();

                EditorUtility.SetDirty(this);
            }

        }

        //Get shader and Substance values
        void getProperties()
        {
            //During runtime, nothing might be selected
            if (!selected) return;

            stylizedWater = selected.GetComponent<StylizedWater_BETA>();
            //In case StylizedWater inspector is locked but another object is selected on runtime
            if (!stylizedWater) return;

            //Get shader and substance properties
            stylizedWater.getProperties();

            material = stylizedWater.material;
            substance = stylizedWater.substance;

            //Meta
            showColors = stylizedWater.showColors;
            showSurface = stylizedWater.showSurface;
            showReflection = stylizedWater.showReflection;
            showIntersection = stylizedWater.showIntersection;
            showHighlights = stylizedWater.showHighlights;
            showWaves = stylizedWater.showWaves;

            isMobileAdvanced = stylizedWater.isMobileAdvanced;
            isMobileBasic = stylizedWater.isMobileBasic;

            shaderName = stylizedWater.shaderName;
            useReflection = stylizedWater.useReflection;
            res = stylizedWater.res;
            reflectLayers = stylizedWater.reflectLayers;

            #region Shader parameters
            waterColor = stylizedWater.waterColor;
            waterShallowColor = stylizedWater.waterShallowColor;
            fresnelColor = stylizedWater.fresnelColor;
            rimColor = stylizedWater.rimColor;
            isUnlit = stylizedWater.isUnlit;

            transparency = stylizedWater.transparency;
            glossiness = stylizedWater.glossiness;
            reflectionCubemap = stylizedWater.reflectionCubemap;
            fresnel = stylizedWater.fresnel;
            normalStrength = stylizedWater.normalStrength;
            reflection = stylizedWater.reflection;

            useIntersectionHighlight = stylizedWater.useIntersectionHighlight;
            surfaceHighlightPanning = stylizedWater.surfaceHighlightPanning;
            surfaceHighlight = stylizedWater.surfaceHighlight;
            surfaceHighlightSize = stylizedWater.surfaceHighlightSize;
            surfaceHighlightTiling = stylizedWater.surfaceHighlightTiling;

            depth = stylizedWater.depth;

            rimSize = stylizedWater.rimSize;
            rimFalloff = stylizedWater.rimFalloff;
            rimDistance = stylizedWater.rimDistance;

            worldSpaceTiling = stylizedWater.worldSpaceTiling;
            tiling = stylizedWater.tiling;
            rimTiling = stylizedWater.rimTiling;

            refractionAmount = stylizedWater.refractionAmount;

            waveSpeed = stylizedWater.waveSpeed;
            waveStrength = stylizedWater.waveStrength;
            waveTint = stylizedWater.waveTint;
            waveFoam = stylizedWater.waveFoam;
            waveSize = stylizedWater.waveSize;
            waveDirection = stylizedWater.waveDirection;

            tessellation = stylizedWater.tessellation;
            #endregion

            #region Substance parameters
            seed = stylizedWater.seed;
            intersectionStyleList = stylizedWater.intersectionStyleList;
            intersectionStyle = stylizedWater.intersectionStyle;
            waveStyle = stylizedWater.waveStyle;
            waveStyleList = stylizedWater.waveStyleList;
            waveHeightmapList = stylizedWater.waveHeightmapList;
            waveHeightmap = stylizedWater.waveHeightmap;

            useCustomNormals = stylizedWater.useCustomNormals;
            useCustomIntersection = stylizedWater.useCustomIntersection;
            useIntersectionAsHighlight = stylizedWater.useIntersectionAsHighlight;

            customIntersection = stylizedWater.customIntersection;
            customNormal = stylizedWater.customNormal;

            normals = stylizedWater.normals;
            shadermap = stylizedWater.shadermap;

            //Substance values are not null, safe to apply
            hasSubstanceParams = stylizedWater.hasSubstanceParams; //true if has Substance
            #endregion

            setSubstanceParameters(); //Go back and re-run this function now that hasSubstanceParams is True

        }

        //Draw inspector fields
        void drawFields()
        {
            EditorGUILayout.HelpBox("This shader is in Beta, some aspects may not work correctly. Please report any issue to contact@staggart.xyz", MessageType.Warning);

            if (material == null || !shaderName.Contains("StylizedWater"))
            {
                EditorGUILayout.HelpBox("Please assign a \"StylizedWater\" shader to the current material", MessageType.Error);
            }
            else
            {
                EditorGUILayout.LabelField("Current shader: ", shaderName.Replace("StylizedWater", string.Empty), EditorStyles.boldLabel);
            }

            #region Substance
            ProceduralMaterial mSubstance = substance;
            substance = (ProceduralMaterial)EditorGUILayout.ObjectField("Substance", substance, typeof(ProceduralMaterial), true);

            if (mSubstance != substance)
            {
                //Debug.Log("Substance has changed: " + mSubstance.name + " > " + substance.name);
                hasSubstanceParams = false;
            }

            if (!substance)
            {
                EditorGUILayout.HelpBox("Please assign a Substance material instance", MessageType.Error);
            }

            //If conditions aren't met, don't display fields
            if (!substance || material && !shaderName.Contains("StylizedWater")) return;

            EditorGUILayout.BeginHorizontal();

            seed = EditorGUILayout.FloatField("Seed", seed);
            if (GUILayout.Button("Randomize", EditorStyles.miniButton))
            {
                seed = Random.Range(1, 9999);
            }
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.Space();
            #endregion

            #region Colors

            if (GUILayout.Button((showColors) ? "[-] Colors" : "[+] Colors", groupFoldout))
            {
                showColors = !showColors;
            }

            if (showColors)
            {
                isUnlit = EditorGUILayout.Toggle("Unlit", isUnlit);

                EditorGUILayout.HelpBox("Alpha channels control intensity!", MessageType.None);
                waterColor = EditorGUILayout.ColorField("Water color", waterColor);
                waterShallowColor = EditorGUILayout.ColorField("Shallow color", waterShallowColor);
                rimColor = EditorGUILayout.ColorField("Rim color", rimColor);

                if (!isMobileBasic)
                {
                    fresnelColor = EditorGUILayout.ColorField("Fresnel color", fresnelColor);
                    fresnel = EditorGUILayout.Slider("Fresnel", fresnel, 0f, 10f);
                    depth = EditorGUILayout.Slider("Depth", depth, 0f, 100f);
                }

                transparency = EditorGUILayout.Slider("Transparency", transparency, 0f, 1f);

            }

            EditorGUILayout.Space();
            #endregion

            #region Surface
            if (GUILayout.Button((showSurface) ? "[-] Surface" : "[+] Surface", groupFoldout))
            {
                showSurface = !showSurface;
            }
            if (showSurface)
            {
                normalStrength = EditorGUILayout.Slider("Normal strength", normalStrength, 0f, 1f);
                glossiness = EditorGUILayout.Slider("Glossiness", glossiness, 0f, 1f);
                worldSpaceTiling = EditorGUILayout.Toggle("World-space tiling", worldSpaceTiling);
                tiling = EditorGUILayout.Slider("Tiling", tiling, 0f, 1f);


                refractionAmount = EditorGUILayout.Slider("Refraction", refractionAmount, 0f, 0.1f);
                //reflectionCubemap = (Texture)EditorGUILayout.ObjectField("Reflection cubemap", reflectionCubemap, typeof(Cubemap), false);

            }

            EditorGUILayout.Space();
            #endregion

            #region Reflection
            if (!isMobileAdvanced && !isMobileBasic)
            {
                if (GUILayout.Button((showReflection) ? "[-] Reflection" : "[+] Reflection", groupFoldout))
                {
                    showReflection = !showReflection;
                }
                if (showReflection)
                {
                    useReflection = EditorGUILayout.Toggle("Enable reflections", useReflection);

                    if (useReflection)
                    {
                        EditorGUILayout.BeginHorizontal();
                        EditorGUILayout.LabelField("Resolution");
                        res = EditorGUILayout.Popup(res, reslist, EditorStyles.toolbarDropDown);
                        EditorGUILayout.EndHorizontal();
                        reflection = EditorGUILayout.Slider("Amount", reflection, 0f, 1f);

                        //TODO
                        //reflectLayers = EditorGUILayout.MaskField("Culling Mask", reflectLayers);
                    }

                }
            }
            EditorGUILayout.Space();
            #endregion

            #region Intersection
            if (GUILayout.Button((showIntersection) ? "[-] Intersection" : "[+] Intersection", groupFoldout))
            {
                showIntersection = !showIntersection;
            }
            if (showIntersection)
            {

                useCustomIntersection = EditorGUILayout.BeginToggleGroup("Use custom texture", useCustomIntersection);
                if (useCustomIntersection)
                {

                    customIntersection = (Texture)EditorGUILayout.ObjectField("Grayscale texture", customIntersection, typeof(Texture2D), false);
                    if (customIntersection == null)
                    {
                        EditorGUILayout.HelpBox("Field cannot be empty", MessageType.Warning);
                    }

                }

                EditorGUILayout.EndToggleGroup();


                if (!useCustomIntersection)
                {
                    EditorGUILayout.BeginHorizontal();
                    EditorGUILayout.LabelField("Style");
                    intersectionStyle = EditorGUILayout.Popup(intersectionStyle, intersectionStyleList, EditorStyles.toolbarDropDown);
                    EditorGUILayout.EndHorizontal();
                }


                rimSize = EditorGUILayout.Slider("Size", rimSize, 0f, 20f);
                if (intersectionStyle != 0)
                {
                    rimFalloff = EditorGUILayout.Slider("Falloff", rimFalloff, 0.01f, 50f);
                }
                if (!isMobileBasic)
                {
                    rimDistance = EditorGUILayout.Slider("Distance", rimDistance, 0f, 3f);
                }

                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("Tiling");
                if (GUILayout.Button("<<", EditorStyles.miniButtonLeft))
                {
                    rimTiling -= .5f;
                }
                if (GUILayout.Button("<", EditorStyles.miniButton))
                {
                    rimTiling -= .1f;
                }
                rimTiling = EditorGUILayout.FloatField(rimTiling, GUILayout.MaxWidth(30));
                if (GUILayout.Button(">", EditorStyles.miniButton))
                {
                    rimTiling += .1f;
                }
                if (GUILayout.Button(">>", EditorStyles.miniButtonRight))
                {
                    rimTiling += .5f;
                }
                EditorGUILayout.EndHorizontal();
            }

            EditorGUILayout.Space();
            #endregion

            #region Surface highlights
            if (!isMobileBasic)
            {
                if (GUILayout.Button((showHighlights) ? "[-] Surface highlights" : "[+] Surface highlights", groupFoldout))
                {
                    showHighlights = !showHighlights;
                }
                if (showHighlights)
                {
                    if (!isMobileAdvanced)
                    {
                        useIntersectionHighlight = EditorGUILayout.BeginToggleGroup("Use intersection texture", useIntersectionHighlight);
                        EditorGUILayout.EndToggleGroup();
                        useIntersectionAsHighlight = useIntersectionHighlight;

                        if (useIntersectionHighlight)
                        {
                            surfaceHighlightPanning = EditorGUILayout.BeginToggleGroup("Cross-panning textures", surfaceHighlightPanning);
                            EditorGUILayout.EndToggleGroup();
                        }
                    }
                    else
                    {
                        useIntersectionHighlight = false;
                        surfaceHighlightPanning = false;
                    }

                    //Sync shader var to Substance
                    useIntersectionAsHighlight = useIntersectionHighlight;

                    surfaceHighlight = EditorGUILayout.Slider("Opacity", surfaceHighlight, 0f, 1f);

                    if (!useIntersectionHighlight)
                    {
                        surfaceHighlightSize = EditorGUILayout.Slider("Size", surfaceHighlightSize, 0f, 1f);
                    }

                    EditorGUILayout.BeginHorizontal();
                    EditorGUILayout.LabelField("Tiling");
                    if (GUILayout.Button("<<", EditorStyles.miniButtonLeft))
                    {
                        surfaceHighlightTiling -= .5f;
                    }
                    if (GUILayout.Button("<", EditorStyles.miniButton))
                    {
                        surfaceHighlightTiling -= .1f;
                    }
                    surfaceHighlightTiling = EditorGUILayout.FloatField(surfaceHighlightTiling, GUILayout.MaxWidth(30));
                    if (GUILayout.Button(">", EditorStyles.miniButton))
                    {
                        surfaceHighlightTiling += .1f;
                    }
                    if (GUILayout.Button(">>", EditorStyles.miniButtonRight))
                    {
                        surfaceHighlightTiling += .5f;
                    }
                    EditorGUILayout.EndHorizontal();
                }

                EditorGUILayout.Space();
            }//not mobile basic
            #endregion

            #region Waves
            if (GUILayout.Button((showWaves) ? "[-] Waves" : "[+] Waves", groupFoldout))
            {
                showWaves = !showWaves;
            }
            if (showWaves)
            {

                useCustomNormals = EditorGUILayout.BeginToggleGroup("Use custom normal map", useCustomNormals);
                if (useCustomNormals)
                {
                    customNormal = (Texture)EditorGUILayout.ObjectField("Normal map", customNormal, typeof(Texture2D), false);
                    if (customNormal == null)
                    {
                        EditorGUILayout.HelpBox("Field cannot be empty", MessageType.Warning);
                    }
                }

                EditorGUILayout.EndToggleGroup();

                if (!useCustomNormals)
                {
                    EditorGUILayout.BeginHorizontal();
                    EditorGUILayout.LabelField("Style");
                    waveStyle = EditorGUILayout.Popup(waveStyle, waveStyleList, EditorStyles.toolbarDropDown);
                    EditorGUILayout.EndHorizontal();
                }

                EditorGUILayout.Space();

                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("Heightmap");
                waveHeightmap = EditorGUILayout.Popup(waveHeightmap, waveHeightmapList, EditorStyles.toolbarDropDown);
                EditorGUILayout.EndHorizontal();

                EditorGUILayout.Space();

                waveSpeed = EditorGUILayout.Slider("Speed", waveSpeed, 0f, 10f);
                if (!isMobileAdvanced && !isMobileBasic)
                {
                    waveStrength = EditorGUILayout.Slider("Strength", waveStrength, 0f, 1f);
                    waveDirection.x = EditorGUILayout.Slider("Direction X", waveDirection.x, -1f, 1f);
                    waveDirection.z = EditorGUILayout.Slider("Direction Z", waveDirection.z, -1f, 1f);
                    waveSize = EditorGUILayout.Slider("Size", waveSize, 0f, 10f);
                    waveTint = EditorGUILayout.Slider("Wave tinting", waveTint, -1f, 1f);
                    waveFoam = EditorGUILayout.Slider("Foam", waveFoam, 0f, 1f);
                }
            }

            EditorGUILayout.Space();
            #endregion

            if (shaderName.Contains("Tessellation") || shaderName.Contains("BETA"))
            {
                tessellation = EditorGUILayout.Slider("Tessellation", tessellation, 0.1f, 10f);
            }
        }

        //Apply values to material shader
        private void setShaderValues()
        {
            if (!material) return;

            //Excluded from mobile shaders
            if (!isMobileAdvanced || !isMobileBasic)
            {
                material.SetFloat("_Transparency", transparency);
                material.SetFloat("_RefractionAmount", refractionAmount);
                material.SetFloat("_Wavesstrength", waveStrength);
                material.SetFloat("_Wavetint", waveTint);
                material.SetFloat("_Wavefoam", waveFoam);
                material.SetFloat("_WaveSize", waveSize);
                material.SetVector("_WaveDirection", waveDirection);

                material.SetFloat("_Reflection", reflection);

                if (useIntersectionHighlight == true)
                {
                    material.SetFloat("_UseIntersectionHighlight", 1);
                }
                else
                {
                    material.SetFloat("_UseIntersectionHighlight", 0);
                }

                if (surfaceHighlightPanning == true)
                {
                    material.SetFloat("_HighlightPanning", 1);
                }
                else
                {
                    material.SetFloat("_HighlightPanning", 0);
                }
            }

            //Excluded from Mobile Basic, but shared by Mobile Advanced and Desktop
            if (!isMobileBasic)
            {
                material.SetColor("_FresnelColor", fresnelColor);
                material.SetColor("_WaterShallowColor", waterShallowColor);

                material.SetFloat("_Fresnelexponent", fresnel);

                material.SetFloat("_SurfaceHighlight", surfaceHighlight);
                material.SetFloat("_SurfaceHightlighttiling", surfaceHighlightTiling);
                material.SetFloat("_Surfacehightlightsize", surfaceHighlightSize);

                material.SetFloat("_Depth", depth);

                material.SetFloat("_RimDistance", rimDistance);
            }

            //Shared by all
            material.SetFloat("_NormalStrength", normalStrength);
            material.SetColor("_WaterColor", waterColor);
            material.SetColor("_RimColor", rimColor);
            material.SetFloat("_Unlit", (isUnlit) ? 1f : 0f);

            material.SetFloat("_Glossiness", glossiness);

            material.SetFloat("_RimSize", rimSize);
            material.SetFloat("_Rimfalloff", rimFalloff);

            material.SetFloat("_Wavesspeed", waveSpeed);

            material.SetFloat("_Worldspacetiling", (worldSpaceTiling) ? 1f : 0f);
            material.SetFloat("_Tiling", tiling);
            material.SetFloat("_Rimtiling", rimTiling);

            if (shaderName.Contains("Tessellation") || shaderName.Contains("BETA"))
            {
                material.SetFloat("_Tessellation", tessellation);
            }


        }

        //Apply values to Substance material
        private void setSubstanceParameters()
        {
            if (!substance || !hasSubstanceParams) return; //Prevent from setting null values
                                                           //Debug.Log("Setting value " + intersectionStyleList[intersectionStyle] + " to " + substance.name);

            substance.SetProceduralFloat("$randomseed", seed);

            if (useCustomIntersection)
            {
                substance.SetProceduralBoolean("useCustomIntersection", useCustomIntersection);
                if (customIntersection)
                {
                    substance.SetProceduralTexture("customIntersectionTex", (Texture2D)customIntersection);
                }
                stylizedWater.customIntersection = customIntersection;
            }
            else
            {
                substance.SetProceduralBoolean("useCustomIntersection", false);
                substance.SetProceduralTexture("customIntersectionTex", null);
            }

            if (useCustomNormals)
            {
                substance.SetProceduralBoolean("useCustomNormal", useCustomNormals);
                if (customNormal)
                {
                    substance.SetProceduralTexture("customNormalTex", (Texture2D)customNormal);
                }
                stylizedWater.customNormal = customNormal;
            }
            else
            {
                substance.SetProceduralBoolean("useCustomNormal", false);
                substance.SetProceduralTexture("customNormalTex", null);
            }


            substance.SetProceduralBoolean("useIntersectionAsHighlight", useIntersectionAsHighlight);


            substance.SetProceduralEnum("intersectionStyle", intersectionStyle);
            substance.SetProceduralEnum("waveStyle", waveStyle);
            substance.SetProceduralEnum("waveHeightmap", waveHeightmap);


            //Excecution order requires this check
            if (shadermap && normals)
            {
                //Debug.Log("SET "+ substance.name + " shadermap: " + shadermap);
                material.SetTexture("_Shadermap", shadermap);
                material.SetTexture("_Normals", normals);
            }
            else { Debug.LogError("Shadermap & normal are null"); }

            if (!substance.isProcessing) substance.RebuildTexturesImmediately();
        }

        //Saving meta data
        private void setScriptValues(StylizedWater_BETA sws)
        {
            if (!sws) return;

            sws.showColors = showColors;
            sws.showSurface = showSurface;
            sws.showReflection = showReflection;
            sws.showIntersection = showIntersection;
            sws.showHighlights = showHighlights;
            sws.showWaves = showWaves;

            sws.substance = substance;
            sws.hasSubstanceParams = hasSubstanceParams; //If Substance changed, is set to false

            //Disable reflection functionality
            if (!useReflection)
            {
                sws.DestroyReflectionCam();
                reflection = 0;
            }

            sws.useReflection = useReflection;
            sws.res = res;
            // sws.reflectLayers = reflectLayers;
            switch (res)
            {
                case 0:
                    sws.reflectionTextureSize = 64;
                    break;
                case 1:
                    sws.reflectionTextureSize = 128;
                    break;
                case 2:
                    sws.reflectionTextureSize = 256;
                    break;
                case 3:
                    sws.reflectionTextureSize = 512;
                    break;
                case 4:
                    sws.reflectionTextureSize = 1024;
                    break;
                case 5:
                    sws.reflectionTextureSize = 2048;
                    break;
            }
        }

        //Set up GUIStyle for headers
        void setStyling()
        {
            if (groupFoldout != null) return;

            groupFoldout = new GUIStyle(EditorStyles.toolbarDropDown);

            groupFoldout.fontSize = 11;
            groupFoldout.fixedHeight = 22f;

            RectOffset groupFoldoutPadding = new RectOffset();
            groupFoldoutPadding.left = 10;
            groupFoldoutPadding.top = 4;
            groupFoldoutPadding.bottom = 5;
            groupFoldout.padding = groupFoldoutPadding;
        }

    }

}
#endif
