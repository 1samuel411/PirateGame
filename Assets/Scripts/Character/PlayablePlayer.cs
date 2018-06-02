using System.Collections;
using System.Collections.Generic;
using PirateGame.Managers;
using RootMotion.FinalIK;
using UnityEngine;
using UnityEngine.Networking;
using PirateGame.Entity;
using PirateGame.Entity.Animations;
using UMA;

namespace PirateGame.Networking
{
    public class PlayablePlayer : NetworkingBase
    {

        public MonoBehaviour[] localOnlyComponents;

        public Character.Character character;

        public LayerMask groundedLayerMask;

        private NetworkedPlayer networkedPlayer;

        public Transform rightHandWeaponHolder;

        public Transform head;
        public Transform armLeft;
        public Transform armRight;

        private Entity.EntityPlayer entityPlayer;

        void OnEnable()
        {
            networkedPlayer = GetComponentInParent<NetworkedPlayer>();
            entityPlayer = GetComponent<Entity.EntityPlayer>();
            StartCoroutine(SetUpUMA());
        }

        public void SetUpUMACC(UMAData umaData)
        {
            // Add Left arm IK
            LimbIK leftArmLimb = AddLimbIK("LeftArm", "LeftForeArm", "LeftHand");
            leftArmLimb.solver.goal = AvatarIKGoal.LeftHand;
            leftArmLimb.solver.IKRotationWeight = 0;
            leftArmLimb.solver.maintainRotationWeight = 0;
            leftArmLimb.solver.target = entityPlayer.leftTarget;

            // Add Right arm IK
            LimbIK rightArmLimb = AddLimbIK("RightArm", "RightForeArm", "RightHand");
            rightArmLimb.solver.goal = AvatarIKGoal.RightHand;
            rightArmLimb.solver.IKRotationWeight = 0;
            rightArmLimb.solver.maintainRotationWeight = 0;
            rightArmLimb.solver.target = entityPlayer.rightTarget;

            head = GetCharacterChild("Head");
            armLeft = GetCharacterChild("LeftArm");
            armRight = GetCharacterChild("RightArm");

            GameObject rightHandWeaponBone = new GameObject();
            rightHandWeaponBone.name = "RightHandBone";
            rightHandWeaponBone.transform.SetParent(FindChildRecursive(character.transform, "RightHand"));
            rightHandWeaponBone.transform.localPosition = new Vector3(-0.1225f, -0.008f, -0.028f);
            rightHandWeaponBone.transform.localEulerAngles = new Vector3(3.7f, -1.503f, 2.008f);
            rightHandWeaponBone.transform.localScale = new Vector3(2, 2, 2);
            rightHandWeaponHolder = rightHandWeaponBone.transform;

            // Set IK
            Transform[] spine = new Transform[] { GetCharacterChild("LowerBack"), GetCharacterChild("Spine"), GetCharacterChild("Spine1") };
            LookAtIK lookatIk = gameObject.AddComponent<LookAtIK>();
            lookatIk.solver.SetChain(spine, head, null, GetCharacterChild("Position"));
            lookatIk.solver.target = entityPlayer.fakeCameraForward;
            entityPlayer.leftArmIk = leftArmLimb;
            entityPlayer.rightArmIk = rightArmLimb;

            GetComponent<Entity.Animations.AnimateHumanoid>().animator = character.GetComponent<Animator>();

            // Add char IK
            LimbIKGrounderUMA2 grounder = gameObject.AddComponent<LimbIKGrounderUMA2>();
            grounder.animator = character.GetComponent<Animator>();
            grounder.layers = groundedLayerMask;
        }

        IEnumerator SetUpUMA()
        {
            // Apply char settings
            while(ServerManager.instance.networkUsers.ContainsKey(networkedPlayer.networkId) == false || ServerManager.instance.networkUsers[networkedPlayer.networkId].userData.character == null)
            {
                yield return null;
            }
            yield return null;
            Debug.Log("Setting up UMA");
            
            character.SetCharacter(ServerManager.instance.networkUsers[networkedPlayer.networkId].userData.character);
        }

        void Update()
        {
            for (int i = 0; i < localOnlyComponents.Length; i++)
            {
                localOnlyComponents[i].enabled = ServerManager.instance.myNetworkPlayer.isLocalPlayer;
            }
        }

        private void LateUpdate()
        {
            if (head)
            {
                //head.transform.eulerAngles = new Vector3(CameraManager.instance.cameraObject.transform.localEulerAngles.x, 0, 0);
                //armLeft.transform.eulerAngles = new Vector3(CameraManager.instance.cameraObject.transform.localEulerAngles.x, armLeft.transform.eulerAngles.y, armLeft.transform.eulerAngles.z);
                //armRight.transform.eulerAngles = new Vector3(CameraManager.instance.cameraObject.transform.localEulerAngles.x, armRight.transform.eulerAngles.y, armRight.transform.eulerAngles.z);
            }
        }

        LimbIK AddLimbIK(string bone1, string bone2, string bone3)
        {
            LimbIK limbIk = null;
            Transform bone1Limb = GetCharacterChild(bone1);
            limbIk = bone1Limb.gameObject.AddComponent<LimbIK>();
            limbIk.solver.bone1.transform = bone1Limb;

            Transform bone2Limb = GetCharacterChild(bone2);
            limbIk.solver.bone2.transform = bone2Limb;

            Transform bone3Limb = GetCharacterChild(bone3);
            limbIk.solver.bone3.transform = bone3Limb;

            return limbIk;
        }

        Transform GetCharacterChild(string name)
        {
            return FindChildRecursive(character.transform, name);
        }

        public static Transform FindChildRecursive(Transform trm, string name)
        {

            Transform child = null;

            // Loop through top level
            foreach (Transform t in trm)
            {
                if (t.name == name)
                {
                    child = t;
                    return child;
                }
                else if (t.childCount > 0)
                {
                    child = FindChildRecursive(t, name);
                    if (child)
                    {
                        return child;
                    }
                }
            }

            return child;
        }

    }
}