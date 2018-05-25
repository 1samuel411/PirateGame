using PirateGame;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

namespace PirateGame
{
    public class NetworkTransformNew : NetworkingBase
    {

        [SyncVar]
        public Vector3 syncPos;
        [SyncVar]
        public float syncRot;

        public float lerpPosRate = 15;
        public float lerpRotRate = 15;

        public float sendRate = 0.1f;
        private float sendTime;

        void FixedUpdate()
        {
            if(Time.time >= sendTime)
            {
                TransmitPosition();
                TransmitRotation();
                sendTime = Time.time + sendRate;
            }
            LerpPosition();
        }

        void LerpPosition()
        {
            if (!isLocalPlayer)
            {
                transform.position = Vector3.Lerp(transform.position, syncPos, lerpPosRate * Time.deltaTime);
                transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.Euler(new Vector3(0, syncRot, 0)), lerpRotRate * Time.deltaTime);
            }
        }

        [Command]
        void CmdProvidePositionToServer(Vector3 pos)
        {
            syncPos = pos;
        }

        [ClientCallback]
        void TransmitPosition()
        {
            if (isLocalPlayer)
                CmdProvidePositionToServer(transform.position);
        }

        [Command]
        void CmdProvideRotationToServer(float rot)
        {
            syncRot = rot;
        }

        [ClientCallback]
        void TransmitRotation()
        {
            if (isLocalPlayer)
                CmdProvideRotationToServer(transform.localEulerAngles.y);
        }
    }
}