using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IEffect
{
    float GetAutoDestructTime();

    void CheckAutoDestruct();

    void SetAutoDestructTime();

    GameObject GetGameObject();
}
