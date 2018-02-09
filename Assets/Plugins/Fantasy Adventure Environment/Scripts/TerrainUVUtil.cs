// Fantasy Adventure Environment
// Copyright Staggart Creations
// staggart.xyz

using UnityEngine;
using System.Collections;

namespace FAE {

#if UNITY_EDITOR
    [ExecuteInEditMode]
#endif

    public class TerrainUVUtil : ScriptableObject {

        //Public
        public enum Workflow
        {
            None,
            Terrain,
            Mesh
        }
        public Workflow workflow = Workflow.None;

        public Bounds meshBounds;
        public Vector3 pivotPos;

        public float height;
        public Vector3 size;
        public Vector3 centerPostion;
        public Vector3 position;
        public Terrain terrain;

        //Private
        private MeshRenderer mesh;
        //private GameObject terrainObject = null;
        //private GameObject meshObject = null;

        private Vector4 shaderVector;

        public void GetObjectPlanarUV(GameObject go)
        {
            //Avoid unreachable code warning
            //terrainObject = null;
           // meshObject = null;

            if (go.GetComponent<Terrain>())
            {
                workflow = Workflow.Terrain;
                //terrainObject = go;
                //meshObject = null;
                GetTerrainInfo(go);
            }
            else if (go.GetComponent<MeshRenderer>())
            {
                workflow = Workflow.Mesh;
                //meshObject = go;
                //terrainObject = null;
                GetMeshInfo(go);
            }
            else
            {

                workflow = Workflow.None;
                Debug.LogError("Terrain UV Utility: Current object is neither a terrain or a mesh!");
                return;
            }
        }

        private void GetMeshInfo(GameObject meshObject)
        {
            mesh = meshObject.GetComponent<MeshRenderer>();
            meshBounds = mesh.bounds;

            //Mesh size has to be uniform
            if (!IsApproximatelyEqual(meshBounds.extents.x, meshBounds.extents.z))
            {
                Debug.LogErrorFormat(this.name + ": size of \"{0}\" is not inform at {1}!", mesh.name, meshBounds.extents.x + "x" + meshBounds.extents.z);
                return;
            }

            height = meshBounds.extents.y;
            size = meshBounds.extents * 2;
            position = meshObject.transform.position;
            centerPostion = meshBounds.center;

            //Shader vector
            float sizeX = meshBounds.extents.x * 2;
            float sizeZ = meshBounds.extents.z * 2;

            //Center minus half of the size = corner
            float posX = centerPostion.x - meshBounds.extents.x +1;
            float posZ = centerPostion.z - meshBounds.extents.z +1;

            shaderVector = new Vector4(sizeX, sizeZ, posX, posZ);

            Shader.SetGlobalVector("_TerrainUV", shaderVector);
        }

        private void GetTerrainInfo(GameObject terrainObject)
        {
            terrain = terrainObject.GetComponent<Terrain>();
            
            //Terrain size has to be uniform
            if (!IsApproximatelyEqual(terrain.terrainData.size.x, terrain.terrainData.size.z))
            {
                Debug.LogErrorFormat(this.name + ": size of \"{0}\" is not inform at {1}!", terrain.name, terrain.terrainData.size.x + "x" + terrain.terrainData.size.z);
                return;
            }

            height = terrain.terrainData.size.y;
            size = terrain.terrainData.size;
            position = terrainObject.transform.position;

            shaderVector = new Vector4(size.x, size.z, Mathf.Abs(position.x - 1), Mathf.Abs(position.z - 1));

            Shader.SetGlobalVector("_TerrainUV", shaderVector);
        }

        //Check if values are equal, has error margin for floating point precision
        private bool IsApproximatelyEqual(float a, float b)
        {
            return Mathf.Abs(a - b) < 0.02f;
        }
    }
}
