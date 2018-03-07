using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PirateGame.Effects
{
    public interface IEffect
    {
        float GetAutoDestructTime();

        void CheckAutoDestruct();

        void SetAutoDestructTime();

        GameObject GetGameObject();
    }
}