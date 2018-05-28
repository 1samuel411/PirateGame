using RootMotion.FinalIK;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LimbIKGrounderUMA2 : MonoBehaviour
{

    public Animator animator;
    public LayerMask layers;

    void Update()
    {
        if (animator != null)
        {
            LimbIK leftIK = gameObject.AddComponent<LimbIK>();
            leftIK.solver.SetChain(
                animator.GetBoneTransform(HumanBodyBones.LeftUpperLeg),
                animator.GetBoneTransform(HumanBodyBones.LeftLowerLeg),
                animator.GetBoneTransform(HumanBodyBones.LeftFoot),
                animator.transform);

            leftIK.solver.goal = AvatarIKGoal.LeftFoot;

            LimbIK rightIK = gameObject.AddComponent<LimbIK>();
            rightIK.solver.SetChain(
                animator.GetBoneTransform(HumanBodyBones.RightUpperLeg),
                animator.GetBoneTransform(HumanBodyBones.RightLowerLeg),
                animator.GetBoneTransform(HumanBodyBones.RightFoot),
                animator.transform);

            rightIK.solver.goal = AvatarIKGoal.RightFoot;

            GrounderIK grounder = gameObject.AddComponent<GrounderIK>();
            grounder.legs = new IK[2] { leftIK as IK, rightIK as IK };
            grounder.pelvis = animator.GetBoneTransform(HumanBodyBones.Hips);
            grounder.characterRoot = animator.transform;
            grounder.solver.layers = layers;

            grounder.solver.heightOffset = -0.1f;
            grounder.solver.maxStep = 0.75f;
            grounder.solver.footRadius = 0.25f;
            grounder.solver.liftPelvisWeight = 0;

            transform.parent = animator.transform;

            Destroy(this);
        }
    }
}