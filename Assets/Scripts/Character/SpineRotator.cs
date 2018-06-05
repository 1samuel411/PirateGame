using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PirateGame.Character
{
    public class SpineRotator : Base
    {

        public float offset;

        public void SetOffset(float offset)
        {
            this.offset = offset;
        }

        void LateUpdate()
        {
            transform.localEulerAngles = new Vector3(offset, transform.localEulerAngles.y, transform.localEulerAngles.z);
        }
    }
}