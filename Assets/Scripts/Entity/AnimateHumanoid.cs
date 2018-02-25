using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PirateGame.Entity.Animations
{
    [RequireComponent(typeof(EntityHumanoid))]
    public class AnimateHumanoid : Base
    {

        public string velocityXParameterName = "velocityX";
        public string velocityYParameterName = "velocityY";

        public new Animator animator;

        private EntityHumanoid humanoid;

        void Awake()
        {
            humanoid = GetComponent<EntityHumanoid>();

            humanoid.UnGroundAction += UnGround;
            humanoid.LandAction += Land;
            humanoid.SprintEndAction += SprintStop;
            humanoid.StateChangeAction += StateChange;
            humanoid.InteractBeginSequenceAction += InteractBeginSequence;
            humanoid.InteractStopSequenceAction += InteractStopSequence;
        }

        void Update()
        {
            animator.SetFloat(velocityXParameterName, humanoid.velocityVectorDirectionInverse.x);
            animator.SetFloat(velocityYParameterName, humanoid.velocityVectorDirectionInverse.z);
        }

        private void InteractBeginSequence()
        {
            if (string.IsNullOrEmpty(humanoid.currentInteractable.GetInteractAnimation()))
            {
                humanoid.InteractBeginInteractable();
                return;
            }
            animator.CrossFadeInFixedTime(humanoid.currentInteractable.GetInteractAnimation(), 0.1f);
        }

        private void InteractStopSequence()
        {
            if (string.IsNullOrEmpty(humanoid.currentInteractable.GetUnInteractAnimation()))
            {
                humanoid.InteractStopInteractable();
                return;
            }
            animator.CrossFadeInFixedTime(humanoid.currentInteractable.GetUnInteractAnimation(), 0.1f);
        }

        private float timeSinceSprinting;
        void SprintStop()
        {
            timeSinceSprinting = Time.time;
        }

        private void StateChange(EntityEnums.HumanoidState state)
        {
            if (humanoid.interactingBegin)
                return;

            if (state == EntityEnums.HumanoidState.Walking)
            {
                animator.CrossFadeInFixedTime(humanoid.aiming ? "Walk" : "WalkStart", 0.2f);
            }
            if (state == EntityEnums.HumanoidState.Sprinting)
            {
                animator.CrossFadeInFixedTime("SprintStart", 0.3f);
            }
            if (state == EntityEnums.HumanoidState.Idle)
            {
                if (Time.time < timeSinceSprinting + 0.8f)
                    animator.CrossFadeInFixedTime("SprintStop", 0.2f);
                else
                    animator.CrossFadeInFixedTime("WalkStop", 0.2f);
            }
        }

        private void UnGround(bool jumping)
        {
            if (humanoid.interactingBegin)
                return;

            if (jumping)
            {
                if (humanoid.velocityPlanarMagnitude > 0.5f)
                {
                    if (humanoid.sprinting)
                        animator.CrossFadeInFixedTime("JumpSprint", 0.2f);
                    else
                        animator.CrossFadeInFixedTime("JumpWalk", 0.2f);
                }
                else
                    animator.CrossFadeInFixedTime("JumpIdle", 0.2f);
            }
            else
            {
                animator.CrossFadeInFixedTime("Falling", 0.2f);
            }
        }

        private void Land(bool jumping)
        {
            if (humanoid.interactingBegin)
                return;

            if (humanoid.velocityPlanarMagnitude > 2f)
            {
                if (humanoid.sprinting)
                    animator.CrossFadeInFixedTime("LandSprint", 0.2f);
                else
                    animator.CrossFadeInFixedTime("LandWalk", 0.2f);
            }
            else
                animator.CrossFadeInFixedTime("LandIdle", 0.2f);
        }
    }
}