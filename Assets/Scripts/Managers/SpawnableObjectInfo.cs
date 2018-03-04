using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnableObjectInfo : MonoBehaviour
{

    public bool ready = false;

    void OnEnable()
    {
        ready = false;
    }

    void OnDisable()
    {
        ready = true;
    }
}
