using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Base : MonoBehaviour
{
	
    private Transform _transform;
    [HideInInspector]
    public Transform transform
    {
	    get
        {
            if(_transform == null)
            {
                _transform = GetComponent<Transform>();
            }

            return _transform;
        }
    }

    void Start()
    {

    }

    void Update()
    {

    }
}
