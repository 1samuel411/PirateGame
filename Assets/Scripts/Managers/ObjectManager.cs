using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ObjectManager : MonoBehaviour
{

    public static ObjectManager instance;

    public List<SpawnableObject> spawnableObjects = new List<SpawnableObject>();
    
    void Awake()
    {
        instance = this;

        Initialize();
    }

    void Initialize()
    {
        for (int i = 0; i < spawnableObjects.Count; i++)
        {
            for (int x = 0; x < spawnableObjects[i].initAmountToSpawn; x++)
            {
                spawnableObjects[i].CreateObject();
            }
        }
    }

    public SpawnableObjectInfo Instantiate(string name, Vector3 position)
    {
        SpawnableObjectInfo obj = Instantiate(name);
        if (obj)
        {
            obj.transform.position = position;
            return obj;
        }
        return null;
    }

    public SpawnableObjectInfo Instantiate(string name, Vector3 position, Quaternion rotation)
    {
        SpawnableObjectInfo objInfo = Instantiate(name, position);
        if (objInfo)
        {
            objInfo.transform.rotation = rotation;
            return objInfo;
        }
        return null;
    }

    public SpawnableObjectInfo Instantiate(string name)
    {
        SpawnableObjectInfo objInfo = spawnableObjects.FirstOrDefault(x => x.name == name).GetAvailableObject();
        if (objInfo)
        {
            objInfo.gameObject.SetActive(true);
        }
        return objInfo;
    }

    public SpawnableObjectInfo Instantiate(GameObject gameObject, Vector3 position)
    {
        SpawnableObjectInfo obj = Instantiate(gameObject);
        if (obj)
        {
            obj.transform.position = position;
            return obj;
        }
        return null;
    }

    public SpawnableObjectInfo Instantiate(GameObject gameObject, Vector3 position, Quaternion rotation)
    {
        SpawnableObjectInfo obj = Instantiate(gameObject, position);
        if (obj)
        {
            obj.transform.rotation = rotation;
            return obj;
        }
        return null;
    }

    public SpawnableObjectInfo Instantiate(GameObject gameObject)
    {
        SpawnableObject obj = spawnableObjects.FirstOrDefault(x => x.gameObject == gameObject);
        SpawnableObjectInfo objInfo = null;
        if (obj != null)
        {
            objInfo = obj.GetAvailableObject();
            objInfo.gameObject.SetActive(true);
            return objInfo;
        }

        // make a new one
        obj = new SpawnableObject();
        obj.name = gameObject.name;
        obj.gameObject = gameObject;
        obj.initAmountToSpawn = 0;
        spawnableObjects.Add(obj);
        objInfo = obj.GetAvailableObject();
        objInfo.gameObject.SetActive(true);
        return objInfo;
    }
}

[System.Serializable]
public class SpawnableObject
{
    public string name;

    public GameObject gameObject;

    public int initAmountToSpawn;

    public List<SpawnableObjectInfo> spawnedGameObjects = new List<SpawnableObjectInfo>();

    public SpawnableObjectInfo GetAvailableObject()
    {
        for (int i = 0; i < spawnedGameObjects.Count; i++)
        {
            if (spawnedGameObjects[i].ready)
            {
                return spawnedGameObjects[i];
            }
        }

        return CreateObject();
    }

    public SpawnableObjectInfo CreateObject()
    {
        // make a new one
        GameObject spawnedObject = GameObject.Instantiate(gameObject, Vector3.zero, Quaternion.identity);
        SpawnableObjectInfo spawnedObjectInfo = spawnedObject.AddComponent<SpawnableObjectInfo>();
        spawnedGameObjects.Add(spawnedObjectInfo);
        spawnedObject.SetActive(false);
        return spawnedObjectInfo;
    }
}
