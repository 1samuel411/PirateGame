using System.Collections;
using System.Collections.Generic;
using PirateGame.Managers;
using RootMotion.FinalIK;
using UnityEngine;
using UnityEngine.Networking;

namespace PirateGame.Networking
{
    public class PlayablePlayer : Base
    {

        public MonoBehaviour[] localOnlyComponents;

        public Character.Character character;

        void Start()
        {
            StartCoroutine(WaitFrame());
        }

        IEnumerator WaitFrame()
        {
            yield return null;
            yield return null;
            // Set up UMA Character
            SetUpUMA();
        }

        void SetUpUMA()
        {
            Debug.Log("Setting up UMA");
            Entity.EntityPlayer entityPlayer = GetComponent<Entity.EntityPlayer>();
            GrounderIK grounderIk = GetComponentInChildren<GrounderIK>();

            // Apply char settings
            character.SetCharacter(PlayerManager.instance.user.character);

            // Add char IK

            // Add Left leg IK
            LimbIK leftLimb = AddLimbIK("LeftUpLeg", "LeftLeg", "LeftFoot");
            leftLimb.solver.goal = AvatarIKGoal.LeftFoot;
            // Add Right leg IK
            LimbIK rightLimb = AddLimbIK("RightUpLeg", "RightLeg", "RightFoot");
            leftLimb.solver.goal = AvatarIKGoal.RightFoot;
            // Set up grounder
            grounderIk.legs = new LimbIK[] { leftLimb, rightLimb };
            grounderIk.pelvis = GetCharacterChild("Hips");
            grounderIk.characterRoot = GetCharacterChild("Position");

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

            // Set IK
            entityPlayer.leftArmIk = leftArmLimb;
            entityPlayer.rightArmIk = rightArmLimb;

            GetComponent<Entity.Animations.AnimateHumanoid>().animator = character.GetComponent<Animator>();
        }

        void Update()
        {
            for (int i = 0; i < localOnlyComponents.Length; i++)
            {
                localOnlyComponents[i].enabled = ServerManager.instance.myNetworkPlayer.isLocalPlayer;
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