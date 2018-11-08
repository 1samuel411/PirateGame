// Stylized Water Shader by Staggart Creations http://u3d.as/A2R
// Online documentation can be found at http://staggart.xyz

using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Staggart.BETA
{
    [ExecuteInEditMode]
    [RequireComponent(typeof(MeshRenderer))]

    public class StylizedWater_BETA : MonoBehaviour
    {

        #region Shader variables
        [Header("Color")]
        public Color waterColor;
        public Color waterShallowColor;
        public Color fresnelColor;
        public Color rimColor;
        public float depth;
        public float transparency;
        public bool isUnlit;

        [Header("Surface")]

        public Texture reflectionCubemap;
        public float reflection;
        public float glossiness;
        public float normalStrength;

        public float fresnel;
        public bool useIntersectionHighlight;
        public float surfaceHighlight;
        public float surfaceHighlightTiling;
        public float surfaceHighlightSize;
        public bool surfaceHighlightPanning;

        public float rimSize;
        public float rimFalloff;
        public float rimDistance;

        public bool worldSpaceTiling;
        public float tiling;
        public float rimTiling;

        public float refractionAmount;

        [Header("Waves")]
        public float waveSpeed;
        public float waveStrength;
        [Range(0f, 10f)]
        public float waveSize;
        public float waveTint;
        public float waveFoam;
        public Vector4 waveDirection;

        public Texture customIntersection;
        public Texture customNormal;
        public Texture normals;
        public Texture shadermap;

        public float tessellation;
        #endregion

        #region  Substance variables
        [Header("Substance")]

#if UNITY_EDITOR
        public ProceduralMaterial substance;
#endif

        public float seed;

        public string[] intersectionStyleList = new string[] { "None", "Windwaker", "Foamy", "Foamy2", "Triangular", "Cells", "Perlin" };
        public int intersectionStyle;

        public string[] waveStyleList = new string[] { "Perlin", "Plasma", "Circular", "Stream", "Sharp", "Cells", "Cloudy" };
        public int waveStyle;

        public string[] waveHeightmapList = new string[] { "Smooth", "Sharp", "Billowed" };
        public int waveHeightmap;

        public bool useCustomIntersection;
        public bool useCustomNormals;
        public bool useIntersectionAsHighlight;
        #endregion

        #region Meta
        //Script vars
        public Material material;
        public bool isMobileAdvanced = false;
        public bool isMobileBasic = false;
        public bool isBeta = false;
        public string shaderName = null;

        //Toggle bools
        [Header("Toggles")]
        public bool showColors = true;
        public bool showSurface = true;
        public bool showReflection;
        public bool showIntersection;
        public bool showHighlights;
        public bool showWaves;

        public bool hasSubstanceParams;
        #endregion

        #region Reflection

        public bool useCustomDepth;
        public bool useReflection;

        public string[] reslist = new string[] { "64x64", "128x128", "256x256", "512x512", "1024x1024", "2048x2048" };
        public int res;
        public int reflectionTextureSize = 256;
        public float clipPlaneOffset = 0.07f;
        public LayerMask reflectLayers = -1;
        public LayerMask depthLayers = -1;


        private Dictionary<Camera, Camera> m_ReflectionCameras = new Dictionary<Camera, Camera>(); // Camera -> Camera table
        private Dictionary<Camera, Camera> m_DepthCameras = new Dictionary<Camera, Camera>(); // Camera -> Camera table
        private RenderTexture m_ReflectionTexture;
        private RenderTexture m_DepthTexture;
        private int m_OldReflectionTextureSize;
        private int m_OldDepthTextureSize;
        private static bool s_InsideWater;
        #endregion

        public void getProperties()
        {
            //Debug.Log("StylizedWater.cs: getProperties()");

            material = GetComponent<MeshRenderer>().sharedMaterial; //Requires typeof
            shaderName = material.shader.name;

#if UNITY_EDITOR
            if (!substance)
            {
                getSubstance(material);

            }

            getSubstanceProperties();
#endif
            getShaderProperties();
        }

#if UNITY_EDITOR
        private void getSubstanceProperties()
        {
            if (!substance)
            {
                return;
            }
            //Debug.Log("StylizedWater getSubstanceProperties()");

            seed = substance.GetProceduralFloat("$randomseed");

            intersectionStyle = substance.GetProceduralEnum("intersectionStyle");
            waveStyle = substance.GetProceduralEnum("waveStyle");
            waveHeightmap = substance.GetProceduralEnum("waveHeightmap");

            useCustomIntersection = substance.GetProceduralBoolean("useCustomIntersection");
            useCustomNormals = substance.GetProceduralBoolean("useCustomNormal");
            useIntersectionAsHighlight = substance.GetProceduralBoolean("useIntersectionAsHighlight");

            if (!substance.isProcessing) substance.RebuildTexturesImmediately(); //Force output generation to avoid blank results

            Texture[] proceduralTextures = substance.GetGeneratedTextures();
            //Debug.Log("GET " + substance.name + " map [0]: " + proceduralTextures[0]);
            shadermap = proceduralTextures[0];
            normals = proceduralTextures[1];

            //Debug.Log("Getting value " + intersectionStyleList[intersectionStyle] + " from " + substance.name);

            hasSubstanceParams = true;

        }
#endif

        private void getShaderProperties()
        {
            if (!material) return;

            isMobileBasic = (shaderName.Contains("Mobile Basic")) ? true : false;
            isMobileAdvanced = (shaderName.Contains("Mobile Advanced")) ? true : false;

            //Not basic, get these parameters
            if (!isMobileBasic)
            {
                fresnelColor = material.GetColor("_FresnelColor");
                fresnel = material.GetFloat("_Fresnelexponent");

                surfaceHighlight = material.GetFloat("_SurfaceHighlight");
                surfaceHighlightTiling = material.GetFloat("_SurfaceHightlighttiling");
                surfaceHighlightSize = material.GetFloat("_Surfacehightlightsize");

                depth = material.GetFloat("_Depth");
                waterShallowColor = material.GetColor("_WaterShallowColor");

                rimDistance = material.GetFloat("_RimDistance");
            }

            //Excluded from both mobile versions
            if (!isMobileAdvanced && !isMobileBasic)
            {
                useIntersectionHighlight = (material.GetFloat("_UseIntersectionHighlight") == 1) ? true : false;
                surfaceHighlightPanning = (material.GetFloat("_HighlightPanning") == 1) ? true : false;
                refractionAmount = material.GetFloat("_RefractionAmount");
                waveStrength = material.GetFloat("_Wavesstrength");
                reflectionCubemap = material.GetTexture("_Reflection");

                waveTint = material.GetFloat("_Wavetint");
                waveFoam = material.GetFloat("_Wavefoam");
                waveSize = material.GetFloat("_WaveSize");
                waveDirection = material.GetVector("_WaveDirection");

                reflection = material.GetFloat("_Reflection");
            }

            transparency = material.GetFloat("_Transparency");

            //Shared by all
            normalStrength = material.GetFloat("_NormalStrength");
            worldSpaceTiling = (material.GetFloat("_Worldspacetiling") == 1) ? true : false;

            waterColor = material.GetColor("_WaterColor");
            rimColor = material.GetColor("_RimColor");
            isUnlit = (material.GetFloat("_Unlit") == 1) ? true : false;

            glossiness = material.GetFloat("_Glossiness");

            rimSize = material.GetFloat("_RimSize");
            rimFalloff = material.GetFloat("_Rimfalloff");

            tiling = material.GetFloat("_Tiling");
            rimTiling = material.GetFloat("_Rimtiling");

            waveSpeed = material.GetFloat("_Wavesspeed");

            //Tesselation shader only
            if (shaderName.Contains("Tessellation") || shaderName.Contains("BETA"))
            {
                tessellation = material.GetFloat("_Tessellation");
            }
        }

#if UNITY_EDITOR
        private void getSubstance(Material mat)
        {
            if (mat == null)
            {
                Debug.LogError("StylizedWater: No material assigned to MeshRenderer component!");
                return;
            }

            if (mat.GetTexture("_Shadermap"))
            {
                string[] assets;
                string substanceName = mat.GetTexture("_Shadermap").name.Replace("_shadermap", ""); //eg StylizedWater_frozen_shadermap to StylizedWater_frozen

                assets = AssetDatabase.FindAssets("t:ProceduralMaterial " + substanceName);
                string assetPath = AssetDatabase.GUIDToAssetPath(assets[0]);

                SubstanceImporter si = AssetImporter.GetAtPath(assetPath) as SubstanceImporter; //Substance .sbsar container
                ProceduralMaterial[] substanceContainer = si.GetMaterials();

                //Look for the substance instance matching the material name we're looking for
                foreach (ProceduralMaterial substanceInstance in substanceContainer)
                {
                    if (substanceInstance.name == substanceName)
                    {
                        substance = substanceInstance; //Gotcha
                    }
                }

                //Debug.Log("Found substance using " + material.name + ". Result: " + substance.name);
            }

            else
            {
                Debug.LogError("StylizedWater: Shadermap is not assigned to the current material, cannot locate associated Substance without it.");
                return;
            }

        }
#endif

        #region Render textures
        // This is called when it's known that the object will be rendered by some
        // camera. We render reflections / refractions and do other updates here.
        // Because the script executes in edit mode, reflections for the scene view
        // camera will just work!
        public void OnWillRenderObject()
        {

            if (!enabled || !material)
            {
                return;
            }

            Camera cam = Camera.current;
            if (!cam)
            {
                return;
            }

            // Safeguard from recursive water reflections.
            if (s_InsideWater)
            {
                return;
            }
            s_InsideWater = true;

            Camera reflectionCamera, depthCamera;
            CreateWaterObjects(cam, out reflectionCamera, out depthCamera);

            // find out the reflection plane: position and normal in world space
            Vector3 pos = transform.position;
            Vector3 normal = transform.up;

            UpdateCameraModes(cam, reflectionCamera);
            UpdateCameraModes(cam, depthCamera);

            // Render reflection if needed
            if (useReflection)
            {
                // Reflect camera around reflection plane
                float d = -Vector3.Dot(normal, pos) - clipPlaneOffset;
                Vector4 reflectionPlane = new Vector4(normal.x, normal.y, normal.z, d);

                Matrix4x4 reflection = Matrix4x4.zero;
                CalculateReflectionMatrix(ref reflection, reflectionPlane);
                Vector3 oldpos = cam.transform.position;
                Vector3 newpos = reflection.MultiplyPoint(oldpos);
                reflectionCamera.worldToCameraMatrix = cam.worldToCameraMatrix * reflection;

                // Setup oblique projection matrix so that near plane is our reflection
                // plane. This way we clip everything below/above it for free.
                Vector4 clipPlane = CameraSpacePlane(reflectionCamera, pos, normal, 1.0f);
                reflectionCamera.projectionMatrix = cam.CalculateObliqueMatrix(clipPlane);

                reflectionCamera.cullingMask = ~(1 << 4) & reflectLayers.value; // never render water layer
                reflectionCamera.targetTexture = m_ReflectionTexture;
                bool oldCulling = GL.invertCulling;
                GL.invertCulling = !oldCulling;
                reflectionCamera.transform.position = newpos;
                Vector3 euler = cam.transform.eulerAngles;
                reflectionCamera.transform.eulerAngles = new Vector3(-euler.x, euler.y, euler.z);
                reflectionCamera.Render();
                reflectionCamera.transform.position = oldpos;
                GL.invertCulling = oldCulling;
                material.SetTexture("_ReflectionTex", m_ReflectionTexture);
            }

            // Render depth
            if (useCustomDepth)
            {
                depthCamera.worldToCameraMatrix = cam.worldToCameraMatrix;

                // Setup oblique projection matrix so that near plane is our reflection
                // plane. This way we clip everything below/above it for free.
                Vector4 clipPlane = CameraSpacePlane(depthCamera, pos, normal, -1.0f);
                depthCamera.projectionMatrix = cam.CalculateObliqueMatrix(clipPlane);

                depthCamera.cullingMask = ~(1 << 4) & depthLayers.value; // never render water layer
                depthCamera.targetTexture = m_DepthTexture;
                depthCamera.transform.position = cam.transform.position;
                depthCamera.transform.rotation = cam.transform.rotation;
                // reflectionCamera.RenderWithShader(Shader.Find("Hidden/RenderDepth"), "Opaque");
                depthCamera.Render();
                material.SetTexture("_RefractionTex", m_DepthTexture);
            }

            s_InsideWater = false;
        }

        void UpdateCameraModes(Camera src, Camera dest)
        {
            if (dest == null)
            {
                return;
            }
            // set water camera to clear the same way as current camera
            dest.clearFlags = src.clearFlags;
            dest.backgroundColor = src.backgroundColor;
            if (src.clearFlags == CameraClearFlags.Skybox)
            {
                Skybox sky = src.GetComponent<Skybox>();
                Skybox mysky = dest.GetComponent<Skybox>();
                if (!sky || !sky.material)
                {
                    mysky.enabled = false;
                }
                else
                {
                    mysky.enabled = true;
                    mysky.material = sky.material;
                }
            }
            // update other values to match current camera.
            // even if we are supplying custom camera&projection matrices,
            // some of values are used elsewhere (e.g. skybox uses far plane)
            dest.farClipPlane = src.farClipPlane;
            dest.nearClipPlane = src.nearClipPlane;
            dest.orthographic = src.orthographic;
            dest.fieldOfView = src.fieldOfView;
            dest.aspect = src.aspect;
            dest.orthographicSize = src.orthographicSize;
        }
        // On-demand create any objects we need for water
        void CreateWaterObjects(Camera currentCamera, out Camera reflectionCamera, out Camera depthCamera)
        {

            reflectionCamera = null;
            depthCamera = null;

            if (useReflection)
            {
                // Reflection render texture
                if (!m_ReflectionTexture || m_OldReflectionTextureSize != reflectionTextureSize)
                {
                    if (m_ReflectionTexture)
                    {
                        DestroyImmediate(m_ReflectionTexture);
                    }
                    m_ReflectionTexture = new RenderTexture(reflectionTextureSize, reflectionTextureSize, 16);
                    m_ReflectionTexture.name = "__WaterReflection" + GetInstanceID();
                    m_ReflectionTexture.isPowerOfTwo = true;
                    m_ReflectionTexture.hideFlags = HideFlags.DontSave;
                    m_OldReflectionTextureSize = reflectionTextureSize;
                }

                // Camera for reflection
                m_ReflectionCameras.TryGetValue(currentCamera, out reflectionCamera);
                if (!reflectionCamera) // catch both not-in-dictionary and in-dictionary-but-deleted-GO
                {
                    GameObject go = new GameObject("Reflection Camera " + GetInstanceID() + " for " + currentCamera.GetInstanceID(), typeof(Camera), typeof(Skybox));
                    reflectionCamera = go.GetComponent<Camera>();
                    reflectionCamera.enabled = false;
                    reflectionCamera.transform.position = transform.position;
                    reflectionCamera.transform.rotation = transform.rotation;
                    reflectionCamera.gameObject.AddComponent<FlareLayer>();
                    go.hideFlags = HideFlags.HideAndDontSave;
                    m_ReflectionCameras[currentCamera] = reflectionCamera;
                }
            }

            if (useCustomDepth)
            {
                // Depth render texture
                if (!m_DepthTexture || m_OldDepthTextureSize != reflectionTextureSize)
                {
                    if (m_DepthTexture)
                    {
                        DestroyImmediate(m_DepthTexture);
                    }
                    m_DepthTexture = new RenderTexture(reflectionTextureSize, reflectionTextureSize, 16);
                    m_DepthTexture.name = "__WaterDepth" + GetInstanceID();
                    m_DepthTexture.isPowerOfTwo = true;
                    m_DepthTexture.hideFlags = HideFlags.DontSave;
                    m_OldDepthTextureSize = reflectionTextureSize;
                }

                // Camera for depth
                m_DepthCameras.TryGetValue(currentCamera, out depthCamera);
                if (!depthCamera) // catch both not-in-dictionary and in-dictionary-but-deleted-GO
                {
                    GameObject go =
                        new GameObject("Depth Camera " + GetInstanceID() + " for " + currentCamera.GetInstanceID(),
                            typeof(Camera), typeof(Skybox));
                    depthCamera = go.GetComponent<Camera>();
                    depthCamera.enabled = false;
                    depthCamera.transform.position = transform.position;
                    depthCamera.transform.rotation = transform.rotation;
                    depthCamera.gameObject.AddComponent<FlareLayer>();
                    //depthCamera.gameObject.AddComponent<RenderDepth>();
                    go.hideFlags = HideFlags.HideAndDontSave;
                    m_DepthCameras[currentCamera] = depthCamera;
                }
            }
        }

        // Given position/normal of the plane, calculates plane in camera space.
        Vector4 CameraSpacePlane(Camera cam, Vector3 pos, Vector3 normal, float sideSign)
        {
            Vector3 offsetPos = pos + normal * clipPlaneOffset;
            Matrix4x4 m = cam.worldToCameraMatrix;
            Vector3 cpos = m.MultiplyPoint(offsetPos);
            Vector3 cnormal = m.MultiplyVector(normal).normalized * sideSign;
            return new Vector4(cnormal.x, cnormal.y, cnormal.z, -Vector3.Dot(cpos, cnormal));
        }

        // Calculates reflection matrix around the given plane
        static void CalculateReflectionMatrix(ref Matrix4x4 reflectionMat, Vector4 plane)
        {
            reflectionMat.m00 = (1F - 2F * plane[0] * plane[0]);
            reflectionMat.m01 = (-2F * plane[0] * plane[1]);
            reflectionMat.m02 = (-2F * plane[0] * plane[2]);
            reflectionMat.m03 = (-2F * plane[3] * plane[0]);

            reflectionMat.m10 = (-2F * plane[1] * plane[0]);
            reflectionMat.m11 = (1F - 2F * plane[1] * plane[1]);
            reflectionMat.m12 = (-2F * plane[1] * plane[2]);
            reflectionMat.m13 = (-2F * plane[3] * plane[1]);

            reflectionMat.m20 = (-2F * plane[2] * plane[0]);
            reflectionMat.m21 = (-2F * plane[2] * plane[1]);
            reflectionMat.m22 = (1F - 2F * plane[2] * plane[2]);
            reflectionMat.m23 = (-2F * plane[3] * plane[2]);

            reflectionMat.m30 = 0F;
            reflectionMat.m31 = 0F;
            reflectionMat.m32 = 0F;
            reflectionMat.m33 = 1F;
        }
        #endregion

        // Cleanup all the objects we possibly have created
        void OnDisable()
        {
            if (m_DepthTexture)
            {
                DestroyImmediate(m_DepthTexture);
                m_DepthTexture = null;
            }

            foreach (var kvp in m_DepthCameras)
            {
                DestroyImmediate((kvp.Value).gameObject);
            }
            m_DepthCameras.Clear();

            DestroyReflectionCam();
        }

        public void DestroyReflectionCam()
        {
            //Clear texture
            if (m_ReflectionTexture)
            {
                DestroyImmediate(m_ReflectionTexture);
                m_ReflectionTexture = null;
            }

            //Clear camera
            foreach (var kvp in m_ReflectionCameras)
            {
                DestroyImmediate((kvp.Value).gameObject);
            }
            m_ReflectionCameras.Clear();
        }

        public void CreateReflectionCam()
        {

        }

    }//class end
}

//Easter egg, good job :)